using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Sorcery
{
    [Serializable]
    internal class SorcerySpellSlowMonster : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetAimDir(out int dir))
            {
                return;
            }
            saveGame.SpellEffects.SlowMonster(dir);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Slow Monster";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 11;
                    ManaCost = 7;
                    BaseFailure = 75;
                    FirstCastExperience = 7;
                    break;

                case CharacterClass.Rogue:
                    Level = 29;
                    ManaCost = 17;
                    BaseFailure = 75;
                    FirstCastExperience = 2;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 12;
                    ManaCost = 11;
                    BaseFailure = 75;
                    FirstCastExperience = 7;
                    break;

                case CharacterClass.HighMage:
                    Level = 9;
                    ManaCost = 5;
                    BaseFailure = 65;
                    FirstCastExperience = 7;
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