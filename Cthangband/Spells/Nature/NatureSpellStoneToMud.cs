using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Nature
{
    [Serializable]
    internal class NatureSpellStoneToMud : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetAimDir(out int dir))
            {
                return;
            }
            saveGame.SpellEffects.WallToMud(dir);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Stone to Mud";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 5;
                    ManaCost = 5;
                    BaseFailure = 40;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Priest:
                    Level = 7;
                    ManaCost = 7;
                    BaseFailure = 40;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Ranger:
                    Level = 9;
                    ManaCost = 7;
                    BaseFailure = 80;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 10;
                    ManaCost = 10;
                    BaseFailure = 40;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Druid:
                    Level = 5;
                    ManaCost = 4;
                    BaseFailure = 30;
                    FirstCastExperience = 6;
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