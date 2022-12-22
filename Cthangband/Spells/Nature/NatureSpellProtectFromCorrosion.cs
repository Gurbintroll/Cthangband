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

namespace Cthangband.Spells.Nature
{
    [Serializable]
    internal class NatureSpellProtectFromCorrosion : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.CommandEngine.Rustproof();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Protection from Corrosion";
            switch (characterClass)
            {
                case CharacterClassId.Mage:
                    Level = 40;
                    VisCost = 90;
                    BaseFailure = 90;
                    FirstCastExperience = 250;
                    break;

                case CharacterClassId.Priest:
                    Level = 42;
                    VisCost = 90;
                    BaseFailure = 90;
                    FirstCastExperience = 250;
                    break;

                case CharacterClassId.Ranger:
                    Level = 42;
                    VisCost = 80;
                    BaseFailure = 90;
                    FirstCastExperience = 250;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Cultist:
                    Level = 49;
                    VisCost = 95;
                    BaseFailure = 90;
                    FirstCastExperience = 250;
                    break;

                case CharacterClassId.HighMage:
                case CharacterClassId.Druid:
                    Level = 37;
                    VisCost = 65;
                    BaseFailure = 80;
                    FirstCastExperience = 250;
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