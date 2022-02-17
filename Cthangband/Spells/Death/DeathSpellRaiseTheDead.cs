using Cthangband.Enumerations;
using Cthangband.StaticData;
using System;

namespace Cthangband.Spells.Death
{
    [Serializable]
    internal class DeathSpellRaiseTheDead : Spell
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
                case CharacterClass.Mage:
                    Level = 20;
                    ManaCost = 20;
                    BaseFailure = 75;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.Priest:
                    Level = 25;
                    ManaCost = 25;
                    BaseFailure = 75;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.Rogue:
                    Level = 30;
                    ManaCost = 30;
                    BaseFailure = 80;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.Ranger:
                    Level = 35;
                    ManaCost = 35;
                    BaseFailure = 75;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.Paladin:
                    Level = 30;
                    ManaCost = 35;
                    BaseFailure = 75;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 24;
                    ManaCost = 24;
                    BaseFailure = 75;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.HighMage:
                    Level = 16;
                    ManaCost = 16;
                    BaseFailure = 65;
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
            return "control 67%";
        }
    }
}