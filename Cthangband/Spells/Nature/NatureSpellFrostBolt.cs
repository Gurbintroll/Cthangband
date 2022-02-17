using Cthangband.Enumerations;
using Cthangband.Projection;
using System;

namespace Cthangband.Spells.Nature
{
    [Serializable]
    internal class NatureSpellFrostBolt : Spell
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
            SaveGame.Instance.SpellEffects.FireBoltOrBeam(beam - 10, new ProjectCold(SaveGame.Instance.SpellEffects), dir,
                Program.Rng.DiceRoll(5 + ((player.Level - 5) / 4), 8));
        }

        public override void Initialise(int characterClass)
        {
            Name = "Frost Bolt";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 7;
                    ManaCost = 6;
                    BaseFailure = 40;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Priest:
                    Level = 10;
                    ManaCost = 10;
                    BaseFailure = 40;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Ranger:
                    Level = 12;
                    ManaCost = 9;
                    BaseFailure = 55;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 13;
                    ManaCost = 13;
                    BaseFailure = 40;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Druid:
                    Level = 5;
                    ManaCost = 5;
                    BaseFailure = 30;
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
            return $"dam {5 + ((player.Level - 5) / 4)}d8";
        }
    }
}