// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Death
{
    [Serializable]
    internal class DeathSpellBattleFrenzy : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.SetTimedSuperheroism(player.TimedSuperheroism + Program.Rng.DieRoll(25) + 25);
            player.RestoreHealth(30);
            player.SetTimedFear(0);
            if (player.TimedHaste == 0)
            {
                player.SetTimedHaste(Program.Rng.DieRoll(20 + (player.Level / 2)) + (player.Level / 2));
            }
            else
            {
                player.SetTimedHaste(player.TimedHaste + Program.Rng.DieRoll(5));
            }
        }

        public override void Initialise(int characterClass)
        {
            Name = "Battle Frenzy";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 30;
                    ManaCost = 25;
                    BaseFailure = 75;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.Priest:
                    Level = 33;
                    ManaCost = 33;
                    BaseFailure = 70;
                    FirstCastExperience = 33;
                    break;

                case CharacterClass.Rogue:
                    Level = 32;
                    ManaCost = 32;
                    BaseFailure = 80;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.Ranger:
                    Level = 39;
                    ManaCost = 39;
                    BaseFailure = 76;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.Paladin:
                    Level = 38;
                    ManaCost = 38;
                    BaseFailure = 70;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 30;
                    ManaCost = 30;
                    BaseFailure = 75;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.HighMage:
                    Level = 25;
                    ManaCost = 20;
                    BaseFailure = 65;
                    FirstCastExperience = 50;
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
            return "max dur 50";
        }
    }
}