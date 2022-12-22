// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Spells.Base;
using System;

namespace Cthangband.Spells.Nature
{
    [Serializable]
    internal class NatureSpellWhirlwindAttack : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            for (int dir = 0; dir <= 9; dir++)
            {
                int y = player.MapY + level.KeypadDirectionYOffset[dir];
                int x = player.MapX + level.KeypadDirectionXOffset[dir];
                GridTile cPtr = level.Grid[y][x];
                Monster mPtr = level.Monsters[cPtr.MonsterIndex];
                if (cPtr.MonsterIndex != 0 && (mPtr.IsVisible || level.GridPassable(y, x)))
                {
                    SaveGame.Instance.CommandEngine.PlayerAttackMonster(y, x);
                }
            }
        }

        public override void Initialise(int characterClass)
        {
            Name = "Whirlwind Attack";
            switch (characterClass)
            {
                case CharacterClassId.Mage:
                    Level = 23;
                    VisCost = 23;
                    BaseFailure = 80;
                    FirstCastExperience = 50;
                    break;

                case CharacterClassId.Priest:
                    Level = 25;
                    VisCost = 25;
                    BaseFailure = 60;
                    FirstCastExperience = 25;
                    break;

                case CharacterClassId.Ranger:
                    Level = 26;
                    VisCost = 26;
                    BaseFailure = 60;
                    FirstCastExperience = 100;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Cultist:
                    Level = 27;
                    VisCost = 27;
                    BaseFailure = 60;
                    FirstCastExperience = 25;
                    break;

                case CharacterClassId.HighMage:
                case CharacterClassId.Druid:
                    Level = 20;
                    VisCost = 20;
                    BaseFailure = 70;
                    FirstCastExperience = 50;
                    break;

                default:
                    Level = 99;
                    VisCost = 0;
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