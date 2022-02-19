using Cthangband.Enumerations;
using Cthangband.Mutations;
using Cthangband.Projection;
using Cthangband.StaticData;
using Cthangband.UI;
using System;

namespace Cthangband
{
    /// <summary>
    /// Class for handling most player commands
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
        private NavigationState _navigationState = new NavigationState();

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
                        if (!targetEngine.GetAimDir(out direction))
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
                        if (!targetEngine.GetAimDir(out direction))
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
                        if (!targetEngine.GetAimDir(out direction))
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
                        if (!targetEngine.GetAimDir(out direction))
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
                        if (!targetEngine.GetAimDir(out direction))
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
                        if (!targetEngine.GetAimDir(out direction))
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
                        if (!targetEngine.GetAimDir(out direction))
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
                        if (!targetEngine.GetAimDir(out direction))
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
                        if (!targetEngine.GetAimDir(out direction))
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
                        if (!targetEngine.GetAimDir(out direction))
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
                        if (!targetEngine.GetAimDir(out direction))
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
                        if (!targetEngine.GetAimDir(out direction))
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
                        if (!targetEngine.GetAimDir(out direction))
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
                        if (!targetEngine.GetAimDir(out direction))
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
                        if (!targetEngine.GetAimDir(out direction))
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
                        if (!targetEngine.GetAimDir(out direction))
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
                        if (!targetEngine.GetAimDir(out direction))
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
                        if (!targetEngine.GetAimDir(out direction))
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
                                Monster mPtr = Level.Monsters[cPtr.Monster];
                                if (cPtr.Monster != 0 && (mPtr.IsVisible || Level.GridPassable(y, x)))
                                {
                                    PyAttack(y, x);
                                }
                            }
                        }
                        item.RechargeTimeLeft = 250;
                        break;
                    }
                case RandomArtifactPower.ActVampire2:
                    {
                        // Drain 100 health from an opponent, and give it to the player
                        if (!targetEngine.GetAimDir(out direction))
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
                        if (!targetEngine.GetAimDir(out direction))
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
                        if (!targetEngine.GetAimDir(out direction))
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
                        if (!targetEngine.GetAimDir(out direction))
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
                        if (!targetEngine.GetAimDir(out direction))
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
                        if (!targetEngine.GetAimDir(out direction))
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
                        if (!targetEngine.GetAimDir(out direction))
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
                        if (!targetEngine.GetAimDir(out direction))
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
                        if (!targetEngine.GetAimDir(out direction))
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
                Player.UpdateFlags |= Constants.PuView | Constants.PuLight;
                Player.UpdateFlags |= Constants.PuDistance;
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
                Player.UpdateFlags |= Constants.PuView | Constants.PuLight | Constants.PuMonsters;
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
            Player.UpdateFlags |= Constants.PuTorch;
        }

        /// <summary>
        /// Work out whether the player's missile attack was a critical hit
        /// </summary>
        /// <param name="weight"> The weight of the player's weapon </param>
        /// <param name="plus"> The magical plusses on the weapon </param>
        /// <param name="damage"> The damage done </param>
        /// <returns> The modified damage based on the level of critical </returns>
        public int CriticalShot(int weight, int plus, int damage)
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
                Player.UpdateFlags |= Constants.PuBonus;
                Player.UpdateFlags |= Constants.PuMana;
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
                Player.UpdateFlags |= Constants.PuBonus;
                Player.UpdateFlags |= Constants.PuMana;
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
                Level.CaveRemoveFeat(y, x);
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
                Player.RedrawFlags |= RedrawFlag.PrMana;
                return true;
            }
            // Use some mana in the attempt, even if we failed
            Profile.Instance.MsgPrint("You mana is insufficient to power the effect.");
            Player.Mana -= Program.Rng.RandomLessThan(Player.Mana / 2);
            Player.RedrawFlags |= RedrawFlag.PrMana;
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
        /// <param name="doPickup"> Whether or not to pick up any objects we step on </param>
        public void MovePlayer(int direction, bool doPickup)
        {
            bool canPassWalls = false;
            int newY = Player.MapY + Level.KeypadDirectionYOffset[direction];
            int newX = Player.MapX + Level.KeypadDirectionXOffset[direction];
            GridTile tile = Level.Grid[newY][newX];
            Monster monster = Level.Monsters[tile.Monster];
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
            if (tile.Monster != 0 && (monster.IsVisible || Level.GridPassable(newY, newX) || canPassWalls))
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
                        _saveGame.HealthTrack(tile.Monster);
                    }
                    // If we can't see it then let us push past it and tell us what happened
                    else if (Level.GridPassable(Player.MapY, Player.MapX) ||
                             (monster.Race.Flags2 & MonsterFlag2.PassWall) != 0)
                    {
                        Profile.Instance.MsgPrint($"You push past {monsterName}.");
                        monster.MapY = Player.MapY;
                        monster.MapX = Player.MapX;
                        Level.Grid[Player.MapY][Player.MapX].Monster = tile.Monster;
                        tile.Monster = 0;
                        Level.Monsters.UpdateMonsterVisibility(Level.Grid[Player.MapY][Player.MapX].Monster, true);
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
                    PyAttack(newY, newX);
                    return;
                }
            }
            // We didn't attack a monster or get blocked by one, so start testing terrain features
            if (!doPickup && tile.FeatureType.IsTrap)
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
                Player.RedrawFlags |= RedrawFlag.PrDtrap;
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
            Player.UpdateFlags |= Constants.PuView | Constants.PuLight | Constants.PuFlow;
            Player.UpdateFlags |= Constants.PuDistance;
            Player.RedrawFlags |= RedrawFlag.PrMap;
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
            PickUpItems(!doPickup);
            // If we've just entered a shop tile, then enter the actual shop
            if (tile.FeatureType.IsShop)
            {
                _saveGame.Disturb(false);
                Gui.CommandNew = '_';
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
                    Player.UpdateFlags |= Constants.PuView | Constants.PuLight | Constants.PuMonsters;
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
                Player.UpdateFlags |= Constants.PuView | Constants.PuLight | Constants.PuMonsters;
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
            for (int thisItemIndex = tile.Item; thisItemIndex != 0; thisItemIndex = nextItemIndex)
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
                    Player.RedrawFlags |= RedrawFlag.PrGold;
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
                            Player.RedrawFlags |= RedrawFlag.PrMana;
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
                            Player.UpdateFlags |= Constants.PuBonus;
                            _saveGame.HandleStuff();
                        }
                        if (Player.RaceIndex != Player.RaceIndexAtBirth)
                        {
                            Profile.Instance.MsgPrint("You are restored to your original race.");
                            Player.ChangeRace(Player.RaceIndexAtBirth);
                            SaveGame.Instance.Level.RedrawSingleLocation(Player.MapY, Player.MapX);
                        }
                        identified = true;
                        break;
                    }
            }
            return identified;
        }

        public void PyAttack(int y, int x)
        {
            GridTile cPtr = Level.Grid[y][x];
            Monster mPtr = Level.Monsters[cPtr.Monster];
            MonsterRace rPtr = mPtr.Race;
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
            if (Player.ProfessionIndex == CharacterClass.Rogue)
            {
                if (mPtr.SleepLevel != 0 && mPtr.IsVisible)
                {
                    backstab = true;
                }
                else if (mPtr.FearLevel != 0 && mPtr.IsVisible)
                {
                    stabFleeing = true;
                }
            }
            _saveGame.Disturb(true);
            mPtr.SleepLevel = 0;
            string mName = mPtr.MonsterDesc(0);
            if (mPtr.IsVisible)
            {
                _saveGame.HealthTrack(cPtr.Monster);
            }
            if ((mPtr.Mind & Constants.SmFriendly) != 0 &&
                !(Player.TimedStun != 0 || Player.TimedConfusion != 0 || Player.TimedHallucinations != 0 || !mPtr.IsVisible))
            {
                if (string.IsNullOrEmpty(Player.Inventory[InventorySlot.MeleeWeapon].RandartName))
                {
                    Profile.Instance.MsgPrint($"You stop to avoid hitting {mName}.");
                    return;
                }
                if (Player.Inventory[InventorySlot.MeleeWeapon].RandartName != "'Stormbringer'")
                {
                    Profile.Instance.MsgPrint($"You stop to avoid hitting {mName}.");
                    return;
                }
                Profile.Instance.MsgPrint($"Your black blade greedily attacks {mName}!");
            }
            if (Player.TimedFear != 0)
            {
                Profile.Instance.MsgPrint(mPtr.IsVisible
                    ? $"You are too afraid to attack {mName}!"
                    : "There is something scary in your way!");
                return;
            }
            Item oPtr = Player.Inventory[InventorySlot.MeleeWeapon];
            int bonus = Player.AttackBonus + oPtr.BonusToHit;
            int chance = Player.SkillMelee + (bonus * Constants.BthPlusAdj);
            _saveGame.EnergyUse = 100;
            int num = 0;
            while (num++ < Player.MeleeAttacksPerRound)
            {
                if (TestHitNorm(chance, rPtr.ArmourClass, mPtr.IsVisible))
                {
                    PlayerStatus playerStatus = new PlayerStatus(Player, Level);
                    Gui.PlaySound(SoundEffect.MeleeHit);
                    if (!(backstab || stabFleeing))
                    {
                        if (!((Player.ProfessionIndex == CharacterClass.Monk || Player.ProfessionIndex == CharacterClass.Mystic) && playerStatus.MartialArtistEmptyHands()))
                        {
                            Profile.Instance.MsgPrint($"You hit {mName}.");
                        }
                    }
                    else if (backstab)
                    {
                        Profile.Instance.MsgPrint(
                            $"You cruelly stab the helpless, sleeping {mPtr.Race.Name}!");
                    }
                    else
                    {
                        Profile.Instance.MsgPrint(
                            $"You backstab the fleeing {mPtr.Race.Name}!");
                    }
                    int k = 1;
                    oPtr.GetMergedFlags(f1, f2, f3);
                    bool chaosEffect = f1.IsSet(ItemFlag1.Chaotic) && Program.Rng.DieRoll(2) == 1;
                    if (f1.IsSet(ItemFlag1.Vampiric) || (chaosEffect && Program.Rng.DieRoll(5) < 3))
                    {
                        chaosEffect = false;
                        if (!((rPtr.Flags3 & MonsterFlag3.Undead) != 0 || (rPtr.Flags3 & MonsterFlag3.Nonliving) != 0))
                        {
                            drainResult = mPtr.Health;
                        }
                        else
                        {
                            drainResult = 0;
                        }
                    }
                    bool vorpalCut = f1.IsSet(ItemFlag1.Vorpal) &&
                        Program.Rng.DieRoll(oPtr.FixedArtifactIndex == FixedArtifactId.SwordVorpalBlade ? 3 : 6) == 1;
                    if ((Player.ProfessionIndex == CharacterClass.Monk || Player.ProfessionIndex == CharacterClass.Mystic) && playerStatus.MartialArtistEmptyHands())
                    {
                        int specialEffect = 0, stunEffect = 0, times;
                        MartialArtsAttack maPtr = GlobalData.MaBlows[0];
                        MartialArtsAttack oldPtr = GlobalData.MaBlows[0];
                        int resistStun = 0;
                        if ((rPtr.Flags1 & MonsterFlag1.Unique) != 0)
                        {
                            resistStun += 88;
                        }
                        if ((rPtr.Flags3 & MonsterFlag3.ImmuneConfusion) != 0)
                        {
                            resistStun += 44;
                        }
                        if ((rPtr.Flags3 & MonsterFlag3.ImmuneSleep) != 0)
                        {
                            resistStun += 44;
                        }
                        if ((rPtr.Flags3 & MonsterFlag3.Undead) != 0 || (rPtr.Flags3 & MonsterFlag3.Nonliving) != 0)
                        {
                            resistStun += 88;
                        }
                        for (times = 0; times < (Player.Level < 7 ? 1 : Player.Level / 7); times++)
                        {
                            do
                            {
                                maPtr = GlobalData.MaBlows[Program.Rng.DieRoll(Constants.MaxMa) - 1];
                            } while (maPtr.MinLevel > Player.Level || Program.Rng.DieRoll(Player.Level) < maPtr.Chance);
                            if (maPtr.MinLevel > oldPtr.MinLevel && !(Player.TimedStun != 0 || Player.TimedConfusion != 0))
                            {
                                oldPtr = maPtr;
                            }
                            else
                            {
                                maPtr = oldPtr;
                            }
                        }
                        k = Program.Rng.DiceRoll(maPtr.Dd, maPtr.Ds);
                        if (maPtr.Effect == Constants.MaKnee)
                        {
                            if ((rPtr.Flags1 & MonsterFlag1.Male) != 0)
                            {
                                Profile.Instance.MsgPrint($"You hit {mName} in the groin with your knee!");
                                specialEffect = Constants.MaKnee;
                            }
                            else
                            {
                                Profile.Instance.MsgPrint(string.Format(maPtr.Desc, mName));
                            }
                        }
                        else if (maPtr.Effect == Constants.MaSlow)
                        {
                            if ((rPtr.Flags1 & MonsterFlag1.NeverMove) == 0 ||
                                "UjmeEv$,DdsbBFIJQSXclnw!=?".Contains(rPtr.Character.ToString()))
                            {
                                Profile.Instance.MsgPrint($"You kick {mName} in the ankle.");
                                specialEffect = Constants.MaSlow;
                            }
                            else
                            {
                                Profile.Instance.MsgPrint(string.Format(maPtr.Desc, mName));
                            }
                        }
                        else
                        {
                            if (maPtr.Effect != 0)
                            {
                                stunEffect = (maPtr.Effect / 2) + Program.Rng.DieRoll(maPtr.Effect / 2);
                            }
                            Profile.Instance.MsgPrint(string.Format(maPtr.Desc, mName));
                        }
                        k = CriticalNorm(Player.Level * Program.Rng.DieRoll(10), maPtr.MinLevel, k);
                        if (specialEffect == Constants.MaKnee && k + Player.DamageBonus < mPtr.Health)
                        {
                            Profile.Instance.MsgPrint($"{mName} moans in agony!");
                            stunEffect = 7 + Program.Rng.DieRoll(13);
                            resistStun /= 3;
                        }
                        else if (specialEffect == Constants.MaSlow && k + Player.DamageBonus < mPtr.Health)
                        {
                            if ((rPtr.Flags1 & MonsterFlag1.Unique) == 0 && Program.Rng.DieRoll(Player.Level) > rPtr.Level &&
                                mPtr.Speed > 60)
                            {
                                Profile.Instance.MsgPrint($"{mName} starts limping slower.");
                                mPtr.Speed -= 10;
                            }
                        }
                        if (stunEffect != 0 && k + Player.DamageBonus < mPtr.Health)
                        {
                            if (Player.Level > Program.Rng.DieRoll(rPtr.Level + resistStun + 10))
                            {
                                Profile.Instance.MsgPrint(mPtr.StunLevel != 0
                                    ? $"{mName} is more stunned."
                                    : $"{mName} is stunned.");
                                mPtr.StunLevel += stunEffect;
                            }
                        }
                    }
                    else if (oPtr.ItemType != null)
                    {
                        k = Program.Rng.DiceRoll(oPtr.DamageDice, oPtr.DamageDiceSides);
                        k = oPtr.AdjustDamageForMonsterType(k, mPtr);
                        if (backstab)
                        {
                            k *= 3 + (Player.Level / 40);
                        }
                        else if (stabFleeing)
                        {
                            k = 3 * k / 2;
                        }
                        if ((Player.HasQuakeWeapon && (k > 50 || Program.Rng.DieRoll(7) == 1)) ||
                            (chaosEffect && Program.Rng.DieRoll(250) == 1))
                        {
                            doQuake = true;
                            chaosEffect = false;
                        }
                        k = CriticalNorm(oPtr.Weight, oPtr.BonusToHit, k);
                        if (vorpalCut)
                        {
                            int stepK = k;
                            Profile.Instance.MsgPrint(oPtr.FixedArtifactIndex == FixedArtifactId.SwordVorpalBlade
                                ? "Your Vorpal Blade goes snicker-snack!"
                                : $"Your weapon cuts deep into {mName}!");
                            do
                            {
                                k += stepK;
                            } while (Program.Rng.DieRoll(oPtr.FixedArtifactIndex == FixedArtifactId.SwordVorpalBlade
                                         ? 2
                                         : 4) == 1);
                        }
                        k += oPtr.BonusDamage;
                    }
                    k += Player.DamageBonus;
                    if (k < 0)
                    {
                        k = 0;
                    }
                    if (Level.Monsters.DamageMonster(cPtr.Monster, k, out fear, null))
                    {
                        noExtra = true;
                        break;
                    }
                    if ((mPtr.Mind & Constants.SmFriendly) != 0)
                    {
                        Profile.Instance.MsgPrint($"{mName} gets angry!");
                        mPtr.Mind &= ~Constants.SmFriendly;
                    }
                    TouchZapPlayer(mPtr);
                    if (drainResult != 0)
                    {
                        drainResult -= mPtr.Health;
                        if (drainResult > 0)
                        {
                            int drainHeal = Program.Rng.DiceRoll(4, drainResult / 6);
                            if (drainLeft != 0)
                            {
                                if (drainHeal >= drainLeft)
                                {
                                    drainHeal = drainLeft;
                                }
                                if (drainMsg)
                                {
                                    Profile.Instance.MsgPrint($"Your weapon drains life from {mName}!");
                                }
                                Player.RestoreHealth(drainHeal);
                            }
                        }
                    }
                    if (Player.HasConfusingTouch || (chaosEffect && Program.Rng.DieRoll(10) != 1))
                    {
                        Player.HasConfusingTouch = false;
                        if (!chaosEffect)
                        {
                            Profile.Instance.MsgPrint("Your hands stop glowing.");
                        }
                        if ((rPtr.Flags3 & MonsterFlag3.ImmuneConfusion) != 0)
                        {
                            if (mPtr.IsVisible)
                            {
                                rPtr.Knowledge.RFlags3 |= MonsterFlag3.ImmuneConfusion;
                            }
                            Profile.Instance.MsgPrint($"{mName} is unaffected.");
                        }
                        else if (Program.Rng.RandomLessThan(100) < rPtr.Level)
                        {
                            Profile.Instance.MsgPrint($"{mName} is unaffected.");
                        }
                        else
                        {
                            Profile.Instance.MsgPrint($"{mName} appears confused.");
                            mPtr.ConfusionLevel += 10 + (Program.Rng.RandomLessThan(Player.Level) / 5);
                        }
                    }
                    else if (chaosEffect && Program.Rng.DieRoll(2) == 1)
                    {
                        Profile.Instance.MsgPrint($"{mName} disappears!");
                        _saveGame.SpellEffects.TeleportAway(cPtr.Monster, 50);
                        noExtra = true;
                        break;
                    }
                    else if (chaosEffect && Level.GridPassable(y, x) && Program.Rng.DieRoll(90) > rPtr.Level)
                    {
                        if (!((rPtr.Flags1 & MonsterFlag1.Unique) != 0 || (rPtr.Flags4 & MonsterFlag4.BreatheChaos) != 0 ||
                              (rPtr.Flags1 & MonsterFlag1.Guardian) != 0))
                        {
                            int tmp = _saveGame.SpellEffects.PolyRIdx(mPtr.Race);
                            if (tmp != mPtr.Race.Index)
                            {
                                Profile.Instance.MsgPrint($"{mName} changes!");
                                Level.Monsters.DeleteMonsterByIndex(cPtr.Monster, true);
                                MonsterRace race = Profile.Instance.MonsterRaces[tmp];
                                Level.Monsters.PlaceMonsterAux(y, x, race, false, false, false);
                                mPtr = Level.Monsters[cPtr.Monster];
                                mName = mPtr.MonsterDesc(0);
                                fear = false;
                            }
                        }
                        else
                        {
                            Profile.Instance.MsgPrint($"{mName} is unaffected.");
                        }
                    }
                }
                else
                {
                    Gui.PlaySound(SoundEffect.Miss);
                    Profile.Instance.MsgPrint($"You miss {mName}.");
                }
            }
            if (!noExtra)
            {
                foreach (Mutation naturalAttack in Player.Dna.NaturalAttacks)
                {
                    NaturalAttack(cPtr.Monster, naturalAttack, out fear, out _);
                }
            }
            if (fear && mPtr.IsVisible)
            {
                Gui.PlaySound(SoundEffect.MonsterFlees);
                Profile.Instance.MsgPrint($"{mName} flees in terror!");
            }
            if (doQuake)
            {
                _saveGame.SpellEffects.Earthquake(Player.MapY, Player.MapX, 10);
            }
        }

        public bool RacialAux(int minLevel, int cost, int useStat, int difficulty)
        {
            bool useHp = Player.Mana < cost;
            if (Player.Level < minLevel)
            {
                Profile.Instance.MsgPrint($"You need to attain level {minLevel} to use this power.");
                _saveGame.EnergyUse = 0;
                return false;
            }
            if (Player.TimedConfusion != 0)
            {
                Profile.Instance.MsgPrint("You are too confused to use this power.");
                _saveGame.EnergyUse = 0;
                return false;
            }
            if (useHp && Player.Health < cost)
            {
                if (!Gui.GetCheck("Really use the power in your weakened state? "))
                {
                    _saveGame.EnergyUse = 0;
                    return false;
                }
            }
            if (Player.TimedStun != 0)
            {
                difficulty += Player.TimedStun;
            }
            else if (Player.Level > minLevel)
            {
                int levAdj = (Player.Level - minLevel) / 3;
                if (levAdj > 10)
                {
                    levAdj = 10;
                }
                difficulty -= levAdj;
            }
            if (difficulty < 5)
            {
                difficulty = 5;
            }
            _saveGame.EnergyUse = 100;
            if (useHp)
            {
                Player.TakeHit((cost / 2) + Program.Rng.DieRoll(cost / 2), "concentrating too hard");
            }
            else
            {
                Player.Mana -= (cost / 2) + Program.Rng.DieRoll(cost / 2);
            }
            Player.RedrawFlags |= RedrawFlag.PrHp;
            Player.RedrawFlags |= RedrawFlag.PrMana;
            if (Program.Rng.DieRoll(Player.AbilityScores[useStat].Innate) >=
                (difficulty / 2) + Program.Rng.DieRoll(difficulty / 2))
            {
                return true;
            }
            Profile.Instance.MsgPrint("You've failed to concentrate hard enough.");
            return false;
        }

        public int RandomPotion()
        {
            var fakeSval = 0;
            switch (Program.Rng.DieRoll(48))
            {
                case 1:
                    fakeSval = PotionType.Water;
                    break;

                case 2:
                    fakeSval = PotionType.AppleJuice;
                    break;

                case 3:
                    fakeSval = PotionType.Slowness;
                    break;

                case 4:
                    fakeSval = PotionType.SaltWater;
                    break;

                case 5:
                    fakeSval = PotionType.Poison;
                    break;

                case 6:
                    fakeSval = PotionType.Blindness;
                    break;

                case 7:
                    fakeSval = PotionType.Confusion;
                    break;

                case 8:
                    fakeSval = PotionType.Sleep;
                    break;

                case 9:
                    fakeSval = PotionType.Infravision;
                    break;

                case 10:
                    fakeSval = PotionType.DetectInvis;
                    break;

                case 11:
                    fakeSval = PotionType.SlowPoison;
                    break;

                case 12:
                    fakeSval = PotionType.CurePoison;
                    break;

                case 13:
                    fakeSval = PotionType.Boldness;
                    break;

                case 14:
                    fakeSval = PotionType.Speed;
                    break;

                case 15:
                    fakeSval = PotionType.ResistHeat;
                    break;

                case 16:
                    fakeSval = PotionType.ResistCold;
                    break;

                case 17:
                    fakeSval = PotionType.Heroism;
                    break;

                case 18:
                    fakeSval = PotionType.BeserkStrength;
                    break;

                case 19:
                    fakeSval = PotionType.CureLight;
                    break;

                case 20:
                    fakeSval = PotionType.CureSerious;
                    break;

                case 21:
                    fakeSval = PotionType.CureCritical;
                    break;

                case 22:
                    fakeSval = PotionType.Healing;
                    break;

                case 23:
                    fakeSval = PotionType.StarHealing;
                    break;

                case 24:
                    fakeSval = PotionType.Life;
                    break;

                case 25:
                    fakeSval = PotionType.RestoreMana;
                    break;

                case 26:
                    fakeSval = PotionType.RestoreExp;
                    break;

                case 27:
                    fakeSval = PotionType.ResStr;
                    break;

                case 28:
                    fakeSval = PotionType.ResInt;
                    break;

                case 29:
                    fakeSval = PotionType.ResWis;
                    break;

                case 30:
                    fakeSval = PotionType.ResDex;
                    break;

                case 31:
                    fakeSval = PotionType.ResCon;
                    break;

                case 32:
                    fakeSval = PotionType.ResCha;
                    break;

                case 33:
                    fakeSval = PotionType.IncStr;
                    break;

                case 34:
                    fakeSval = PotionType.IncInt;
                    break;

                case 35:
                    fakeSval = PotionType.IncWis;
                    break;

                case 36:
                    fakeSval = PotionType.IncDex;
                    break;

                case 38:
                    fakeSval = PotionType.IncCon;
                    break;

                case 39:
                    fakeSval = PotionType.IncCha;
                    break;

                case 40:
                    fakeSval = PotionType.Augmentation;
                    break;

                case 41:
                    fakeSval = PotionType.Enlightenment;
                    break;

                case 42:
                    fakeSval = PotionType.StarEnlightenment;
                    break;

                case 43:
                    fakeSval = PotionType.SelfKnowledge;
                    break;

                case 44:
                    fakeSval = PotionType.Experience;
                    break;

                case 45:
                    fakeSval = PotionType.Resistance;
                    break;

                case 46:
                    fakeSval = PotionType.Curing;
                    break;

                case 47:
                    fakeSval = PotionType.Invulnerability;
                    break;

                case 48:
                    fakeSval = PotionType.NewLife;
                    break;
            }
            return fakeSval;
        }

        public void RingOfPower(int dir)
        {
            switch (Program.Rng.DieRoll(10))
            {
                case 1:
                case 2:
                    {
                        Profile.Instance.MsgPrint("You are surrounded by a malignant aura.");
                        Player.DecreaseAbilityScore(Ability.Strength, 50, true);
                        Player.DecreaseAbilityScore(Ability.Intelligence, 50, true);
                        Player.DecreaseAbilityScore(Ability.Wisdom, 50, true);
                        Player.DecreaseAbilityScore(Ability.Dexterity, 50, true);
                        Player.DecreaseAbilityScore(Ability.Constitution, 50, true);
                        Player.DecreaseAbilityScore(Ability.Charisma, 50, true);
                        Player.ExperiencePoints -= Player.ExperiencePoints / 4;
                        Player.MaxExperienceGained -= Player.ExperiencePoints / 4;
                        Player.CheckExperience();
                        break;
                    }
                case 3:
                    {
                        Profile.Instance.MsgPrint("You are surrounded by a powerful aura.");
                        _saveGame.SpellEffects.DispelMonsters(1000);
                        break;
                    }
                case 4:
                case 5:
                case 6:
                    {
                        _saveGame.SpellEffects.FireBall(new ProjectMana(SaveGame.Instance.SpellEffects), dir, 300, 3);
                        break;
                    }
                case 7:
                case 8:
                case 9:
                case 10:
                    {
                        _saveGame.SpellEffects.FireBolt(new ProjectMana(SaveGame.Instance.SpellEffects), dir, 250);
                        break;
                    }
            }
        }

        public void RunStep(int dir)
        {
            if (dir != 0)
            {
                if (SeeWall(dir, Player.MapY, Player.MapX))
                {
                    Profile.Instance.MsgPrint("You cannot run in that direction.");
                    _saveGame.Disturb(false);
                    return;
                }
                Player.UpdateFlags |= Constants.PuTorch;
                RunInit(dir);
            }
            else
            {
                if (RunTest())
                {
                    _saveGame.Disturb(false);
                    return;
                }
            }
            if (--_saveGame.Running <= 0)
            {
                return;
            }
            _saveGame.EnergyUse = 100;
            MovePlayer(_navigationState._findCurrent, false);
        }

        public void Rustproof()
        {
            _saveGame.ItemFilter = _saveGame.SpellEffects.ItemTesterHookArmour;
            if (!_saveGame.GetItem(out int item, "Rustproof which piece of armour? ", true, true, true))
            {
                if (item == -2)
                {
                    Profile.Instance.MsgPrint("You have nothing to rustproof.");
                }
                return;
            }
            Item oPtr = item >= 0 ? Player.Inventory[item] : Level.Items[0 - item];
            string oName = oPtr.Description(false, 0);
            oPtr.RandartFlags3.Set(ItemFlag3.IgnoreAcid);
            string your;
            string s;
            if (oPtr.BonusArmourClass < 0 && oPtr.IdentifyFlags.IsClear(Constants.IdentCursed))
            {
                your = item > 0 ? "Your" : "The";
                s = oPtr.Count > 1 ? "" : "s";
                Profile.Instance.MsgPrint($"{your} {oName} look{s} as good as new!");
                oPtr.BonusArmourClass = 0;
            }
            your = item > 0 ? "Your" : "The";
            s = oPtr.Count > 1 ? "are" : "is";
            Profile.Instance.MsgPrint($"{your} {oName} {s} now protected against corrosion.");
        }

        public void Search()
        {
            int chance = Player.SkillSearching;
            if (Player.TimedBlindness != 0 || Level.NoLight())
            {
                chance /= 10;
            }
            if (Player.TimedConfusion != 0 || Player.TimedHallucinations != 0)
            {
                chance /= 10;
            }
            for (int y = Player.MapY - 1; y <= Player.MapY + 1; y++)
            {
                for (int x = Player.MapX - 1; x <= Player.MapX + 1; x++)
                {
                    if (Program.Rng.RandomLessThan(100) < chance)
                    {
                        GridTile cPtr = Level.Grid[y][x];
                        if (cPtr.FeatureType.Name == "Invis")
                        {
                            _saveGame.Level.PickTrap(y, x);
                            Profile.Instance.MsgPrint("You have found a trap.");
                            _saveGame.Disturb(false);
                        }
                        if (cPtr.FeatureType.Name == "SecretDoor")
                        {
                            Profile.Instance.MsgPrint("You have found a secret door.");
                            Player.GainExperience(1);
                            Level.ReplaceSecretDoor(y, x);
                            _saveGame.Disturb(false);
                        }
                        int nextOIdx;
                        for (int thisOIdx = cPtr.Item; thisOIdx != 0; thisOIdx = nextOIdx)
                        {
                            Item oPtr = Level.Items[thisOIdx];
                            nextOIdx = oPtr.NextInStack;
                            if (oPtr.Category != ItemCategory.Chest)
                            {
                                continue;
                            }
                            if (GlobalData.ChestTraps[oPtr.TypeSpecificValue] == 0)
                            {
                                continue;
                            }
                            if (!oPtr.IsKnown())
                            {
                                Profile.Instance.MsgPrint("You have discovered a trap on the chest!");
                                oPtr.BecomeKnown();
                                _saveGame.Disturb(false);
                            }
                        }
                    }
                }
            }
        }

        public void SummonObject(int dir, int wgt, bool requireLos)
        {
            int ty, tx;
            GridTile cPtr;
            if (Level.Grid[Player.MapY][Player.MapX].Item != 0)
            {
                Profile.Instance.MsgPrint("You can't fetch when you're already standing on something.");
                return;
            }
            TargetEngine targetEngine = new TargetEngine(Player, Level);
            if (dir == 5 && targetEngine.TargetOkay())
            {
                tx = _saveGame.TargetCol;
                ty = _saveGame.TargetRow;
                if (Level.Distance(Player.MapY, Player.MapX, ty, tx) > Constants.MaxRange)
                {
                    Profile.Instance.MsgPrint("You can't fetch something that far away!");
                    return;
                }
                cPtr = Level.Grid[ty][tx];
                if (requireLos && !Level.PlayerHasLosBold(ty, tx))
                {
                    Profile.Instance.MsgPrint("You have no direct line of sight to that location.");
                    return;
                }
            }
            else
            {
                ty = Player.MapY;
                tx = Player.MapX;
                do
                {
                    ty += Level.KeypadDirectionYOffset[dir];
                    tx += Level.KeypadDirectionXOffset[dir];
                    cPtr = Level.Grid[ty][tx];
                    if (Level.Distance(Player.MapY, Player.MapX, ty, tx) > Constants.MaxRange ||
                        !Level.GridPassable(ty, tx))
                    {
                        return;
                    }
                } while (cPtr.Item == 0);
            }
            Item oPtr = Level.Items[cPtr.Item];
            if (oPtr.Weight > wgt)
            {
                Profile.Instance.MsgPrint("The object is too heavy.");
                return;
            }
            int i = cPtr.Item;
            cPtr.Item = 0;
            Level.Grid[Player.MapY][Player.MapX].Item = i;
            oPtr.Y = Player.MapY;
            oPtr.X = Player.MapX;
            Level.NoteSpot(Player.MapY, Player.MapX);
            Player.RedrawFlags |= RedrawFlag.PrMap;
        }

        public bool TestHitFire(int chance, int ac, bool vis)
        {
            int k = Program.Rng.RandomLessThan(100);
            if (k < 10)
            {
                return k < 5;
            }
            if (chance <= 0)
            {
                return false;
            }
            if (!vis)
            {
                chance = (chance + 1) / 2;
            }
            return Program.Rng.RandomLessThan(chance) >= ac * 3 / 4;
        }

        public bool TunnelThroughTile(int y, int x)
        {
            bool more = false;
            _saveGame.EnergyUse = 100;
            GridTile cPtr = Level.Grid[y][x];
            if (cPtr.FeatureType.Category == FloorTileTypeCategory.Tree)
            {
                if (Player.SkillDigging > 40 + Program.Rng.RandomLessThan(100) && Twall(y, x))
                {
                    Profile.Instance.MsgPrint($"You have chopped down the {cPtr.FeatureType.Description}.");
                }
                else
                {
                    Profile.Instance.MsgPrint($"You hack away at the {cPtr.FeatureType.Description}.");
                    more = true;
                }
            }
            else if (cPtr.FeatureType.Name == "Pillar")
            {
                if (Player.SkillDigging > 40 + Program.Rng.RandomLessThan(300) && Twall(y, x))
                {
                    Profile.Instance.MsgPrint("You have broken down the pillar.");
                }
                else
                {
                    Profile.Instance.MsgPrint("You hack away at the pillar.");
                    more = true;
                }
            }
            else if (cPtr.FeatureType.Name == "Water")
            {
                Profile.Instance.MsgPrint("The water fills up your tunnel as quickly as you dig!");
            }
            else if (cPtr.FeatureType.IsPermanent)
            {
                Profile.Instance.MsgPrint($"The {cPtr.FeatureType.Description} resists your attempts to tunnel through it.");
            }
            else if (cPtr.FeatureType.Name.Contains("Wall"))
            {
                if (Player.SkillDigging > 40 + Program.Rng.RandomLessThan(1600) && Twall(y, x))
                {
                    Profile.Instance.MsgPrint("You have finished the tunnel.");
                }
                else
                {
                    Profile.Instance.MsgPrint("You tunnel into the granite wall.");
                    more = true;
                }
            }
            else if (cPtr.FeatureType.Name.Contains("Magma") || cPtr.FeatureType.Name.Contains("Quartz"))
            {
                bool okay;
                bool gold = false;
                bool hard = false;
                if (cPtr.FeatureType.Name.Contains("Treas"))
                {
                    gold = true;
                }
                if (cPtr.FeatureType.Name.Contains("Magma"))
                {
                    hard = true;
                }
                if (hard)
                {
                    okay = Player.SkillDigging > 20 + Program.Rng.RandomLessThan(800);
                }
                else
                {
                    okay = Player.SkillDigging > 10 + Program.Rng.RandomLessThan(400);
                }
                if (okay && Twall(y, x))
                {
                    if (gold)
                    {
                        _saveGame.Level.PlaceGold(y, x);
                        Profile.Instance.MsgPrint("You have found something!");
                    }
                    else
                    {
                        Profile.Instance.MsgPrint("You have finished the tunnel.");
                    }
                }
                else if (hard)
                {
                    Profile.Instance.MsgPrint("You tunnel into the quartz vein.");
                    more = true;
                }
                else
                {
                    Profile.Instance.MsgPrint("You tunnel into the magma vein.");
                    more = true;
                }
            }
            else if (cPtr.FeatureType.Name == "Rubble")
            {
                if (Player.SkillDigging > Program.Rng.RandomLessThan(200) && Twall(y, x))
                {
                    Profile.Instance.MsgPrint("You have removed the rubble.");
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
                    more = true;
                }
            }
            else
            {
                Profile.Instance.MsgPrint($"The {cPtr.FeatureType.Description} resists your attempts to tunnel through it.");
                more = true;
                Search();
            }
            if (!Level.GridPassable(y, x))
            {
                Player.UpdateFlags |= Constants.PuView | Constants.PuLight | Constants.PuFlow | Constants.PuMonsters;
            }
            if (!more)
            {
                Gui.PlaySound(SoundEffect.Dig);
            }
            return more;
        }

        public void UseRacialPower()
        {
            int plev = Player.Level;
            int dir;
            Projectile type;
            string typeDesc;
            if (Program.Rng.DieRoll(3) == 1)
            {
                type = new ProjectCold(SaveGame.Instance.SpellEffects);
                typeDesc = "cold";
            }
            else
            {
                type = new ProjectFire(SaveGame.Instance.SpellEffects);
                typeDesc = "fire";
            }
            TargetEngine targetEngine = new TargetEngine(Player, Level);
            switch (Player.RaceIndex)
            {
                case RaceId.Dwarf:
                    if (RacialAux(5, 5, Ability.Wisdom, 12))
                    {
                        Profile.Instance.MsgPrint("You examine your surroundings.");
                        _saveGame.SpellEffects.DetectTraps();
                        _saveGame.SpellEffects.DetectDoors();
                        _saveGame.SpellEffects.DetectStairs();
                    }
                    break;

                case RaceId.Hobbit:
                    if (RacialAux(15, 10, Ability.Intelligence, 10))
                    {
                        Item qPtr = new Item();
                        qPtr.AssignItemType(Profile.Instance.ItemTypes.LookupKind(ItemCategory.Food, FoodType.Ration));
                        _saveGame.Level.DropNear(qPtr, -1, Player.MapY, Player.MapX);
                        Profile.Instance.MsgPrint("You cook some food.");
                    }
                    break;

                case RaceId.Gnome:
                    if (RacialAux(5, 5 + (plev / 5), Ability.Intelligence, 12))
                    {
                        Profile.Instance.MsgPrint("Blink!");
                        _saveGame.SpellEffects.TeleportPlayer(10 + plev);
                    }
                    break;

                case RaceId.HalfOrc:
                    if (RacialAux(3, 5, Ability.Wisdom, Player.ProfessionIndex == CharacterClass.Warrior ? 5 : 10))
                    {
                        Profile.Instance.MsgPrint("You play tough.");
                        Player.SetTimedFear(0);
                    }
                    break;

                case RaceId.HalfTroll:
                    if (RacialAux(10, 12, Ability.Wisdom, Player.ProfessionIndex == CharacterClass.Warrior ? 6 : 12))
                    {
                        Profile.Instance.MsgPrint("RAAAGH!");
                        Player.SetTimedFear(0);
                        Player.SetTimedSuperheroism(Player.TimedSuperheroism + 10 + Program.Rng.DieRoll(plev));
                        Player.RestoreHealth(30);
                    }
                    break;

                case RaceId.Great:
                    int amberPower;
                    while (true)
                    {
                        if (!Gui.GetCom("Use Dream [T]ravel or [D]reaming? ", out char ch))
                        {
                            amberPower = 0;
                            break;
                        }
                        if (ch == 'D' || ch == 'd')
                        {
                            amberPower = 1;
                            break;
                        }
                        if (ch == 'T' || ch == 't')
                        {
                            amberPower = 2;
                            break;
                        }
                    }
                    if (amberPower == 1)
                    {
                        if (RacialAux(40, 75, Ability.Wisdom, 50))
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
                    else if (amberPower == 2)
                    {
                        if (RacialAux(30, 50, Ability.Intelligence, 50))
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

                case RaceId.TchoTcho:
                    if (RacialAux(8, 10, Ability.Wisdom, Player.ProfessionIndex == CharacterClass.Warrior ? 6 : 12))
                    {
                        Profile.Instance.MsgPrint("Raaagh!");
                        Player.SetTimedFear(0);
                        Player.SetTimedSuperheroism(Player.TimedSuperheroism + 10 + Program.Rng.DieRoll(plev));
                        Player.RestoreHealth(30);
                    }
                    break;

                case RaceId.HalfOgre:
                    if (RacialAux(25, 35, Ability.Intelligence, 15))
                    {
                        Profile.Instance.MsgPrint("You carefully set an Yellow Sign...");
                        _saveGame.SpellEffects.YellowSign();
                    }
                    break;

                case RaceId.HalfGiant:
                    if (RacialAux(20, 10, Ability.Strength, 12))
                    {
                        if (!targetEngine.GetAimDir(out dir))
                        {
                            break;
                        }
                        Profile.Instance.MsgPrint("You bash at a stone wall.");
                        _saveGame.SpellEffects.WallToMud(dir);
                    }
                    break;

                case RaceId.HalfTitan:
                    if (RacialAux(35, 20, Ability.Intelligence, 12))
                    {
                        Profile.Instance.MsgPrint("You examine your foes...");
                        _saveGame.SpellEffects.Probing();
                    }
                    break;

                case RaceId.Cyclops:
                    if (RacialAux(20, 15, Ability.Strength, 12))
                    {
                        if (!targetEngine.GetAimDir(out dir))
                        {
                            break;
                        }
                        Profile.Instance.MsgPrint("You throw a huge boulder.");
                        _saveGame.SpellEffects.FireBolt(new ProjectMissile(SaveGame.Instance.SpellEffects), dir,
                            3 * Player.Level / 2);
                    }
                    break;

                case RaceId.Yeek:
                    if (RacialAux(15, 15, Ability.Wisdom, 10))
                    {
                        if (!targetEngine.GetAimDir(out dir))
                        {
                            break;
                        }
                        Profile.Instance.MsgPrint("You make a horrible scream!");
                        _saveGame.SpellEffects.FearMonster(dir, plev);
                    }
                    break;

                case RaceId.Klackon:
                    if (RacialAux(9, 9, Ability.Dexterity, 14))
                    {
                        if (!targetEngine.GetAimDir(out dir))
                        {
                            break;
                        }
                        Profile.Instance.MsgPrint("You spit acid.");
                        if (Player.Level < 25)
                        {
                            _saveGame.SpellEffects.FireBolt(new ProjectAcid(SaveGame.Instance.SpellEffects), dir, plev);
                        }
                        else
                        {
                            _saveGame.SpellEffects.FireBall(new ProjectAcid(SaveGame.Instance.SpellEffects), dir, plev,
                                2);
                        }
                    }
                    break;

                case RaceId.Kobold:
                    if (RacialAux(12, 8, Ability.Dexterity, 14))
                    {
                        if (!targetEngine.GetAimDir(out dir))
                        {
                            break;
                        }
                        Profile.Instance.MsgPrint("You throw a dart of poison.");
                        _saveGame.SpellEffects.FireBolt(new ProjectPois(SaveGame.Instance.SpellEffects), dir, plev);
                    }
                    break;

                case RaceId.Nibelung:
                    if (RacialAux(5, 5, Ability.Wisdom, 10))
                    {
                        Profile.Instance.MsgPrint("You examine your surroundings.");
                        _saveGame.SpellEffects.DetectTraps();
                        _saveGame.SpellEffects.DetectDoors();
                        _saveGame.SpellEffects.DetectStairs();
                    }
                    break;

                case RaceId.DarkElf:
                    if (RacialAux(2, 2, Ability.Intelligence, 9))
                    {
                        if (!targetEngine.GetAimDir(out dir))
                        {
                            break;
                        }
                        Profile.Instance.MsgPrint("You cast a magic missile.");
                        _saveGame.SpellEffects.FireBoltOrBeam(10, new ProjectMissile(SaveGame.Instance.SpellEffects),
                            dir, Program.Rng.DiceRoll(3 + ((plev - 1) / 5), 4));
                    }
                    break;

                case RaceId.Draconian:
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
                                    type = new ProjectMissile(SaveGame.Instance.SpellEffects);
                                    typeDesc = "the elements";
                                }
                                else
                                {
                                    type = new ProjectExplode(SaveGame.Instance.SpellEffects);
                                    typeDesc = "shards";
                                }
                                break;

                            case CharacterClass.Mage:
                            case CharacterClass.WarriorMage:
                            case CharacterClass.HighMage:
                            case CharacterClass.Channeler:
                                if (Program.Rng.DieRoll(3) == 1)
                                {
                                    type = new ProjectMana(SaveGame.Instance.SpellEffects);
                                    typeDesc = "mana";
                                }
                                else
                                {
                                    type = new ProjectDisenchant(SaveGame.Instance.SpellEffects);
                                    typeDesc = "disenchantment";
                                }
                                break;

                            case CharacterClass.Fanatic:
                            case CharacterClass.Cultist:
                                if (Program.Rng.DieRoll(3) != 1)
                                {
                                    type = new ProjectConfusion(SaveGame.Instance.SpellEffects);
                                    typeDesc = "confusion";
                                }
                                else
                                {
                                    type = new ProjectChaos(SaveGame.Instance.SpellEffects);
                                    typeDesc = "chaos";
                                }
                                break;

                            case CharacterClass.Monk:
                                if (Program.Rng.DieRoll(3) != 1)
                                {
                                    type = new ProjectConfusion(SaveGame.Instance.SpellEffects);
                                    typeDesc = "confusion";
                                }
                                else
                                {
                                    type = new ProjectSound(SaveGame.Instance.SpellEffects);
                                    typeDesc = "sound";
                                }
                                break;

                            case CharacterClass.Mindcrafter:
                            case CharacterClass.Mystic:
                                if (Program.Rng.DieRoll(3) != 1)
                                {
                                    type = new ProjectConfusion(SaveGame.Instance.SpellEffects);
                                    typeDesc = "confusion";
                                }
                                else
                                {
                                    type = new ProjectPsi(SaveGame.Instance.SpellEffects);
                                    typeDesc = "mental energy";
                                }
                                break;

                            case CharacterClass.Priest:
                            case CharacterClass.Paladin:
                                if (Program.Rng.DieRoll(3) == 1)
                                {
                                    type = new ProjectHellFire(SaveGame.Instance.SpellEffects);
                                    typeDesc = "hellfire";
                                }
                                else
                                {
                                    type = new ProjectHolyFire(SaveGame.Instance.SpellEffects);
                                    typeDesc = "holy fire";
                                }
                                break;

                            case CharacterClass.Rogue:
                                if (Program.Rng.DieRoll(3) == 1)
                                {
                                    type = new ProjectDark(SaveGame.Instance.SpellEffects);
                                    typeDesc = "darkness";
                                }
                                else
                                {
                                    type = new ProjectPois(SaveGame.Instance.SpellEffects);
                                    typeDesc = "poison";
                                }
                                break;
                        }
                    }
                    if (RacialAux(1, Player.Level, Ability.Constitution, 12))
                    {
                        if (!targetEngine.GetAimDir(out dir))
                        {
                            break;
                        }
                        Profile.Instance.MsgPrint($"You breathe {typeDesc}.");
                        _saveGame.SpellEffects.FireBall(type, dir, Player.Level * 2, -(Player.Level / 15) + 1);
                    }
                    break;

                case RaceId.MindFlayer:
                    if (RacialAux(15, 12, Ability.Intelligence, 14))
                    {
                        if (!targetEngine.GetAimDir(out dir))
                        {
                            break;
                        }
                        Profile.Instance.MsgPrint("You concentrate and your eyes glow red...");
                        _saveGame.SpellEffects.FireBolt(new ProjectPsi(SaveGame.Instance.SpellEffects), dir, plev);
                    }
                    break;

                case RaceId.Imp:
                    if (RacialAux(9, 15, Ability.Wisdom, 15))
                    {
                        if (!targetEngine.GetAimDir(out dir))
                        {
                            break;
                        }
                        if (Player.Level >= 30)
                        {
                            Profile.Instance.MsgPrint("You cast a ball of fire.");
                            _saveGame.SpellEffects.FireBall(new ProjectFire(SaveGame.Instance.SpellEffects), dir, plev,
                                2);
                        }
                        else
                        {
                            Profile.Instance.MsgPrint("You cast a bolt of fire.");
                            _saveGame.SpellEffects.FireBolt(new ProjectFire(SaveGame.Instance.SpellEffects), dir, plev);
                        }
                    }
                    break;

                case RaceId.Golem:
                    if (RacialAux(20, 15, Ability.Constitution, 8))
                    {
                        Player.SetTimedStoneskin(Player.TimedStoneskin + Program.Rng.DieRoll(20) + 30);
                    }
                    break;

                case RaceId.Skeleton:
                case RaceId.Zombie:
                    if (RacialAux(30, 30, Ability.Wisdom, 18))
                    {
                        Profile.Instance.MsgPrint("You attempt to restore your lost energies.");
                        Player.RestoreLevel();
                    }
                    break;

                case RaceId.Vampire:
                    if (RacialAux(2, 1 + (plev / 3), Ability.Constitution, 9))
                    {
                        if (!targetEngine.GetRepDir(out dir))
                        {
                            break;
                        }
                        int y = Player.MapY + Level.KeypadDirectionYOffset[dir];
                        int x = Player.MapX + Level.KeypadDirectionXOffset[dir];
                        GridTile cPtr = Level.Grid[y][x];
                        if (cPtr.Monster == 0)
                        {
                            Profile.Instance.MsgPrint("You bite into thin air!");
                            break;
                        }
                        Profile.Instance.MsgPrint("You grin and bare your fangs...");
                        int dummy = plev + (Program.Rng.DieRoll(plev) * Math.Max(1, plev / 10));
                        if (_saveGame.SpellEffects.DrainLife(dir, dummy))
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

                case RaceId.Spectre:
                    if (RacialAux(4, 6, Ability.Intelligence, 3))
                    {
                        Profile.Instance.MsgPrint("You emit an eldritch howl!");
                        if (!targetEngine.GetAimDir(out dir))
                        {
                            break;
                        }
                        _saveGame.SpellEffects.FearMonster(dir, plev);
                    }
                    break;

                case RaceId.Sprite:
                    if (RacialAux(12, 12, Ability.Intelligence, 15))
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

                default:
                    Profile.Instance.MsgPrint("This race has no bonus power.");
                    _saveGame.EnergyUse = 0;
                    break;
            }
            Player.RedrawFlags |= RedrawFlag.PrHp | RedrawFlag.PrMana;
        }

        private bool CheckHit(int power)
        {
            int k = Program.Rng.RandomLessThan(100);
            if (k < 10)
            {
                return k < 5;
            }
            if (power <= 0)
            {
                return false;
            }
            int ac = Player.BaseArmourClass + Player.ArmourClassBonus;
            return Program.Rng.DieRoll(power) > ac * 3 / 4;
        }

        private int CriticalNorm(int weight, int plus, int dam)
        {
            int i = weight + ((Player.AttackBonus + plus) * 5) + (Player.Level * 3);
            if (Program.Rng.DieRoll(5000) <= i)
            {
                int k = weight + Program.Rng.DieRoll(650);
                if (k < 400)
                {
                    Profile.Instance.MsgPrint("It was a good hit!");
                    dam = (2 * dam) + 5;
                }
                else if (k < 700)
                {
                    Profile.Instance.MsgPrint("It was a great hit!");
                    dam = (2 * dam) + 10;
                }
                else if (k < 900)
                {
                    Profile.Instance.MsgPrint("It was a superb hit!");
                    dam = (3 * dam) + 15;
                }
                else if (k < 1300)
                {
                    Profile.Instance.MsgPrint("It was a *GREAT* hit!");
                    dam = (3 * dam) + 20;
                }
                else
                {
                    Profile.Instance.MsgPrint("It was a *SUPERB* hit!");
                    dam = (7 * dam / 2) + 25;
                }
            }
            return dam;
        }

        private bool EasyOpenDoor(int y, int x)
        {
            GridTile cPtr = Level.Grid[y][x];
            if (!cPtr.FeatureType.IsClosedDoor)
            {
                return false;
            }
            if (cPtr.FeatureType.Name.Contains("Jammed"))
            {
                Profile.Instance.MsgPrint("The door appears to be stuck.");
            }
            else if (!cPtr.FeatureType.Name.Contains("0"))
            {
                int i = Player.SkillDisarmTraps;
                if (Player.TimedBlindness != 0 || Level.NoLight())
                {
                    i /= 10;
                }
                if (Player.TimedConfusion != 0 || Player.TimedHallucinations != 0)
                {
                    i /= 10;
                }
                int j = int.Parse(cPtr.FeatureType.Name.Substring(10));
                j = i - (j * 4);
                if (j < 2)
                {
                    j = 2;
                }
                if (Program.Rng.RandomLessThan(100) < j)
                {
                    Profile.Instance.MsgPrint("You have picked the lock.");
                    Level.CaveSetFeat(y, x, "OpenDoor");
                    Player.UpdateFlags |= Constants.PuView | Constants.PuLight | Constants.PuMonsters;
                    Gui.PlaySound(SoundEffect.LockpickSuccess);
                    Player.GainExperience(1);
                }
                else
                {
                    Profile.Instance.MsgPrint("You failed to pick the lock.");
                }
            }
            else
            {
                Level.CaveSetFeat(y, x, "OpenDoor");
                Player.UpdateFlags |= Constants.PuView | Constants.PuLight | Constants.PuMonsters;
                Gui.PlaySound(SoundEffect.OpenDoor);
            }
            return true;
        }

        private void NaturalAttack(int mIdx, Mutation attack, out bool fear, out bool mdeath)
        {
            fear = false;
            mdeath = false;
            Monster mPtr = Level.Monsters[mIdx];
            MonsterRace rPtr = mPtr.Race;
            int dss = attack.Dss;
            int ddd = attack.Ddd;
            int nWeight = attack.NWeight;
            string atkDesc = attack.AtkDesc;
            string mName = mPtr.MonsterDesc(0);
            int bonus = Player.AttackBonus;
            int chance = Player.SkillMelee + (bonus * Constants.BthPlusAdj);
            if (TestHitNorm(chance, rPtr.ArmourClass, mPtr.IsVisible))
            {
                Gui.PlaySound(SoundEffect.MeleeHit);
                Profile.Instance.MsgPrint($"You hit {mName} with your {atkDesc}.");
                int k = Program.Rng.DiceRoll(ddd, dss);
                k = CriticalNorm(nWeight, Player.AttackBonus, k);
                k += Player.DamageBonus;
                if (k < 0)
                {
                    k = 0;
                }
                if ((mPtr.Mind & Constants.SmFriendly) != 0)
                {
                    Profile.Instance.MsgPrint($"{mName} gets angry!");
                    mPtr.Mind &= ~Constants.SmFriendly;
                }
                switch (attack.MutationAttackType)
                {
                    case MutationAttackType.Physical:
                        mdeath = Level.Monsters.DamageMonster(mIdx, k, out fear, null);
                        break;

                    case MutationAttackType.Poison:
                        _saveGame.SpellEffects.Project(0, 0, mPtr.MapY, mPtr.MapX, k,
                            new ProjectPois(SaveGame.Instance.SpellEffects), ProjectionFlag.ProjectKill);
                        break;

                    case MutationAttackType.Hellfire:
                        _saveGame.SpellEffects.Project(0, 0, mPtr.MapY, mPtr.MapX, k,
                            new ProjectHellFire(SaveGame.Instance.SpellEffects), ProjectionFlag.ProjectKill);
                        break;
                }
                TouchZapPlayer(mPtr);
            }
            else
            {
                Gui.PlaySound(SoundEffect.Miss);
                Profile.Instance.MsgPrint($"You miss {mName}.");
            }
        }

        private void RunInit(int dir)
        {
            _navigationState = new NavigationState();
            _navigationState._findCurrent = dir;
            _navigationState._findPrevdir = dir;
            _navigationState._findOpenarea = true;
            _navigationState._findBreakright = false;
            _navigationState._findBreakleft = false;
            bool deepleft = false;
            bool deepright = false;
            bool shortright = false;
            bool shortleft = false;
            int row = Player.MapY + Level.KeypadDirectionYOffset[dir];
            int col = Player.MapX + Level.KeypadDirectionXOffset[dir];
            int i = _navigationState.Chome[dir];
            if (SeeWall(_navigationState.Cycle[i + 1], Player.MapY, Player.MapX))
            {
                _navigationState._findBreakleft = true;
                shortleft = true;
            }
            else if (SeeWall(_navigationState.Cycle[i + 1], row, col))
            {
                _navigationState._findBreakleft = true;
                deepleft = true;
            }
            if (SeeWall(_navigationState.Cycle[i - 1], Player.MapY, Player.MapX))
            {
                _navigationState._findBreakright = true;
                shortright = true;
            }
            else if (SeeWall(_navigationState.Cycle[i - 1], row, col))
            {
                _navigationState._findBreakright = true;
                deepright = true;
            }
            if (_navigationState._findBreakleft && _navigationState._findBreakright)
            {
                _navigationState._findOpenarea = false;
                if ((dir & 0x01) != 0)
                {
                    if (deepleft && !deepright)
                    {
                        _navigationState._findPrevdir = _navigationState.Cycle[i - 1];
                    }
                    else if (deepright && !deepleft)
                    {
                        _navigationState._findPrevdir = _navigationState.Cycle[i + 1];
                    }
                }
                else if (SeeWall(_navigationState.Cycle[i], row, col))
                {
                    if (shortleft && !shortright)
                    {
                        _navigationState._findPrevdir = _navigationState.Cycle[i - 2];
                    }
                    else if (shortright && !shortleft)
                    {
                        _navigationState._findPrevdir = _navigationState.Cycle[i + 2];
                    }
                }
            }
        }

        private bool RunTest()
        {
            int newDir, checkDir = 0;
            int row, col;
            int i;
            GridTile cPtr;
            int option = 0;
            int option2 = 0;
            int prevDir = _navigationState._findPrevdir;
            int max = (prevDir & 0x01) + 1;
            for (i = -max; i <= max; i++)
            {
                int nextOIdx;
                newDir = _navigationState.Cycle[_navigationState.Chome[prevDir] + i];
                row = Player.MapY + Level.KeypadDirectionYOffset[newDir];
                col = Player.MapX + Level.KeypadDirectionXOffset[newDir];
                cPtr = Level.Grid[row][col];
                if (cPtr.Monster != 0)
                {
                    Monster mPtr = Level.Monsters[cPtr.Monster];
                    if (mPtr.IsVisible)
                    {
                        return true;
                    }
                }
                for (int thisOIdx = cPtr.Item; thisOIdx != 0; thisOIdx = nextOIdx)
                {
                    Item oPtr = Level.Items[thisOIdx];
                    nextOIdx = oPtr.NextInStack;
                    if (oPtr.Marked)
                    {
                        return true;
                    }
                }
                bool inv = true;
                if (cPtr.TileFlags.IsSet(GridTile.PlayerMemorised))
                {
                    bool notice = !cPtr.FeatureType.RunPast;
                    if (notice)
                    {
                        return true;
                    }
                    inv = false;
                }
                if (inv || Level.GridPassable(row, col))
                {
                    if (_navigationState._findOpenarea)
                    {
                    }
                    else if (option == 0)
                    {
                        option = newDir;
                    }
                    else if (option2 != 0)
                    {
                        return true;
                    }
                    else if (option != _navigationState.Cycle[_navigationState.Chome[prevDir] + i - 1])
                    {
                        return true;
                    }
                    else if ((newDir & 0x01) != 0)
                    {
                        checkDir = _navigationState.Cycle[_navigationState.Chome[prevDir] + i - 2];
                        option2 = newDir;
                    }
                    else
                    {
                        checkDir = _navigationState.Cycle[_navigationState.Chome[prevDir] + i + 1];
                        option2 = option;
                        option = newDir;
                    }
                }
                else
                {
                    if (_navigationState._findOpenarea)
                    {
                        if (i < 0)
                        {
                            _navigationState._findBreakright = true;
                        }
                        else if (i > 0)
                        {
                            _navigationState._findBreakleft = true;
                        }
                    }
                }
            }
            if (_navigationState._findOpenarea)
            {
                for (i = -max; i < 0; i++)
                {
                    newDir = _navigationState.Cycle[_navigationState.Chome[prevDir] + i];
                    row = Player.MapY + Level.KeypadDirectionYOffset[newDir];
                    col = Player.MapX + Level.KeypadDirectionXOffset[newDir];
                    cPtr = Level.Grid[row][col];
                    if (cPtr.TileFlags.IsClear(GridTile.PlayerMemorised) || !cPtr.FeatureType.IsWall)
                    {
                        if (_navigationState._findBreakright)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (_navigationState._findBreakleft)
                        {
                            return true;
                        }
                    }
                }
                for (i = max; i > 0; i--)
                {
                    newDir = _navigationState.Cycle[_navigationState.Chome[prevDir] + i];
                    row = Player.MapY + Level.KeypadDirectionYOffset[newDir];
                    col = Player.MapX + Level.KeypadDirectionXOffset[newDir];
                    cPtr = Level.Grid[row][col];
                    if (cPtr.TileFlags.IsClear(GridTile.PlayerMemorised) || !cPtr.FeatureType.IsWall)
                    {
                        if (_navigationState._findBreakleft)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (_navigationState._findBreakright)
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                if (option == 0)
                {
                    return true;
                }
                if (option2 == 0)
                {
                    _navigationState._findCurrent = option;
                    _navigationState._findPrevdir = option;
                }
                else
                {
                    row = Player.MapY + Level.KeypadDirectionYOffset[option];
                    col = Player.MapX + Level.KeypadDirectionXOffset[option];
                    if (!SeeWall(option, row, col) || !SeeWall(checkDir, row, col))
                    {
                        if (SeeNothing(option, row, col) && SeeNothing(option2, row, col))
                        {
                            _navigationState._findCurrent = option;
                            _navigationState._findPrevdir = option2;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        _navigationState._findCurrent = option2;
                        _navigationState._findPrevdir = option2;
                    }
                }
            }
            return SeeWall(_navigationState._findCurrent, Player.MapY, Player.MapX);
        }

        private bool SeeNothing(int dir, int y, int x)
        {
            y += Level.KeypadDirectionYOffset[dir];
            x += Level.KeypadDirectionXOffset[dir];
            if (!Level.InBounds2(y, x))
            {
                return true;
            }
            if (Level.Grid[y][x].TileFlags.IsSet(GridTile.PlayerMemorised))
            {
                return false;
            }
            if (!Level.GridPassable(y, x))
            {
                return true;
            }
            return !Level.PlayerCanSeeBold(y, x);
        }

        private bool SeeWall(int dir, int y, int x)
        {
            y += Level.KeypadDirectionYOffset[dir];
            x += Level.KeypadDirectionXOffset[dir];
            if (!Level.InBounds2(y, x))
            {
                return false;
            }
            if (Level.GridPassable(y, x))
            {
                return false;
            }
            if (Level.Grid[y][x].TileFlags.IsClear(GridTile.PlayerMemorised))
            {
                return false;
            }
            return true;
        }

        private void StepOnTrap()
        {
            int dam;
            string name = "a trap";
            _saveGame.Disturb(false);
            GridTile cPtr = Level.Grid[Player.MapY][Player.MapX];
            switch (cPtr.FeatureType.Name)
            {
                case "TrapDoor":
                    {
                        if (Player.HasFeatherFall)
                        {
                            Profile.Instance.MsgPrint("You fly over a trap door.");
                        }
                        else
                        {
                            Profile.Instance.MsgPrint("You fell through a trap door!");
                            dam = Program.Rng.DiceRoll(2, 8);
                            name = "a trap door";
                            Player.TakeHit(dam, name);
                            if (Player.Health >= 0)
                            {
                                _saveGame.IsAutosave = true;
                                _saveGame.DoCmdSaveGame();
                                _saveGame.IsAutosave = false;
                            }
                            _saveGame.NewLevelFlag = true;
                            if (_saveGame.CurDungeon.Tower)
                            {
                                _saveGame.DunLevel--;
                            }
                            else
                            {
                                _saveGame.DunLevel++;
                            }
                        }
                        break;
                    }
                case "Pit":
                    {
                        if (Player.HasFeatherFall)
                        {
                            Profile.Instance.MsgPrint("You fly over a pit trap.");
                        }
                        else
                        {
                            Profile.Instance.MsgPrint("You fell into a pit!");
                            dam = Program.Rng.DiceRoll(2, 6);
                            name = "a pit trap";
                            Player.TakeHit(dam, name);
                        }
                        break;
                    }
                case "SpikedPit":
                    {
                        if (Player.HasFeatherFall)
                        {
                            Profile.Instance.MsgPrint("You fly over a spiked pit.");
                        }
                        else
                        {
                            Profile.Instance.MsgPrint("You fall into a spiked pit!");
                            name = "a pit trap";
                            dam = Program.Rng.DiceRoll(2, 6);
                            if (Program.Rng.RandomLessThan(100) < 50)
                            {
                                Profile.Instance.MsgPrint("You are impaled!");
                                name = "a spiked pit";
                                dam *= 2;
                                Player.SetTimedBleeding(Player.TimedBleeding + Program.Rng.DieRoll(dam));
                            }
                            Player.TakeHit(dam, name);
                        }
                        break;
                    }
                case "PoisonPit":
                    {
                        if (Player.HasFeatherFall)
                        {
                            Profile.Instance.MsgPrint("You fly over a spiked pit.");
                        }
                        else
                        {
                            Profile.Instance.MsgPrint("You fall into a spiked pit!");
                            dam = Program.Rng.DiceRoll(2, 6);
                            name = "a pit trap";
                            if (Program.Rng.RandomLessThan(100) < 50)
                            {
                                Profile.Instance.MsgPrint("You are impaled on poisonous spikes!");
                                name = "a spiked pit";
                                dam *= 2;
                                Player.SetTimedBleeding(Player.TimedBleeding + Program.Rng.DieRoll(dam));
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
                                    dam *= 2;
                                    Player.SetTimedPoison(Player.TimedPoison + Program.Rng.DieRoll(dam));
                                }
                            }
                            Player.TakeHit(dam, name);
                        }
                        break;
                    }
                case "SummonRune":
                    {
                        Profile.Instance.MsgPrint("There is a flash of shimmering light!");
                        cPtr.TileFlags.Clear(GridTile.PlayerMemorised);
                        Level.CaveRemoveFeat(Player.MapY, Player.MapX);
                        int num = 2 + Program.Rng.DieRoll(3);
                        for (int i = 0; i < num; i++)
                        {
                            Level.Monsters.SummonSpecific(Player.MapY, Player.MapX, _saveGame.Difficulty, 0);
                        }
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
                        Profile.Instance.MsgPrint("You hit a teleport trap!");
                        _saveGame.SpellEffects.TeleportPlayer(100);
                        break;
                    }
                case "FireTrap":
                    {
                        Profile.Instance.MsgPrint("You are enveloped in flames!");
                        dam = Program.Rng.DiceRoll(4, 6);
                        _saveGame.SpellEffects.FireDam(dam, "a fire trap");
                        break;
                    }
                case "AcidTrap":
                    {
                        Profile.Instance.MsgPrint("You are splashed with acid!");
                        dam = Program.Rng.DiceRoll(4, 6);
                        _saveGame.SpellEffects.AcidDam(dam, "an acid trap");
                        break;
                    }
                case "SlowDart":
                    {
                        if (CheckHit(125))
                        {
                            Profile.Instance.MsgPrint("A small dart hits you!");
                            dam = Program.Rng.DiceRoll(1, 4);
                            Player.TakeHit(dam, name);
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
                        if (CheckHit(125))
                        {
                            Profile.Instance.MsgPrint("A small dart hits you!");
                            dam = Program.Rng.DiceRoll(1, 4);
                            Player.TakeHit(dam, "a dart trap");
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
                        if (CheckHit(125))
                        {
                            Profile.Instance.MsgPrint("A small dart hits you!");
                            dam = Program.Rng.DiceRoll(1, 4);
                            Player.TakeHit(dam, "a dart trap");
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
                        if (CheckHit(125))
                        {
                            Profile.Instance.MsgPrint("A small dart hits you!");
                            dam = Program.Rng.DiceRoll(1, 4);
                            Player.TakeHit(dam, "a dart trap");
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
                        Profile.Instance.MsgPrint("A black gas surrounds you!");
                        if (!Player.HasBlindnessResistance)
                        {
                            Player.SetTimedBlindness(Player.TimedBlindness + Program.Rng.RandomLessThan(50) + 25);
                        }
                        break;
                    }
                case "ConfuseGas":
                    {
                        Profile.Instance.MsgPrint("A gas of scintillating colours surrounds you!");
                        if (!Player.HasConfusionResistance)
                        {
                            Player.SetTimedConfusion(Player.TimedConfusion + Program.Rng.RandomLessThan(20) + 10);
                        }
                        break;
                    }
                case "PoisonGas":
                    {
                        Profile.Instance.MsgPrint("A pungent green gas surrounds you!");
                        if (!Player.HasPoisonResistance && Player.TimedPoisonResistance == 0)
                        {
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
                        Profile.Instance.MsgPrint("A strange white mist surrounds you!");
                        if (!Player.HasFreeAction)
                        {
                            Player.SetTimedParalysis(Player.TimedParalysis + Program.Rng.RandomLessThan(10) + 5);
                        }
                        break;
                    }
            }
        }

        private bool TestHitNorm(int chance, int ac, bool vis)
        {
            int k = Program.Rng.RandomLessThan(100);
            if (k < 10)
            {
                return k < 5;
            }
            if (chance <= 0)
            {
                return false;
            }
            if (!vis)
            {
                chance = (chance + 1) / 2;
            }
            return Program.Rng.RandomLessThan(chance) >= ac * 3 / 4;
        }

        private void TouchZapPlayer(Monster mPtr)
        {
            int auraDamage;
            MonsterRace rPtr = mPtr.Race;
            if ((rPtr.Flags2 & MonsterFlag2.FireAura) != 0)
            {
                if (!Player.HasFireImmunity)
                {
                    auraDamage = Program.Rng.DiceRoll(1 + (rPtr.Level / 26), 1 + (rPtr.Level / 17));
                    string auraDam = mPtr.MonsterDesc(0x88);
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
                    rPtr.Knowledge.RFlags2 |= MonsterFlag2.FireAura;
                    _saveGame.HandleStuff();
                }
            }
            if ((rPtr.Flags2 & MonsterFlag2.LightningAura) != 0 && !Player.HasLightningImmunity)
            {
                auraDamage = Program.Rng.DiceRoll(1 + (rPtr.Level / 26), 1 + (rPtr.Level / 17));
                string auraDam = mPtr.MonsterDesc(0x88);
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
                rPtr.Knowledge.RFlags2 |= MonsterFlag2.LightningAura;
                _saveGame.HandleStuff();
            }
        }

        private bool Twall(int y, int x)
        {
            GridTile cPtr = Level.Grid[y][x];
            if (Level.GridPassable(y, x))
            {
                return false;
            }
            cPtr.TileFlags.Clear(GridTile.PlayerMemorised);
            Level.CaveRemoveFeat(y, x);
            Player.UpdateFlags |= Constants.PuView | Constants.PuLight | Constants.PuFlow | Constants.PuMonsters;
            return true;
        }
    }
}