using Cthangband.Enumerations;
using Cthangband.Projection;
using System;

namespace Cthangband.Spells.Tarot
{
    [Serializable]
    internal class TarotSpellMindBlast : Spell
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
            saveGame.SpellEffects.FireBoltOrBeam(beam - 10, new ProjectPsi(SaveGame.Instance.SpellEffects), dir,
                Program.Rng.DiceRoll(3 + ((player.Level - 1) / 5), 3));
        }

        public override void Initialise(int characterClass)
        {
            Name = "Mind Blast";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 3;
                    ManaCost = 3;
                    BaseFailure = 50;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Priest:
                case CharacterClass.Monk:
                    Level = 4;
                    ManaCost = 4;
                    BaseFailure = 50;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Rogue:
                    Level = 7;
                    ManaCost = 5;
                    BaseFailure = 50;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Ranger:
                    Level = 6;
                    ManaCost = 6;
                    BaseFailure = 50;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 5;
                    ManaCost = 5;
                    BaseFailure = 50;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.HighMage:
                    Level = 2;
                    ManaCost = 2;
                    BaseFailure = 40;
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
            return $"dam {3 + ((player.Level - 1) / 5)}d3";
        }
    }
}