﻿// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Projection;
using Cthangband.Spells.Base;
using System;

namespace Cthangband.Spells.Chaos
{
    [Serializable]
    internal class ChaosSpellSonicBoom : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.Project(0, 2 + (player.Level / 10), player.MapY, player.MapX, 45 + player.Level,
                new ProjectSound(SaveGame.Instance.SpellEffects), ProjectionFlag.ProjectKill | ProjectionFlag.ProjectItem);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Sonic Boom";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 21;
                    ManaCost = 13;
                    BaseFailure = 45;
                    FirstCastExperience = 10;
                    break;

                case CharacterClass.Priest:
                    Level = 23;
                    ManaCost = 18;
                    BaseFailure = 80;
                    FirstCastExperience = 20;
                    break;

                case CharacterClass.Ranger:
                    Level = 33;
                    ManaCost = 30;
                    BaseFailure = 70;
                    FirstCastExperience = 13;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                    Level = 27;
                    ManaCost = 25;
                    BaseFailure = 50;
                    FirstCastExperience = 20;
                    break;

                case CharacterClass.Fanatic:
                    Level = 25;
                    ManaCost = 17;
                    BaseFailure = 50;
                    FirstCastExperience = 20;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Cultist:
                    Level = 19;
                    ManaCost = 11;
                    BaseFailure = 35;
                    FirstCastExperience = 10;
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
            return $"dam {45 + player.Level}";
        }
    }
}