using Cthangband.Enumerations;
using Cthangband.StaticData;
using System;

namespace Cthangband.Spells.Tarot
{
    [Serializable]
    internal class TarotSpellSummonAncientDragon : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            Profile.Instance.MsgPrint("You concentrate on the image of an ancient dragon...");
            if (Program.Rng.DieRoll(10) > 3)
            {
                if (!level.Monsters.SummonSpecificFriendly(player.MapY, player.MapX, player.Level,
                    Constants.SummonHiDragonNoUniques, true))
                {
                    Profile.Instance.MsgPrint("No-one ever turns up.");
                }
            }
            else if (level.Monsters.SummonSpecific(player.MapY, player.MapX, player.Level, Constants.SummonHiDragonNoUniques))
            {
                Profile.Instance.MsgPrint("The summoned dragon gets angry!");
            }
            else
            {
                Profile.Instance.MsgPrint("No-one ever turns up.");
            }
        }

        public override void Initialise(int characterClass)
        {
            Name = "Summon Ancient Dragon";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 48;
                    ManaCost = 100;
                    BaseFailure = 80;
                    FirstCastExperience = 200;
                    break;

                case CharacterClass.Priest:
                case CharacterClass.Monk:
                    Level = 49;
                    ManaCost = 120;
                    BaseFailure = 80;
                    FirstCastExperience = 200;
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

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 49;
                    ManaCost = 130;
                    BaseFailure = 80;
                    FirstCastExperience = 200;
                    break;

                case CharacterClass.HighMage:
                    Level = 44;
                    ManaCost = 90;
                    BaseFailure = 70;
                    FirstCastExperience = 200;
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
            return "control 70%";
        }
    }
}