﻿// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
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
    internal class LifeSpellCallLight : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.LightArea(Program.Rng.DiceRoll(2, player.Level / 2), (player.Level / 10) + 1);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Call Light";
            switch (characterClass)
            {
                case CharacterClassId.Mage:
                    Level = 7;
                    VisCost = 7;
                    BaseFailure = 35;
                    FirstCastExperience = 4;
                    break;

                case CharacterClassId.Priest:
                    Level = 3;
                    VisCost = 3;
                    BaseFailure = 27;
                    FirstCastExperience = 2;
                    break;

                case CharacterClassId.Paladin:
                    Level = 5;
                    VisCost = 4;
                    BaseFailure = 35;
                    FirstCastExperience = 4;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Cultist:
                    Level = 8;
                    VisCost = 8;
                    BaseFailure = 35;
                    FirstCastExperience = 4;
                    break;

                case CharacterClassId.HighMage:
                    Level = 5;
                    VisCost = 5;
                    BaseFailure = 25;
                    FirstCastExperience = 4;
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
            return $"dam {10 + (player.Level / 2)}";
        }
    }
}