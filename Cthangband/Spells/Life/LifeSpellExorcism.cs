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

namespace Cthangband.Spells.Life
{
    [Serializable]
    internal class LifeSpellExorcism : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.DispelUndead(player.Level);
            saveGame.SpellEffects.DispelDemons(player.Level);
            saveGame.SpellEffects.TurnEvil(player.Level);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Exorcism";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 26;
                    VisCost = 30;
                    BaseFailure = 50;
                    FirstCastExperience = 75;
                    break;

                case CharacterClass.Priest:
                    Level = 15;
                    VisCost = 14;
                    BaseFailure = 50;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.Paladin:
                    Level = 25;
                    VisCost = 22;
                    BaseFailure = 50;
                    FirstCastExperience = 75;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 28;
                    VisCost = 28;
                    BaseFailure = 50;
                    FirstCastExperience = 75;
                    break;

                case CharacterClass.HighMage:
                    Level = 20;
                    VisCost = 20;
                    BaseFailure = 40;
                    FirstCastExperience = 75;
                    break;

                default:
                    Level = 99;
                    VisCost = 0;
                    BaseFailure = 0;
                    FirstCastExperience = 0;
                    break;
            }
        }

        protected override string Comment(Player player)
        {
            return $"dam {player.Level}+{player.Level}";
        }
    }
}