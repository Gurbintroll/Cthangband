﻿// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
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
    internal class TarotSpellSummonMonster : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            Profile.Instance.MsgPrint("You concentrate on the image of a monster...");
            if (Program.Rng.DieRoll(5) > 2)
            {
                if (!level.Monsters.SummonSpecificFriendly(player.MapY, player.MapX, player.Level, Constants.SummonNoUniques,
                    false))
                {
                    Profile.Instance.MsgPrint("No-one ever turns up.");
                }
            }
            else if (level.Monsters.SummonSpecific(player.MapY, player.MapX, player.Level, 0))
            {
                Profile.Instance.MsgPrint("The summoned creature gets angry!");
            }
            else
            {
                Profile.Instance.MsgPrint("No-one ever turns up.");
            }
        }

        public override void Initialise(int characterClass)
        {
            Name = "Summon Monster";
            switch (characterClass)
            {
                case CharacterClassId.Mage:
                    Level = 30;
                    VisCost = 25;
                    BaseFailure = 70;
                    FirstCastExperience = 9;
                    break;

                case CharacterClassId.Priest:
                case CharacterClassId.Monk:
                    Level = 32;
                    VisCost = 30;
                    BaseFailure = 70;
                    FirstCastExperience = 9;
                    break;

                case CharacterClassId.Rogue:
                    Level = 35;
                    VisCost = 32;
                    BaseFailure = 70;
                    FirstCastExperience = 9;
                    break;

                case CharacterClassId.Ranger:
                    Level = 36;
                    VisCost = 32;
                    BaseFailure = 70;
                    FirstCastExperience = 9;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Cultist:
                    Level = 33;
                    VisCost = 30;
                    BaseFailure = 70;
                    FirstCastExperience = 9;
                    break;

                case CharacterClassId.HighMage:
                    Level = 28;
                    VisCost = 24;
                    BaseFailure = 60;
                    FirstCastExperience = 9;
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