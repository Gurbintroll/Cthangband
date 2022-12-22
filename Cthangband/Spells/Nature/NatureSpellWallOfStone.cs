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
    internal class NatureSpellWallOfStone : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.WallStone();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Wall of Stone";
            switch (characterClass)
            {
                case CharacterClassId.Mage:
                    Level = 38;
                    VisCost = 45;
                    BaseFailure = 75;
                    FirstCastExperience = 200;
                    break;

                case CharacterClassId.Priest:
                    Level = 40;
                    VisCost = 50;
                    BaseFailure = 85;
                    FirstCastExperience = 250;
                    break;

                case CharacterClassId.Ranger:
                    Level = 40;
                    VisCost = 55;
                    BaseFailure = 90;
                    FirstCastExperience = 250;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Cultist:
                    Level = 45;
                    VisCost = 48;
                    BaseFailure = 75;
                    FirstCastExperience = 200;
                    break;

                case CharacterClassId.HighMage:
                case CharacterClassId.Druid:
                    Level = 35;
                    VisCost = 44;
                    BaseFailure = 65;
                    FirstCastExperience = 200;
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