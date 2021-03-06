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
    internal class ChaosSpellArcaneBinding : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.Recharge(40);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Arcane Binding";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 16;
                    ManaCost = 14;
                    BaseFailure = 80;
                    FirstCastExperience = 35;
                    break;

                case CharacterClass.Priest:
                    Level = 20;
                    ManaCost = 18;
                    BaseFailure = 80;
                    FirstCastExperience = 35;
                    break;

                case CharacterClass.Ranger:
                    Level = 28;
                    ManaCost = 25;
                    BaseFailure = 80;
                    FirstCastExperience = 45;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                    Level = 20;
                    ManaCost = 18;
                    BaseFailure = 80;
                    FirstCastExperience = 35;
                    break;

                case CharacterClass.Fanatic:
                    Level = 16;
                    ManaCost = 15;
                    BaseFailure = 80;
                    FirstCastExperience = 35;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Cultist:
                    Level = 14;
                    ManaCost = 12;
                    BaseFailure = 70;
                    FirstCastExperience = 35;
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