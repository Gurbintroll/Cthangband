// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
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
    internal class ChaosSpellChaosBolt : BaseSpell
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
            saveGame.SpellEffects.FireBoltOrBeam(beam, new ProjectChaos(SaveGame.Instance.SpellEffects), dir,
                Program.Rng.DiceRoll(10 + ((player.Level - 5) / 4), 8));
        }

        public override void Initialise(int characterClass)
        {
            Name = "Chaos Bolt";
            switch (characterClass)
            {
                case CharacterClassId.Mage:
                    Level = 19;
                    VisCost = 12;
                    BaseFailure = 45;
                    FirstCastExperience = 9;
                    break;

                case CharacterClassId.Priest:
                    Level = 21;
                    VisCost = 16;
                    BaseFailure = 50;
                    FirstCastExperience = 9;
                    break;

                case CharacterClassId.Ranger:
                    Level = 30;
                    VisCost = 25;
                    BaseFailure = 60;
                    FirstCastExperience = 8;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Monk:
                    Level = 23;
                    VisCost = 22;
                    BaseFailure = 45;
                    FirstCastExperience = 9;
                    break;

                case CharacterClassId.Fanatic:
                    Level = 22;
                    VisCost = 14;
                    BaseFailure = 45;
                    FirstCastExperience = 9;
                    break;

                case CharacterClassId.HighMage:
                case CharacterClassId.Cultist:
                    Level = 17;
                    VisCost = 10;
                    BaseFailure = 35;
                    FirstCastExperience = 9;
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
            return $"dam {10 + ((player.Level - 5) / 4)}d8";
        }
    }
}