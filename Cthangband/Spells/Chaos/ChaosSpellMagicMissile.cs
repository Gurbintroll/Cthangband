using Cthangband.Enumerations;
using Cthangband.Projection;
using System;

namespace Cthangband.Spells.Chaos
{
    [Serializable]
    internal class ChaosSpellMagicMissile : Spell
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
            saveGame.SpellEffects.FireBoltOrBeam(beam - 10, new ProjectMissile(SaveGame.Instance.SpellEffects), dir,
                Program.Rng.DiceRoll(3 + ((player.Level - 1) / 5), 4));
        }

        public override void Initialise(int characterClass)
        {
            Name = "Magic Missile";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 1;
                    ManaCost = 1;
                    BaseFailure = 20;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Priest:
                    Level = 2;
                    ManaCost = 1;
                    BaseFailure = 22;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Ranger:
                    Level = 3;
                    ManaCost = 1;
                    BaseFailure = 20;
                    FirstCastExperience = 1;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                    Level = 2;
                    ManaCost = 2;
                    BaseFailure = 20;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Fanatic:
                    Level = 2;
                    ManaCost = 1;
                    BaseFailure = 20;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Cultist:
                    Level = 1;
                    ManaCost = 1;
                    BaseFailure = 15;
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
            return $"dam {3 + ((player.Level - 1) / 5)}d4";
        }
    }
}