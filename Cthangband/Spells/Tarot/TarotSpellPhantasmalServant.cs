// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.StaticData;
using System;

namespace Cthangband.Spells.Tarot
{
    [Serializable]
    internal class TarotSpellPhantasmalServant : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            Profile.Instance.MsgPrint(
                level.Monsters.SummonSpecificFriendly(player.MapY, player.MapX, player.Level * 3 / 2, Constants.SummonPhantom,
                    false)
                    ? "'Your wish, master?'"
                    : "No-one ever turns up.");
        }

        public override void Initialise(int characterClass)
        {
            Name = "Phantasmal Servant";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 28;
                    ManaCost = 24;
                    BaseFailure = 60;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.Priest:
                case CharacterClass.Monk:
                    Level = 30;
                    ManaCost = 25;
                    BaseFailure = 60;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.Rogue:
                    Level = 33;
                    ManaCost = 26;
                    BaseFailure = 60;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.Ranger:
                    Level = 33;
                    ManaCost = 26;
                    BaseFailure = 60;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 31;
                    ManaCost = 26;
                    BaseFailure = 60;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.HighMage:
                    Level = 25;
                    ManaCost = 22;
                    BaseFailure = 50;
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
            return "control 100%";
        }
    }
}