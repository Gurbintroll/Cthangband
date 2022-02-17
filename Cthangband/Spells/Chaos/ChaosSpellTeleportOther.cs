using Cthangband.Enumerations;
using Cthangband.Projection;
using System;

namespace Cthangband.Spells.Chaos
{
    [Serializable]
    internal class ChaosSpellTeleportOther : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetAimDir(out int dir))
            {
                return;
            }
            saveGame.SpellEffects.FireBeam(new ProjectAwayAll(SaveGame.Instance.SpellEffects), dir, player.Level);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Teleport Other";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 25;
                    ManaCost = 18;
                    BaseFailure = 60;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.Priest:
                    Level = 29;
                    ManaCost = 22;
                    BaseFailure = 60;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.Ranger:
                    Level = 39;
                    ManaCost = 29;
                    BaseFailure = 65;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                    Level = 37;
                    ManaCost = 35;
                    BaseFailure = 60;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.Fanatic:
                    Level = 33;
                    ManaCost = 24;
                    BaseFailure = 60;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Cultist:
                    Level = 23;
                    ManaCost = 15;
                    BaseFailure = 50;
                    FirstCastExperience = 8;
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