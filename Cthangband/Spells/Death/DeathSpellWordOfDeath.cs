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

namespace Cthangband.Spells.Death
{
    [Serializable]
    internal class DeathSpellWordOfDeath : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.DispelLiving(player.Level * 3);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Word of Death";
            switch (characterClass)
            {
                case CharacterClassId.Mage:
                    Level = 33;
                    VisCost = 35;
                    BaseFailure = 70;
                    FirstCastExperience = 40;
                    break;

                case CharacterClassId.Priest:
                    Level = 40;
                    VisCost = 40;
                    BaseFailure = 70;
                    FirstCastExperience = 40;
                    break;

                case CharacterClassId.Rogue:
                    Level = 99;
                    VisCost = 0;
                    BaseFailure = 0;
                    FirstCastExperience = 0;
                    break;

                case CharacterClassId.Ranger:
                    Level = 48;
                    VisCost = 50;
                    BaseFailure = 30;
                    FirstCastExperience = 75;
                    break;

                case CharacterClassId.Paladin:
                    Level = 45;
                    VisCost = 45;
                    BaseFailure = 70;
                    FirstCastExperience = 40;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Cultist:
                    Level = 42;
                    VisCost = 50;
                    BaseFailure = 70;
                    FirstCastExperience = 40;
                    break;

                case CharacterClassId.HighMage:
                    Level = 29;
                    VisCost = 30;
                    BaseFailure = 60;
                    FirstCastExperience = 40;
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
            return $"dam {player.Level * 3}";
        }
    }
}