using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Nature
{
    [Serializable]
    internal class NatureSpellRayOfSunlight : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetAimDir(out int dir))
            {
                return;
            }
            Profile.Instance.MsgPrint("A line of sunlight appears.");
            saveGame.SpellEffects.LightLine(dir);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Ray of Sunlight";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 9;
                    ManaCost = 6;
                    BaseFailure = 30;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Priest:
                    Level = 11;
                    ManaCost = 11;
                    BaseFailure = 30;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Ranger:
                    Level = 14;
                    ManaCost = 9;
                    BaseFailure = 55;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 14;
                    ManaCost = 14;
                    BaseFailure = 30;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Druid:
                    Level = 7;
                    ManaCost = 5;
                    BaseFailure = 30;
                    FirstCastExperience = 5;
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
            return "dam 6d8";
        }
    }
}