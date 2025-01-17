﻿/*
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
 
using Hybrasyl.Enums;
using Hybrasyl.Scripting;
using Hybrasyl.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using Hybrasyl.Xml;

namespace Hybrasyl.Objects;

public class ItemObject : VisibleObject
{
    public string TemplateId { get; private set; }

    public StatInfo Stats { get; private set; }

    /// <summary>
    /// Check to see if a specified user can equip an ItemObject. Returns a boolean indicating whether
    /// the ItemObject can be equipped and if not, sets the message reference to contain an appropriate
    /// message to be sent to the user.
    /// </summary>
    /// <param name="userobj">User object to check for meeting this ItemObject's requirements.</param>
    /// <param name="message">A reference that will be used in the case of failure to set an appropriate error message.</param>
    /// <returns></returns>
    public bool CheckRequirements(User userobj, out string message)
    {
        // We check a variety of conditions and return the first failure.

        message = string.Empty;

        // Check gender

        if (Gender != 0 && (Gender != userobj.Gender))
        {
            message = World.GetLocalString("item_equip_wrong_gender");
            return false;
        }

        // Check class

        if (userobj.Class != Class && Class != Xml.Class.Peasant)
        {
            message = userobj.Class == Xml.Class.Peasant
                ? World.GetLocalString("item_equip_peasant")
                : World.GetLocalString("item_equip_wrong_class");
            return false;
        }

        // Check level / AB

        if (userobj.Stats.Level < MinLevel || (MinAbility != 0 && userobj.Stats.Ability < MinAbility))
        {
            message = World.GetLocalString("item_equip_more_insight");
            return false;
        }

        if (userobj.Stats.Level > MaxLevel || userobj.Stats.Ability > MaxAbility)
        {
            message = World.GetLocalString("item_equip_less_insight");
            return false;
        }

        if (userobj.Equipment.Weight + Weight > userobj.MaximumWeight/2)
        {
            message = World.GetLocalString("item_equip_too_heavy");
            return false;
        }

        // Check if user is equipping a shield while holding a two-handed weapon

        if (EquipmentSlot == (byte)ItemSlots.Shield && userobj.Equipment.Weapon != null && userobj.Equipment.Weapon.WeaponType == Xml.WeaponType.TwoHand)
        {
            message = World.GetLocalString("item_equip_shield_2h");
            return false;
        }

        // Check if user is equipping a two-handed weapon while holding a shield

        if (EquipmentSlot == (byte) ItemSlots.Weapon && (WeaponType == Xml.WeaponType.TwoHand || WeaponType == Xml.WeaponType.Staff) && userobj.Equipment.Shield != null)
        {
            message = World.GetLocalString("item_equip_2h_shield");
            return false;
        }

        // Check unique-equipped
        if (UniqueEquipped && userobj.Equipment.FindById(TemplateId) != null)
        {
            message = World.GetLocalString("item_equip_unique_equipped");
            return false;
        }

        // Check item slot prohibitions

        foreach (var restriction in Template.Properties.Restrictions?.SlotRestrictions ?? new List<SlotRestriction>())
        {
            var restrictionMessage = World.GetLocalString(restriction.Message == string.Empty ? "item_equip_slot_restriction" : restriction.Message);

            if (restriction.Type == SlotRestrictionType.ItemProhibited)
            {
                if ((restriction.Slot == Xml.EquipmentSlot.Ring && userobj.Equipment.LRing != null ||
                     userobj.Equipment.RRing != null) || (restriction.Slot == Xml.EquipmentSlot.Gauntlet &&
                                                          userobj.Equipment.LGauntlet != null ||
                                                          userobj.Equipment.RGauntlet != null)
                                                      || userobj.Equipment[(byte) restriction.Slot] != null)
                {
                    message = restrictionMessage;
                    return false;
                }
            }
            else
            {
                if ((restriction.Slot == Xml.EquipmentSlot.Ring && userobj.Equipment.LRing != null ||
                     userobj.Equipment.RRing != null) || (restriction.Slot == Xml.EquipmentSlot.Gauntlet &&
                                                          userobj.Equipment.LGauntlet != null ||
                                                          userobj.Equipment.RGauntlet != null)
                                                      || userobj.Equipment[(byte) restriction.Slot] == null)
                {
                    message = restrictionMessage;
                    return false;
                }
            }
        }

        // Check other equipped item slot restrictions 
        var items = userobj.Equipment.Where(x => x.Template.Properties.Restrictions?.SlotRestrictions != null);
        foreach (var restriction in items.SelectMany(x => x.Template.Properties.Restrictions.SlotRestrictions))
        {
            var restrictionMessage = World.GetLocalString(restriction.Message == string.Empty ? "item_equip_slot_restriction" : restriction.Message);

            if (restriction.Type == SlotRestrictionType.ItemProhibited)
            {
                if ((restriction.Slot == Xml.EquipmentSlot.Ring &&
                     EquipmentSlot == (byte) Xml.EquipmentSlot.LeftHand ||
                     EquipmentSlot == (byte) Xml.EquipmentSlot.RightHand) ||
                    (restriction.Slot == Xml.EquipmentSlot.Gauntlet &&
                     EquipmentSlot == (byte) Xml.EquipmentSlot.LeftArm ||
                     EquipmentSlot == (byte) Xml.EquipmentSlot.RightArm) ||
                    EquipmentSlot == (byte) restriction.Slot)
                {
                    message = restrictionMessage;
                    return false;
                }
            }
            else
            {
                if ((restriction.Slot == Xml.EquipmentSlot.Ring && userobj.Equipment.LRing != null ||
                     userobj.Equipment.RRing != null) || (restriction.Slot == Xml.EquipmentSlot.Gauntlet && userobj.Equipment.LGauntlet != null || userobj.Equipment.RGauntlet != null)
                                                      || EquipmentSlot != (byte) restriction.Slot)
                {
                    message = restrictionMessage;
                    return false;
                }
            }

        }

        // Check castable requirements
        if (Template.Properties?.Restrictions?.Castables != null)
        {
            var hasCast = false;
            // Behavior is ANY castable, not ALL in list
            foreach (var castable in Template.Properties.Restrictions.Castables)
            {
                if (userobj.SkillBook.IndexOf(castable) != -1 ||
                    userobj.SpellBook.IndexOf(castable) != -1)
                {
                    hasCast = true;
                }
            }
            if (!hasCast && Template.Properties.Restrictions.Castables.Count > 0)
            {
                message = World.GetLocalString("item_equip_missing_castable");
                return false;
            }
        }

        // Check mastership requirement
        if (MasterOnly && (!userobj.IsMaster))
        {
            message = World.GetLocalString("item_equip_not_master");
            return false;
        }
        return true;
    }

    private Xml.Item _template;

    public Xml.Item Template
    {
        get
        {
            return _template ?? World.WorldData.Get<Item>(TemplateId);
        }
        set => _template = value;
    }

    public new string Name => Template.Name;

    public new ushort Sprite => Template.Properties.Appearance.Sprite;

    public bool Usable => Template.Properties.Use != null;
    public Xml.Use Use => Template.Properties.Use;

    public ushort EquipSprite => Template.Properties.Appearance.EquipSprite == 0 ? Template.Properties.Appearance.Sprite : Template.Properties.Appearance.EquipSprite;

    public ItemObjectType ItemObjectType
    {
        get
        {
            if ((Template?.Properties?.Equipment?.Slot ?? Xml.EquipmentSlot.None) != Xml.EquipmentSlot.None)
                return ItemObjectType.Equipment;
            else if (Template.Properties.Flags.HasFlag(Xml.ItemFlags.Consumable) || Template.Use != null)
                return ItemObjectType.CanUse;
            return ItemObjectType.CannotUse;
        }
    }

    public Xml.WeaponType WeaponType => Template.Properties.Equipment.WeaponType;
    public byte EquipmentSlot => Convert.ToByte(Template.Properties.Equipment.Slot);
    public string SlotName => Enum.GetName(typeof(Xml.EquipmentSlot), EquipmentSlot) ?? "None";
    public int Weight => Template.Properties.Physical.Weight > int.MaxValue ? int.MaxValue : Convert.ToInt32(Template.Properties.Physical.Weight);
    public int MaximumStack => Template.MaximumStack;
    public bool Stackable => Template.Stackable;

    public List<Xml.CastModifier> CastModifiers => Template.Properties.CastModifiers;

    public uint MaximumDurability => Template.Properties?.Physical?.Durability > uint.MaxValue
        ? uint.MaxValue
        : Convert.ToUInt32(Template.Properties.Physical.Durability);

    public uint RepairCost
    {
        get
        {
            if (MaximumDurability != 0)
                return Durability == 0 ? Value : (uint) ((Durability / MaximumDurability) * Value);
            return 0;
        }
    } 

    // For future use / expansion re: unidentified items.
    // Should pull from template and only allow false to be set when
    // Identifiable flag is set.
    public bool Identified => true;

    public byte MinLevel => Template.MinLevel;
    public byte MinAbility => Template.MinAbility;
    public byte MaxLevel => Template.MaxLevel;
    public byte MaxAbility => Template.MaxAbility;

    public Xml.Class Class => Template.Class;
    public Xml.Gender Gender => Template.Gender;

    public byte Color => Convert.ToByte(Template.Properties.Appearance.Color);
    public List<string> Categories => Template.Categories;

    public byte BodyStyle => Convert.ToByte(Template.Properties.Appearance.BodyStyle);

    public Xml.ElementType Element => Template.Element;

    public ushort MinLDamage => Template.MinLDamage;
    public ushort MaxLDamage => Template.MaxLDamage;
    public ushort MinSDamage => Template.MinSDamage;
    public ushort MaxSDamage => Template.MaxSDamage;
    public ushort DisplaySprite => Template.Properties.Appearance.DisplaySprite;

    public uint Value => Template.Properties.Physical.Value > uint.MaxValue
        ? uint.MaxValue
        : Convert.ToUInt32(Template.Properties.Physical.Value);

    public bool HideBoots => Template.Properties.Appearance.HideBoots;


    public bool Enchantable => Template.Properties.Flags.HasFlag(Xml.ItemFlags.Enchantable);
    public bool Depositable => Template.Properties.Flags.HasFlag(ItemFlags.Depositable);

    public bool Consecratable => Template.Properties.Flags.HasFlag(Xml.ItemFlags.Consecratable);

    public bool Tailorable => Template.Properties.Flags.HasFlag(Xml.ItemFlags.Tailorable);

    public bool Smithable => Template.Properties.Flags.HasFlag(Xml.ItemFlags.Smithable);

    public bool Exchangeable => Template.Properties.Flags.HasFlag(Xml.ItemFlags.Exchangeable);

    public bool MasterOnly => Template.Properties.Flags.HasFlag(Xml.ItemFlags.MasterOnly);

    public bool Perishable => Template.Properties.Flags.HasFlag(Xml.ItemFlags.Perishable);

    public bool UniqueInventory => Template.Properties.Flags.HasFlag(Xml.ItemFlags.UniqueInventory);

    public bool UniqueEquipped => Template.Properties.Flags.HasFlag(Xml.ItemFlags.UniqueEquipped);

    public bool Consumable => Template.Properties.Flags.HasFlag(Xml.ItemFlags.Consumable);

    public bool Undamageable => Template.Properties.Flags.HasFlag(Xml.ItemFlags.Undamageable);

    public bool IsVariant => Template.IsVariant;

    public Xml.Item ParentItem => Template.ParentItem;

    public Xml.Variant CurrentVariant => Template.CurrentVariant;

    private Lockable<int> _count { get; set; }
    public int Count
    {
        get { return _count.Value; }
        set { _count.Value = value; }
    }

    private Lockable<double> _durability { get; set; }

    public double Durability
    {
        get { return _durability.Value; }
        set { _durability.Value = value; }
    }

    public uint DisplayDurability => Convert.ToUInt32(Math.Round(Durability));

    public void EvalFormula(Creature source)
    {
        if (Template.Properties?.StatModifiers != null)
            Stats = NumberCruncher.CalculateItemModifiers(this, source);
    }

    public void Invoke(User trigger)
    {
        if (Stackable && Count <= 0)
        {
            trigger.RemoveItem(Name);
            return;
        }

        GameLog.InfoFormat($"User {trigger.Name}: used item {Name}");

        if (Consumable && Template.Properties.StatModifiers != null)
        {
            var statChange = NumberCruncher.CalculateItemModifiers(this, trigger);
            trigger.Stats.Apply(statChange);
            trigger.UpdateAttributes(StatUpdateFlags.Full);
        }

        // Run through all the different potential uses. We allow combinations of any
        // use specified in the item XML.

        if (Use?.Script != null)
        {
            Script invokeScript;
            if (!World.ScriptProcessor.TryGetScript(Use.Script, out invokeScript))
            {
                trigger.SendSystemMessage("It doesn't work.");
                return;
            }

            invokeScript.ExecuteFunction("OnUse", trigger, null, this, true);
        }            

        if (Use?.Effect != null)
        {
            trigger.SendEffect(trigger.Id, Use.Effect.Id, Use.Effect.Speed); 
        }
        
        if (Use?.Sound != null)
        {
            trigger.SendSound((byte) Use.Sound.Id);
        }
        if (Use?.Teleport != null)
        {
            trigger.Teleport(Use.Teleport.Value, Use.Teleport.X, Use.Teleport.Y);
        }
        if (Consumable)
        {
            Count--;
        }
    }

    public ItemObject(string id, Guid containingWorld = default, Guid guid = default)
    {
        ServerGuid = containingWorld;
        TemplateId = id;
        _durability = new Lockable<double>(MaximumDurability);
        _count = new Lockable<int>(1);
        Guid = guid != default ? guid : Guid.NewGuid();
    }

    public ItemObject(Xml.Item template, Guid containingWorld = default, Guid guid = default)
    {
        Template = template;
        TemplateId = template.Id;
        ServerGuid = containingWorld;
        _count = new Lockable<int>(1);
        _durability = new Lockable<double>(MaximumDurability);
        Guid = guid != default ? guid : Guid.NewGuid();
    }

    // Simple copy constructor for an ItemObject, mostly used when we split a stack and it results
    // in the creation of a new ItemObject.
    public ItemObject(ItemObject previousItemObject)
    {
        _count = new Lockable<int>(previousItemObject.Count);
        _durability = new Lockable<double>(previousItemObject.Durability);
        ServerGuid = previousItemObject.ServerGuid;
        TemplateId = previousItemObject.TemplateId;
        Durability = previousItemObject.Durability;
        Count = previousItemObject.Count;
        Guid = Guid.NewGuid();
    }

    public override void ShowTo(VisibleObject obj)
    {
        if (obj is User)
        {
            var user = obj as User;
            user.SendVisibleItem(this);
        }
    }
}