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

namespace Cthangband.Spells.Death
{
    [Serializable]
    internal class DeathSpellRaiseTheDead : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            if (Program.Rng.DieRoll(3) == 1)
            {
                if (level.Monsters.SummonSpecific(player.MapY, player.MapX, player.Level * 3 / 2,
                    player.Level > 47 ? Constants.SummonHiUndead : Constants.SummonUndead))
                {
                    Profile.Instance.MsgPrint(
                        "Cold winds begin to swirl around you, carrying with them the stench of decay...");
                    Profile.Instance.MsgPrint("'The dead arise... to punish you for disturbing them!'");
                }
                else
                {
                    Profile.Instance.MsgPrint("No-one ever turns up.");
                }
            }
            else
            {
                if (level.Monsters.SummonSpecificFriendly(player.MapY, player.MapX, player.Level * 3 / 2,
                    player.Level > 47 ? Constants.SummonHiUndeadNoUniques : Constants.SummonUndead,
                    player.Level > 24 && Program.Rng.DieRoll(3) == 1))
                {
                    Profile.Instance.MsgPrint(
                        "Cold winds begin to swirl around you, carrying with them the stench of decay...");
                    Profile.Instance.MsgPrint("Ancient, long-dead forms arise from the ground to serve you!");
                }
                else
                {
                    Profile.Instance.MsgPrint("No-one ever turns up.");
                }
            }
        }

        public override void Initialise(int characterClass)
        {
            Name = "Raise the Dead";
            switch (characterClass)
            {
                case CharacterClassId.Mage:
                    Level = 20;
                    VisCost = 20;
                    BaseFailure = 75;
                    FirstCastExperience = 50;
                    break;

                case CharacterClassId.Priest:
                    Level = 25;
                    VisCost = 25;
                    BaseFailure = 75;
                    FirstCastExperience = 50;
                    break;

                case CharacterClassId.Rogue:
                    Level = 30;
                    VisCost = 30;
                    BaseFailure = 80;
                    FirstCastExperience = 50;
                    break;

                case CharacterClassId.Ranger:
                    Level = 35;
                    VisCost = 35;
                    BaseFailure = 75;
                    FirstCastExperience = 50;
                    break;

                case CharacterClassId.Paladin:
                    Level = 30;
                    VisCost = 35;
                    BaseFailure = 75;
                    FirstCastExperience = 50;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Cultist:
                    Level = 24;
                    VisCost = 24;
                    BaseFailure = 75;
                    FirstCastExperience = 50;
                    break;

                case CharacterClassId.HighMage:
                    Level = 16;
                    VisCost = 16;
                    BaseFailure = 65;
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
            return "control 67%";
        }
    }
}