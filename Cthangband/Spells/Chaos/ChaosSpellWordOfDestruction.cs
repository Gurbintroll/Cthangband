using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Chaos
{
    [Serializable]
    internal class ChaosSpellWordOfDestruction : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.DestroyArea(player.MapY, player.MapX, 15);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Word of Destruction";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 30;
                    ManaCost = 20;
                    BaseFailure = 80;
                    FirstCastExperience = 15;
                    break;

                case CharacterClass.Priest:
                    Level = 33;
                    ManaCost = 23;
                    BaseFailure = 80;
                    FirstCastExperience = 15;
                    break;

                case CharacterClass.Ranger:
                    Level = 43;
                    ManaCost = 30;
                    BaseFailure = 95;
                    FirstCastExperience = 15;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                    Level = 41;
                    ManaCost = 40;
                    BaseFailure = 80;
                    FirstCastExperience = 15;
                    break;

                case CharacterClass.Fanatic:
                    Level = 36;
                    ManaCost = 26;
                    BaseFailure = 80;
                    FirstCastExperience = 15;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Cultist:
                    Level = 27;
                    ManaCost = 17;
                    BaseFailure = 70;
                    FirstCastExperience = 15;
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
            return string.Empty;
        }
    }
}