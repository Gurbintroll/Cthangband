// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Spells.Base;
using System;

namespace Cthangband.Spells.Nature
{
    [Serializable]
    internal class NatureSpellStairBuilding : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.StairCreation();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Stair Building";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 9;
                    VrilCost = 12;
                    BaseFailure = 40;
                    FirstCastExperience = 44;
                    break;

                case CharacterClass.Priest:
                    Level = 11;
                    VrilCost = 12;
                    BaseFailure = 40;
                    FirstCastExperience = 44;
                    break;

                case CharacterClass.Ranger:
                    Level = 12;
                    VrilCost = 12;
                    BaseFailure = 50;
                    FirstCastExperience = 25;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 12;
                    VrilCost = 12;
                    BaseFailure = 40;
                    FirstCastExperience = 44;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Druid:
                    Level = 7;
                    VrilCost = 7;
                    BaseFailure = 20;
                    FirstCastExperience = 44;
                    break;

                default:
                    Level = 99;
                    VrilCost = 0;
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