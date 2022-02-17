using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Death
{
    [Serializable]
    internal class DeathSpellEnslaveUndead : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetAimDir(out int dir))
            {
                return;
            }
            saveGame.SpellEffects.ControlOneUndead(dir, player.Level);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Enslave Undead";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 10;
                    ManaCost = 10;
                    BaseFailure = 30;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Priest:
                    Level = 12;
                    ManaCost = 12;
                    BaseFailure = 40;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.Rogue:
                    Level = 19;
                    ManaCost = 19;
                    BaseFailure = 30;
                    FirstCastExperience = 1;
                    break;

                case CharacterClass.Ranger:
                    Level = 22;
                    ManaCost = 22;
                    BaseFailure = 50;
                    FirstCastExperience = 3;
                    break;

                case CharacterClass.Paladin:
                    Level = 15;
                    ManaCost = 15;
                    BaseFailure = 40;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 12;
                    ManaCost = 12;
                    BaseFailure = 30;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.HighMage:
                    Level = 8;
                    ManaCost = 8;
                    BaseFailure = 20;
                    FirstCastExperience = 4;
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