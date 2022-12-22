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

namespace Cthangband.Spells.Chaos
{
    [Serializable]
    internal class ChaosSpellTeleportSelf : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.TeleportPlayer(player.Level * 5);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Teleport Self";
            switch (characterClass)
            {
                case CharacterClassId.Mage:
                    Level = 15;
                    VisCost = 9;
                    BaseFailure = 35;
                    FirstCastExperience = 5;
                    break;

                case CharacterClassId.Priest:
                    Level = 17;
                    VisCost = 11;
                    BaseFailure = 35;
                    FirstCastExperience = 5;
                    break;

                case CharacterClassId.Ranger:
                    Level = 25;
                    VisCost = 22;
                    BaseFailure = 60;
                    FirstCastExperience = 3;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Monk:
                    Level = 18;
                    VisCost = 17;
                    BaseFailure = 35;
                    FirstCastExperience = 5;
                    break;

                case CharacterClassId.Fanatic:
                    Level = 16;
                    VisCost = 10;
                    BaseFailure = 35;
                    FirstCastExperience = 5;
                    break;

                case CharacterClassId.HighMage:
                case CharacterClassId.Cultist:
                    Level = 14;
                    VisCost = 7;
                    BaseFailure = 25;
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
            return $"range {player.Level * 5}";
        }
    }
}