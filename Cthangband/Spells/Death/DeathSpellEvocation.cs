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

namespace Cthangband.Spells.Death
{
    [Serializable]
    internal class DeathSpellEvocation : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.DispelMonsters(player.Level * 4);
            saveGame.SpellEffects.TurnMonsters(player.Level * 4);
            saveGame.SpellEffects.BanishMonsters(player.Level * 4);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Evocation";
            switch (characterClass)
            {
                case CharacterClassId.Mage:
                    Level = 37;
                    VisCost = 35;
                    BaseFailure = 80;
                    FirstCastExperience = 70;
                    break;

                case CharacterClassId.Priest:
                    Level = 42;
                    VisCost = 40;
                    BaseFailure = 80;
                    FirstCastExperience = 70;
                    break;

                case CharacterClassId.Rogue:
                    Level = 99;
                    VisCost = 0;
                    BaseFailure = 0;
                    FirstCastExperience = 0;
                    break;

                case CharacterClassId.Ranger:
                    Level = 50;
                    VisCost = 50;
                    BaseFailure = 90;
                    FirstCastExperience = 75;
                    break;

                case CharacterClassId.Paladin:
                    Level = 47;
                    VisCost = 45;
                    BaseFailure = 80;
                    FirstCastExperience = 70;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Cultist:
                    Level = 45;
                    VisCost = 55;
                    BaseFailure = 80;
                    FirstCastExperience = 70;
                    break;

                case CharacterClassId.HighMage:
                    Level = 33;
                    VisCost = 30;
                    BaseFailure = 70;
                    FirstCastExperience = 70;
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
            return $"dam {player.Level * 4}";
        }
    }
}