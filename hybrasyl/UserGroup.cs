/*
 * This file is part of Project Hybrasyl.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the Affero General Public License as published by
 * the Free Software Foundation, version 3.
 *
 * This program is distributed in the hope that it will be useful, but
 * without ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
 * or FITNESS FOR A PARTICULAR PURPOSE. See the Affero General Public License
 * for more details.
 *
 * You should have received a copy of the Affero General Public License along
 * with this program. If not, see <http://www.gnu.org/licenses/>.
 *
 * (C) 2020 ERISCO, LLC 
 *
 * For contributors and individual authors please refer to CONTRIBUTORS.MD.
 * 
 */

 using Hybrasyl.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hybrasyl;

/**
     * This class defines a group of users. Grouped users can whisper to the full group
     * and will also split experience with nearby group members.
     */
public class UserGroup
{

    private List<User> _expShareInRange = new List<User>();
 
    // Group-related info
    public User Founder { get; private set; }
    public List<User> Members { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public Dictionary<Xml.Class, uint> ClassCount { get; private set; }
    public uint MaxMembers;
    public GroupRecruit RecruitInfo => Founder.GroupRecruit;

    private delegate Dictionary<uint, uint> DistributionFunc(User source, uint full);

    private DistributionFunc ExperienceDistributionFunc;

    private object _lock = new object();

    public byte AverageLevel => (byte) Math.Ceiling(Members.Average(x => x.Stats.Level));

    public UserGroup(User founder)
    {
        Founder = founder;
        Members = new List<User>();
        ClassCount = new Dictionary<Xml.Class, uint>();
        foreach (var cl in Enum.GetValues(typeof(Xml.Class)).Cast<Xml.Class>())
        {
            ClassCount[cl] = 0;
        }

        GameLog.InfoFormat("Creating new group with {0} as founder.", founder.Name);
        // Distribute full experience to everyone with a bonus if a member of each
        // class is present.
        ExperienceDistributionFunc = Distribution_AllClassBonus;

        Add(founder);
        CreatedOn = DateTime.Now;
    }

    // Group-related functions and accessors
    public bool Add(User user)
    {
        // You can only join a group if you're not already a member in another group.
        // Check this condition and notify the invitee and existing group members if
        // there's an issue.
        if (user.Grouped)
        {
            user.SendMessage("You're already in a group.", MessageTypes.SYSTEM);

            foreach (var member in Members)
            {
                member.SendMessage(string.Format("{0} is in another group.", user.Name), MessageTypes.SYSTEM);
            }

            // If this fails when the group only contains one other person, the group should be abandoned.
            if (Count == 1)
            {
                Remove(Members[0]);
            }

            return false;
        }

        // Send a message to notify everyone else that someone's joined.
        foreach (var member in Members)
        {
            member.SendMessage(user.Name + " has joined your group.", MessageTypes.SYSTEM);
        }

        lock (_lock)
        {
            Members.Add(user);
            user.Group = this;
            ClassCount[user.Class]++;
            MaxMembers = (uint)Math.Max(MaxMembers, Members.Count);
        }

        if (user != Founder && user.GroupRecruit != null)
        {
            user.GroupRecruit = null;
            user.Show();
        }

        // Send a distinct message to the new user.
        user.SendMessage("You've joined a group.", MessageTypes.SYSTEM);
	user.SendProfile();
        return true;
    }

    // TODO: refactor to use hashset
    public bool Contains(User user) => Members.Where(e => e.Name == user.Name).Count() > 0;

    public void Remove(User user)
    {
        lock (_lock)
        {
            Members.Remove(user);
            user.Group = null;
            ClassCount[user.Class]--;
        }
        // If this has ever been a true group from a user's perspective, talk about it. Otherwise
        // don't send user-facing messages.
        if (MaxMembers > 1)
        {
            // User has already been removed from Members so no need to exclude.
            foreach (var member in Members)
            {
                member.SendMessage(user.Name + " has left your group.", MessageTypes.SYSTEM);
            }
            user.SendMessage("You've left a group.", MessageTypes.SYSTEM);
	    user.SendProfile();
        }

        // If there's only one person left, disband the group.
        if (Count == 1)
        {
            Remove(Members[0]);
        }
    }

    public int Count
    {
        get { return Members.Count; }
    }

    public bool ContainsAllClasses()
    {
        return (ClassCount[Xml.Class.Monk] > 0 &&
                ClassCount[Xml.Class.Priest] > 0 &&
                ClassCount[Xml.Class.Rogue] > 0 &&
                ClassCount[Xml.Class.Warrior] > 0 &&
                ClassCount[Xml.Class.Wizard] > 0);
    }

    /**
         * Find out whether a given user is close enough to the action in order
         * to receive a share. It's not OK to be (a) on a different map or
         * (b) really far away on the same map.
         */

    private List<User> MembersWithinRange(User user)
    {
        var inRange = user.Map.EntityTree.GetObjects(user.GetViewport()).OfType<User>();

        return inRange.Intersect(Members).ToList();
    }

    private uint ScaleExperience(uint exp, byte mobLevel)
    {
        var difference = AverageLevel - mobLevel;
        switch (difference)
        {
            case > 5:
                exp = 1;
                break;
            case 5:
                exp = (uint)Math.Ceiling(exp * 0.40);
                break;
            case 4:
                exp = (uint)Math.Ceiling(exp * 0.80);
                break;
            case -6:
                exp = (uint)Math.Ceiling(exp * 1.15);
                break;
            case -5:
                exp = (uint)Math.Ceiling(exp * 1.10);
                break;
            case -4:
                exp = (uint)Math.Ceiling(exp * 1.05);
                break;
            case < -7:
                exp = (uint)Math.Ceiling(exp * 1.20);
                break;
            default:
                break;
        }
        return exp;
    }

    /**
         * Distribute a pool of experience across members of the group.
         */

    public void ShareExperience(User source, uint exp, byte mobLevel)
    {
        Dictionary<uint, uint> share = ExperienceDistributionFunc(source, exp);
        var inRange = MembersWithinRange(source);
        var difference = AverageLevel - mobLevel;
        for (int i = 0; i < inRange.Count; i++)
        {
            // Note: this will only work for positive numbers at this point.
            inRange[i].GiveExperience(ScaleExperience(share[inRange[i].Id],  mobLevel));
        }
    }

    /**
         * Distribution functions. Should be assigned to ExperienceDistributionFunc or a similar
         * function to be put to use. See the constructor of UserGroup for an example. Note that
         * these are intended to be generic and can apply to any numeric quantity (experience,
         * gold, even #'s of items).
         */

    /**
     * This distribution function gives the full quantity of a resource to each of the
     * group members.
     */

    private Dictionary<uint, uint> Distribution_Full(User source, uint full)
    {
        Dictionary<uint, uint> share = new Dictionary<uint, uint>();

        foreach (var member in MembersWithinRange(source))
        {
            share[member.Id] = full;
        }

        return share;
    }

    /**
         * Give the full quantity of a resource to each of the group members, +10% bonus
         * if there's at least one representative from each class.
         */

    private Dictionary<uint, uint> Distribution_AllClassBonus(User source, uint full)
    {
        // Check to see if at least one representative from each class is in the group.
        if (!ContainsAllClasses())
        {
            var inRange = MembersWithinRange(source).Count - 1; // will always be 1 when source is in range. set back to 0 to not penalize solo while grouped.
            if (inRange > 5) inRange = 5; //limit to max 45% decrease

            full = (uint)(full * (( 100 - (inRange * 7.5)) / 100));
        }

        return Distribution_Full(source, full);
    }

    /// <summary>
    /// Send a system message as a group message, that the entire group can see.
    /// </summary>
    /// <param name="message"></param>
    public void SendMessage(string message)
    {
        foreach (var member in Members)
            member.SendMessage($"[Notice] {message}", MessageTypes.GROUP);
    }
}

public class GroupRecruit
{
    private int[] _wanted = new int[5];

    public GroupRecruit(User user)
    {
        Recruiter = user;
    }

    public User Recruiter { get; }

    public UserGroup Group => Recruiter.Group;

    public string Name { get; set; }

    public string Note { get; set; }

    public int StartingLevelRange { get; set; }

    public int EndingLevelRange { get; set; }

    public int WarriorsWanted => _wanted[0];

    public int WizardsWanted => _wanted[1];

    public int RoguesWanted => _wanted[2];

    public int PriestsWanted => _wanted[3];

    public int MonksWanted => _wanted[4];

    public int TotalWanted => _wanted.Sum();

    public int Wanted(Xml.Class cl)
        => cl switch
        {
            Xml.Class.Peasant => 0,
            Xml.Class.Rogue => RoguesWanted,
            Xml.Class.Wizard => WizardsWanted,
            _ => _wanted[(int)cl - 1]
        };

    public static GroupRecruit Read(ClientPacket packet, User recruiter)
    {
        return new GroupRecruit(recruiter)
        {
            Name = packet.ReadString8(),
            Note = packet.ReadString8(),
            StartingLevelRange = Math.Max((int)packet.ReadByte(), 1),
            EndingLevelRange = Math.Clamp((int)packet.ReadByte(), 1, 99),
            _wanted = new int[]
            {
                Math.Min((int)packet.ReadByte(), 13),
                Math.Min((int)packet.ReadByte(), 13),
                Math.Min((int)packet.ReadByte(), 13),
                Math.Min((int)packet.ReadByte(), 13),
                Math.Min((int)packet.ReadByte(), 13),
            },
        };
    }

    public void ShowTo(User user)
    {
        var groupPacket = new ServerPacket(0x63);
        groupPacket.WriteByte((byte)GroupServerPacketType.RecruitInfo);
        WriteInfo(groupPacket);
        user.Enqueue(groupPacket);
    }

    public void WriteInfo(ServerPacket packet)
    {
        packet.WriteString8(Recruiter.Name);
        packet.WriteString8(Name);
        packet.WriteString8(Note);
        packet.WriteByte((byte)StartingLevelRange);
        packet.WriteByte((byte)EndingLevelRange);
        packet.WriteByte((byte)WarriorsWanted);
        packet.WriteByte((byte)(Group?.ClassCount[Xml.Class.Warrior] ?? 0));
        packet.WriteByte((byte)WizardsWanted);
        packet.WriteByte((byte)(Group?.ClassCount[Xml.Class.Wizard] ?? 0));
        packet.WriteByte((byte)RoguesWanted);
        packet.WriteByte((byte)(Group?.ClassCount[Xml.Class.Rogue] ?? 0));
        packet.WriteByte((byte)PriestsWanted);
        packet.WriteByte((byte)(Group?.ClassCount[Xml.Class.Priest] ?? 0));
        packet.WriteByte((byte)MonksWanted);
        packet.WriteByte((byte)(Group?.ClassCount[Xml.Class.Monk] ?? 0));
    }

    public bool InviteToGroup(User invitee)
    {
        if (invitee.Stats.Level < StartingLevelRange)
        {
            invitee.SendSystemMessage("Your level is too low to join that group.");
            return false;
        }

        if (invitee.Stats.Level > EndingLevelRange)
        {
            invitee.SendSystemMessage("Your level is too high to join that group.");
            return false;
        }

        if ((Group?.ClassCount[invitee.Class] ?? 0) >= Wanted(invitee.Class))
        {
            invitee.SendSystemMessage($"No more {invitee.Class}s are needed.");
            return false;
        }

        return Recruiter.InviteToGroup(invitee);
    }
}

public enum GroupClientPacketType
{
    Request = 2,
    Answer = 3,
    RecruitInit = 4,
    RecruitInfo = 5,
    RecruitEnd = 6,
    RecruitAsk = 7,
}

public enum GroupServerPacketType
{
    Ask = 1,
    Member = 2,
    RecruitInfo = 4,
    RecruitAsk = 5,

}