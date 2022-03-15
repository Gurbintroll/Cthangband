// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.StaticData;
using Cthangband.UI;
using System;

namespace Cthangband.Spells.Death
{
    [Serializable]
    internal class DeathSpellAnnihilation : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.Mana -= 100;
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
                player.Mana++;
                level.MoveCursorRelative(player.MapY, player.MapX);
                player.RedrawNeeded.Set(RedrawFlag.PrHp | RedrawFlag.PrMana);
                SaveGame.Instance.HandleStuff();
                Gui.Refresh();
                Gui.Pause(GlobalData.DelayFactor * GlobalData.DelayFactor * GlobalData.DelayFactor);
            }
            player.Mana += 100;
        }

        public override void Initialise(int characterClass)
        {
            Name = "Annihilation";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 45;
                    ManaCost = 100;
                    BaseFailure = 95;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.Priest:
                    Level = 49;
                    ManaCost = 100;
                    BaseFailure = 95;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.Rogue:
                    Level = 99;
                    ManaCost = 0;
                    BaseFailure = 0;
                    FirstCastExperience = 0;
                    break;

                case CharacterClass.Ranger:
                    Level = 99;
                    ManaCost = 0;
                    BaseFailure = 0;
                    FirstCastExperience = 0;
                    break;

                case CharacterClass.Paladin:
                    Level = 50;
                    ManaCost = 100;
                    BaseFailure = 95;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 50;
                    ManaCost = 100;
                    BaseFailure = 95;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.HighMage:
                    Level = 41;
                    ManaCost = 85;
                    BaseFailure = 80;
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
            return string.Empty;
        }
    }
}