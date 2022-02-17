using Cthangband.Enumerations;
using Cthangband.StaticData;
using System;

namespace Cthangband.Spells.Tarot
{
    [Serializable]
    internal class TarotSpellSummonHounds : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            Profile.Instance.MsgPrint("You concentrate on the image of a hound...");
            if (Program.Rng.DieRoll(5) > 2)
            {
                if (!level.Monsters.SummonSpecificFriendly(player.MapY, player.MapX, player.Level, Constants.SummonHound,
                    true))
                {
                    Profile.Instance.MsgPrint("No-one ever turns up.");
                }
            }
            else if (level.Monsters.SummonSpecific(player.MapY, player.MapX, player.Level, Constants.SummonHound))
            {
                Profile.Instance.MsgPrint("The summoned hounds get angry!");
            }
            else
            {
                Profile.Instance.MsgPrint("No-one ever turns up.");
            }
        }

        public override void Initialise(int characterClass)
        {
            Name = "Summon Hounds";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 30;
                    ManaCost = 30;
                    BaseFailure = 70;
                    FirstCastExperience = 35;
                    break;

                case CharacterClass.Priest:
                case CharacterClass.Monk:
                    Level = 33;
                    ManaCost = 30;
                    BaseFailure = 70;
                    FirstCastExperience = 35;
                    break;

                case CharacterClass.Rogue:
                    Level = 38;
                    ManaCost = 33;
                    BaseFailure = 70;
                    FirstCastExperience = 35;
                    break;

                case CharacterClass.Ranger:
                    Level = 36;
                    ManaCost = 33;
                    BaseFailure = 70;
                    FirstCastExperience = 35;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 35;
                    ManaCost = 33;
                    BaseFailure = 70;
                    FirstCastExperience = 35;
                    break;

                case CharacterClass.HighMage:
                    Level = 25;
                    ManaCost = 25;
                    BaseFailure = 60;
                    FirstCastExperience = 35;
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
            return "control 60%";
        }
    }
}