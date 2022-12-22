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

namespace Cthangband.Spells.Folk
{
    [Serializable]
    internal class FolkSpellResistAcid : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.SetTimedAcidResistance(player.TimedColdResistance + Program.Rng.DieRoll(20) + 20);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Resist Acid";
            switch (characterClass)
            {
                case CharacterClassId.Mage:
                    Level = 15;
                    VisCost = 12;
                    BaseFailure = 50;
                    FirstCastExperience = 5;
                    break;

                case CharacterClassId.Priest:
                    Level = 17;
                    VisCost = 16;
                    BaseFailure = 50;
                    FirstCastExperience = 5;
                    break;

                case CharacterClassId.Rogue:
                    Level = 19;
                    VisCost = 18;
                    BaseFailure = 50;
                    FirstCastExperience = 5;
                    break;

                case CharacterClassId.Ranger:
                    Level = 19;
                    VisCost = 18;
                    BaseFailure = 50;
                    FirstCastExperience = 5;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Cultist:
                    Level = 18;
                    VisCost = 17;
                    BaseFailure = 50;
                    FirstCastExperience = 5;
                    break;

                case CharacterClassId.HighMage:
                    Level = 13;
                    VisCost = 10;
                    BaseFailure = 40;
                    FirstCastExperience = 5;
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
            return "dur 20+d20";
        }
    }
}