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
    internal class NatureSpellWallOfStone : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.WallStone();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Wall of Stone";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 38;
                    ManaCost = 45;
                    BaseFailure = 75;
                    FirstCastExperience = 200;
                    break;

                case CharacterClass.Priest:
                    Level = 40;
                    ManaCost = 50;
                    BaseFailure = 85;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.Ranger:
                    Level = 40;
                    ManaCost = 55;
                    BaseFailure = 90;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 45;
                    ManaCost = 48;
                    BaseFailure = 75;
                    FirstCastExperience = 200;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Druid:
                    Level = 35;
                    ManaCost = 44;
                    BaseFailure = 65;
                    FirstCastExperience = 200;
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
            return string.Empty;
        }
    }
}