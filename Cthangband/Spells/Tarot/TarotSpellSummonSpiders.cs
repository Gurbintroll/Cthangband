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
using System;

namespace Cthangband.Spells.Tarot
{
    [Serializable]
    internal class TarotSpellSummonSpiders : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            Profile.Instance.MsgPrint("You concentrate on the image of a spider...");
            if (Program.Rng.DieRoll(5) > 2)
            {
                if (!level.Monsters.SummonSpecificFriendly(player.MapY, player.MapX, player.Level, Constants.SummonSpider,
                    true))
                {
                    Profile.Instance.MsgPrint("No-one ever turns up.");
                }
            }
            else if (level.Monsters.SummonSpecific(player.MapY, player.MapX, player.Level, Constants.SummonSpider))
            {
                Profile.Instance.MsgPrint("The summoned spiders get angry!");
            }
            else
            {
                Profile.Instance.MsgPrint("No-one ever turns up.");
            }
        }

        public override void Initialise(int characterClass)
        {
            Name = "Summon Spiders";
            switch (characterClass)
            {
                case CharacterClassId.Mage:
                    Level = 24;
                    VisCost = 24;
                    BaseFailure = 70;
                    FirstCastExperience = 25;
                    break;

                case CharacterClassId.Priest:
                case CharacterClassId.Monk:
                    Level = 27;
                    VisCost = 25;
                    BaseFailure = 70;
                    FirstCastExperience = 25;
                    break;

                case CharacterClassId.Rogue:
                    Level = 30;
                    VisCost = 30;
                    BaseFailure = 70;
                    FirstCastExperience = 25;
                    break;

                case CharacterClassId.Ranger:
                    Level = 28;
                    VisCost = 26;
                    BaseFailure = 70;
                    FirstCastExperience = 25;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Cultist:
                    Level = 29;
                    VisCost = 27;
                    BaseFailure = 70;
                    FirstCastExperience = 25;
                    break;

                case CharacterClassId.HighMage:
                    Level = 21;
                    VisCost = 21;
                    BaseFailure = 60;
                    FirstCastExperience = 25;
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
            return "control 60%";
        }
    }
}