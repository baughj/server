using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hybrasyl.Objects;
using Newtonsoft.Json;
using Hybrasyl.Enums;

namespace Hybrasyl.Scripting
{
  

    public abstract class HybrasylCreature : IHybrasylCreature
    {
        protected Creature CreatureObject { get; set; }

        protected HybrasylCreature(Creature obj)
        {
            CreatureObject = obj;
        }

        public uint Hp
        {
            get { return CreatureObject.Hp; }
            set
            {
                CreatureObject.Hp = value;
                var user = CreatureObject as User;
                user?.UpdateAttributes(StatUpdateFlags.Current);
            }
        }

        public uint Mp
        {
            get { return CreatureObject.Mp; }
            set
            {
                CreatureObject.Mp = value;
                var user = CreatureObject as User;
                user?.UpdateAttributes(StatUpdateFlags.Current);
            }
        }

        // Passthrough attributes (read-only)
        public sbyte Ac => CreatureObject.Ac;
        public sbyte Mr => CreatureObject.Mr;
        public sbyte Regen => CreatureObject.Regen;
        public byte Level => CreatureObject.Level;
        public byte Ability => CreatureObject.Ability;
        public uint Experience => CreatureObject.Experience;
        public uint AbilityExp => CreatureObject.AbilityExp;
        public byte Str => CreatureObject.Str;
        public byte Dex => CreatureObject.Dex;
        public byte Int => CreatureObject.Int;
        public byte Con => CreatureObject.Con;
        public byte Wis => CreatureObject.Wis;
        public long BaseHp => CreatureObject.BaseHp;
        public long BaseMp => CreatureObject.BaseHp;
        public long BaseStr => CreatureObject.BaseHp;
        public long BaseDex => CreatureObject.BaseHp;
        public long BaseInt => CreatureObject.BaseHp;
        public long BaseCon => CreatureObject.BaseHp;
        public long BaseWis => CreatureObject.BaseHp;

        // Passthrough attributes (Read/write)
        public uint Gold
        {
            get { return CreatureObject.Gold; }
            set { CreatureObject.Gold = value; }
        }
        public long BonusHp
        {
            get { return CreatureObject.BonusHp; }
            set { CreatureObject.BonusHp = value; }
        }

        public long BonusMp
        {
            get { return CreatureObject.BonusMp; }
            set { CreatureObject.BonusMp = value; }
        }

        public long BonusStr
        {
            get { return CreatureObject.BonusStr; }
            set { CreatureObject.BonusStr = value; }
        }
        public long BonusInt
        {
            get { return CreatureObject.BonusStr; }
            set { CreatureObject.BonusStr = value; }
        }
        public long BonusDex
        {
            get { return CreatureObject.BonusStr; }
            set { CreatureObject.BonusStr = value; }
        }
        public long BonusCon
        {
            get { return CreatureObject.BonusStr; }
            set { CreatureObject.BonusStr = value; }
        }
        public long BonusWis
        {
            get { return CreatureObject.BonusStr; }
            set { CreatureObject.BonusStr = value; }
        }
        public long BonusDmg
        {
            get { return CreatureObject.BonusStr; }
            set { CreatureObject.BonusStr = value; }
        }
        public long BonusHit
        {
            get { return CreatureObject.BonusStr; }
            set { CreatureObject.BonusStr = value; }
        }

        public long BonusMr
        {
            get { return CreatureObject.BonusStr; }
            set { CreatureObject.BonusStr = value; }
        }

        public long BonusAc
        {
            get { return CreatureObject.BonusStr; }
            set { CreatureObject.BonusStr = value; }
        }

        public long BonusRegen
        {
            get { return CreatureObject.BonusRegen; }
            set { CreatureObject.BonusRegen = value; }
        }

        public bool IsMonster => CreatureObject is Monster && !(CreatureObject is User);
        public bool IsPlayer => CreatureObject is User;

        public Enums.Element OffensiveElement => CreatureObject.OffensiveElement;
        public Enums.Element DefensiveElement => CreatureObject.DefensiveElement;


        public HybrasylMap Map => new HybrasylMap(CreatureObject.Map);
    }
}
