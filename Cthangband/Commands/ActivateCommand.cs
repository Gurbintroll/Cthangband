using Cthangband.Enumerations;
using Cthangband.Projection;
using Cthangband.StaticData;
using Cthangband.UI;
using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Activate an artifact or similar
    /// </summary>
    /// <param name="itemIndex">
    /// The inventory index of the item to be activated, or -999 to select item
    /// </param>
    [Serializable]
    internal class ActivateCommand : ICommand
    {
        public char Key => 'A';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
            int itemIndex = -999;
            int dir;
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (itemIndex == -999)
            {
                // No item passed in, so get one; filtering to activatable items only
                SaveGame.Instance.ItemFilter = SaveGame.Instance.CommandEngine.ItemFilterActivatable;
                if (!SaveGame.Instance.GetItem(out itemIndex, "Activate which item? ", true, false, false))
                {
                    if (itemIndex == -2)
                    {
                        Profile.Instance.MsgPrint("You have nothing to activate.");
                    }
                    return;
                }
            }
            // Get the item from the index
            Item item = itemIndex >= 0 ? player.Inventory[itemIndex] : level.Items[0 - itemIndex];
            // Check if the item is activatable
            SaveGame.Instance.ItemFilter = SaveGame.Instance.CommandEngine.ItemFilterActivatable;
            if (!player.Inventory.ItemMatchesFilter(item))
            {
                Profile.Instance.MsgPrint("You can't activate that!");
                SaveGame.Instance.ItemFilter = null;
                return;
            }
            SaveGame.Instance.ItemFilter = null;
            // Activating an item uses 100 energy
            SaveGame.Instance.EnergyUse = 100;
            // Get the level of the item
            int itemLevel = item.ItemType.Level;
            if (item.IsFixedArtifact())
            {
                itemLevel = Profile.Instance.FixedArtifacts[item.FixedArtifactIndex].Level;
            }
            // Work out the chance of using the item successfully based on its level and the
            // player's skill
            int chance = player.SkillUseDevice;
            if (player.TimedConfusion != 0)
            {
                chance /= 2;
            }
            chance -= itemLevel > 50 ? 50 : itemLevel;
            // Always give a slight chance of success
            if (chance < Constants.UseDevice && Program.Rng.RandomLessThan(Constants.UseDevice - chance + 1) == 0)
            {
                chance = Constants.UseDevice;
            }
            // If we fail our use item roll just tell us and quit
            if (chance < Constants.UseDevice || Program.Rng.DieRoll(chance) < Constants.UseDevice)
            {
                Profile.Instance.MsgPrint("You failed to activate it properly.");
                return;
            }
            // If the item is still recharging, then just tell us and quit
            if (item.RechargeTimeLeft != 0)
            {
                Profile.Instance.MsgPrint("It whines, glows and fades...");
                return;
            }
            // We passed the checks, so the item is activated
            Profile.Instance.MsgPrint("You activate it...");
            Gui.PlaySound(SoundEffect.ActivateArtifact);
            // If it is a random artifact then use its ability and quit
            if (string.IsNullOrEmpty(item.RandartName) == false)
            {
                SaveGame.Instance.CommandEngine.ActivateRandomArtifact(item);
                return;
            }
            // If it's a fixed artifact then use its ability
            if (item.FixedArtifactIndex != 0)
            {
                switch (item.FixedArtifactIndex)
                {
                    // Star Essence of Polaris lights the area
                    case FixedArtifactId.StarEssenceOfPolaris:
                        {
                            Profile.Instance.MsgPrint("The essence wells with clear light...");
                            SaveGame.Instance.SpellEffects.LightArea(Program.Rng.DiceRoll(2, 15), 3);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(10) + 10;
                            break;
                        }
                    // Star essence of Xoth lights and maps the area
                    case FixedArtifactId.StarEssenceOfXoth:
                        {
                            Profile.Instance.MsgPrint("The essence shines brightly...");
                            level.MapArea();
                            SaveGame.Instance.SpellEffects.LightArea(Program.Rng.DiceRoll(2, 15), 3);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(50) + 50;
                            break;
                        }
                    // Shining Trapezohedron lights the entire level and recalls us, but drains
                    // health to do so
                    case FixedArtifactId.ShiningTrapezohedron:
                        {
                            Profile.Instance.MsgPrint("The gemstone flashes bright red!");
                            level.WizLight();
                            Profile.Instance.MsgPrint("The gemstone drains your vitality...");
                            player.TakeHit(Program.Rng.DiceRoll(3, 8), "the Gemstone 'Trapezohedron'");
                            SaveGame.Instance.SpellEffects.DetectTraps();
                            SaveGame.Instance.SpellEffects.DetectDoors();
                            SaveGame.Instance.SpellEffects.DetectStairs();
                            if (Gui.GetCheck("Activate recall? "))
                            {
                                player.ToggleRecall();
                            }
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(20) + 20;
                            break;
                        }
                    // Amulet of Lobon protects us from evil
                    case FixedArtifactId.AmuletOfLobon:
                        {
                            Profile.Instance.MsgPrint("The amulet lets out a shrill wail...");
                            int k = 3 * player.Level;
                            player.SetTimedProtectionFromEvil(player.TimedProtectionFromEvil + Program.Rng.DieRoll(25) + k);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(225) + 225;
                            break;
                        }
                    // Amulet of Abdul Alhazred dispels evil
                    case FixedArtifactId.AmuletOfAbdulAlhazred:
                        {
                            Profile.Instance.MsgPrint("The amulet floods the area with goodness...");
                            SaveGame.Instance.SpellEffects.DispelEvil(player.Level * 5);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(300) + 300;
                            break;
                        }
                    // Ring of Magic has a djinn in it that drains life from an opponent
                    case FixedArtifactId.RingOfMagic:
                        {
                            Profile.Instance.MsgPrint("You order Frakir to strangle your opponent.");
                            if (!targetEngine.GetDirectionWithAim(out dir))
                            {
                                return;
                            }
                            if (SaveGame.Instance.SpellEffects.DrainLife(dir, 100))
                            {
                                item.RechargeTimeLeft = Program.Rng.RandomLessThan(100) + 100;
                            }
                            break;
                        }
                    // Ring of Bast hastes you
                    case FixedArtifactId.RingOfBast:
                        {
                            Profile.Instance.MsgPrint("The ring glows brightly...");
                            if (player.TimedHaste == 0)
                            {
                                player.SetTimedHaste(Program.Rng.DieRoll(75) + 75);
                            }
                            else
                            {
                                player.SetTimedHaste(player.TimedHaste + 5);
                            }
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(150) + 150;
                            break;
                        }
                    // Ring of Elemental Fire casts a fireball
                    case FixedArtifactId.RingOfElementalPowerFire:
                        {
                            Profile.Instance.MsgPrint("The ring glows deep red...");
                            if (!targetEngine.GetDirectionWithAim(out dir))
                            {
                                return;
                            }
                            SaveGame.Instance.SpellEffects.FireBall(new ProjectFire(SaveGame.Instance.SpellEffects), dir, 120, 3);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(225) + 225;
                            break;
                        }
                    // Ring of Elemental Ice casts a coldball
                    case FixedArtifactId.RingOfElementalPowerIce:
                        {
                            Profile.Instance.MsgPrint("The ring glows bright white...");
                            if (!targetEngine.GetDirectionWithAim(out dir))
                            {
                                return;
                            }
                            SaveGame.Instance.SpellEffects.FireBall(new ProjectCold(SaveGame.Instance.SpellEffects), dir, 200, 3);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(325) + 325;
                            break;
                        }
                    // Ring of Elemental Lightning casts a lightning ball
                    case FixedArtifactId.RingOfElementalPowerStorm:
                        {
                            Profile.Instance.MsgPrint("The ring glows deep blue...");
                            if (!targetEngine.GetDirectionWithAim(out dir))
                            {
                                return;
                            }
                            SaveGame.Instance.SpellEffects.FireBall(new ProjectElec(SaveGame.Instance.SpellEffects), dir, 250, 3);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(425) + 425;
                            break;
                        }
                    // Ring of Set has a random effect
                    case FixedArtifactId.RingOfSet:
                        {
                            Profile.Instance.MsgPrint("The ring glows intensely black...");
                            if (!targetEngine.GetDirectionWithAim(out dir))
                            {
                                return;
                            }
                            SaveGame.Instance.CommandEngine.RingOfSetPower(dir);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(450) + 450;
                            break;
                        }
                    // Razorback gives you a point-blank lightning ball
                    case FixedArtifactId.DragonScaleRazorback:
                        {
                            Profile.Instance.MsgPrint("Your armor is surrounded by lightning...");
                            for (int i = 0; i < 8; i++)
                            {
                                SaveGame.Instance.SpellEffects.FireBall(new ProjectElec(SaveGame.Instance.SpellEffects), level.OrderedDirection[i], 150, 3);
                            }
                            item.RechargeTimeLeft = 1000;
                            break;
                        }
                    // Bladeturner heals you and gives you timed resistances
                    case FixedArtifactId.DragonScaleBladeturner:
                        {
                            if (!targetEngine.GetDirectionWithAim(out dir))
                            {
                                return;
                            }
                            Profile.Instance.MsgPrint("You breathe the elements.");
                            SaveGame.Instance.SpellEffects.FireBall(new ProjectMissile(SaveGame.Instance.SpellEffects), dir, 300, 4);
                            Profile.Instance.MsgPrint("Your armor glows many colors...");
                            player.SetTimedFear(0);
                            player.SetTimedSuperheroism(player.TimedSuperheroism + Program.Rng.DieRoll(50) + 50);
                            player.RestoreHealth(30);
                            player.SetTimedBlessing(player.TimedBlessing + Program.Rng.DieRoll(50) + 50);
                            player.SetTimedAcidResistance(player.TimedAcidResistance + Program.Rng.DieRoll(50) + 50);
                            player.SetTimedLightningResistance(player.TimedLightningResistance + Program.Rng.DieRoll(50) + 50);
                            player.SetTimedFireResistance(player.TimedFireResistance + Program.Rng.DieRoll(50) + 50);
                            player.SetTimedColdResistance(player.TimedColdResistance + Program.Rng.DieRoll(50) + 50);
                            player.SetTimedPoisonResistance(player.TimedPoisonResistance + Program.Rng.DieRoll(50) + 50);
                            item.RechargeTimeLeft = 400;
                            break;
                        }
                    // Soulkeeper heals you a lot
                    case FixedArtifactId.PlateMailSoulkeeper:
                        {
                            Profile.Instance.MsgPrint("Your armor glows a bright white...");
                            Profile.Instance.MsgPrint("You feel much better...");
                            player.RestoreHealth(1000);
                            player.SetTimedBleeding(0);
                            item.RechargeTimeLeft = 888;
                            break;
                        }
                    // Vampire Hunter cures most ailments
                    case FixedArtifactId.ArmourOfTheVampireHunter:
                        {
                            Profile.Instance.MsgPrint("A heavenly choir sings...");
                            player.SetTimedPoison(0);
                            player.SetTimedBleeding(0);
                            player.SetTimedStun(0);
                            player.SetTimedConfusion(0);
                            player.SetTimedBlindness(0);
                            player.SetTimedHeroism(player.TimedHeroism + Program.Rng.DieRoll(25) + 25);
                            player.RestoreHealth(777);
                            item.RechargeTimeLeft = 300;
                            break;
                        }
                    // Orc does Carnage
                    case FixedArtifactId.ArmourOfTheOrcs:
                        {
                            Profile.Instance.MsgPrint("Your armor glows deep blue...");
                            SaveGame.Instance.SpellEffects.Carnage(true);
                            item.RechargeTimeLeft = 500;
                            break;
                        }
                    // Ogre Lords destroys doors
                    case FixedArtifactId.ArmourOfTheOgreLords:
                        {
                            Profile.Instance.MsgPrint("Your armor glows bright red...");
                            SaveGame.Instance.SpellEffects.DestroyDoorsTouch();
                            item.RechargeTimeLeft = 10;
                            break;
                        }
                    // Dragon Helm and Terror Mask cause fear
                    case FixedArtifactId.DragonHelmOfPower:
                    case FixedArtifactId.HelmTerrorMask:
                        {
                            SaveGame.Instance.SpellEffects.TurnMonsters(40 + player.Level);
                            item.RechargeTimeLeft = 3 * (player.Level + 10);
                            break;
                        }
                    // Skull Keeper detects everything
                    case FixedArtifactId.HelmSkullkeeper:
                        {
                            Profile.Instance.MsgPrint("Your helm glows bright white...");
                            Profile.Instance.MsgPrint("An image forms in your mind...");
                            SaveGame.Instance.SpellEffects.DetectAll();
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(55) + 55;
                            break;
                        }
                    // Sun Crown heals
                    case FixedArtifactId.CrownOfTheSun:
                        {
                            Profile.Instance.MsgPrint("Your crown glows deep yellow...");
                            Profile.Instance.MsgPrint("You feel a warm tingling inside...");
                            player.RestoreHealth(700);
                            player.SetTimedBleeding(0);
                            item.RechargeTimeLeft = 250;
                            break;
                        }
                    // Cloak of Barzai gives resistances
                    case FixedArtifactId.CloakOfBarzai:
                        {
                            Profile.Instance.MsgPrint("Your cloak glows many colours...");
                            player.SetTimedAcidResistance(player.TimedAcidResistance + Program.Rng.DieRoll(20) + 20);
                            player.SetTimedLightningResistance(player.TimedLightningResistance + Program.Rng.DieRoll(20) + 20);
                            player.SetTimedFireResistance(player.TimedFireResistance + Program.Rng.DieRoll(20) + 20);
                            player.SetTimedColdResistance(player.TimedColdResistance + Program.Rng.DieRoll(20) + 20);
                            player.SetTimedPoisonResistance(player.TimedPoisonResistance + Program.Rng.DieRoll(20) + 20);
                            item.RechargeTimeLeft = 111;
                            break;
                        }
                    // Darkness sends monsters to sleep
                    case FixedArtifactId.CloakDarkness:
                        {
                            Profile.Instance.MsgPrint("Your cloak glows deep blue...");
                            SaveGame.Instance.SpellEffects.SleepMonstersTouch();
                            item.RechargeTimeLeft = 55;
                            break;
                        }
                    // Swashbuckler recharges items
                    case FixedArtifactId.CloakOfTheSwashbuckler:
                        {
                            Profile.Instance.MsgPrint("Your cloak glows bright yellow...");
                            SaveGame.Instance.SpellEffects.Recharge(60);
                            item.RechargeTimeLeft = 70;
                            break;
                        }
                    // Shifter teleports you
                    case FixedArtifactId.CloakShifter:
                        {
                            Profile.Instance.MsgPrint("Your cloak twists space around you...");
                            SaveGame.Instance.SpellEffects.TeleportPlayer(100);
                            item.RechargeTimeLeft = 45;
                            break;
                        }
                    // Nyogtha restores experience
                    case FixedArtifactId.ShadowCloakOfNyogtha:
                        {
                            Profile.Instance.MsgPrint("Your cloak glows a deep red...");
                            player.RestoreLevel();
                            item.RechargeTimeLeft = 450;
                            break;
                        }
                    // Light shoots magic missiles
                    case FixedArtifactId.GlovesOfLight:
                        {
                            Profile.Instance.MsgPrint("Your gloves glow extremely brightly...");
                            if (!targetEngine.GetDirectionWithAim(out dir))
                            {
                                return;
                            }
                            SaveGame.Instance.SpellEffects.FireBolt(new ProjectMissile(SaveGame.Instance.SpellEffects), dir,
                                Program.Rng.DiceRoll(2, 6));
                            item.RechargeTimeLeft = 2;
                            break;
                        }
                    // Iron Fist shoots fire bolts
                    case FixedArtifactId.GauntletIronfist:
                        {
                            Profile.Instance.MsgPrint("Your gauntlets are covered in fire...");
                            if (!targetEngine.GetDirectionWithAim(out dir))
                            {
                                return;
                            }
                            SaveGame.Instance.SpellEffects.FireBolt(new ProjectFire(SaveGame.Instance.SpellEffects), dir, Program.Rng.DiceRoll(9, 8));
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(8) + 8;
                            break;
                        }
                    // Ghouls shoot cold bolts
                    case FixedArtifactId.GauntletsOfGhouls:
                        {
                            Profile.Instance.MsgPrint("Your gauntlets are covered in frost...");
                            if (!targetEngine.GetDirectionWithAim(out dir))
                            {
                                return;
                            }
                            SaveGame.Instance.SpellEffects.FireBolt(new ProjectCold(SaveGame.Instance.SpellEffects), dir, Program.Rng.DiceRoll(6, 8));
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(7) + 7;
                            break;
                        }
                    // White Spark shoot lightning bolts
                    case FixedArtifactId.GauntletsWhiteSpark:
                        {
                            Profile.Instance.MsgPrint("Your gauntlets are covered in sparks...");
                            if (!targetEngine.GetDirectionWithAim(out dir))
                            {
                                return;
                            }
                            SaveGame.Instance.SpellEffects.FireBolt(new ProjectElec(SaveGame.Instance.SpellEffects), dir, Program.Rng.DiceRoll(4, 8));
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(6) + 6;
                            break;
                        }
                    // The Dead shoot acid bolts
                    case FixedArtifactId.GauntletsOfTheDead:
                        {
                            Profile.Instance.MsgPrint("Your gauntlets are covered in acid...");
                            if (!targetEngine.GetDirectionWithAim(out dir))
                            {
                                return;
                            }
                            SaveGame.Instance.SpellEffects.FireBolt(new ProjectAcid(SaveGame.Instance.SpellEffects), dir, Program.Rng.DiceRoll(5, 8));
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(5) + 5;
                            break;
                        }
                    // Cesti shoot arrows
                    case FixedArtifactId.CestiOfCombat:
                        {
                            Profile.Instance.MsgPrint("Your cesti grows magical spikes...");
                            if (!targetEngine.GetDirectionWithAim(out dir))
                            {
                                return;
                            }
                            SaveGame.Instance.SpellEffects.FireBolt(new ProjectArrow(SaveGame.Instance.SpellEffects), dir, 150);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(90) + 90;
                            break;
                        }
                    // Boots haste you
                    case FixedArtifactId.BootsOfIthaqua:
                        {
                            Profile.Instance.MsgPrint("A wind swirls around your boots...");
                            if (player.TimedHaste == 0)
                            {
                                player.SetTimedHaste(Program.Rng.DieRoll(20) + 20);
                            }
                            else
                            {
                                player.SetTimedHaste(player.TimedHaste + 5);
                            }
                            item.RechargeTimeLeft = 200;
                            break;
                        }
                    // Dancing heal poison and fear
                    case FixedArtifactId.BootsOfDancing:
                        {
                            Profile.Instance.MsgPrint("Your boots glow deep blue...");
                            player.SetTimedFear(0);
                            player.SetTimedPoison(0);
                            item.RechargeTimeLeft = 5;
                            break;
                        }
                    // Faith shoots a fire bolt
                    case FixedArtifactId.DaggerOfFaith:
                        {
                            Profile.Instance.MsgPrint("Your dagger is covered in fire...");
                            if (!targetEngine.GetDirectionWithAim(out dir))
                            {
                                return;
                            }
                            SaveGame.Instance.SpellEffects.FireBolt(new ProjectFire(SaveGame.Instance.SpellEffects), dir, Program.Rng.DiceRoll(9, 8));
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(8) + 8;
                            break;
                        }
                    // Hope shoots a frost bolt
                    case FixedArtifactId.DaggerOfHope:
                        {
                            Profile.Instance.MsgPrint("Your dagger is covered in frost...");
                            if (!targetEngine.GetDirectionWithAim(out dir))
                            {
                                return;
                            }
                            SaveGame.Instance.SpellEffects.FireBolt(new ProjectCold(SaveGame.Instance.SpellEffects), dir, Program.Rng.DiceRoll(6, 8));
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(7) + 7;
                            break;
                        }
                    // Charity shoots a lightning bolt
                    case FixedArtifactId.DaggerOfCharity:
                        {
                            Profile.Instance.MsgPrint("Your dagger is covered in sparks...");
                            if (!targetEngine.GetDirectionWithAim(out dir))
                            {
                                return;
                            }
                            SaveGame.Instance.SpellEffects.FireBolt(new ProjectElec(SaveGame.Instance.SpellEffects), dir, Program.Rng.DiceRoll(4, 8));
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(6) + 6;
                            break;
                        }
                    // Thoth shoots a poison ball
                    case FixedArtifactId.DaggerOfThoth:
                        {
                            Profile.Instance.MsgPrint("Your dagger throbs deep green...");
                            if (!targetEngine.GetDirectionWithAim(out dir))
                            {
                                return;
                            }
                            SaveGame.Instance.SpellEffects.FireBall(new ProjectPois(SaveGame.Instance.SpellEffects), dir, 12, 3);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(4) + 4;
                            break;
                        }
                    // Icicle shoots a cold ball
                    case FixedArtifactId.DaggerIcicle:
                        {
                            Profile.Instance.MsgPrint("Your dagger is covered in frost...");
                            if (!targetEngine.GetDirectionWithAim(out dir))
                            {
                                return;
                            }
                            SaveGame.Instance.SpellEffects.FireBall(new ProjectCold(SaveGame.Instance.SpellEffects), dir, 48, 2);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(5) + 5;
                            break;
                        }
                    // Karakal teleports you randomly
                    case FixedArtifactId.SwordOfKarakal:
                        {
                            switch (Program.Rng.DieRoll(13))
                            {
                                case 1:
                                case 2:
                                case 3:
                                case 4:
                                case 5:
                                    SaveGame.Instance.SpellEffects.TeleportPlayer(10);
                                    break;

                                case 6:
                                case 7:
                                case 8:
                                case 9:
                                case 10:
                                    SaveGame.Instance.SpellEffects.TeleportPlayer(222);
                                    break;

                                case 11:
                                case 12:
                                    SaveGame.Instance.SpellEffects.StairCreation();
                                    break;

                                default:
                                    if (Gui.GetCheck("Leave this level? "))
                                    {
                                        {
                                            SaveGame.Instance.IsAutosave = true;
                                            SaveGame.Instance.DoCmdSaveGame();
                                            SaveGame.Instance.IsAutosave = false;
                                        }
                                        SaveGame.Instance.NewLevelFlag = true;
                                        SaveGame.Instance.CameFrom = LevelStart.StartRandom;
                                    }
                                    break;
                            }
                            item.RechargeTimeLeft = 35;
                            break;
                        }
                    // Excalibur shoots a frost ball
                    case FixedArtifactId.SwordExcalibur:
                        {
                            Profile.Instance.MsgPrint("Your sword glows an intense blue...");
                            if (!targetEngine.GetDirectionWithAim(out dir))
                            {
                                return;
                            }
                            SaveGame.Instance.SpellEffects.FireBall(new ProjectCold(SaveGame.Instance.SpellEffects), dir, 100, 2);
                            item.RechargeTimeLeft = 300;
                            break;
                        }
                    // Dawn Sword summons a reaver
                    case FixedArtifactId.SwordOfTheDawn:
                        {
                            Profile.Instance.MsgPrint("Your sword flickers black for a moment...");
                            level.Monsters.SummonSpecificFriendly(player.MapY, player.MapX, SaveGame.Instance.Difficulty,
                                Constants.SummonReaver, true);
                            item.RechargeTimeLeft = 500 + Program.Rng.DieRoll(500);
                            break;
                        }
                    // Everflame shoots a fire ball
                    case FixedArtifactId.SwordOfEverflame:
                        {
                            Profile.Instance.MsgPrint("Your sword glows an intense red...");
                            if (!targetEngine.GetDirectionWithAim(out dir))
                            {
                                return;
                            }
                            SaveGame.Instance.SpellEffects.FireBall(new ProjectFire(SaveGame.Instance.SpellEffects), dir, 72, 2);
                            item.RechargeTimeLeft = 400;
                            break;
                        }
                    // Theoden drains life
                    case FixedArtifactId.AxeOfTheoden:
                        {
                            Profile.Instance.MsgPrint("Your axe blade glows black...");
                            if (!targetEngine.GetDirectionWithAim(out dir))
                            {
                                return;
                            }
                            SaveGame.Instance.SpellEffects.DrainLife(dir, 120);
                            item.RechargeTimeLeft = 400;
                            break;
                        }
                    // Grungnir shoots a lightning ball
                    case FixedArtifactId.SpearGungnir:
                        {
                            Profile.Instance.MsgPrint("Your spear crackles with electricity...");
                            if (!targetEngine.GetDirectionWithAim(out dir))
                            {
                                return;
                            }
                            SaveGame.Instance.SpellEffects.FireBall(new ProjectElec(SaveGame.Instance.SpellEffects), dir, 100, 3);
                            item.RechargeTimeLeft = 500;
                            break;
                        }
                    // Destiny does rock to mud
                    case FixedArtifactId.SpearOfDestiny:
                        {
                            Profile.Instance.MsgPrint("Your spear pulsates...");
                            if (!targetEngine.GetDirectionWithAim(out dir))
                            {
                                return;
                            }
                            SaveGame.Instance.SpellEffects.WallToMud(dir);
                            item.RechargeTimeLeft = 5;
                            break;
                        }
                    // Trolls does mass carnage
                    case FixedArtifactId.AxeOfTheTrolls:
                        {
                            Profile.Instance.MsgPrint("Your axe lets out a long, shrill note...");
                            SaveGame.Instance.SpellEffects.MassCarnage(true);
                            item.RechargeTimeLeft = 1000;
                            break;
                        }
                    // Spleens Slicer heals you
                    case FixedArtifactId.AxeSpleenSlicer:
                        {
                            Profile.Instance.MsgPrint("Your battle axe radiates deep purple...");
                            player.RestoreHealth(Program.Rng.DiceRoll(4, 8));
                            player.SetTimedBleeding((player.TimedBleeding / 2) - 50);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(3) + 3;
                            break;
                        }
                    // Gnorri teleports monsters away
                    case FixedArtifactId.TridentOfTheGnorri:
                        {
                            Profile.Instance.MsgPrint("Your trident glows deep red...");
                            if (!targetEngine.GetDirectionWithAim(out dir))
                            {
                                return;
                            }
                            SaveGame.Instance.SpellEffects.TeleportMonster(dir);
                            item.RechargeTimeLeft = 150;
                            break;
                        }
                    // G'Harne does Word of Recall
                    case FixedArtifactId.ScytheOfGharne:
                        {
                            Profile.Instance.MsgPrint("Your scythe glows soft white...");
                            player.ToggleRecall();
                            item.RechargeTimeLeft = 200;
                            break;
                        }
                    // Totila does confusion
                    case FixedArtifactId.FlailTotila:
                        {
                            Profile.Instance.MsgPrint("Your flail glows in scintillating colours...");
                            if (!targetEngine.GetDirectionWithAim(out dir))
                            {
                                return;
                            }
                            SaveGame.Instance.SpellEffects.ConfuseMonster(dir, 20);
                            item.RechargeTimeLeft = 15;
                            break;
                        }
                    // Firestarter does fire ball
                    case FixedArtifactId.MorningStarFirestarter:
                        {
                            Profile.Instance.MsgPrint("Your morning star rages in fire...");
                            if (!targetEngine.GetDirectionWithAim(out dir))
                            {
                                return;
                            }
                            SaveGame.Instance.SpellEffects.FireBall(new ProjectFire(SaveGame.Instance.SpellEffects), dir, 72, 3);
                            item.RechargeTimeLeft = 100;
                            break;
                        }
                    // Thunder does haste
                    case FixedArtifactId.MaceThunder:
                        {
                            Profile.Instance.MsgPrint("Your mace glows bright green...");
                            if (player.TimedHaste == 0)
                            {
                                player.SetTimedHaste(Program.Rng.DieRoll(20) + 20);
                            }
                            else
                            {
                                player.SetTimedHaste(player.TimedHaste + 5);
                            }
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(100) + 100;
                            break;
                        }
                    // Ereril does identify
                    case FixedArtifactId.QuarterstaffEriril:
                        {
                            Profile.Instance.MsgPrint("Your quarterstaff glows yellow...");
                            if (!SaveGame.Instance.SpellEffects.IdentifyItem())
                            {
                                return;
                            }
                            item.RechargeTimeLeft = 10;
                            break;
                        }
                    // Atal does full identify
                    case FixedArtifactId.QuarterstaffOfAtal:
                        {
                            Profile.Instance.MsgPrint("Your quarterstaff glows brightly...");
                            SaveGame.Instance.SpellEffects.DetectAll();
                            SaveGame.Instance.SpellEffects.Probing();
                            SaveGame.Instance.SpellEffects.IdentifyFully();
                            item.RechargeTimeLeft = 1000;
                            break;
                        }
                    // Justice drains life
                    case FixedArtifactId.HammerJustice:
                        {
                            Profile.Instance.MsgPrint("Your hammer glows white...");
                            if (!targetEngine.GetDirectionWithAim(out dir))
                            {
                                return;
                            }
                            SaveGame.Instance.SpellEffects.DrainLife(dir, 90);
                            item.RechargeTimeLeft = 70;
                            break;
                        }
                    // Death brands your bolts
                    case FixedArtifactId.CrossbowOfDeath:
                        {
                            Profile.Instance.MsgPrint("Your crossbow glows deep red...");
                            SaveGame.Instance.CommandEngine.BrandBolts();
                            item.RechargeTimeLeft = 999;
                            break;
                        }
                }
                return;
            }
            // If it wasn't an artifact, then check the other types of activatable item Planar
            // weapon teleports you
            if (item.RareItemTypeIndex == Enumerations.RareItemType.WeaponPlanarWeapon)
            {
                SaveGame.Instance.SpellEffects.TeleportPlayer(100);
                item.RechargeTimeLeft = 50 + Program.Rng.DieRoll(50);
                return;
            }
            // Dragon armour gives you a ball of the relevant damage type
            if (item.Category == ItemCategory.DragArmor)
            {
                if (!targetEngine.GetDirectionWithAim(out dir))
                {
                    return;
                }
                switch (item.ItemSubCategory)
                {
                    case DragonArmour.SvDragonBlue:
                        {
                            Profile.Instance.MsgPrint("You breathe lightning.");
                            SaveGame.Instance.SpellEffects.FireBall(new ProjectElec(SaveGame.Instance.SpellEffects), dir, 100, -2);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(450) + 450;
                            break;
                        }
                    case DragonArmour.SvDragonWhite:
                        {
                            Profile.Instance.MsgPrint("You breathe frost.");
                            SaveGame.Instance.SpellEffects.FireBall(new ProjectCold(SaveGame.Instance.SpellEffects), dir, 110, -2);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(450) + 450;
                            break;
                        }
                    case DragonArmour.SvDragonBlack:
                        {
                            Profile.Instance.MsgPrint("You breathe acid.");
                            SaveGame.Instance.SpellEffects.FireBall(new ProjectAcid(SaveGame.Instance.SpellEffects), dir, 130, -2);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(450) + 450;
                            break;
                        }
                    case DragonArmour.SvDragonGreen:
                        {
                            Profile.Instance.MsgPrint("You breathe poison gas.");
                            SaveGame.Instance.SpellEffects.FireBall(new ProjectPois(SaveGame.Instance.SpellEffects), dir, 150, -2);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(450) + 450;
                            break;
                        }
                    case DragonArmour.SvDragonRed:
                        {
                            Profile.Instance.MsgPrint("You breathe fire.");
                            SaveGame.Instance.SpellEffects.FireBall(new ProjectFire(SaveGame.Instance.SpellEffects), dir, 200, -2);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(450) + 450;
                            break;
                        }
                    case DragonArmour.SvDragonMultihued:
                        {
                            chance = Program.Rng.RandomLessThan(5);
                            string element = chance == 1
                                ? "lightning"
                                : (chance == 2 ? "frost" : (chance == 3 ? "acid" : (chance == 4 ? "poison gas" : "fire")));
                            Profile.Instance.MsgPrint($"You breathe {element}.");
                            switch (chance)
                            {
                                case 0:
                                    SaveGame.Instance.SpellEffects.FireBall(new ProjectFire(SaveGame.Instance.SpellEffects),
                                        dir, 250, -2);
                                    break;

                                case 1:
                                    SaveGame.Instance.SpellEffects.FireBall(new ProjectElec(SaveGame.Instance.SpellEffects),
                                        dir, 250, -2);
                                    break;

                                case 2:
                                    SaveGame.Instance.SpellEffects.FireBall(new ProjectCold(SaveGame.Instance.SpellEffects),
                                        dir, 250, -2);
                                    break;

                                case 3:
                                    SaveGame.Instance.SpellEffects.FireBall(new ProjectAcid(SaveGame.Instance.SpellEffects),
                                        dir, 250, -2);
                                    break;

                                case 4:
                                    SaveGame.Instance.SpellEffects.FireBall(new ProjectPois(SaveGame.Instance.SpellEffects),
                                        dir, 250, -2);
                                    break;
                            }
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(225) + 225;
                            break;
                        }
                    case DragonArmour.SvDragonBronze:
                        {
                            Profile.Instance.MsgPrint("You breathe confusion.");
                            SaveGame.Instance.SpellEffects.FireBall(new ProjectConfusion(SaveGame.Instance.SpellEffects), dir, 120, -2);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(450) + 450;
                            break;
                        }
                    case DragonArmour.SvDragonGold:
                        {
                            Profile.Instance.MsgPrint("You breathe sound.");
                            SaveGame.Instance.SpellEffects.FireBall(new ProjectSound(SaveGame.Instance.SpellEffects), dir, 130, -2);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(450) + 450;
                            break;
                        }
                    case DragonArmour.SvDragonChaos:
                        {
                            chance = Program.Rng.RandomLessThan(2);
                            string element = chance == 1 ? "chaos" : "disenchantment";
                            Profile.Instance.MsgPrint($"You breathe {element}.");
                            SaveGame.Instance.SpellEffects.FireBall(
                                projectile: chance == 1 ? (Projectile)new ProjectChaos(SaveGame.Instance.SpellEffects) : new ProjectDisenchant(SaveGame.Instance.SpellEffects), dir: dir, dam: 220, rad: -2);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(300) + 300;
                            break;
                        }
                    case DragonArmour.SvDragonLaw:
                        {
                            chance = Program.Rng.RandomLessThan(2);
                            string element = chance == 1 ? "sound" : "shards";
                            Profile.Instance.MsgPrint($"You breathe {element}.");
                            SaveGame.Instance.SpellEffects.FireBall(
                                chance == 1 ? (Projectile)new ProjectSound(SaveGame.Instance.SpellEffects) : new ProjectExplode(SaveGame.Instance.SpellEffects), dir, 230, -2);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(300) + 300;
                            break;
                        }
                    case DragonArmour.SvDragonBalance:
                        {
                            chance = Program.Rng.RandomLessThan(4);
                            string element = chance == 1
                                ? "chaos"
                                : (chance == 2 ? "disenchantment" : (chance == 3 ? "sound" : "shards"));
                            Profile.Instance.MsgPrint($"You breathe {element}.");
                            SaveGame.Instance.SpellEffects.FireBall(
                                chance == 1
                                    ? new ProjectChaos(SaveGame.Instance.SpellEffects)
                                    : (chance == 2
                                        ? new ProjectDisenchant(SaveGame.Instance.SpellEffects)
                                        : (chance == 3 ? (Projectile)new ProjectSound(SaveGame.Instance.SpellEffects) : new ProjectExplode(SaveGame.Instance.SpellEffects))), dir, 250, -2);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(300) + 300;
                            break;
                        }
                    case DragonArmour.SvDragonShining:
                        {
                            chance = Program.Rng.RandomLessThan(2);
                            string element = chance == 0 ? "light" : "darkness";
                            Profile.Instance.MsgPrint($"You breathe {element}.");
                            SaveGame.Instance.SpellEffects.FireBall(
                                chance == 0 ? (Projectile)new ProjectLight(SaveGame.Instance.SpellEffects) : new ProjectDark(SaveGame.Instance.SpellEffects), dir, 200, -2);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(300) + 300;
                            break;
                        }
                    case DragonArmour.SvDragonPower:
                        {
                            Profile.Instance.MsgPrint("You breathe the elements.");
                            SaveGame.Instance.SpellEffects.FireBall(new ProjectMissile(SaveGame.Instance.SpellEffects), dir, 300, -3);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(300) + 300;
                            break;
                        }
                }
                return;
            }
            // Elemental rings give you a ball of the appropriate element
            if (item.Category == ItemCategory.Ring)
            {
                if (!targetEngine.GetDirectionWithAim(out dir))
                {
                    return;
                }
                switch (item.ItemSubCategory)
                {
                    case RingType.Acid:
                        {
                            SaveGame.Instance.SpellEffects.FireBall(new ProjectAcid(SaveGame.Instance.SpellEffects), dir, 50, 2);
                            player.SetTimedAcidResistance(player.TimedAcidResistance + Program.Rng.DieRoll(20) + 20);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(50) + 50;
                            break;
                        }
                    case RingType.Ice:
                        {
                            SaveGame.Instance.SpellEffects.FireBall(new ProjectCold(SaveGame.Instance.SpellEffects), dir, 50, 2);
                            player.SetTimedColdResistance(player.TimedColdResistance + Program.Rng.DieRoll(20) + 20);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(50) + 50;
                            break;
                        }
                    case RingType.Flames:
                        {
                            SaveGame.Instance.SpellEffects.FireBall(new ProjectFire(SaveGame.Instance.SpellEffects), dir, 50, 2);
                            player.SetTimedFireResistance(player.TimedFireResistance + Program.Rng.DieRoll(20) + 20);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(50) + 50;
                            break;
                        }
                }
                return;
            }
            // We ran out of item types
            Profile.Instance.MsgPrint("Oops. That object cannot be activated.");
        }
    }
}
