﻿// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Spells.Base;
using Cthangband.StaticData;
using System;

namespace Cthangband.Spells.Death
{
    [Serializable]
    internal class DeathSpellVampiricDrain : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetDirectionWithAim(out int dir))
            {
                return;
            }
            int dummy = player.Level + (Program.Rng.DieRoll(player.Level) * Math.Max(1, player.Level / 10));
            if (!SaveGame.Instance.SpellEffects.DrainLife(dir, dummy))
            {
                return;
            }
            player.RestoreHealth(dummy);
            dummy = player.Food + Math.Min(5000, 100 * dummy);
            if (player.Food < Constants.PyFoodMax)
            {
                player.SetFood(dummy >= Constants.PyFoodMax ? Constants.PyFoodMax - 1 : dummy);
            }
        }

        public override void Initialise(int characterClass)
        {
            Name = "Vampiric Drain";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 23;
                    VrilCost = 20;
                    BaseFailure = 60;
                    FirstCastExperience = 16;
                    break;

                case CharacterClass.Priest:
                    Level = 25;
                    VrilCost = 24;
                    BaseFailure = 60;
                    FirstCastExperience = 16;
                    break;

                case CharacterClass.Rogue:
                    Level = 30;
                    VrilCost = 30;
                    BaseFailure = 75;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Ranger:
                    Level = 30;
                    VrilCost = 30;
                    BaseFailure = 80;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.Paladin:
                    Level = 28;
                    VrilCost = 26;
                    BaseFailure = 60;
                    FirstCastExperience = 16;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 25;
                    VrilCost = 25;
                    BaseFailure = 60;
                    FirstCastExperience = 16;
                    break;

                case CharacterClass.HighMage:
                    Level = 20;
                    VrilCost = 16;
                    BaseFailure = 50;
                    FirstCastExperience = 16;
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
            return $"dam {Math.Max(1, player.Level / 10)}d{player.Level}+{player.Level}";
        }
    }
}