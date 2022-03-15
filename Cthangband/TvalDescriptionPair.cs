// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using System;

namespace Cthangband
{
    [Serializable]
    internal class TvalDescriptionPair
    {
        public static readonly TvalDescriptionPair[] Tvals =
        {
            new TvalDescriptionPair(ItemCategory.Hafted, "Hafted Weapons"),
            new TvalDescriptionPair(ItemCategory.Polearm, "Polearms"),
            new TvalDescriptionPair(ItemCategory.Sword, "Swords"),
            new TvalDescriptionPair(ItemCategory.Bow, "Bows"),
            new TvalDescriptionPair(ItemCategory.Arrow, "Arrows"),
            new TvalDescriptionPair(ItemCategory.Bolt, "Bolts"),
            new TvalDescriptionPair(ItemCategory.Shot, "Shots"),
            new TvalDescriptionPair(ItemCategory.Digging, "Diggers"),
            new TvalDescriptionPair(ItemCategory.Light, "Light Sources"),
            new TvalDescriptionPair(ItemCategory.Chest, "Chests"),
            new TvalDescriptionPair(ItemCategory.SoftArmor, "Soft Armours"),
            new TvalDescriptionPair(ItemCategory.HardArmor, "Hard Armours"),
            new TvalDescriptionPair(ItemCategory.DragArmor, "Dragon Scale Mails"),
            new TvalDescriptionPair(ItemCategory.Cloak, "Cloaks"),
            new TvalDescriptionPair(ItemCategory.Shield, "Shields"),
            new TvalDescriptionPair(ItemCategory.Crown, "Crowns"),
            new TvalDescriptionPair(ItemCategory.Helm, "Helms"),
            new TvalDescriptionPair(ItemCategory.Gloves, "Gloves"),
            new TvalDescriptionPair(ItemCategory.Boots, "Boots"),
            new TvalDescriptionPair(ItemCategory.Amulet, "Amulets"),
            new TvalDescriptionPair(ItemCategory.Potion, "Potions"),
            new TvalDescriptionPair(ItemCategory.Ring, "Rings"),
            new TvalDescriptionPair(ItemCategory.Rod, "Rods"),
            new TvalDescriptionPair(ItemCategory.Scroll, "Scrolls"),
            new TvalDescriptionPair(ItemCategory.Staff, "Staffs"),
            new TvalDescriptionPair(ItemCategory.Wand, "Wands"),
            new TvalDescriptionPair(ItemCategory.ChaosBook, "Chaos Spellbooks"),
            new TvalDescriptionPair(ItemCategory.CorporealBook, "Corporeal Spellbooks"),
            new TvalDescriptionPair(ItemCategory.DeathBook, "Death Spellbooks"),
            new TvalDescriptionPair(ItemCategory.FolkBook, "Folk Spellbooks"),
            new TvalDescriptionPair(ItemCategory.LifeBook, "Life Spellbooks"),
            new TvalDescriptionPair(ItemCategory.NatureBook, "Nature Spellbooks"),
            new TvalDescriptionPair(ItemCategory.SorceryBook, "Sorcery Spellbooks"),
            new TvalDescriptionPair(ItemCategory.TarotBook, "Tarot Spellbooks"),
            new TvalDescriptionPair(ItemCategory.Bottle, "Bottles"),
            new TvalDescriptionPair(ItemCategory.Flask, "Flasks"),
            new TvalDescriptionPair(ItemCategory.Food, "Food"),
            new TvalDescriptionPair(ItemCategory.Junk, "Junk"),
            new TvalDescriptionPair(ItemCategory.Skeleton, "Skeletons"),
            new TvalDescriptionPair(ItemCategory.Spike, "Spikes"),
            new TvalDescriptionPair(0, null)
        };

        public readonly string Desc;
        public readonly ItemCategory Tval;

        private TvalDescriptionPair(ItemCategory tval, string desc)
        {
            Tval = tval;
            Desc = desc;
        }
    }
}