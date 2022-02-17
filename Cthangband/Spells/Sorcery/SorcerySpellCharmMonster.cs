using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Sorcery
{
    [Serializable]
    internal class SorcerySpellCharmMonster : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetAimDir(out int dir))
            {
                return;
            }
            saveGame.SpellEffects.CharmMonster(dir, player.Level);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Charm Monster";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 10;
                    ManaCost = 10;
                    BaseFailure = 80;
                    FirstCastExperience = 40;
                    break;

                case CharacterClass.Rogue:
                    Level = 14;
                    ManaCost = 10;
                    BaseFailure = 80;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 14;
                    ManaCost = 12;
                    BaseFailure = 80;
                    FirstCastExperience = 10;
                    break;

                case CharacterClass.HighMage:
                    Level = 8;
                    ManaCost = 8;
                    BaseFailure = 70;
                    FirstCastExperience = 40;
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