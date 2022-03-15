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
    internal class DeathSpellBerserk : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.SetTimedSuperheroism(player.TimedSuperheroism + Program.Rng.DieRoll(25) + 25);
            player.RestoreHealth(30);
            player.SetTimedFear(0);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Berserk";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 10;
                    ManaCost = 20;
                    BaseFailure = 80;
                    FirstCastExperience = 180;
                    break;

                case CharacterClass.Priest:
                    Level = 13;
                    ManaCost = 20;
                    BaseFailure = 80;
                    FirstCastExperience = 180;
                    break;

                case CharacterClass.Rogue:
                    Level = 20;
                    ManaCost = 25;
                    BaseFailure = 80;
                    FirstCastExperience = 100;
                    break;

                case CharacterClass.Ranger:
                    Level = 25;
                    ManaCost = 30;
                    BaseFailure = 80;
                    FirstCastExperience = 125;
                    break;

                case CharacterClass.Paladin:
                    Level = 15;
                    ManaCost = 20;
                    BaseFailure = 80;
                    FirstCastExperience = 180;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 10;
                    ManaCost = 22;
                    BaseFailure = 80;
                    FirstCastExperience = 120;
                    break;

                case CharacterClass.HighMage:
                    Level = 8;
                    ManaCost = 15;
                    BaseFailure = 70;
                    FirstCastExperience = 180;
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
            return "dur 25+d25";
        }
    }
}