using Cthangband.Enumerations;
using Cthangband.Projection;
using System;

namespace Cthangband.Spells.Nature
{
    [Serializable]
    internal class NatureSpellLightningBolt : Spell
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
            saveGame.SpellEffects.FireBoltOrBeam(beam - 10, new ProjectElec(SaveGame.Instance.SpellEffects), dir,
                Program.Rng.DiceRoll(3 + ((player.Level - 5) / 4), 8));
        }

        public override void Initialise(int characterClass)
        {
            Name = "Lightning Bolt";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 5;
                    ManaCost = 5;
                    BaseFailure = 30;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Priest:
                    Level = 8;
                    ManaCost = 7;
                    BaseFailure = 30;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Ranger:
                    Level = 10;
                    ManaCost = 7;
                    BaseFailure = 40;
                    FirstCastExperience = 3;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 11;
                    ManaCost = 11;
                    BaseFailure = 30;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Druid:
                    Level = 5;
                    ManaCost = 4;
                    BaseFailure = 20;
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
            return $"dam {3 + ((player.Level - 5) / 4)}d8";
        }
    }
}