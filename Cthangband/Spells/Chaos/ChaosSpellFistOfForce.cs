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
    internal class ChaosSpellFistOfForce : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetDirectionWithAim(out int dir))
            {
                return;
            }
            saveGame.SpellEffects.FireBall(new ProjectDisintegrate(SaveGame.Instance.SpellEffects), dir,
                Program.Rng.DiceRoll(8 + ((player.Level - 5) / 4), 8), 0);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Fist of Force";
            switch (characterClass)
            {
                case CharacterClassId.Mage:
                    Level = 14;
                    VisCost = 9;
                    BaseFailure = 45;
                    FirstCastExperience = 6;
                    break;

                case CharacterClassId.Priest:
                    Level = 16;
                    VisCost = 11;
                    BaseFailure = 50;
                    FirstCastExperience = 6;
                    break;

                case CharacterClassId.Ranger:
                    Level = 25;
                    VisCost = 21;
                    BaseFailure = 60;
                    FirstCastExperience = 3;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Monk:
                    Level = 17;
                    VisCost = 15;
                    BaseFailure = 45;
                    FirstCastExperience = 6;
                    break;

                case CharacterClassId.Fanatic:
                    Level = 15;
                    VisCost = 9;
                    BaseFailure = 45;
                    FirstCastExperience = 6;
                    break;

                case CharacterClassId.HighMage:
                case CharacterClassId.Cultist:
                    Level = 12;
                    VisCost = 6;
                    BaseFailure = 35;
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
            return $"dam {8 + ((player.Level - 5) / 4)}d8";
        }
    }
}