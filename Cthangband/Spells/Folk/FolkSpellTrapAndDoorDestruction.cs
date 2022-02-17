using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Folk
{
    [Serializable]
    internal class FolkSpellTrapAndDoorDestruction : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetAimDir(out int dir))
            {
                return;
            }
            saveGame.SpellEffects.DestroyDoor(dir);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Trap & Door Destruction";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 5;
                    ManaCost = 5;
                    BaseFailure = 33;
                    FirstCastExperience = 7;
                    break;

                case CharacterClass.Priest:
                    Level = 6;
                    ManaCost = 6;
                    BaseFailure = 33;
                    FirstCastExperience = 7;
                    break;

                case CharacterClass.Rogue:
                    Level = 8;
                    ManaCost = 7;
                    BaseFailure = 33;
                    FirstCastExperience = 7;
                    break;

                case CharacterClass.Ranger:
                    Level = 7;
                    ManaCost = 7;
                    BaseFailure = 33;
                    FirstCastExperience = 7;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 6;
                    ManaCost = 6;
                    BaseFailure = 33;
                    FirstCastExperience = 7;
                    break;

                case CharacterClass.HighMage:
                    Level = 4;
                    ManaCost = 4;
                    BaseFailure = 23;
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