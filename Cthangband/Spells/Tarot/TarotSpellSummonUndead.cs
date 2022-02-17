using Cthangband.Enumerations;
using Cthangband.StaticData;
using System;

namespace Cthangband.Spells.Tarot
{
    [Serializable]
    internal class TarotSpellSummonUndead : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            Profile.Instance.MsgPrint("You concentrate on the image of an undead creature...");
            if (Program.Rng.DieRoll(10) > 3)
            {
                if (!level.Monsters.SummonSpecificFriendly(player.MapY, player.MapX, player.Level, Constants.SummonUndead,
                    true))
                {
                    Profile.Instance.MsgPrint("No-one ever turns up.");
                }
            }
            else if (level.Monsters.SummonSpecific(player.MapY, player.MapX, player.Level, Constants.SummonUndead))
            {
                Profile.Instance.MsgPrint("The summoned undead creature gets angry!");
            }
            else
            {
                Profile.Instance.MsgPrint("No-one ever turns up.");
            }
        }

        public override void Initialise(int characterClass)
        {
            Name = "Summon Undead";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 36;
                    ManaCost = 80;
                    BaseFailure = 80;
                    FirstCastExperience = 150;
                    break;

                case CharacterClass.Priest:
                case CharacterClass.Monk:
                    Level = 40;
                    ManaCost = 85;
                    BaseFailure = 80;
                    FirstCastExperience = 150;
                    break;

                case CharacterClass.Rogue:
                    Level = 44;
                    ManaCost = 100;
                    BaseFailure = 80;
                    FirstCastExperience = 150;
                    break;

                case CharacterClass.Ranger:
                    Level = 99;
                    ManaCost = 0;
                    BaseFailure = 0;
                    FirstCastExperience = 0;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 42;
                    ManaCost = 95;
                    BaseFailure = 80;
                    FirstCastExperience = 150;
                    break;

                case CharacterClass.HighMage:
                    Level = 34;
                    ManaCost = 75;
                    BaseFailure = 70;
                    FirstCastExperience = 150;
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