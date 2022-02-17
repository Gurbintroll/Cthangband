using Cthangband.Enumerations;
using Cthangband.StaticData;
using System;

namespace Cthangband.Spells.Chaos
{
    [Serializable]
    internal class ChaosSpellSummonDemon : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            if (Program.Rng.DieRoll(3) == 1)
            {
                if (level.Monsters.SummonSpecific(player.MapY, player.MapX, player.Level * 3 / 2, Constants.SummonDemon))
                {
                    Profile.Instance.MsgPrint("The area fills with a stench of sulphur and brimstone.");
                    Profile.Instance.MsgPrint("'NON SERVIAM! Wretch! I shall feast on thy mortal soul!'");
                }
                else
                {
                    Profile.Instance.MsgPrint("No-one ever turns up.");
                }
            }
            else
            {
                if (level.Monsters.SummonSpecificFriendly(player.MapY, player.MapX, player.Level * 3 / 2,
                    Constants.SummonDemon, player.Level == 50))
                {
                    Profile.Instance.MsgPrint("The area fills with a stench of sulphur and brimstone.");
                    Profile.Instance.MsgPrint("'What is thy bidding... Master?'");
                }
                else
                {
                    Profile.Instance.MsgPrint("No-one ever turns up.");
                }
            }
        }

        public override void Initialise(int characterClass)
        {
            Name = "Summon Demon";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 47;
                    ManaCost = 100;
                    BaseFailure = 90;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.Priest:
                    Level = 49;
                    ManaCost = 100;
                    BaseFailure = 90;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.Ranger:
                    Level = 99;
                    ManaCost = 0;
                    BaseFailure = 0;
                    FirstCastExperience = 0;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                    Level = 50;
                    ManaCost = 111;
                    BaseFailure = 80;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.Fanatic:
                    Level = 47;
                    ManaCost = 100;
                    BaseFailure = 80;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Cultist:
                    Level = 44;
                    ManaCost = 90;
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
            return "control 67%";
        }
    }
}