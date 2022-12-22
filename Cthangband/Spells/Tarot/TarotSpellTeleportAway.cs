﻿// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Projection;
using Cthangband.Spells.Base;
using System;

namespace Cthangband.Spells.Tarot
{
    [Serializable]
    internal class TarotSpellTeleportAway : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetDirectionWithAim(out int dir))
            {
                return;
            }
            saveGame.SpellEffects.FireBeam(new ProjectAwayAll(SaveGame.Instance.SpellEffects), dir, player.Level);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Teleport Away";
            switch (characterClass)
            {
                case CharacterClassId.Mage:
                    Level = 17;
                    VisCost = 15;
                    BaseFailure = 60;
                    FirstCastExperience = 5;
                    break;

                case CharacterClassId.Priest:
                case CharacterClassId.Monk:
                    Level = 19;
                    VisCost = 17;
                    BaseFailure = 60;
                    FirstCastExperience = 5;
                    break;

                case CharacterClassId.Rogue:
                    Level = 21;
                    VisCost = 20;
                    BaseFailure = 60;
                    FirstCastExperience = 5;
                    break;

                case CharacterClassId.Ranger:
                    Level = 22;
                    VisCost = 20;
                    BaseFailure = 60;
                    FirstCastExperience = 5;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Cultist:
                    Level = 20;
                    VisCost = 18;
                    BaseFailure = 60;
                    FirstCastExperience = 5;
                    break;

                case CharacterClassId.HighMage:
                    Level = 14;
                    VisCost = 12;
                    BaseFailure = 50;
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
            return string.Empty;
        }
    }
}