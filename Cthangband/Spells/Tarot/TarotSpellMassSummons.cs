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
    internal class TarotSpellMassSummons : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            bool noneCame = true;
            Profile.Instance.MsgPrint("You concentrate on several images at once...");
            for (int dummy = 0; dummy < 3 + (player.Level / 10); dummy++)
            {
                if (Program.Rng.DieRoll(10) > 3)
                {
                    if (level.Monsters.SummonSpecificFriendly(player.MapY, player.MapX, player.Level,
                        Constants.SummonNoUniques, false))
                    {
                        noneCame = false;
                    }
                }
                else if (level.Monsters.SummonSpecific(player.MapY, player.MapX, player.Level, 0))
                {
                    Profile.Instance.MsgPrint("A summoned creature gets angry!");
                    noneCame = false;
                }
            }
            if (noneCame)
            {
                Profile.Instance.MsgPrint("No-one ever turns up.");
            }
        }

        public override void Initialise(int characterClass)
        {
            Name = "Mass Summons";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 42;
                    ManaCost = 100;
                    BaseFailure = 80;
                    FirstCastExperience = 200;
                    break;

                case CharacterClass.Priest:
                case CharacterClass.Monk:
                    Level = 46;
                    ManaCost = 110;
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
                    Level = 50;
                    ManaCost = 120;
                    BaseFailure = 80;
                    FirstCastExperience = 200;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 46;
                    ManaCost = 120;
                    BaseFailure = 80;
                    FirstCastExperience = 200;
                    break;

                case CharacterClass.HighMage:
                    Level = 38;
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