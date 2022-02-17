using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Death
{
    [Serializable]
    internal class DeathSpellDeathRay : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetAimDir(out int dir))
            {
                return;
            }
            saveGame.SpellEffects.DeathRay(dir, player.Level);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Death Ray";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 20;
                    ManaCost = 20;
                    BaseFailure = 75;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.Priest:
                    Level = 25;
                    ManaCost = 25;
                    BaseFailure = 75;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.Rogue:
                    Level = 30;
                    ManaCost = 30;
                    BaseFailure = 80;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.Ranger:
                    Level = 35;
                    ManaCost = 35;
                    BaseFailure = 75;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.Paladin:
                    Level = 30;
                    ManaCost = 35;
                    BaseFailure = 75;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 24;
                    ManaCost = 24;
                    BaseFailure = 75;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.HighMage:
                    Level = 16;
                    ManaCost = 16;
                    BaseFailure = 65;
                    FirstCastExperience = 50;
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
            return string.Empty;
        }
    }
}