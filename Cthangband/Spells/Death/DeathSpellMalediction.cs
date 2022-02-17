using Cthangband.Enumerations;
using Cthangband.Projection;
using System;

namespace Cthangband.Spells.Death
{
    [Serializable]
    internal class DeathSpellMalediction : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetAimDir(out int dir))
            {
                return;
            }
            saveGame.SpellEffects.FireBall(new ProjectHellFire(SaveGame.Instance.SpellEffects), dir,
                Program.Rng.DiceRoll(3 + ((player.Level - 1) / 5), 3), 0);
            if (Program.Rng.DieRoll(5) != 1)
            {
                return;
            }
            int dummy = Program.Rng.DieRoll(1000);
            if (dummy == 666)
            {
                saveGame.SpellEffects.FireBolt(new ProjectDeathRay(SaveGame.Instance.SpellEffects), dir, player.Level);
            }
            if (dummy < 500)
            {
                saveGame.SpellEffects.FireBolt(new ProjectTurnAll(SaveGame.Instance.SpellEffects), dir, player.Level);
            }
            if (dummy < 800)
            {
                saveGame.SpellEffects.FireBolt(new ProjectOldConf(SaveGame.Instance.SpellEffects), dir, player.Level);
            }
            saveGame.SpellEffects.FireBolt(new ProjectStun(SaveGame.Instance.SpellEffects), dir, player.Level);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Malediction";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 2;
                    ManaCost = 2;
                    BaseFailure = 25;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Priest:
                    Level = 2;
                    ManaCost = 2;
                    BaseFailure = 25;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.Rogue:
                    Level = 7;
                    ManaCost = 4;
                    BaseFailure = 40;
                    FirstCastExperience = 1;
                    break;

                case CharacterClass.Ranger:
                    Level = 5;
                    ManaCost = 3;
                    BaseFailure = 40;
                    FirstCastExperience = 3;
                    break;

                case CharacterClass.Paladin:
                    Level = 3;
                    ManaCost = 3;
                    BaseFailure = 25;
                    FirstCastExperience = 1;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 2;
                    ManaCost = 2;
                    BaseFailure = 25;
                    FirstCastExperience = 4;
                    break;

                case CharacterClass.HighMage:
                    Level = 1;
                    ManaCost = 1;
                    BaseFailure = 20;
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