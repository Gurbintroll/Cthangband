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

namespace Cthangband.Spells.Corporeal
{
    [Serializable]
    internal class CorporealSpellHeroism : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.SetTimedHeroism(player.TimedHeroism + Program.Rng.DieRoll(25) + 25);
            player.RestoreHealth(10);
            player.SetTimedFear(0);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Heroism";
            switch (characterClass)
            {
                case CharacterClassId.Mage:
                    Level = 10;
                    VisCost = 10;
                    BaseFailure = 40;
                    FirstCastExperience = 20;
                    break;

                case CharacterClassId.Priest:
                    Level = 13;
                    VisCost = 13;
                    BaseFailure = 40;
                    FirstCastExperience = 20;
                    break;

                case CharacterClassId.Ranger:
                    Level = 20;
                    VisCost = 20;
                    BaseFailure = 45;
                    FirstCastExperience = 5;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Monk:
                case CharacterClassId.Cultist:
                    Level = 12;
                    VisCost = 12;
                    BaseFailure = 40;
                    FirstCastExperience = 20;
                    break;

                case CharacterClassId.HighMage:
                    Level = 8;
                    VisCost = 8;
                    BaseFailure = 30;
                    FirstCastExperience = 20;
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
            return "dur 25+d25";
        }
    }
}