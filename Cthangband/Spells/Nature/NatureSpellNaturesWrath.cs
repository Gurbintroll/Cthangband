using Cthangband.Enumerations;
using Cthangband.Projection;
using System;

namespace Cthangband.Spells.Nature
{
    [Serializable]
    internal class NatureSpellNaturesWrath : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.DispelMonsters(player.Level * 4);
            saveGame.SpellEffects.Earthquake(player.MapY, player.MapX, 20 + (player.Level / 2));
            saveGame.SpellEffects.Project(0, 1 + (player.Level / 12), player.MapY, player.MapX, 100 + player.Level,
                new ProjectDisintegrate(SaveGame.Instance.SpellEffects), ProjectionFlag.ProjectKill | ProjectionFlag.ProjectItem);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Nature's Wrath";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 40;
                    ManaCost = 90;
                    BaseFailure = 95;
                    FirstCastExperience = 200;
                    break;

                case CharacterClass.Priest:
                    Level = 42;
                    ManaCost = 90;
                    BaseFailure = 95;
                    FirstCastExperience = 200;
                    break;

                case CharacterClass.Ranger:
                    Level = 41;
                    ManaCost = 80;
                    BaseFailure = 95;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 45;
                    ManaCost = 95;
                    BaseFailure = 95;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Druid:
                    Level = 36;
                    ManaCost = 80;
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
            return $"dam {4 * player.Level}+{100 + player.Level}";
        }
    }
}