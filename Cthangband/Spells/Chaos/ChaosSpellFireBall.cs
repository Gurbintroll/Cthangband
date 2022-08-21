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
    internal class ChaosSpellFireBall : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetDirectionWithAim(out int dir))
            {
                return;
            }
            saveGame.SpellEffects.FireBall(new ProjectFire(SaveGame.Instance.SpellEffects), dir, 55 + player.Level, 2);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Fire Ball";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 25;
                    ManaCost = 16;
                    BaseFailure = 50;
                    FirstCastExperience = 12;
                    break;

                case CharacterClass.Priest:
                    Level = 27;
                    ManaCost = 20;
                    BaseFailure = 65;
                    FirstCastExperience = 12;
                    break;

                case CharacterClass.Ranger:
                    Level = 37;
                    ManaCost = 35;
                    BaseFailure = 75;
                    FirstCastExperience = 15;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                    Level = 33;
                    ManaCost = 33;
                    BaseFailure = 50;
                    FirstCastExperience = 12;
                    break;

                case CharacterClass.Fanatic:
                    Level = 30;
                    ManaCost = 20;
                    BaseFailure = 50;
                    FirstCastExperience = 12;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Cultist:
                    Level = 22;
                    ManaCost = 13;
                    BaseFailure = 40;
                    FirstCastExperience = 12;
                    break;

                default:
                    Level = 99;
                    ManaCost = 0;
                    BaseFailure = 0;
                    FirstCastExperience = 0;
                    break;
            }
        }

        protected override string Comment(Player player)
        {
            return $"dam {55 + player.Level}";
        }
    }
}