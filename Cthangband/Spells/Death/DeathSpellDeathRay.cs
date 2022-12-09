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

namespace Cthangband.Spells.Death
{
    [Serializable]
    internal class DeathSpellDeathRay : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetDirectionWithAim(out int dir))
            {
                return;
            }
            saveGame.SpellEffects.DeathRay(dir, player.Level);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Death Ray";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 20;
                    VrilCost = 20;
                    BaseFailure = 75;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.Priest:
                    Level = 25;
                    VrilCost = 25;
                    BaseFailure = 75;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.Rogue:
                    Level = 30;
                    VrilCost = 30;
                    BaseFailure = 80;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.Ranger:
                    Level = 35;
                    VrilCost = 35;
                    BaseFailure = 75;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.Paladin:
                    Level = 30;
                    VrilCost = 35;
                    BaseFailure = 75;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 24;
                    VrilCost = 24;
                    BaseFailure = 75;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.HighMage:
                    Level = 16;
                    VrilCost = 16;
                    BaseFailure = 65;
                    FirstCastExperience = 50;
                    break;

                default:
                    Level = 99;
                    VrilCost = 0;
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