﻿using Hybrasyl.Objects;
using MoonSharp.Interpreter;

namespace Hybrasyl.Scripting;

[MoonSharpUserData]
public class HybrasylMonster
{
    internal Monster Monster { get; set; }
    internal HybrasylWorld World { get; set; }
    internal HybrasylMap Map { get; set; }
    public bool IsPlayer => false;

    public string Name => Monster.Name;


    public void DebugFunction(string x)
    {
        GameLog.ScriptingWarning(x);
    }
    public Xml.Direction Direction => Monster.Direction;

    public ThreatInfo ThreatInfo => Monster.ThreatInfo;
    public WorldObject Target => Monster.Target;
    public bool AbsoluteImmortal => Monster.AbsoluteImmortal;
    public bool PhysicalImmortal => Monster.PhysicalImmortal;
    public bool MagicalImmortal => Monster.MagicalImmortal;

    public WorldObject FirstHitter => Monster.FirstHitter;
    public WorldObject LastHitter => Monster.LastHitter;
    public string LastHitTime => Monster.LastHitTime.ToString();

    public void ForceThreatChange(HybrasylUser invoker) => Monster.ThreatInfo.ForceThreatChange(invoker.User);
    public void MakeHostile() => Monster.MakeHostile();

    /// <summary>
    /// Access the StatInfo of the specified user directly (all stats).
    /// </summary>
    public StatInfo Stats => Monster.Stats;

    public HybrasylMonster(Monster monster)
    {
        Monster = monster;
        World = new HybrasylWorld(monster.World);
        Map = new HybrasylMap(monster.Map);
    }
    public void SystemMessage(string nil) {}

    /// <summary>
    /// Deal damage to the current player.
    /// </summary>
    /// <param name="damage">Integer amount of damage to deal.</param>
    /// <param name="element">Element of the damage (e.g. fire, air)</param>
    /// <param name="damageType">Type of damage (direct, magical, etc)</param>
    public void Damage(int damage, Xml.ElementType element = Xml.ElementType.None,
        Xml.DamageType damageType = Xml.DamageType.Direct)
    {
        Monster.Damage(damage, element, damageType);
    }

    public string GetGMMonsterInfo()
    {
        var s = "Monster Debug Info\n----------------------------\n\n";

        ////this is for debug only
        s += $"Name: {Monster.Name} | Id: {Monster.Id}\n";
        s += $"Level: {Monster.Stats.Level}  Health: {Monster.Stats.Hp}/{Monster.Stats.MaximumHp}  Mana: {Monster.Stats.Mp} / {Monster.Stats.MaximumMp}\n";
        s += $"Stats: STR {Monster.Stats.Str} CON {Monster.Stats.Con} WIS {Monster.Stats.Wis} INT {Monster.Stats.Int} DEX {Monster.Stats.Dex}\n";
        s += $"Experience: {Monster.LootableXP}\n\n";
        s += "Castables:\n";

        foreach (var rotation in Monster.CastableController)
        {
            s += $"  Set Type: {rotation.CastingSet.Type}, {rotation.Interval} second timer, target priority {rotation.CastingSet.TargetPriority} \n  Rotation:\n";
            foreach (var entry in rotation)
                s += $"    Castable: {entry.Name}, {entry.Directive} second timer, {entry.Threshold}% health, UseOnce: {entry.UseOnce}, Triggered: {entry.ThresholdTriggered}";
        }

        s += $"AbsoluteImmortal: {Monster.AbsoluteImmortal}\n";
        s += $"PhysicalImmortal: {Monster.PhysicalImmortal}\n";
        s += $"MagicalImmortal: {Monster.MagicalImmortal}\n";
        s += $"IsHostile: {Monster.IsHostile}\n";
        s += $"ShouldWander: {Monster.ShouldWander}\n";

        if (Monster.Target != null) s += $"Target: {Monster.Target.Name}\n";
        if (Monster.FirstHitter != null) s += $"FirstHitter: {Monster.FirstHitter.Name}\n";
        if (Monster.LastHitter != null) s += $"LastHitter: {Monster.LastHitter.Name}\n";
        if (Monster.LastHitTime != default) s += $"LastHitTime: {Monster.LastHitTime}\n";
        if (Monster.ThreatInfo != null)
        {
            s += $"ThreatInfo:\n";
            foreach (var user in Monster.ThreatInfo.ThreatTableByCreature)
            {
                s += $"Name: {Game.World.WorldData.GetWorldObject<VisibleObject>(user.Key)?.Name ?? "unknown"} | Threat: {user.Value}\n";
            }
        }

        return s;
    }
}