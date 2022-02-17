using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Nature
{
    [Serializable]
    internal class NatureSpellWhirlwindAttack : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            for (int dir = 0; dir <= 9; dir++)
            {
                int y = player.MapY + level.KeypadDirectionYOffset[dir];
                int x = player.MapX + level.KeypadDirectionXOffset[dir];
                GridTile cPtr = level.Grid[y][x];
                Monster mPtr = level.Monsters[cPtr.Monster];
                if (cPtr.Monster != 0 && (mPtr.IsVisible || level.GridPassable(y, x)))
                {
                    SaveGame.Instance.CommandEngine.PyAttack(y, x);
                }
            }
        }

        public override void Initialise(int characterClass)
        {
            Name = "Whirlwind Attack";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 23;
                    ManaCost = 23;
                    BaseFailure = 80;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.Priest:
                    Level = 25;
                    ManaCost = 25;
                    BaseFailure = 60;
                    FirstCastExperience = 25;
                    break;

                case CharacterClass.Ranger:
                    Level = 26;
                    ManaCost = 26;
                    BaseFailure = 60;
                    FirstCastExperience = 100;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 27;
                    ManaCost = 27;
                    BaseFailure = 60;
                    FirstCastExperience = 25;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Druid:
                    Level = 20;
                    ManaCost = 20;
                    BaseFailure = 70;
                    FirstCastExperience = 50;
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