
using System;
using Hybrasyl.Enums;
using Hybrasyl.Objects;
using Hybrasyl.Scripting;
using Hybrasyl.XML;
using Hybrasyl.Castables;
using System.Collections.Generic;

namespace Hybrasyl
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ProhibitedConditions : Attribute
    {
        public PlayerCondition Condition;
        public PlayerState State;

        public ProhibitedConditions(params object[] requirements)
        {
            foreach (var obj in requirements)
            {
                if (obj is PlayerCondition) { Condition |= (PlayerCondition)obj; }
                else if (obj is PlayerState) { State |= (PlayerState)obj; }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RequiredConditions : Attribute
    {
        public PlayerCondition Condition;
        public PlayerState State;

        public RequiredConditions(params object[] requirements)
        {
            foreach (var obj in requirements)
            {
                if (obj is PlayerCondition) { Condition |= (PlayerCondition)obj; }
                else if (obj is PlayerState) { State |= (PlayerState)obj; }
            }
        }
    }

    public interface IPlayerStatus
    {

        string Name { get; }
        string OnTickMessage { get; set; }
        string OnStartMessage { get; set; }
        string OnEndMessage { get; set; }
        string ActionProhibitedMessage { get; set; }
        int Duration { get; set; }
        int Tick { get; set; }
        DateTime Start { get; }
        DateTime LastTick { get; }
        PlayerState Conditions { get; set; }
        ushort Icon { get; }

        bool Expired { get; }
        double Elapsed { get; }
        double Remaining { get; }
        double ElapsedSinceTick { get; }

        void OnStart();
        void OnTick();
        void OnEnd();

        int GetHashCode();
    }

    public abstract class PlayerStatus : IPlayerStatus
    {
        public string Name { get; set; }
        public ushort Icon { get; set; }
        public int Tick { get; set; }
        public int Duration { get; set; }
        protected User User { get; set; }
        public PlayerState Conditions { get; set; }

        public DateTime Start { get; }

        public DateTime LastTick { get; private set; }

        public virtual void OnStart()
        {
            if (OnStartMessage != string.Empty) User.SendSystemMessage(OnStartMessage);
            var tickEffect = (ushort?) GetType().GetField("OnTickEffect").GetValue(null);
            if (tickEffect == null) return;
            if (!User.Condition.HasFlag(PlayerCondition.Coma))
                User.Effect((ushort)tickEffect, 120);
        }

        public virtual void OnTick()
        {
            LastTick = DateTime.Now;
            if (OnTickMessage != string.Empty) User.SendSystemMessage(OnTickMessage);
            var tickEffect = (ushort?)GetType().GetField("OnTickEffect").GetValue(null);
            if (tickEffect == null) return;
            if (!User.Condition.HasFlag(PlayerCondition.Coma))
                User.Effect((ushort)tickEffect, 120);
        }

        public virtual void OnEnd()
        {
            if (OnEndMessage != string.Empty) User.SendSystemMessage(OnEndMessage);
        }

        public bool Expired => (DateTime.Now - Start).TotalSeconds >= Duration;

        public double Elapsed => (DateTime.Now - Start).TotalSeconds;
        public double Remaining => Duration - Elapsed;

        public double ElapsedSinceTick => (DateTime.Now - LastTick).TotalSeconds;

        public string OnTickMessage { get; set; }
        public string OnStartMessage { get; set; }
        public string OnEndMessage { get; set; }
        public string ActionProhibitedMessage { get; set; }

        protected PlayerStatus(User user, int duration, int tick, ushort icon, string name)
        {
            User = user;
            Duration = duration;
            Tick = tick;
            Icon = icon;
            Name = name;
            Start = DateTime.Now;
            OnTickMessage = string.Empty;
            OnStartMessage = string.Empty;
            OnEndMessage = string.Empty;
        }

        protected PlayerStatus(User user)
        {
            User = user;
        }
    }

    internal class BlindStatus : PlayerStatus
    {

        public new static ushort Icon = 3;
        public new static string Name = "blind";
        public new static string ActionProhibitedMessage = "You can't see well enough to do that.";

        public BlindStatus(User user, int duration, int tick) : base(user, duration, tick, Icon, Name)
        {
            OnStartMessage = "The world goes dark!";
            OnEndMessage = "You can see again.";
        }

        public override void OnStart()
        {
            base.OnEnd();
            User.ToggleBlind();
        }

        public override void OnEnd()
        {
            base.OnEnd();
            User.ToggleBlind();
        }

    }

    internal class PoisonStatus : PlayerStatus
    {
        private new static ushort Icon = 97;
        public new static string Name = "poison";
        public static ushort OnTickEffect = 25;

        public new const string ActionProhibitedMessage = "You double over in pain.";

        private readonly double _damagePerTick;

        public PoisonStatus(User user, int duration, int tick, double damagePerTick)
            : base(user, duration, tick, Icon, Name)
        {
            OnStartMessage = "Poison";
            OnTickMessage = "Poison is coursing through your veins.";
            OnEndMessage = "You feel better.";
            _damagePerTick = damagePerTick;
        }

        public override void OnTick()
        {
            base.OnTick();
            if (_damagePerTick >= User.Hp)
                User.Damage(User.Hp - 1);
            else
                User.Damage(_damagePerTick);
        }
    }

    internal class ParalyzeStatus : PlayerStatus
    {
        public new static ushort Icon = 36;
        public new static string Name = "paralyze";
        public static ushort OnTickEffect = 41;

        public new const string ActionProhibitedMessage = "You cannot move!";

        public ParalyzeStatus(User user, int duration, int tick) : base(user, duration, tick, Icon, Name)
        {
            OnStartMessage = "Stunned!";
            OnEndMessage = "You can move again.";
        }

        public override void OnStart()
        {
            base.OnStart();
            User.ToggleParalyzed();
        }

        public override void OnEnd()
        {
            base.OnEnd();
            User.ToggleParalyzed();
        }

    }

    internal class FreezeStatus : PlayerStatus
    {
        public new static ushort Icon = 50;
        public new static string Name = "freeze";
        public static ushort OnTickEffect = 40;

        public new const string ActionProhibitedMessage = "You cannot move!";

        public FreezeStatus(User user, int duration, int tick) : base(user, duration, tick, Icon, Name)
        {
            OnStartMessage = "You are in hibernation.";
            OnEndMessage = "Your body thaws.";
        }

        public override void OnStart()
        {
            base.OnStart();
            User.ToggleFreeze();
        }

        public override void OnEnd()
        {
            base.OnEnd();
            User.ToggleFreeze();
        }
    }

    internal class SleepStatus : PlayerStatus
    {
        public new static ushort Icon = 2;
        public new static string Name = "sleep";
        public static ushort OnTickEffect = 28;

        public new const string ActionProhibitedMessage = "You are too sleepy to even raise your hands!";

        public SleepStatus(User user, int duration, int tick) : base(user, duration, tick, Icon, Name)
        {
            OnStartMessage = "You are asleep.";
            OnEndMessage = "You awaken.";
        }

        public override void OnStart()
        {
            base.OnStart();
            User.ToggleAsleep();
        }

        public override void OnEnd()
        {
            base.OnEnd();
            User.ToggleAsleep();
        }
    }

    internal class NearDeathStatus : PlayerStatus
    {
        public new static ushort Icon = 24;
        public new static string Name = "neardeath";
        public static ushort OnTickEffect = 24;

        public new const string ActionProhibitedMessage = "The life is draining from your body.";


        public NearDeathStatus(User user, int duration, int tick) : base(user, duration, tick, Icon, Name)
        {
            OnStartMessage = "You are near death.";
            OnEndMessage = "You have died!";
        }

        public override void OnStart()
        {
            base.OnStart();
            User.ToggleNearDeath();
            User.Group?.SendMessage($"{User.Name} is dying!");
        }

        public override void OnTick()
        {
            base.OnTick();
            if (User.Condition.HasFlag(PlayerCondition.Coma))
                User.Effect(OnTickEffect, 120);
            if (Remaining < 5)
                User.Group?.SendMessage($"{User.Name}'s soul hangs by a thread!");
        }

        public override void OnEnd()
        {
            base.OnEnd();
            User.OnDeath();
        }
    }


    internal class CastableStatus : PlayerStatus
    {
        private readonly Script _script;
        public ushort OnTickEffect;

        public CastableStatus(User user, Script script)
            : base(user)
        {
            _script = script;
            Icon = _script.Scope.GetVariable<ushort>("Icon");
            Tick = _script.Scope.GetVariable<ushort>("Tick");
            Duration = _script.Scope.GetVariable<int>("Duration");
            OnTickEffect = script.Scope.GetVariable<ushort>("OnTickEffect");
        }
    }
}
