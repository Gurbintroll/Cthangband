using Cthangband.Enumerations;
using Cthangband.Projection;
using System;

namespace Cthangband.Spells.Death
{
    [Serializable]
    internal class DeathSpellStinkingCloud : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetAimDir(out int dir))
            {
                return;
            }
            saveGame.SpellEffects.FireBall(new ProjectPois(SaveGame.Instance.SpellEffects), dir, 10 + (player.Level / 2), 2);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Stinking Cloud";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 3;
                    ManaCost = 3;
                    BaseFailure = 27;
                    FirstCastExperience = 3;
                    break;

                case CharacterClass.Priest:
                    Level = 5;
                    ManaCost = 4;
                    BaseFailure = 27;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Rogue:
                    Level = 13;
                    ManaCost = 7;
                    BaseFailure = 60;
                    FirstCastExperience = 1;
                    break;

                case CharacterClass.Ranger:
                    Level = 9;
                    ManaCost = 5;
                    BaseFailure = 40;
                    FirstCastExperience = 3;
                    break;

                case CharacterClass.Paladin:
                    Level = 6;
                    ManaCost = 5;
                    BaseFailure = 27;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 4;
                    ManaCost = 4;
                    BaseFailure = 27;
                    FirstCastExperience = 3;
                    break;

                case CharacterClass.HighMage:
                    Level = 2;
                    ManaCost = 2;
                    BaseFailure = 20;
                    FirstCastExperience = 3;
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
            return $"dam {10 + (player.Level / 2)}";
        }
    }
}