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
    internal class TarotSpellTheFool : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            int dummy = 0;
            Profile.Instance.MsgPrint("You concentrate on the Fool card...");
            switch (Program.Rng.DieRoll(4))
            {
                case 1:
                    dummy = Constants.SummonBizarre1;
                    break;

                case 2:
                    dummy = Constants.SummonBizarre2;
                    break;

                case 3:
                    dummy = Constants.SummonBizarre4;
                    break;

                case 4:
                    dummy = Constants.SummonBizarre5;
                    break;
            }
            if (Program.Rng.DieRoll(2) == 1)
            {
                Profile.Instance.MsgPrint(level.Monsters.SummonSpecific(player.MapY, player.MapX, player.Level, dummy)
                    ? "The summoned creature gets angry!"
                    : "No-one ever turns up.");
            }
            else
            {
                if (!level.Monsters.SummonSpecificFriendly(player.MapY, player.MapX, player.Level, dummy, false))
                {
                    Profile.Instance.MsgPrint("No-one ever turns up.");
                }
            }
        }

        public override void Initialise(int characterClass)
        {
            Name = "The Fool";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 15;
                    VrilCost = 15;
                    BaseFailure = 80;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.Priest:
                case CharacterClass.Monk:
                    Level = 17;
                    VrilCost = 17;
                    BaseFailure = 80;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.Rogue:
                    Level = 20;
                    VrilCost = 15;
                    BaseFailure = 80;
                    FirstCastExperience = 20;
                    break;

                case CharacterClass.Ranger:
                    Level = 20;
                    VrilCost = 20;
                    BaseFailure = 80;
                    FirstCastExperience = 20;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 19;
                    VrilCost = 18;
                    BaseFailure = 80;
                    FirstCastExperience = 20;
                    break;

                case CharacterClass.HighMage:
                    Level = 11;
                    VrilCost = 11;
                    BaseFailure = 70;
                    FirstCastExperience = 20;
                    break;

                default:
                    Level = 99;
                    VrilCost = 0;
                    BaseFailure = 0;
                    FirstCastExperience = 0;
                    break;
            }
        }

        protected override string Comment(Player player)
        {
            return "control 50%";
        }
    }
}