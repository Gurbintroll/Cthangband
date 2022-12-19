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
    internal class NatureSpellNatureAwareness : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            level.MapArea();
            saveGame.SpellEffects.DetectTraps();
            saveGame.SpellEffects.DetectDoors();
            saveGame.SpellEffects.DetectStairs();
            saveGame.SpellEffects.DetectMonstersNormal();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Nature Awareness";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 7;
                    VisCost = 6;
                    BaseFailure = 45;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Priest:
                    Level = 9;
                    VisCost = 10;
                    BaseFailure = 40;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Ranger:
                    Level = 11;
                    VisCost = 9;
                    BaseFailure = 40;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 12;
                    VisCost = 12;
                    BaseFailure = 45;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Druid:
                    Level = 5;
                    VisCost = 5;
                    BaseFailure = 35;
                    FirstCastExperience = 6;
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