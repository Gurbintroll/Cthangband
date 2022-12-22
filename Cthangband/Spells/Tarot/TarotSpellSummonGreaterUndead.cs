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
    internal class TarotSpellSummonGreaterUndead : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            Profile.Instance.MsgPrint("You concentrate on the image of a greater undead being...");
            if (Program.Rng.DieRoll(10) > 3)
            {
                if (!level.Monsters.SummonSpecificFriendly(player.MapY, player.MapX, player.Level,
                    Constants.SummonHiUndeadNoUniques, true))
                {
                    Profile.Instance.MsgPrint("No-one ever turns up.");
                }
            }
            else if (level.Monsters.SummonSpecific(player.MapY, player.MapX, player.Level, Constants.SummonHiUndeadNoUniques))
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
            Name = "Summon Greater Undead";
            switch (characterClass)
            {
                case CharacterClassId.Mage:
                    Level = 49;
                    VisCost = 100;
                    BaseFailure = 80;
                    FirstCastExperience = 220;
                    break;

                case CharacterClassId.Priest:
                case CharacterClassId.Monk:
                    Level = 50;
                    VisCost = 125;
                    BaseFailure = 80;
                    FirstCastExperience = 220;
                    break;

                case CharacterClassId.Rogue:
                    Level = 99;
                    VisCost = 0;
                    BaseFailure = 0;
                    FirstCastExperience = 0;
                    break;

                case CharacterClassId.Ranger:
                    Level = 99;
                    VisCost = 0;
                    BaseFailure = 0;
                    FirstCastExperience = 0;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Cultist:
                    Level = 50;
                    VisCost = 135;
                    BaseFailure = 80;
                    FirstCastExperience = 220;
                    break;

                case CharacterClassId.HighMage:
                    Level = 46;
                    VisCost = 90;
                    BaseFailure = 70;
                    FirstCastExperience = 220;
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
            return "control 70%";
        }
    }
}