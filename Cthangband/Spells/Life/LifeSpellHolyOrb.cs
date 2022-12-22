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

namespace Cthangband.Spells.Life
{
    [Serializable]
    internal class LifeSpellHolyOrb : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetDirectionWithAim(out int dir))
            {
                return;
            }
            SaveGame.Instance.SpellEffects.FireBall(new ProjectHolyFire(SaveGame.Instance.SpellEffects), dir,
                Program.Rng.DiceRoll(3, 6) + player.Level + (player.Level /
                (player.CharacterClassIndex == CharacterClassId.Priest || player.CharacterClassIndex == CharacterClassId.HighMage ? 2 : 4)),
                player.Level < 30 ? 2 : 3);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Holy Orb";
            switch (characterClass)
            {
                case CharacterClassId.Mage:
                    Level = 20;
                    VisCost = 20;
                    BaseFailure = 50;
                    FirstCastExperience = 4;
                    break;

                case CharacterClassId.Priest:
                    Level = 10;
                    VisCost = 8;
                    BaseFailure = 40;
                    FirstCastExperience = 4;
                    break;

                case CharacterClassId.Paladin:
                    Level = 18;
                    VisCost = 15;
                    BaseFailure = 50;
                    FirstCastExperience = 4;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Cultist:
                    Level = 26;
                    VisCost = 26;
                    BaseFailure = 50;
                    FirstCastExperience = 4;
                    break;

                case CharacterClassId.HighMage:
                    Level = 19;
                    VisCost = 17;
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
            int orb = player.Level / (player.CharacterClassIndex == CharacterClassId.Priest || player.CharacterClassIndex == CharacterClassId.HighMage
                          ? 2
                          : 4);
            return $" dam 3d6+{player.Level + orb}";
        }
    }
}