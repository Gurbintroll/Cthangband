using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Folk
{
    [Serializable]
    internal class FolkSpellStoneToMud : Spell
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
                    Level = 20;
                    ManaCost = 16;
                    BaseFailure = 60;
                    FirstCastExperience = 9;
                    break;

                case CharacterClass.Priest:
                    Level = 22;
                    ManaCost = 20;
                    BaseFailure = 60;
                    FirstCastExperience = 9;
                    break;

                case CharacterClass.Rogue:
                    Level = 25;
                    ManaCost = 23;
                    BaseFailure = 60;
                    FirstCastExperience = 9;
                    break;

                case CharacterClass.Ranger:
                    Level = 25;
                    ManaCost = 23;
                    BaseFailure = 60;
                    FirstCastExperience = 9;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 23;
                    ManaCost = 22;
                    BaseFailure = 60;
                    FirstCastExperience = 9;
                    break;

                case CharacterClass.HighMage:
                    Level = 17;
                    ManaCost = 15;
                    BaseFailure = 50;
                    FirstCastExperience = 9;
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