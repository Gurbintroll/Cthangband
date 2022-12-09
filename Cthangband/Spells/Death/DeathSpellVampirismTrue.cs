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
    internal class DeathSpellVampirismTrue : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetDirectionWithAim(out int dir))
            {
                return;
            }
            for (int dummy = 0; dummy < 3; dummy++)
            {
                if (saveGame.SpellEffects.DrainLife(dir, 100))
                {
                    player.RestoreHealth(100);
                }
            }
        }

        public override void Initialise(int characterClass)
        {
            Name = "Vampirism True";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 33;
                    VrilCost = 35;
                    BaseFailure = 60;
                    FirstCastExperience = 125;
                    break;

                case CharacterClass.Priest:
                    Level = 35;
                    VrilCost = 35;
                    BaseFailure = 60;
                    FirstCastExperience = 125;
                    break;

                case CharacterClass.Rogue:
                    Level = 46;
                    VrilCost = 45;
                    BaseFailure = 75;
                    FirstCastExperience = 40;
                    break;

                case CharacterClass.Ranger:
                    Level = 45;
                    VrilCost = 45;
                    BaseFailure = 80;
                    FirstCastExperience = 100;
                    break;

                case CharacterClass.Paladin:
                    Level = 40;
                    VrilCost = 40;
                    BaseFailure = 60;
                    FirstCastExperience = 125;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 40;
                    VrilCost = 40;
                    BaseFailure = 60;
                    FirstCastExperience = 125;
                    break;

                case CharacterClass.HighMage:
                    Level = 30;
                    VrilCost = 30;
                    BaseFailure = 50;
                    FirstCastExperience = 125;
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
            return "dam 3*100";
        }
    }
}