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

namespace Cthangband.Spells.Chaos
{
    [Serializable]
    internal class ChaosSpellTeleportOther : BaseSpell
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
            Name = "Teleport Other";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 25;
                    VisCost = 18;
                    BaseFailure = 60;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.Priest:
                    Level = 29;
                    VisCost = 22;
                    BaseFailure = 60;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.Ranger:
                    Level = 39;
                    VisCost = 29;
                    BaseFailure = 65;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                    Level = 37;
                    VisCost = 35;
                    BaseFailure = 60;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.Fanatic:
                    Level = 33;
                    VisCost = 24;
                    BaseFailure = 60;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Cultist:
                    Level = 23;
                    VisCost = 15;
                    BaseFailure = 50;
                    FirstCastExperience = 8;
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