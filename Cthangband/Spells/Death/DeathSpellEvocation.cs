using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Death
{
    [Serializable]
    internal class DeathSpellEvocation : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.DispelMonsters(player.Level * 4);
            saveGame.SpellEffects.TurnMonsters(player.Level * 4);
            saveGame.SpellEffects.BanishMonsters(player.Level * 4);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Evocation";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 37;
                    ManaCost = 35;
                    BaseFailure = 80;
                    FirstCastExperience = 70;
                    break;

                case CharacterClass.Priest:
                    Level = 42;
                    ManaCost = 40;
                    BaseFailure = 80;
                    FirstCastExperience = 70;
                    break;

                case CharacterClass.Rogue:
                    Level = 99;
                    ManaCost = 0;
                    BaseFailure = 0;
                    FirstCastExperience = 0;
                    break;

                case CharacterClass.Ranger:
                    Level = 50;
                    ManaCost = 50;
                    BaseFailure = 90;
                    FirstCastExperience = 75;
                    break;

                case CharacterClass.Paladin:
                    Level = 47;
                    ManaCost = 45;
                    BaseFailure = 80;
                    FirstCastExperience = 70;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 45;
                    ManaCost = 55;
                    BaseFailure = 80;
                    FirstCastExperience = 70;
                    break;

                case CharacterClass.HighMage:
                    Level = 33;
                    ManaCost = 30;
                    BaseFailure = 70;
                    FirstCastExperience = 70;
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
            return $"dam {player.Level * 4}";
        }
    }
}