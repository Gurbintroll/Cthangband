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
    internal class TarotSpellMindBlast : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            int beam;
            switch (player.CharacterClassIndex)
            {
                case CharacterClassId.Mage:
                    beam = player.Level;
                    break;

                case CharacterClassId.HighMage:
                    beam = player.Level + 10;
                    break;

                default:
                    beam = player.Level / 2;
                    break;
            }
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetDirectionWithAim(out int dir))
            {
                return;
            }
            saveGame.SpellEffects.FireBoltOrBeam(beam - 10, new ProjectPsi(SaveGame.Instance.SpellEffects), dir,
                Program.Rng.DiceRoll(3 + ((player.Level - 1) / 5), 3));
        }

        public override void Initialise(int characterClass)
        {
            Name = "Mind Blast";
            switch (characterClass)
            {
                case CharacterClassId.Mage:
                    Level = 3;
                    VisCost = 3;
                    BaseFailure = 50;
                    FirstCastExperience = 4;
                    break;

                case CharacterClassId.Priest:
                case CharacterClassId.Monk:
                    Level = 4;
                    VisCost = 4;
                    BaseFailure = 50;
                    FirstCastExperience = 4;
                    break;

                case CharacterClassId.Rogue:
                    Level = 7;
                    VisCost = 5;
                    BaseFailure = 50;
                    FirstCastExperience = 4;
                    break;

                case CharacterClassId.Ranger:
                    Level = 6;
                    VisCost = 6;
                    BaseFailure = 50;
                    FirstCastExperience = 4;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Cultist:
                    Level = 5;
                    VisCost = 5;
                    BaseFailure = 50;
                    FirstCastExperience = 4;
                    break;

                case CharacterClassId.HighMage:
                    Level = 2;
                    VisCost = 2;
                    BaseFailure = 40;
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
            return $"dam {3 + ((player.Level - 1) / 5)}d3";
        }
    }
}