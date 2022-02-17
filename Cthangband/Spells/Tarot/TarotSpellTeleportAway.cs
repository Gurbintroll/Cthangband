using Cthangband.Enumerations;
using Cthangband.Projection;
using System;

namespace Cthangband.Spells.Tarot
{
    [Serializable]
    internal class TarotSpellTeleportAway : Spell
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
            Name = "Teleport Away";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 17;
                    ManaCost = 15;
                    BaseFailure = 60;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.Priest:
                case CharacterClass.Monk:
                    Level = 19;
                    ManaCost = 17;
                    BaseFailure = 60;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.Rogue:
                    Level = 21;
                    ManaCost = 20;
                    BaseFailure = 60;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.Ranger:
                    Level = 22;
                    ManaCost = 20;
                    BaseFailure = 60;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 20;
                    ManaCost = 18;
                    BaseFailure = 60;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.HighMage:
                    Level = 14;
                    ManaCost = 12;
                    BaseFailure = 50;
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
            return string.Empty;
        }
    }
}