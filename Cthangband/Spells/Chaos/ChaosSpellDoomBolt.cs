using Cthangband.Enumerations;
using Cthangband.Projection;
using System;

namespace Cthangband.Spells.Chaos
{
    [Serializable]
    internal class ChaosSpellDoomBolt : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetAimDir(out int dir))
            {
                return;
            }
            saveGame.SpellEffects.FireBeam(new ProjectMana(SaveGame.Instance.SpellEffects), dir,
                Program.Rng.DiceRoll(11 + ((player.Level - 5) / 4), 8));
        }

        public override void Initialise(int characterClass)
        {
            Name = "Doom Bolt";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 23;
                    ManaCost = 15;
                    BaseFailure = 50;
                    FirstCastExperience = 11;
                    break;

                case CharacterClass.Priest:
                    Level = 25;
                    ManaCost = 18;
                    BaseFailure = 50;
                    FirstCastExperience = 11;
                    break;

                case CharacterClass.Ranger:
                    Level = 35;
                    ManaCost = 31;
                    BaseFailure = 70;
                    FirstCastExperience = 10;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                    Level = 29;
                    ManaCost = 30;
                    BaseFailure = 50;
                    FirstCastExperience = 11;
                    break;

                case CharacterClass.Fanatic:
                    Level = 28;
                    ManaCost = 18;
                    BaseFailure = 50;
                    FirstCastExperience = 11;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Cultist:
                    Level = 21;
                    ManaCost = 12;
                    BaseFailure = 40;
                    FirstCastExperience = 11;
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
            return $"dam {11 + ((player.Level - 5) / 4)}d8";
        }
    }
}