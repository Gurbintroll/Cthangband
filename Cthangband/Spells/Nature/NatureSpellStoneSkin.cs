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
    internal class NatureSpellStoneSkin : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.SetTimedStoneskin(player.TimedStoneskin + Program.Rng.DieRoll(20) + 30);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Stone Skin";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 10;
                    ManaCost = 12;
                    BaseFailure = 75;
                    FirstCastExperience = 120;
                    break;

                case CharacterClass.Priest:
                    Level = 12;
                    ManaCost = 13;
                    BaseFailure = 75;
                    FirstCastExperience = 120;
                    break;

                case CharacterClass.Ranger:
                    Level = 14;
                    ManaCost = 15;
                    BaseFailure = 70;
                    FirstCastExperience = 60;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 15;
                    ManaCost = 15;
                    BaseFailure = 75;
                    FirstCastExperience = 120;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Druid:
                    Level = 8;
                    ManaCost = 8;
                    BaseFailure = 65;
                    FirstCastExperience = 120;
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
            return "dur 20+d30";
        }
    }
}