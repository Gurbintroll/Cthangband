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
                case CharacterClass.Mage:
                    Level = 25;
                    VrilCost = 20;
                    BaseFailure = 60;
                    FirstCastExperience = 13;
                    break;

                case CharacterClass.Priest:
                    Level = 29;
                    VrilCost = 26;
                    BaseFailure = 60;
                    FirstCastExperience = 13;
                    break;

                case CharacterClass.Rogue:
                    Level = 33;
                    VrilCost = 30;
                    BaseFailure = 60;
                    FirstCastExperience = 13;
                    break;

                case CharacterClass.Ranger:
                    Level = 33;
                    VrilCost = 30;
                    BaseFailure = 60;
                    FirstCastExperience = 13;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 30;
                    VrilCost = 27;
                    BaseFailure = 60;
                    FirstCastExperience = 13;
                    break;

                case CharacterClass.HighMage:
                    Level = 22;
                    VrilCost = 18;
                    BaseFailure = 50;
                    FirstCastExperience = 13;
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
            return "dur 24+d24";
        }
    }
}