using Cthangband.Enumerations;
using Cthangband.Projection;
using System;

namespace Cthangband.Spells.Death
{
    [Serializable]
    internal class DeathSpellDarknessStorm : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetAimDir(out int dir))
            {
                return;
            }
            saveGame.SpellEffects.FireBall(new ProjectDark(SaveGame.Instance.SpellEffects), dir, 120, 4);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Darkness Storm";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 40;
                    ManaCost = 40;
                    BaseFailure = 70;
                    FirstCastExperience = 200;
                    break;

                case CharacterClass.Priest:
                    Level = 44;
                    ManaCost = 44;
                    BaseFailure = 80;
                    FirstCastExperience = 200;
                    break;

                case CharacterClass.Rogue:
                    Level = 50;
                    ManaCost = 50;
                    BaseFailure = 75;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.Ranger:
                    Level = 99;
                    ManaCost = 0;
                    BaseFailure = 0;
                    FirstCastExperience = 0;
                    break;

                case CharacterClass.Paladin:
                    Level = 48;
                    ManaCost = 50;
                    BaseFailure = 80;
                    FirstCastExperience = 200;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 45;
                    ManaCost = 50;
                    BaseFailure = 75;
                    FirstCastExperience = 200;
                    break;

                case CharacterClass.HighMage:
                    Level = 36;
                    ManaCost = 35;
                    BaseFailure = 60;
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
            return "dam 120";
        }
    }
}