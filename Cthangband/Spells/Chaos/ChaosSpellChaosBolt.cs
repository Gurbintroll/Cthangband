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
            switch (player.ProfessionIndex)
            {
                case CharacterClass.Mage:
                    beam = player.Level;
                    break;

                case CharacterClass.HighMage:
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
                case CharacterClass.Mage:
                    Level = 19;
                    VrilCost = 12;
                    BaseFailure = 45;
                    FirstCastExperience = 9;
                    break;

                case CharacterClass.Priest:
                    Level = 21;
                    VrilCost = 16;
                    BaseFailure = 50;
                    FirstCastExperience = 9;
                    break;

                case CharacterClass.Ranger:
                    Level = 30;
                    VrilCost = 25;
                    BaseFailure = 60;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                    Level = 23;
                    VrilCost = 22;
                    BaseFailure = 45;
                    FirstCastExperience = 9;
                    break;

                case CharacterClass.Fanatic:
                    Level = 22;
                    VrilCost = 14;
                    BaseFailure = 45;
                    FirstCastExperience = 9;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Cultist:
                    Level = 17;
                    VrilCost = 10;
                    BaseFailure = 35;
                    FirstCastExperience = 9;
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
            return $"dam {10 + ((player.Level - 5) / 4)}d8";
        }
    }
}