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
    internal class FolkSpellSeeInvisible : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.SetTimedSeeInvisibility(player.TimedSeeInvisibility + Program.Rng.DieRoll(24) + 24);
        }

        public override void Initialise(int characterClass)
        {
            Name = "See Invisible";
            switch (characterClass)
            {
                case CharacterClassId.Mage:
                    Level = 25;
                    VisCost = 20;
                    BaseFailure = 60;
                    FirstCastExperience = 13;
                    break;

                case CharacterClassId.Priest:
                    Level = 29;
                    VisCost = 26;
                    BaseFailure = 60;
                    FirstCastExperience = 13;
                    break;

                case CharacterClassId.Rogue:
                    Level = 33;
                    VisCost = 30;
                    BaseFailure = 60;
                    FirstCastExperience = 13;
                    break;

                case CharacterClassId.Ranger:
                    Level = 33;
                    VisCost = 30;
                    BaseFailure = 60;
                    FirstCastExperience = 13;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Cultist:
                    Level = 30;
                    VisCost = 27;
                    BaseFailure = 60;
                    FirstCastExperience = 13;
                    break;

                case CharacterClassId.HighMage:
                    Level = 22;
                    VisCost = 18;
                    BaseFailure = 50;
                    FirstCastExperience = 13;
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
            return "dur 24+d24";
        }
    }
}