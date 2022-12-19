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
    internal class DeathSpellTerror : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.TurnMonsters(30 + player.Level);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Terror";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 18;
                    VisCost = 15;
                    BaseFailure = 50;
                    FirstCastExperience = 10;
                    break;

                case CharacterClass.Priest:
                    Level = 21;
                    VisCost = 20;
                    BaseFailure = 50;
                    FirstCastExperience = 10;
                    break;

                case CharacterClass.Rogue:
                    Level = 27;
                    VisCost = 25;
                    BaseFailure = 75;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Ranger:
                    Level = 28;
                    VisCost = 28;
                    BaseFailure = 75;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Paladin:
                    Level = 23;
                    VisCost = 23;
                    BaseFailure = 50;
                    FirstCastExperience = 10;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 21;
                    VisCost = 21;
                    BaseFailure = 50;
                    FirstCastExperience = 10;
                    break;

                case CharacterClass.HighMage:
                    Level = 14;
                    VisCost = 12;
                    BaseFailure = 40;
                    FirstCastExperience = 10;
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
            return string.Empty;
        }
    }
}