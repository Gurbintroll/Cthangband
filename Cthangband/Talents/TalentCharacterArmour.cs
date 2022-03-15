// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using System;

namespace Cthangband.Talents
{
    [Serializable]
    internal class TalentCharacterArmour : Talent
    {
        public override void Initialise(int characterClass)
        {
            Name = "Character Armour";
            Level = 13;
            ManaCost = 12;
            BaseFailure = 50;
        }

        public override void Use(Player player, Level level, SaveGame saveGame)
        {
            player.SetTimedStoneskin(player.TimedStoneskin + player.Level);
            if (player.Level > 14)
            {
                player.SetTimedAcidResistance(player.TimedAcidResistance + player.Level);
            }
            if (player.Level > 19)
            {
                player.SetTimedFireResistance(player.TimedFireResistance + player.Level);
            }
            if (player.Level > 24)
            {
                player.SetTimedColdResistance(player.TimedColdResistance + player.Level);
            }
            if (player.Level > 29)
            {
                player.SetTimedLightningResistance(player.TimedLightningResistance + player.Level);
            }
            if (player.Level > 34)
            {
                player.SetTimedPoisonResistance(player.TimedPoisonResistance + player.Level);
            }
        }

        protected override string Comment(Player player)
        {
            return $"dur {player.Level}";
        }
    }
}