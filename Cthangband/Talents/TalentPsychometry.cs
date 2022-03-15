﻿// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.StaticData;
using System;

namespace Cthangband.Talents
{
    [Serializable]
    internal class TalentPsychometry : Talent
    {
        public override void Initialise(int characterClass)
        {
            Name = "Psychometry";
            Level = 15;
            ManaCost = 12;
            BaseFailure = 60;
        }

        public override void Use(Player player, Level level, SaveGame saveGame)
        {
            if (player.Level < 40)
            {
                Psychometry(player, level);
            }
            else
            {
                saveGame.SpellEffects.IdentifyItem();
            }
        }

        protected override string Comment(Player player)
        {
            return string.Empty;
        }

        private void Psychometry(Player player, Level level)
        {
            if (!SaveGame.Instance.GetItem(out int item, "Meditate on which item? ", true, true, true))
            {
                if (item == -2)
                {
                    Profile.Instance.MsgPrint("You have nothing appropriate.");
                }
                return;
            }
            Item oPtr = item >= 0 ? player.Inventory[item] : level.Items[0 - item];
            if (oPtr.IsKnown() || oPtr.IdentifyFlags.IsSet(Constants.IdentSense))
            {
                Profile.Instance.MsgPrint("You cannot find out anything more about that.");
                return;
            }
            string feel = oPtr.GetDetailedFeeling();
            string oName = oPtr.Description(false, 0);
            if (string.IsNullOrEmpty(feel))
            {
                Profile.Instance.MsgPrint($"You do not perceive anything unusual about the {oName}.");
                return;
            }
            string s = oPtr.Count == 1 ? "is" : "are";
            Profile.Instance.MsgPrint($"You feel that the {oName} {s} {feel}...");
            oPtr.IdentifyFlags.Set(Constants.IdentSense);
            if (string.IsNullOrEmpty(oPtr.Inscription))
            {
                oPtr.Inscription = feel;
            }
            player.NoticeFlags |= Constants.PnCombine | Constants.PnReorder;
        }
    }
}