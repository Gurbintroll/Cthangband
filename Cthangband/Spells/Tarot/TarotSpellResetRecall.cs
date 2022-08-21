// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Spells.Base;
using Cthangband.UI;
using System;

namespace Cthangband.Spells.Tarot
{
    [Serializable]
    internal class TarotSpellResetRecall : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            string ppp = $"Reset to which level (1-{player.MaxDlv[SaveGame.Instance.CurDungeon.Index]}): ";
            string def = $"{Math.Max(SaveGame.Instance.CurrentDepth, 1)}";
            if (!Gui.GetString(ppp, out string tmpVal, def, 10))
            {
                return;
            }
            if (!int.TryParse(tmpVal, out int dummy))
            {
                dummy = 1;
            }
            if (dummy < 1)
            {
                dummy = 1;
            }
            if (dummy > player.MaxDlv[SaveGame.Instance.CurDungeon.Index])
            {
                dummy = player.MaxDlv[SaveGame.Instance.CurDungeon.Index];
            }
            Profile.Instance.MsgPrint($"Recall depth set to level {dummy}.");
        }

        public override void Initialise(int characterClass)
        {
            Name = "Reset Recall";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 6;
                    ManaCost = 6;
                    BaseFailure = 80;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.Priest:
                case CharacterClass.Monk:
                    Level = 7;
                    ManaCost = 7;
                    BaseFailure = 80;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.Rogue:
                    Level = 11;
                    ManaCost = 9;
                    BaseFailure = 80;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.Ranger:
                    Level = 10;
                    ManaCost = 8;
                    BaseFailure = 80;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 8;
                    ManaCost = 7;
                    BaseFailure = 80;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.HighMage:
                    Level = 7;
                    ManaCost = 7;
                    BaseFailure = 80;
                    FirstCastExperience = 8;
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