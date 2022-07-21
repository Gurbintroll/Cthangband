// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Mutations;
using Cthangband.Projection;
using Cthangband.StaticData;
using Cthangband.UI;
using System;

namespace Cthangband
{
    /// <summary>
    /// Class for handling complex player commands
    /// </summary>
    [Serializable]
    internal class CommandEngine
    {
        /// <summary>
        /// Maximum amount of health that can be drained from an opponent in one turn
        /// </summary>
        private const int _maxVampiricDrain = 100;

        private readonly SaveGame _saveGame;

        /// <summary>
        /// The current state of navigation stored from turn to turn while the player is running
        /// </summary>
        private AutoNavigator _autoNavigator = new AutoNavigator();

        public CommandEngine(SaveGame saveGame)
        {
            _saveGame = saveGame;
        }

        private Level Level => _saveGame.Level;

        private Player Player => _saveGame.Player;

        /// <summary>
        /// Activate a randomly generated artifact that will therefore have been given a random power
        /// </summary>
        /// <param name="item"> The artifact being activated </param>
        public void ActivateRandomArtifact(Item item)
        {
            int playerLevel = Player.Level;
            TargetEngine targetEngine = new TargetEngine(Player, Level);
            int direction;
            int i;
            // If we don't have a random artifact, abort
            if (string.IsNullOrEmpty(item.RandartName))
            {
                return;
            }
            // Big stonking switch based on the artifact type
            switch (item.BonusPowerSubType)
            {
                case RandomArtifactPower.ActSunlight:
                    {
                        // Aim a line of light in a direction of the player's choice
                        if (!targetEngine.GetDirectionWithAim(out direction))
                        {
                            return;
                        }
                        Profile.Instance.MsgPrint("A line of sunlight appears.");
                        _saveGame.SpellEffects.LightLine(direction);
                        item.RechargeTimeLeft = 10;
                        break;
                    }
                case RandomArtifactPower.ActBoMiss1:
                    {
                        // Shoot a magic missile that does 2d6 damage
                        Profile.Instance.MsgPrint("It glows extremely brightly...");
                        if (!targetEngine.GetDirectionWithAim(out direction))
                        {
                            return;
                        }
                        _saveGame.SpellEffects.FireBolt(new ProjectMissile(SaveGame.Instance.SpellEffects), direction,
                            Program.Rng.DiceRoll(2, 6));
                        item.RechargeTimeLeft = 2;
                        break;
                    }
                case RandomArtifactPower.ActBaPois1:
                    {
                        // Shoot a 12-damage ball of poison
                        Profile.Instance.MsgPrint("It throbs deep green...");
                        if (!targetEngine.GetDirectionWithAim(out direction))
                        {
                            return;
                        }
                        _saveGame.SpellEffects.FireBall(new ProjectPois(SaveGame.Instance.SpellEffects), direction, 12, 3);
                        item.RechargeTimeLeft = Program.Rng.RandomLessThan(4) + 4;
                        break;
                    }
                case RandomArtifactPower.ActBoElec1:
                    {
                        //Shoot a lightning bolt that does 4d8 damage
                        Profile.Instance.MsgPrint("It is covered in sparks...");
                        if (!targetEngine.GetDirectionWithAim(out direction))
                        {
                            return;
                        }
                        _saveGame.SpellEffects.FireBolt(new ProjectElec(SaveGame.Instance.SpellEffects), direction,
                            Program.Rng.DiceRoll(4, 8));
                        item.RechargeTimeLeft = Program.Rng.RandomLessThan(6) + 6;
                        break;
                    }
                case RandomArtifactPower.ActBoAcid1:
                    {
                        // Shoot an acid bolt that does 5d8 damage
                        Profile.Instance.MsgPrint("It is covered in acid...");
                        if (!targetEngine.GetDirectionWithAim(out direction))
                        {
                            return;
                        }
                        _saveGame.SpellEffects.FireBolt(new ProjectAcid(SaveGame.Instance.SpellEffects), direction,
                            Program.Rng.DiceRoll(5, 8));
                        item.RechargeTimeLeft = Program.Rng.RandomLessThan(5) + 5;
                        break;
                    }
                case RandomArtifactPower.ActBoCold1:
                    {
                        // Shoot a frost bolt that does 6d8 damage
                        Profile.Instance.MsgPrint("It is covered in frost...");
                        if (!targetEngine.GetDirectionWithAim(out direction))
                        {
                            return;
                        }
                        _saveGame.SpellEffects.FireBolt(new ProjectCold(SaveGame.Instance.SpellEffects), direction,
                            Program.Rng.DiceRoll(6, 8));
                        item.RechargeTimeLeft = Program.Rng.RandomLessThan(7) + 7;
                        break;
                    }
                case RandomArtifactPower.ActBoFire1:
                    {
                        // Shoot a fire bolt that does 9d8 damage
                        Profile.Instance.MsgPrint("It is covered in fire...");
                        if (!targetEngine.GetDirectionWithAim(out direction))
                        {
                            return;
                        }
                        _saveGame.SpellEffects.FireBolt(new ProjectFire(SaveGame.Instance.SpellEffects), direction,
                            Program.Rng.DiceRoll(9, 8));
                        item.RechargeTimeLeft = Program.Rng.RandomLessThan(8) + 8;
                        break;
                    }
                case RandomArtifactPower.ActBaCold1:
                    {
                        // Shoot a frost ball that does 48 damage
                        Profile.Instance.MsgPrint("It is covered in frost...");
                        if (!targetEngine.GetDirectionWithAim(out direction))
                        {
                            return;
                        }
                        _saveGame.SpellEffects.FireBall(new ProjectCold(SaveGame.Instance.SpellEffects), direction, 48, 2);
                        item.RechargeTimeLeft = 400;
                        break;
                    }
                case RandomArtifactPower.ActBaFire1:
                    {
                        // Shoot a fire ball that does 72 damage
                        Profile.Instance.MsgPrint("It glows an intense red...");
                        if (!targetEngine.GetDirectionWithAim(out direction))
                        {
                            return;
                        }
                        _saveGame.SpellEffects.FireBall(new ProjectFire(SaveGame.Instance.SpellEffects), direction, 72, 2);
                        item.RechargeTimeLeft = 400;
                        break;
                    }
                case RandomArtifactPower.ActDrain1:
                    {
                        // Drain up to 100 life from an opponent
                        Profile.Instance.MsgPrint("It glows black...");
                        if (!targetEngine.GetDirectionWithAim(out direction))
                        {
                            return;
                        }
                        if (_saveGame.SpellEffects.DrainLife(direction, 100))
                        {
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(100) + 100;
                        }
                        break;
                    }
                case RandomArtifactPower.ActBaCold2:
                    {
                        // Shoot a frost ball that does 100 damage
                        Profile.Instance.MsgPrint("It glows an intense blue...");
                        if (!targetEngine.GetDirectionWithAim(out direction))
                        {
                            return;
                        }
                        _saveGame.SpellEffects.FireBall(new ProjectCold(SaveGame.Instance.SpellEffects), direction, 100, 2);
                        item.RechargeTimeLeft = 300;
                        break;
                    }
                case RandomArtifactPower.ActBaElec2:
                    {
                        // Shoot a lightning storm that does 100 damage with a larger radius
                        Profile.Instance.MsgPrint("It crackles with electricity...");
                        if (!targetEngine.GetDirectionWithAim(out direction))
                        {
                            return;
                        }
                        _saveGame.SpellEffects.FireBall(new ProjectElec(SaveGame.Instance.SpellEffects), direction, 100, 3);
                        item.RechargeTimeLeft = 500;
                        break;
                    }
                case RandomArtifactPower.ActDrain2:
                    {
                        // Drain up to 120 life from an opponent
                        Profile.Instance.MsgPrint("It glows black...");
                        if (!targetEngine.GetDirectionWithAim(out direction))
                        {
                            return;
                        }
                        _saveGame.SpellEffects.DrainLife(direction, 120);
                        item.RechargeTimeLeft = 400;
                        break;
                    }
                case RandomArtifactPower.ActVampire1:
                    {
                        // Drain up to 50 life from an opponent, and give it to the player
                        if (!targetEngine.GetDirectionWithAim(out direction))
                        {
                            return;
                        }
                        for (i = 0; i < 3; i++)
                        {
                            if (_saveGame.SpellEffects.DrainLife(direction, 50))
                            {
                                Player.RestoreHealth(50);
                            }
                        }
                        item.RechargeTimeLeft = 400;
                        break;
                    }
                case RandomArtifactPower.ActBoMiss2:
                    {
                        // Shoot an arrow that does 150 damage
                        Profile.Instance.MsgPrint("It grows magical spikes...");
                        if (!targetEngine.GetDirectionWithAim(out direction))
                        {
                            return;
                        }
                        _saveGame.SpellEffects.FireBolt(new ProjectArrow(SaveGame.Instance.SpellEffects), direction, 150);
                        item.RechargeTimeLeft = Program.Rng.RandomLessThan(90) + 90;
                        break;
                    }
                case RandomArtifactPower.ActBaFire2:
                    {
                        // Shoot a fire ball that does 120 damage with a larger radius
                        Profile.Instance.MsgPrint("It glows deep red...");
                        if (!targetEngine.GetDirectionWithAim(out direction))
                        {
                            return;
                        }
                        _saveGame.SpellEffects.FireBall(new ProjectFire(SaveGame.Instance.SpellEffects), direction, 120, 3);
                        item.RechargeTimeLeft = Program.Rng.RandomLessThan(225) + 225;
                        break;
                    }
                case RandomArtifactPower.ActBaCold3:
                    {
                        // Shoot a frost ball that does 200 damage with a larger radius
                        Profile.Instance.MsgPrint("It glows bright white...");
                        if (!targetEngine.GetDirectionWithAim(out direction))
                        {
                            return;
                        }
                        _saveGame.SpellEffects.FireBall(new ProjectCold(SaveGame.Instance.SpellEffects), direction, 200, 3);
                        item.RechargeTimeLeft = Program.Rng.RandomLessThan(325) + 325;
                        break;
                    }
                case RandomArtifactPower.ActBaElec3:
                    {
                        // Shoot a lightning storm that does 250 damage with a larger radius
                        Profile.Instance.MsgPrint("It glows deep blue...");
                        if (!targetEngine.GetDirectionWithAim(out direction))
                        {
                            return;
                        }
                        _saveGame.SpellEffects.FireBall(new ProjectElec(SaveGame.Instance.SpellEffects), direction, 250, 3);
                        item.RechargeTimeLeft = Program.Rng.RandomLessThan(425) + 425;
                        break;
                    }
                case RandomArtifactPower.ActWhirlwind:
                    {
                        {
                            // Make a physical attack against each adjacent monster
                            for (direction = 0; direction <= 9; direction++)
                            {
                                int y = Player.MapY + Level.KeypadDirectionYOffset[direction];
                                int x = Player.MapX + Level.KeypadDirectionXOffset[direction];
                                GridTile cPtr = Level.Grid[y][x];
                                Monster mPtr = Level.Monsters[cPtr.MonsterIndex];
                                if (cPtr.MonsterIndex != 0 && (mPtr.IsVisible || Level.GridPassable(y, x)))
                                {
                                    PlayerAttackMonster(y, x);
                                }
                            }
                        }
                        item.RechargeTimeLeft = 250;
                        break;
                    }
                case RandomArtifactPower.ActVampire2:
                    {
                        // Drain 100 health from an opponent, and give it to the player
                        if (!targetEngine.GetDirectionWithAim(out direction))
                        {
                            return;
                        }
                        for (i = 0; i < 3; i++)
                        {
                            if (_saveGame.SpellEffects.DrainLife(direction, 100))
                            {
                                Player.RestoreHealth(100);
                            }
                        }
                        item.RechargeTimeLeft = 400;
                        break;
                    }
                case RandomArtifactPower.ActCallChaos:
                    {
                        // Activate the Call Chaos spell
                        Profile.Instance.MsgPrint("It glows in scintillating colours...");
                        _saveGame.SpellEffects.CallChaos();
                        item.RechargeTimeLeft = 350;
                        break;
                    }
                case RandomArtifactPower.ActShard:
                    {
                        // Shoot a shard ball for 120 + level damage
                        if (!targetEngine.GetDirectionWithAim(out direction))
                        {
                            return;
                        }
                        _saveGame.SpellEffects.FireBall(new ProjectShard(SaveGame.Instance.SpellEffects), direction, 120 + playerLevel,
                            2);
                        item.RechargeTimeLeft = 400;
                        break;
                    }
                case RandomArtifactPower.ActDispEvil:
                    {
                        // Dispel evil with a strength of x5
                        Profile.Instance.MsgPrint("It floods the area with goodness...");
                        _saveGame.SpellEffects.DispelEvil(Player.Level * 5);
                        item.RechargeTimeLeft = Program.Rng.RandomLessThan(300) + 300;
                        break;
                    }
                case RandomArtifactPower.ActDispGood:
                    {
                        // Dispel good with a strength of x5
                        Profile.Instance.MsgPrint("It floods the area with evil...");
                        _saveGame.SpellEffects.DispelGood(Player.Level * 5);
                        item.RechargeTimeLeft = Program.Rng.RandomLessThan(300) + 300;
                        break;
                    }
                case RandomArtifactPower.ActBaMiss3:
                    {
                        // Shoot a 'magic missile' cone that does 300 damage
                        if (!targetEngine.GetDirectionWithAim(out direction))
                        {
                            return;
                        }
                        Profile.Instance.MsgPrint("You breathe the elements.");
                        _saveGame.SpellEffects.FireBall(new ProjectMissile(SaveGame.Instance.SpellEffects), direction, 300, -4);
                        item.RechargeTimeLeft = 500;
                        break;
                    }
                case RandomArtifactPower.ActConfuse:
                    {
                        // Confuse an opponent
                        Profile.Instance.MsgPrint("It glows in scintillating colours...");
                        if (!targetEngine.GetDirectionWithAim(out direction))
                        {
                            return;
                        }
                        _saveGame.SpellEffects.ConfuseMonster(direction, 20);
                        item.RechargeTimeLeft = 15;
                        break;
                    }
                case RandomArtifactPower.ActSleep:
                    {
                        // Activate sleep on touch
                        Profile.Instance.MsgPrint("It glows deep blue...");
                        _saveGame.SpellEffects.SleepMonstersTouch();
                        item.RechargeTimeLeft = 55;
                        break;
                    }
                case RandomArtifactPower.ActQuake:
                    {
                        // Cause an earthquake around the player
                        _saveGame.SpellEffects.Earthquake(Player.MapY, Player.MapX, 10);
                        item.RechargeTimeLeft = 50;
                        break;
                    }
                case RandomArtifactPower.ActTerror:
                    {
                        // Scare monsters with a 40+level strength
                        _saveGame.SpellEffects.TurnMonsters(40 + playerLevel);
                        item.RechargeTimeLeft = 3 * (Player.Level + 10);
                        break;
                    }
                case RandomArtifactPower.ActTeleAway:
                    {
                        // Teleport away an opponent
                        if (!targetEngine.GetDirectionWithAim(out direction))
                        {
                            return;
                        }
                        _saveGame.SpellEffects.FireBeam(new ProjectAwayAll(SaveGame.Instance.SpellEffects), direction, playerLevel);
                        item.RechargeTimeLeft = 200;
                        break;
                    }
                case RandomArtifactPower.ActBanishEvil:
                    {
                        // Banish evil creatures
                        if (_saveGame.SpellEffects.BanishEvil(100))
                        {
                            Profile.Instance.MsgPrint("The power of the artifact banishes evil!");
                        }
                        item.RechargeTimeLeft = 250 + Program.Rng.DieRoll(250);
                        break;
                    }
                case RandomArtifactPower.ActCarnage:
                    {
                        // Carnage a chosen creature type
                        Profile.Instance.MsgPrint("It glows deep blue...");
                        _saveGame.SpellEffects.Carnage(true);
                        item.RechargeTimeLeft = 500;
                        break;
                    }
                case RandomArtifactPower.ActMassGeno:
                    {
                        // Mass carnage creatures near the player
                        Profile.Instance.MsgPrint("It lets out a long, shrill note...");
                        _saveGame.SpellEffects.MassCarnage(true);
                        item.RechargeTimeLeft = 1000;
                        break;
                    }
                case RandomArtifactPower.ActCharmAnimal:
                    {
                        // Charm an animal
                        if (!targetEngine.GetDirectionWithAim(out direction))
                        {
                            return;
                        }
                        _saveGame.SpellEffects.CharmAnimal(direction, playerLevel);
                        item.RechargeTimeLeft = 300;
                        break;
                    }
                case RandomArtifactPower.ActCharmUndead:
                    {
                        // Charm an undead
                        if (!targetEngine.GetDirectionWithAim(out direction))
                        {
                            return;
                        }
                        _saveGame.SpellEffects.ControlOneUndead(direction, playerLevel);
                        item.RechargeTimeLeft = 333;
                        break;
                    }
                case RandomArtifactPower.ActCharmOther:
                    {
                        // Charm a monster
                        if (!targetEngine.GetDirectionWithAim(out direction))
                        {
                            return;
                        }
                        _saveGame.SpellEffects.CharmMonster(direction, playerLevel);
                        item.RechargeTimeLeft = 400;
                        break;
                    }
                case RandomArtifactPower.ActCharmAnimals:
                    {
                        // Charm multiple animals
                        _saveGame.SpellEffects.CharmAnimals(playerLevel * 2);
                        item.RechargeTimeLeft = 500;
                        break;
                    }
                case RandomArtifactPower.ActCharmOthers:
                    {
                        // Charm multiple monsters
                        _saveGame.SpellEffects.CharmMonsters(playerLevel * 2);
                        item.RechargeTimeLeft = 750;
                        break;
                    }
                case RandomArtifactPower.ActSummonAnimal:
                    {
                        // Summon animals
                        Level.Monsters.SummonSpecificFriendly(Player.MapY, Player.MapX, playerLevel, Constants.SummonAnimalRanger,
                            true);
                        item.RechargeTimeLeft = 200 + Program.Rng.DieRoll(300);
                        break;
                    }
                case RandomArtifactPower.ActSummonPhantom:
                    {
                        // Summon phantom warriors or beasts
                        Profile.Instance.MsgPrint("You summon a phantasmal servant.");
                        Level.Monsters.SummonSpecificFriendly(Player.MapY, Player.MapX, _saveGame.Difficulty,
                            Constants.SummonPhantom, true);
                        item.RechargeTimeLeft = 200 + Program.Rng.DieRoll(200);
                        break;
                    }
                case RandomArtifactPower.ActSummonElemental:
                    {
                        // Summon an elemental, with a 1-in-3 chance of it being hostile
                        if (Program.Rng.DieRoll(3) == 1)
                        {
                            if (Level.Monsters.SummonSpecific(Player.MapY, Player.MapX, (int)(playerLevel * 1.5),
                                Constants.SummonElemental))
                            {
                                Profile.Instance.MsgPrint("An elemental materializes...");
                                Profile.Instance.MsgPrint("You fail to control it!");
                            }
                        }
                        else
                        {
                            if (Level.Monsters.SummonSpecificFriendly(Player.MapY, Player.MapX, (int)(playerLevel * 1.5),
                                Constants.SummonElemental, playerLevel == 50))
                            {
                                Profile.Instance.MsgPrint("An elemental materializes...");
                                Profile.Instance.MsgPrint("It seems obedient to you.");
                            }
                        }
                        item.RechargeTimeLeft = 750;
                        break;
                    }
                case RandomArtifactPower.ActSummonDemon:
                    {
                        // Summon a demon, with a 1-in-3 chance of it being hostile
                        if (Program.Rng.DieRoll(3) == 1)
                        {
                            if (Level.Monsters.SummonSpecific(Player.MapY, Player.MapX, (int)(playerLevel * 1.5),
                                Constants.SummonDemon))
                            {
                                Profile.Instance.MsgPrint("The area fills with a stench of sulphur and brimstone.");
                                Profile.Instance.MsgPrint("'NON SERVIAM! Wretch! I shall feast on thy mortal soul!'");
                            }
                        }
                        else
                        {
                            if (Level.Monsters.SummonSpecificFriendly(Player.MapY, Player.MapX, (int)(playerLevel * 1.5),
                                Constants.SummonDemon, playerLevel == 50))
                            {
                                Profile.Instance.MsgPrint("The area fills with a stench of sulphur and brimstone.");
                                Profile.Instance.MsgPrint("'What is thy bidding... Master?'");
                            }
                        }
                        item.RechargeTimeLeft = 666 + Program.Rng.DieRoll(333);
                        break;
                    }
                case RandomArtifactPower.ActSummonUndead:
                    {
                        // Summon undead, with a 1-in-3 chance of it being hostile
                        if (Program.Rng.DieRoll(3) == 1)
                        {
                            if (Level.Monsters.SummonSpecific(Player.MapY, Player.MapX, (int)(playerLevel * 1.5),
                                playerLevel > 47 ? Constants.SummonHiUndead : Constants.SummonUndead))
                            {
                                Profile.Instance.MsgPrint(
                                    "Cold winds begin to Attack around you, carrying with them the stench of decay...");
                                Profile.Instance.MsgPrint("'The dead arise... to punish you for disturbing them!'");
                            }
                        }
                        else
                        {
                            if (Level.Monsters.SummonSpecificFriendly(Player.MapY, Player.MapX, (int)(playerLevel * 1.5),
                                playerLevel > 47 ? Constants.SummonHiUndeadNoUniques : Constants.SummonUndead,
                                playerLevel > 24 && Program.Rng.DieRoll(3) == 1))
                            {
                                Profile.Instance.MsgPrint(
                                    "Cold winds begin to Attack around you, carrying with them the stench of decay...");
                                Profile.Instance.MsgPrint("Ancient, long-dead forms arise from the ground to serve you!");
                            }
                        }
                        item.RechargeTimeLeft = 666 + Program.Rng.DieRoll(333);
                        break;
                    }
                case RandomArtifactPower.ActCureLw:
                    {
                        // Heal 30 health and remove fear
                        Player.SetTimedFear(0);
                        Player.RestoreHealth(30);
                        item.RechargeTimeLeft = 10;
                        break;
                    }
                case RandomArtifactPower.ActCureMw:
                    {
                        // Heal 4d8 health and reduce bleeding
                        Profile.Instance.MsgPrint("It radiates deep purple...");
                        Player.RestoreHealth(Program.Rng.DiceRoll(4, 8));
                        Player.SetTimedBleeding((Player.TimedBleeding / 2) - 50);
                        item.RechargeTimeLeft = Program.Rng.RandomLessThan(3) + 3;
                        break;
                    }
                case RandomArtifactPower.ActCurePoison:
                    {
                        // Remove fear and poison
                        Profile.Instance.MsgPrint("It glows deep blue...");
                        Player.SetTimedFear(0);
                        Player.SetTimedPoison(0);
                        item.RechargeTimeLeft = 5;
                        break;
                    }
                case RandomArtifactPower.ActRestLife:
                    {
                        // Restore lost experience
                        Profile.Instance.MsgPrint("It glows a deep red...");
                        Player.RestoreLevel();
                        item.RechargeTimeLeft = 450;
                        break;
                    }
                case RandomArtifactPower.ActRestAll:
                    {
                        // Restore all ability drain and lost experience
                        Profile.Instance.MsgPrint("It glows a deep green...");
                        Player.TryRestoringAbilityScore(Ability.Strength);
                        Player.TryRestoringAbilityScore(Ability.Intelligence);
                        Player.TryRestoringAbilityScore(Ability.Wisdom);
                        Player.TryRestoringAbilityScore(Ability.Dexterity);
                        Player.TryRestoringAbilityScore(Ability.Constitution);
                        Player.TryRestoringAbilityScore(Ability.Charisma);
                        Player.RestoreLevel();
                        item.RechargeTimeLeft = 750;
                        break;
                    }
                case RandomArtifactPower.ActCure700:
                    {
                        // Heal 700 health and remove all bleding
                        Profile.Instance.MsgPrint("It glows deep blue...");
                        Profile.Instance.MsgPrint("You feel a warm tingling inside...");
                        Player.RestoreHealth(700);
                        Player.SetTimedBleeding(0);
                        item.RechargeTimeLeft = 250;
                        break;
                    }
                case RandomArtifactPower.ActCure1000:
                    {
                        // Heal 1000 health and remove all bleeding
                        Profile.Instance.MsgPrint("It glows a bright white...");
                        Profile.Instance.MsgPrint("You feel much better...");
                        Player.RestoreHealth(1000);
                        Player.SetTimedBleeding(0);
                        item.RechargeTimeLeft = 888;
                        break;
                    }
                case RandomArtifactPower.ActEsp:
                    {
                        // Give temporary telepathy
                        Player.SetTimedTelepathy(Player.TimedTelepathy + Program.Rng.DieRoll(30) + 25);
                        item.RechargeTimeLeft = 200;
                        break;
                    }
                case RandomArtifactPower.ActBerserk:
                    {
                        // Bless us and make us a superhero
                        Player.SetTimedSuperheroism(Player.TimedSuperheroism + Program.Rng.DieRoll(50) + 50);
                        Player.SetTimedBlessing(Player.TimedBlessing + Program.Rng.DieRoll(50) + 50);
                        item.RechargeTimeLeft = 100 + Program.Rng.DieRoll(100);
                        break;
                    }
                case RandomArtifactPower.ActProtEvil:
                    {
                        // Give us protection from evil
                        Profile.Instance.MsgPrint("It lets out a shrill wail...");
                        int k = 3 * Player.Level;
                        Player.SetTimedProtectionFromEvil(Player.TimedProtectionFromEvil + Program.Rng.DieRoll(25) + k);
                        item.RechargeTimeLeft = Program.Rng.RandomLessThan(225) + 225;
                        break;
                    }
                case RandomArtifactPower.ActResistAll:
                    {
                        // Give us temporary resistance to the basic elements
                        Profile.Instance.MsgPrint("It glows many colours...");
                        Player.SetTimedAcidResistance(Player.TimedAcidResistance + Program.Rng.DieRoll(40) + 40);
                        Player.SetTimedLightningResistance(Player.TimedLightningResistance + Program.Rng.DieRoll(40) + 40);
                        Player.SetTimedFireResistance(Player.TimedFireResistance + Program.Rng.DieRoll(40) + 40);
                        Player.SetTimedColdResistance(Player.TimedColdResistance + Program.Rng.DieRoll(40) + 40);
                        Player.SetTimedPoisonResistance(Player.TimedPoisonResistance + Program.Rng.DieRoll(40) + 40);
                        item.RechargeTimeLeft = 200;
                        break;
                    }
                case RandomArtifactPower.ActSpeed:
                    {
                        // Give us temporary haste
                        Profile.Instance.MsgPrint("It glows bright green...");
                        if (Player.TimedHaste == 0)
                        {
                            Player.SetTimedHaste(Program.Rng.DieRoll(20) + 20);
                        }
                        else
                        {
                            Player.SetTimedHaste(Player.TimedHaste + 5);
                        }
                        item.RechargeTimeLeft = 250;
                        break;
                    }
                case RandomArtifactPower.ActXtraSpeed:
                    {
                        // Give us extra haste for a long time
                        Profile.Instance.MsgPrint("It glows brightly...");
                        if (Player.TimedHaste == 0)
                        {
                            Player.SetTimedHaste(Program.Rng.DieRoll(75) + 75);
                        }
                        else
                        {
                            Player.SetTimedHaste(Player.TimedHaste + 5);
                        }
                        item.RechargeTimeLeft = Program.Rng.RandomLessThan(200) + 200;
                        break;
                    }
                case RandomArtifactPower.ActWraith:
                    {
                        // Give us temporary etherealness
                        Player.SetTimedEtherealness(Player.TimedEtherealness + Program.Rng.DieRoll(playerLevel / 2) + (playerLevel / 2));
                        item.RechargeTimeLeft = 1000;
                        break;
                    }
                case RandomArtifactPower.ActInvuln:
                    {
                        // Give us temporary invulnerabliity
                        Player.SetTimedInvulnerability(Player.TimedInvulnerability + Program.Rng.DieRoll(8) + 8);
                        item.RechargeTimeLeft = 1000;
                        break;
                    }
                case RandomArtifactPower.ActLight:
                    {
                        // Light the area
                        Profile.Instance.MsgPrint("It wells with clear light...");
                        _saveGame.SpellEffects.LightArea(Program.Rng.DiceRoll(2, 15), 3);
                        item.RechargeTimeLeft = Program.Rng.RandomLessThan(10) + 10;
                        break;
                    }
                case RandomArtifactPower.ActMapLight:
                    {
                        // Map the local area
                        Profile.Instance.MsgPrint("It shines brightly...");
                        Level.MapArea();
                        _saveGame.SpellEffects.LightArea(Program.Rng.DiceRoll(2, 15), 3);
                        item.RechargeTimeLeft = Program.Rng.RandomLessThan(50) + 50;
                        break;
                    }
                case RandomArtifactPower.ActDetectAll:
                    {
                        // Detect everything
                        Profile.Instance.MsgPrint("It glows bright white...");
                        Profile.Instance.MsgPrint("An image forms in your mind...");
                        _saveGame.SpellEffects.DetectAll();
                        item.RechargeTimeLeft = Program.Rng.RandomLessThan(55) + 55;
                        break;
                    }
                case RandomArtifactPower.ActDetectXtra:
                    {
                        // Detect everything and do probing and identify an item fully
                        Profile.Instance.MsgPrint("It glows brightly...");
                        _saveGame.SpellEffects.DetectAll();
                        _saveGame.SpellEffects.Probing();
                        _saveGame.SpellEffects.IdentifyFully();
                        item.RechargeTimeLeft = 1000;
                        break;
                    }
                case RandomArtifactPower.ActIdFull:
                    {
                        // Identify an item fully
                        Profile.Instance.MsgPrint("It glows yellow...");
                        _saveGame.SpellEffects.IdentifyFully();
                        item.RechargeTimeLeft = 750;
                        break;
                    }
                case RandomArtifactPower.ActIdPlain:
                    {
                        // Identify an item
                        if (!_saveGame.SpellEffects.IdentifyItem())
                        {
                            return;
                        }
                        item.RechargeTimeLeft = 10;
                        break;
                    }
                case RandomArtifactPower.ActRuneExplo:
                    {
                        // Place a Yellow Sign
                        Profile.Instance.MsgPrint("It glows a sickly yellow...");
                        _saveGame.SpellEffects.YellowSign();
                        item.RechargeTimeLeft = 200;
                        break;
                    }
                case RandomArtifactPower.ActRuneProt:
                    {
                        // Place an ElderSign
                        Profile.Instance.MsgPrint("It glows light blue...");
                        _saveGame.SpellEffects.ElderSign();
                        item.RechargeTimeLeft = 400;
                        break;
                    }
                case RandomArtifactPower.ActSatiate:
                    {
                        // Fill us up
                        Player.SetFood(Constants.PyFoodMax - 1);
                        item.RechargeTimeLeft = 200;
                        break;
                    }
                case RandomArtifactPower.ActDestDoor:
                    {
                        // Destroy nearby doors
                        Profile.Instance.MsgPrint("It glows bright red...");
                        _saveGame.SpellEffects.DestroyDoorsTouch();
                        item.RechargeTimeLeft = 10;
                        break;
                    }
                case RandomArtifactPower.ActStoneMud:
                    {
                        // Rock to mud
                        Profile.Instance.MsgPrint("It pulsates...");
                        if (!targetEngine.GetDirectionWithAim(out direction))
                        {
                            return;
                        }
                        _saveGame.SpellEffects.WallToMud(direction);
                        item.RechargeTimeLeft = 5;
                        break;
                    }
                case RandomArtifactPower.ActRecharge:
                    {
                        // Recharge an item
                        _saveGame.SpellEffects.Recharge(60);
                        item.RechargeTimeLeft = 70;
                        break;
                    }
                case RandomArtifactPower.ActAlchemy:
                    {
                        // Turn an item to gold
                        Profile.Instance.MsgPrint("It glows bright yellow...");
                        _saveGame.SpellEffects.Alchemy();
                        item.RechargeTimeLeft = 500;
                        break;
                    }
                case RandomArtifactPower.ActDimDoor:
                    {
                        // Short range teleport to a specific destination
                        Profile.Instance.MsgPrint("You open a dimensional gate. Choose a destination.");
                        if (!targetEngine.TgtPt(out int ii, out int ij))
                        {
                            return;
                        }
                        Player.Energy -= 60 - playerLevel;
                        if (!Level.GridPassableNoCreature(ij, ii) || Level.Grid[ij][ii].TileFlags.IsSet(GridTile.InVault) ||
                            Level.Distance(ij, ii, Player.MapY, Player.MapX) > playerLevel + 2 ||
                            Program.Rng.RandomLessThan(playerLevel * playerLevel / 2) == 0)
                        {
                            Profile.Instance.MsgPrint("You fail to exit the astral plane correctly!");
                            Player.Energy -= 100;
                            _saveGame.SpellEffects.TeleportPlayer(10);
                        }
                        else
                        {
                            _saveGame.SpellEffects.TeleportPlayerTo(ij, ii);
                        }
                        item.RechargeTimeLeft = 100;
                        break;
                    }
                case RandomArtifactPower.ActTeleport:
                    {
                        // Long range teleport
                        Profile.Instance.MsgPrint("It twists space around you...");
                        _saveGame.SpellEffects.TeleportPlayer(100);
                        item.RechargeTimeLeft = 45;
                        break;
                    }
                case RandomArtifactPower.ActRecall:
                    {
                        // Word of Recall
                        Profile.Instance.MsgPrint("It glows soft white...");
                        Player.ToggleRecall();
                        item.RechargeTimeLeft = 200;
                        break;
                    }
                default:
                    Profile.Instance.MsgPrint($"Unknown activation effect: {item.BonusPowerSubType}.");
                    return;
            }
        }

        public bool BashClosedDoor(int y, int x, int dir)
        {
            bool more = false;
            _saveGame.EnergyUse = 100;
            GridTile cPtr = Level.Grid[y][x];
            Profile.Instance.MsgPrint("You smash into the door!");
            int bash = Player.AbilityScores[Ability.Strength].StrAttackSpeedComponent;
            int temp = int.Parse(cPtr.FeatureType.Name.Substring(10));
            temp = bash - (temp * 10);
            if (temp < 1)
            {
                temp = 1;
            }
            if (Program.Rng.RandomLessThan(100) < temp)
            {
                Profile.Instance.MsgPrint("The door crashes open!");
                Level.CaveSetFeat(y, x, Program.Rng.RandomLessThan(100) < 50 ? "BrokenDoor" : "OpenDoor");
                Gui.PlaySound(SoundEffect.OpenDoor);
                MovePlayer(dir, false);
                Player.UpdatesNeeded.Set(UpdateFlags.UpdateView | UpdateFlags.UpdateLight);
                Player.UpdatesNeeded.Set(UpdateFlags.UpdateDistances);
            }
            else if (Program.Rng.RandomLessThan(100) < Player.AbilityScores[Ability.Dexterity].DexTheftAvoidance + Player.Level)
            {
                Profile.Instance.MsgPrint("The door holds firm.");
                more = true;
            }
            else
            {
                Profile.Instance.MsgPrint("You are off-balance.");
                Player.SetTimedParalysis(Player.TimedParalysis + 2 + Program.Rng.RandomLessThan(2));
            }
            return more;
        }

        /// <summary>
        /// Give a fire brand to a set of bolts we're carrying
        /// </summary>
        public void BrandBolts()
        {
            for (int i = 0; i < InventorySlot.Pack; i++)
            {
                // Find a set of non-artifact bolts in our inventory
                Item item = Player.Inventory[i];
                if (item.Category != ItemCategory.Bolt)
                {
                    continue;
                }
                if (!string.IsNullOrEmpty(item.RandartName) || item.IsFixedArtifact() || item.IsRare())
                {
                    continue;
                }
                // Skip cursed or broken bolts
                if (item.IsCursed() || item.IsBroken())
                {
                    continue;
                }
                // Only a 25% chance of success per set of bolts
                if (Program.Rng.RandomLessThan(100) < 75)
                {
                    continue;
                }
                // Make the bolts into bolts of flame
                Profile.Instance.MsgPrint("Your bolts are covered in a fiery aura!");
                item.RareItemTypeIndex = Enumerations.RareItemType.AmmoOfFlame;
                _saveGame.SpellEffects.Enchant(item, Program.Rng.RandomLessThan(3) + 4,
                    Constants.EnchTohit | Constants.EnchTodam);
                // Quit after the first bolts have been upgraded
                return;
            }
            // We fell off the end of the inventory without enchanting anything
            Profile.Instance.MsgPrint("The fiery enchantment failed.");
        }

        /// <summary>
        /// Give a brand type to our melee weapon
        /// </summary>
        /// <param name="brandType"> The type of brand to give the weapon </param>
        public void BrandWeapon(int brandType)
        {
            Item item = Player.Inventory[InventorySlot.MeleeWeapon];
            // We must have a non-rare, non-artifact weapon that isn't cursed
            if (item.ItemType != null && !item.IsFixedArtifact() && !item.IsRare() &&
                string.IsNullOrEmpty(item.RandartName) && !item.IsCursed())
            {
                string act;
                string itemName = item.Description(false, 0);
                switch (brandType)
                {
                    case 4:
                        // Make it a planar weapon
                        act = "seems very unstable now.";
                        item.RareItemTypeIndex = Enumerations.RareItemType.WeaponPlanarWeapon;
                        item.TypeSpecificValue = Program.Rng.DieRoll(2);
                        break;

                    case 3:
                        // Make it a vampiric weapon
                        act = "thirsts for blood!";
                        item.RareItemTypeIndex = Enumerations.RareItemType.WeaponVampiric;
                        break;

                    case 2:
                        // Make it a poison brand
                        act = "is coated with poison.";
                        item.RareItemTypeIndex = Enumerations.RareItemType.WeaponOfPoisoning;
                        break;

                    case 1:
                        // Make it a chaotic weapon
                        act = "is engulfed in raw chaos!";
                        item.RareItemTypeIndex = Enumerations.RareItemType.WeaponChaotic;
                        break;

                    default:
                        // Make it a fire or ice weapon
                        if (Program.Rng.RandomLessThan(100) < 25)
                        {
                            act = "is covered in a fiery shield!";
                            item.RareItemTypeIndex = Enumerations.RareItemType.WeaponOfBurning;
                        }
                        else
                        {
                            act = "glows deep, icy blue!";
                            item.RareItemTypeIndex = Enumerations.RareItemType.WeaponOfFreezing;
                        }
                        break;
                }
                // Let the player know what happened
                Profile.Instance.MsgPrint($"Your {itemName} {act}");
                _saveGame.SpellEffects.Enchant(item, Program.Rng.RandomLessThan(3) + 4,
                    Constants.EnchTohit | Constants.EnchTodam);
            }
            else
            {
                Profile.Instance.MsgPrint("The Branding failed.");
            }
        }

        /// <summary>
        /// Blast energy in all directions
        /// </summary>
        public void CallTheVoid()
        {
            // Make sure we're not next to a wall
            if (Level.GridPassable(Player.MapY - 1, Player.MapX - 1) && Level.GridPassable(Player.MapY - 1, Player.MapX) &&
                Level.GridPassable(Player.MapY - 1, Player.MapX + 1) && Level.GridPassable(Player.MapY, Player.MapX - 1) &&
                Level.GridPassable(Player.MapY, Player.MapX + 1) && Level.GridPassable(Player.MapY + 1, Player.MapX - 1) &&
                Level.GridPassable(Player.MapY + 1, Player.MapX) && Level.GridPassable(Player.MapY + 1, Player.MapX + 1))
            {
                // Fire area effect shards, mana, and nukes in all directions
                int i;
                for (i = 1; i < 10; i++)
                {
                    if (i - 5 != 0)
                    {
                        _saveGame.SpellEffects.FireBall(new ProjectShard(SaveGame.Instance.SpellEffects), i, 175, 2);
                    }
                }
                for (i = 1; i < 10; i++)
                {
                    if (i - 5 != 0)
                    {
                        _saveGame.SpellEffects.FireBall(new ProjectMana(SaveGame.Instance.SpellEffects), i, 175, 3);
                    }
                }
                for (i = 1; i < 10; i++)
                {
                    if (i - 5 != 0)
                    {
                        _saveGame.SpellEffects.FireBall(new ProjectNuke(SaveGame.Instance.SpellEffects), i, 175, 4);
                    }
                }
            }
            else
            {
                // We were too close to a wall, so earthquake instead
                string cast = Player.Spellcasting.Type == CastingType.Divine ? "recite" : "cast";
                string spell = Player.Spellcasting.Type == CastingType.Divine ? "prayer" : "spell";
                Profile.Instance.MsgPrint($"You {cast} the {spell} too close to a wall!");
                Profile.Instance.MsgPrint("There is a loud explosion!");
                _saveGame.SpellEffects.DestroyArea(Player.MapY, Player.MapX, 20 + Player.Level);
                Profile.Instance.MsgPrint("The dungeon collapses...");
                Player.TakeHit(100 + Program.Rng.DieRoll(150), "a suicidal Call the Void");
            }
        }

        /// <summary>
        /// Check to see if a racial power works
        /// </summary>
        /// <param name="minLevel"> The minimum level for the power </param>
        /// <param name="cost"> The cost in mana to use the power </param>
        /// <param name="useStat"> The ability score used for the power </param>
        /// <param name="difficulty"> The difficulty of the power to use </param>
        /// <returns> True if the power worked, false if it didn't </returns>
        public bool CheckIfRacialPowerWorks(int minLevel, int cost, int useStat, int difficulty)
        {
            // If we don't have enough mana we'll use health instead
            bool useHealth = Player.Mana < cost;
            // Can't use it if we're too low level
            if (Player.Level < minLevel)
            {
                Profile.Instance.MsgPrint($"You need to attain level {minLevel} to use this power.");
                _saveGame.EnergyUse = 0;
                return false;
            }
            // Can't use it if we're confused
            if (Player.TimedConfusion != 0)
            {
                Profile.Instance.MsgPrint("You are too confused to use this power.");
                _saveGame.EnergyUse = 0;
                return false;
            }
            // If we're about to kill ourselves, give us chance to back out
            if (useHealth && Player.Health < cost)
            {
                if (!Gui.GetCheck("Really use the power in your weakened state? "))
                {
                    _saveGame.EnergyUse = 0;
                    return false;
                }
            }
            // Harder to use powers when you're stunned
            if (Player.TimedStun != 0)
            {
                difficulty += Player.TimedStun;
            }
            // Easier to use powers if you're higher level than you need to be
            else if (Player.Level > minLevel)
            {
                int levAdj = (Player.Level - minLevel) / 3;
                if (levAdj > 10)
                {
                    levAdj = 10;
                }
                difficulty -= levAdj;
            }
            // We have a minimum difficulty
            if (difficulty < 5)
            {
                difficulty = 5;
            }
            // Using a power takes a turn
            _saveGame.EnergyUse = 100;
            // Reduce our health or mana
            if (useHealth)
            {
                Player.TakeHit((cost / 2) + Program.Rng.DieRoll(cost / 2), "concentrating too hard");
            }
            else
            {
                Player.Mana -= (cost / 2) + Program.Rng.DieRoll(cost / 2);
            }
            // We'll need to redraw
            Player.RedrawNeeded.Set(RedrawFlag.PrHp);
            Player.RedrawNeeded.Set(RedrawFlag.PrMana);
            // Check to see if we were successful
            if (Program.Rng.DieRoll(Player.AbilityScores[useStat].Innate) >=
                (difficulty / 2) + Program.Rng.DieRoll(difficulty / 2))
            {
                return true;
            }
            // Let us know we failed
            Profile.Instance.MsgPrint("You've failed to concentrate hard enough.");
            return false;
        }

        /// <summary>
        /// Close a door
        /// </summary>
        /// <param name="y"> The y map coordinate of the door </param>
        /// <param name="x"> The x map coordinate of the door </param>
        /// <returns> True if the player should be disturbed by the door closing </returns>
        public bool CloseDoor(int y, int x)
        {
            _saveGame.EnergyUse = 100;
            GridTile cPtr = Level.Grid[y][x];
            if (cPtr.FeatureType.Name == "BrokenDoor")
            {
                Profile.Instance.MsgPrint("The door appears to be broken.");
            }
            else
            {
                Level.CaveSetFeat(y, x, "LockedDoor0");
                Player.UpdatesNeeded.Set(UpdateFlags.UpdateView | UpdateFlags.UpdateLight | UpdateFlags.UpdateMonsters);
                Gui.PlaySound(SoundEffect.ShutDoor);
            }
            return false;
        }

        /// <summary>
        /// Count the number of chests adjacent to the player, filling in a map coordinate with the
        /// location of the last one found
        /// </summary>
        /// <param name="mapCoordinate"> The coordinate to fill in with the location </param>
        /// <param name="trappedOnly"> True if we're only interested in trapped chests </param>
        /// <returns> The number of chests </returns>
        public int CountChests(MapCoordinate mapCoordinate, bool trappedOnly)
        {
            int count = 0;
            for (int orderedDirection = 0; orderedDirection < 9; orderedDirection++)
            {
                int yy = Player.MapY + Level.OrderedDirectionYOffset[orderedDirection];
                int xx = Player.MapX + Level.OrderedDirectionXOffset[orderedDirection];
                // Get the index of first item in the tile that is a chest
                int itemIndex;
                if ((itemIndex = Level.ChestCheck(yy, xx)) == 0)
                {
                    // There wasn't a chest on the grid so skip
                    continue;
                }
                // Get the actual item from the index
                Item item = Level.Items[itemIndex];
                if (item.TypeSpecificValue == 0)
                {
                    continue;
                }
                // If we're only interested in trapped chests, skip those that aren't
                if (trappedOnly && (!item.IsKnown() || GlobalData.ChestTraps[item.TypeSpecificValue] == 0))
                {
                    continue;
                }
                count++;
                // Remember the coordinate
                mapCoordinate.Y = yy;
                mapCoordinate.X = xx;
            }
            return count;
        }

        /// <summary>
        /// Count the number of closed doors around the player, filling in a map coordinate with the
        /// location of the last one found
        /// </summary>
        /// <param name="mapCoordinate"> The location around which to search </param>
        /// <returns> The number of closed doors </returns>
        public int CountClosedDoors(MapCoordinate mapCoordinate)
        {
            int count = 0;
            for (int orderedDirection = 0; orderedDirection < 9; orderedDirection++)
            {
                int yy = Player.MapY + Level.OrderedDirectionYOffset[orderedDirection];
                int xx = Player.MapX + Level.OrderedDirectionXOffset[orderedDirection];
                // We need to be aware of the door
                if (Level.Grid[yy][xx].TileFlags.IsClear(GridTile.PlayerMemorised))
                {
                    continue;
                }
                // It needs to be an actual door
                if (!Level.Grid[yy][xx].FeatureType.IsClosedDoor)
                {
                    continue;
                }
                // It can't be a secret door
                if (Level.Grid[yy][xx].FeatureType.Name == "SecretDoor")
                {
                    continue;
                }
                count++;
                // Remember the coordinate
                mapCoordinate.Y = yy;
                mapCoordinate.X = xx;
            }
            return count;
        }

        /// <summary>
        /// Get the number of known traps around the player, storing the map coordinate of the last
        /// one found
        /// </summary>
        /// <param name="mapCoordinate">
        /// The coordinate in which to store the location of the last trap found
        /// </param>
        /// <returns> The number of traps found </returns>
        public int CountKnownTraps(MapCoordinate mapCoordinate)
        {
            int count = 0;
            for (int orderedDirection = 0; orderedDirection < 9; orderedDirection++)
            {
                int yy = Player.MapY + Level.OrderedDirectionYOffset[orderedDirection];
                int xx = Player.MapX + Level.OrderedDirectionXOffset[orderedDirection];
                // We need to be aware of the trap
                if (Level.Grid[yy][xx].TileFlags.IsClear(GridTile.PlayerMemorised))
                {
                    continue;
                }
                // It needs to actually be a trap
                if (!Level.Grid[yy][xx].FeatureType.IsTrap)
                {
                    continue;
                }
                count++;
                // Remember its location
                mapCoordinate.Y = yy;
                mapCoordinate.X = xx;
            }
            return count;
        }

        /// <summary>
        /// Count the number of open doors around the players location, puting the location of the
        /// last one found into a map coordinate
        /// </summary>
        /// <param name="mapCoordinate">
        /// The map coordinate into which the location should be placed
        /// </param>
        /// <returns> The number of open doors found </returns>
        public int CountOpenDoors(MapCoordinate mapCoordinate)
        {
            int count = 0;
            for (int orderedDirection = 0; orderedDirection < 9; orderedDirection++)
            {
                int yy = Player.MapY + Level.OrderedDirectionYOffset[orderedDirection];
                int xx = Player.MapX + Level.OrderedDirectionXOffset[orderedDirection];
                // We must be aware of the door
                if (Level.Grid[yy][xx].TileFlags.IsClear(GridTile.PlayerMemorised))
                {
                    continue;
                }
                // It must actually be an open door
                if (Level.Grid[yy][xx].FeatureType.Name != "OpenDoor")
                {
                    continue;
                }
                count++;
                // Remember the location
                mapCoordinate.Y = yy;
                mapCoordinate.X = xx;
            }
            return count;
        }

        /// <summary>
        /// Create phlogiston to refill a lantern or torch with
        /// </summary>
        public void CreatePhlogiston()
        {
            int maxPhlogiston;
            Item item = Player.Inventory[InventorySlot.Lightsource];
            // Maximum phlogiston is the capacity of the light source
            if (item.Category == ItemCategory.Light && item.ItemSubCategory == LightType.Lantern)
            {
                maxPhlogiston = Constants.FuelLamp;
            }
            else if (item.Category == ItemCategory.Light && item.ItemSubCategory == LightType.Torch)
            {
                maxPhlogiston = Constants.FuelTorch;
            }
            // Probably using an orb or a star essence (or maybe not holding a light source at all)
            else
            {
                Profile.Instance.MsgPrint("You are not wielding anything which uses phlogiston.");
                return;
            }
            // Item is already full
            if (item.TypeSpecificValue >= maxPhlogiston)
            {
                Profile.Instance.MsgPrint("No more phlogiston can be put in this item.");
                return;
            }
            // Add half the max fuel of the item to its current fuel
            item.TypeSpecificValue += maxPhlogiston / 2;
            Profile.Instance.MsgPrint("You add phlogiston to your light item.");
            // Make sure it doesn't overflow
            if (item.TypeSpecificValue >= maxPhlogiston)
            {
                item.TypeSpecificValue = maxPhlogiston;
                Profile.Instance.MsgPrint("Your light item is full.");
            }
            // We need to update our light after this
            Player.UpdatesNeeded.Set(UpdateFlags.UpdateTorchRadius);
        }

        /// <summary>
        /// Heavily curse the players armour
        /// </summary>
        /// <returns> true if there was armour to curse, false otherwise </returns>
        public bool CurseArmour()
        {
            Item item = Player.Inventory[InventorySlot.Body];
            // If we're not wearing armour then nothing can happen
            if (item.ItemType == null)
            {
                return false;
            }
            // Artifacts can't be cursed, and normal armour has a chance to save
            string itemName = item.Description(false, 3);
            if ((!string.IsNullOrEmpty(item.RandartName) || item.IsFixedArtifact()) &&
                Program.Rng.RandomLessThan(100) < 50)
            {
                Profile.Instance.MsgPrint(
                    $"A terrible black aura tries to surround your armour, but your {itemName} resists the effects!");
            }
            else
            {
                // Completely remake the armour into a set of blasted armour
                Profile.Instance.MsgPrint($"A terrible black aura blasts your {itemName}!");
                item.FixedArtifactIndex = 0;
                item.RareItemTypeIndex = Enumerations.RareItemType.ArmourBlasted;
                item.BonusArmourClass = 0 - Program.Rng.DieRoll(5) - Program.Rng.DieRoll(5);
                item.BonusToHit = 0;
                item.BonusDamage = 0;
                item.BaseArmourClass = 0;
                item.DamageDice = 0;
                item.DamageDiceSides = 0;
                item.RandartFlags1.Clear();
                item.RandartFlags2.Clear();
                item.RandartFlags3.Clear();
                item.IdentifyFlags.Set(Constants.IdentCursed);
                item.IdentifyFlags.Set(Constants.IdentBroken);
                Player.UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
                Player.UpdatesNeeded.Set(UpdateFlags.UpdateMana);
            }
            return true;
        }

        /// <summary>
        /// Heavily curse the player's weapon
        /// </summary>
        /// <returns> True if the player was carrying a weapon, false if not </returns>
        public bool CurseWeapon()
        {
            Item item = Player.Inventory[InventorySlot.MeleeWeapon];
            // If we don't have a weapon then nothing happens
            if (item.ItemType == null)
            {
                return false;
            }
            string itemName = item.Description(false, 3);
            // Artifacts can't be cursed, and other items have a chance to resist
            if ((item.IsFixedArtifact() || !string.IsNullOrEmpty(item.RandartName)) &&
                Program.Rng.RandomLessThan(100) < 50)
            {
                Profile.Instance.MsgPrint(
                    $"A terrible black aura tries to surround your weapon, but your {itemName} resists the effects!");
            }
            else
            {
                // Completely remake the item into a shattered weapon
                Profile.Instance.MsgPrint($"A terrible black aura blasts your {itemName}!");
                item.FixedArtifactIndex = 0;
                item.RareItemTypeIndex = Enumerations.RareItemType.WeaponShattered;
                item.BonusToHit = 0 - Program.Rng.DieRoll(5) - Program.Rng.DieRoll(5);
                item.BonusDamage = 0 - Program.Rng.DieRoll(5) - Program.Rng.DieRoll(5);
                item.BonusArmourClass = 0;
                item.BaseArmourClass = 0;
                item.DamageDice = 0;
                item.DamageDiceSides = 0;
                item.RandartFlags1.Clear();
                item.RandartFlags2.Clear();
                item.RandartFlags3.Clear();
                item.IdentifyFlags.Set(Constants.IdentCursed);
                item.IdentifyFlags.Set(Constants.IdentBroken);
                Player.UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
                Player.UpdatesNeeded.Set(UpdateFlags.UpdateMana);
            }
            return true;
        }

        /// <summary>
        /// Disarm a chest at a given location
        /// </summary>
        /// <param name="y"> The y coordinate of the location </param>
        /// <param name="x"> The x coordinate of the location </param>
        /// <param name="itemIndex"> The index of the chest item </param>
        /// <returns> True if the player should be disturbed by the aciton </returns>
        public bool DisarmChest(int y, int x, int itemIndex)
        {
            bool more = false;
            Item item = Level.Items[itemIndex];
            // Disarming a chest takes a turn
            _saveGame.EnergyUse = 100;
            int i = Player.SkillDisarmTraps;
            // Disarming is tricky when you can't see
            if (Player.TimedBlindness != 0 || Level.NoLight())
            {
                i /= 10;
            }
            // Disarming is tricky when confused
            if (Player.TimedConfusion != 0 || Player.TimedHallucinations != 0)
            {
                i /= 10;
            }
            // Penalty for difficulty of trap
            int j = i - item.TypeSpecificValue;
            if (j < 2)
            {
                j = 2;
            }
            // If we don't know about the traps, we don't know what to disarm
            if (!item.IsKnown())
            {
                Profile.Instance.MsgPrint("I don't see any traps.");
            }
            // If it has no traps there's nothing to disarm
            else if (item.TypeSpecificValue <= 0)
            {
                Profile.Instance.MsgPrint("The chest is not trapped.");
            }
            // If it has a null trap then there's nothing to disarm
            else if (GlobalData.ChestTraps[item.TypeSpecificValue] == 0)
            {
                Profile.Instance.MsgPrint("The chest is not trapped.");
            }
            // If we made the skill roll then we disarmed it
            else if (Program.Rng.RandomLessThan(100) < j)
            {
                Profile.Instance.MsgPrint("You have disarmed the chest.");
                Player.GainExperience(item.TypeSpecificValue);
                item.TypeSpecificValue = 0 - item.TypeSpecificValue;
            }
            // If we failed to disarm it there's a chance it goes off
            else if (i > 5 && Program.Rng.DieRoll(i) > 5)
            {
                more = true;
                Profile.Instance.MsgPrint("You failed to disarm the chest.");
            }
            else
            {
                Profile.Instance.MsgPrint("You set off a trap!");
                _saveGame.ChestTrap(y, x, itemIndex);
            }
            return more;
        }

        /// <summary>
        /// Disarm a trap on the floor
        /// </summary>
        /// <param name="y"> The y coordinate of the trap </param>
        /// <param name="x"> The x coordinate of the trap </param>
        /// <param name="dir"> The direction the player should move in </param>
        /// <returns> </returns>
        public bool DisarmTrap(int y, int x, int dir)
        {
            bool more = false;
            // Disarming a trap costs a turn
            _saveGame.EnergyUse = 100;
            GridTile tile = Level.Grid[y][x];
            string trapName = tile.FeatureType.Description;
            int i = Player.SkillDisarmTraps;
            // Difficult, but possible, to disarm by feel
            if (Player.TimedBlindness != 0 || Level.NoLight())
            {
                i /= 10;
            }
            // Difficult to disarm when we're confused
            if (Player.TimedConfusion != 0 || Player.TimedHallucinations != 0)
            {
                i /= 10;
            }
            const int power = 5;
            int j = i - power;
            if (j < 2)
            {
                j = 2;
            }
            // Check the modified disarm skill
            if (Program.Rng.RandomLessThan(100) < j)
            {
                Profile.Instance.MsgPrint($"You have disarmed the {trapName}.");
                Player.GainExperience(power);
                tile.TileFlags.Clear(GridTile.PlayerMemorised);
                Level.RevertTileToBackground(y, x);
                MovePlayer(dir, true);
            }
            // We might set the trap off if we failed to disarm it
            else if (i > 5 && Program.Rng.DieRoll(i) > 5)
            {
                Profile.Instance.MsgPrint($"You failed to disarm the {trapName}.");
                more = true;
            }
            else
            {
                Profile.Instance.MsgPrint($"You set off the {trapName}!");
                MovePlayer(dir, true);
            }
            return more;
        }

        /// <summary>
        /// Channel mana to power an item instead
        /// </summary>
        /// <param name="item"> The item that we wish to power </param>
        /// <returns> True if we successfully channeled it, false if not </returns>
        public bool DoCmdChannel(Item item)
        {
            int cost;
            int price = item.ItemType.Cost;
            // Never channel worthless items
            if (price <= 0)
            {
                return false;
            }
            // Cost to channel is based on how much the item is worth and what type
            switch (item.Category)
            {
                case ItemCategory.Wand:
                    cost = price / 150;
                    break;

                case ItemCategory.Scroll:
                    cost = price / 10;
                    break;

                case ItemCategory.Potion:
                    cost = price / 20;
                    break;

                case ItemCategory.Rod:
                    cost = price / 250;
                    break;

                case ItemCategory.Staff:
                    cost = price / 100;
                    break;

                default:
                    Profile.Instance.MsgPrint("Tried to channel an unknown object type!");
                    return false;
            }
            // Always cost at least 1 mana
            if (cost < 1)
            {
                cost = 1;
            }
            // Spend the mana if we can
            if (cost <= Player.Mana)
            {
                Profile.Instance.MsgPrint("You channel mana to power the effect.");
                Player.Mana -= cost;
                Player.RedrawNeeded.Set(RedrawFlag.PrMana);
                return true;
            }
            // Use some mana in the attempt, even if we failed
            Profile.Instance.MsgPrint("You mana is insufficient to power the effect.");
            Player.Mana -= Program.Rng.RandomLessThan(Player.Mana / 2);
            Player.RedrawNeeded.Set(RedrawFlag.PrMana);
            return false;
        }

        /// <summary>
        /// Give us a rumour, if possible one that we've not heard before
        /// </summary>
        public void GetRumour()
        {
            string rumor;
            // Build an array of all the possible rumours we can get
            char[] rumorType = new char[_saveGame.Quests.Count + Constants.MaxCaves + Constants.MaxCaves];
            int[] rumorIndex = new int[_saveGame.Quests.Count + Constants.MaxCaves + Constants.MaxCaves];
            int maxRumor = 0;
            // Add a rumour for each undiscovered quest
            for (int i = 0; i < _saveGame.Quests.Count; i++)
            {
                if (_saveGame.Quests[i].Level > 0 && !_saveGame.Quests[i].Discovered)
                {
                    rumorType[maxRumor] = 'q';
                    rumorIndex[maxRumor] = i;
                    maxRumor++;
                }
            }
            for (int i = 0; i < Constants.MaxCaves; i++)
            {
                // Add a rumour for each dungeon we don't know the depth of
                if (!_saveGame.Dungeons[i].KnownDepth)
                {
                    rumorType[maxRumor] = 'd';
                    rumorIndex[maxRumor] = i;
                    maxRumor++;
                }
                //Add a rumour for each dungeon we don't know the offset of
                if (!_saveGame.Dungeons[i].KnownOffset)
                {
                    rumorType[maxRumor] = 'o';
                    rumorIndex[maxRumor] = i;
                    maxRumor++;
                }
            }
            // If we already know everything, we're going to need to be told something so add all
            // the quest rumours and we'll get given a repeat of one of those
            if (maxRumor == 0)
            {
                maxRumor = 0;
                for (int i = 0; i < _saveGame.Quests.Count; i++)
                {
                    rumorType[maxRumor] = 'q';
                    rumorIndex[maxRumor] = i;
                    maxRumor++;
                }
            }
            // Pick a random rumour from the list
            int choice = Program.Rng.RandomLessThan(maxRumor);
            char type = rumorType[choice];
            int index = rumorIndex[choice];
            // Give us the appropriate information based on the rumour's type
            if (type == 'q')
            {
                // The rumour describes a quest
                _saveGame.Quests[index].Discovered = true;
                rumor = _saveGame.Quests.DescribeQuest(index);
            }
            else if (type == 'd')
            {
                // The rumour describes a dungeon depth
                Dungeon d = _saveGame.Dungeons[index];
                rumor = d.Tower
                    ? $"They say that {d.Name} has {d.MaxLevel} floors."
                    : $"They say that {d.Name} has {d.MaxLevel} levels.";
                d.KnownDepth = true;
            }
            else
            {
                // The rumour describes a dungeon difficulty
                Dungeon d = _saveGame.Dungeons[index];
                rumor = $"They say that {d.Name} has a relative difficulty of {d.Offset}.";
                d.KnownOffset = true;
            }
            Profile.Instance.MsgPrint(rumor);
        }

        /// <summary>
        /// Find a spike in the player's inventory
        /// </summary>
        /// <param name="inventoryIndex"> The inventory index of the spike found (if any) </param>
        /// <returns> Whether or not a spike was found </returns>
        public bool GetSpike(out int inventoryIndex)
        {
            // Loop through the inventory
            for (int i = 0; i < InventorySlot.Pack; i++)
            {
                Item item = Player.Inventory[i];
                if (item.ItemType == null)
                {
                    continue;
                }
                // If the item is a spike, return it
                if (item.Category == ItemCategory.Spike)
                {
                    inventoryIndex = i;
                    return true;
                }
            }
            // We found nothing, so return false
            inventoryIndex = -1;
            return false;
        }

        /// <summary>
        /// Return whether or not an item can be activated
        /// </summary>
        /// <param name="item"> The item to check </param>
        /// <returns> True if the item can be activated </returns>
        public bool ItemFilterActivatable(Item item)
        {
            FlagSet f1 = new FlagSet();
            FlagSet f2 = new FlagSet();
            FlagSet f3 = new FlagSet();
            if (!item.IsKnown())
            {
                return false;
            }
            item.GetMergedFlags(f1, f2, f3);
            return f3.IsSet(ItemFlag3.Activate);
        }

        /// <summary>
        /// Return whether or not an item is a high level book
        /// </summary>
        /// <param name="item"> The item to check </param>
        /// <returns> True if the item is a high level book </returns>
        public bool ItemFilterHighLevelBook(Item item)
        {
            if (item.Category == ItemCategory.LifeBook || item.Category == ItemCategory.SorceryBook ||
                item.Category == ItemCategory.NatureBook || item.Category == ItemCategory.ChaosBook ||
                item.Category == ItemCategory.DeathBook || item.Category == ItemCategory.TarotBook)
            {
                return item.ItemSubCategory > 1;
            }
            return false;
        }

        /// <summary>
        /// Return whether or not an item can fuel a lantern
        /// </summary>
        /// <param name="item"> The item to check </param>
        /// <returns> True if the item can fuel a lantern </returns>
        public bool ItemFilterLanternFuel(Item item)
        {
            if (item.Category == ItemCategory.Flask)
            {
                return true;
            }
            if (item.Category == ItemCategory.Light && item.ItemSubCategory == LightType.Lantern)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Return whether or not an item can fuel a torch
        /// </summary>
        /// <param name="item"> The item to check </param>
        /// <returns> True if the item can fuel a torch </returns>
        public bool ItemFilterTorchFuel(Item item)
        {
            return item.Category == ItemCategory.Light && item.ItemSubCategory == LightType.Torch;
        }

        /// <summary>
        /// Return whether or not an item can be worn or wielded
        /// </summary>
        /// <param name="item"> The item to check </param>
        /// <returns> True if the item can be worn or wielded </returns>
        public bool ItemFilterWearable(Item item)
        {
            return Player.Inventory.WieldSlot(item) >= InventorySlot.MeleeWeapon;
        }

        /// <summary>
        /// Attempt to move the player in the given direction
        /// </summary>
        /// <param name="direction"> The direction in which to move </param>
        /// <param name="dontPickup"> Whether or not to skip picking up any objects we step on </param>
        public void MovePlayer(int direction, bool dontPickup)
        {
            bool canPassWalls = false;
            int newY = Player.MapY + Level.KeypadDirectionYOffset[direction];
            int newX = Player.MapX + Level.KeypadDirectionXOffset[direction];
            GridTile tile = Level.Grid[newY][newX];
            Monster monster = Level.Monsters[tile.MonsterIndex];
            // Check if we can pass through walls
            if (Player.TimedEtherealness != 0 || Player.RaceIndex == RaceId.Spectre)
            {
                canPassWalls = true;
                // Permanent features can't be passed through even if we could otherwise
                if (Level.Grid[newY][newX].FeatureType.IsPermanent)
                {
                    canPassWalls = false;
                }
            }
            // If there's a monster we can see or an invisible monster on a tile we can move to,
            // deal with it
            if (tile.MonsterIndex != 0 && (monster.IsVisible || Level.GridPassable(newY, newX) || canPassWalls))
            {
                // Check if it's a friend, and if we are in a fit state to distinguish friend from foe
                if ((monster.Mind & Constants.SmFriendly) != 0 &&
                    !(Player.TimedConfusion != 0 || Player.TimedHallucinations != 0 || !monster.IsVisible || Player.TimedStun != 0) &&
                    (Level.GridPassable(newY, newX) || canPassWalls))
                {
                    // Wake up the monster, and track it
                    monster.SleepLevel = 0;
                    string monsterName = monster.MonsterDesc(0);
                    // If we can see it, no need to mention it
                    if (monster.IsVisible)
                    {
                        _saveGame.HealthTrack(tile.MonsterIndex);
                    }
                    // If we can't see it then let us push past it and tell us what happened
                    else if (Level.GridPassable(Player.MapY, Player.MapX) ||
                             (monster.Race.Flags2 & MonsterFlag2.PassWall) != 0)
                    {
                        Profile.Instance.MsgPrint($"You push past {monsterName}.");
                        monster.MapY = Player.MapY;
                        monster.MapX = Player.MapX;
                        Level.Grid[Player.MapY][Player.MapX].MonsterIndex = tile.MonsterIndex;
                        tile.MonsterIndex = 0;
                        Level.Monsters.UpdateMonsterVisibility(Level.Grid[Player.MapY][Player.MapX].MonsterIndex, true);
                    }
                    // If we couldn't push past it, tell us it was in the way
                    else
                    {
                        Profile.Instance.MsgPrint($"{monsterName} is in your way!");
                        _saveGame.EnergyUse = 0;
                        return;
                    }
                }
                // If the monster wasn't friendly, attack it
                else
                {
                    PlayerAttackMonster(newY, newX);
                    return;
                }
            }
            // We didn't attack a monster or get blocked by one, so start testing terrain features
            if (!dontPickup && tile.FeatureType.IsTrap)
            {
                // If we're walking onto a known trap, assume we're trying to disarm it
                DisarmTrap(newY, newX, direction);
                return;
            }
            // If the tile we're moving onto isn't passable then we can't move onto it
            if (!Level.GridPassable(newY, newX) && !canPassWalls)
            {
                _saveGame.Disturb(false);
                // If we can't see it and haven't memories it, tell us what we bumped into
                if (tile.TileFlags.IsClear(GridTile.PlayerMemorised) &&
                    (Player.TimedBlindness != 0 || tile.TileFlags.IsClear(GridTile.PlayerLit)))
                {
                    if (tile.FeatureType.Name == "Rubble")
                    {
                        Profile.Instance.MsgPrint("You feel some rubble blocking your way.");
                        tile.TileFlags.Set(GridTile.PlayerMemorised);
                        Level.RedrawSingleLocation(newY, newX);
                    }
                    else if (tile.FeatureType.Category == FloorTileTypeCategory.Tree)
                    {
                        Profile.Instance.MsgPrint($"You feel a {tile.FeatureType.Description} blocking your way.");
                        tile.TileFlags.Set(GridTile.PlayerMemorised);
                        Level.RedrawSingleLocation(newY, newX);
                    }
                    else if (tile.FeatureType.Name == "Pillar")
                    {
                        Profile.Instance.MsgPrint("You feel a pillar blocking your way.");
                        tile.TileFlags.Set(GridTile.PlayerMemorised);
                        Level.RedrawSingleLocation(newY, newX);
                    }
                    else if (tile.FeatureType.Name.Contains("Water"))
                    {
                        Profile.Instance.MsgPrint("Your way seems to be blocked by water.");
                        tile.TileFlags.Set(GridTile.PlayerMemorised);
                        Level.RedrawSingleLocation(newY, newX);
                    }
                    // If we're moving onto a border, change wilderness location
                    else if (tile.FeatureType.Name.Contains("Border"))
                    {
                        if (_saveGame.Wilderness[Player.WildernessY][Player.WildernessX].Town != null)
                        {
                            _saveGame.CurTown = _saveGame.Wilderness[Player.WildernessY][Player.WildernessX].Town;
                            Profile.Instance.MsgPrint($"You stumble out of {_saveGame.CurTown.Name}.");
                        }
                        if (newY == 0)
                        {
                            Player.MapY = Level.CurHgt - 2;
                            Player.WildernessY--;
                        }
                        if (newY == Level.CurHgt - 1)
                        {
                            Player.MapY = 1;
                            Player.WildernessY++;
                        }
                        if (newX == 0)
                        {
                            Player.MapX = Level.CurWid - 2;
                            Player.WildernessX--;
                        }
                        if (newX == Level.CurWid - 1)
                        {
                            Player.MapX = 1;
                            Player.WildernessX++;
                        }
                        if (_saveGame.Wilderness[Player.WildernessY][Player.WildernessX].Town != null)
                        {
                            _saveGame.CurTown = _saveGame.Wilderness[Player.WildernessY][Player.WildernessX].Town;
                            Profile.Instance.MsgPrint($"You stumble into {_saveGame.CurTown.Name}.");
                            _saveGame.CurTown.Visited = true;
                        }
                        // We'll need a new level
                        _saveGame.NewLevelFlag = true;
                        _saveGame.CameFrom = LevelStart.StartWalk;
                        _saveGame.IsAutosave = true;
                        _saveGame.DoCmdSaveGame();
                        _saveGame.IsAutosave = false;
                    }
                    else if (tile.FeatureType.IsClosedDoor)
                    {
                        Profile.Instance.MsgPrint("You feel a closed door blocking your way.");
                        tile.TileFlags.Set(GridTile.PlayerMemorised);
                        Level.RedrawSingleLocation(newY, newX);
                    }
                    else
                    {
                        Profile.Instance.MsgPrint($"You feel a {tile.FeatureType.Description} blocking your way.");
                        tile.TileFlags.Set(GridTile.PlayerMemorised);
                        Level.RedrawSingleLocation(newY, newX);
                    }
                }
                // We can see it, so give a different message
                else
                {
                    if (tile.FeatureType.Name == "Rubble")
                    {
                        Profile.Instance.MsgPrint("There is rubble blocking your way.");
                        if (!(Player.TimedConfusion != 0 || Player.TimedStun != 0 || Player.TimedHallucinations != 0))
                        {
                            _saveGame.EnergyUse = 0;
                        }
                    }
                    else if (tile.FeatureType.Category == FloorTileTypeCategory.Tree)
                    {
                        Profile.Instance.MsgPrint($"There is a {tile.FeatureType.Description} blocking your way.");
                        tile.TileFlags.Set(GridTile.PlayerMemorised);
                        Level.RedrawSingleLocation(newY, newX);
                    }
                    else if (tile.FeatureType.Name == "Pillar")
                    {
                        Profile.Instance.MsgPrint("There is a pillar blocking your way.");
                        tile.TileFlags.Set(GridTile.PlayerMemorised);
                        Level.RedrawSingleLocation(newY, newX);
                    }
                    else if (tile.FeatureType.Name.Contains("Water"))
                    {
                        Profile.Instance.MsgPrint("You cannot swim.");
                        tile.TileFlags.Set(GridTile.PlayerMemorised);
                        Level.RedrawSingleLocation(newY, newX);
                    }
                    // Again, walking onto a border means a change of wilderness grid
                    else if (tile.FeatureType.Name.Contains("Border"))
                    {
                        if (_saveGame.Wilderness[Player.WildernessY][Player.WildernessX].Town != null)
                        {
                            _saveGame.CurTown = _saveGame.Wilderness[Player.WildernessY][Player.WildernessX].Town;
                            Profile.Instance.MsgPrint($"You leave {_saveGame.CurTown.Name}.");
                            _saveGame.CurTown.Visited = true;
                        }
                        if (newY == 0)
                        {
                            Player.MapY = Level.CurHgt - 2;
                            Player.WildernessY--;
                        }
                        if (newY == Level.CurHgt - 1)
                        {
                            Player.MapY = 1;
                            Player.WildernessY++;
                        }
                        if (newX == 0)
                        {
                            Player.MapX = Level.CurWid - 2;
                            Player.WildernessX--;
                        }
                        if (newX == Level.CurWid - 1)
                        {
                            Player.MapX = 1;
                            Player.WildernessX++;
                        }
                        if (_saveGame.Wilderness[Player.WildernessY][Player.WildernessX].Town != null)
                        {
                            _saveGame.CurTown = _saveGame.Wilderness[Player.WildernessY][Player.WildernessX].Town;
                            Profile.Instance.MsgPrint($"You enter {_saveGame.CurTown.Name}.");
                            _saveGame.CurTown.Visited = true;
                        }
                        // We need a new map
                        _saveGame.NewLevelFlag = true;
                        _saveGame.CameFrom = LevelStart.StartWalk;
                        _saveGame.IsAutosave = true;
                        _saveGame.DoCmdSaveGame();
                        _saveGame.IsAutosave = false;
                    }
                    // If we can see that we're walking into a closed door, try to open it
                    else if (tile.FeatureType.IsClosedDoor)
                    {
                        if (EasyOpenDoor(newY, newX))
                        {
                            return;
                        }
                    }
                    else
                    {
                        Profile.Instance.MsgPrint($"There is a {tile.FeatureType.Description} blocking your way.");
                        if (!(Player.TimedConfusion != 0 || Player.TimedStun != 0 || Player.TimedHallucinations != 0))
                        {
                            _saveGame.EnergyUse = 0;
                        }
                    }
                }
                Gui.PlaySound(SoundEffect.BumpWall);
                return;
            }
            // Assuming we didn't bump into anything, maybe we can actually move
            bool oldTrapsDetected = Level.Grid[Player.MapY][Player.MapX].TileFlags.IsSet(GridTile.TrapsDetected);
            bool newTrapsDetected = Level.Grid[newY][newX].TileFlags.IsSet(GridTile.TrapsDetected);
            // If we're moving into or out of an area where we've detected traps, remember to redraw
            // the notification
            if (oldTrapsDetected != newTrapsDetected)
            {
                Player.RedrawNeeded.Set(RedrawFlag.PrDtrap);
            }
            // If we're leaving an area where we've detected traps at a run, then stop running
            if (_saveGame.Running != 0 && oldTrapsDetected && !newTrapsDetected)
            {
                if (!(Player.TimedConfusion != 0 || Player.TimedStun != 0 || Player.TimedHallucinations != 0))
                {
                    _saveGame.EnergyUse = 0;
                }
                _saveGame.Disturb(false);
                return;
            }
            // We've run out of things that could prevent us moving, so do the move
            int oldY = Player.MapY;
            int oldX = Player.MapX;
            Player.MapY = newY;
            Player.MapX = newX;
            // Redraw our old and new locations
            Level.RedrawSingleLocation(Player.MapY, Player.MapX);
            Level.RedrawSingleLocation(oldY, oldX);
            // Recenter the screen if we have to
            TargetEngine targetEngine = new TargetEngine(Player, Level);
            targetEngine.RecenterScreenAroundPlayer();
            // We'll need to update and redraw various things
            Player.UpdatesNeeded.Set(UpdateFlags.UpdateView | UpdateFlags.UpdateLight | UpdateFlags.UpdateScent);
            Player.UpdatesNeeded.Set(UpdateFlags.UpdateDistances);
            Player.RedrawNeeded.Set(RedrawFlag.PrMap);
            // If we're not actively searching, then have a chance of doing it passively
            if (Player.SkillSearchFrequency >= 50 || 0 == Program.Rng.RandomLessThan(50 - Player.SkillSearchFrequency))
            {
                Search();
            }
            // If we're actively searching then always do it
            if (Player.IsSearching)
            {
                Search();
            }
            // Pick up an object on our tile if there is one
            PickUpItems(!dontPickup);
            // If we've just entered a shop tile, then enter the actual shop
            if (tile.FeatureType.IsShop)
            {
                _saveGame.Disturb(false);
                Gui.QueuedCommand = '_';
            }
            // If we've just stepped on an unknown trap then activate it
            else if (tile.FeatureType.Name == "Invis")
            {
                _saveGame.Disturb(false);
                Profile.Instance.MsgPrint("You found a trap!");
                _saveGame.Level.PickTrap(Player.MapY, Player.MapX);
                StepOnTrap();
            }
            // If it's a trap we couldn't (or didn't) disarm, then activate it
            else if (tile.FeatureType.IsTrap)
            {
                _saveGame.Disturb(false);
                StepOnTrap();
            }
        }

        /// <summary>
        /// Open a chest at a given location
        /// </summary>
        /// <param name="y"> The y coordinate of the location </param>
        /// <param name="x"> The x coordinate of the location </param>
        /// <param name="itemIndex"> The index of the chest item </param>
        /// <returns> Whether or not the player should be disturbed by the action </returns>
        public bool OpenChest(int y, int x, int itemIndex)
        {
            bool openedSuccessfully = true;
            bool more = false;
            Item item = Level.Items[itemIndex];
            // Opening a chest takes an action
            _saveGame.EnergyUse = 100;
            // If the chest is locked, we may need to pick it
            if (item.TypeSpecificValue > 0)
            {
                openedSuccessfully = false;
                // Our disable traps skill also doubles up as a lockpicking skill
                int i = Player.SkillDisarmTraps;
                // Hard to pick locks in the dark
                if (Player.TimedBlindness != 0 || Level.NoLight())
                {
                    i /= 10;
                }
                // Hard to pick locks when you're confused or hallucinating
                if (Player.TimedConfusion != 0 || Player.TimedHallucinations != 0)
                {
                    i /= 10;
                }
                // Some locks are harder to pick than others
                int j = i - item.TypeSpecificValue;
                if (j < 2)
                {
                    j = 2;
                }
                // See if we succeeded
                if (Program.Rng.RandomLessThan(100) < j)
                {
                    Profile.Instance.MsgPrint("You have picked the lock.");
                    Player.GainExperience(1);
                    openedSuccessfully = true;
                }
                else
                {
                    more = true;
                    Profile.Instance.MsgPrint("You failed to pick the lock.");
                }
            }
            // If we successfully opened it, set of any traps and then actually open the chest
            if (openedSuccessfully)
            {
                _saveGame.ChestTrap(y, x, itemIndex);
                _saveGame.OpenChest(y, x, itemIndex);
            }
            return more;
        }

        /// <summary>
        /// Open a door at a given location
        /// </summary>
        /// <param name="y"> The y coordinate of the location </param>
        /// <param name="x"> The x coordinate of the location </param>
        /// <returns> True if opening the door should disturb the player </returns>
        public bool OpenDoor(int y, int x)
        {
            bool more = false;
            // Opening a door takes an action
            _saveGame.EnergyUse = 100;
            GridTile tile = Level.Grid[y][x];
            // Some doors are simply jammed
            if (tile.FeatureType.Name.Contains("Jammed"))
            {
                Profile.Instance.MsgPrint("The door appears to be stuck.");
            }
            // Some doors are locked
            else if (tile.FeatureType.Name != "LockedDoor0")
            {
                // Our disarm traps skill doubles up as a lockpicking skill
                int i = Player.SkillDisarmTraps;
                // Hard to pick locks when you can't see
                if (Player.TimedBlindness != 0 || Level.NoLight())
                {
                    i /= 10;
                }
                // Hard to pick locks when you're confused or hallucinating
                if (Player.TimedConfusion != 0 || Player.TimedHallucinations != 0)
                {
                    i /= 10;
                }
                // Work out the difficulty from the feature name
                int j = int.Parse(tile.FeatureType.Name.Substring(10));
                j = i - (j * 4);
                if (j < 2)
                {
                    j = 2;
                }
                // Check if we succeeded in opening it
                if (Program.Rng.RandomLessThan(100) < j)
                {
                    Profile.Instance.MsgPrint("You have picked the lock.");
                    Level.CaveSetFeat(y, x, "OpenDoor");
                    Player.UpdatesNeeded.Set(UpdateFlags.UpdateView | UpdateFlags.UpdateLight | UpdateFlags.UpdateMonsters);
                    Gui.PlaySound(SoundEffect.LockpickSuccess);
                    // Picking a lock gains you an experience point
                    Player.GainExperience(1);
                }
                else
                {
                    Profile.Instance.MsgPrint("You failed to pick the lock.");
                    more = true;
                }
            }
            // If the door wasn't locked, simply open it
            else
            {
                Level.CaveSetFeat(y, x, "OpenDoor");
                Player.UpdatesNeeded.Set(UpdateFlags.UpdateView | UpdateFlags.UpdateLight | UpdateFlags.UpdateMonsters);
                Gui.PlaySound(SoundEffect.OpenDoor);
            }
            return more;
        }

        /// <summary>
        /// Step onto a grid with an item, possibly picking it up and possibly stomping on it
        /// </summary>
        /// <param name="pickup">
        /// True if we should pick up the object, or false if we should leave it where it is
        /// </param>
        public void PickUpItems(bool pickup)
        {
            GridTile tile = Level.Grid[Player.MapY][Player.MapX];
            int nextItemIndex;
            for (int thisItemIndex = tile.ItemIndex; thisItemIndex != 0; thisItemIndex = nextItemIndex)
            {
                Item item = Level.Items[thisItemIndex];
                string itemName = item.Description(true, 3);
                nextItemIndex = item.NextInStack;
                _saveGame.Disturb(false);
                // We always pick up gold
                if (item.Category == ItemCategory.Gold)
                {
                    Profile.Instance.MsgPrint($"You collect {item.TypeSpecificValue} gold pieces worth of {itemName}.");
                    Player.Gold += item.TypeSpecificValue;
                    Player.RedrawNeeded.Set(RedrawFlag.PrGold);
                    _saveGame.Level.DeleteObjectIdx(thisItemIndex);
                }
                else
                {
                    // If we're not interested, simply say that we see it
                    if (!pickup)
                    {
                        Profile.Instance.MsgPrint($"You see {itemName}.");
                    }
                    // If it's worthless, stomp on it
                    else if (item.Stompable())
                    {
                        _saveGame.Level.DeleteObjectIdx(thisItemIndex);
                        Profile.Instance.MsgPrint($"You stomp on {itemName}.");
                    }
                    // If we can't carry the item, let us know
                    else if (!Player.Inventory.InvenCarryOkay(item))
                    {
                        Profile.Instance.MsgPrint($"You have no room for {itemName}.");
                    }
                    else
                    {
                        // Actually pick up the item
                        int slot = Player.Inventory.InvenCarry(item, false);
                        item = Player.Inventory[slot];
                        itemName = item.Description(true, 3);
                        Profile.Instance.MsgPrint($"You have {itemName} ({slot.IndexToLabel()}).");
                        _saveGame.Level.DeleteObjectIdx(thisItemIndex);
                    }
                }
            }
        }

        /// <summary>
        /// Have the player attack a monster
        /// </summary>
        /// <param name="y"> The y coordinate of the location being attacked </param>
        /// <param name="x"> The x coordinate of the location being attacked </param>
        public void PlayerAttackMonster(int y, int x)
        {
            GridTile tile = Level.Grid[y][x];
            Monster monster = Level.Monsters[tile.MonsterIndex];
            MonsterRace race = monster.Race;
            bool fear = false;
            bool backstab = false;
            bool stabFleeing = false;
            bool doQuake = false;
            const bool drainMsg = true;
            int drainResult = 0;
            const int drainLeft = _maxVampiricDrain;
            FlagSet f1 = new FlagSet();
            FlagSet f2 = new FlagSet();
            FlagSet f3 = new FlagSet();
            bool noExtra = false;
            _saveGame.Disturb(false);
            // If we're a rogue then we can backstab monsters
            if (Player.ProfessionIndex == CharacterClass.Rogue)
            {
                if (monster.SleepLevel != 0 && monster.IsVisible)
                {
                    backstab = true;
                }
                else if (monster.FearLevel != 0 && monster.IsVisible)
                {
                    stabFleeing = true;
                }
            }
            _saveGame.Disturb(true);
            // Being attacked always wakes a monster
            monster.SleepLevel = 0;
            string monsterName = monster.MonsterDesc(0);
            // If we can see the monster, track its health
            if (monster.IsVisible)
            {
                _saveGame.HealthTrack(tile.MonsterIndex);
            }
            // if the monster is our friend and we're not confused, we can avoid hitting it
            if ((monster.Mind & Constants.SmFriendly) != 0 &&
                !(Player.TimedStun != 0 || Player.TimedConfusion != 0 || Player.TimedHallucinations != 0 || !monster.IsVisible))
            {
                Profile.Instance.MsgPrint($"You stop to avoid hitting {monsterName}.");
                return;
            }
            // Can't attack if we're afraid
            if (Player.TimedFear != 0)
            {
                Profile.Instance.MsgPrint(monster.IsVisible
                    ? $"You are too afraid to attack {monsterName}!"
                    : "There is something scary in your way!");
                return;
            }
            Item item = Player.Inventory[InventorySlot.MeleeWeapon];
            int bonus = Player.AttackBonus + item.BonusToHit;
            int chance = Player.SkillMelee + (bonus * Constants.BthPlusAdj);
            // Attacking uses a full turn
            _saveGame.EnergyUse = 100;
            int num = 0;
            // We have a number of attacks per round
            while (num++ < Player.MeleeAttacksPerRound)
            {
                // Check if we hit
                if (PlayerCheckHitOnMonster(chance, race.ArmourClass, monster.IsVisible))
                {
                    PlayerStatus playerStatus = new PlayerStatus(Player, Level);
                    Gui.PlaySound(SoundEffect.MeleeHit);
                    // Tell the player they hit it with the appropriate message
                    if (!(backstab || stabFleeing))
                    {
                        if (!((Player.ProfessionIndex == CharacterClass.Monk || Player.ProfessionIndex == CharacterClass.Mystic) && playerStatus.MartialArtistEmptyHands()))
                        {
                            Profile.Instance.MsgPrint($"You hit {monsterName}.");
                        }
                    }
                    else if (backstab)
                    {
                        Profile.Instance.MsgPrint(
                            $"You cruelly stab the helpless, sleeping {monster.Race.Name}!");
                    }
                    else
                    {
                        Profile.Instance.MsgPrint(
                            $"You backstab the fleeing {monster.Race.Name}!");
                    }
                    // Default to 1 damage for an unarmed hit
                    int totalDamage = 1;
                    // Get our weapon's flags to see if we need to do anything special
                    item.GetMergedFlags(f1, f2, f3);
                    bool chaosEffect = f1.IsSet(ItemFlag1.Chaotic) && Program.Rng.DieRoll(2) == 1;
                    if (f1.IsSet(ItemFlag1.Vampiric) || (chaosEffect && Program.Rng.DieRoll(5) < 3))
                    {
                        // Vampiric overrides chaotic
                        chaosEffect = false;
                        if (!((race.Flags3 & MonsterFlag3.Undead) != 0 || (race.Flags3 & MonsterFlag3.Nonliving) != 0))
                        {
                            drainResult = monster.Health;
                        }
                        else
                        {
                            drainResult = 0;
                        }
                    }
                    // Vorpal weapons have a chance of a deep cut
                    bool vorpalCut = f1.IsSet(ItemFlag1.Vorpal) &&
                        Program.Rng.DieRoll(item.FixedArtifactIndex == FixedArtifactId.SwordVorpalBlade ? 3 : 6) == 1;
                    // If we're a martial artist then we have special attacks
                    if ((Player.ProfessionIndex == CharacterClass.Monk || Player.ProfessionIndex == CharacterClass.Mystic) && playerStatus.MartialArtistEmptyHands())
                    {
                        int specialEffect = 0;
                        int stunEffect = 0;
                        int times;
                        MartialArtsAttack martialArtsAttack = GlobalData.MaBlows[0];
                        MartialArtsAttack oldMartialArtsAttack = GlobalData.MaBlows[0];
                        // Monsters of various types resist being stunned by martial arts
                        int resistStun = 0;
                        if ((race.Flags1 & MonsterFlag1.Unique) != 0)
                        {
                            resistStun += 88;
                        }
                        if ((race.Flags3 & MonsterFlag3.ImmuneConfusion) != 0)
                        {
                            resistStun += 44;
                        }
                        if ((race.Flags3 & MonsterFlag3.ImmuneSleep) != 0)
                        {
                            resistStun += 44;
                        }
                        if ((race.Flags3 & MonsterFlag3.Undead) != 0 || (race.Flags3 & MonsterFlag3.Nonliving) != 0)
                        {
                            resistStun += 88;
                        }
                        // Have a number of attempts to choose a martial arts attack
                        for (times = 0; times < (Player.Level < 7 ? 1 : Player.Level / 7); times++)
                        {
                            // Choose an attack randomly, but reject it and re-choose if it's too
                            // high level or we fail a chance roll
                            do
                            {
                                martialArtsAttack = GlobalData.MaBlows[Program.Rng.DieRoll(Constants.MaxMa) - 1];
                            } while (martialArtsAttack.MinLevel > Player.Level || Program.Rng.DieRoll(Player.Level) < martialArtsAttack.Chance);
                            // We've chosen an attack, use it if it's better than the previous
                            // choice (unless we're stunned or confused in which case we're stuck
                            // with the weakest attack type
                            if (martialArtsAttack.MinLevel > oldMartialArtsAttack.MinLevel && !(Player.TimedStun != 0 || Player.TimedConfusion != 0))
                            {
                                oldMartialArtsAttack = martialArtsAttack;
                            }
                            else
                            {
                                martialArtsAttack = oldMartialArtsAttack;
                            }
                        }
                        // Get damage from the martial arts attack
                        totalDamage = Program.Rng.DiceRoll(martialArtsAttack.Dd, martialArtsAttack.Ds);
                        // If it was a knee attack and the monster is male, hit it in the groin
                        if (martialArtsAttack.Effect == Constants.MaKnee)
                        {
                            if ((race.Flags1 & MonsterFlag1.Male) != 0)
                            {
                                Profile.Instance.MsgPrint($"You hit {monsterName} in the groin with your knee!");
                                specialEffect = Constants.MaKnee;
                            }
                            else
                            {
                                Profile.Instance.MsgPrint(string.Format(martialArtsAttack.Desc, monsterName));
                            }
                        }
                        // If it was an ankle kick and the monster has legs, slow it
                        else if (martialArtsAttack.Effect == Constants.MaSlow)
                        {
                            if ((race.Flags1 & MonsterFlag1.NeverMove) == 0 ||
                                "UjmeEv$,DdsbBFIJQSXclnw!=?".Contains(race.Character.ToString()))
                            {
                                Profile.Instance.MsgPrint($"You kick {monsterName} in the ankle.");
                                specialEffect = Constants.MaSlow;
                            }
                            else
                            {
                                Profile.Instance.MsgPrint(string.Format(martialArtsAttack.Desc, monsterName));
                            }
                        }
                        // Have a chance of stunning based on the martial arts attack type chosen
                        else
                        {
                            if (martialArtsAttack.Effect != 0)
                            {
                                stunEffect = (martialArtsAttack.Effect / 2) + Program.Rng.DieRoll(martialArtsAttack.Effect / 2);
                            }
                            Profile.Instance.MsgPrint(string.Format(martialArtsAttack.Desc, monsterName));
                        }
                        // It might be a critical hit
                        totalDamage = PlayerCriticalMelee(Player.Level * Program.Rng.DieRoll(10), martialArtsAttack.MinLevel, totalDamage);
                        // Make a groin attack into a stunning attack
                        if (specialEffect == Constants.MaKnee && totalDamage + Player.DamageBonus < monster.Health)
                        {
                            Profile.Instance.MsgPrint($"{monsterName} moans in agony!");
                            stunEffect = 7 + Program.Rng.DieRoll(13);
                            resistStun /= 3;
                        }
                        // Slow if we had a knee attack
                        else if (specialEffect == Constants.MaSlow && totalDamage + Player.DamageBonus < monster.Health)
                        {
                            if ((race.Flags1 & MonsterFlag1.Unique) == 0 && Program.Rng.DieRoll(Player.Level) > race.Level &&
                                monster.Speed > 60)
                            {
                                Profile.Instance.MsgPrint($"{monsterName} starts limping slower.");
                                monster.Speed -= 10;
                            }
                        }
                        // Stun if we had a stunning attack
                        if (stunEffect != 0 && totalDamage + Player.DamageBonus < monster.Health)
                        {
                            if (Player.Level > Program.Rng.DieRoll(race.Level + resistStun + 10))
                            {
                                Profile.Instance.MsgPrint(monster.StunLevel != 0
                                    ? $"{monsterName} is more stunned."
                                    : $"{monsterName} is stunned.");
                                monster.StunLevel += stunEffect;
                            }
                        }
                    }
                    // We have a weapon
                    else if (item.ItemType != null)
                    {
                        // Roll damage for the weapon
                        totalDamage = Program.Rng.DiceRoll(item.DamageDice, item.DamageDiceSides);
                        totalDamage = item.AdjustDamageForMonsterType(totalDamage, monster);
                        // Extra damage for backstabbing
                        if (backstab)
                        {
                            totalDamage *= 3 + (Player.Level / 40);
                        }
                        else if (stabFleeing)
                        {
                            totalDamage = 3 * totalDamage / 2;
                        }
                        // We might need to do an earthquake
                        if ((Player.HasQuakeWeapon && (totalDamage > 50 || Program.Rng.DieRoll(7) == 1)) ||
                            (chaosEffect && Program.Rng.DieRoll(250) == 1))
                        {
                            doQuake = true;
                            chaosEffect = false;
                        }
                        // Check if we did a critical
                        totalDamage = PlayerCriticalMelee(item.Weight, item.BonusToHit, totalDamage);
                        // If we did a vorpal cut, do extra damage
                        if (vorpalCut)
                        {
                            int stepK = totalDamage;
                            Profile.Instance.MsgPrint(item.FixedArtifactIndex == FixedArtifactId.SwordVorpalBlade
                                ? "Your Vorpal Blade goes snicker-snack!"
                                : $"Your weapon cuts deep into {monsterName}!");
                            do
                            {
                                totalDamage += stepK;
                            } while (Program.Rng.DieRoll(item.FixedArtifactIndex == FixedArtifactId.SwordVorpalBlade
                                         ? 2
                                         : 4) == 1);
                        }
                        // Add bonus damage for the weapon
                        totalDamage += item.BonusDamage;
                    }
                    // Add bonus damage for strength etc.
                    totalDamage += Player.DamageBonus;
                    // Can't do negative damage
                    if (totalDamage < 0)
                    {
                        totalDamage = 0;
                    }
                    // Apply damage to the monster
                    if (Level.Monsters.DamageMonster(tile.MonsterIndex, totalDamage, out fear, null))
                    {
                        // Can't have any more attacks because the monster's dead
                        noExtra = true;
                        break;
                    }
                    // Hitting a friend gets it angry
                    if ((monster.Mind & Constants.SmFriendly) != 0)
                    {
                        Profile.Instance.MsgPrint($"{monsterName} gets angry!");
                        monster.Mind &= ~Constants.SmFriendly;
                    }
                    // The monster might have an aura that hurts the player
                    TouchZapPlayer(monster);
                    // If we drain health, do so
                    if (drainResult != 0)
                    {
                        // drainResult was set to the monsters health before we hit it, so
                        // subtracting the monster's new health lets us know how much damage we've done
                        drainResult -= monster.Health;
                        if (drainResult > 0)
                        {
                            // Draining heals us
                            int drainHeal = Program.Rng.DiceRoll(4, drainResult / 6);
                            // We have a maximum drain per round to prevent it from getting out of
                            // hand if we have multiple attacks
                            if (drainLeft != 0)
                            {
                                if (drainHeal >= drainLeft)
                                {
                                    drainHeal = drainLeft;
                                }
                                if (drainMsg)
                                {
                                    Profile.Instance.MsgPrint($"Your weapon drains life from {monsterName}!");
                                }
                                Player.RestoreHealth(drainHeal);
                            }
                        }
                    }
                    // We might have a confusing touch (or have this effect from a chaos blade)
                    if (Player.HasConfusingTouch || (chaosEffect && Program.Rng.DieRoll(10) != 1))
                    {
                        // If it wasn't from a chaos blade, cancel the confusing touch and let us know
                        Player.HasConfusingTouch = false;
                        if (!chaosEffect)
                        {
                            Profile.Instance.MsgPrint("Your hands stop glowing.");
                        }
                        // Some monsters are immune
                        if ((race.Flags3 & MonsterFlag3.ImmuneConfusion) != 0)
                        {
                            if (monster.IsVisible)
                            {
                                race.Knowledge.RFlags3 |= MonsterFlag3.ImmuneConfusion;
                            }
                            Profile.Instance.MsgPrint($"{monsterName} is unaffected.");
                        }
                        // Even if not immune, the monster might resist
                        else if (Program.Rng.RandomLessThan(100) < race.Level)
                        {
                            Profile.Instance.MsgPrint($"{monsterName} is unaffected.");
                        }
                        // It didn't resist, so it's confused
                        else
                        {
                            Profile.Instance.MsgPrint($"{monsterName} appears confused.");
                            monster.ConfusionLevel += 10 + (Program.Rng.RandomLessThan(Player.Level) / 5);
                        }
                    }
                    // A chaos blade might teleport the monster away
                    else if (chaosEffect && Program.Rng.DieRoll(2) == 1)
                    {
                        Profile.Instance.MsgPrint($"{monsterName} disappears!");
                        _saveGame.SpellEffects.TeleportAway(tile.MonsterIndex, 50);
                        // Can't have any more attacks because the monster isn't here any more
                        noExtra = true;
                        break;
                    }
                    // a chaos blade might polymorph the monsterf
                    else if (chaosEffect && Level.GridPassable(y, x) && Program.Rng.DieRoll(90) > race.Level)
                    {
                        // Can't polymorph a unique or a guardian
                        if (!((race.Flags1 & MonsterFlag1.Unique) != 0 || (race.Flags4 & MonsterFlag4.BreatheChaos) != 0 ||
                              (race.Flags1 & MonsterFlag1.Guardian) != 0))
                        {
                            int newRaceIndex = _saveGame.SpellEffects.PolymorphMonster(monster.Race);
                            if (newRaceIndex != monster.Race.Index)
                            {
                                Profile.Instance.MsgPrint($"{monsterName} changes!");
                                Level.Monsters.DeleteMonsterByIndex(tile.MonsterIndex, true);
                                MonsterRace newRace = Profile.Instance.MonsterRaces[newRaceIndex];
                                Level.Monsters.PlaceMonsterAux(y, x, newRace, false, false, false);
                                monster = Level.Monsters[tile.MonsterIndex];
                                monsterName = monster.MonsterDesc(0);
                                fear = false;
                            }
                        }
                        // Monster was immune to polymorph
                        else
                        {
                            Profile.Instance.MsgPrint($"{monsterName} is unaffected.");
                        }
                    }
                }
                // We missed
                else
                {
                    Gui.PlaySound(SoundEffect.Miss);
                    Profile.Instance.MsgPrint($"You miss {monsterName}.");
                }
            }
            // Only make extra attacks if the monster is still there
            foreach (Mutation naturalAttack in Player.Dna.NaturalAttacks)
            {
                if (!noExtra)
                {
                    PlayerNaturalAttackOnMonster(tile.MonsterIndex, naturalAttack, out fear, out noExtra);
                }
            }
            if (fear && monster.IsVisible && !noExtra)
            {
                Gui.PlaySound(SoundEffect.MonsterFlees);
                Profile.Instance.MsgPrint($"{monsterName} flees in terror!");
            }
            if (doQuake)
            {
                _saveGame.SpellEffects.Earthquake(Player.MapY, Player.MapX, 10);
            }
        }

        /// <summary>
        /// Check whether a ranged attack by the player hits a monster
        /// </summary>
        /// <param name="attackBonus"> The player's attack bonus </param>
        /// <param name="armourClass"> The monster's armour class </param>
        /// <param name="monsterIsVisible"> Whether or not the monster is visible </param>
        /// <returns> True if the player hit the monster, false otherwise </returns>
        public bool PlayerCheckRangedHitOnMonster(int attackBonus, int armourClass, bool monsterIsVisible)
        {
            int k = Program.Rng.RandomLessThan(100);
            // Always a 5% chance to hit and a 5% chance to miss
            if (k < 10)
            {
                return k < 5;
            }
            // If we have no chance of hitting don't bother checking
            if (attackBonus <= 0)
            {
                return false;
            }
            // Invisible monsters are hard to hit
            if (!monsterIsVisible)
            {
                attackBonus = (attackBonus + 1) / 2;
            }
            // Return the hit or miss
            return Program.Rng.RandomLessThan(attackBonus) >= armourClass * 3 / 4;
        }

        /// <summary>
        /// Work out whether the player's missile attack was a critical hit
        /// </summary>
        /// <param name="weight"> The weight of the player's weapon </param>
        /// <param name="plus"> The magical plusses on the weapon </param>
        /// <param name="damage"> The damage done </param>
        /// <returns> The modified damage based on the level of critical </returns>
        public int PlayerCriticalRanged(int weight, int plus, int damage)
        {
            // Chance of a critical is based on weight, level, and plusses
            int i = weight + ((Player.AttackBonus + plus) * 4) + (Player.Level * 2);
            if (Program.Rng.DieRoll(5000) <= i)
            {
                int k = weight + Program.Rng.DieRoll(500);
                if (k < 500)
                {
                    Profile.Instance.MsgPrint("It was a good hit!");
                    damage = (2 * damage) + 5;
                }
                else if (k < 1000)
                {
                    Profile.Instance.MsgPrint("It was a great hit!");
                    damage = (2 * damage) + 10;
                }
                else
                {
                    Profile.Instance.MsgPrint("It was a superb hit!");
                    damage = (3 * damage) + 15;
                }
            }
            return damage;
        }

        /// <summary>
        /// Have a potion affect the player
        /// </summary>
        /// <param name="itemSubCategory"> The sub-category of potion we're drinking </param>
        /// <returns> True if drinking the potion identified it </returns>
        public bool PotionEffect(int itemSubCategory)
        {
            bool identified = false;
            switch (itemSubCategory)
            {
                // Water or apple juice has no effect
                case PotionType.Water:
                case PotionType.AppleJuice:
                    {
                        Profile.Instance.MsgPrint("You feel less thirsty.");
                        identified = true;
                        break;
                    }
                // Slime mold juice has a random effect (calling this function again recusively)
                case PotionType.SlimeMold:
                    {
                        Profile.Instance.MsgPrint("That tastes awful.");
                        PotionEffect(RandomPotion());
                        identified = true;
                        break;
                    }
                // Slowness slows you down
                case PotionType.Slowness:
                    {
                        if (Player.SetTimedSlow(Player.TimedSlow + Program.Rng.DieRoll(25) + 15))
                        {
                            identified = true;
                        }
                        break;
                    }
                // Salt water makes you vomit, but gets rid of poison
                case PotionType.SaltWater:
                    {
                        Profile.Instance.MsgPrint("The saltiness makes you vomit!");
                        Player.SetFood(Constants.PyFoodStarve - 1);
                        Player.SetTimedPoison(0);
                        Player.SetTimedParalysis(Player.TimedParalysis + 4);
                        identified = true;
                        break;
                    }
                // Poison simply poisons you
                case PotionType.Poison:
                    {
                        if (!(Player.HasPoisonResistance || Player.TimedPoisonResistance != 0))
                        {
                            // Hagarg Ryonis can protect you against poison
                            if (Program.Rng.DieRoll(10) <= Player.Religion.GetNamedDeity(Pantheon.GodName.Hagarg_Ryonis).AdjustedFavour)
                            {
                                Profile.Instance.MsgPrint("Hagarg Ryonis's favour protects you!");
                            }
                            else if (Player.SetTimedPoison(Player.TimedPoison + Program.Rng.RandomLessThan(15) + 10))
                            {
                                identified = true;
                            }
                        }
                        break;
                    }
                // Blindness makes you blind
                case PotionType.Blindness:
                    {
                        if (!Player.HasBlindnessResistance)
                        {
                            if (Player.SetTimedBlindness(Player.TimedBlindness + Program.Rng.RandomLessThan(100) + 100))
                            {
                                identified = true;
                            }
                        }
                        break;
                    }
                // Confusion makes you confused and possibly other effects
                case PotionType.Confusion:
                    {
                        if (!(Player.HasConfusionResistance || Player.HasChaosResistance))
                        {
                            if (Player.SetTimedConfusion(Player.TimedConfusion + Program.Rng.RandomLessThan(20) + 15))
                            {
                                identified = true;
                            }
                            // 50% chance of having hallucinations
                            if (Program.Rng.DieRoll(2) == 1)
                            {
                                if (Player.SetTimedHallucinations(Player.TimedHallucinations + Program.Rng.RandomLessThan(150) + 150))
                                {
                                    identified = true;
                                }
                            }
                            // 1 in 13 chance of blacking out and waking up somewhere else
                            if (Program.Rng.DieRoll(13) == 1)
                            {
                                identified = true;
                                // 1 in 3 chance of losing your memories after blacking out
                                if (Program.Rng.DieRoll(3) == 1)
                                {
                                    _saveGame.SpellEffects.LoseAllInfo();
                                }
                                else
                                {
                                    Level.WizDark();
                                }
                                _saveGame.SpellEffects.TeleportPlayer(100);
                                Level.WizDark();
                                Profile.Instance.MsgPrint("You wake up somewhere with a sore head...");
                                Profile.Instance.MsgPrint("You can't remember a thing, or how you got here!");
                            }
                        }
                        break;
                    }
                // Sleep paralyses you
                case PotionType.Sleep:
                    {
                        if (!Player.HasFreeAction)
                        {
                            if (Player.SetTimedParalysis(Player.TimedParalysis + Program.Rng.RandomLessThan(4) + 4))
                            {
                                identified = true;
                            }
                        }
                        break;
                    }
                // Lose Memories reduces your experience
                case PotionType.LoseMemories:
                    {
                        if (!Player.HasHoldLife && Player.ExperiencePoints > 0)
                        {
                            Profile.Instance.MsgPrint("You feel your memories fade.");
                            Player.LoseExperience(Player.ExperiencePoints / 4);
                            identified = true;
                        }
                        break;
                    }
                // Ruination does 10d10 damage and reduces all your ability scores, bypassing
                // sustains and divine protection
                case PotionType.Ruination:
                    {
                        Profile.Instance.MsgPrint("Your nerves and muscles feel weak and lifeless!");
                        Player.TakeHit(Program.Rng.DiceRoll(10, 10), "a potion of Ruination");
                        Player.DecreaseAbilityScore(Ability.Dexterity, 25, true);
                        Player.DecreaseAbilityScore(Ability.Wisdom, 25, true);
                        Player.DecreaseAbilityScore(Ability.Constitution, 25, true);
                        Player.DecreaseAbilityScore(Ability.Strength, 25, true);
                        Player.DecreaseAbilityScore(Ability.Charisma, 25, true);
                        Player.DecreaseAbilityScore(Ability.Intelligence, 25, true);
                        identified = true;
                        break;
                    }
                // Weakness tries to reduce your strength
                case PotionType.DecStr:
                    {
                        if (Player.TryDecreasingAbilityScore(Ability.Strength))
                        {
                            identified = true;
                        }
                        break;
                    }
                // Stupidity tries to reduce your intelligence
                case PotionType.DecInt:
                    {
                        if (Player.TryDecreasingAbilityScore(Ability.Intelligence))
                        {
                            identified = true;
                        }
                        break;
                    }
                // Naivety tries to reduce your wisdom
                case PotionType.DecWis:
                    {
                        if (Player.TryDecreasingAbilityScore(Ability.Wisdom))
                        {
                            identified = true;
                        }
                        break;
                    }
                // Clumsiness tries to reduce your dexterity
                case PotionType.DecDex:
                    {
                        if (Player.TryDecreasingAbilityScore(Ability.Dexterity))
                        {
                            identified = true;
                        }
                        break;
                    }
                // Sickliness tries to reduce your constitution
                case PotionType.DecCon:
                    {
                        if (Player.TryDecreasingAbilityScore(Ability.Constitution))
                        {
                            identified = true;
                        }
                        break;
                    }
                // Ugliness tries to reduce your charisma
                case PotionType.DecCha:
                    {
                        if (Player.TryDecreasingAbilityScore(Ability.Charisma))
                        {
                            identified = true;
                        }
                        break;
                    }
                // Detonations does 50d20 damage, stuns you, and gives you a stupid amount of bleeding
                case PotionType.Detonations:
                    {
                        Profile.Instance.MsgPrint("Massive explosions rupture your body!");
                        Player.TakeHit(Program.Rng.DiceRoll(50, 20), "a potion of Detonation");
                        Player.SetTimedStun(Player.TimedStun + 75);
                        Player.SetTimedBleeding(Player.TimedBleeding + 5000);
                        identified = true;
                        break;
                    }
                // Iocaine simply does 5000 damage
                case PotionType.Death:
                    {
                        Profile.Instance.MsgPrint("A feeling of Death flows through your body.");
                        Player.TakeHit(5000, "a potion of Death");
                        identified = true;
                        break;
                    }
                // Infravision gives you timed infravision
                case PotionType.Infravision:
                    {
                        if (Player.SetTimedInfravision(Player.TimedInfravision + 100 + Program.Rng.DieRoll(100)))
                        {
                            identified = true;
                        }
                        break;
                    }
                // Detect invisible gives you times see invisibility
                case PotionType.DetectInvis:
                    {
                        if (Player.SetTimedSeeInvisibility(Player.TimedSeeInvisibility + 12 + Program.Rng.DieRoll(12)))
                        {
                            identified = true;
                        }
                        break;
                    }
                // Slow poison halves the remaining duration of any poison you have
                case PotionType.SlowPoison:
                    {
                        if (Player.SetTimedPoison(Player.TimedPoison / 2))
                        {
                            identified = true;
                        }
                        break;
                    }
                // Cure poison removes any poison you have
                case PotionType.CurePoison:
                    {
                        if (Player.SetTimedPoison(0))
                        {
                            identified = true;
                        }
                        break;
                    }
                // Boldness stops you being afraid
                case PotionType.Boldness:
                    {
                        if (Player.SetTimedFear(0))
                        {
                            identified = true;
                        }
                        break;
                    }
                // Speed temporarily hastes you
                case PotionType.Speed:
                    {
                        if (Player.TimedHaste == 0)
                        {
                            if (Player.SetTimedHaste(Program.Rng.DieRoll(25) + 15))
                            {
                                identified = true;
                            }
                        }
                        else
                        {
                            Player.SetTimedHaste(Player.TimedHaste + 5);
                        }
                        break;
                    }
                // Resist heat gives you timed fire resistance
                case PotionType.ResistHeat:
                    {
                        if (Player.SetTimedFireResistance(Player.TimedFireResistance + Program.Rng.DieRoll(10) + 10))
                        {
                            identified = true;
                        }
                        break;
                    }
                // Resist cold gives you timed frost resistance
                case PotionType.ResistCold:
                    {
                        if (Player.SetTimedColdResistance(Player.TimedColdResistance + Program.Rng.DieRoll(10) + 10))
                        {
                            identified = true;
                        }
                        break;
                    }
                // Heroism removes fear, cures 10 health, and gives you timed heroism
                case PotionType.Heroism:
                    {
                        if (Player.SetTimedFear(0))
                        {
                            identified = true;
                        }
                        if (Player.SetTimedHeroism(Player.TimedHeroism + Program.Rng.DieRoll(25) + 25))
                        {
                            identified = true;
                        }
                        if (Player.RestoreHealth(10))
                        {
                            identified = true;
                        }
                        break;
                    }
                // Berserk strength removes fear, heals 30 health, and gives you timed super heroism
                case PotionType.BeserkStrength:
                    {
                        if (Player.SetTimedFear(0))
                        {
                            identified = true;
                        }
                        if (Player.SetTimedSuperheroism(Player.TimedSuperheroism + Program.Rng.DieRoll(25) + 25))
                        {
                            identified = true;
                        }
                        if (Player.RestoreHealth(30))
                        {
                            identified = true;
                        }
                        break;
                    }
                // Cure light wounds heals you 2d8 health and reduces bleeding
                case PotionType.CureLight:
                    {
                        if (Player.RestoreHealth(Program.Rng.DiceRoll(2, 8)))
                        {
                            identified = true;
                        }
                        if (Player.SetTimedBleeding(Player.TimedBleeding - 10))
                        {
                            identified = true;
                        }
                        break;
                    }
                // Cure serious wounds heals you 4d8 health, cures blindness and confusion, and
                // reduces bleeding
                case PotionType.CureSerious:
                    {
                        if (Player.RestoreHealth(Program.Rng.DiceRoll(4, 8)))
                        {
                            identified = true;
                        }
                        if (Player.SetTimedBlindness(0))
                        {
                            identified = true;
                        }
                        if (Player.SetTimedConfusion(0))
                        {
                            identified = true;
                        }
                        if (Player.SetTimedBleeding((Player.TimedBleeding / 2) - 50))
                        {
                            identified = true;
                        }
                        break;
                    }
                // Cure critical wounds heals you 6d8 health, and cures blindness, confusion, stun,
                // poison, and bleeding
                case PotionType.CureCritical:
                    {
                        if (Player.RestoreHealth(Program.Rng.DiceRoll(6, 8)))
                        {
                            identified = true;
                        }
                        if (Player.SetTimedBlindness(0))
                        {
                            identified = true;
                        }
                        if (Player.SetTimedConfusion(0))
                        {
                            identified = true;
                        }
                        if (Player.SetTimedPoison(0))
                        {
                            identified = true;
                        }
                        if (Player.SetTimedStun(0))
                        {
                            identified = true;
                        }
                        if (Player.SetTimedBleeding(0))
                        {
                            identified = true;
                        }
                        break;
                    }
                // Healing heals you 300 health, and cures blindness, confusion, stun, poison, and bleeding
                case PotionType.Healing:
                    {
                        if (Player.RestoreHealth(300))
                        {
                            identified = true;
                        }
                        if (Player.SetTimedBlindness(0))
                        {
                            identified = true;
                        }
                        if (Player.SetTimedConfusion(0))
                        {
                            identified = true;
                        }
                        if (Player.SetTimedPoison(0))
                        {
                            identified = true;
                        }
                        if (Player.SetTimedStun(0))
                        {
                            identified = true;
                        }
                        if (Player.SetTimedBleeding(0))
                        {
                            identified = true;
                        }
                        break;
                    }
                // *Healing* heals you 1200 health, and cures blindness, confusion, stun, poison,
                // and bleeding
                case PotionType.StarHealing:
                    {
                        if (Player.RestoreHealth(1200))
                        {
                            identified = true;
                        }
                        if (Player.SetTimedBlindness(0))
                        {
                            identified = true;
                        }
                        if (Player.SetTimedConfusion(0))
                        {
                            identified = true;
                        }
                        if (Player.SetTimedPoison(0))
                        {
                            identified = true;
                        }
                        if (Player.SetTimedStun(0))
                        {
                            identified = true;
                        }
                        if (Player.SetTimedBleeding(0))
                        {
                            identified = true;
                        }
                        break;
                    }
                // Life heals you 5000 health, removes experience and ability score drains, and
                // cures blindness, confusion, stun, poison, and bleeding
                case PotionType.Life:
                    {
                        Profile.Instance.MsgPrint("You feel life flow through your body!");
                        Player.RestoreLevel();
                        Player.RestoreHealth(5000);
                        Player.SetTimedPoison(0);
                        Player.SetTimedBlindness(0);
                        Player.SetTimedConfusion(0);
                        Player.SetTimedHallucinations(0);
                        Player.SetTimedStun(0);
                        Player.SetTimedBleeding(0);
                        Player.TryRestoringAbilityScore(Ability.Strength);
                        Player.TryRestoringAbilityScore(Ability.Constitution);
                        Player.TryRestoringAbilityScore(Ability.Dexterity);
                        Player.TryRestoringAbilityScore(Ability.Wisdom);
                        Player.TryRestoringAbilityScore(Ability.Intelligence);
                        Player.TryRestoringAbilityScore(Ability.Charisma);
                        identified = true;
                        break;
                    }
                // Restore mana restores your to maximum mana
                case PotionType.RestoreMana:
                    {
                        if (Player.Mana < Player.MaxMana)
                        {
                            Player.Mana = Player.MaxMana;
                            Player.FractionalMana = 0;
                            Profile.Instance.MsgPrint("Your feel your head clear.");
                            Player.RedrawNeeded.Set(RedrawFlag.PrMana);
                            identified = true;
                        }
                        break;
                    }
                // Restore life levels restores any lost experience
                case PotionType.RestoreExp:
                    {
                        if (Player.RestoreLevel())
                        {
                            identified = true;
                        }
                        break;
                    }
                // Restore strength restores your strength
                case PotionType.ResStr:
                    {
                        if (Player.TryRestoringAbilityScore(Ability.Strength))
                        {
                            identified = true;
                        }
                        break;
                    }
                // Restore intelligence restores your intelligence
                case PotionType.ResInt:
                    {
                        if (Player.TryRestoringAbilityScore(Ability.Intelligence))
                        {
                            identified = true;
                        }
                        break;
                    }
                // Restore wisdom restores your wisdom
                case PotionType.ResWis:
                    {
                        if (Player.TryRestoringAbilityScore(Ability.Wisdom))
                        {
                            identified = true;
                        }
                        break;
                    }
                // Restore dexterity restores your dexterity
                case PotionType.ResDex:
                    {
                        if (Player.TryRestoringAbilityScore(Ability.Dexterity))
                        {
                            identified = true;
                        }
                        break;
                    }
                // Restore constitution restores your constitution
                case PotionType.ResCon:
                    {
                        if (Player.TryRestoringAbilityScore(Ability.Constitution))
                        {
                            identified = true;
                        }
                        break;
                    }
                // Restore charisma restores your charisma
                case PotionType.ResCha:
                    {
                        if (Player.TryRestoringAbilityScore(Ability.Charisma))
                        {
                            identified = true;
                        }
                        break;
                    }
                // Strength increases your strength
                case PotionType.IncStr:
                    {
                        if (Player.TryIncreasingAbilityScore(Ability.Strength))
                        {
                            identified = true;
                        }
                        break;
                    }
                // Intelligence increases your intelligence
                case PotionType.IncInt:
                    {
                        if (Player.TryIncreasingAbilityScore(Ability.Intelligence))
                        {
                            identified = true;
                        }
                        break;
                    }
                // Wisdom increases your wisdom
                case PotionType.IncWis:
                    {
                        if (Player.TryIncreasingAbilityScore(Ability.Wisdom))
                        {
                            identified = true;
                        }
                        break;
                    }
                // Dexterity increases your dexterity
                case PotionType.IncDex:
                    {
                        if (Player.TryIncreasingAbilityScore(Ability.Dexterity))
                        {
                            identified = true;
                        }
                        break;
                    }
                // Constitution increases your constitution
                case PotionType.IncCon:
                    {
                        if (Player.TryIncreasingAbilityScore(Ability.Constitution))
                        {
                            identified = true;
                        }
                        break;
                    }
                // Charisma increases your charisma
                case PotionType.IncCha:
                    {
                        if (Player.TryIncreasingAbilityScore(Ability.Charisma))
                        {
                            identified = true;
                        }
                        break;
                    }
                // Augmentation increases all ability scores
                case PotionType.Augmentation:
                    {
                        if (Player.TryIncreasingAbilityScore(Ability.Strength))
                        {
                            identified = true;
                        }
                        if (Player.TryIncreasingAbilityScore(Ability.Intelligence))
                        {
                            identified = true;
                        }
                        if (Player.TryIncreasingAbilityScore(Ability.Wisdom))
                        {
                            identified = true;
                        }
                        if (Player.TryIncreasingAbilityScore(Ability.Dexterity))
                        {
                            identified = true;
                        }
                        if (Player.TryIncreasingAbilityScore(Ability.Constitution))
                        {
                            identified = true;
                        }
                        if (Player.TryIncreasingAbilityScore(Ability.Charisma))
                        {
                            identified = true;
                        }
                        break;
                    }
                // Enlightenment shows you the whole level
                case PotionType.Enlightenment:
                    {
                        Profile.Instance.MsgPrint("An image of your surroundings forms in your mind...");
                        Level.WizLight();
                        identified = true;
                        break;
                    }
                // *Enlightenment* shows you the whole level, increases your intelligence and
                // wisdom, identifies all your items, and detects everything
                case PotionType.StarEnlightenment:
                    {
                        Profile.Instance.MsgPrint("You begin to feel more enlightened...");
                        Profile.Instance.MsgPrint(null);
                        Level.WizLight();
                        Player.TryIncreasingAbilityScore(Ability.Intelligence);
                        Player.TryIncreasingAbilityScore(Ability.Wisdom);
                        _saveGame.SpellEffects.DetectTraps();
                        _saveGame.SpellEffects.DetectDoors();
                        _saveGame.SpellEffects.DetectStairs();
                        _saveGame.SpellEffects.DetectTreasure();
                        _saveGame.SpellEffects.DetectObjectsGold();
                        _saveGame.SpellEffects.DetectObjectsNormal();
                        _saveGame.SpellEffects.IdentifyPack();
                        _saveGame.SpellEffects.SelfKnowledge();
                        identified = true;
                        break;
                    }
                // Self knowledge gives you information about yourself
                case PotionType.SelfKnowledge:
                    {
                        Profile.Instance.MsgPrint("You begin to know yourself a little better...");
                        Profile.Instance.MsgPrint(null);
                        _saveGame.SpellEffects.SelfKnowledge();
                        identified = true;
                        break;
                    }
                // Experience increases your experience points by 50%, with a minimum of +10 and
                // maximuum of +10,000
                case PotionType.Experience:
                    {
                        if (Player.ExperiencePoints < Constants.PyMaxExp)
                        {
                            int ee = (Player.ExperiencePoints / 2) + 10;
                            if (ee > 100000)
                            {
                                ee = 100000;
                            }
                            Profile.Instance.MsgPrint("You feel more experienced.");
                            Player.GainExperience(ee);
                            identified = true;
                        }
                        break;
                    }
                // Resistance gives you all timed resistances
                case PotionType.Resistance:
                    {
                        Player.SetTimedAcidResistance(Player.TimedAcidResistance + Program.Rng.DieRoll(20) + 20);
                        Player.SetTimedLightningResistance(Player.TimedLightningResistance + Program.Rng.DieRoll(20) + 20);
                        Player.SetTimedFireResistance(Player.TimedFireResistance + Program.Rng.DieRoll(20) + 20);
                        Player.SetTimedColdResistance(Player.TimedColdResistance + Program.Rng.DieRoll(20) + 20);
                        Player.SetTimedPoisonResistance(Player.TimedPoisonResistance + Program.Rng.DieRoll(20) + 20);
                        identified = true;
                        break;
                    }
                // Curing heals you 50 health, and cures blindness, confusion, stun, poison,
                // bleeding, and hallucinations
                case PotionType.Curing:
                    {
                        if (Player.RestoreHealth(50))
                        {
                            identified = true;
                        }
                        if (Player.SetTimedBlindness(0))
                        {
                            identified = true;
                        }
                        if (Player.SetTimedPoison(0))
                        {
                            identified = true;
                        }
                        if (Player.SetTimedConfusion(0))
                        {
                            identified = true;
                        }
                        if (Player.SetTimedStun(0))
                        {
                            identified = true;
                        }
                        if (Player.SetTimedBleeding(0))
                        {
                            identified = true;
                        }
                        if (Player.SetTimedHallucinations(0))
                        {
                            identified = true;
                        }
                        break;
                    }
                // Invulnerability gives you temporary invulnerability
                case PotionType.Invulnerability:
                    {
                        Player.SetTimedInvulnerability(Player.TimedInvulnerability + Program.Rng.DieRoll(7) + 7);
                        identified = true;
                        break;
                    }
                // New life rerolls your health, cures all mutations, and restores you to your birth race
                case PotionType.NewLife:
                    {
                        Player.RerollHitPoints();
                        if (Player.Dna.HasMutations)
                        {
                            Profile.Instance.MsgPrint("You are cured of all mutations.");
                            Player.Dna.LoseAllMutations();
                            Player.UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
                            _saveGame.HandleStuff();
                        }
                        if (Player.RaceIndex != Player.RaceIndexAtBirth)
                        {
                            var oldRaceName = Race.RaceInfo[Player.RaceIndexAtBirth].Title;
                            Profile.Instance.MsgPrint($"You feel more {oldRaceName} again.");
                            Player.ChangeRace(Player.RaceIndexAtBirth);
                            SaveGame.Instance.Level.RedrawSingleLocation(Player.MapY, Player.MapX);
                        }
                        identified = true;
                        break;
                    }
            }
            return identified;
        }

        /// <summary>
        /// Pick a random potion to use from a selection that won't kill us
        /// </summary>
        /// <returns> The item sub-category of the potion to use </returns>
        public int RandomPotion()
        {
            var itemSubCategory = 0;
            switch (Program.Rng.DieRoll(48))
            {
                case 1:
                    itemSubCategory = PotionType.Water;
                    break;

                case 2:
                    itemSubCategory = PotionType.AppleJuice;
                    break;

                case 3:
                    itemSubCategory = PotionType.Slowness;
                    break;

                case 4:
                    itemSubCategory = PotionType.SaltWater;
                    break;

                case 5:
                    itemSubCategory = PotionType.Poison;
                    break;

                case 6:
                    itemSubCategory = PotionType.Blindness;
                    break;

                case 7:
                    itemSubCategory = PotionType.Confusion;
                    break;

                case 8:
                    itemSubCategory = PotionType.Sleep;
                    break;

                case 9:
                    itemSubCategory = PotionType.Infravision;
                    break;

                case 10:
                    itemSubCategory = PotionType.DetectInvis;
                    break;

                case 11:
                    itemSubCategory = PotionType.SlowPoison;
                    break;

                case 12:
                    itemSubCategory = PotionType.CurePoison;
                    break;

                case 13:
                    itemSubCategory = PotionType.Boldness;
                    break;

                case 14:
                    itemSubCategory = PotionType.Speed;
                    break;

                case 15:
                    itemSubCategory = PotionType.ResistHeat;
                    break;

                case 16:
                    itemSubCategory = PotionType.ResistCold;
                    break;

                case 17:
                    itemSubCategory = PotionType.Heroism;
                    break;

                case 18:
                    itemSubCategory = PotionType.BeserkStrength;
                    break;

                case 19:
                    itemSubCategory = PotionType.CureLight;
                    break;

                case 20:
                    itemSubCategory = PotionType.CureSerious;
                    break;

                case 21:
                    itemSubCategory = PotionType.CureCritical;
                    break;

                case 22:
                    itemSubCategory = PotionType.Healing;
                    break;

                case 23:
                    itemSubCategory = PotionType.StarHealing;
                    break;

                case 24:
                    itemSubCategory = PotionType.Life;
                    break;

                case 25:
                    itemSubCategory = PotionType.RestoreMana;
                    break;

                case 26:
                    itemSubCategory = PotionType.RestoreExp;
                    break;

                case 27:
                    itemSubCategory = PotionType.ResStr;
                    break;

                case 28:
                    itemSubCategory = PotionType.ResInt;
                    break;

                case 29:
                    itemSubCategory = PotionType.ResWis;
                    break;

                case 30:
                    itemSubCategory = PotionType.ResDex;
                    break;

                case 31:
                    itemSubCategory = PotionType.ResCon;
                    break;

                case 32:
                    itemSubCategory = PotionType.ResCha;
                    break;

                case 33:
                    itemSubCategory = PotionType.IncStr;
                    break;

                case 34:
                    itemSubCategory = PotionType.IncInt;
                    break;

                case 35:
                    itemSubCategory = PotionType.IncWis;
                    break;

                case 36:
                    itemSubCategory = PotionType.IncDex;
                    break;

                case 38:
                    itemSubCategory = PotionType.IncCon;
                    break;

                case 39:
                    itemSubCategory = PotionType.IncCha;
                    break;

                case 40:
                    itemSubCategory = PotionType.Augmentation;
                    break;

                case 41:
                    itemSubCategory = PotionType.Enlightenment;
                    break;

                case 42:
                    itemSubCategory = PotionType.StarEnlightenment;
                    break;

                case 43:
                    itemSubCategory = PotionType.SelfKnowledge;
                    break;

                case 44:
                    itemSubCategory = PotionType.Experience;
                    break;

                case 45:
                    itemSubCategory = PotionType.Resistance;
                    break;

                case 46:
                    itemSubCategory = PotionType.Curing;
                    break;

                case 47:
                    itemSubCategory = PotionType.Invulnerability;
                    break;

                case 48:
                    itemSubCategory = PotionType.NewLife;
                    break;
            }
            return itemSubCategory;
        }

        /// <summary>
        /// Invoke a random power from the Ring of Set
        /// </summary>
        /// <param name="direction"> The direction the player is aiming </param>
        public void RingOfSetPower(int direction)
        {
            switch (Program.Rng.DieRoll(10))
            {
                case 1:
                case 2:
                    {
                        // Decrease all the players ability scores, bypassing sustain and divine protection
                        Profile.Instance.MsgPrint("You are surrounded by a malignant aura.");
                        Player.DecreaseAbilityScore(Ability.Strength, 50, true);
                        Player.DecreaseAbilityScore(Ability.Intelligence, 50, true);
                        Player.DecreaseAbilityScore(Ability.Wisdom, 50, true);
                        Player.DecreaseAbilityScore(Ability.Dexterity, 50, true);
                        Player.DecreaseAbilityScore(Ability.Constitution, 50, true);
                        Player.DecreaseAbilityScore(Ability.Charisma, 50, true);
                        // Reduce both experience and maximum experience
                        Player.ExperiencePoints -= Player.ExperiencePoints / 4;
                        Player.MaxExperienceGained -= Player.ExperiencePoints / 4;
                        Player.CheckExperience();
                        break;
                    }
                case 3:
                    {
                        // Dispel monsters
                        Profile.Instance.MsgPrint("You are surrounded by a powerful aura.");
                        _saveGame.SpellEffects.DispelMonsters(1000);
                        break;
                    }
                case 4:
                case 5:
                case 6:
                    {
                        // Do a 300 damage mana ball
                        _saveGame.SpellEffects.FireBall(new ProjectMana(SaveGame.Instance.SpellEffects), direction, 300, 3);
                        break;
                    }
                case 7:
                case 8:
                case 9:
                case 10:
                    {
                        // Do a 250 damage mana bolt
                        _saveGame.SpellEffects.FireBolt(new ProjectMana(SaveGame.Instance.SpellEffects), direction, 250);
                        break;
                    }
            }
        }

        /// <summary>
        /// Run a single step
        /// </summary>
        /// <param name="direction">
        /// The direction in which we wish to run, or 0 if we are already running
        /// </param>
        public void RunOneStep(int direction)
        {
            if (direction != 0)
            {
                // Check if we can actually run in that direction
                if (_autoNavigator.SeeWall(direction, Player.MapY, Player.MapX))
                {
                    Profile.Instance.MsgPrint("You cannot run in that direction.");
                    _saveGame.Disturb(false);
                    return;
                }
                Player.UpdatesNeeded.Set(UpdateFlags.UpdateTorchRadius);
                // Initialise our navigation state
                _autoNavigator = new AutoNavigator(direction);
            }
            else
            {
                // We're already running, so check if we have to stop
                if (_autoNavigator.NavigateNextStep())
                {
                    _saveGame.Disturb(false);
                    return;
                }
            }
            // Running has a limit, just in case, but in practice we'll never reach it
            if (--_saveGame.Running <= 0)
            {
                return;
            }
            // We can run, so use a move's worth of energy and actually make the move
            _saveGame.EnergyUse = 100;
            MovePlayer(_autoNavigator.CurrentRunDirection, false);
        }

        /// <summary>
        /// Make a piece of armour immune to acid damage, removing any penalty at the same time
        /// </summary>
        public void Rustproof()
        {
            // Get a piece of armour
            _saveGame.ItemFilter = _saveGame.SpellEffects.ItemTesterHookArmour;
            if (!_saveGame.GetItem(out int itemIndex, "Rustproof which piece of armour? ", true, true, true))
            {
                if (itemIndex == -2)
                {
                    Profile.Instance.MsgPrint("You have nothing to rustproof.");
                }
                return;
            }
            Item item = itemIndex >= 0 ? Player.Inventory[itemIndex] : Level.Items[0 - itemIndex];
            string itenName = item.Description(false, 0);
            // Set the ignore acid flag
            item.RandartFlags3.Set(ItemFlag3.IgnoreAcid);
            // Make sure the grammar of the message is correct
            string your;
            string s;
            if (item.BonusArmourClass < 0 && item.IdentifyFlags.IsClear(Constants.IdentCursed))
            {
                your = itemIndex > 0 ? "Your" : "The";
                s = item.Count > 1 ? "" : "s";
                Profile.Instance.MsgPrint($"{your} {itenName} look{s} as good as new!");
                item.BonusArmourClass = 0;
            }
            your = itemIndex > 0 ? "Your" : "The";
            s = item.Count > 1 ? "are" : "is";
            Profile.Instance.MsgPrint($"{your} {itenName} {s} now protected against corrosion.");
        }

        /// <summary>
        /// Search around the player for secret doors and traps
        /// </summary>
        public void Search()
        {
            // The basic chance is equal to our searching skill
            int chance = Player.SkillSearching;
            // If we can't see it's hard to search
            if (Player.TimedBlindness != 0 || Level.NoLight())
            {
                chance /= 10;
            }
            // If we're confused it's hard to search
            if (Player.TimedConfusion != 0 || Player.TimedHallucinations != 0)
            {
                chance /= 10;
            }
            // Check the eight squares around us
            for (int y = Player.MapY - 1; y <= Player.MapY + 1; y++)
            {
                for (int x = Player.MapX - 1; x <= Player.MapX + 1; x++)
                {
                    // Check if we succeed
                    if (Program.Rng.RandomLessThan(100) < chance)
                    {
                        // If there's a trap, then find it
                        GridTile tile = Level.Grid[y][x];
                        if (tile.FeatureType.Name == "Invis")
                        {
                            // Pick a random trap to replace the undetected one with
                            _saveGame.Level.PickTrap(y, x);
                            Profile.Instance.MsgPrint("You have found a trap.");
                            _saveGame.Disturb(false);
                        }
                        if (tile.FeatureType.Name == "SecretDoor")
                        {
                            // Replace the secret door with a visible door
                            Profile.Instance.MsgPrint("You have found a secret door.");
                            Player.GainExperience(1);
                            Level.ReplaceSecretDoor(y, x);
                            _saveGame.Disturb(false);
                        }
                        int nextItemIndex;
                        // Check the items on the tile
                        for (int itemIndex = tile.ItemIndex; itemIndex != 0; itemIndex = nextItemIndex)
                        {
                            Item item = Level.Items[itemIndex];
                            nextItemIndex = item.NextInStack;
                            // If one of them is a chest, determine if it is trapped
                            if (item.Category != ItemCategory.Chest)
                            {
                                continue;
                            }
                            if (item.TypeSpecificValue <= 0)
                            {
                                continue;
                            }
                            if (GlobalData.ChestTraps[item.TypeSpecificValue] == 0)
                            {
                                continue;
                            }
                            // It was a trapped chest - if we didn't already know that then let us know
                            if (!item.IsKnown())
                            {
                                Profile.Instance.MsgPrint("You have discovered a trap on the chest!");
                                item.BecomeKnown();
                                _saveGame.Disturb(false);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Summon an item to the player via telekinesis
        /// </summary>
        /// <param name="dir"> The direction to check for items </param>
        /// <param name="maxWeight"> The maximum weight we can summon </param>
        /// <param name="requireLos"> Whether or not we require line of sight to the item </param>
        public void SummonItem(int dir, int maxWeight, bool requireLos)
        {
            int targetY;
            int targetX;
            GridTile tile;
            // Can't summon something if we're already standing on something
            if (Level.Grid[Player.MapY][Player.MapX].ItemIndex != 0)
            {
                Profile.Instance.MsgPrint("You can't fetch when you're already standing on something.");
                return;
            }
            TargetEngine targetEngine = new TargetEngine(Player, Level);
            // If we didn't have a direction, we might have an existing target
            if (dir == 5 && targetEngine.TargetOkay())
            {
                targetX = _saveGame.TargetCol;
                targetY = _saveGame.TargetRow;
                // Check the range
                if (Level.Distance(Player.MapY, Player.MapX, targetY, targetX) > Constants.MaxRange)
                {
                    Profile.Instance.MsgPrint("You can't fetch something that far away!");
                    return;
                }
                // Check the line of sight if needed
                tile = Level.Grid[targetY][targetX];
                if (requireLos && !Level.PlayerHasLosBold(targetY, targetX))
                {
                    Profile.Instance.MsgPrint("You have no direct line of sight to that location.");
                    return;
                }
            }
            else
            {
                // We have a direction, so move along it until we find an item
                targetY = Player.MapY;
                targetX = Player.MapX;
                do
                {
                    targetY += Level.KeypadDirectionYOffset[dir];
                    targetX += Level.KeypadDirectionXOffset[dir];
                    tile = Level.Grid[targetY][targetX];
                    // Stop if we hit max range or we're blocked by something
                    if (Level.Distance(Player.MapY, Player.MapX, targetY, targetX) > Constants.MaxRange ||
                        !Level.GridPassable(targetY, targetX))
                    {
                        return;
                    }
                } while (tile.ItemIndex == 0);
            }
            Item item = Level.Items[tile.ItemIndex];
            // Check the weight of the item
            if (item.Weight > maxWeight)
            {
                Profile.Instance.MsgPrint("The object is too heavy.");
                return;
            }
            // Remove the entire item stack from the tile and move it to the player's tile
            int itemIndex = tile.ItemIndex;
            tile.ItemIndex = 0;
            Level.Grid[Player.MapY][Player.MapX].ItemIndex = itemIndex;
            item.Y = Player.MapY;
            item.X = Player.MapX;
            Level.NoteSpot(Player.MapY, Player.MapX);
            Player.RedrawNeeded.Set(RedrawFlag.PrMap);
        }

        /// <summary>
        /// Tunnel through a grid tile
        /// </summary>
        /// <param name="y"> The y coordinate of the tile to be tunneled through </param>
        /// <param name="x"> The x coordinate of the tile to be tunneled through </param>
        /// <returns> Whether or not the command can be repeated </returns>
        public bool TunnelThroughTile(int y, int x)
        {
            bool repeat = false;
            // Tunnelling uses an entire turn
            _saveGame.EnergyUse = 100;
            GridTile tile = Level.Grid[y][x];
            // Trees are easy to chop down
            if (tile.FeatureType.Category == FloorTileTypeCategory.Tree)
            {
                if (Player.SkillDigging > 40 + Program.Rng.RandomLessThan(100) && RemoveTileViaTunnelling(y, x))
                {
                    Profile.Instance.MsgPrint($"You have chopped down the {tile.FeatureType.Description}.");
                }
                else
                {
                    Profile.Instance.MsgPrint($"You hack away at the {tile.FeatureType.Description}.");
                    repeat = true;
                }
            }
            // Pillars are a bit easier than walls
            else if (tile.FeatureType.Name == "Pillar")
            {
                if (Player.SkillDigging > 40 + Program.Rng.RandomLessThan(300) && RemoveTileViaTunnelling(y, x))
                {
                    Profile.Instance.MsgPrint("You have broken down the pillar.");
                }
                else
                {
                    Profile.Instance.MsgPrint("You hack away at the pillar.");
                    repeat = true;
                }
            }
            // We can't tunnel through water
            else if (tile.FeatureType.Name == "Water")
            {
                Profile.Instance.MsgPrint("The water fills up your tunnel as quickly as you dig!");
            }
            // Permanent features resist being tunneled through
            else if (tile.FeatureType.IsPermanent)
            {
                Profile.Instance.MsgPrint($"The {tile.FeatureType.Description} resists your attempts to tunnel through it.");
            }
            // It's a wall, so we tunnel normally
            else if (tile.FeatureType.Name.Contains("Wall"))
            {
                if (Player.SkillDigging > 40 + Program.Rng.RandomLessThan(1600) && RemoveTileViaTunnelling(y, x))
                {
                    Profile.Instance.MsgPrint("You have finished the tunnel.");
                }
                else
                {
                    Profile.Instance.MsgPrint("You tunnel into the granite wall.");
                    repeat = true;
                }
            }
            // It's a rock seam, so it might have treasure
            else if (tile.FeatureType.Name.Contains("Magma") || tile.FeatureType.Name.Contains("Quartz"))
            {
                bool okay;
                bool hasValue = false;
                bool isMagma = false;
                if (tile.FeatureType.Name.Contains("Treas"))
                {
                    hasValue = true;
                }
                if (tile.FeatureType.Name.Contains("Magma"))
                {
                    isMagma = true;
                }
                // Magma needs a higher tunneling skill than quartz
                if (isMagma)
                {
                    okay = Player.SkillDigging > 20 + Program.Rng.RandomLessThan(800);
                }
                else
                {
                    okay = Player.SkillDigging > 10 + Program.Rng.RandomLessThan(400);
                }
                // Do the actual tunnelling
                if (okay && RemoveTileViaTunnelling(y, x))
                {
                    if (hasValue)
                    {
                        _saveGame.Level.PlaceGold(y, x);
                        Profile.Instance.MsgPrint("You have found something!");
                    }
                    else
                    {
                        Profile.Instance.MsgPrint("You have finished the tunnel.");
                    }
                }
                // Tunnelling failed, so let us know
                else if (isMagma)
                {
                    Profile.Instance.MsgPrint("You tunnel into the magma vein.");
                    repeat = true;
                }
                else
                {
                    Profile.Instance.MsgPrint("You tunnel into the quartz vein.");
                    repeat = true;
                }
            }
            // Rubble is easy to tunnel through
            else if (tile.FeatureType.Name == "Rubble")
            {
                if (Player.SkillDigging > Program.Rng.RandomLessThan(200) && RemoveTileViaTunnelling(y, x))
                {
                    Profile.Instance.MsgPrint("You have removed the rubble.");
                    // 10% chance of finding something
                    if (Program.Rng.RandomLessThan(100) < 10)
                    {
                        _saveGame.Level.PlaceObject(y, x, false, false);
                        if (Level.PlayerCanSeeBold(y, x))
                        {
                            Profile.Instance.MsgPrint("You have found something!");
                        }
                    }
                }
                else
                {
                    Profile.Instance.MsgPrint("You dig in the rubble.");
                    repeat = true;
                }
            }
            // We don't recognise what we're tunnelling into, so just assume it's permanent
            else
            {
                Profile.Instance.MsgPrint($"The {tile.FeatureType.Description} resists your attempts to tunnel through it.");
                repeat = true;
                Search();
            }
            // If we successfully made the tunnel,
            if (!Level.GridPassable(y, x))
            {
                Player.UpdatesNeeded.Set(UpdateFlags.UpdateView | UpdateFlags.UpdateLight | UpdateFlags.UpdateScent | UpdateFlags.UpdateMonsters);
            }
            if (!repeat)
            {
                Gui.PlaySound(SoundEffect.Dig);
            }
            return repeat;
        }

        /// <summary>
        /// Use the player's racial power, if they have one
        /// </summary>
        public void UseRacialPower()
        {
            int playerLevel = Player.Level;
            int direction;
            Projectile projectile;
            string projectileDescription;
            // Default to being randomly fire (66% chance) or cold (33% chance)
            if (Program.Rng.DieRoll(3) == 1)
            {
                projectile = new ProjectCold(SaveGame.Instance.SpellEffects);
                projectileDescription = "cold";
            }
            else
            {
                projectile = new ProjectFire(SaveGame.Instance.SpellEffects);
                projectileDescription = "fire";
            }
            TargetEngine targetEngine = new TargetEngine(Player, Level);
            // Check the player's race to see what their power is
            switch (Player.RaceIndex)
            {
                // Dwarves can detect traps, doors, and stairs
                case RaceId.Dwarf:
                    if (CheckIfRacialPowerWorks(5, 5, Ability.Wisdom, 12))
                    {
                        Profile.Instance.MsgPrint("You examine your surroundings.");
                        _saveGame.SpellEffects.DetectTraps();
                        _saveGame.SpellEffects.DetectDoors();
                        _saveGame.SpellEffects.DetectStairs();
                    }
                    break;
                // Hobbits can cook food
                case RaceId.Hobbit:
                    if (CheckIfRacialPowerWorks(15, 10, Ability.Intelligence, 10))
                    {
                        Item item = new Item();
                        item.AssignItemType(Profile.Instance.ItemTypes.LookupKind(ItemCategory.Food, FoodType.Ration));
                        _saveGame.Level.DropNear(item, -1, Player.MapY, Player.MapX);
                        Profile.Instance.MsgPrint("You cook some food.");
                    }
                    break;
                // Gnomes can do a short-range teleport
                case RaceId.Gnome:
                    if (CheckIfRacialPowerWorks(5, 5 + (playerLevel / 5), Ability.Intelligence, 12))
                    {
                        Profile.Instance.MsgPrint("Blink!");
                        _saveGame.SpellEffects.TeleportPlayer(10 + playerLevel);
                    }
                    break;
                // Half-orcs can remove fear
                case RaceId.HalfOrc:
                    if (CheckIfRacialPowerWorks(3, 5, Ability.Wisdom, Player.ProfessionIndex == CharacterClass.Warrior ? 5 : 10))
                    {
                        Profile.Instance.MsgPrint("You play tough.");
                        Player.SetTimedFear(0);
                    }
                    break;
                // Half-trolls can go berserk, which also heals them
                case RaceId.HalfTroll:
                    if (CheckIfRacialPowerWorks(10, 12, Ability.Wisdom, Player.ProfessionIndex == CharacterClass.Warrior ? 6 : 12))
                    {
                        Profile.Instance.MsgPrint("RAAAGH!");
                        Player.SetTimedFear(0);
                        Player.SetTimedSuperheroism(Player.TimedSuperheroism + 10 + Program.Rng.DieRoll(playerLevel));
                        Player.RestoreHealth(30);
                    }
                    break;
                // Great ones can heal themselves or swap to a new level
                case RaceId.Great:
                    int dreamPower;
                    while (true)
                    {
                        if (!Gui.GetCom("Use Dream [T]ravel or [D]reaming? ", out char ch))
                        {
                            dreamPower = 0;
                            break;
                        }
                        if (ch == 'D' || ch == 'd')
                        {
                            dreamPower = 1;
                            break;
                        }
                        if (ch == 'T' || ch == 't')
                        {
                            dreamPower = 2;
                            break;
                        }
                    }
                    if (dreamPower == 1)
                    {
                        if (CheckIfRacialPowerWorks(40, 75, Ability.Wisdom, 50))
                        {
                            Profile.Instance.MsgPrint("You dream of a time of health and peace...");
                            Player.SetTimedPoison(0);
                            Player.SetTimedHallucinations(0);
                            Player.SetTimedStun(0);
                            Player.SetTimedBleeding(0);
                            Player.SetTimedBlindness(0);
                            Player.SetTimedFear(0);
                            Player.TryRestoringAbilityScore(Ability.Strength);
                            Player.TryRestoringAbilityScore(Ability.Intelligence);
                            Player.TryRestoringAbilityScore(Ability.Wisdom);
                            Player.TryRestoringAbilityScore(Ability.Dexterity);
                            Player.TryRestoringAbilityScore(Ability.Constitution);
                            Player.TryRestoringAbilityScore(Ability.Charisma);
                            Player.RestoreLevel();
                        }
                    }
                    else if (dreamPower == 2)
                    {
                        if (CheckIfRacialPowerWorks(30, 50, Ability.Intelligence, 50))
                        {
                            Profile.Instance.MsgPrint("You start walking around. Your surroundings change.");
                            _saveGame.IsAutosave = true;
                            _saveGame.DoCmdSaveGame();
                            _saveGame.IsAutosave = false;
                            _saveGame.NewLevelFlag = true;
                            _saveGame.CameFrom = LevelStart.StartRandom;
                        }
                    }
                    break;
                // Tcho-Tcho can create The Yellow Sign
                case RaceId.TchoTcho:
                    if (CheckIfRacialPowerWorks(25, 35, Ability.Intelligence, 15))
                    {
                        Profile.Instance.MsgPrint("You carefully draw The Yellow Sign...");
                        _saveGame.SpellEffects.YellowSign();
                    }
                    break;
                // Hlf-Ogres can go berserk
                case RaceId.HalfOgre:
                    if (CheckIfRacialPowerWorks(8, 10, Ability.Wisdom, Player.ProfessionIndex == CharacterClass.Warrior ? 6 : 12))
                    {
                        Profile.Instance.MsgPrint("Raaagh!");
                        Player.SetTimedFear(0);
                        Player.SetTimedSuperheroism(Player.TimedSuperheroism + 10 + Program.Rng.DieRoll(playerLevel));
                        Player.RestoreHealth(30);
                    }
                    break;
                // Half-giants can bash through stone walls
                case RaceId.HalfGiant:
                    if (CheckIfRacialPowerWorks(20, 10, Ability.Strength, 12))
                    {
                        if (!targetEngine.GetDirectionWithAim(out direction))
                        {
                            break;
                        }
                        Profile.Instance.MsgPrint("You bash at a stone wall.");
                        _saveGame.SpellEffects.WallToMud(direction);
                    }
                    break;
                // Half-Titans can probe enemies
                case RaceId.HalfTitan:
                    if (CheckIfRacialPowerWorks(35, 20, Ability.Intelligence, 12))
                    {
                        Profile.Instance.MsgPrint("You examine your foes...");
                        _saveGame.SpellEffects.Probing();
                    }
                    break;
                // Cyclopes can throw boulders
                case RaceId.Cyclops:
                    if (CheckIfRacialPowerWorks(20, 15, Ability.Strength, 12))
                    {
                        if (!targetEngine.GetDirectionWithAim(out direction))
                        {
                            break;
                        }
                        Profile.Instance.MsgPrint("You throw a huge boulder.");
                        _saveGame.SpellEffects.FireBolt(new ProjectMissile(SaveGame.Instance.SpellEffects), direction,
                            3 * Player.Level / 2);
                    }
                    break;
                // Yeeks can scream
                case RaceId.Yeek:
                    if (CheckIfRacialPowerWorks(15, 15, Ability.Wisdom, 10))
                    {
                        if (!targetEngine.GetDirectionWithAim(out direction))
                        {
                            break;
                        }
                        Profile.Instance.MsgPrint("You make a horrible scream!");
                        _saveGame.SpellEffects.FearMonster(direction, playerLevel);
                    }
                    break;
                // Klackons can spit acid
                case RaceId.Klackon:
                    if (CheckIfRacialPowerWorks(9, 9, Ability.Dexterity, 14))
                    {
                        if (!targetEngine.GetDirectionWithAim(out direction))
                        {
                            break;
                        }
                        Profile.Instance.MsgPrint("You spit acid.");
                        if (Player.Level < 25)
                        {
                            _saveGame.SpellEffects.FireBolt(new ProjectAcid(SaveGame.Instance.SpellEffects), direction, playerLevel);
                        }
                        else
                        {
                            _saveGame.SpellEffects.FireBall(new ProjectAcid(SaveGame.Instance.SpellEffects), direction, playerLevel,
                                2);
                        }
                    }
                    break;
                // Kobolds can throw poison darts
                case RaceId.Kobold:
                    if (CheckIfRacialPowerWorks(12, 8, Ability.Dexterity, 14))
                    {
                        if (!targetEngine.GetDirectionWithAim(out direction))
                        {
                            break;
                        }
                        Profile.Instance.MsgPrint("You throw a dart of poison.");
                        _saveGame.SpellEffects.FireBolt(new ProjectPois(SaveGame.Instance.SpellEffects), direction, playerLevel);
                    }
                    break;
                // Nibelungen can detect traps, doors, and stairs
                case RaceId.Nibelung:
                    if (CheckIfRacialPowerWorks(5, 5, Ability.Wisdom, 10))
                    {
                        Profile.Instance.MsgPrint("You examine your surroundings.");
                        _saveGame.SpellEffects.DetectTraps();
                        _saveGame.SpellEffects.DetectDoors();
                        _saveGame.SpellEffects.DetectStairs();
                    }
                    break;
                // Dark elves can cast magic missile
                case RaceId.DarkElf:
                    if (CheckIfRacialPowerWorks(2, 2, Ability.Intelligence, 9))
                    {
                        if (!targetEngine.GetDirectionWithAim(out direction))
                        {
                            break;
                        }
                        Profile.Instance.MsgPrint("You cast a magic missile.");
                        _saveGame.SpellEffects.FireBoltOrBeam(10, new ProjectMissile(SaveGame.Instance.SpellEffects),
                            direction, Program.Rng.DiceRoll(3 + ((playerLevel - 1) / 5), 4));
                    }
                    break;
                // Draconians can breathe an element based on their class and level
                case RaceId.Draconian:
                    // Chance of replacing the default fire/cold element with a special one
                    if (Program.Rng.DieRoll(100) < Player.Level)
                    {
                        switch (Player.ProfessionIndex)
                        {
                            case CharacterClass.Warrior:
                            case CharacterClass.Ranger:
                            case CharacterClass.Druid:
                            case CharacterClass.ChosenOne:
                                if (Program.Rng.DieRoll(3) == 1)
                                {
                                    projectile = new ProjectMissile(SaveGame.Instance.SpellEffects);
                                    projectileDescription = "the elements";
                                }
                                else
                                {
                                    projectile = new ProjectExplode(SaveGame.Instance.SpellEffects);
                                    projectileDescription = "shards";
                                }
                                break;

                            case CharacterClass.Mage:
                            case CharacterClass.WarriorMage:
                            case CharacterClass.HighMage:
                            case CharacterClass.Channeler:
                                if (Program.Rng.DieRoll(3) == 1)
                                {
                                    projectile = new ProjectMana(SaveGame.Instance.SpellEffects);
                                    projectileDescription = "mana";
                                }
                                else
                                {
                                    projectile = new ProjectDisenchant(SaveGame.Instance.SpellEffects);
                                    projectileDescription = "disenchantment";
                                }
                                break;

                            case CharacterClass.Fanatic:
                            case CharacterClass.Cultist:
                                if (Program.Rng.DieRoll(3) != 1)
                                {
                                    projectile = new ProjectConfusion(SaveGame.Instance.SpellEffects);
                                    projectileDescription = "confusion";
                                }
                                else
                                {
                                    projectile = new ProjectChaos(SaveGame.Instance.SpellEffects);
                                    projectileDescription = "chaos";
                                }
                                break;

                            case CharacterClass.Monk:
                                if (Program.Rng.DieRoll(3) != 1)
                                {
                                    projectile = new ProjectConfusion(SaveGame.Instance.SpellEffects);
                                    projectileDescription = "confusion";
                                }
                                else
                                {
                                    projectile = new ProjectSound(SaveGame.Instance.SpellEffects);
                                    projectileDescription = "sound";
                                }
                                break;

                            case CharacterClass.Mindcrafter:
                            case CharacterClass.Mystic:
                                if (Program.Rng.DieRoll(3) != 1)
                                {
                                    projectile = new ProjectConfusion(SaveGame.Instance.SpellEffects);
                                    projectileDescription = "confusion";
                                }
                                else
                                {
                                    projectile = new ProjectPsi(SaveGame.Instance.SpellEffects);
                                    projectileDescription = "mental energy";
                                }
                                break;

                            case CharacterClass.Priest:
                            case CharacterClass.Paladin:
                                if (Program.Rng.DieRoll(3) == 1)
                                {
                                    projectile = new ProjectHellFire(SaveGame.Instance.SpellEffects);
                                    projectileDescription = "hellfire";
                                }
                                else
                                {
                                    projectile = new ProjectHolyFire(SaveGame.Instance.SpellEffects);
                                    projectileDescription = "holy fire";
                                }
                                break;

                            case CharacterClass.Rogue:
                                if (Program.Rng.DieRoll(3) == 1)
                                {
                                    projectile = new ProjectDark(SaveGame.Instance.SpellEffects);
                                    projectileDescription = "darkness";
                                }
                                else
                                {
                                    projectile = new ProjectPois(SaveGame.Instance.SpellEffects);
                                    projectileDescription = "poison";
                                }
                                break;
                        }
                    }
                    if (CheckIfRacialPowerWorks(1, Player.Level, Ability.Constitution, 12))
                    {
                        if (!targetEngine.GetDirectionWithAim(out direction))
                        {
                            break;
                        }
                        Profile.Instance.MsgPrint($"You breathe {projectileDescription}.");
                        _saveGame.SpellEffects.FireBall(projectile, direction, Player.Level * 2, -(Player.Level / 15) + 1);
                    }
                    break;
                // Mind Flayers can shoot psychic bolts
                case RaceId.MindFlayer:
                    if (CheckIfRacialPowerWorks(15, 12, Ability.Intelligence, 14))
                    {
                        if (!targetEngine.GetDirectionWithAim(out direction))
                        {
                            break;
                        }
                        Profile.Instance.MsgPrint("You concentrate and your eyes glow red...");
                        _saveGame.SpellEffects.FireBolt(new ProjectPsi(SaveGame.Instance.SpellEffects), direction, playerLevel);
                    }
                    break;
                // Imps can cast fire bolt/ball
                case RaceId.Imp:
                    if (CheckIfRacialPowerWorks(9, 15, Ability.Wisdom, 15))
                    {
                        if (!targetEngine.GetDirectionWithAim(out direction))
                        {
                            break;
                        }
                        if (Player.Level >= 30)
                        {
                            Profile.Instance.MsgPrint("You cast a ball of fire.");
                            _saveGame.SpellEffects.FireBall(new ProjectFire(SaveGame.Instance.SpellEffects), direction, playerLevel,
                                2);
                        }
                        else
                        {
                            Profile.Instance.MsgPrint("You cast a bolt of fire.");
                            _saveGame.SpellEffects.FireBolt(new ProjectFire(SaveGame.Instance.SpellEffects), direction, playerLevel);
                        }
                    }
                    break;
                // Golems can harden their skin
                case RaceId.Golem:
                    if (CheckIfRacialPowerWorks(20, 15, Ability.Constitution, 8))
                    {
                        Player.SetTimedStoneskin(Player.TimedStoneskin + Program.Rng.DieRoll(20) + 30);
                    }
                    break;
                // Skeletons and zombies can restore their life energy
                case RaceId.Skeleton:
                case RaceId.Zombie:
                    if (CheckIfRacialPowerWorks(30, 30, Ability.Wisdom, 18))
                    {
                        Profile.Instance.MsgPrint("You attempt to restore your lost energies.");
                        Player.RestoreLevel();
                    }
                    break;
                // Vampires can drain health
                case RaceId.Vampire:
                    if (CheckIfRacialPowerWorks(2, 1 + (playerLevel / 3), Ability.Constitution, 9))
                    {
                        if (!targetEngine.GetDirectionNoAim(out direction))
                        {
                            break;
                        }
                        int y = Player.MapY + Level.KeypadDirectionYOffset[direction];
                        int x = Player.MapX + Level.KeypadDirectionXOffset[direction];
                        GridTile tile = Level.Grid[y][x];
                        if (tile.MonsterIndex == 0)
                        {
                            Profile.Instance.MsgPrint("You bite into thin air!");
                            break;
                        }
                        Profile.Instance.MsgPrint("You grin and bare your fangs...");
                        int dummy = playerLevel + (Program.Rng.DieRoll(playerLevel) * Math.Max(1, playerLevel / 10));
                        if (_saveGame.SpellEffects.DrainLife(direction, dummy))
                        {
                            if (Player.Food < Constants.PyFoodFull)
                            {
                                Player.RestoreHealth(dummy);
                            }
                            else
                            {
                                Profile.Instance.MsgPrint("You were not hungry.");
                            }
                            dummy = Player.Food + Math.Min(5000, 100 * dummy);
                            if (Player.Food < Constants.PyFoodMax)
                            {
                                Player.SetFood(dummy >= Constants.PyFoodMax ? Constants.PyFoodMax - 1 : dummy);
                            }
                        }
                        else
                        {
                            Profile.Instance.MsgPrint("Yechh. That tastes foul.");
                        }
                    }
                    break;
                // Spectres can howl
                case RaceId.Spectre:
                    if (CheckIfRacialPowerWorks(4, 6, Ability.Intelligence, 3))
                    {
                        Profile.Instance.MsgPrint("You emit an eldritch howl!");
                        if (!targetEngine.GetDirectionWithAim(out direction))
                        {
                            break;
                        }
                        _saveGame.SpellEffects.FearMonster(direction, playerLevel);
                    }
                    break;
                // Sprites can sleep monsters
                case RaceId.Sprite:
                    if (CheckIfRacialPowerWorks(12, 12, Ability.Intelligence, 15))
                    {
                        Profile.Instance.MsgPrint("You throw some magic dust...");
                        if (Player.Level < 25)
                        {
                            _saveGame.SpellEffects.SleepMonstersTouch();
                        }
                        else
                        {
                            _saveGame.SpellEffects.SleepMonsters();
                        }
                    }
                    break;
                // Other races don't have powers
                default:
                    Profile.Instance.MsgPrint("This race has no bonus power.");
                    _saveGame.EnergyUse = 0;
                    break;
            }
            Player.RedrawNeeded.Set(RedrawFlag.PrHp | RedrawFlag.PrMana);
        }

        /// <summary>
        /// React to having walked into a door by trying to open it
        /// </summary>
        /// <param name="y"> The y coordinate of the door tile </param>
        /// <param name="x"> The x coordinate of the door tile </param>
        /// <returns> True if we opened it, false otherwise </returns>
        private bool EasyOpenDoor(int y, int x)
        {
            GridTile tile = Level.Grid[y][x];
            // If it isn't closed, we can't open it
            if (!tile.FeatureType.IsClosedDoor)
            {
                return false;
            }
            // If it's jammed we'll need to bash it
            if (tile.FeatureType.Name.Contains("Jammed"))
            {
                Profile.Instance.MsgPrint("The door appears to be stuck.");
            }
            // Most doors are locked, so try to pick the lock
            else if (!tile.FeatureType.Name.Contains("0"))
            {
                int skill = Player.SkillDisarmTraps;
                // Lockpicking is hard in the dark
                if (Player.TimedBlindness != 0 || Level.NoLight())
                {
                    skill /= 10;
                }
                // Lockpicking is hard when you're confused
                if (Player.TimedConfusion != 0 || Player.TimedHallucinations != 0)
                {
                    skill /= 10;
                }
                int chance = int.Parse(tile.FeatureType.Name.Substring(10));
                chance = skill - (chance * 4);
                if (chance < 2)
                {
                    chance = 2;
                }
                // See if we succeed
                if (Program.Rng.RandomLessThan(100) < chance)
                {
                    Profile.Instance.MsgPrint("You have picked the lock.");
                    Level.CaveSetFeat(y, x, "OpenDoor");
                    Player.UpdatesNeeded.Set(UpdateFlags.UpdateView | UpdateFlags.UpdateLight | UpdateFlags.UpdateMonsters);
                    Gui.PlaySound(SoundEffect.LockpickSuccess);
                    Player.GainExperience(1);
                }
                // If we failed, simply let us know
                else
                {
                    Profile.Instance.MsgPrint("You failed to pick the lock.");
                }
            }
            // It wasn't locked, so simply open it
            else
            {
                Level.CaveSetFeat(y, x, "OpenDoor");
                Player.UpdatesNeeded.Set(UpdateFlags.UpdateView | UpdateFlags.UpdateLight | UpdateFlags.UpdateMonsters);
                Gui.PlaySound(SoundEffect.OpenDoor);
            }
            return true;
        }

        /// <summary>
        /// Determine if a player's attack hits a monster
        /// </summary>
        /// <param name="power"> The strength of the attack </param>
        /// <param name="armourClass"> The monster's armour class </param>
        /// <param name="isVisible"> Whether or not the monster is visible </param>
        /// <returns> True if the attack hit, false if not </returns>
        private bool PlayerCheckHitOnMonster(int power, int armourClass, bool isVisible)
        {
            // Always have a 5% chance to hit or miss
            int roll = Program.Rng.RandomLessThan(100);
            if (roll < 10)
            {
                return roll < 5;
            }
            if (power <= 0)
            {
                return false;
            }
            // It's hard to hit invisible monsters
            if (!isVisible)
            {
                power = (power + 1) / 2;
            }
            // Work out whether we hit or not
            return Program.Rng.RandomLessThan(power) >= armourClass * 3 / 4;
        }

        /// <summary>
        /// Work out the level of critical hit the player did in melee
        /// </summary>
        /// <param name="weight"> The weight of the player's weapon </param>
        /// <param name="plus"> The bonuses to hit the player has </param>
        /// <param name="damage"> The amount of base damage that was done </param>
        /// <returns> The damage total modified for a critical hit </returns>
        private int PlayerCriticalMelee(int weight, int plus, int damage)
        {
            int i = weight + ((Player.AttackBonus + plus) * 5) + (Player.Level * 3);
            if (Program.Rng.DieRoll(5000) <= i)
            {
                int k = weight + Program.Rng.DieRoll(650);
                if (k < 400)
                {
                    Profile.Instance.MsgPrint("It was a good hit!");
                    damage = (2 * damage) + 5;
                }
                else if (k < 700)
                {
                    Profile.Instance.MsgPrint("It was a great hit!");
                    damage = (2 * damage) + 10;
                }
                else if (k < 900)
                {
                    Profile.Instance.MsgPrint("It was a superb hit!");
                    damage = (3 * damage) + 15;
                }
                else if (k < 1300)
                {
                    Profile.Instance.MsgPrint("It was a *GREAT* hit!");
                    damage = (3 * damage) + 20;
                }
                else
                {
                    Profile.Instance.MsgPrint("It was a *SUPERB* hit!");
                    damage = (7 * damage / 2) + 25;
                }
            }
            return damage;
        }

        /// <summary>
        /// Resolve a natural attack the player has from a mutation
        /// </summary>
        /// <param name="monsterIndex"> The monster being attacked </param>
        /// <param name="mutation"> The mutation being used to attack </param>
        /// <param name="fear"> Whether or not the monster is scared by the attack </param>
        /// <param name="monsterDies"> Whether or not the monster is killed by the attack </param>
        private void PlayerNaturalAttackOnMonster(int monsterIndex, Mutation mutation, out bool fear, out bool monsterDies)
        {
            fear = false;
            monsterDies = false;
            Monster monster = Level.Monsters[monsterIndex];
            MonsterRace race = monster.Race;
            int damageSides = mutation.DamageDiceSize;
            int damageDice = mutation.DamageDiceNumber;
            int effectiveWeight = mutation.EquivalentWeaponWeight;
            string attackDescription = mutation.AttackDescription;
            string monsterName = monster.MonsterDesc(0);
            // See if the player hit the monster
            int bonus = Player.AttackBonus;
            int chance = Player.SkillMelee + (bonus * Constants.BthPlusAdj);
            if (PlayerCheckHitOnMonster(chance, race.ArmourClass, monster.IsVisible))
            {
                // It was a hit, so let the player know
                Gui.PlaySound(SoundEffect.MeleeHit);
                Profile.Instance.MsgPrint($"You hit {monsterName} with your {attackDescription}.");
                // Roll the damage, with possible critical damage
                int damage = Program.Rng.DiceRoll(damageDice, damageSides);
                damage = PlayerCriticalMelee(effectiveWeight, Player.AttackBonus, damage);
                damage += Player.DamageBonus;
                // Can't have negative damage
                if (damage < 0)
                {
                    damage = 0;
                }
                // If it's a friend, it will get angry
                if ((monster.Mind & Constants.SmFriendly) != 0)
                {
                    Profile.Instance.MsgPrint($"{monsterName} gets angry!");
                    monster.Mind &= ~Constants.SmFriendly;
                }
                // Apply damage of the correct type to the monster
                switch (mutation.MutationAttackType)
                {
                    case MutationAttackType.Physical:
                        monsterDies = Level.Monsters.DamageMonster(monsterIndex, damage, out fear, null);
                        break;

                    case MutationAttackType.Poison:
                        _saveGame.SpellEffects.Project(0, 0, monster.MapY, monster.MapX, damage,
                            new ProjectPois(SaveGame.Instance.SpellEffects), ProjectionFlag.ProjectKill);
                        break;

                    case MutationAttackType.Hellfire:
                        _saveGame.SpellEffects.Project(0, 0, monster.MapY, monster.MapX, damage,
                            new ProjectHellFire(SaveGame.Instance.SpellEffects), ProjectionFlag.ProjectKill);
                        break;
                }
                // The monster might hurt when we touch it
                TouchZapPlayer(monster);
            }
            else
            {
                // We missed, so just let us know
                Gui.PlaySound(SoundEffect.Miss);
                Profile.Instance.MsgPrint($"You miss {monsterName}.");
            }
        }

        /// <summary>
        /// Remove a tile by tunnelling through it
        /// </summary>
        /// <param name="y"> The y coordinate of the tile </param>
        /// <param name="x"> The x coordinate of the tile </param>
        /// <returns> True if the tunnel succeeded, false if not </returns>
        private bool RemoveTileViaTunnelling(int y, int x)
        {
            GridTile tile = Level.Grid[y][x];
            // If we can already get through it, we can't tunnel through it
            if (Level.GridPassable(y, x))
            {
                return false;
            }
            // Clear the tile
            tile.TileFlags.Clear(GridTile.PlayerMemorised);
            Level.RevertTileToBackground(y, x);
            Player.UpdatesNeeded.Set(UpdateFlags.UpdateView | UpdateFlags.UpdateLight | UpdateFlags.UpdateScent | UpdateFlags.UpdateMonsters);
            return true;
        }

        /// <summary>
        /// Handle the player stepping on a trap
        /// </summary>
        private void StepOnTrap()
        {
            int damage;
            string name = "a trap";
            _saveGame.Disturb(false);
            GridTile tile = Level.Grid[Player.MapY][Player.MapX];
            // Check the type of trap
            switch (tile.FeatureType.Name)
            {
                case "TrapDoor":
                    {
                        // Trap doors can be flown over with feather fall
                        if (Player.HasFeatherFall)
                        {
                            Profile.Instance.MsgPrint("You fly over a trap door.");
                        }
                        else
                        {
                            Profile.Instance.MsgPrint("You fell through a trap door!");
                            // Trap doors do 2d8 fall damage
                            damage = Program.Rng.DiceRoll(2, 8);
                            name = "a trap door";
                            Player.TakeHit(damage, name);
                            // Even if we survived, we need a new level
                            if (Player.Health >= 0)
                            {
                                _saveGame.IsAutosave = true;
                                _saveGame.DoCmdSaveGame();
                                _saveGame.IsAutosave = false;
                            }
                            _saveGame.NewLevelFlag = true;
                            // In dungeons we fall to a deeper level, but in towers we fall to a
                            // shallower level because they go up instead of down
                            if (_saveGame.CurDungeon.Tower)
                            {
                                _saveGame.CurrentDepth--;
                            }
                            else
                            {
                                _saveGame.CurrentDepth++;
                            }
                        }
                        break;
                    }
                case "Pit":
                    {
                        // A pit can be flown over with feather fall
                        if (Player.HasFeatherFall)
                        {
                            Profile.Instance.MsgPrint("You fly over a pit trap.");
                        }
                        else
                        {
                            Profile.Instance.MsgPrint("You fell into a pit!");
                            // Pits do 2d6 fall damage
                            damage = Program.Rng.DiceRoll(2, 6);
                            name = "a pit trap";
                            Player.TakeHit(damage, name);
                        }
                        break;
                    }
                case "SpikedPit":
                    {
                        // A pit can be flown over with feather fall
                        if (Player.HasFeatherFall)
                        {
                            Profile.Instance.MsgPrint("You fly over a spiked pit.");
                        }
                        else
                        {
                            Profile.Instance.MsgPrint("You fall into a spiked pit!");
                            name = "a pit trap";
                            // A pit does 2d6 fall damage
                            damage = Program.Rng.DiceRoll(2, 6);
                            // 50% chance of doing double damage plus bleeding
                            if (Program.Rng.RandomLessThan(100) < 50)
                            {
                                Profile.Instance.MsgPrint("You are impaled!");
                                name = "a spiked pit";
                                damage *= 2;
                                Player.SetTimedBleeding(Player.TimedBleeding + Program.Rng.DieRoll(damage));
                            }
                            Player.TakeHit(damage, name);
                        }
                        break;
                    }
                case "PoisonPit":
                    {
                        // A pit can be flown over with feather fall
                        if (Player.HasFeatherFall)
                        {
                            Profile.Instance.MsgPrint("You fly over a spiked pit.");
                        }
                        else
                        {
                            Profile.Instance.MsgPrint("You fall into a spiked pit!");
                            // A pit does 2d6 fall damage
                            damage = Program.Rng.DiceRoll(2, 6);
                            name = "a pit trap";
                            // 50% chance of doing double damage plus bleeding plus poison
                            if (Program.Rng.RandomLessThan(100) < 50)
                            {
                                Profile.Instance.MsgPrint("You are impaled on poisonous spikes!");
                                name = "a spiked pit";
                                damage *= 2;
                                Player.SetTimedBleeding(Player.TimedBleeding + Program.Rng.DieRoll(damage));
                                // Hagarg Ryonis can save us from the poison
                                if (Player.HasPoisonResistance || Player.TimedPoisonResistance != 0)
                                {
                                    Profile.Instance.MsgPrint("The poison does not affect you!");
                                }
                                else if (Program.Rng.DieRoll(10) <= Player.Religion.GetNamedDeity(Pantheon.GodName.Hagarg_Ryonis).AdjustedFavour)
                                {
                                    Profile.Instance.MsgPrint("Hagarg Ryonis's favour protects you!");
                                }
                                else
                                {
                                    damage *= 2;
                                    Player.SetTimedPoison(Player.TimedPoison + Program.Rng.DieRoll(damage));
                                }
                            }
                            Player.TakeHit(damage, name);
                        }
                        break;
                    }
                case "SummonRune":
                    {
                        Profile.Instance.MsgPrint("There is a flash of shimmering light!");
                        // Trap disappears when triggered
                        tile.TileFlags.Clear(GridTile.PlayerMemorised);
                        Level.RevertTileToBackground(Player.MapY, Player.MapX);
                        // Summon 1d3+2 monsters
                        int num = 2 + Program.Rng.DieRoll(3);
                        for (int i = 0; i < num; i++)
                        {
                            Level.Monsters.SummonSpecific(Player.MapY, Player.MapX, _saveGame.Difficulty, 0);
                        }
                        // Have a chance of also cursing the player
                        if (_saveGame.Difficulty > Program.Rng.DieRoll(100))
                        {
                            do
                            {
                                _saveGame.ActivateDreadCurse();
                            } while (Program.Rng.DieRoll(6) == 1);
                        }
                        break;
                    }
                case "TeleportRune":
                    {
                        // Teleport the player up to 100 squares
                        Profile.Instance.MsgPrint("You hit a teleport trap!");
                        _saveGame.SpellEffects.TeleportPlayer(100);
                        break;
                    }
                case "FireTrap":
                    {
                        // Do 4d6 fire damage
                        Profile.Instance.MsgPrint("You are enveloped in flames!");
                        damage = Program.Rng.DiceRoll(4, 6);
                        _saveGame.SpellEffects.FireDam(damage, "a fire trap");
                        break;
                    }
                case "AcidTrap":
                    {
                        // Do 4d6 acid damage
                        Profile.Instance.MsgPrint("You are splashed with acid!");
                        damage = Program.Rng.DiceRoll(4, 6);
                        _saveGame.SpellEffects.AcidDam(damage, "an acid trap");
                        break;
                    }
                case "SlowDart":
                    {
                        // Dart traps need a to-hit roll
                        if (TrapCheckHitOnPlayer(125))
                        {
                            Profile.Instance.MsgPrint("A small dart hits you!");
                            // Do 1d4 damage plus slow
                            damage = Program.Rng.DiceRoll(1, 4);
                            Player.TakeHit(damage, name);
                            Player.SetTimedSlow(Player.TimedSlow + Program.Rng.RandomLessThan(20) + 20);
                        }
                        else
                        {
                            Profile.Instance.MsgPrint("A small dart barely misses you.");
                        }
                        break;
                    }
                case "StrDart":
                    {
                        // Dart traps need a to-hit roll
                        if (TrapCheckHitOnPlayer(125))
                        {
                            Profile.Instance.MsgPrint("A small dart hits you!");
                            // Do 1d4 damage plus strength drain
                            damage = Program.Rng.DiceRoll(1, 4);
                            Player.TakeHit(damage, "a dart trap");
                            Player.TryDecreasingAbilityScore(Ability.Strength);
                        }
                        else
                        {
                            Profile.Instance.MsgPrint("A small dart barely misses you.");
                        }
                        break;
                    }
                case "DexDart":
                    {
                        // Dart traps need a to-hit roll
                        if (TrapCheckHitOnPlayer(125))
                        {
                            Profile.Instance.MsgPrint("A small dart hits you!");
                            // Do 1d4 damage plus dexterity drain
                            damage = Program.Rng.DiceRoll(1, 4);
                            Player.TakeHit(damage, "a dart trap");
                            Player.TryDecreasingAbilityScore(Ability.Dexterity);
                        }
                        else
                        {
                            Profile.Instance.MsgPrint("A small dart barely misses you.");
                        }
                        break;
                    }
                case "ConDart":
                    {
                        // Dart traps need a to-hit roll
                        if (TrapCheckHitOnPlayer(125))
                        {
                            Profile.Instance.MsgPrint("A small dart hits you!");
                            // Do 1d4 damage plus constitution drain
                            damage = Program.Rng.DiceRoll(1, 4);
                            Player.TakeHit(damage, "a dart trap");
                            Player.TryDecreasingAbilityScore(Ability.Constitution);
                        }
                        else
                        {
                            Profile.Instance.MsgPrint("A small dart barely misses you.");
                        }
                        break;
                    }
                case "BlindGas":
                    {
                        // Blind the player
                        Profile.Instance.MsgPrint("A black gas surrounds you!");
                        if (!Player.HasBlindnessResistance)
                        {
                            Player.SetTimedBlindness(Player.TimedBlindness + Program.Rng.RandomLessThan(50) + 25);
                        }
                        break;
                    }
                case "ConfuseGas":
                    {
                        // Confuse the player
                        Profile.Instance.MsgPrint("A gas of scintillating colours surrounds you!");
                        if (!Player.HasConfusionResistance)
                        {
                            Player.SetTimedConfusion(Player.TimedConfusion + Program.Rng.RandomLessThan(20) + 10);
                        }
                        break;
                    }
                case "PoisonGas":
                    {
                        // Poison the player
                        Profile.Instance.MsgPrint("A pungent green gas surrounds you!");
                        if (!Player.HasPoisonResistance && Player.TimedPoisonResistance == 0)
                        {
                            // Hagarg Ryonis may save you from the poison
                            if (Program.Rng.DieRoll(10) <= Player.Religion.GetNamedDeity(Pantheon.GodName.Hagarg_Ryonis).AdjustedFavour)
                            {
                                Profile.Instance.MsgPrint("Hagarg Ryonis's favour protects you!");
                            }
                            else
                            {
                                Player.SetTimedPoison(Player.TimedPoison + Program.Rng.RandomLessThan(20) + 10);
                            }
                        }
                        break;
                    }
                case "SleepGas":
                    {
                        // Paralyse the player
                        Profile.Instance.MsgPrint("A strange white mist surrounds you!");
                        if (!Player.HasFreeAction)
                        {
                            Player.SetTimedParalysis(Player.TimedParalysis + Program.Rng.RandomLessThan(10) + 5);
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// Check to see if a monster has damaging aura, and if so damage the player with it
        /// </summary>
        /// <param name="monster"> The monster to check </param>
        private void TouchZapPlayer(Monster monster)
        {
            int auraDamage;
            MonsterRace race = monster.Race;
            // If we have a fire aura, apply it
            if ((race.Flags2 & MonsterFlag2.FireAura) != 0)
            {
                if (!Player.HasFireImmunity)
                {
                    auraDamage = Program.Rng.DiceRoll(1 + (race.Level / 26), 1 + (race.Level / 17));
                    string auraDam = monster.MonsterDesc(0x88);
                    Profile.Instance.MsgPrint("You are suddenly very hot!");
                    if (Player.TimedFireResistance != 0)
                    {
                        auraDamage = (auraDamage + 2) / 3;
                    }
                    if (Player.HasFireResistance)
                    {
                        auraDamage = (auraDamage + 2) / 3;
                    }
                    Player.TakeHit(auraDamage, auraDam);
                    race.Knowledge.RFlags2 |= MonsterFlag2.FireAura;
                    _saveGame.HandleStuff();
                }
            }
            // If we have a lightning aura, apply it
            if ((race.Flags2 & MonsterFlag2.LightningAura) != 0 && !Player.HasLightningImmunity)
            {
                auraDamage = Program.Rng.DiceRoll(1 + (race.Level / 26), 1 + (race.Level / 17));
                string auraDam = monster.MonsterDesc(0x88);
                if (Player.TimedLightningResistance != 0)
                {
                    auraDamage = (auraDamage + 2) / 3;
                }
                if (Player.HasLightningResistance)
                {
                    auraDamage = (auraDamage + 2) / 3;
                }
                Profile.Instance.MsgPrint("You get zapped!");
                Player.TakeHit(auraDamage, auraDam);
                race.Knowledge.RFlags2 |= MonsterFlag2.LightningAura;
                _saveGame.HandleStuff();
            }
        }

        /// <summary>
        /// Check to see if a trap hits a player
        /// </summary>
        /// <param name="attackStrength"> The power of the trap's attack </param>
        /// <returns> True if the player was hit, false otherwise </returns>
        private bool TrapCheckHitOnPlayer(int attackStrength)
        {
            // Always a 5% chance to hit and 5% chance to miss
            int k = Program.Rng.RandomLessThan(100);
            if (k < 10)
            {
                return k < 5;
            }
            // If negative chance we miss
            if (attackStrength <= 0)
            {
                return false;
            }
            // Roll for the attack
            int armourClass = Player.BaseArmourClass + Player.ArmourClassBonus;
            return Program.Rng.DieRoll(attackStrength) > armourClass * 3 / 4;
        }
    }
}