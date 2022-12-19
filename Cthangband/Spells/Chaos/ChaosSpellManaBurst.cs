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
    internal class ChaosSpellVrilBurst : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetDirectionWithAim(out int dir))
            {
                return;
            }
            SaveGame.Instance.SpellEffects.FireBall(new ProjectMissile(SaveGame.Instance.SpellEffects), dir,
                Program.Rng.DiceRoll(3, 5) + player.Level + (player.Level /
                (player.ProfessionIndex == CharacterClass.Mage || player.ProfessionIndex == CharacterClass.HighMage ? 2 : 4)),
                player.Level < 30 ? 2 : 3);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Vis Burst";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 9;
                    VrilCost = 6;
                    BaseFailure = 50;
                    FirstCastExperience = 1;
                    break;

                case CharacterClass.Priest:
                    Level = 10;
                    VrilCost = 6;
                    BaseFailure = 30;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.Ranger:
                    Level = 14;
                    VrilCost = 12;
                    BaseFailure = 40;
                    FirstCastExperience = 2;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                    Level = 8;
                    VrilCost = 8;
                    BaseFailure = 30;
                    FirstCastExperience = 1;
                    break;

                case CharacterClass.Fanatic:
                    Level = 7;
                    VrilCost = 7;
                    BaseFailure = 30;
                    FirstCastExperience = 1;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Cultist:
                    Level = 6;
                    VrilCost = 4;
                    BaseFailure = 40;
                    FirstCastExperience = 1;
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
            int i = player.Level + (player.Level /
                    (player.ProfessionIndex == CharacterClass.Mage || player.ProfessionIndex == CharacterClass.HighMage ? 2 : 4));
            return $"dam 3d5+{i}";
        }
    }
}