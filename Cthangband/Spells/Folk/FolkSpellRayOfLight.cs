using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Folk
{
    [Serializable]
    internal class FolkSpellRayOfLight : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetAimDir(out int dir))
            {
                return;
            }
            Profile.Instance.MsgPrint("A line of light appears.");
            saveGame.SpellEffects.LightLine(dir);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Ray of Light";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 23;
                    ManaCost = 18;
                    BaseFailure = 60;
                    FirstCastExperience = 9;
                    break;

                case CharacterClass.Priest:
                    Level = 24;
                    ManaCost = 22;
                    BaseFailure = 60;
                    FirstCastExperience = 9;
                    break;

                case CharacterClass.Rogue:
                    Level = 27;
                    ManaCost = 26;
                    BaseFailure = 60;
                    FirstCastExperience = 9;
                    break;

                case CharacterClass.Ranger:
                    Level = 27;
                    ManaCost = 26;
                    BaseFailure = 60;
                    FirstCastExperience = 9;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 25;
                    ManaCost = 23;
                    BaseFailure = 60;
                    FirstCastExperience = 9;
                    break;

                case CharacterClass.HighMage:
                    Level = 20;
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
            return "dam 6d8";
        }
    }
}