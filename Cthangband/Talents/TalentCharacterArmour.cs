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