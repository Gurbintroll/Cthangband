using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Sorcery
{
    [Serializable]
    internal class SorcerySpellConfuseMonster : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetAimDir(out int dir))
            {
                return;
            }
            SaveGame.Instance.SpellEffects.ConfuseMonster(dir, player.Level * 3 / 2);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Confuse Monster";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 4;
                    ManaCost = 4;
                    BaseFailure = 30;
                    FirstCastExperience = 1;
                    break;

                case CharacterClass.Rogue:
                    Level = 13;
                    ManaCost = 6;
                    BaseFailure = 75;
                    FirstCastExperience = 1;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 5;
                    ManaCost = 5;
                    BaseFailure = 30;
                    FirstCastExperience = 1;
                    break;

                case CharacterClass.HighMage:
                    Level = 3;
                    ManaCost = 3;
                    BaseFailure = 20;
                    FirstCastExperience = 1;
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