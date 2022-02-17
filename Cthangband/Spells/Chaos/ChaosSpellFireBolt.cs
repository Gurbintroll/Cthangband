using Cthangband.Enumerations;
using Cthangband.Projection;
using System;

namespace Cthangband.Spells.Chaos
{
    [Serializable]
    internal class ChaosSpellFireBolt : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            int beam;
            switch (player.ProfessionIndex)
            {
                case CharacterClass.Mage:
                    beam = player.Level;
                    break;

                case CharacterClass.HighMage:
                    beam = player.Level + 10;
                    break;

                default:
                    beam = player.Level / 2;
                    break;
            }
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetAimDir(out int dir))
            {
                return;
            }
            saveGame.SpellEffects.FireBoltOrBeam(beam, new ProjectFire(SaveGame.Instance.SpellEffects), dir,
                Program.Rng.DiceRoll(8 + ((player.Level - 5) / 4), 8));
        }

        public override void Initialise(int characterClass)
        {
            Name = "Fire Bolt";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 13;
                    ManaCost = 9;
                    BaseFailure = 45;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Priest:
                    Level = 11;
                    ManaCost = 6;
                    BaseFailure = 50;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.Ranger:
                    Level = 20;
                    ManaCost = 16;
                    BaseFailure = 50;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                    Level = 11;
                    ManaCost = 11;
                    BaseFailure = 45;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.Fanatic:
                    Level = 8;
                    ManaCost = 7;
                    BaseFailure = 45;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Cultist:
                    Level = 10;
                    ManaCost = 5;
                    BaseFailure = 35;
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
            return $"dam {6 + ((player.Level - 5) / 4)}d8";
        }
    }
}