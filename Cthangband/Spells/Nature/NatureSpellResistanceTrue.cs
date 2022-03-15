// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Nature
{
    [Serializable]
    internal class NatureSpellResistanceTrue : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.SetTimedAcidResistance(player.TimedAcidResistance + Program.Rng.DieRoll(20) + 20);
            player.SetTimedLightningResistance(player.TimedLightningResistance + Program.Rng.DieRoll(20) + 20);
            player.SetTimedFireResistance(player.TimedFireResistance + Program.Rng.DieRoll(20) + 20);
            player.SetTimedColdResistance(player.TimedColdResistance + Program.Rng.DieRoll(20) + 20);
            player.SetTimedPoisonResistance(player.TimedPoisonResistance + Program.Rng.DieRoll(20) + 20);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Resistance True";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 15;
                    ManaCost = 20;
                    BaseFailure = 85;
                    FirstCastExperience = 60;
                    break;

                case CharacterClass.Priest:
                    Level = 18;
                    ManaCost = 20;
                    BaseFailure = 85;
                    FirstCastExperience = 60;
                    break;

                case CharacterClass.Ranger:
                    Level = 20;
                    ManaCost = 30;
                    BaseFailure = 85;
                    FirstCastExperience = 70;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 20;
                    ManaCost = 22;
                    BaseFailure = 85;
                    FirstCastExperience = 60;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Druid:
                    Level = 12;
                    ManaCost = 15;
                    BaseFailure = 75;
                    FirstCastExperience = 60;
                    break;

                default:
                    Level = 99;
                    ManaCost = 0;
                    BaseFailure = 0;
                    FirstCastExperience = 0;
                    break;
            }
        }

        protected override string Comment(Player player)
        {
            return "dur 20+d20";
        }
    }
}