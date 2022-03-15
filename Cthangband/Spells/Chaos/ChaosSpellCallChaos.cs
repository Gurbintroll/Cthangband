// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Chaos
{
    [Serializable]
    internal class ChaosSpellCallChaos : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.CallChaos();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Call Chaos";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 41;
                    ManaCost = 42;
                    BaseFailure = 85;
                    FirstCastExperience = 100;
                    break;

                case CharacterClass.Priest:
                    Level = 43;
                    ManaCost = 45;
                    BaseFailure = 85;
                    FirstCastExperience = 100;
                    break;

                case CharacterClass.Ranger:
                    Level = 48;
                    ManaCost = 48;
                    BaseFailure = 85;
                    FirstCastExperience = 100;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                    Level = 46;
                    ManaCost = 44;
                    BaseFailure = 85;
                    FirstCastExperience = 100;
                    break;

                case CharacterClass.Fanatic:
                    Level = 41;
                    ManaCost = 42;
                    BaseFailure = 85;
                    FirstCastExperience = 100;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Cultist:
                    Level = 36;
                    ManaCost = 36;
                    BaseFailure = 75;
                    FirstCastExperience = 100;
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
            return "dam 75 / 150";
        }
    }
}