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
    internal class TarotSpellConjureElemental : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            if (Program.Rng.DieRoll(6) > 3)
            {
                if (!level.Monsters.SummonSpecificFriendly(player.MapY, player.MapX, player.Level, Constants.SummonElemental,
                    false))
                {
                    Profile.Instance.MsgPrint("No-one ever turns up.");
                }
            }
            else if (level.Monsters.SummonSpecific(player.MapY, player.MapX, player.Level, Constants.SummonElemental))
            {
                Profile.Instance.MsgPrint("You fail to control the elemental creature!");
            }
            else
            {
                Profile.Instance.MsgPrint("No-one ever turns up.");
            }
        }

        public override void Initialise(int characterClass)
        {
            Name = "Conjure Elemental";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 33;
                    VrilCost = 28;
                    BaseFailure = 80;
                    FirstCastExperience = 12;
                    break;

                case CharacterClass.Priest:
                case CharacterClass.Monk:
                    Level = 35;
                    VrilCost = 30;
                    BaseFailure = 80;
                    FirstCastExperience = 12;
                    break;

                case CharacterClass.Rogue:
                    Level = 40;
                    VrilCost = 35;
                    BaseFailure = 80;
                    FirstCastExperience = 12;
                    break;

                case CharacterClass.Ranger:
                    Level = 38;
                    VrilCost = 33;
                    BaseFailure = 80;
                    FirstCastExperience = 12;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 38;
                    VrilCost = 32;
                    BaseFailure = 80;
                    FirstCastExperience = 12;
                    break;

                case CharacterClass.HighMage:
                    Level = 28;
                    VrilCost = 26;
                    BaseFailure = 70;
                    FirstCastExperience = 12;
                    break;

                default:
                    Level = 99;
                    VrilCost = 0;
                    BaseFailure = 0;
                    FirstCastExperience = 0;
                    break;
            }
        }

        protected override string Comment(Player player)
        {
            return "control 50%";
        }
    }
}