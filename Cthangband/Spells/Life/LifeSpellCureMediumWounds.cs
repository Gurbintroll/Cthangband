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
    internal class LifeSpellCureMediumWounds : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.RestoreHealth(Program.Rng.DiceRoll(4, 10));
            player.SetTimedBleeding((player.TimedBleeding / 2) - 20);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Cure Medium Wounds";
            switch (characterClass)
            {
                case CharacterClassId.Mage:
                    Level = 12;
                    VisCost = 12;
                    BaseFailure = 40;
                    FirstCastExperience = 3;
                    break;

                case CharacterClassId.Priest:
                    Level = 5;
                    VisCost = 4;
                    BaseFailure = 32;
                    FirstCastExperience = 4;
                    break;

                case CharacterClassId.Paladin:
                    Level = 11;
                    VisCost = 9;
                    BaseFailure = 40;
                    FirstCastExperience = 3;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Cultist:
                    Level = 14;
                    VisCost = 14;
                    BaseFailure = 40;
                    FirstCastExperience = 3;
                    break;

                case CharacterClassId.HighMage:
                    Level = 9;
                    VisCost = 9;
                    BaseFailure = 30;
                    FirstCastExperience = 3;
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
            return "heal 4d10";
        }
    }
}