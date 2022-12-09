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

namespace Cthangband.Spells.Chaos
{
    [Serializable]
    internal class ChaosSpellCallTheVoid : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.CommandEngine.CallTheVoid();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Call the Void";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 49;
                    VrilCost = 100;
                    BaseFailure = 85;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.Priest:
                    Level = 50;
                    VrilCost = 100;
                    BaseFailure = 95;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.Ranger:
                    Level = 99;
                    VrilCost = 0;
                    BaseFailure = 0;
                    FirstCastExperience = 0;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                    Level = 50;
                    VrilCost = 100;
                    BaseFailure = 85;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.Fanatic:
                    Level = 49;
                    VrilCost = 100;
                    BaseFailure = 85;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Cultist:
                    Level = 46;
                    VrilCost = 90;
                    BaseFailure = 75;
                    FirstCastExperience = 250;
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
            return "dam 3 * 175";
        }
    }
}