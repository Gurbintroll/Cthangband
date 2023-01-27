// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Projection;
using Cthangband.Projection.Base;
using Cthangband.StaticData;
using Cthangband.UI;
using Cthangband.Patron.Base;
using System;

namespace Cthangband
{
    /// <summary>
    /// A handler for all types of item activation
    /// </summary>
    [Serializable]
    internal class ActivationHandler
    {
        private readonly Level _level;
        private readonly Player _player;

        public ActivationHandler(Level level, Player player)
        {
            _level = level;
            _player = player;
        }

        /// <summary>
        /// Activate an artifact or similar
        /// </summary>
        /// <param name="itemIndex">
        /// The inventory index of the item to be activated, or -999 to select item
        /// </param>
        public void DoCmdActivate(int itemIndex)
        {
            int dir;
            TargetEngine targetEngine = new TargetEngine(_player, _level);
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
            Item item = itemIndex >= 0 ? _player.Inventory[itemIndex] : _level.Items[0 - itemIndex];
            // Check if the item is activatable
            SaveGame.Instance.ItemFilter = SaveGame.Instance.CommandEngine.ItemFilterActivatable;
            if (!_player.Inventory.ItemMatchesFilter(item))
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
            if (item.IsArtifact())
            {
                itemLevel = Profile.Instance.Artifacts[item.ArtifactIndex].Level;
            }
            // Work out the chance of using the item successfully based on its level and the
            // player's skill
            int chance = _player.SkillUseDevice;
            if (_player.TimedConfusion != 0)
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
            // If it is a legendary item then use its ability and quit
            if (item.IsLegendary())
            {
                SaveGame.Instance.CommandEngine.ActivateLegendary(item);
                return;
            }
            // If it's an artifact then use its ability
            if (item.ArtifactIndex != 0)
            {
                switch (item.ArtifactIndex)
                {
                    // Star Essence of Polaris lights the area
                    case ArtifactId.StarEssenceOfPolaris:
                        {
                            Profile.Instance.MsgPrint("The essence wells with clear light...");
                            SaveGame.Instance.SpellEffects.LightArea(Program.Rng.DiceRoll(2, 15), 3);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(10) + 10;
                            break;
                        }
                    // Star essence of Xoth lights and maps the area
                    case ArtifactId.StarEssenceOfXoth:
                        {
                            Profile.Instance.MsgPrint("The essence shines brightly...");
                            _level.MapArea();
                            SaveGame.Instance.SpellEffects.LightArea(Program.Rng.DiceRoll(2, 15), 3);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(50) + 50;
                            break;
                        }
                    // Shining Trapezohedron lights the entire level and recalls us, but drains
                    // health to do so
                    case ArtifactId.ShiningTrapezohedron:
                        {
                            Profile.Instance.MsgPrint("The gemstone flashes bright red!");
                            _level.WizLight();
                            Profile.Instance.MsgPrint("The gemstone drains your vitality...");
                            _player.TakeHit(Program.Rng.DiceRoll(3, 8), "the Gemstone 'Trapezohedron'");
                            SaveGame.Instance.SpellEffects.DetectTraps();
                            SaveGame.Instance.SpellEffects.DetectDoors();
                            SaveGame.Instance.SpellEffects.DetectStairs();
                            if (Gui.GetCheck("Activate recall? "))
                            {
                                _player.ToggleRecall();
                            }
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(20) + 20;
                            break;
                        }
                    // Amulet of Lobon protects us from evil
                    case ArtifactId.AmuletOfLobon:
                        {
                            Profile.Instance.MsgPrint("The amulet lets out a shrill wail...");
                            int k = 3 * _player.Level;
                            _player.SetTimedProtectionFromEvil(_player.TimedProtectionFromEvil + Program.Rng.DieRoll(25) + k);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(225) + 225;
                            break;
                        }
                    // Amulet of Abdul Alhazred dispels evil
                    case ArtifactId.AmuletOfAbdulAlhazred:
                        {
                            Profile.Instance.MsgPrint("The amulet floods the area with goodness...");
                            SaveGame.Instance.SpellEffects.DispelEvil(_player.Level * 5);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(300) + 300;
                            break;
                        }
                    // Ring of Magic has a djinn in it that drains life from an opponent
                    case ArtifactId.RingOfMagic:
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
                    case ArtifactId.RingOfBast:
                        {
                            Profile.Instance.MsgPrint("The ring glows brightly...");
                            if (_player.TimedHaste == 0)
                            {
                                _player.SetTimedHaste(Program.Rng.DieRoll(75) + 75);
                            }
                            else
                            {
                                _player.SetTimedHaste(_player.TimedHaste + 5);
                            }
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(150) + 150;
                            break;
                        }
                    // Ring of Elemental Fire casts a fireball
                    case ArtifactId.RingOfElementalPowerFire:
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
                    case ArtifactId.RingOfElementalPowerIce:
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
                    case ArtifactId.RingOfElementalPowerStorm:
                        {
                            Profile.Instance.MsgPrint("The ring glows deep blue...");
                            if (!targetEngine.GetDirectionWithAim(out dir))
                            {
                                return;
                            }
                            SaveGame.Instance.SpellEffects.FireBall(new ProjectElectricity(SaveGame.Instance.SpellEffects), dir, 250, 3);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(425) + 425;
                            break;
                        }
                    // Ring of Set has a random effect
                    case ArtifactId.RingOfSet:
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
                    case ArtifactId.DragonScaleRazorback:
                        {
                            Profile.Instance.MsgPrint("Your armor is surrounded by lightning...");
                            for (int i = 0; i < 8; i++)
                            {
                                SaveGame.Instance.SpellEffects.FireBall(new ProjectElectricity(SaveGame.Instance.SpellEffects), _level.OrderedDirection[i], 150, 3);
                            }
                            item.RechargeTimeLeft = 1000;
                            break;
                        }
                    // Bladeturner heals you and gives you timed resistances
                    case ArtifactId.DragonScaleBladeturner:
                        {
                            if (!targetEngine.GetDirectionWithAim(out dir))
                            {
                                return;
                            }
                            Profile.Instance.MsgPrint("You breathe the elements.");
                            SaveGame.Instance.SpellEffects.FireBall(new ProjectMissile(SaveGame.Instance.SpellEffects), dir, 300, 4);
                            Profile.Instance.MsgPrint("Your armor glows many colors...");
                            _player.SetTimedFear(0);
                            _player.SetTimedSuperheroism(_player.TimedSuperheroism + Program.Rng.DieRoll(50) + 50);
                            _player.RestoreHealth(30);
                            _player.SetTimedBlessing(_player.TimedBlessing + Program.Rng.DieRoll(50) + 50);
                            _player.SetTimedAcidResistance(_player.TimedAcidResistance + Program.Rng.DieRoll(50) + 50);
                            _player.SetTimedLightningResistance(_player.TimedLightningResistance + Program.Rng.DieRoll(50) + 50);
                            _player.SetTimedFireResistance(_player.TimedFireResistance + Program.Rng.DieRoll(50) + 50);
                            _player.SetTimedColdResistance(_player.TimedColdResistance + Program.Rng.DieRoll(50) + 50);
                            _player.SetTimedPoisonResistance(_player.TimedPoisonResistance + Program.Rng.DieRoll(50) + 50);
                            item.RechargeTimeLeft = 400;
                            break;
                        }
                    // Soulkeeper heals you a lot
                    case ArtifactId.PlateMailSoulkeeper:
                        {
                            Profile.Instance.MsgPrint("Your armor glows a bright white...");
                            Profile.Instance.MsgPrint("You feel much better...");
                            _player.RestoreHealth(1000);
                            _player.SetTimedBleeding(0);
                            item.RechargeTimeLeft = 888;
                            break;
                        }
                    // Vampire Hunter cures most ailments
                    case ArtifactId.ArmourOfTheVampireHunter:
                        {
                            Profile.Instance.MsgPrint("A heavenly choir sings...");
                            _player.SetTimedPoison(0);
                            _player.SetTimedBleeding(0);
                            _player.SetTimedStun(0);
                            _player.SetTimedConfusion(0);
                            _player.SetTimedBlindness(0);
                            _player.SetTimedHeroism(_player.TimedHeroism + Program.Rng.DieRoll(25) + 25);
                            _player.RestoreHealth(777);
                            item.RechargeTimeLeft = 300;
                            break;
                        }
                    // Orc does Carnage
                    case ArtifactId.ArmourOfTheOrcs:
                        {
                            Profile.Instance.MsgPrint("Your armor glows deep blue...");
                            SaveGame.Instance.SpellEffects.Carnage(true);
                            item.RechargeTimeLeft = 500;
                            break;
                        }
                    // Ogre Lords destroys doors
                    case ArtifactId.ArmourOfTheOgreLords:
                        {
                            Profile.Instance.MsgPrint("Your armor glows bright red...");
                            SaveGame.Instance.SpellEffects.DestroyDoorsTouch();
                            item.RechargeTimeLeft = 10;
                            break;
                        }
                    // Dragon Helm and Terror Mask cause fear
                    case ArtifactId.DragonHelmOfPower:
                    case ArtifactId.HelmTerrorMask:
                        {
                            SaveGame.Instance.SpellEffects.TurnMonsters(40 + _player.Level);
                            item.RechargeTimeLeft = 3 * (_player.Level + 10);
                            break;
                        }
                    // Skull Keeper detects everything
                    case ArtifactId.HelmSkullkeeper:
                        {
                            Profile.Instance.MsgPrint("Your helm glows bright white...");
                            Profile.Instance.MsgPrint("An image forms in your mind...");
                            SaveGame.Instance.SpellEffects.DetectAll();
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(55) + 55;
                            break;
                        }
                    // Sun Crown heals
                    case ArtifactId.CrownOfTheSun:
                        {
                            Profile.Instance.MsgPrint("Your crown glows deep yellow...");
                            Profile.Instance.MsgPrint("You feel a warm tingling inside...");
                            _player.RestoreHealth(700);
                            _player.SetTimedBleeding(0);
                            item.RechargeTimeLeft = 250;
                            break;
                        }
                    // Cloak of Barzai gives resistances
                    case ArtifactId.CloakOfBarzai:
                        {
                            Profile.Instance.MsgPrint("Your cloak glows many colours...");
                            _player.SetTimedAcidResistance(_player.TimedAcidResistance + Program.Rng.DieRoll(20) + 20);
                            _player.SetTimedLightningResistance(_player.TimedLightningResistance + Program.Rng.DieRoll(20) + 20);
                            _player.SetTimedFireResistance(_player.TimedFireResistance + Program.Rng.DieRoll(20) + 20);
                            _player.SetTimedColdResistance(_player.TimedColdResistance + Program.Rng.DieRoll(20) + 20);
                            _player.SetTimedPoisonResistance(_player.TimedPoisonResistance + Program.Rng.DieRoll(20) + 20);
                            item.RechargeTimeLeft = 111;
                            break;
                        }
                    // Darkness sends monsters to sleep
                    case ArtifactId.CloakDarkness:
                        {
                            Profile.Instance.MsgPrint("Your cloak glows deep blue...");
                            SaveGame.Instance.SpellEffects.SleepMonstersTouch();
                            item.RechargeTimeLeft = 55;
                            break;
                        }
                    // Swashbuckler recharges items
                    case ArtifactId.CloakOfTheSwashbuckler:
                        {
                            Profile.Instance.MsgPrint("Your cloak glows bright yellow...");
                            SaveGame.Instance.SpellEffects.Recharge(60);
                            item.RechargeTimeLeft = 70;
                            break;
                        }
                    // Shifter teleports you
                    case ArtifactId.CloakShifter:
                        {
                            Profile.Instance.MsgPrint("Your cloak twists space around you...");
                            SaveGame.Instance.SpellEffects.TeleportPlayer(100);
                            item.RechargeTimeLeft = 45;
                            break;
                        }
                    // Nyogtha restores experience
                    case ArtifactId.ShadowCloakOfNyogtha:
                        {
                            Profile.Instance.MsgPrint("Your cloak glows a deep red...");
                            _player.RestoreLevel();
                            item.RechargeTimeLeft = 450;
                            break;
                        }
                    // Light shoots magic missiles
                    case ArtifactId.GlovesOfLight:
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
                    case ArtifactId.GauntletIronfist:
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
                    case ArtifactId.GauntletsOfGhouls:
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
                    case ArtifactId.GauntletsWhiteSpark:
                        {
                            Profile.Instance.MsgPrint("Your gauntlets are covered in sparks...");
                            if (!targetEngine.GetDirectionWithAim(out dir))
                            {
                                return;
                            }
                            SaveGame.Instance.SpellEffects.FireBolt(new ProjectElectricity(SaveGame.Instance.SpellEffects), dir, Program.Rng.DiceRoll(4, 8));
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(6) + 6;
                            break;
                        }
                    // The Dead shoot acid bolts
                    case ArtifactId.GauntletsOfTheDead:
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
                    case ArtifactId.CestiOfCombat:
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
                    case ArtifactId.BootsOfIthaqua:
                        {
                            Profile.Instance.MsgPrint("A wind swirls around your boots...");
                            if (_player.TimedHaste == 0)
                            {
                                _player.SetTimedHaste(Program.Rng.DieRoll(20) + 20);
                            }
                            else
                            {
                                _player.SetTimedHaste(_player.TimedHaste + 5);
                            }
                            item.RechargeTimeLeft = 200;
                            break;
                        }
                    // Dancing heal poison and fear
                    case ArtifactId.BootsOfDancing:
                        {
                            Profile.Instance.MsgPrint("Your boots glow deep blue...");
                            _player.SetTimedFear(0);
                            _player.SetTimedPoison(0);
                            item.RechargeTimeLeft = 5;
                            break;
                        }
                    // Faith shoots a fire bolt
                    case ArtifactId.DaggerOfFaith:
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
                    case ArtifactId.DaggerOfHope:
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
                    case ArtifactId.DaggerOfCharity:
                        {
                            Profile.Instance.MsgPrint("Your dagger is covered in sparks...");
                            if (!targetEngine.GetDirectionWithAim(out dir))
                            {
                                return;
                            }
                            SaveGame.Instance.SpellEffects.FireBolt(new ProjectElectricity(SaveGame.Instance.SpellEffects), dir, Program.Rng.DiceRoll(4, 8));
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(6) + 6;
                            break;
                        }
                    // Thoth shoots a poison ball
                    case ArtifactId.DaggerOfThoth:
                        {
                            Profile.Instance.MsgPrint("Your dagger throbs deep green...");
                            if (!targetEngine.GetDirectionWithAim(out dir))
                            {
                                return;
                            }
                            SaveGame.Instance.SpellEffects.FireBall(new ProjectPoison(SaveGame.Instance.SpellEffects), dir, 12, 3);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(4) + 4;
                            break;
                        }
                    // Icicle shoots a cold ball
                    case ArtifactId.DaggerIcicle:
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
                    case ArtifactId.SwordOfKarakal:
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
                    case ArtifactId.SwordExcalibur:
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
                    case ArtifactId.SwordOfTheDawn:
                        {
                            Profile.Instance.MsgPrint("Your sword flickers black for a moment...");
                            _level.Monsters.SummonSpecificFriendly(_player.MapY, _player.MapX, SaveGame.Instance.Difficulty,
                                Constants.SummonReaver, true);
                            item.RechargeTimeLeft = 500 + Program.Rng.DieRoll(500);
                            break;
                        }
                    // Everflame shoots a fire ball
                    case ArtifactId.SwordOfEverflame:
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
                    case ArtifactId.AxeOfTheoden:
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
                    case ArtifactId.SpearGungnir:
                        {
                            Profile.Instance.MsgPrint("Your spear crackles with electricity...");
                            if (!targetEngine.GetDirectionWithAim(out dir))
                            {
                                return;
                            }
                            SaveGame.Instance.SpellEffects.FireBall(new ProjectElectricity(SaveGame.Instance.SpellEffects), dir, 100, 3);
                            item.RechargeTimeLeft = 500;
                            break;
                        }
                    // Destiny does rock to mud
                    case ArtifactId.SpearOfDestiny:
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
                    case ArtifactId.AxeOfTheTrolls:
                        {
                            Profile.Instance.MsgPrint("Your axe lets out a long, shrill note...");
                            SaveGame.Instance.SpellEffects.MassCarnage(true);
                            item.RechargeTimeLeft = 1000;
                            break;
                        }
                    // Spleens Slicer heals you
                    case ArtifactId.AxeSpleenSlicer:
                        {
                            Profile.Instance.MsgPrint("Your battle axe radiates deep purple...");
                            _player.RestoreHealth(Program.Rng.DiceRoll(4, 8));
                            _player.SetTimedBleeding((_player.TimedBleeding / 2) - 50);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(3) + 3;
                            break;
                        }
                    // Gnorri teleports monsters away
                    case ArtifactId.TridentOfTheGnorri:
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
                    case ArtifactId.ScytheOfGharne:
                        {
                            Profile.Instance.MsgPrint("Your scythe glows soft white...");
                            _player.ToggleRecall();
                            item.RechargeTimeLeft = 200;
                            break;
                        }
                    // Totila does confusion
                    case ArtifactId.FlailTotila:
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
                    case ArtifactId.MorningStarFirestarter:
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
                    case ArtifactId.MaceThunder:
                        {
                            Profile.Instance.MsgPrint("Your mace glows bright green...");
                            if (_player.TimedHaste == 0)
                            {
                                _player.SetTimedHaste(Program.Rng.DieRoll(20) + 20);
                            }
                            else
                            {
                                _player.SetTimedHaste(_player.TimedHaste + 5);
                            }
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(100) + 100;
                            break;
                        }
                    // Ereril does identify
                    case ArtifactId.QuarterstaffEriril:
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
                    case ArtifactId.QuarterstaffOfAtal:
                        {
                            Profile.Instance.MsgPrint("Your quarterstaff glows brightly...");
                            SaveGame.Instance.SpellEffects.DetectAll();
                            SaveGame.Instance.SpellEffects.Probing();
                            SaveGame.Instance.SpellEffects.IdentifyFully();
                            item.RechargeTimeLeft = 1000;
                            break;
                        }
                    // Justice drains life
                    case ArtifactId.HammerJustice:
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
                    case ArtifactId.CrossbowOfDeath:
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
                    case DragonArmour.Blue:
                        {
                            Profile.Instance.MsgPrint("You breathe lightning.");
                            SaveGame.Instance.SpellEffects.FireBall(new ProjectElectricity(SaveGame.Instance.SpellEffects), dir, 100, -2);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(450) + 450;
                            break;
                        }
                    case DragonArmour.White:
                        {
                            Profile.Instance.MsgPrint("You breathe frost.");
                            SaveGame.Instance.SpellEffects.FireBall(new ProjectCold(SaveGame.Instance.SpellEffects), dir, 110, -2);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(450) + 450;
                            break;
                        }
                    case DragonArmour.Black:
                        {
                            Profile.Instance.MsgPrint("You breathe acid.");
                            SaveGame.Instance.SpellEffects.FireBall(new ProjectAcid(SaveGame.Instance.SpellEffects), dir, 130, -2);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(450) + 450;
                            break;
                        }
                    case DragonArmour.Green:
                        {
                            Profile.Instance.MsgPrint("You breathe poison gas.");
                            SaveGame.Instance.SpellEffects.FireBall(new ProjectPoison(SaveGame.Instance.SpellEffects), dir, 150, -2);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(450) + 450;
                            break;
                        }
                    case DragonArmour.Red:
                        {
                            Profile.Instance.MsgPrint("You breathe fire.");
                            SaveGame.Instance.SpellEffects.FireBall(new ProjectFire(SaveGame.Instance.SpellEffects), dir, 200, -2);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(450) + 450;
                            break;
                        }
                    case DragonArmour.MultiHued:
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
                                    SaveGame.Instance.SpellEffects.FireBall(new ProjectElectricity(SaveGame.Instance.SpellEffects),
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
                                    SaveGame.Instance.SpellEffects.FireBall(new ProjectPoison(SaveGame.Instance.SpellEffects),
                                        dir, 250, -2);
                                    break;
                            }
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(225) + 225;
                            break;
                        }
                    case DragonArmour.Bronze:
                        {
                            Profile.Instance.MsgPrint("You breathe confusion.");
                            SaveGame.Instance.SpellEffects.FireBall(new ProjectConfusion(SaveGame.Instance.SpellEffects), dir, 120, -2);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(450) + 450;
                            break;
                        }
                    case DragonArmour.Gold:
                        {
                            Profile.Instance.MsgPrint("You breathe sound.");
                            SaveGame.Instance.SpellEffects.FireBall(new ProjectSound(SaveGame.Instance.SpellEffects), dir, 130, -2);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(450) + 450;
                            break;
                        }
                    case DragonArmour.Chaos:
                        {
                            chance = Program.Rng.RandomLessThan(2);
                            string element = chance == 1 ? "chaos" : "disenchantment";
                            Profile.Instance.MsgPrint($"You breathe {element}.");
                            SaveGame.Instance.SpellEffects.FireBall(
                                projectile: chance == 1 ? (IProjection)new ProjectChaos(SaveGame.Instance.SpellEffects) : new ProjectDisenchant(SaveGame.Instance.SpellEffects), dir: dir, dam: 220, rad: -2);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(300) + 300;
                            break;
                        }
                    case DragonArmour.Law:
                        {
                            chance = Program.Rng.RandomLessThan(2);
                            string element = chance == 1 ? "sound" : "shards";
                            Profile.Instance.MsgPrint($"You breathe {element}.");
                            SaveGame.Instance.SpellEffects.FireBall(
                                chance == 1 ? (IProjection)new ProjectSound(SaveGame.Instance.SpellEffects) : new ProjectExplode(SaveGame.Instance.SpellEffects), dir, 230, -2);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(300) + 300;
                            break;
                        }
                    case DragonArmour.Balance:
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
                                        : (chance == 3 ? (IProjection)new ProjectSound(SaveGame.Instance.SpellEffects) : new ProjectExplode(SaveGame.Instance.SpellEffects))), dir, 250, -2);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(300) + 300;
                            break;
                        }
                    case DragonArmour.Pseudo:
                        {
                            chance = Program.Rng.RandomLessThan(2);
                            string element = chance == 0 ? "light" : "darkness";
                            Profile.Instance.MsgPrint($"You breathe {element}.");
                            SaveGame.Instance.SpellEffects.FireBall(
                                chance == 0 ? (IProjection)new ProjectLight(SaveGame.Instance.SpellEffects) : new ProjectDark(SaveGame.Instance.SpellEffects), dir, 200, -2);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(300) + 300;
                            break;
                        }
                    case DragonArmour.Power:
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
                            _player.SetTimedAcidResistance(_player.TimedAcidResistance + Program.Rng.DieRoll(20) + 20);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(50) + 50;
                            break;
                        }
                    case RingType.Ice:
                        {
                            SaveGame.Instance.SpellEffects.FireBall(new ProjectCold(SaveGame.Instance.SpellEffects), dir, 50, 2);
                            _player.SetTimedColdResistance(_player.TimedColdResistance + Program.Rng.DieRoll(20) + 20);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(50) + 50;
                            break;
                        }
                    case RingType.Flames:
                        {
                            SaveGame.Instance.SpellEffects.FireBall(new ProjectFire(SaveGame.Instance.SpellEffects), dir, 50, 2);
                            _player.SetTimedFireResistance(_player.TimedFireResistance + Program.Rng.DieRoll(20) + 20);
                            item.RechargeTimeLeft = Program.Rng.RandomLessThan(50) + 50;
                            break;
                        }
                }
                return;
            }
            // We ran out of item types
            Profile.Instance.MsgPrint("Oops. That object cannot be activated.");
        }

        /// <summary>
        /// Aim a wand from your inventory
        /// </summary>
        /// <param name="itemIndex"> The inventory index of the wand, or -999 to select one </param>
        public void DoCmdAimWand(int itemIndex)
        {
            if (itemIndex == -999)
            {
                // Prompt for an item, showing only wands
                Inventory.ItemFilterCategory = ItemCategory.Wand;
                if (!SaveGame.Instance.GetItem(out itemIndex, "Aim which wand? ", true, true, true))
                {
                    if (itemIndex == -2)
                    {
                        Profile.Instance.MsgPrint("You have no wand to aim.");
                    }
                    return;
                }
            }
            // Get the item and check if it is really a wand
            Item item = itemIndex >= 0 ? _player.Inventory[itemIndex] : _level.Items[0 - itemIndex];
            Inventory.ItemFilterCategory = ItemCategory.Wand;
            if (!_player.Inventory.ItemMatchesFilter(item))
            {
                Profile.Instance.MsgPrint("That is not a wand!");
                Inventory.ItemFilterCategory = 0;
                return;
            }
            Inventory.ItemFilterCategory = 0;
            // We can't use wands directly from the floor, since we need to aim them
            if (itemIndex < 0 && item.Count > 1)
            {
                Profile.Instance.MsgPrint("You must first pick up the wands.");
                return;
            }
            // Aim the wand
            TargetEngine targetEngine = new TargetEngine(_player, _level);
            if (!targetEngine.GetDirectionWithAim(out int dir))
            {
                return;
            }
            // Using a wand takes 100 energy
            SaveGame.Instance.EnergyUse = 100;
            bool ident = false;
            int itemLevel = item.ItemType.Level;
            // Chance of success is your skill - item level, with item level capped at 50 and your
            // skill halved if you're confused
            int chance = _player.SkillUseDevice;
            if (_player.TimedConfusion != 0)
            {
                chance /= 2;
            }
            chance -= itemLevel > 50 ? 50 : itemLevel;
            // Always a small chance of success
            if (chance < Constants.UseDevice && Program.Rng.RandomLessThan(Constants.UseDevice - chance + 1) == 0)
            {
                chance = Constants.UseDevice;
            }
            if (chance < Constants.UseDevice || Program.Rng.DieRoll(chance) < Constants.UseDevice)
            {
                Profile.Instance.MsgPrint("You failed to use the wand properly.");
                return;
            }
            // Make sure we have charges
            if (item.TypeSpecificValue <= 0)
            {
                Profile.Instance.MsgPrint("The wand has no charges left.");
                item.IdentifyFlags.Set(Constants.IdentEmpty);
                return;
            }
            Gui.PlaySound(SoundEffect.ZapRod);
            int subCategory = item.ItemSubCategory;
            // Wand of wonder just chooses another type of wand less than its own index
            if (subCategory == WandType.Wonder)
            {
                subCategory = Program.Rng.RandomLessThan(WandType.Wonder);
            }
            switch (subCategory)
            {
                case WandType.HealMonster:
                    {
                        if (SaveGame.Instance.SpellEffects.HealMonster(dir))
                        {
                            ident = true;
                        }
                        break;
                    }
                case WandType.HasteMonster:
                    {
                        if (SaveGame.Instance.SpellEffects.SpeedMonster(dir))
                        {
                            ident = true;
                        }
                        break;
                    }
                case WandType.CloneMonster:
                    {
                        if (SaveGame.Instance.SpellEffects.CloneMonster(dir))
                        {
                            ident = true;
                        }
                        break;
                    }
                case WandType.TeleportAway:
                    {
                        if (SaveGame.Instance.SpellEffects.TeleportMonster(dir))
                        {
                            ident = true;
                        }
                        break;
                    }
                case WandType.Disarming:
                    {
                        if (SaveGame.Instance.SpellEffects.DisarmTrap(dir))
                        {
                            ident = true;
                        }
                        break;
                    }
                case WandType.TrapDoorDest:
                    {
                        if (SaveGame.Instance.SpellEffects.DestroyDoor(dir))
                        {
                            ident = true;
                        }
                        break;
                    }
                case WandType.StoneToMud:
                    {
                        if (SaveGame.Instance.SpellEffects.WallToMud(dir))
                        {
                            ident = true;
                        }
                        break;
                    }
                case WandType.Light:
                    {
                        Profile.Instance.MsgPrint("A line of blue shimmering light appears.");
                        SaveGame.Instance.SpellEffects.LightLine(dir);
                        ident = true;
                        break;
                    }
                case WandType.SleepMonster:
                    {
                        if (SaveGame.Instance.SpellEffects.SleepMonster(dir))
                        {
                            ident = true;
                        }
                        break;
                    }
                case WandType.SlowMonster:
                    {
                        if (SaveGame.Instance.SpellEffects.SlowMonster(dir))
                        {
                            ident = true;
                        }
                        break;
                    }
                case WandType.ConfuseMonster:
                    {
                        if (SaveGame.Instance.SpellEffects.ConfuseMonster(dir, 10))
                        {
                            ident = true;
                        }
                        break;
                    }
                case WandType.FearMonster:
                    {
                        if (SaveGame.Instance.SpellEffects.FearMonster(dir, 10))
                        {
                            ident = true;
                        }
                        break;
                    }
                case WandType.DrainLife:
                    {
                        if (SaveGame.Instance.SpellEffects.DrainLife(dir, 75))
                        {
                            ident = true;
                        }
                        break;
                    }
                case WandType.Polymorph:
                    {
                        if (SaveGame.Instance.SpellEffects.PolyMonster(dir))
                        {
                            ident = true;
                        }
                        break;
                    }
                case WandType.StinkingCloud:
                    {
                        SaveGame.Instance.SpellEffects.FireBall(new ProjectPoison(SaveGame.Instance.SpellEffects), dir, 12, 2);
                        ident = true;
                        break;
                    }
                case WandType.MagicMissile:
                    {
                        SaveGame.Instance.SpellEffects.FireBoltOrBeam(20, new ProjectMissile(SaveGame.Instance.SpellEffects), dir,
                            Program.Rng.DiceRoll(2, 6));
                        ident = true;
                        break;
                    }
                case WandType.AcidBolt:
                    {
                        SaveGame.Instance.SpellEffects.FireBoltOrBeam(20, new ProjectAcid(SaveGame.Instance.SpellEffects), dir,
                            Program.Rng.DiceRoll(3, 8));
                        ident = true;
                        break;
                    }
                case WandType.CharmMonster:
                    {
                        if (SaveGame.Instance.SpellEffects.CharmMonster(dir, 45))
                        {
                            ident = true;
                        }
                        break;
                    }
                case WandType.FireBolt:
                    {
                        SaveGame.Instance.SpellEffects.FireBoltOrBeam(20, new ProjectFire(SaveGame.Instance.SpellEffects), dir,
                            Program.Rng.DiceRoll(6, 8));
                        ident = true;
                        break;
                    }
                case WandType.ColdBolt:
                    {
                        SaveGame.Instance.SpellEffects.FireBoltOrBeam(20, new ProjectCold(SaveGame.Instance.SpellEffects), dir,
                            Program.Rng.DiceRoll(3, 8));
                        ident = true;
                        break;
                    }
                case WandType.AcidBall:
                    {
                        SaveGame.Instance.SpellEffects.FireBall(new ProjectAcid(SaveGame.Instance.SpellEffects), dir, 60, 2);
                        ident = true;
                        break;
                    }
                case WandType.ElecBall:
                    {
                        SaveGame.Instance.SpellEffects.FireBall(new ProjectElectricity(SaveGame.Instance.SpellEffects), dir, 32, 2);
                        ident = true;
                        break;
                    }
                case WandType.FireBall:
                    {
                        SaveGame.Instance.SpellEffects.FireBall(new ProjectFire(SaveGame.Instance.SpellEffects), dir, 72, 2);
                        ident = true;
                        break;
                    }
                case WandType.ColdBall:
                    {
                        SaveGame.Instance.SpellEffects.FireBall(new ProjectCold(SaveGame.Instance.SpellEffects), dir, 48, 2);
                        ident = true;
                        break;
                    }
                case WandType.Wonder:
                    {
                        Profile.Instance.MsgPrint("Oops. Wand of wonder activated.");
                        break;
                    }
                case WandType.DragonFire:
                    {
                        SaveGame.Instance.SpellEffects.FireBall(new ProjectFire(SaveGame.Instance.SpellEffects), dir, 100, 3);
                        ident = true;
                        break;
                    }
                case WandType.DragonCold:
                    {
                        SaveGame.Instance.SpellEffects.FireBall(new ProjectCold(SaveGame.Instance.SpellEffects), dir, 80, 3);
                        ident = true;
                        break;
                    }
                case WandType.DragonBreath:
                    {
                        switch (Program.Rng.DieRoll(5))
                        {
                            case 1:
                                {
                                    SaveGame.Instance.SpellEffects.FireBall(new ProjectAcid(SaveGame.Instance.SpellEffects), dir, 100, -3);
                                    break;
                                }
                            case 2:
                                {
                                    SaveGame.Instance.SpellEffects.FireBall(new ProjectElectricity(SaveGame.Instance.SpellEffects), dir, 80, -3);
                                    break;
                                }
                            case 3:
                                {
                                    SaveGame.Instance.SpellEffects.FireBall(new ProjectFire(SaveGame.Instance.SpellEffects), dir, 100, -3);
                                    break;
                                }
                            case 4:
                                {
                                    SaveGame.Instance.SpellEffects.FireBall(new ProjectCold(SaveGame.Instance.SpellEffects), dir, 80, -3);
                                    break;
                                }
                            default:
                                {
                                    SaveGame.Instance.SpellEffects.FireBall(new ProjectPoison(SaveGame.Instance.SpellEffects), dir, 60, -3);
                                    break;
                                }
                        }
                        ident = true;
                        break;
                    }
                case WandType.Annihilation:
                    {
                        if (SaveGame.Instance.SpellEffects.DrainLife(dir, 125))
                        {
                            ident = true;
                        }
                        break;
                    }
                case WandType.Shard:
                    {
                        SaveGame.Instance.SpellEffects.FireBall(new ProjectShard(SaveGame.Instance.SpellEffects), dir, 75 + Program.Rng.DieRoll(50),
                            2);
                        ident = true;
                        break;
                    }
            }
            _player.NoticeFlags |= Constants.PnCombine | Constants.PnReorder;
            // Mark the wand as having been tried
            item.ObjectTried();
            // If we just discovered the item's flavour, mark it as so
            if (ident && !item.IsFlavourAware())
            {
                item.BecomeFlavourAware();
                _player.GainExperience((itemLevel + (_player.Level >> 1)) / _player.Level);
            }
            // If we're a channeler then we should be using vis instead of charges
            bool channeled = false;
            if (_player.Spellcasting.Type == CastingType.Channelling)
            {
                channeled = SaveGame.Instance.CommandEngine.DoCmdChannel(item);
            }
            // We didn't use vis, so decrease the wand's charges
            if (!channeled)
            {
                item.TypeSpecificValue--;
                // If the wand is part of a stack, split it off from the others
                if (itemIndex >= 0 && item.Count > 1)
                {
                    Item splitItem = new Item(item) { Count = 1 };
                    item.TypeSpecificValue++;
                    item.Count--;
                    _player.WeightCarried -= splitItem.Weight;
                    itemIndex = _player.Inventory.InvenCarry(splitItem, false);
                    Profile.Instance.MsgPrint("You unstack your wand.");
                }
                // Let us know we have used a charge
                if (itemIndex >= 0)
                {
                    _player.Inventory.ReportChargeUsageFromInventory(itemIndex);
                }
                else
                {
                    SaveGame.Instance.Level.ReportChargeUsageFromFloor(0 - itemIndex);
                }
            }
        }

        /// <summary>
        /// Eat some food
        /// </summary>
        /// <param name="itemIndex"> The inventory index of the food item </param>
        public void DoCmdEatFood(int itemIndex)
        {
            // Get a food item from the inventory if one wasn't already specified
            Inventory.ItemFilterCategory = ItemCategory.Food;
            if (itemIndex == -999)
            {
                if (!SaveGame.Instance.GetItem(out itemIndex, "Eat which item? ", false, true, true))
                {
                    if (itemIndex == -2)
                    {
                        Profile.Instance.MsgPrint("You have nothing to eat.");
                    }
                    return;
                }
            }
            Item item = itemIndex >= 0 ? _player.Inventory[itemIndex] : _level.Items[0 - itemIndex];
            // Make sure the item is edible
            Inventory.ItemFilterCategory = ItemCategory.Food;
            if (!_player.Inventory.ItemMatchesFilter(item))
            {
                Profile.Instance.MsgPrint("You can't eat that!");
                Inventory.ItemFilterCategory = 0;
                return;
            }
            Inventory.ItemFilterCategory = 0;
            // We don't actually eat dwarf bread
            if (item.ItemSubCategory != FoodType.Dwarfbread)
            {
                Gui.PlaySound(SoundEffect.Eat);
            }
            // Eating costs 100 energy
            SaveGame.Instance.EnergyUse = 100;
            bool ident = false;
            int itemLevel = item.ItemType.Level;
            switch (item.ItemSubCategory)
            {
                case FoodType.Poison:
                    {
                        if (!(_player.HasPoisonResistance || _player.TimedPoisonResistance != 0))
                        {
                            // Hagarg Ryonis may protect us from poison
                            if (Program.Rng.DieRoll(10) <= _player.Religion.GetNamedDeity(Pantheon.GodName.Hagarg_Ryonis).AdjustedFavour)
                            {
                                Profile.Instance.MsgPrint("Hagarg Ryonis's favour protects you!");
                            }
                            else if (_player.SetTimedPoison(_player.TimedPoison + Program.Rng.RandomLessThan(10) + 10))
                            {
                                ident = true;
                            }
                        }
                        break;
                    }
                case FoodType.Blindness:
                    {
                        if (!_player.HasBlindnessResistance)
                        {
                            if (_player.SetTimedBlindness(_player.TimedBlindness + Program.Rng.RandomLessThan(200) + 200))
                            {
                                ident = true;
                            }
                        }
                        break;
                    }
                case FoodType.Paranoia:
                    {
                        if (!_player.HasFearResistance)
                        {
                            if (_player.SetTimedFear(_player.TimedFear + Program.Rng.RandomLessThan(10) + 10))
                            {
                                ident = true;
                            }
                        }
                        break;
                    }
                case FoodType.Confusion:
                    {
                        if (!_player.HasConfusionResistance)
                        {
                            if (_player.SetTimedConfusion(_player.TimedConfusion + Program.Rng.RandomLessThan(10) + 10))
                            {
                                ident = true;
                            }
                        }
                        break;
                    }
                case FoodType.Hallucination:
                    {
                        if (!_player.HasChaosResistance)
                        {
                            if (_player.SetTimedHallucinations(_player.TimedHallucinations + Program.Rng.RandomLessThan(250) + 250))
                            {
                                ident = true;
                            }
                        }
                        break;
                    }
                case FoodType.Paralysis:
                    {
                        if (!_player.HasFreeAction)
                        {
                            if (_player.SetTimedParalysis(_player.TimedParalysis + Program.Rng.RandomLessThan(10) + 10))
                            {
                                ident = true;
                            }
                        }
                        break;
                    }
                case FoodType.Weakness:
                    {
                        _player.TakeHit(Program.Rng.DiceRoll(6, 6), "poisonous food.");
                        _player.TryDecreasingAbilityScore(Ability.Strength);
                        ident = true;
                        break;
                    }
                case FoodType.Sickness:
                    {
                        _player.TakeHit(Program.Rng.DiceRoll(6, 6), "poisonous food.");
                        _player.TryDecreasingAbilityScore(Ability.Constitution);
                        ident = true;
                        break;
                    }
                case FoodType.Stupidity:
                    {
                        _player.TakeHit(Program.Rng.DiceRoll(8, 8), "poisonous food.");
                        _player.TryDecreasingAbilityScore(Ability.Intelligence);
                        ident = true;
                        break;
                    }
                case FoodType.Naivety:
                    {
                        _player.TakeHit(Program.Rng.DiceRoll(8, 8), "poisonous food.");
                        _player.TryDecreasingAbilityScore(Ability.Wisdom);
                        ident = true;
                        break;
                    }
                case FoodType.Unhealth:
                    {
                        _player.TakeHit(Program.Rng.DiceRoll(10, 10), "poisonous food.");
                        _player.TryDecreasingAbilityScore(Ability.Constitution);
                        ident = true;
                        break;
                    }
                case FoodType.Disease:
                    {
                        _player.TakeHit(Program.Rng.DiceRoll(10, 10), "poisonous food.");
                        _player.TryDecreasingAbilityScore(Ability.Strength);
                        ident = true;
                        break;
                    }
                case FoodType.CurePoison:
                    {
                        if (_player.SetTimedPoison(0))
                        {
                            ident = true;
                        }
                        break;
                    }
                case FoodType.CureBlindness:
                    {
                        if (_player.SetTimedBlindness(0))
                        {
                            ident = true;
                        }
                        break;
                    }
                case FoodType.CureParanoia:
                    {
                        if (_player.SetTimedFear(0))
                        {
                            ident = true;
                        }
                        break;
                    }
                case FoodType.CureConfusion:
                    {
                        if (_player.SetTimedConfusion(0))
                        {
                            ident = true;
                        }
                        break;
                    }
                case FoodType.CureSerious:
                    {
                        if (_player.RestoreHealth(Program.Rng.DiceRoll(4, 8)))
                        {
                            ident = true;
                        }
                        break;
                    }
                case FoodType.RestoreStr:
                    {
                        if (_player.TryRestoringAbilityScore(Ability.Strength))
                        {
                            ident = true;
                        }
                        break;
                    }
                case FoodType.RestoreCon:
                    {
                        if (_player.TryRestoringAbilityScore(Ability.Constitution))
                        {
                            ident = true;
                        }
                        break;
                    }
                case FoodType.Restoring:
                    {
                        if (_player.TryRestoringAbilityScore(Ability.Strength))
                        {
                            ident = true;
                        }
                        if (_player.TryRestoringAbilityScore(Ability.Intelligence))
                        {
                            ident = true;
                        }
                        if (_player.TryRestoringAbilityScore(Ability.Wisdom))
                        {
                            ident = true;
                        }
                        if (_player.TryRestoringAbilityScore(Ability.Dexterity))
                        {
                            ident = true;
                        }
                        if (_player.TryRestoringAbilityScore(Ability.Constitution))
                        {
                            ident = true;
                        }
                        if (_player.TryRestoringAbilityScore(Ability.Charisma))
                        {
                            ident = true;
                        }
                        break;
                    }
                case FoodType.Ration:
                case FoodType.Biscuit:
                case FoodType.Venison:
                    {
                        Profile.Instance.MsgPrint("That tastes good.");
                        ident = true;
                        break;
                    }
                case FoodType.Dwarfbread:
                    {
                        Profile.Instance.MsgPrint("You look at the dwarf bread, and don't feel quite so hungry anymore.");
                        ident = true;
                        break;
                    }
                case FoodType.SlimeMold:
                    {
                        SaveGame.Instance.CommandEngine.PotionEffect(PotionType.SlimeMold);
                        ident = true;
                        break;
                    }
                case FoodType.Waybread:
                    {
                        Profile.Instance.MsgPrint("That tastes good.");
                        _player.SetTimedPoison(0);
                        _player.RestoreHealth(Program.Rng.DiceRoll(4, 8));
                        ident = true;
                        break;
                    }
                case FoodType.PintOfAle:
                case FoodType.PintOfWine:
                    {
                        Profile.Instance.MsgPrint("That tastes good.");
                        ident = true;
                        break;
                    }
                case FoodType.Warpstone:
                    {
                        Profile.Instance.MsgPrint("That tastes... funky.");
                        _player.Dna.GainMutation();
                        if (Program.Rng.DieRoll(3) == 1)
                        {
                            _player.Dna.GainMutation();
                        }
                        if (Program.Rng.DieRoll(3) == 1)
                        {
                            _player.Dna.GainMutation();
                        }
                        ident = true;
                        break;
                    }
            }
            _player.NoticeFlags |= Constants.PnCombine | Constants.PnReorder;
            // We've tried this type of object
            item.ObjectTried();
            // Learn its flavour if necessary
            if (ident && !item.IsFlavourAware())
            {
                item.BecomeFlavourAware();
                _player.GainExperience((itemLevel + (_player.Level >> 1)) / _player.Level);
            }
            // Dwarf bread isn't actually eaten so reduce our hunger and return early
            if (item.ItemSubCategory == FoodType.Dwarfbread)
            {
                _player.SetFood(_player.Food + item.TypeSpecificValue);
                return;
            }
            _player.Race.ConsumeFood(_player, item);
            // Use up the item (if it fell to the floor this will have already been dealt with)
            if (itemIndex >= 0)
            {
                _player.Inventory.InvenItemIncrease(itemIndex, -1);
                _player.Inventory.InvenItemDescribe(itemIndex);
                _player.Inventory.InvenItemOptimize(itemIndex);
            }
            else
            {
                SaveGame.Instance.Level.FloorItemIncrease(0 - itemIndex, -1);
                SaveGame.Instance.Level.FloorItemDescribe(0 - itemIndex);
                SaveGame.Instance.Level.FloorItemOptimize(0 - itemIndex);
            }
        }

        /// <summary>
        /// Fire the missile weapon we have in our hand
        /// </summary>
        public void DoCmdFire()
        {
            // Check that we're actually wielding a ranged weapon
            Item missileWeapon = _player.Inventory[InventorySlot.RangedWeapon];
            if (missileWeapon.Category == 0)
            {
                Profile.Instance.MsgPrint("You have nothing to fire with.");
                return;
            }
            // Get the ammunition to fire
            Inventory.ItemFilterCategory = _player.AmmunitionItemCategory;
            if (!SaveGame.Instance.GetItem(out int itemIndex, "Fire which item? ", false, true, true))
            {
                if (itemIndex == -2)
                {
                    Profile.Instance.MsgPrint("You have nothing to fire.");
                }
                return;
            }
            Item ammunitionStack = itemIndex >= 0 ? _player.Inventory[itemIndex] : _level.Items[0 - itemIndex];
            TargetEngine targetEngine = new TargetEngine(_player, _level);
            // Find out where we're aiming at
            if (!targetEngine.GetDirectionWithAim(out int dir))
            {
                return;
            }
            // Copy an ammunition piece from the stack...
            Item individualAmmunition = new Item(ammunitionStack) { Count = 1 };
            // ...and reduced the amount in the stack
            if (itemIndex >= 0)
            {
                _player.Inventory.InvenItemIncrease(itemIndex, -1);
                _player.Inventory.InvenItemDescribe(itemIndex);
                _player.Inventory.InvenItemOptimize(itemIndex);
            }
            else
            {
                SaveGame.Instance.Level.FloorItemIncrease(0 - itemIndex, -1);
                SaveGame.Instance.Level.FloorItemOptimize(0 - itemIndex);
            }
            Gui.PlaySound(SoundEffect.Shoot);
            // Get the details of the shot
            string missileName = individualAmmunition.Description(false, 3);
            Colour missileColour = individualAmmunition.ItemType.Colour;
            char missileCharacter = individualAmmunition.ItemType.Character;
            int shotSpeed = _player.MissileAttacksPerRound;
            int shotDamage = Program.Rng.DiceRoll(individualAmmunition.DamageDice, individualAmmunition.DamageDiceSides) + individualAmmunition.BonusDamage +
                       missileWeapon.BonusDamage;
            int attackBonus = _player.AttackBonus + individualAmmunition.BonusToHit + missileWeapon.BonusToHit;
            int chanceToHit = _player.SkillRanged + (attackBonus * Constants.BthPlusAdj);
            // Damage multiplier depends on weapon
            int damageMultiplier = 1;
            switch (missileWeapon.ItemSubCategory)
            {
                case BowType.Sling:
                    {
                        damageMultiplier = 2;
                        break;
                    }
                case BowType.Shortbow:
                    {
                        damageMultiplier = 2;
                        break;
                    }
                case BowType.Longbow:
                    {
                        damageMultiplier = 3;
                        break;
                    }
                case BowType.LightCrossbow:
                    {
                        damageMultiplier = 3;
                        break;
                    }
                case BowType.HeavyCrossbow:
                    {
                        damageMultiplier = 4;
                        break;
                    }
            }
            // Extra might gives us an increased multiplier
            if (_player.HasExtraMight)
            {
                damageMultiplier++;
            }
            shotDamage *= damageMultiplier;
            // We're actually going to track the shot and draw it square by square
            int shotDistance = 10 + (5 * damageMultiplier);
            // Divide by our shot speed to give the equivalent of x shots per turn
            SaveGame.Instance.EnergyUse = 100 / shotSpeed;
            int y = _player.MapY;
            int x = _player.MapX;
            int targetX = _player.MapX + (99 * _level.KeypadDirectionXOffset[dir]);
            int targetY = _player.MapY + (99 * _level.KeypadDirectionYOffset[dir]);
            // Special case for if we're hitting our own square
            if (dir == 5 && targetEngine.TargetOkay())
            {
                targetX = SaveGame.Instance.TargetCol;
                targetY = SaveGame.Instance.TargetRow;
            }
            SaveGame.Instance.HandleStuff();
            bool hitBody = false;
            // Loop until we've reached our distance or hit something
            for (int curDis = 0; curDis <= shotDistance;)
            {
                if (y == targetY && x == targetX)
                {
                    break;
                }
                // Move a step towards the target
                _level.MoveOneStepTowards(out int newY, out int newX, y, x, _player.MapY, _player.MapX, targetY, targetX);
                // If we were blocked by a wall or something then stop short
                if (!_level.GridPassable(newY, newX))
                {
                    break;
                }
                curDis++;
                x = newX;
                y = newY;
                int msec = GlobalData.DelayFactor * GlobalData.DelayFactor * GlobalData.DelayFactor;
                // If we can see the current projectile location, show it briefly
                if (_level.PanelContains(y, x) && _level.PlayerCanSeeBold(y, x))
                {
                    _level.PrintCharacterAtMapLocation(missileCharacter, missileColour, y, x);
                    _level.MoveCursorRelative(y, x);
                    Gui.Refresh();
                    Gui.Pause(msec);
                    _level.RedrawSingleLocation(y, x);
                    Gui.Refresh();
                }
                else
                {
                    // Pause even if we can't see it so it doesn't look weird if it goes in and out
                    // of sight
                    Gui.Pause(msec);
                }
                // Check if we might hit a monster (not necessarily the one we were aiming at)
                if (_level.Grid[y][x].MonsterIndex != 0)
                {
                    GridTile tile = _level.Grid[y][x];
                    Monster monster = _level.Monsters[tile.MonsterIndex];
                    MonsterRace race = monster.Race;
                    bool visible = monster.IsVisible;
                    hitBody = true;
                    // Check if we actually hit it
                    if (SaveGame.Instance.CommandEngine.PlayerCheckRangedHitOnMonster(chanceToHit - curDis, race.ArmourClass, monster.IsVisible))
                    {
                        string noteDies = " dies.";
                        if ((race.Flags3 & MonsterFlag3.Demon) != 0 || (race.Flags3 & MonsterFlag3.Undead) != 0 ||
                            (race.Flags3 & MonsterFlag3.Cthuloid) != 0 || (race.Flags2 & MonsterFlag2.Stupid) != 0 ||
                            "Evg".Contains(race.Character.ToString()))
                        {
                            noteDies = " is destroyed.";
                        }
                        if (!visible)
                        {
                            Profile.Instance.MsgPrint($"The {missileName} finds a mark.");
                        }
                        else
                        {
                            string monsterName = monster.MonsterDesc(0);
                            Profile.Instance.MsgPrint($"The {missileName} hits {monsterName}.");
                            if (monster.IsVisible)
                            {
                                SaveGame.Instance.HealthTrack(tile.MonsterIndex);
                            }
                            // Note that pets only get angry if they see us and we see them
                            if ((monster.Mind & Constants.SmFriendly) != 0)
                            {
                                monsterName = monster.MonsterDesc(0);
                                Profile.Instance.MsgPrint($"{monsterName} gets angry!");
                                monster.Mind &= ~Constants.SmFriendly;
                            }
                        }
                        // Work out the damage done
                        shotDamage = individualAmmunition.AdjustDamageForMonsterType(shotDamage, monster);
                        shotDamage = SaveGame.Instance.CommandEngine.PlayerCriticalRanged(individualAmmunition.Weight, individualAmmunition.BonusToHit, shotDamage);
                        if (shotDamage < 0)
                        {
                            shotDamage = 0;
                        }
                        if (_level.Monsters.DamageMonster(tile.MonsterIndex, shotDamage, out bool fear, noteDies))
                        {
                            // The monster is dead, so don't add further statuses or messages
                        }
                        else
                        {
                            _level.Monsters.MessagePain(tile.MonsterIndex, shotDamage);
                            if (fear && monster.IsVisible)
                            {
                                Gui.PlaySound(SoundEffect.MonsterFlees);
                                string mName = monster.MonsterDesc(0);
                                Profile.Instance.MsgPrint($"{mName} flees in terror!");
                            }
                        }
                    }
                    // Stop the ammo's travel since we hit something
                    break;
                }
            }
            // If we hit something we have a chance to break the ammo, otherwise it just drops at
            // the end of its travel
            int j = hitBody ? individualAmmunition.BreakageChance() : 0;
            SaveGame.Instance.Level.DropNear(individualAmmunition, j, y, x);
        }

        /// <summary>
        /// Quaff a potion from the inventory or the ground
        /// </summary>
        /// <param name="itemIndex"> The inventory index of the potion to quaff </param>
        public void DoCmdQuaffPotion(int itemIndex)
        {
            // Get an item if we didn't already have one
            Inventory.ItemFilterCategory = ItemCategory.Potion;
            if (itemIndex == -999)
            {
                if (!SaveGame.Instance.GetItem(out itemIndex, "Quaff which potion? ", true, true, true))
                {
                    if (itemIndex == -2)
                    {
                        Profile.Instance.MsgPrint("You have no potions to quaff.");
                    }
                    return;
                }
            }
            Item item = itemIndex >= 0 ? _player.Inventory[itemIndex] : _level.Items[0 - itemIndex];
            // Make sure the item is a potion
            Inventory.ItemFilterCategory = ItemCategory.Potion;
            if (!_player.Inventory.ItemMatchesFilter(item))
            {
                Profile.Instance.MsgPrint("That is not a potion!");
                return;
            }
            Inventory.ItemFilterCategory = 0;
            Gui.PlaySound(SoundEffect.Quaff);
            // Drinking a potion costs a whole turn
            SaveGame.Instance.EnergyUse = 100;
            int itemLevel = item.ItemType.Level;
            // Do the actual potion effect
            bool identified = SaveGame.Instance.CommandEngine.PotionEffect(item.ItemSubCategory);
            // Skeletons are messy drinkers
            if (_player.Race.SpillsPotions && Program.Rng.DieRoll(12) == 1)
            {
                Profile.Instance.MsgPrint("Some of the fluid falls through your jaws!");
                SaveGame.Instance.SpellEffects.PotionSmashEffect(0, _player.MapY, _player.MapX, item.ItemSubCategory);
            }
            _player.NoticeFlags |= Constants.PnCombine | Constants.PnReorder;
            // We may now know the potion's type
            item.ObjectTried();
            if (identified && !item.IsFlavourAware())
            {
                item.BecomeFlavourAware();
                _player.GainExperience((itemLevel + (_player.Level >> 1)) / _player.Level);
            }
            // Most potions give us a bit of food too
            _player.SetFood(_player.Food + item.TypeSpecificValue);
            bool channeled = false;
            // If we're a channeler, we might be able to spend vis instead of using it up
            if (_player.Spellcasting.Type == CastingType.Channelling)
            {
                channeled = SaveGame.Instance.CommandEngine.DoCmdChannel(item);
            }
            if (!channeled)
            {
                // We didn't channel it, so use up one potion from the stack
                if (itemIndex >= 0)
                {
                    _player.Inventory.InvenItemIncrease(itemIndex, -1);
                    _player.Inventory.InvenItemDescribe(itemIndex);
                    _player.Inventory.InvenItemOptimize(itemIndex);
                }
                else
                {
                    SaveGame.Instance.Level.FloorItemIncrease(0 - itemIndex, -1);
                    SaveGame.Instance.Level.FloorItemDescribe(0 - itemIndex);
                    SaveGame.Instance.Level.FloorItemOptimize(0 - itemIndex);
                }
            }
        }

        /// <summary>
        /// Read a scroll from the inventory or floor
        /// </summary>
        /// <param name="itemIndex"> The inventory index of the scroll to be read </param>
        public void DoCmdReadScroll(int itemIndex)
        {
            int i;
            // Make sure we're in a situation where we can read
            if (_player.TimedBlindness != 0)
            {
                Profile.Instance.MsgPrint("You can't see anything.");
                return;
            }
            if (_level.NoLight())
            {
                Profile.Instance.MsgPrint("You have no light to read by.");
                return;
            }
            if (_player.TimedConfusion != 0)
            {
                Profile.Instance.MsgPrint("You are too confused!");
                return;
            }
            // If we weren't passed in an item, prompt for one
            Inventory.ItemFilterCategory = ItemCategory.Scroll;
            if (itemIndex == -999)
            {
                if (!SaveGame.Instance.GetItem(out itemIndex, "Read which scroll? ", true, true, true))
                {
                    if (itemIndex == -2)
                    {
                        Profile.Instance.MsgPrint("You have no scrolls to read.");
                    }
                    return;
                }
            }
            Item item = itemIndex >= 0 ? _player.Inventory[itemIndex] : _level.Items[0 - itemIndex];
            // Make sure the item is actually a scroll
            Inventory.ItemFilterCategory = ItemCategory.Scroll;
            if (!_player.Inventory.ItemMatchesFilter(item))
            {
                Profile.Instance.MsgPrint("That is not a scroll!");
                Inventory.ItemFilterCategory = 0;
                return;
            }
            Inventory.ItemFilterCategory = 0;
            // Scrolls use a full turn
            SaveGame.Instance.EnergyUse = 100;
            bool identified = false;
            int itemLevel = item.ItemType.Level;
            bool usedUp = true;
            // Most types of scroll are obvious
            switch (item.ItemSubCategory)
            {
                case ScrollType.Darkness:
                    {
                        if (!_player.HasBlindnessResistance && !_player.HasDarkResistance)
                        {
                            _player.SetTimedBlindness(_player.TimedBlindness + 3 + Program.Rng.DieRoll(5));
                        }
                        if (SaveGame.Instance.SpellEffects.UnlightArea(10, 3))
                        {
                            identified = true;
                        }
                        break;
                    }
                case ScrollType.AggravateMonster:
                    {
                        Profile.Instance.MsgPrint("There is a high pitched humming noise.");
                        SaveGame.Instance.SpellEffects.AggravateMonsters(1);
                        identified = true;
                        break;
                    }
                case ScrollType.CurseArmour:
                    {
                        if (SaveGame.Instance.CommandEngine.CurseArmour())
                        {
                            identified = true;
                        }
                        break;
                    }
                case ScrollType.CurseWeapon:
                    {
                        if (SaveGame.Instance.CommandEngine.CurseWeapon())
                        {
                            identified = true;
                        }
                        break;
                    }
                case ScrollType.SummonMonster:
                    {
                        for (i = 0; i < Program.Rng.DieRoll(3); i++)
                        {
                            if (_level.Monsters.SummonSpecific(_player.MapY, _player.MapX, SaveGame.Instance.Difficulty, 0))
                            {
                                identified = true;
                            }
                        }
                        break;
                    }
                case ScrollType.SummonUndead:
                    {
                        for (i = 0; i < Program.Rng.DieRoll(3); i++)
                        {
                            if (_level.Monsters.SummonSpecific(_player.MapY, _player.MapX, SaveGame.Instance.Difficulty,
                                Constants.SummonUndead))
                            {
                                identified = true;
                            }
                        }
                        break;
                    }
                case ScrollType.TrapCreation:
                    {
                        if (SaveGame.Instance.SpellEffects.TrapCreation())
                        {
                            identified = true;
                        }
                        break;
                    }
                case ScrollType.PhaseDoor:
                    {
                        SaveGame.Instance.SpellEffects.TeleportPlayer(10);
                        identified = true;
                        break;
                    }
                case ScrollType.Teleport:
                    {
                        SaveGame.Instance.SpellEffects.TeleportPlayer(100);
                        identified = true;
                        break;
                    }
                case ScrollType.TeleportLevel:
                    {
                        SaveGame.Instance.SpellEffects.TeleportPlayerLevel();
                        identified = true;
                        break;
                    }
                case ScrollType.WordOfRecall:
                    {
                        _player.ToggleRecall();
                        identified = true;
                        break;
                    }
                case ScrollType.Identify:
                    {
                        identified = true;
                        if (!SaveGame.Instance.SpellEffects.IdentifyItem())
                        {
                            usedUp = false;
                        }
                        break;
                    }
                case ScrollType.StarIdentify:
                    {
                        identified = true;
                        if (!SaveGame.Instance.SpellEffects.IdentifyFully())
                        {
                            usedUp = false;
                        }
                        break;
                    }
                case ScrollType.RemoveCurse:
                    {
                        if (SaveGame.Instance.SpellEffects.RemoveCurse())
                        {
                            Profile.Instance.MsgPrint("You feel as if someone is watching over you.");
                            identified = true;
                        }
                        break;
                    }
                case ScrollType.StarRemoveCurse:
                    {
                        SaveGame.Instance.SpellEffects.RemoveAllCurse();
                        identified = true;
                        break;
                    }
                case ScrollType.EnchantArmor:
                    {
                        identified = true;
                        if (!SaveGame.Instance.SpellEffects.EnchantSpell(0, 0, 1))
                        {
                            usedUp = false;
                        }
                        break;
                    }
                case ScrollType.EnchantWeaponToHit:
                    {
                        if (!SaveGame.Instance.SpellEffects.EnchantSpell(1, 0, 0))
                        {
                            usedUp = false;
                        }
                        identified = true;
                        break;
                    }
                case ScrollType.EnchantWeaponToDam:
                    {
                        if (!SaveGame.Instance.SpellEffects.EnchantSpell(0, 1, 0))
                        {
                            usedUp = false;
                        }
                        identified = true;
                        break;
                    }
                case ScrollType.StarEnchantArmor:
                    {
                        if (!SaveGame.Instance.SpellEffects.EnchantSpell(0, 0, Program.Rng.DieRoll(3) + 2))
                        {
                            usedUp = false;
                        }
                        identified = true;
                        break;
                    }
                case ScrollType.StarEnchantWeapon:
                    {
                        if (!SaveGame.Instance.SpellEffects.EnchantSpell(Program.Rng.DieRoll(3), Program.Rng.DieRoll(3), 0))
                        {
                            usedUp = false;
                        }
                        identified = true;
                        break;
                    }
                case ScrollType.Recharging:
                    {
                        if (!SaveGame.Instance.SpellEffects.Recharge(60))
                        {
                            usedUp = false;
                        }
                        identified = true;
                        break;
                    }
                case ScrollType.Light:
                    {
                        if (SaveGame.Instance.SpellEffects.LightArea(Program.Rng.DiceRoll(2, 8), 2))
                        {
                            identified = true;
                        }
                        break;
                    }
                case ScrollType.Mapping:
                    {
                        _level.MapArea();
                        identified = true;
                        break;
                    }
                case ScrollType.DetectGold:
                    {
                        if (SaveGame.Instance.SpellEffects.DetectTreasure())
                        {
                            identified = true;
                        }
                        if (SaveGame.Instance.SpellEffects.DetectObjectsGold())
                        {
                            identified = true;
                        }
                        break;
                    }
                case ScrollType.DetectItem:
                    {
                        if (SaveGame.Instance.SpellEffects.DetectObjectsNormal())
                        {
                            identified = true;
                        }
                        break;
                    }
                case ScrollType.DetectTrap:
                    {
                        if (SaveGame.Instance.SpellEffects.DetectTraps())
                        {
                            identified = true;
                        }
                        break;
                    }
                case ScrollType.DetectDoor:
                    {
                        if (SaveGame.Instance.SpellEffects.DetectDoors())
                        {
                            identified = true;
                        }
                        if (SaveGame.Instance.SpellEffects.DetectStairs())
                        {
                            identified = true;
                        }
                        break;
                    }
                case ScrollType.DetectInvis:
                    {
                        if (SaveGame.Instance.SpellEffects.DetectMonstersInvis())
                        {
                            identified = true;
                        }
                        break;
                    }
                case ScrollType.SatisfyHunger:
                    {
                        if (_player.SetFood(Constants.PyFoodMax - 1))
                        {
                            identified = true;
                        }
                        break;
                    }
                case ScrollType.Blessing:
                    {
                        if (_player.SetTimedBlessing(_player.TimedBlessing + Program.Rng.DieRoll(12) + 6))
                        {
                            identified = true;
                        }
                        break;
                    }
                case ScrollType.HolyChant:
                    {
                        if (_player.SetTimedBlessing(_player.TimedBlessing + Program.Rng.DieRoll(24) + 12))
                        {
                            identified = true;
                        }
                        break;
                    }
                case ScrollType.HolyPrayer:
                    {
                        if (_player.SetTimedBlessing(_player.TimedBlessing + Program.Rng.DieRoll(48) + 24))
                        {
                            identified = true;
                        }
                        break;
                    }
                case ScrollType.MonsterConfusion:
                    {
                        if (!_player.HasConfusingTouch)
                        {
                            Profile.Instance.MsgPrint("Your hands begin to glow.");
                            _player.HasConfusingTouch = true;
                            identified = true;
                        }
                        break;
                    }
                case ScrollType.ProtectionFromEvil:
                    {
                        i = 3 * _player.Level;
                        if (_player.SetTimedProtectionFromEvil(_player.TimedProtectionFromEvil + Program.Rng.DieRoll(25) + i))
                        {
                            identified = true;
                        }
                        break;
                    }
                case ScrollType.RuneOfProtection:
                    {
                        SaveGame.Instance.SpellEffects.ElderSign();
                        identified = true;
                        break;
                    }
                case ScrollType.TrapDoorDestruction:
                    {
                        if (SaveGame.Instance.SpellEffects.DestroyDoorsTouch())
                        {
                            identified = true;
                        }
                        break;
                    }
                case ScrollType.StarDestruction:
                    {
                        SaveGame.Instance.SpellEffects.DestroyArea(_player.MapY, _player.MapX, 15);
                        identified = true;
                        break;
                    }
                case ScrollType.DispelUndead:
                    {
                        if (SaveGame.Instance.SpellEffects.DispelUndead(60))
                        {
                            identified = true;
                        }
                        break;
                    }
                case ScrollType.Carnage:
                    {
                        SaveGame.Instance.SpellEffects.Carnage(true);
                        identified = true;
                        break;
                    }
                case ScrollType.MassCarnage:
                    {
                        SaveGame.Instance.SpellEffects.MassCarnage(true);
                        identified = true;
                        break;
                    }
                case ScrollType.Acquirement:
                    {
                        SaveGame.Instance.Level.Acquirement(_player.MapY, _player.MapX, 1, true);
                        identified = true;
                        break;
                    }
                case ScrollType.StarAcquirement:
                    {
                        SaveGame.Instance.Level.Acquirement(_player.MapY, _player.MapX, Program.Rng.DieRoll(2) + 1, true);
                        identified = true;
                        break;
                    }
                case ScrollType.Fire:
                    {
                        SaveGame.Instance.SpellEffects.FireBall(new ProjectFire(SaveGame.Instance.SpellEffects), 0, 150, 4);
                        if (!(_player.TimedFireResistance != 0 || _player.HasFireResistance || _player.HasFireImmunity))
                        {
                            _player.TakeHit(50 + Program.Rng.DieRoll(50), "a Scroll of Fire");
                        }
                        identified = true;
                        break;
                    }
                case ScrollType.Ice:
                    {
                        SaveGame.Instance.SpellEffects.FireBall(new ProjectIce(SaveGame.Instance.SpellEffects), 0, 175, 4);
                        if (!(_player.TimedColdResistance != 0 || _player.HasColdResistance || _player.HasColdImmunity))
                        {
                            _player.TakeHit(100 + Program.Rng.DieRoll(100), "a Scroll of Ice");
                        }
                        identified = true;
                        break;
                    }
                case ScrollType.Chaos:
                    {
                        SaveGame.Instance.SpellEffects.FireBall(new ProjectChaos(SaveGame.Instance.SpellEffects), 0, 222, 4);
                        if (!_player.HasChaosResistance)
                        {
                            _player.TakeHit(111 + Program.Rng.DieRoll(111), "a Scroll of Chaos");
                        }
                        identified = true;
                        break;
                    }
                case ScrollType.Rumor:
                    {
                        Profile.Instance.MsgPrint("There is message on the scroll. It says:");
                        Profile.Instance.MsgPrint(null);
                        SaveGame.Instance.CommandEngine.GetRumour();
                        identified = true;
                        break;
                    }
                case ScrollType.Invocation:
                    {
                        var patron = Patrons.Instance[Patrons.Instance.RandomPatronName()];
                        Profile.Instance.MsgPrint($"You invoke the secret name of {patron.LongName}.");
                        patron.GetReward(_player, SaveGame.Instance.Level, SaveGame.Instance);
                        identified = true;
                        break;
                    }
                case ScrollType.Artifact:
                    {
                        SaveGame.Instance.SpellEffects.ArtifactScroll();
                        identified = true;
                        break;
                    }
            }
            _player.NoticeFlags |= Constants.PnCombine | Constants.PnReorder;
            // We might have just identified the scroll
            item.ObjectTried();
            if (identified && !item.IsFlavourAware())
            {
                item.BecomeFlavourAware();
                _player.GainExperience((itemLevel + (_player.Level >> 1)) / _player.Level);
            }
            bool channeled = false;
            // Channellers can use vis instead of the scroll being used up
            if (_player.Spellcasting.Type == CastingType.Channelling)
            {
                channeled = SaveGame.Instance.CommandEngine.DoCmdChannel(item);
            }
            if (!channeled)
            {
                if (!usedUp)
                {
                    return;
                }
                // If it wasn't used up then decrease the amount in the stack
                if (itemIndex >= 0)
                {
                    _player.Inventory.InvenItemIncrease(itemIndex, -1);
                    _player.Inventory.InvenItemDescribe(itemIndex);
                    _player.Inventory.InvenItemOptimize(itemIndex);
                }
                else
                {
                    SaveGame.Instance.Level.FloorItemIncrease(0 - itemIndex, -1);
                    SaveGame.Instance.Level.FloorItemDescribe(0 - itemIndex);
                    SaveGame.Instance.Level.FloorItemOptimize(0 - itemIndex);
                }
            }
        }

        /// <summary>
        /// Refill a light source with fuel
        /// </summary>
        /// <param name="itemIndex"> The inventory index of the fuel to be used </param>
        public void DoCmdRefill(int itemIndex)
        {
            // Make sure we actually have a light source to refuel
            Item lightSource = _player.Inventory[InventorySlot.Lightsource];
            if (lightSource.Category != ItemCategory.Light)
            {
                Profile.Instance.MsgPrint("You are not wielding a light.");
            }
            else if (lightSource.ItemSubCategory == LightType.Lantern)
            {
                RefillLamp(itemIndex);
            }
            else if (lightSource.ItemSubCategory == LightType.Torch)
            {
                RefillTorch(itemIndex);
            }
            else
            {
                Profile.Instance.MsgPrint("Your light cannot be refilled.");
            }
        }

        /// <summary>
        /// Spike a door closed
        /// </summary>
        public void DoCmdSpike()
        {
            // Get the location to be spiked
            TargetEngine targetEngine = new TargetEngine(_player, _level);
            if (targetEngine.GetDirectionNoAim(out int dir))
            {
                int y = _player.MapY + _level.KeypadDirectionYOffset[dir];
                int x = _player.MapX + _level.KeypadDirectionXOffset[dir];
                GridTile tile = _level.Grid[y][x];
                // Make sure it can be spiked and we have spikes to do it with
                if (!tile.FeatureType.IsClosedDoor)
                {
                    Profile.Instance.MsgPrint("You see nothing there to spike.");
                }
                else
                {
                    if (!SaveGame.Instance.CommandEngine.GetSpike(out int itemIndex))
                    {
                        Profile.Instance.MsgPrint("You have no spikes!");
                    }
                    // Can't close a door if there's someone in the way
                    else if (tile.MonsterIndex != 0)
                    {
                        // Attempting costs a turn anyway
                        SaveGame.Instance.EnergyUse = 100;
                        Profile.Instance.MsgPrint("There is a monster in the way!");
                        SaveGame.Instance.CommandEngine.PlayerAttackMonster(y, x);
                    }
                    else
                    {
                        // Spiking a door costs a turn
                        SaveGame.Instance.EnergyUse = 100;
                        Profile.Instance.MsgPrint("You jam the door with a spike.");
                        // Replace the door feature with a jammed door
                        if (tile.FeatureType.Category == FloorTileTypeCategory.LockedDoor)
                        {
                            tile.SetFeature(tile.FeatureType.Name.Replace("Locked", "Jammed"));
                        }
                        // If it's already jammed, strengthen it
                        if (tile.FeatureType.Category == FloorTileTypeCategory.JammedDoor)
                        {
                            int strength = int.Parse(tile.FeatureType.Name.Substring(10));
                            if (strength < 7)
                            {
                                tile.SetFeature($"JammedDoor{strength + 1}");
                            }
                        }
                        // Use up the spike from the player's inventory
                        _player.Inventory.InvenItemIncrease(itemIndex, -1);
                        _player.Inventory.InvenItemDescribe(itemIndex);
                        _player.Inventory.InvenItemOptimize(itemIndex);
                    }
                }
            }
        }

        /// <summary>
        /// Throw an item
        /// </summary>
        public void DoCmdThrow(int damageMultiplier)
        {
            // Get an item to throw
            if (!SaveGame.Instance.GetItem(out int itemIndex, "Throw which item? ", false, true, true))
            {
                if (itemIndex == -2)
                {
                    Profile.Instance.MsgPrint("You have nothing to throw.");
                }
                return;
            }
            Item item = itemIndex >= 0 ? _player.Inventory[itemIndex] : _level.Items[0 - itemIndex];
            TargetEngine targetEngine = new TargetEngine(_player, _level);
            if (!targetEngine.GetDirectionWithAim(out int dir))
            {
                return;
            }
            // Copy a single item from the item stack as the thrown item
            Item missile = new Item(item) { Count = 1 };
            if (itemIndex >= 0)
            {
                _player.Inventory.InvenItemIncrease(itemIndex, -1);
                _player.Inventory.InvenItemDescribe(itemIndex);
                _player.Inventory.InvenItemOptimize(itemIndex);
            }
            else
            {
                SaveGame.Instance.Level.FloorItemIncrease(0 - itemIndex, -1);
                SaveGame.Instance.Level.FloorItemOptimize(0 - itemIndex);
            }
            string missileName = missile.Description(false, 3);
            Colour missileColour = missile.ItemType.Colour;
            char missileCharacter = missile.ItemType.Character;
            // Thrown distance is based on the weight of the missile
            int multiplier = 10 + (2 * (damageMultiplier - 1));
            int divider = missile.Weight > 10 ? missile.Weight : 10;
            int throwDistance = (_player.AbilityScores[Ability.Strength].StrAttackSpeedComponent + 20) * multiplier / divider;
            if (throwDistance > 10)
            {
                throwDistance = 10;
            }
            // Work out the damage done
            int damage = Program.Rng.DiceRoll(missile.DamageDice, missile.DamageDiceSides) + missile.BonusDamage;
            damage *= damageMultiplier;
            int chance = _player.SkillThrowing + (_player.AttackBonus * Constants.BthPlusAdj);
            // Throwing something always uses a full turn, even if you can make multiple missile attacks
            SaveGame.Instance.EnergyUse = 100;
            int y = _player.MapY;
            int x = _player.MapX;
            int targetX = _player.MapX + (99 * _level.KeypadDirectionXOffset[dir]);
            int targetY = _player.MapY + (99 * _level.KeypadDirectionYOffset[dir]);
            if (dir == 5 && targetEngine.TargetOkay())
            {
                targetX = SaveGame.Instance.TargetCol;
                targetY = SaveGame.Instance.TargetRow;
            }
            SaveGame.Instance.HandleStuff();
            int newY = _player.MapY;
            int newX = _player.MapX;
            bool hitBody = false;
            // Send the thrown object in the right direction one square at a time
            for (int curDis = 0; curDis <= throwDistance;)
            {
                // If we reach our limit, stop the object moving
                if (y == targetY && x == targetX)
                {
                    break;
                }
                _level.MoveOneStepTowards(out newY, out newX, y, x, _player.MapY, _player.MapX, targetY, targetX);
                // If we hit a wall or something stop moving
                if (!_level.GridPassable(newY, newX))
                {
                    break;
                }
                curDis++;
                x = newX;
                y = newY;
                const int msec = GlobalData.DelayFactor * GlobalData.DelayFactor * GlobalData.DelayFactor;
                // If we can see, display the thrown item with a suitable delay
                if (_level.PanelContains(y, x) && _level.PlayerCanSeeBold(y, x))
                {
                    _level.PrintCharacterAtMapLocation(missileCharacter, missileColour, y, x);
                    _level.MoveCursorRelative(y, x);
                    Gui.Refresh();
                    Gui.Pause(msec);
                    _level.RedrawSingleLocation(y, x);
                    Gui.Refresh();
                }
                else
                {
                    // Delay even if we don't see it, so it doesn't look weird when it passes behind something
                    Gui.Pause(msec);
                }
                // If there's a monster in the way, we might hit it regardless of whether or not it
                // is our intended target
                if (_level.Grid[y][x].MonsterIndex != 0)
                {
                    GridTile tile = _level.Grid[y][x];
                    Monster monster = _level.Monsters[tile.MonsterIndex];
                    MonsterRace race = monster.Race;
                    bool visible = monster.IsVisible;
                    hitBody = true;
                    // See if it actually hit the monster
                    if (SaveGame.Instance.CommandEngine.PlayerCheckRangedHitOnMonster(chance - curDis, race.ArmourClass, monster.IsVisible))
                    {
                        string noteDies = " dies.";
                        if ((race.Flags3 & MonsterFlag3.Demon) != 0 || (race.Flags3 & MonsterFlag3.Undead) != 0 ||
                            (race.Flags3 & MonsterFlag3.Cthuloid) != 0 || (race.Flags2 & MonsterFlag2.Stupid) != 0 ||
                            "Evg".Contains(race.Character.ToString()))
                        {
                            noteDies = " is destroyed.";
                        }
                        // Let the player know what happened
                        if (!visible)
                        {
                            Profile.Instance.MsgPrint($"The {missileName} finds a mark.");
                        }
                        else
                        {
                            string mName = monster.MonsterDesc(0);
                            Profile.Instance.MsgPrint($"The {missileName} hits {mName}.");
                            if (monster.IsVisible)
                            {
                                SaveGame.Instance.HealthTrack(tile.MonsterIndex);
                            }
                        }
                        // Adjust the damage for the particular monster type
                        damage = missile.AdjustDamageForMonsterType(damage, monster);
                        damage = SaveGame.Instance.CommandEngine.PlayerCriticalRanged(missile.Weight, missile.BonusToHit, damage);
                        if (damage < 0)
                        {
                            damage = 0;
                        }
                        if (_level.Monsters.DamageMonster(tile.MonsterIndex, damage, out bool fear, noteDies))
                        {
                            // The monster is dead, so don't add further statuses or messages
                        }
                        else
                        {
                            // Let the player know what happens to the monster
                            _level.Monsters.MessagePain(tile.MonsterIndex, damage);
                            if ((monster.Mind & Constants.SmFriendly) != 0 &&
                                missile.ItemType.Category != ItemCategory.Potion)
                            {
                                string mName = monster.MonsterDesc(0);
                                Profile.Instance.MsgPrint($"{mName} gets angry!");
                                monster.Mind &= ~Constants.SmFriendly;
                            }
                            if (fear && monster.IsVisible)
                            {
                                Gui.PlaySound(SoundEffect.MonsterFlees);
                                string mName = monster.MonsterDesc(0);
                                Profile.Instance.MsgPrint($"{mName} flees in terror!");
                            }
                        }
                    }
                    break;
                }
            }
            // There's a chance of breakage if we hit a creature
            int chanceToBreak = hitBody ? missile.BreakageChance() : 0;
            // If we hit with a potion, the potion might affect the creature
            if (missile.ItemType.Category == ItemCategory.Potion)
            {
                if (hitBody || !_level.GridPassable(newY, newX) || Program.Rng.DieRoll(100) < chanceToBreak)
                {
                    Profile.Instance.MsgPrint($"The {missileName} shatters!");
                    if (SaveGame.Instance.SpellEffects.PotionSmashEffect(1, y, x, missile.ItemSubCategory))
                    {
                        if (_level.Grid[y][x].MonsterIndex != 0 &&
                            (_level.Monsters[_level.Grid[y][x].MonsterIndex].Mind & Constants.SmFriendly) != 0)
                        {
                            string mName = _level.Monsters[_level.Grid[y][x].MonsterIndex].MonsterDesc(0);
                            Profile.Instance.MsgPrint($"{mName} gets angry!");
                            _level.Monsters[_level.Grid[y][x].MonsterIndex].Mind &= ~Constants.SmFriendly;
                        }
                    }
                    return;
                }
                chanceToBreak = 0;
            }
            // Drop the item on the floor
            SaveGame.Instance.Level.DropNear(missile, chanceToBreak, y, x);
        }

        /// <summary>
        /// Use a staff from the inventory or floor
        /// </summary>
        /// <param name="itemIndex"> The inventory index of the item to use </param>
        public void DoCmdUseStaff(int itemIndex)
        {
            // Get an item if we weren't passed one
            Inventory.ItemFilterCategory = ItemCategory.Staff;
            if (itemIndex == -999)
            {
                if (!SaveGame.Instance.GetItem(out itemIndex, "Use which staff? ", false, true, true))
                {
                    if (itemIndex == -2)
                    {
                        Profile.Instance.MsgPrint("You have no staff to use.");
                    }
                    return;
                }
            }
            Item item = itemIndex >= 0 ? _player.Inventory[itemIndex] : _level.Items[0 - itemIndex];
            // Make sure the item is actually a staff
            Inventory.ItemFilterCategory = ItemCategory.Staff;
            if (!_player.Inventory.ItemMatchesFilter(item))
            {
                Profile.Instance.MsgPrint("That is not a staff!");
                Inventory.ItemFilterCategory = 0;
                return;
            }
            Inventory.ItemFilterCategory = 0;
            // We can't use a staff from the floor
            if (itemIndex < 0 && item.Count > 1)
            {
                Profile.Instance.MsgPrint("You must first pick up the staffs.");
                return;
            }
            // Using a staff costs a full turn
            SaveGame.Instance.EnergyUse = 100;
            bool identified = false;
            int itemLevel = item.ItemType.Level;
            // We have a chance of the device working equal to skill (halved if confused) - item
            // level (capped at 50)
            int chance = _player.SkillUseDevice;
            if (_player.TimedConfusion != 0)
            {
                chance /= 2;
            }
            chance -= itemLevel > 50 ? 50 : itemLevel;
            // Always a small chance of it working
            if (chance < Constants.UseDevice && Program.Rng.RandomLessThan(Constants.UseDevice - chance + 1) == 0)
            {
                chance = Constants.UseDevice;
            }
            // Check to see if we use it properly
            if (chance < Constants.UseDevice || Program.Rng.DieRoll(chance) < Constants.UseDevice)
            {
                Profile.Instance.MsgPrint("You failed to use the staff properly.");
                return;
            }
            // Make sure it has charges left
            if (item.TypeSpecificValue <= 0)
            {
                Profile.Instance.MsgPrint("The staff has no charges left.");
                item.IdentifyFlags.Set(Constants.IdentEmpty);
                return;
            }
            Gui.PlaySound(SoundEffect.UseStaff);
            int k;
            bool useCharge = true;
            // Do the specific effect for the type of staff
            switch (item.ItemSubCategory)
            {
                case StaffType.Darkness:
                    {
                        if (!_player.HasBlindnessResistance && !_player.HasDarkResistance)
                        {
                            if (_player.SetTimedBlindness(_player.TimedBlindness + 3 + Program.Rng.DieRoll(5)))
                            {
                                identified = true;
                            }
                        }
                        if (SaveGame.Instance.SpellEffects.UnlightArea(10, 3))
                        {
                            identified = true;
                        }
                        break;
                    }
                case StaffType.Slowness:
                    {
                        if (_player.SetTimedSlow(_player.TimedSlow + Program.Rng.DieRoll(30) + 15))
                        {
                            identified = true;
                        }
                        break;
                    }
                case StaffType.HasteMonsters:
                    {
                        if (SaveGame.Instance.SpellEffects.HasteMonsters())
                        {
                            identified = true;
                        }
                        break;
                    }
                case StaffType.Summoning:
                    {
                        for (k = 0; k < Program.Rng.DieRoll(4); k++)
                        {
                            if (_level.Monsters.SummonSpecific(_player.MapY, _player.MapX, SaveGame.Instance.Difficulty, 0))
                            {
                                identified = true;
                            }
                        }
                        break;
                    }
                case StaffType.Teleportation:
                    {
                        SaveGame.Instance.SpellEffects.TeleportPlayer(100);
                        identified = true;
                        break;
                    }
                case StaffType.Identify:
                    {
                        if (!SaveGame.Instance.SpellEffects.IdentifyItem())
                        {
                            useCharge = false;
                        }
                        identified = true;
                        break;
                    }
                case StaffType.RemoveCurse:
                    {
                        if (SaveGame.Instance.SpellEffects.RemoveCurse())
                        {
                            if (_player.TimedBlindness == 0)
                            {
                                Profile.Instance.MsgPrint("The staff glows blue for a moment...");
                            }
                            identified = true;
                        }
                        break;
                    }
                case StaffType.Starlight:
                    {
                        if (_player.TimedBlindness == 0)
                        {
                            Profile.Instance.MsgPrint("The end of the staff glows brightly...");
                        }
                        for (k = 0; k < 8; k++)
                        {
                            SaveGame.Instance.SpellEffects.LightLine(_level.OrderedDirection[k]);
                        }
                        identified = true;
                        break;
                    }
                case StaffType.Light:
                    {
                        if (SaveGame.Instance.SpellEffects.LightArea(Program.Rng.DiceRoll(2, 8), 2))
                        {
                            identified = true;
                        }
                        break;
                    }
                case StaffType.Mapping:
                    {
                        _level.MapArea();
                        identified = true;
                        break;
                    }
                case StaffType.DetectGold:
                    {
                        if (SaveGame.Instance.SpellEffects.DetectTreasure())
                        {
                            identified = true;
                        }
                        if (SaveGame.Instance.SpellEffects.DetectObjectsGold())
                        {
                            identified = true;
                        }
                        break;
                    }
                case StaffType.DetectItem:
                    {
                        if (SaveGame.Instance.SpellEffects.DetectObjectsNormal())
                        {
                            identified = true;
                        }
                        break;
                    }
                case StaffType.DetectTrap:
                    {
                        if (SaveGame.Instance.SpellEffects.DetectTraps())
                        {
                            identified = true;
                        }
                        break;
                    }
                case StaffType.DetectDoor:
                    {
                        if (SaveGame.Instance.SpellEffects.DetectDoors())
                        {
                            identified = true;
                        }
                        if (SaveGame.Instance.SpellEffects.DetectStairs())
                        {
                            identified = true;
                        }
                        break;
                    }
                case StaffType.DetectInvis:
                    {
                        if (SaveGame.Instance.SpellEffects.DetectMonstersInvis())
                        {
                            identified = true;
                        }
                        break;
                    }
                case StaffType.DetectEvil:
                    {
                        if (SaveGame.Instance.SpellEffects.DetectMonstersEvil())
                        {
                            identified = true;
                        }
                        break;
                    }
                case StaffType.CureLight:
                    {
                        if (_player.RestoreHealth(Program.Rng.DieRoll(8)))
                        {
                            identified = true;
                        }
                        break;
                    }
                case StaffType.Curing:
                    {
                        if (_player.SetTimedBlindness(0))
                        {
                            identified = true;
                        }
                        if (_player.SetTimedPoison(0))
                        {
                            identified = true;
                        }
                        if (_player.SetTimedConfusion(0))
                        {
                            identified = true;
                        }
                        if (_player.SetTimedStun(0))
                        {
                            identified = true;
                        }
                        if (_player.SetTimedBleeding(0))
                        {
                            identified = true;
                        }
                        if (_player.SetTimedHallucinations(0))
                        {
                            identified = true;
                        }
                        break;
                    }
                case StaffType.Healing:
                    {
                        if (_player.RestoreHealth(300))
                        {
                            identified = true;
                        }
                        if (_player.SetTimedStun(0))
                        {
                            identified = true;
                        }
                        if (_player.SetTimedBleeding(0))
                        {
                            identified = true;
                        }
                        break;
                    }
                case StaffType.TheMagi:
                    {
                        if (_player.TryRestoringAbilityScore(Ability.Intelligence))
                        {
                            identified = true;
                        }
                        if (_player.Vis < _player.MaxVis)
                        {
                            _player.Vis = _player.MaxVis;
                            _player.FractionalVis = 0;
                            identified = true;
                            Profile.Instance.MsgPrint("Your feel your head clear.");
                            _player.RedrawNeeded.Set(RedrawFlag.PrVis);
                        }
                        break;
                    }
                case StaffType.SleepMonsters:
                    {
                        if (SaveGame.Instance.SpellEffects.SleepMonsters())
                        {
                            identified = true;
                        }
                        break;
                    }
                case StaffType.SlowMonsters:
                    {
                        if (SaveGame.Instance.SpellEffects.SlowMonsters())
                        {
                            identified = true;
                        }
                        break;
                    }
                case StaffType.Speed:
                    {
                        if (_player.TimedHaste == 0)
                        {
                            if (_player.SetTimedHaste(Program.Rng.DieRoll(30) + 15))
                            {
                                identified = true;
                            }
                        }
                        else
                        {
                            _player.SetTimedHaste(_player.TimedHaste + 5);
                        }
                        break;
                    }
                case StaffType.Probing:
                    {
                        SaveGame.Instance.SpellEffects.Probing();
                        identified = true;
                        break;
                    }
                case StaffType.DispelEvil:
                    {
                        if (SaveGame.Instance.SpellEffects.DispelEvil(60))
                        {
                            identified = true;
                        }
                        break;
                    }
                case StaffType.Power:
                    {
                        if (SaveGame.Instance.SpellEffects.DispelMonsters(120))
                        {
                            identified = true;
                        }
                        break;
                    }
                case StaffType.Holiness:
                    {
                        if (SaveGame.Instance.SpellEffects.DispelEvil(120))
                        {
                            identified = true;
                        }
                        k = 3 * _player.Level;
                        if (_player.SetTimedProtectionFromEvil(_player.TimedProtectionFromEvil + Program.Rng.DieRoll(25) + k))
                        {
                            identified = true;
                        }
                        if (_player.SetTimedPoison(0))
                        {
                            identified = true;
                        }
                        if (_player.SetTimedFear(0))
                        {
                            identified = true;
                        }
                        if (_player.RestoreHealth(50))
                        {
                            identified = true;
                        }
                        if (_player.SetTimedStun(0))
                        {
                            identified = true;
                        }
                        if (_player.SetTimedBleeding(0))
                        {
                            identified = true;
                        }
                        break;
                    }
                case StaffType.Carnage:
                    {
                        SaveGame.Instance.SpellEffects.Carnage(true);
                        identified = true;
                        break;
                    }
                case StaffType.Earthquakes:
                    {
                        SaveGame.Instance.SpellEffects.Earthquake(_player.MapY, _player.MapX, 10);
                        identified = true;
                        break;
                    }
                case StaffType.Destruction:
                    {
                        SaveGame.Instance.SpellEffects.DestroyArea(_player.MapY, _player.MapX, 15);
                        identified = true;
                        break;
                    }
            }
            _player.NoticeFlags |= Constants.PnCombine | Constants.PnReorder;
            // We might now know what the staff does
            item.ObjectTried();
            if (identified && !item.IsFlavourAware())
            {
                item.BecomeFlavourAware();
                _player.GainExperience((itemLevel + (_player.Level >> 1)) / _player.Level);
            }
            // We may not have used up a charge
            if (!useCharge)
            {
                return;
            }
            // Channellers can use vis instead of a charge
            bool channeled = false;
            if (_player.Spellcasting.Type == CastingType.Channelling)
            {
                channeled = SaveGame.Instance.CommandEngine.DoCmdChannel(item);
            }
            if (!channeled)
            {
                // Use the actual charge
                item.TypeSpecificValue--;
                // If the staff was part of a stack, separate it from the rest
                if (itemIndex >= 0 && item.Count > 1)
                {
                    Item singleStaff = new Item(item) { Count = 1 };
                    item.TypeSpecificValue++;
                    item.Count--;
                    _player.WeightCarried -= singleStaff.Weight;
                    itemIndex = _player.Inventory.InvenCarry(singleStaff, false);
                    Profile.Instance.MsgPrint("You unstack your staff.");
                }
                // Let the player know what happened
                if (itemIndex >= 0)
                {
                    _player.Inventory.ReportChargeUsageFromInventory(itemIndex);
                }
                else
                {
                    SaveGame.Instance.Level.ReportChargeUsageFromFloor(0 - itemIndex);
                }
            }
        }

        /// <summary>
        /// Use a rod from the inventory or ground
        /// </summary>
        /// <param name="itemIndex"> The inventory index of the item to be used </param>
        public void DoCmdZapRod(int itemIndex)
        {
            // Get the item if we weren't passed it
            Inventory.ItemFilterCategory = ItemCategory.Rod;
            if (itemIndex == -999)
            {
                if (!SaveGame.Instance.GetItem(out itemIndex, "Zap which rod? ", false, true, true))
                {
                    if (itemIndex == -2)
                    {
                        Profile.Instance.MsgPrint("You have no rod to zap.");
                    }
                    return;
                }
            }
            Item item = itemIndex >= 0 ? _player.Inventory[itemIndex] : _level.Items[0 - itemIndex];
            // Make sure the item is actually a rod
            Inventory.ItemFilterCategory = ItemCategory.Rod;
            if (!_player.Inventory.ItemMatchesFilter(item))
            {
                Profile.Instance.MsgPrint("That is not a rod!");
                Inventory.ItemFilterCategory = 0;
                return;
            }
            Inventory.ItemFilterCategory = 0;
            // Rods can't be used from the floor
            if (itemIndex < 0 && item.Count > 1)
            {
                Profile.Instance.MsgPrint("You must first pick up the rods.");
                return;
            }
            // We may need to aim the rod
            int dir = 5;
            if ((item.ItemSubCategory >= RodType.MinimumAimed && item.ItemSubCategory != RodType.Havoc) ||
                !item.IsFlavourAware())
            {
                TargetEngine targetEngine = new TargetEngine(_player, _level);
                if (!targetEngine.GetDirectionWithAim(out dir))
                {
                    return;
                }
            }
            // Using a rod takes a whole turn
            SaveGame.Instance.EnergyUse = 100;
            bool identified = false;
            int itemLevel = item.ItemType.Level;
            // Chance to successfully use it is skill (halved if confused) - rod level (capped at 50)
            int chance = _player.SkillUseDevice;
            if (_player.TimedConfusion != 0)
            {
                chance /= 2;
            }
            chance -= itemLevel > 50 ? 50 : itemLevel;
            // There's always a small chance of success
            if (chance < Constants.UseDevice && Program.Rng.RandomLessThan(Constants.UseDevice - chance + 1) == 0)
            {
                chance = Constants.UseDevice;
            }
            // Do the actual check
            if (chance < Constants.UseDevice || Program.Rng.DieRoll(chance) < Constants.UseDevice)
            {
                Profile.Instance.MsgPrint("You failed to use the rod properly.");
                return;
            }
            // Rods only have a single charge but recharge over time
            if (item.TypeSpecificValue != 0)
            {
                Profile.Instance.MsgPrint("The rod is still charging.");
                return;
            }
            Gui.PlaySound(SoundEffect.ZapRod);
            // Do the rod-specific effect
            bool useCharge = true;
            switch (item.ItemSubCategory)
            {
                case RodType.DetectTrap:
                    {
                        if (SaveGame.Instance.SpellEffects.DetectTraps())
                        {
                            identified = true;
                        }
                        item.TypeSpecificValue = 10 + Program.Rng.DieRoll(10);
                        break;
                    }
                case RodType.DetectDoor:
                    {
                        if (SaveGame.Instance.SpellEffects.DetectDoors())
                        {
                            identified = true;
                        }
                        if (SaveGame.Instance.SpellEffects.DetectStairs())
                        {
                            identified = true;
                        }
                        item.TypeSpecificValue = 70;
                        break;
                    }
                case RodType.Identify:
                    {
                        identified = true;
                        if (!SaveGame.Instance.SpellEffects.IdentifyItem())
                        {
                            useCharge = false;
                        }
                        item.TypeSpecificValue = 10;
                        break;
                    }
                case RodType.Recall:
                    {
                        _player.ToggleRecall();
                        identified = true;
                        item.TypeSpecificValue = 60;
                        break;
                    }
                case RodType.Illumination:
                    {
                        if (SaveGame.Instance.SpellEffects.LightArea(Program.Rng.DiceRoll(2, 8), 2))
                        {
                            identified = true;
                        }
                        item.TypeSpecificValue = 10 + Program.Rng.DieRoll(11);
                        break;
                    }
                case RodType.Mapping:
                    {
                        _level.MapArea();
                        identified = true;
                        item.TypeSpecificValue = 99;
                        break;
                    }
                case RodType.Detection:
                    {
                        SaveGame.Instance.SpellEffects.DetectAll();
                        identified = true;
                        item.TypeSpecificValue = 99;
                        break;
                    }
                case RodType.Probing:
                    {
                        SaveGame.Instance.SpellEffects.Probing();
                        identified = true;
                        item.TypeSpecificValue = 50;
                        break;
                    }
                case RodType.Curing:
                    {
                        if (_player.SetTimedBlindness(0))
                        {
                            identified = true;
                        }
                        if (_player.SetTimedPoison(0))
                        {
                            identified = true;
                        }
                        if (_player.SetTimedConfusion(0))
                        {
                            identified = true;
                        }
                        if (_player.SetTimedStun(0))
                        {
                            identified = true;
                        }
                        if (_player.SetTimedBleeding(0))
                        {
                            identified = true;
                        }
                        if (_player.SetTimedHallucinations(0))
                        {
                            identified = true;
                        }
                        item.TypeSpecificValue = 999;
                        break;
                    }
                case RodType.Healing:
                    {
                        if (_player.RestoreHealth(500))
                        {
                            identified = true;
                        }
                        if (_player.SetTimedStun(0))
                        {
                            identified = true;
                        }
                        if (_player.SetTimedBleeding(0))
                        {
                            identified = true;
                        }
                        item.TypeSpecificValue = 999;
                        break;
                    }
                case RodType.Restoration:
                    {
                        if (_player.RestoreLevel())
                        {
                            identified = true;
                        }
                        if (_player.TryRestoringAbilityScore(Ability.Strength))
                        {
                            identified = true;
                        }
                        if (_player.TryRestoringAbilityScore(Ability.Intelligence))
                        {
                            identified = true;
                        }
                        if (_player.TryRestoringAbilityScore(Ability.Wisdom))
                        {
                            identified = true;
                        }
                        if (_player.TryRestoringAbilityScore(Ability.Dexterity))
                        {
                            identified = true;
                        }
                        if (_player.TryRestoringAbilityScore(Ability.Constitution))
                        {
                            identified = true;
                        }
                        if (_player.TryRestoringAbilityScore(Ability.Charisma))
                        {
                            identified = true;
                        }
                        item.TypeSpecificValue = 999;
                        break;
                    }
                case RodType.Speed:
                    {
                        if (_player.TimedHaste == 0)
                        {
                            if (_player.SetTimedHaste(Program.Rng.DieRoll(30) + 15))
                            {
                                identified = true;
                            }
                        }
                        else
                        {
                            _player.SetTimedHaste(_player.TimedHaste + 5);
                        }
                        item.TypeSpecificValue = 99;
                        break;
                    }
                case RodType.TeleportAway:
                    {
                        if (SaveGame.Instance.SpellEffects.TeleportMonster(dir))
                        {
                            identified = true;
                        }
                        item.TypeSpecificValue = 25;
                        break;
                    }
                case RodType.Disarming:
                    {
                        if (SaveGame.Instance.SpellEffects.DisarmTrap(dir))
                        {
                            identified = true;
                        }
                        item.TypeSpecificValue = 15 + Program.Rng.DieRoll(15);
                        break;
                    }
                case RodType.Light:
                    {
                        Profile.Instance.MsgPrint("A line of blue shimmering light appears.");
                        SaveGame.Instance.SpellEffects.LightLine(dir);
                        identified = true;
                        item.TypeSpecificValue = 9;
                        break;
                    }
                case RodType.SleepMonster:
                    {
                        if (SaveGame.Instance.SpellEffects.SleepMonster(dir))
                        {
                            identified = true;
                        }
                        item.TypeSpecificValue = 18;
                        break;
                    }
                case RodType.SlowMonster:
                    {
                        if (SaveGame.Instance.SpellEffects.SlowMonster(dir))
                        {
                            identified = true;
                        }
                        item.TypeSpecificValue = 20;
                        break;
                    }
                case RodType.DrainLife:
                    {
                        if (SaveGame.Instance.SpellEffects.DrainLife(dir, 75))
                        {
                            identified = true;
                        }
                        item.TypeSpecificValue = 23;
                        break;
                    }
                case RodType.Polymorph:
                    {
                        if (SaveGame.Instance.SpellEffects.PolyMonster(dir))
                        {
                            identified = true;
                        }
                        item.TypeSpecificValue = 25;
                        break;
                    }
                case RodType.AcidBolt:
                    {
                        SaveGame.Instance.SpellEffects.FireBoltOrBeam(10, new ProjectAcid(SaveGame.Instance.SpellEffects), dir,
                            Program.Rng.DiceRoll(6, 8));
                        identified = true;
                        item.TypeSpecificValue = 12;
                        break;
                    }
                case RodType.ElecBolt:
                    {
                        SaveGame.Instance.SpellEffects.FireBoltOrBeam(10, new ProjectElectricity(SaveGame.Instance.SpellEffects), dir,
                            Program.Rng.DiceRoll(3, 8));
                        identified = true;
                        item.TypeSpecificValue = 11;
                        break;
                    }
                case RodType.FireBolt:
                    {
                        SaveGame.Instance.SpellEffects.FireBoltOrBeam(10, new ProjectFire(SaveGame.Instance.SpellEffects), dir,
                            Program.Rng.DiceRoll(8, 8));
                        identified = true;
                        item.TypeSpecificValue = 15;
                        break;
                    }
                case RodType.ColdBolt:
                    {
                        SaveGame.Instance.SpellEffects.FireBoltOrBeam(10, new ProjectCold(SaveGame.Instance.SpellEffects), dir,
                            Program.Rng.DiceRoll(5, 8));
                        identified = true;
                        item.TypeSpecificValue = 13;
                        break;
                    }
                case RodType.AcidBall:
                    {
                        SaveGame.Instance.SpellEffects.FireBall(new ProjectAcid(SaveGame.Instance.SpellEffects), dir, 60, 2);
                        identified = true;
                        item.TypeSpecificValue = 27;
                        break;
                    }
                case RodType.ElecBall:
                    {
                        SaveGame.Instance.SpellEffects.FireBall(new ProjectElectricity(SaveGame.Instance.SpellEffects), dir, 32, 2);
                        identified = true;
                        item.TypeSpecificValue = 23;
                        break;
                    }
                case RodType.FireBall:
                    {
                        SaveGame.Instance.SpellEffects.FireBall(new ProjectFire(SaveGame.Instance.SpellEffects), dir, 72, 2);
                        identified = true;
                        item.TypeSpecificValue = 30;
                        break;
                    }
                case RodType.ColdBall:
                    {
                        SaveGame.Instance.SpellEffects.FireBall(new ProjectCold(SaveGame.Instance.SpellEffects), dir, 48, 2);
                        identified = true;
                        item.TypeSpecificValue = 25;
                        break;
                    }
                case RodType.Havoc:
                    {
                        SaveGame.Instance.SpellEffects.CallChaos();
                        identified = true;
                        item.TypeSpecificValue = 250;
                        break;
                    }
            }
            _player.NoticeFlags |= Constants.PnCombine | Constants.PnReorder;
            // We may have just discovered what the rod does
            item.ObjectTried();
            if (identified && !item.IsFlavourAware())
            {
                item.BecomeFlavourAware();
                _player.GainExperience((itemLevel + (_player.Level >> 1)) / _player.Level);
            }
            // We may not have actually used a charge
            if (!useCharge)
            {
                item.TypeSpecificValue = 0;
                return;
            }
            // Channellers can spend vis instead of a charge
            bool channeled = false;
            if (_player.Spellcasting.Type == CastingType.Channelling)
            {
                channeled = SaveGame.Instance.CommandEngine.DoCmdChannel(item);
                if (channeled)
                {
                    item.TypeSpecificValue = 0;
                }
            }
            if (!channeled)
            {
                // If the rod was part of a stack, remove it
                if (itemIndex >= 0 && item.Count > 1)
                {
                    Item singleRod = new Item(item) { Count = 1 };
                    item.TypeSpecificValue = 0;
                    item.Count--;
                    _player.WeightCarried -= singleRod.Weight;
                    _player.Inventory.InvenCarry(singleRod, false);
                    Profile.Instance.MsgPrint("You unstack your rod.");
                }
            }
        }

        /// <summary>
        /// Refill a lamp
        /// </summary>
        /// <param name="itemIndex"> The inventory index of the fuel </param>
        public void RefillLamp(int itemIndex)
        {
            // Get an item if we don't already have one
            SaveGame.Instance.ItemFilter = SaveGame.Instance.CommandEngine.ItemFilterLanternFuel;
            if (itemIndex == -999)
            {
                if (!SaveGame.Instance.GetItem(out itemIndex, "Refill with which flask? ", true, true, true))
                {
                    if (itemIndex == -2)
                    {
                        Profile.Instance.MsgPrint("You have no flasks of oil.");
                    }
                    return;
                }
            }
            Item fuelSource = itemIndex >= 0 ? _player.Inventory[itemIndex] : _level.Items[0 - itemIndex];
            // Make sure our item is suitable fuel
            SaveGame.Instance.ItemFilter = SaveGame.Instance.CommandEngine.ItemFilterLanternFuel;
            if (!_player.Inventory.ItemMatchesFilter(fuelSource))
            {
                Profile.Instance.MsgPrint("You can't refill a lantern from that!");
                SaveGame.Instance.ItemFilter = null;
                return;
            }
            SaveGame.Instance.ItemFilter = null;
            // Refilling takes half a turn
            SaveGame.Instance.EnergyUse = 50;
            Item lamp = _player.Inventory[InventorySlot.Lightsource];
            // Add the fuel
            lamp.TypeSpecificValue += fuelSource.TypeSpecificValue;
            Profile.Instance.MsgPrint("You fuel your lamp.");
            // Check for overfilling
            if (lamp.TypeSpecificValue >= Constants.FuelLamp)
            {
                lamp.TypeSpecificValue = Constants.FuelLamp;
                Profile.Instance.MsgPrint("Your lamp is full.");
            }
            // Update the inventory
            if (itemIndex >= 0)
            {
                _player.Inventory.InvenItemIncrease(itemIndex, -1);
                _player.Inventory.InvenItemDescribe(itemIndex);
                _player.Inventory.InvenItemOptimize(itemIndex);
            }
            else
            {
                SaveGame.Instance.Level.FloorItemIncrease(0 - itemIndex, -1);
                SaveGame.Instance.Level.FloorItemDescribe(0 - itemIndex);
                SaveGame.Instance.Level.FloorItemOptimize(0 - itemIndex);
            }
            _player.UpdatesNeeded.Set(UpdateFlags.UpdateTorchRadius);
        }

        /// <summary>
        /// Refill a torch from another torch
        /// </summary>
        /// <param name="itemIndex"> The inventory index of the fuel </param>
        public void RefillTorch(int itemIndex)
        {
            // Get an item if we don't already have one
            SaveGame.Instance.ItemFilter = SaveGame.Instance.CommandEngine.ItemFilterTorchFuel;
            if (itemIndex == -999)
            {
                if (!SaveGame.Instance.GetItem(out itemIndex, "Refuel with which torch? ", false, true, true))
                {
                    if (itemIndex == -2)
                    {
                        Profile.Instance.MsgPrint("You have no extra torches.");
                    }
                    return;
                }
            }
            Item fuelSource = itemIndex >= 0 ? _player.Inventory[itemIndex] : _level.Items[0 - itemIndex];
            // Check that our fuel is suitable
            SaveGame.Instance.ItemFilter = SaveGame.Instance.CommandEngine.ItemFilterTorchFuel;
            if (!_player.Inventory.ItemMatchesFilter(fuelSource))
            {
                Profile.Instance.MsgPrint("You can't refill a torch with that!");
                SaveGame.Instance.ItemFilter = null;
                return;
            }
            SaveGame.Instance.ItemFilter = null;
            // Refueling takes half a turn
            SaveGame.Instance.EnergyUse = 50;
            Item torch = _player.Inventory[InventorySlot.Lightsource];
            // Add the fuel
            torch.TypeSpecificValue += fuelSource.TypeSpecificValue + 5;
            Profile.Instance.MsgPrint("You combine the torches.");
            // Check for overfilling
            if (torch.TypeSpecificValue >= Constants.FuelTorch)
            {
                torch.TypeSpecificValue = Constants.FuelTorch;
                Profile.Instance.MsgPrint("Your torch is fully fueled.");
            }
            else
            {
                Profile.Instance.MsgPrint("Your torch glows more brightly.");
            }
            // Update the player's inventory
            if (itemIndex >= 0)
            {
                _player.Inventory.InvenItemIncrease(itemIndex, -1);
                _player.Inventory.InvenItemDescribe(itemIndex);
                _player.Inventory.InvenItemOptimize(itemIndex);
            }
            else
            {
                SaveGame.Instance.Level.FloorItemIncrease(0 - itemIndex, -1);
                SaveGame.Instance.Level.FloorItemDescribe(0 - itemIndex);
                SaveGame.Instance.Level.FloorItemOptimize(0 - itemIndex);
            }
            _player.UpdatesNeeded.Set(UpdateFlags.UpdateTorchRadius);
        }
    }
}