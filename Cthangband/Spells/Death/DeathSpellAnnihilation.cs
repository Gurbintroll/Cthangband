// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Spells.Base;
using Cthangband.StaticData;
using Cthangband.UI;
using System;

namespace Cthangband.Spells.Death
{
    [Serializable]
    internal class DeathSpellAnnihilation : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.Vis -= 100;
            for (int i = 1; i < level.MMax; i++)
            {
                Monster mPtr = level.Monsters[i];
                MonsterRace rPtr = mPtr.Race;
                if (mPtr.Race == null)
                {
                    continue;
                }
                if ((rPtr.Flags1 & MonsterFlag1.Unique) != 0)
                {
                    continue;
                }
                if ((rPtr.Flags1 & MonsterFlag1.Guardian) != 0)
                {
                    continue;
                }
                level.Monsters.DeleteMonsterByIndex(i, true);
                player.TakeHit(Program.Rng.DieRoll(4), "the strain of casting Annihilation");
                player.Vis++;
                level.MoveCursorRelative(player.MapY, player.MapX);
                player.RedrawNeeded.Set(RedrawFlag.PrHp | RedrawFlag.PrVis);
                SaveGame.Instance.HandleStuff();
                Gui.Refresh();
                Gui.Pause(GlobalData.DelayFactor * GlobalData.DelayFactor * GlobalData.DelayFactor);
            }
            player.Vis += 100;
        }

        public override void Initialise(int characterClass)
        {
            Name = "Annihilation";
            switch (characterClass)
            {
                case CharacterClassId.Mage:
                    Level = 45;
                    VisCost = 100;
                    BaseFailure = 95;
                    FirstCastExperience = 250;
                    break;

                case CharacterClassId.Priest:
                    Level = 49;
                    VisCost = 100;
                    BaseFailure = 95;
                    FirstCastExperience = 250;
                    break;

                case CharacterClassId.Rogue:
                    Level = 99;
                    VisCost = 0;
                    BaseFailure = 0;
                    FirstCastExperience = 0;
                    break;

                case CharacterClassId.Ranger:
                    Level = 99;
                    VisCost = 0;
                    BaseFailure = 0;
                    FirstCastExperience = 0;
                    break;

                case CharacterClassId.Paladin:
                    Level = 50;
                    VisCost = 100;
                    BaseFailure = 95;
                    FirstCastExperience = 250;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Cultist:
                    Level = 50;
                    VisCost = 100;
                    BaseFailure = 95;
                    FirstCastExperience = 250;
                    break;

                case CharacterClassId.HighMage:
                    Level = 41;
                    VisCost = 85;
                    BaseFailure = 80;
                    FirstCastExperience = 250;
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