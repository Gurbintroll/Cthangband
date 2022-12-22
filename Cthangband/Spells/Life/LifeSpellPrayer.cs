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
    internal class LifeSpellPrayer : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.SetTimedBlessing(player.TimedBlessing + Program.Rng.DieRoll(48) + 48);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Prayer";
            switch (characterClass)
            {
                case CharacterClassId.Mage:
                    Level = 25;
                    VisCost = 25;
                    BaseFailure = 50;
                    FirstCastExperience = 50;
                    break;

                case CharacterClassId.Priest:
                    Level = 15;
                    VisCost = 14;
                    BaseFailure = 50;
                    FirstCastExperience = 100;
                    break;

                case CharacterClassId.Paladin:
                    Level = 25;
                    VisCost = 20;
                    BaseFailure = 50;
                    FirstCastExperience = 50;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Cultist:
                    Level = 28;
                    VisCost = 28;
                    BaseFailure = 50;
                    FirstCastExperience = 50;
                    break;

                case CharacterClassId.HighMage:
                    Level = 20;
                    VisCost = 20;
                    BaseFailure = 40;
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
            return "dur 48+d48";
        }
    }
}