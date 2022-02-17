using Cthangband.Enumerations;
using Cthangband.Projection;
using System;

namespace Cthangband.Spells.Death
{
    [Serializable]
    internal class DeathSpellHellfire : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetAimDir(out int dir))
            {
                return;
            }
            saveGame.SpellEffects.FireBall(new ProjectHellFire(SaveGame.Instance.SpellEffects), dir, 666, 3);
            player.TakeHit(50 + Program.Rng.DieRoll(50), "the strain of casting Hellfire");
        }

        public override void Initialise(int characterClass)
        {
            Name = "Hellfire";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 42;
                    ManaCost = 120;
                    BaseFailure = 95;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.Priest:
                    Level = 48;
                    ManaCost = 125;
                    BaseFailure = 95;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.Rogue:
                    Level = 99;
                    ManaCost = 0;
                    BaseFailure = 0;
                    FirstCastExperience = 0;
                    break;

                case CharacterClass.Ranger:
                    Level = 99;
                    ManaCost = 0;
                    BaseFailure = 0;
                    FirstCastExperience = 0;
                    break;

                case CharacterClass.Paladin:
                    Level = 50;
                    ManaCost = 150;
                    BaseFailure = 95;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 50;
                    ManaCost = 135;
                    BaseFailure = 95;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.HighMage:
                    Level = 39;
                    ManaCost = 100;
                    BaseFailure = 85;
                    FirstCastExperience = 250;
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
            return "dam 666";
        }
    }
}