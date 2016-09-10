using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hybrasyl.Enums;
using Newtonsoft.Json;


namespace Hybrasyl.Objects
{
    [JsonObject]
    public class DeathPileInfo
    {
        private string Owner { get; set; }
        private HashSet<string> AllowedLooters { get; set; }
        private DateTime PileTime { get; set; }
        private double PileTimeElapsed => (DateTime.Now - PileTime).TotalSeconds;

        public DeathPileInfo(string owner)
        {
            Owner = owner;
            AllowedLooters = new HashSet<string>();
            PileTime = DateTime.Now;
        }

        public DeathPileInfo(UserGroup group)
        {
            Owner = string.Empty;
            AllowedLooters = new HashSet<string>(group?.Members.Select(n => n.Name) ?? new List<string>());
        }

        public DeathPileInfo(string owner, UserGroup group)
        {
            Owner = owner;
            AllowedLooters = new HashSet<string>(group?.Members.Select(n => n.Name) ?? new List<string>());
        }

        public bool CanLoot(IVisibleObject target)
        {
            if (Owner == target.Name) return true;         
            if (Owner != string.Empty && AllowedLooters.Contains(target.Name))
                return true;
            if (Owner != target.Name && AllowedLooters.Contains(target.Name) && PileTimeElapsed > 1800)
                return true;
            return Owner != target.Name && PileTimeElapsed > 3600;
        }

    }

    [JsonObject]
    public class GuildMembershipInfo
    {
        public String Title { get; set; }
        public String Name { get; set; }
        public String Rank { get; set; }
    }

    [JsonObject]
    public class LocationInfo
    {
        public ushort MapId { get; set; }
        public Direction Direction { get; set; }
        public byte X { get; set; }
        public byte Y { get; set; }
        public bool WorldMap { get; set; }
    }

    [JsonObject]
    public class PasswordInfo
    {
        public String Hash { get; set; }
        public DateTime LastChanged { get; set; }
        public String LastChangedFrom { get; set; }
    }

    [JsonObject]
    public class LoginInfo
    {
        public DateTime LastLogin { get; set; }
        public DateTime LastLogoff { get; set; }
        public DateTime LastLoginFailure { get; set; }
        public String LastLoginFrom { get; set; }
        public Int64 LoginFailureCount { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
