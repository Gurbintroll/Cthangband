// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Projection;
using System;

namespace Cthangband.Spells.Chaos
{
    [Serializable]
    internal class ChaosSpellManaStorm : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetDirectionWithAim(out int dir))
            {
                return;
            }
            saveGame.SpellEffects.FireBall(new ProjectMana(SaveGame.Instance.SpellEffects), dir, 300 + (player.Level * 2), 4);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Mana Storm";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 45;
                    ManaCost = 48;
                    BaseFailure = 85;
                    FirstCastExperience = 200;
                    break;

                case CharacterClass.Priest:
                    Level = 47;
                    ManaCost = 50;
                    BaseFailure = 95;
                    FirstCastExperience = 200;
                    break;

                case CharacterClass.Ranger:
                    Level = 99;
                    ManaCost = 0;
                    BaseFailure = 0;
                    FirstCastExperience = 0;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                    Level = 49;
                    ManaCost = 50;
                    BaseFailure = 85;
                    FirstCastExperience = 200;
                    break;

                case CharacterClass.Fanatic:
                    Level = 45;
                    ManaCost = 48;
                    BaseFailure = 85;
                    FirstCastExperience = 200;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Cultist:
                    Level = 40;
                    ManaCost = 45;
                    BaseFailure = 75;
                    FirstCastExperience = 200;
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
            return $"dam {300 + (player.Level * 2)}";
        }
    }
}