﻿using Cthangband.Enumerations;
using Cthangband.Projection;
using Cthangband.StaticData;
using System;

namespace Cthangband.Spells.Life
{
    [Serializable]
    internal class LifeSpellDivineIntervention : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.Project(0, 1, player.MapY, player.MapX, 777, new ProjectHolyFire(SaveGame.Instance.SpellEffects),
                ProjectionFlag.ProjectKill);
            saveGame.SpellEffects.DispelMonsters(player.Level * 4);
            saveGame.SpellEffects.SlowMonsters();
            saveGame.SpellEffects.StunMonsters(player.Level * 4);
            saveGame.SpellEffects.ConfuseMonsters(player.Level * 4);
            saveGame.SpellEffects.TurnMonsters(player.Level * 4);
            saveGame.SpellEffects.StasisMonsters(player.Level * 4);
            level.Monsters.SummonSpecificFriendly(player.MapY, player.MapX, player.Level, Constants.SummonCthuloid, true);
            player.SetTimedSuperheroism(player.TimedSuperheroism + Program.Rng.DieRoll(25) + 25);
            player.RestoreHealth(300);
            if (player.TimedHaste == 0)
            {
                player.SetTimedHaste(Program.Rng.DieRoll(20 + player.Level) + player.Level);
            }
            else
            {
                player.SetTimedHaste(player.TimedHaste + Program.Rng.DieRoll(5));
            }
            player.SetTimedFear(0);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Divine Intervention";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 48;
                    ManaCost = 50;
                    BaseFailure = 80;
                    FirstCastExperience = 100;
                    break;

                case CharacterClass.Priest:
                    Level = 40;
                    ManaCost = 40;
                    BaseFailure = 80;
                    FirstCastExperience = 200;
                    break;

                case CharacterClass.Paladin:
                    Level = 45;
                    ManaCost = 45;
                    BaseFailure = 80;
                    FirstCastExperience = 100;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 48;
                    ManaCost = 50;
                    BaseFailure = 80;
                    FirstCastExperience = 100;
                    break;

                case CharacterClass.HighMage:
                    Level = 45;
                    ManaCost = 60;
                    BaseFailure = 60;
                    FirstCastExperience = 100;
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
            return $"h300/d{player.Level * 4}+777";
        }
    }
}