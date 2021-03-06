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
    internal class TarotSpellSummonReptiles : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            Profile.Instance.MsgPrint("You concentrate on the image of a reptile...");
            if (Program.Rng.DieRoll(5) > 2)
            {
                if (!level.Monsters.SummonSpecificFriendly(player.MapY, player.MapX, player.Level, Constants.SummonHydra,
                    true))
                {
                    Profile.Instance.MsgPrint("No-one ever turns up.");
                }
            }
            else if (level.Monsters.SummonSpecific(player.MapY, player.MapX, player.Level, Constants.SummonHydra))
            {
                Profile.Instance.MsgPrint("The summoned reptile gets angry!");
            }
            else
            {
                Profile.Instance.MsgPrint("No-one ever turns up.");
            }
        }

        public override void Initialise(int characterClass)
        {
            Name = "Summon Reptiles";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 26;
                    ManaCost = 26;
                    BaseFailure = 70;
                    FirstCastExperience = 30;
                    break;

                case CharacterClass.Priest:
                case CharacterClass.Monk:
                    Level = 29;
                    ManaCost = 27;
                    BaseFailure = 70;
                    FirstCastExperience = 30;
                    break;

                case CharacterClass.Rogue:
                    Level = 33;
                    ManaCost = 30;
                    BaseFailure = 70;
                    FirstCastExperience = 30;
                    break;

                case CharacterClass.Ranger:
                    Level = 31;
                    ManaCost = 30;
                    BaseFailure = 70;
                    FirstCastExperience = 30;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 31;
                    ManaCost = 30;
                    BaseFailure = 70;
                    FirstCastExperience = 30;
                    break;

                case CharacterClass.HighMage:
                    Level = 23;
                    ManaCost = 23;
                    BaseFailure = 60;
                    FirstCastExperience = 30;
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