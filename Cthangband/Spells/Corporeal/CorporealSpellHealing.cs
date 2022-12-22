﻿// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Spells.Base;
using System;

namespace Cthangband.Spells.Corporeal
{
    [Serializable]
    internal class CorporealSpellHealing : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.RestoreHealth(300);
            player.SetTimedStun(0);
            player.SetTimedBleeding(0);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Healing";
            switch (characterClass)
            {
                case CharacterClassId.Mage:
                    Level = 28;
                    VisCost = 20;
                    BaseFailure = 70;
                    FirstCastExperience = 15;
                    break;

                case CharacterClassId.Priest:
                    Level = 30;
                    VisCost = 22;
                    BaseFailure = 70;
                    FirstCastExperience = 15;
                    break;

                case CharacterClassId.Ranger:
                    Level = 38;
                    VisCost = 37;
                    BaseFailure = 70;
                    FirstCastExperience = 8;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Monk:
                case CharacterClassId.Cultist:
                    Level = 33;
                    VisCost = 25;
                    BaseFailure = 70;
                    FirstCastExperience = 15;
                    break;

                case CharacterClassId.HighMage:
                    Level = 24;
                    VisCost = 15;
                    BaseFailure = 60;
                    FirstCastExperience = 15;
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
            return "heal 300";
        }
    }
}