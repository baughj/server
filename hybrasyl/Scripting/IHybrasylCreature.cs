using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hybrasyl.Scripting
{
    public interface IHybrasylCreature
    {
        uint Hp { get; set; }
        uint Mp { get; set; }
        byte Level { get; }
        uint Experience { get; }
        byte Ability { get; }
        uint AbilityExp { get; }
        long BaseHp { get; }
        long BaseMp { get; }
        long BaseStr { get; }
        long BaseInt { get; }
        long BaseWis { get; }
        long BaseCon { get; }
        long BaseDex { get; }

        long BonusHp { get; set; }
        long BonusMp { get; set; }
        long BonusStr { get; set; }
        long BonusInt { get; set; }
        long BonusWis { get; set; }
        long BonusCon { get; set; }
        long BonusDex { get; set; }
        long BonusDmg { get; set; }
        long BonusHit { get; set; }
        long BonusAc { get; set; }
        long BonusMr { get; set; }
        long BonusRegen { get; set; }

        byte Str { get; }
        byte Int { get; }
        byte Wis { get; }
        byte Con { get; }
        byte Dex { get; }

        sbyte Ac { get; }
        sbyte Mr { get; }
        sbyte Regen { get; }
        Enums.Element OffensiveElement { get; }
        Enums.Element DefensiveElement { get; }
        HybrasylMap Map { get; }
        uint Gold { get; set; }
        bool IsMonster { get; }
        bool IsPlayer { get; }

    }
}
