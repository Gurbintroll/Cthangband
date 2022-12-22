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

namespace Cthangband.Spells.Folk
{
    [Serializable]
    internal class FolkSpellDetectObjects : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.DetectObjectsNormal();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Detect Objects";
            switch (characterClass)
            {
                case CharacterClassId.Mage:
                    Level = 9;
                    VisCost = 9;
                    BaseFailure = 50;
                    FirstCastExperience = 6;
                    break;

                case CharacterClassId.Priest:
                    Level = 12;
                    VisCost = 11;
                    BaseFailure = 50;
                    FirstCastExperience = 6;
                    break;

                case CharacterClassId.Rogue:
                    Level = 14;
                    VisCost = 13;
                    BaseFailure = 50;
                    FirstCastExperience = 6;
                    break;

                case CharacterClassId.Ranger:
                    Level = 14;
                    VisCost = 13;
                    BaseFailure = 50;
                    FirstCastExperience = 6;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Cultist:
                    Level = 13;
                    VisCost = 12;
                    BaseFailure = 50;
                    FirstCastExperience = 6;
                    break;

                case CharacterClassId.HighMage:
                    Level = 9;
                    VisCost = 8;
                    BaseFailure = 40;
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