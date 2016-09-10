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
 * (C) 2016 Project Hybrasyl (info@hybrasyl.com)
 *
 * Authors:   Justin Baugh  <baughj@hybrasyl.com>
 * 
 */

using System;
using Hybrasyl.Enums;
using Hybrasyl.Objects;

namespace Hybrasyl
{

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ProhibitedCondition : System.Attribute
    {
        public StatusFlags Condition { get; set; }

        public ProhibitedCondition(StatusFlags requirement)
        {
            Condition = requirement;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RequiredCondition : System.Attribute
    {
        public StatusFlags Condition { get; set; }
        public string ErrorMessage { get; set; }

        public RequiredCondition(StatusFlags requirement)
        {
            Condition = requirement;
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
        Enums.StatusFlags Conditions { get; set; }
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
        public ushort Icon { get; }
        public int Tick { get; set; }
        public int Duration { get; set; }

        protected Creature Target { get; set; }
        protected Creature Caster { get; set; }
        protected User TargetAsUser => Target as User;
        protected User CasterAsUser => Caster as User;

        public Enums.StatusFlags Conditions { get; set; }

        public DateTime Start { get; }

        public DateTime LastTick { get; private set; }

        public virtual void OnStart()
        {
            if (OnStartMessage != string.Empty) TargetAsUser?.SendSystemMessage(OnStartMessage);
            TargetAsUser?.SendStatusUpdate(Icon, Remaining);
        }

        public virtual void OnTick()
        {
            LastTick = DateTime.Now;
            if (OnTickMessage != string.Empty) TargetAsUser?.SendSystemMessage(OnTickMessage);
            TargetAsUser?.SendStatusUpdate(Icon, Remaining);
        }

        public virtual void OnEnd()
        {
            if (OnEndMessage != string.Empty) TargetAsUser?.SendSystemMessage(OnEndMessage);
            TargetAsUser?.SendRemoveStatus(Icon);
        }

        public bool Expired => (DateTime.Now - Start).TotalSeconds >= Duration;

        public double Elapsed => (DateTime.Now - Start).TotalSeconds;
        public double Remaining => Duration - Elapsed;

        public double ElapsedSinceTick => (DateTime.Now - LastTick).TotalSeconds;

        public string OnTickMessage { get; set; }
        public string OnStartMessage { get; set; }
        public string OnEndMessage { get; set; }
        public string ActionProhibitedMessage { get; set; }

        protected void _initStatus()
        {
            OnTickMessage = String.Empty;
            OnStartMessage = String.Empty;
            OnEndMessage = String.Empty;
        }
        protected PlayerStatus(Creature target, int duration, int tick, ushort icon, string name, Creature caster=null)
        {
            Caster = caster;
            Target = target;
            Duration = duration;
            Tick = tick;
            Icon = icon;
            Name = name;
            Start = DateTime.Now;
            _initStatus();
        }

        protected PlayerStatus(int duration, int tick, ushort icon, string name)
        {
            Duration = duration;
            Tick = tick;
            Icon = icon;
            Name = name;
        }
    }

    internal class BlindStatus : PlayerStatus
    {

        public new static ushort Icon = 3;
        public new static string Name = "blind";
        public new static string ActionProhibitedMessage = "You can't see well enough to do that.";

        public BlindStatus(Creature user, int duration, int tick, Creature caster=null) : base(user, duration, tick, Icon, Name, caster)
        {
            OnStartMessage = "The world goes dark!";
            OnEndMessage = "You can see again.";
        }

        public override void OnStart()
        {
            base.OnStart();
            Target.ToggleBlind();
        }

        public override void OnTick()
        {
        }

        public override void OnEnd()
        {
            base.OnEnd();
            Target?.ToggleBlind();
        }

    }

    internal class PoisonStatus : PlayerStatus
    {
        private new static ushort Icon = 36;
        public new static string Name = "poison";
        public static ushort OnTickEffect = 25;

        public new const string ActionProhibitedMessage = "You double over in pain.";

        private readonly double _damagePerTick;

        public PoisonStatus(User user, int duration, int tick, double damagePerTick) : base(user, duration, tick, Icon, Name)
        {
            OnStartMessage = "Poison";
            OnTickMessage = "Poison is coursing through your veins.";
            OnEndMessage = "You feel better.";
            _damagePerTick = damagePerTick;
        }

        public override void OnStart()
        {
            base.OnStart();
            if (!TargetAsUser?.Status.HasFlag(StatusFlags.InComa) ?? true)
                TargetAsUser?.Effect(OnTickEffect, 120);
        }

        public override void OnTick()
        {
            base.OnTick();
            if (!TargetAsUser?.Status.HasFlag(StatusFlags.InComa) ?? true)
                TargetAsUser?.Effect(OnTickEffect, 120);
            Target.Damage(_damagePerTick);
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
            OnStartMessage = "You are in hibernation.";
            OnEndMessage = "Your body thaws.";
        }

        public override void OnStart()
        {
            base.OnStart();
            if (!TargetAsUser?.Status.HasFlag(StatusFlags.InComa) ?? true)
                TargetAsUser?.Effect(OnTickEffect, 120);
            TargetAsUser.ToggleParalyze();
        }
        public override void OnEnd()
        {
            base.OnEnd();
            TargetAsUser.ToggleParalyze();
        }

    }

    internal class FreezeStatus : PlayerStatus
    {
        public new static ushort Icon = 36;
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
            if (!User.Status.HasFlag(StatusFlags.InComa))
                User.Effect(OnTickEffect, 120);
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
        public new static ushort Icon = 36;
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
            if (!User.Status.HasFlag(StatusFlags.InComa))
                User.Effect(OnTickEffect, 120);
            User.ToggleSleep();
        }

        public override void OnTick()
        {
            base.OnTick();
            User.Effect(OnTickEffect, 120);
        }
        public override void OnEnd()
        {
            base.OnEnd();
            User.ToggleSleep();
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
            User.Effect(OnTickEffect, 120);
            User.ToggleComa();
            User.Group?.SendMessage($"{User.Name} is dying!");
        }

        public override void OnTick()
        {
            base.OnTick();
            if (User.Status.HasFlag(StatusFlags.InComa))
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
        private Script _script;
        private VisibleObject _target;
        private VisibleObject _invoker;

        public CastableStatus(VisibleObject invoker, VisibleObject target, Script script, int duration, ushort icon, string name,
            int tick = 1)
        {
            _invoker = invoker;
           
            _script = script;
            script.ExecuteScriptableFunction("OnCast", _invoker, _target);
        }
    }
    /*
        internal class CastableStatus : PlayerEffect
        {
            private Script _script;

            public CastableEffect(User user, Script script, int duration = 30000, int ticks = 1000)
                : base(user, duration, ticks)
            {
                _script = script;
                Icon = icon;
            }
        }
        */
}
