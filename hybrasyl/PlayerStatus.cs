
using System;
using Hybrasyl.Enums;
using Hybrasyl.Objects;
using Hybrasyl.Statuses;
using Hybrasyl.Castables;

namespace Hybrasyl
{

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ProhibitedCondition : System.Attribute
    {
        public PlayerCondition Condition { get; set; }

        public ProhibitedCondition(PlayerCondition requirement)
        {
            Condition = requirement;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RequiredCondition : System.Attribute
    {
        public PlayerCondition Condition { get; set; }
        public string ErrorMessage { get; set; }

        public RequiredCondition(PlayerCondition requirement)
        {
            Condition = requirement;
        }
    }


    public interface ICreatureStatus
    {

        string OnTickMessage { get; }
        string OnStartMessage { get; }
        string OnEndMessage { get; }
        string ActionProhibitedMessage { get; }
        Enums.PlayerCondition Conditions { get; set; }
        string Name { get; }
        int Duration { get; }
        int Tick { get; }
        DateTime Start { get; }
        DateTime LastTick { get; }
        ushort Icon { get; }
        Creature Target { get; set; }
        Creature Caster { get; set; }
        Castable Castable { get; set; }

        bool Expired { get; }
        double Elapsed { get; }
        double Remaining { get; }
        double ElapsedSinceTick { get; }

        void OnStart();
        void OnTick();
        void OnEnd();
       
    }

    public class Status : ICreatureStatus
    {
        public string Name { get; set; }
        public ushort Icon => _status.Icon;
        public int Tick => _status.Tick;
        public int Duration => _status.Duration;
        public Enums.PlayerCondition Conditions { get; set; }

        public DateTime Start { get; }

        public DateTime LastTick { get; private set; }
        public Creature Target { get; set; }
        public Creature Caster { get; set; }
        public Castable Castable { get; set; }

        private Scripting.Script _script;
        private Statuses.Status _status;

        public string ActionProhibitedMessage => _status.ProhibitedMessage ?? string.Empty;
        public User User => Target as User;

        public string OnStartMessage => _status.Effects?.OnApply?.Message ?? string.Empty;
        public string OnTickMessage => _status.Effects?.OnTick?.Message ?? string.Empty;
        public string OnEndMessage => _status.Effects?.OnRemove?.Message ?? string.Empty;

        public bool Expired => (DateTime.Now - Start).TotalSeconds >= Duration;
        public double Elapsed => (DateTime.Now - Start).TotalSeconds;
        public double Remaining => Duration - Elapsed;

        public double ElapsedSinceTick => (DateTime.Now - LastTick).TotalSeconds;

        protected Status(Creature target, Creature caster, Castable castable=null)
        {
            Target = target;
            Caster = caster;
            Start = DateTime.Now;
            Castable = castable;
        }

        private void SendMessage(string message)
        {
            if (message != string.Empty && User != null)
                User.SendSystemMessage(message);
        }

        private void SendEffect(ushort effect, short speed=100)
        {
            if (User != null && User.Status.HasFlag(PlayerCondition.InComa)) return;
            Target.Effect(effect, speed);
        }
        
        private void HandleAnimation(StatusAnimations animation)
        {
            if (animation != null)
            {
                if (animation.Target != null) SendEffect(animation.Target.Id, animation.Target.Speed);                    
            }
        }

        private void HandleMessaging(string message)
        {
            if (User != null) User.SendSystemMessage(message);
        }

        private void HandleHeal(Statuses.Heal heal)
        {
            Random rand = new Random();

            if (heal.Formula == null)
            {
                var amount = rand.Next(Convert.ToInt32(heal.Simple.Min), Convert.ToInt32(heal.Simple.Max));
                Target.Heal(amount);
            }
            else
            {
                FormulaParser parser = new FormulaParser(Caster, Castable, Target);
                var amount = parser.Eval(heal.Formula);
                Target.Heal(amount);
            }
        }

        private void HandleDamage(Statuses.Damage damage)
        {

            Random rand = new Random();

            if (damage.Formula == null)
            {
                var amount = rand.Next(Convert.ToInt32(damage.Simple.Min), Convert.ToInt32(damage.Simple.Max));
                Target.Damage(amount, Enums.Element.None, (Enums.DamageType)damage.Type);
            }
            else
            {
                FormulaParser parser = new FormulaParser(Caster, Castable, Target);
                var amount = parser.Eval(damage.Formula);
                Target.Damage(amount, Enums.Element.None, (Enums.DamageType)damage.Type);
            }
        }

        private void HandleStatModifiers(Statuses.StatModifiers modifier, bool remove = false)
        {
            // Probably should use reflection here eventually as well
            if (!remove)
            {
                Target.BonusHp += modifier.Hp;
                Target.BonusMp += modifier.Mp;
                Target.BonusStr += modifier.Str;
                Target.BonusInt += modifier.Int;
                Target.BonusWis += modifier.Wis;
                Target.BonusCon += modifier.Con;
                Target.BonusDex += modifier.Dex;
                Target.BonusHit += modifier.Hit;
                Target.BonusDmg += modifier.Dmg;
                Target.BonusAc += modifier.Ac;
                Target.BonusMr += modifier.Mr;
                Target.BonusRegen += modifier.Regen;
                Target.ReflectChance = modifier.ReflectChance;
                Target.ReflectChance = modifier.ReflectIntensity;
                Target.DamageModifier = modifier.DamageModifier;
                Target.HealModifier = modifier.HealModifier;
                Target.OffensiveElementOverride = (Enums.Element)modifier.OffensiveElement;
                Target.DefensiveElementOverride = (Enums.Element)modifier.DefensiveElement;

            }
            else
            {
                Target.BonusHp -= modifier.Hp;
                Target.BonusMp -= modifier.Mp;
                Target.BonusStr -= modifier.Str;
                Target.BonusInt -= modifier.Int;
                Target.BonusWis -= modifier.Wis;
                Target.BonusCon -= modifier.Con;
                Target.BonusDex -= modifier.Dex;
                Target.BonusHit -= modifier.Hit;
                Target.BonusDmg -= modifier.Dmg;
                Target.BonusAc -= modifier.Ac;
                Target.BonusMr -= modifier.Mr;
                Target.BonusRegen -= modifier.Regen;
                // Always reset damage / heal / reflect chance to sane values
                Target.ReflectChance = 0;
                Target.ReflectChance = 0;
                Target.DamageModifier = 1;
                Target.HealModifier = 1;
                // Remove elemental overrides
                Target.OffensiveElementOverride = Enums.Element.None;
                Target.DefensiveElementOverride = Enums.Element.None;
            }

        }

        public virtual void OnStart()
        {
            if (_status.Effects.OnApply != null)
            {
                HandleMessaging(_status.Effects.OnApply.Message);
                HandleAnimation(_status.Effects.OnApply.Animations);
                HandleHeal(_status.Effects.OnApply.Heal);
                HandleDamage(_status.Effects.OnApply.Damage);
                HandleStatModifiers(_status.Effects.OnApply.StatModifiers);
            }
        }

        public virtual void OnTick()
        {
            LastTick = DateTime.Now;
            if (_status.Effects.OnTick != null)
            {
                HandleMessaging(_status.Effects.OnTick.Message);
                HandleAnimation(_status.Effects.OnTick.Animations);
                HandleHeal(_status.Effects.OnTick.Heal);
                HandleDamage(_status.Effects.OnTick.Damage);
                // Make sure your XML makes sense here or you're gonna end up with real, total bullshit
                HandleStatModifiers(_status.Effects.OnTick.StatModifiers);
            }
        }

        public virtual void OnEnd()
        {
            if (_status.Effects.OnRemove != null)
            {
                HandleMessaging(_status.Effects.OnRemove.Message);
                HandleAnimation(_status.Effects.OnRemove.Animations);
                HandleHeal(_status.Effects.OnRemove.Heal);
                HandleDamage(_status.Effects.OnRemove.Damage);
            }
            // Note that StatModifiers inside OnRemove are effectively ignored.
            // In all cases we simply remove what we applied before, which I 
            // believe makes way more sense
            if (_status.Effects.OnApply != null)
                HandleStatModifiers(_status.Effects.OnApply.StatModifiers, true);

        }

    }
    

}
