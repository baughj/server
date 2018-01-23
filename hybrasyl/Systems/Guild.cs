using Hybrasyl.Enums;
using Hybrasyl.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hybrasyl.Systems
{

    public interface IOrgRank
    {
        string RankName { get; set; }
        string Description { get; set; }
        OrgAction AllowedActions { get; set; }           
    }

    [JsonObject]
    public class OrgRank : IOrgRank
    {
        public string RankName { get; set; }
        public string Description { get; set; }
        public OrgAction AllowedActions { get; set; }
        
        public OrgRank(string rank, string desc, OrgAction allowed = OrgAction.None)
        {
            RankName = rank;
            Description = desc;
            AllowedActions = allowed;
        }
    }

    public interface IOrgActivityLog
    {
        string Target { get; }
        string Source { get; }
        DateTime Timestamp { get; }
        string Note { get; }
        OrgAction ActionType { get; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class OrgActivityLog : IOrgActivityLog
    {
        [JsonProperty]
        public string Target { get; }
        [JsonProperty]
        public string Source { get; }
        [JsonProperty]
        public DateTime Timestamp { get; }
        [JsonProperty]
        public OrgAction ActionType { get; }
        [JsonProperty]
        private List<string> _notes { get; set; }

        public string Note
        {
            get
            {
                if (_notes.Count > 0)
                    return _notes.Last();
                else return string.Empty;
            }
        }

        public OrgActivityLog(string target, string source, OrgAction actiontype, DateTime? timestamp = null, string notes = "")
        {
            Target = target;
            Source = source;
            ActionType = actiontype;
            if (Timestamp == null)
                Timestamp = DateTime.Now;
            else
                Timestamp = (DateTime) timestamp;
            _notes = new List<string>();
            if (!string.IsNullOrEmpty(notes))
                _notes.Add(notes);
        }
    }

    public interface IOrgMemberInfo
    {
        IOrgRank Rank { get; }
        DateTime Joined { get; }
        DateTime? Left { get; }
        bool CurrentMember { get; }
        List<IOrgActivityLog> ActivityLog { get; }
        List<IOrgActivityLog> ChangeLog { get; }
        void RecordActivity(IOrgActivityLog log);
        void RecordChange(IOrgActivityLog log);
    }
   
    public interface IOrg
    {
        OrgType Type { get; }
        string Name { get; set; }
        string Motto { get; set; }
        bool ChatEnabled { get; set; }
        bool BoardEnabled { get; set; }
        string ChatTarget { get; set; }
        byte ChatColor { get; set; }
        string LeaderName { get; set; }
        ConcurrentDictionary<string, IOrgMemberInfo> Members { get; set; }
        int NationId { get; set; }
        int HeadquartersId { get; set; }
        int MemberCount { get; }
        int OfficerCount { get; }
        Dictionary<string, IOrgRank> Ranks { get; }

        void InviteMember(User user);
        bool AddMember(User user);
        bool RemoveMember(User user, string reason);
        bool PromoteMember(User user, IOrgRank rank);
        bool AddRank(IOrgRank rank);
        bool RemoveRank(IOrgRank rank);
        void MuteMember(User user);
        void SetMotd(string text);
        void SendMessage(string text);
    }

    //public abstract class Org  : IEnumerable<User>, IOrg
    //{

    //    public abstract OrgType type { get; }
    //}

    //class Guild : Org
    //{
    //    public OrgType Type => OrgType.Guild;

    //    private HashSet<User> _members { get; set; }

    //}
}
