// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Projection;
using Cthangband.StaticData;
using System;

namespace Cthangband.Patron.Base
{
    [Serializable]
    internal abstract class BasePatron : IPatron
    {
        protected int PreferredAbility;
        protected Reward[] Rewards;
        protected string ShortName;
        public string LongName { get; set; }
        public bool MultiRew { get; set; }

        public void GetReward(Player player, Level level, SaveGame saveGame)
        {
            int type;
            int dummy;
            var nastyChance = 6;
            if (MultiRew)
            {
                return;
            }
            MultiRew = true;
            if (player.Level == 13)
            {
                nastyChance = 2;
            }
            else if (player.Level % 13 == 0)
            {
                nastyChance = 3;
            }
            else if (player.Level % 14 == 0)
            {
                nastyChance = 12;
            }
            if (Program.Rng.DieRoll(nastyChance) == 1)
            {
                type = Program.Rng.DieRoll(20);
            }
            else
            {
                type = Program.Rng.DieRoll(15) + 5;
            }
            if (type < 1)
            {
                type = 1;
            }
            if (type > 20)
            {
                type = 20;
            }
            type--;
            var wrathReason = $"the Wrath of {ShortName}";
            var effect = Rewards[type];
            if (Program.Rng.DieRoll(6) == 1)
            {
                Profile.Instance.MsgPrint($"{ShortName} rewards you with a mutation!");
                player.Dna.GainMutation();
                return;
            }
            switch (effect)
            {
                case Reward.PolySlf:
                    Profile.Instance.MsgPrint($"The voice of {ShortName} booms out:");
                    Profile.Instance.MsgPrint("'Thou needst a new form, mortal!'");
                    player.PolymorphSelf();
                    break;

                case Reward.GainExp:
                    Profile.Instance.MsgPrint($"The voice of {ShortName} booms out:");
                    Profile.Instance.MsgPrint("'Well done, mortal! Lead on!'");
                    if (player.ExperiencePoints < Constants.PyMaxExp)
                    {
                        var ee = player.ExperiencePoints / 2 + 10;
                        if (ee > 100000)
                        {
                            ee = 100000;
                        }
                        Profile.Instance.MsgPrint("You feel more experienced.");
                        player.GainExperience(ee);
                    }
                    break;

                case Reward.LoseExp:
                    Profile.Instance.MsgPrint($"The voice of {ShortName} booms out:");
                    Profile.Instance.MsgPrint("'Thou didst not deserve that, slave.'");
                    player.LoseExperience(player.ExperiencePoints / 6);
                    break;

                case Reward.GoodObj:
                    Profile.Instance.MsgPrint($"The voice of {ShortName} whispers:");
                    Profile.Instance.MsgPrint("'Use my gift wisely.'");
                    level.Acquirement(player.MapY, player.MapX, 1, false);
                    break;

                case Reward.GreaObj:
                    Profile.Instance.MsgPrint($"The voice of {ShortName} booms out:");
                    Profile.Instance.MsgPrint("'Use my gift wisely.'");
                    level.Acquirement(player.MapY, player.MapX, 1, true);
                    break;

                case Reward.ChaosWp:
                    Profile.Instance.MsgPrint($"The voice of {ShortName} booms out:");
                    Profile.Instance.MsgPrint("'Thy deed hath earned thee a worthy blade.'");
                    var qPtr = new Item();
                    int dummy2;
                    switch (Program.Rng.DieRoll(player.Level))
                    {
                        case 1:
                        case 2:
                        case 0:
                            dummy2 = SwordType.Dagger;
                            break;

                        case 3:
                        case 4:
                            dummy2 = SwordType.MainGauche;
                            break;

                        case 5:
                        case 6:
                            dummy2 = SwordType.Rapier;
                            break;

                        case 7:
                        case 8:
                            dummy2 = SwordType.SmallSword;
                            break;

                        case 9:
                        case 10:
                            dummy2 = SwordType.ShortSword;
                            break;

                        case 11:
                        case 12:
                        case 13:
                            dummy2 = SwordType.Sabre;
                            break;

                        case 14:
                        case 15:
                        case 16:
                            dummy2 = SwordType.Cutlass;
                            break;

                        case 17:
                            dummy2 = SwordType.Tulwar;
                            break;

                        case 18:
                        case 19:
                        case 20:
                            dummy2 = SwordType.BroadSword;
                            break;

                        case 21:
                        case 22:
                        case 23:
                            dummy2 = SwordType.LongSword;
                            break;

                        case 24:
                        case 25:
                        case 26:
                            dummy2 = SwordType.Scimitar;
                            break;

                        case 27:
                            dummy2 = SwordType.Katana;
                            break;

                        case 28:
                        case 29:
                            dummy2 = SwordType.BastardSword;
                            break;

                        case 30:
                        case 31:
                            dummy2 = SwordType.TwoHandedSword;
                            break;

                        case 32:
                            dummy2 = SwordType.ExecutionersSword;
                            break;

                        default:
                            dummy2 = SwordType.BladeOfChaos;
                            break;
                    }
                    qPtr.AssignItemType(Profile.Instance.ItemTypes.LookupKind(ItemCategory.Sword, dummy2));
                    qPtr.BonusToHit = 3 + Program.Rng.DieRoll(saveGame.Difficulty) % 10;
                    qPtr.BonusDamage = 3 + Program.Rng.DieRoll(saveGame.Difficulty) % 10;
                    qPtr.ApplyRandomResistance(Program.Rng.DieRoll(34) + 4);
                    qPtr.RareItemTypeIndex = Enumerations.RareItemType.WeaponChaotic;
                    level.DropNear(qPtr, -1, player.MapY, player.MapX);
                    break;

                case Reward.GoodObs:
                    Profile.Instance.MsgPrint($"The voice of {ShortName} booms out:");
                    Profile.Instance.MsgPrint("'Thy deed hath earned thee a worthy reward.'");
                    level.Acquirement(player.MapY, player.MapX, Program.Rng.DieRoll(2) + 1, false);
                    break;

                case Reward.GreaObs:
                    Profile.Instance.MsgPrint($"The voice of {ShortName} booms out:");
                    Profile.Instance.MsgPrint("'Behold, mortal, how generously I reward thy loyalty.'");
                    level.Acquirement(player.MapY, player.MapX, Program.Rng.DieRoll(2) + 1, true);
                    break;

                case Reward.DreadCurse:
                    Profile.Instance.MsgPrint($"The voice of {ShortName} thunders:");
                    Profile.Instance.MsgPrint("'Thou art growing arrogant, mortal.'");
                    saveGame.ActivateDreadCurse();
                    break;

                case Reward.SummonM:
                    Profile.Instance.MsgPrint($"The voice of {ShortName} booms out:");
                    Profile.Instance.MsgPrint("'My pets, destroy the arrogant mortal!'");
                    for (dummy = 0; dummy < Program.Rng.DieRoll(5) + 1; dummy++)
                    {
                        level.Monsters.SummonSpecific(player.MapY, player.MapX, saveGame.Difficulty, 0);
                    }
                    break;

                case Reward.HSummon:
                    Profile.Instance.MsgPrint($"The voice of {ShortName} booms out:");
                    Profile.Instance.MsgPrint("'Thou needst worthier opponents!'");
                    saveGame.SpellEffects.ActivateHiSummon();
                    break;

                case Reward.DoHavoc:
                    Profile.Instance.MsgPrint($"The voice of {ShortName} whispers out:");
                    Profile.Instance.MsgPrint("'Death and destruction! This pleaseth me!'");
                    saveGame.SpellEffects.CallChaos();
                    break;

                case Reward.GainAbl:
                    Profile.Instance.MsgPrint($"The voice of {ShortName} rings out:");
                    Profile.Instance.MsgPrint("'Stay, mortal, and let me mould thee.'");
                    if (Program.Rng.DieRoll(3) == 1 && !(PreferredAbility < 0))
                    {
                        player.TryIncreasingAbilityScore(PreferredAbility);
                    }
                    else
                    {
                        player.TryIncreasingAbilityScore(Program.Rng.DieRoll(6) - 1);
                    }
                    break;

                case Reward.LoseAbl:
                    Profile.Instance.MsgPrint($"The voice of {ShortName} booms out:");
                    Profile.Instance.MsgPrint("'I grow tired of thee, mortal.'");
                    if (Program.Rng.DieRoll(3) == 1 && !(PreferredAbility < 0))
                    {
                        player.TryDecreasingAbilityScore(PreferredAbility);
                    }
                    else
                    {
                        player.TryDecreasingAbilityScore(Program.Rng.DieRoll(6) - 1);
                    }
                    break;

                case Reward.RuinAbl:
                    Profile.Instance.MsgPrint($"The voice of {ShortName} thunders:");
                    Profile.Instance.MsgPrint("'Thou needst a lesson in humility, mortal!'");
                    Profile.Instance.MsgPrint("You feel less powerful!");
                    for (dummy = 0; dummy < 6; dummy++)
                    {
                        player.DecreaseAbilityScore(dummy, 10 + Program.Rng.DieRoll(15), true);
                    }
                    break;

                case Reward.PolyWnd:
                    Profile.Instance.MsgPrint($"You feel the power of {ShortName} touch you.");
                    player.PolymorphWounds();
                    break;

                case Reward.AugmAbl:
                    Profile.Instance.MsgPrint($"The voice of {ShortName} booms out:");
                    Profile.Instance.MsgPrint("'Receive this modest gift from me!'");
                    for (dummy = 0; dummy < 6; dummy++)
                    {
                        player.TryIncreasingAbilityScore(dummy);
                    }
                    break;

                case Reward.HurtLot:
                    Profile.Instance.MsgPrint($"The voice of {ShortName} booms out:");
                    Profile.Instance.MsgPrint("'Suffer, pathetic fool!'");
                    saveGame.SpellEffects.FireBall(new ProjectDisintegrate(SaveGame.Instance.SpellEffects), 0, player.Level * 4, 4);
                    player.TakeHit(player.Level * 4, wrathReason);
                    break;

                case Reward.HealFul:
                    Profile.Instance.MsgPrint($"The voice of {ShortName} booms out:");
                    Profile.Instance.MsgPrint("'Rise, my servant!'");
                    player.RestoreLevel();
                    player.SetTimedPoison(0);
                    player.SetTimedBlindness(0);
                    player.SetTimedConfusion(0);
                    player.SetTimedHallucinations(0);
                    player.SetTimedStun(0);
                    player.SetTimedBleeding(0);
                    player.RestoreHealth(5000);
                    for (dummy = 0; dummy < 6; dummy++)
                    {
                        player.TryRestoringAbilityScore(dummy);
                    }
                    break;

                case Reward.CurseWp:
                    Profile.Instance.MsgPrint($"The voice of {ShortName} booms out:");
                    Profile.Instance.MsgPrint("'Thou reliest too much on thine weapon.'");
                    saveGame.CommandEngine.CurseWeapon();
                    break;

                case Reward.CurseAr:
                    Profile.Instance.MsgPrint($"The voice of {ShortName} booms out:");
                    Profile.Instance.MsgPrint("'Thou reliest too much on thine equipment.'");
                    saveGame.CommandEngine.CurseArmour();
                    break;

                case Reward.PissOff:
                    Profile.Instance.MsgPrint($"The voice of {ShortName} whispers:");
                    Profile.Instance.MsgPrint("'Now thou shalt pay for annoying me.'");
                    switch (Program.Rng.DieRoll(4))
                    {
                        case 1:
                            saveGame.ActivateDreadCurse();
                            break;

                        case 2:
                            saveGame.SpellEffects.ActivateHiSummon();
                            break;

                        case 3:
                            if (Program.Rng.DieRoll(2) == 1)
                            {
                                saveGame.CommandEngine.CurseWeapon();
                            }
                            else
                            {
                                saveGame.CommandEngine.CurseArmour();
                            }
                            break;

                        default:
                            for (dummy = 0; dummy < 6; dummy++)
                            {
                                player.DecreaseAbilityScore(dummy, 10 + Program.Rng.DieRoll(15), true);
                            }
                            break;
                    }
                    break;

                case Reward.Wrath:
                    Profile.Instance.MsgPrint($"The voice of {ShortName} thunders:");
                    Profile.Instance.MsgPrint("'Die, mortal!'");
                    player.TakeHit(player.Level * 4, wrathReason);
                    for (dummy = 0; dummy < 6; dummy++)
                    {
                        player.DecreaseAbilityScore(dummy, 10 + Program.Rng.DieRoll(15), false);
                    }
                    saveGame.SpellEffects.ActivateHiSummon();
                    saveGame.ActivateDreadCurse();
                    if (Program.Rng.DieRoll(2) == 1)
                    {
                        saveGame.CommandEngine.CurseWeapon();
                    }
                    if (Program.Rng.DieRoll(2) == 1)
                    {
                        saveGame.CommandEngine.CurseArmour();
                    }
                    break;

                case Reward.Destruct:
                    Profile.Instance.MsgPrint($"The voice of {ShortName} booms out:");
                    Profile.Instance.MsgPrint("'Death and destruction! This pleaseth me!'");
                    saveGame.SpellEffects.DestroyArea(player.MapY, player.MapX, 25);
                    break;

                case Reward.Carnage:
                    Profile.Instance.MsgPrint($"The voice of {ShortName} booms out:");
                    Profile.Instance.MsgPrint("'Let me relieve thee of thine oppressors!'");
                    saveGame.SpellEffects.Carnage(false);
                    break;

                case Reward.MassGen:
                    Profile.Instance.MsgPrint($"The voice of {ShortName} rings out:");
                    Profile.Instance.MsgPrint("'Let me relieve thee of thine oppressors!'");
                    saveGame.SpellEffects.MassCarnage(false);
                    break;

                case Reward.DispelC:
                    Profile.Instance.MsgPrint($"You can feel the power of {ShortName} assault your enemies!");
                    saveGame.SpellEffects.DispelMonsters(player.Level * 4);
                    break;

                case Reward.Ignore:
                    Profile.Instance.MsgPrint($"{ShortName} ignores you.");
                    break;

                case Reward.SerDemo:
                    Profile.Instance.MsgPrint($"{ShortName} rewards you with a demonic servant!");
                    if (!level.Monsters.SummonSpecificFriendly(player.MapY, player.MapX, saveGame.Difficulty,
                        Constants.SummonDemon, false))
                    {
                        Profile.Instance.MsgPrint("Nobody ever turns up...");
                    }
                    break;

                case Reward.SerMons:
                    Profile.Instance.MsgPrint($"{ShortName} rewards you with a servant!");
                    if (!level.Monsters.SummonSpecificFriendly(player.MapY, player.MapX, saveGame.Difficulty,
                        Constants.SummonNoUniques, false))
                    {
                        Profile.Instance.MsgPrint("Nobody ever turns up...");
                    }
                    break;

                case Reward.SerUnde:
                    Profile.Instance.MsgPrint($"{ShortName} rewards you with an undead servant!");
                    if (!level.Monsters.SummonSpecificFriendly(player.MapY, player.MapX, saveGame.Difficulty,
                        Constants.SummonUndead, false))
                    {
                        Profile.Instance.MsgPrint("Nobody ever turns up...");
                    }
                    break;

                default:
                    Profile.Instance.MsgPrint($"The voice of {ShortName} stammers:");
                    Profile.Instance.MsgPrint($"'Uh... uh... the answer's {type}/{effect}, what's the question?'");
                    break;
            }
        }

        public override string ToString()
        {
            return ShortName;
        }

    }
}