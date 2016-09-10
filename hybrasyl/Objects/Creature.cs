using System;
using System.Collections.Concurrent;
using System.Linq;
using Hybrasyl.Enums;
using Hybrasyl.XSD;
using log4net;
using log4net.Core;
using Newtonsoft.Json;

namespace Hybrasyl.Objects
{
    public interface ICreature : IVisibleObject
    {
        uint Experience { get; set; }
        uint AbilityExp { get; set; }
        sbyte Ac { get; set; }
        uint Hp { get; set; }
        long BaseHp { get; set; }
        long BonusHp { get; set; }
        long BonusAc { get; set; }
        long BonusMr { get; set; }
        long BonusRegen { get; set; }
        uint Gold { get; set; }
        Inventory Inventory { get; }

        Enums.Element OffensiveElement { get; set; }
        Enums.Element DefensiveElement { get; set; }
        int ActiveStatusCount { get; }
        bool AbsoluteImmortal { get; set; }
        bool PhysicalImmortal { get; set; }
        bool MagicalImmortal { get; set; }

        void Damage(double damage, Enums.Element element = Enums.Element.None,
            Enums.DamageType damageType = Enums.DamageType.Direct,
            ICreature attacker = null);
        void OnDeath();

        void ToggleBlind();
        void ToggleParalyze();
        void ToggleFreeze();
        void ToggleSleep();
        void ToggleComa();
        void ToggleAlive();

        bool IsBlinded { get; }
        bool IsParalyzed { get; }
        bool IsInComa { get; }
        bool IsFrozen { get; }
        bool IsAlive { get; }
        bool IsAsleep { get; }

        void SendAnimation(ServerPacket packet);
        bool Walk(Direction direction);
        bool Turn(Direction direction);
        void Motion(byte motion, short speed);
        void Heal(double heal, ICreature healer = null);

        void Attack(Direction direction, Castable castObject, ICreature target);
        void Attack(Castable castObject, ICreature target);
        void Attack(Castable castObject);

    }

    public abstract class Creature : VisibleObject, ICreature
    {
        public new static readonly ILog Logger =
               LogManager.GetLogger(
               System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [JsonProperty]
        public StatusFlags Status { get; set; }

        [JsonProperty]
        public uint Experience { get; set; }
        [JsonProperty]
        public uint AbilityExp { get; set; }

        [JsonProperty]
        public uint Hp { get; set; }

        [JsonProperty]
        public long BaseHp { get; set; }
        public long BonusHp { get; set; }

        [JsonProperty]
        public uint Mp { get; set; }
        [JsonProperty]
        public long BaseMp { get; set; }
        public long BonusMp { get; set; }

        public uint MaximumMp
        {
            get
            {
                var value = BaseMp + BonusMp;

                if (value > uint.MaxValue)
                    return uint.MaxValue;

                if (value < uint.MinValue)
                    return uint.MinValue;

                return (uint)BindToRange(value, StatLimitConstants.MIN_BASE_HPMP, StatLimitConstants.MAX_BASE_HPMP);
            }
        }
        public sbyte Ac { get; set; }
        public long BonusAc { get; set; }
        public long BonusMr { get; set; }
        public long BonusRegen { get; set; }

        [JsonProperty]
        protected ConcurrentDictionary<ushort, IPlayerStatus> CurrentStatuses;

        public Enums.Element OffensiveElement { get; set; }
        public Enums.Element DefensiveElement { get; set; }

        [JsonProperty]
        public uint Gold { get; set; }

        [JsonProperty]
        public Inventory Inventory { get; protected set; }

        [JsonProperty("Equipment")]
        public Inventory Equipment { get; protected set; }
        public int ActiveStatusCount => CurrentStatuses.Count;

        protected Creature()
        {
            Gold = 0;
            Inventory = new Inventory(59);
            Equipment = new Inventory(18);
        }

        #region Status toggles / Is checks

        /// <summary>
        /// Toggle whether or not the user is frozen.
        /// </summary>
        public void ToggleFreeze()
        {
            Status ^= StatusFlags.Frozen;
        }

        /// <summary>
        /// Toggle whether or not the user is asleep.
        /// </summary>
        public void ToggleSleep()
        {
            Status ^= StatusFlags.Asleep;
        }

        /// <summary>
        /// Toggle whether or not the user is blind.
        /// </summary>
        public void ToggleBlind()
        {
            Status ^= StatusFlags.Blinded;
        }

        /// <summary>
        /// Toggle whether or not the user is paralyzed.
        /// </summary>
        public void ToggleParalyze()
        {
            Status ^= StatusFlags.Paralyzed;
        }

        /// <summary>
        /// Toggle whether or not the user is near death (in a coma).
        /// </summary>
        public void ToggleComa()
        {
            Status ^= StatusFlags.InComa;
        }

        /// <summary>
        /// Toggle whether or not a user is alive.
        /// </summary>
        public void ToggleAlive()
        {
            Status ^= StatusFlags.Alive;
        }

        public bool IsAlive => Status.HasFlag(StatusFlags.Alive);
        public bool IsAsleep => Status.HasFlag(StatusFlags.Asleep);
        public bool IsFrozen => Status.HasFlag(StatusFlags.Frozen);
        public bool IsBlinded => Status.HasFlag(StatusFlags.Blinded);
        public bool IsParalyzed => Status.HasFlag(StatusFlags.Paralyzed);
        public bool IsInComa => Status.HasFlag(StatusFlags.InComa);

        #endregion

        #region Status handling

        /// <summary>
        /// Apply a given status to a player.
        /// </summary>
        /// <param name="status">The status to apply to the player.</param>
        public virtual bool ApplyStatus(IPlayerStatus status)
        {
            if (!CurrentStatuses.TryAdd(status.Icon, status)) return false;
            status.OnStart();
            return true;
        }

        /// <summary>
        /// Remove a status from a client, firing the appropriate OnEnd events and removing the icon from the status bar.
        /// </summary>
        /// <param name="status">The status to remove.</param>
        /// <param name="onEnd">Whether or not to run the onEnd event for the status removal.</param>
        protected virtual void _removeStatus(IPlayerStatus status, bool onEnd = true)
        {
            if (onEnd)
                status.OnEnd();
        }

        /// <summary>
        /// Remove a status from a client.
        /// </summary>
        /// <param name="icon">The icon of the status we are removing.</param>
        /// <param name="onEnd">Whether or not to run the onEnd effect for the status.</param>
        /// <returns></returns>
        public bool RemoveStatus(ushort icon, bool onEnd = true)
        {
            IPlayerStatus status;
            if (!CurrentStatuses.TryRemove(icon, out status)) return false;
            _removeStatus(status, onEnd);
            return true;
        }

        public bool TryGetStatus(string name, out IPlayerStatus status)
        {
            status = CurrentStatuses.Values.FirstOrDefault(s => s.Name == name);
            return status != null;
        }

        /// <summary>
        /// Remove all statuses from a creature.
        /// </summary>
        public void RemoveAllStatuses()
        {
            lock (CurrentStatuses)
            {
                foreach (var status in CurrentStatuses.Values)
                {
                    _removeStatus(status, false);
                }

                CurrentStatuses.Clear();
                Logger.Debug($"Current status count is {CurrentStatuses.Count}");
            }
        }

        /// <summary>
        /// Process all the given status ticks for a creature's active statuses.
        /// </summary>
        public void ProcessStatusTicks()
        {
            foreach (var kvp in CurrentStatuses)
            {
                Logger.DebugFormat("OnTick: {0}, {1}", Name, kvp.Value.Name);

                if (kvp.Value.Expired)
                {
                    var removed = RemoveStatus(kvp.Key);
                    Logger.DebugFormat($"Status {kvp.Value.Name} has expired: removal was {removed}");
                }

                if (kvp.Value.ElapsedSinceTick >= kvp.Value.Tick)
                {
                    kvp.Value.OnTick();
                }
            }
        }
        #endregion

        // Restrict to (inclusive) range between [min, max]. Max is optional, and if its
        // not present then no upper limit will be enforced.
        protected static long BindToRange(long start, long? min, long? max)
        {
            if (min != null && start < min)
                return min.GetValueOrDefault();
            if (max != null && start > max)
                return max.GetValueOrDefault();
            return start;
        }

        public uint MaximumHp
        {
            get
            {
                var value = BaseHp + BonusHp;

                if (value > uint.MaxValue)
                    return uint.MaxValue;

                if (value < uint.MinValue)
                    return uint.MinValue;

                return (uint)BindToRange(value, StatLimitConstants.MIN_BASE_HPMP, StatLimitConstants.MAX_BASE_HPMP);
            }
        }

        public sbyte Mr
        {
            get
            {
                if (BonusMr > sbyte.MaxValue)
                    return sbyte.MaxValue;

                if (BonusMr < sbyte.MinValue)
                    return sbyte.MinValue;

                return (sbyte)BindToRange(BonusMr, StatLimitConstants.MIN_MR, StatLimitConstants.MAX_MR);
            }
        }

        public sbyte Regen
        {
            get
            {
                if (BonusRegen > sbyte.MaxValue)
                    return sbyte.MaxValue;

                if (BonusRegen < sbyte.MinValue)
                    return sbyte.MinValue;

                return (sbyte)BonusRegen;
            }
        }

        private uint _mLastHitter;

        public Creature LastHitter
        {
            get
            {
                return World.Objects.ContainsKey(_mLastHitter) ? (Creature)World.Objects[_mLastHitter] : null;
            }
            set
            {
                _mLastHitter = value?.Id ?? 0;
            }
        }

        public bool AbsoluteImmortal { get; set; }
        public bool PhysicalImmortal { get; set; }
        public bool MagicalImmortal { get; set; }

        public void SendAnimation(ServerPacket packet)
        {
            Logger.InfoFormat("SendAnimation");
            Logger.InfoFormat("SendAnimation byte format is: {0}", BitConverter.ToString(packet.ToArray()));
            foreach (var user in Map.EntityTree.GetObjects(GetViewport()).OfType<User>())
            {
                var nPacket = (ServerPacket)packet.Clone();
                Logger.InfoFormat("SendAnimation to {0}", user.Name);
                user.Enqueue(nPacket);

            }
        }

        public virtual bool Walk(Direction direction)
        {
            return false;
        }

        public virtual bool Turn(Direction direction)
        {
            Direction = direction;

            foreach (var obj in Map.EntityTree.GetObjects(GetViewport()))
            {
                if (!(obj is User)) continue;
                var user = obj as User;
                var x11 = new ServerPacket(0x11);
                x11.WriteUInt32(Id);
                x11.WriteByte((byte)direction);
                user.Enqueue(x11);
            }

            return true;
        }

        public virtual void Motion(byte motion, short speed)
        {
            foreach (var obj in Map.EntityTree.GetObjects(GetViewport()))
            {
                if (obj is User)
                {
                    var user = obj as User;
                    user.SendMotion(Id, motion, speed);
                }
            }
        }

        //public virtual bool AddItem(Item item, bool updateWeight = true) { return false; }
        //public virtual bool AddItem(Item item, int slot, bool updateWeight = true) { return false; }
        //public virtual bool RemoveItem(int slot, bool updateWeight = true) { return false; }
        //public virtual bool RemoveItem(int slot, int count, bool updateWeight = true) { return false; }
        //public virtual bool AddEquipment(Item item) { return false; }
        //public virtual bool AddEquipment(Item item, byte slot, bool sendUpdate = true) { return false; }
        //public virtual bool RemoveEquipment(byte slot) { return false; }

        public virtual void Heal(double heal, ICreature healer = null)
        {
            if (AbsoluteImmortal || PhysicalImmortal)
                return;

            if (Hp == MaximumHp || heal > MaximumHp)
                return;

            Hp = heal > uint.MaxValue ? MaximumHp : Math.Min(MaximumHp, (uint)(Hp + heal));

            SendDamageUpdate();
        }

        public void Damage(double damage, Enums.Element element = Enums.Element.None, Enums.DamageType damageType = Enums.DamageType.Direct, ICreature attacker = null)
        {
            if (IsInComa || !IsAlive) return;

            if (AbsoluteImmortal) return;

            if (damageType == Enums.DamageType.Physical && PhysicalImmortal)
                return;

            if (damageType == Enums.DamageType.Magical && MagicalImmortal)
                return;

            if (damageType != Enums.DamageType.Direct)
            {
                double armor = Ac * -1 + 100;
                var resist = Game.ElementTable[(int)element, 0];
                var reduction = damage * (armor / (armor + 50));
                damage = (damage - reduction) * resist;
            }

            if (attacker != null)
                _mLastHitter = attacker.Id;

            var normalized = (uint)damage;

            if (normalized > Hp)
                normalized = Hp;

            Hp -= normalized;

            SendDamageUpdate();
        }

        public void RegenerateMp(double mp, Creature regenerator = null)
        {
            if (AbsoluteImmortal)
                return;

            if (Mp == MaximumMp || mp > MaximumMp)
                return;

            Mp = mp > uint.MaxValue ? MaximumMp : Math.Min(MaximumMp, (uint)(Mp + mp));
        }
        protected void SendDamageUpdate()
        {
            var percent = ((Hp / (double)MaximumHp) * 100);
            var healthbar = new ServerPacketStructures.HealthBar() { CurrentPercent = (byte)percent, ObjId = Id };

            foreach (var user in Map.EntityTree.GetObjects(GetViewport()).OfType<User>())
            {
                var nPacket = (ServerPacket)healthbar.Packet().Clone();
                user.Enqueue(nPacket);
            }
        }

        public override void ShowTo(IVisibleObject obj)
        {
            if (!(obj is User)) return;
            var user = (User)obj;
            user.SendVisibleCreature(this);
        }


        public void Attack(Direction direction, Castable castObject, ICreature target)
        {
            //do monster attack.
        }

        public void Attack(Castable castObject, ICreature target)
        {
            //do monster spell
        }

        public void Attack(Castable castObject)
        {
            //do monster aoe
        }
    }
}