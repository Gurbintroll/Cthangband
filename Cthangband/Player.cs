// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Mutations;
using Cthangband.Spells;
using Cthangband.StaticData;
using Cthangband.UI;
using Cthangband.Pantheon;
using System;
using Cthangband.Spells.Base;
using Cthangband.PlayerRace.Base;
using System.Linq;
using Cthangband.Patron.Base;
using Cthangband.PlayerClass.Base;

namespace Cthangband
{
    [Serializable]
    internal class Player
    {
        public readonly AbilityScore[] AbilityScores = new AbilityScore[6];
        public readonly Genome Dna = new Genome();
        public readonly string[] History = new string[4];
        public readonly Inventory Inventory;
        public readonly int[] MaxDlv = new int[Constants.MaxCaves];
        public readonly int[] PlayerHp = new int[Constants.PyMaxLevel];
        public int Age;
        public ItemCategory AmmunitionItemCategory;
        public int ArmourClassBonus;
        public int AttackBonus;
        public int BaseArmourClass;
        public string BirthRace;
        public string CurrentClass;
        public string CurrentRace;
        public int DamageBonus;
        public int DisplayedArmourClassBonus;
        public int DisplayedAttackBonus;
        public int DisplayedBaseArmourClass;
        public int DisplayedDamageBonus;
        public int Energy;
        public int ExperienceMultiplier;
        public int ExperiencePoints;
        public int Food;
        public int FractionalExperiencePoints;
        public int FractionalHealth;
        public int FractionalVis;
        public GameTime GameTime;
        public Gender Gender = new Gender();
        public int GenderIndex;
        public int Generation;
        public bool GetFirstLevelMutation;
        public int Gold;
        public IPatron GooPatron;
        public bool HasAcidImmunity;
        public bool HasAcidResistance;
        public bool HasAggravation;
        public bool HasAntiMagic;
        public bool HasAntiTeleport;
        public bool HasAntiTheft;
        public bool HasBlessedBlade;
        public bool HasBlindnessResistance;
        public bool HasChaosResistance;
        public bool HasColdImmunity;
        public bool HasColdResistance;
        public bool HasConfusingTouch;
        public bool HasConfusionResistance;
        public bool HasDarkResistance;
        public bool HasDisenchantResistance;
        public bool HasElementalVulnerability;
        public bool HasExperienceDrain;
        public bool HasExtraMight;
        public bool HasFearResistance;
        public bool HasFeatherFall;
        public bool HasFireImmunity;
        public bool HasFireResistance;
        public bool HasFireShield;
        public bool HasFreeAction;
        public bool HasGlow;
        public bool HasHeavyBow;
        public bool HasHeavyWeapon;
        public bool HasHoldLife;
        public bool HasLightningImmunity;
        public bool HasLightningResistance;
        public bool HasLightningShield;
        public bool HasLightResistance;
        public bool HasNetherResistance;
        public bool HasNexusResistance;
        public bool HasPoisonResistance;
        public bool HasQuakeWeapon;
        public bool HasRandomTeleport;
        public bool HasReflection;
        public bool HasRegeneration;
        public bool HasRestrictingArmour;
        public bool HasRestrictingGloves;
        public bool HasSeeInvisibility;
        public bool HasShardResistance;
        public bool HasSlowDigestion;
        public bool HasSoundResistance;
        public bool HasSustainCharisma;
        public bool HasSustainConstitution;
        public bool HasSustainDexterity;
        public bool HasSustainIntelligence;
        public bool HasSustainStrength;
        public bool HasSustainWisdom;
        public bool HasTelepathy;
        public bool HasTimeResistance;
        public bool HasUnpriestlyWeapon;
        public int Health;
        public int Height;
        public int HitDie;
        public int InfravisionRange;
        public bool IsDead;
        public bool IsSearching;
        public bool IsWinner;
        public bool IsWizard;
        public int Level;
        public int LightLevel;
        public int MapX;
        public int MapY;
        public int MaxExperienceGained;
        public int MaxHealth;
        public int MaxLevelGained;
        public int MaxVis;
        public int MeleeAttacksPerRound;
        public int MissileAttacksPerRound;
        public string Name;
        public uint NoticeFlags;
        public bool OldHeavyBow;
        public bool OldHeavyWeapon;
        public int OldLightLevel;
        public bool OldRestrictingArmour;
        public bool OldRestrictingGloves;
        public int OldSpareSpellSlots;
        public bool OldUnpriestlyWeapon;
        public IPlayerClass PlayerClass = null;
        public IPlayerRace Race = null;
        public Realm Realm1;
        public Realm Realm2;
        public FlagSet RedrawNeeded = new FlagSet();
        public Religion Religion = new Religion();
        public int SkillDigging;
        public int SkillDisarmTraps;
        public int SkillMelee;
        public int SkillRanged;
        public int SkillSavingThrow;
        public int SkillSearchFrequency;
        public int SkillSearching;
        public int SkillStealth;
        public int SkillThrowing;
        public int SkillUseDevice;
        public int SocialClass;
        public int SpareSpellSlots;
        public int Speed;
        public Spellcasting Spellcasting;
        public int TimedAcidResistance;
        public int TimedBleeding;
        public int TimedBlessing;
        public int TimedBlindness;
        public int TimedColdResistance;
        public int TimedConfusion;
        public int TimedEtherealness;
        public int TimedFear;
        public int TimedFireResistance;
        public int TimedHallucinations;
        public int TimedHaste;
        public int TimedHeroism;
        public int TimedInfravision;
        public int TimedInvulnerability;
        public int TimedLightningResistance;
        public int TimedParalysis;
        public int TimedPoison;
        public int TimedPoisonResistance;
        public int TimedProtectionFromEvil;
        public int TimedSeeInvisibility;
        public int TimedSlow;
        public int TimedStoneskin;
        public int TimedStun;
        public int TimedSuperheroism;
        public int TimedTelepathy;
        public int TownWithHouse;
        public FlagSet UpdatesNeeded = new FlagSet();
        public int Vis;
        public int Weight;
        public int WeightCarried;
        public int WildernessX;
        public int WildernessY;
        public int WordOfRecallDelay;

        public Player()
        {
            for (int i = 0; i < 4; i++)
            {
                History[i] = "";
            }
            for (int i = 0; i < 6; i++)
            {
                AbilityScores[i] = new AbilityScore();
            }
            WeightCarried = 0;
            Inventory = new Inventory(this);
            foreach (System.Collections.Generic.KeyValuePair<ArtifactId, Artifact> pair in Profile.Instance.Artifacts)
            {
                Artifact aPtr = pair.Value;
                aPtr.CurNum = 0;
            }
            for (int i = 0; i < Profile.Instance.ItemTypes.Count; i++)
            {
                ItemType kPtr = Profile.Instance.ItemTypes[i];
                kPtr.Tried = false;
                kPtr.FlavourAware = false;
            }
            for (int i = 1; i < Profile.Instance.MonsterRaces.Count; i++)
            {
                MonsterRace rPtr = Profile.Instance.MonsterRaces[i];
                rPtr.CurNum = 0;
                rPtr.MaxNum = 100;
                if ((rPtr.Flags1 & MonsterFlag1.Unique) != 0)
                {
                    rPtr.MaxNum = 1;
                }
                rPtr.Knowledge.RPkills = 0;
            }
            Profile.Instance.MonsterRaces[Profile.Instance.MonsterRaces.Count - 1].MaxNum = 0;
            Food = Constants.PyFoodFull - 1;
            IsWizard = false;
            IsWinner = false;
            TownWithHouse = -1;
            Generation = 1;
        }

        public void ChangeRace(string newRace)
        {
            CurrentRace = newRace;
            Race = PlayerRaces.Instance[CurrentRace];
            ExperienceMultiplier = Race.ExperienceFactor + PlayerClass.ExperienceFactor;
            if (GenderIndex == Constants.SexMale)
            {
                Height = Program.Rng.RandomNormal(Race.MaleBaseHeight, Race.MaleHeightRange);
                Weight = Program.Rng.RandomNormal(Race.MaleBaseWeight, Race.MaleWeightRange);
            }
            else if (GenderIndex == Constants.SexFemale)
            {
                Height = Program.Rng.RandomNormal(Race.FemaleBaseHeight, Race.FemaleHeightRange);
                Weight = Program.Rng.RandomNormal(Race.FemaleBaseWeight, Race.FemaleWeightRange);
            }
            else
            {
                if (Program.Rng.DieRoll(2) == 1)
                {
                    Height = Program.Rng.RandomNormal(Race.MaleBaseHeight, Race.MaleHeightRange);
                    Weight = Program.Rng.RandomNormal(Race.MaleBaseWeight, Race.MaleWeightRange);
                }
                else
                {
                    Height = Program.Rng.RandomNormal(Race.FemaleBaseHeight, Race.FemaleHeightRange);
                    Weight = Program.Rng.RandomNormal(Race.FemaleBaseWeight, Race.FemaleWeightRange);
                }
            }
            CheckExperience();
            MaxLevelGained = Level;
            RedrawNeeded.Set(RedrawFlag.PrBasic);
            UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
            SaveGame.Instance.HandleStuff();
        }

        public void CheckExperience()
        {
            bool levelReward = false;
            bool levelMutation = false;
            if (ExperiencePoints < 0)
            {
                ExperiencePoints = 0;
            }
            if (MaxExperienceGained < 0)
            {
                MaxExperienceGained = 0;
            }
            if (ExperiencePoints > Constants.PyMaxExp)
            {
                ExperiencePoints = Constants.PyMaxExp;
            }
            if (MaxExperienceGained > Constants.PyMaxExp)
            {
                MaxExperienceGained = Constants.PyMaxExp;
            }
            if (ExperiencePoints > MaxExperienceGained)
            {
                MaxExperienceGained = ExperiencePoints;
            }
            RedrawNeeded.Set(RedrawFlag.PrExp);
            SaveGame.Instance.HandleStuff();
            while (Level > 1 && ExperiencePoints < GlobalData.PlayerExp[Level - 2] * ExperienceMultiplier / 100L)
            {
                Level--;
                SaveGame.Instance.Level.RedrawSingleLocation(MapY, MapX);
                UpdatesNeeded.Set(UpdateFlags.UpdateBonuses | UpdateFlags.UpdateHealth | UpdateFlags.UpdateVis | UpdateFlags.UpdateSpells);
                RedrawNeeded.Set(RedrawFlag.PrLev | RedrawFlag.PrTitle);
                SaveGame.Instance.HandleStuff();
            }
            while (Level < Constants.PyMaxLevel && ExperiencePoints >= GlobalData.PlayerExp[Level - 1] * ExperienceMultiplier / 100L)
            {
                Level++;
                SaveGame.Instance.Level.RedrawSingleLocation(MapY, MapX);
                if (Level > MaxLevelGained)
                {
                    MaxLevelGained = Level;
                    if (PlayerClass.HasPatron)
                    {
                        levelReward = true;
                    }
                    if (Dna.ChaosGift)
                    {
                        levelReward = true;
                    }
                    if (Race.Mutates)
                    {
                        if (Program.Rng.DieRoll(5) == 1)
                        {
                            levelMutation = true;
                        }
                    }
                }
                Gui.PlaySound(SoundEffect.LevelGain);
                Profile.Instance.MsgPrint($"Welcome to level {Level}.");
                UpdatesNeeded.Set(UpdateFlags.UpdateBonuses | UpdateFlags.UpdateHealth | UpdateFlags.UpdateVis | UpdateFlags.UpdateSpells);
                RedrawNeeded.Set(RedrawFlag.PrExp | RedrawFlag.PrLev | RedrawFlag.PrTitle);
                SaveGame.Instance.HandleStuff();
                if (levelReward)
                {
                    GainLevelReward();
                    levelReward = false;
                }
                if (levelMutation)
                {
                    Profile.Instance.MsgPrint("You feel different...");
                    Dna.GainMutation();
                    levelMutation = false;
                }
            }
        }

        public void CurseEquipment(int chance, int heavyChance)
        {
            bool changed = false;
            FlagSet o1 = new FlagSet();
            FlagSet o2 = new FlagSet();
            FlagSet o3 = new FlagSet();
            Item oPtr = Inventory[InventorySlot.MeleeWeapon - 1 + Program.Rng.DieRoll(12)];
            if (Program.Rng.DieRoll(100) > chance)
            {
                return;
            }
            if (oPtr.ItemType == null)
            {
                return;
            }
            oPtr.GetMergedFlags(o1, o2, o3);
            if (o3.IsSet(ItemFlag3.Blessed) && Program.Rng.DieRoll(888) > chance)
            {
                string oName = oPtr.Description(false, 0);
                string s = oPtr.Count > 1 ? "" : "s";
                Profile.Instance.MsgPrint($"Your {oName} resist{s} cursing!");
                return;
            }
            if (Program.Rng.DieRoll(100) <= heavyChance &&
                (oPtr.ArtifactIndex != 0 || oPtr.RareItemTypeIndex != 0 ||
                 oPtr.IsLegendary()))
            {
                if (o3.IsClear(ItemFlag3.HeavyCurse))
                {
                    changed = true;
                }
                oPtr.LegendaryFlags3.Set(ItemFlag3.HeavyCurse);
                oPtr.LegendaryFlags3.Set(ItemFlag3.Cursed);
                oPtr.IdentifyFlags.Set(Constants.IdentCursed);
            }
            else
            {
                if (oPtr.IdentifyFlags.IsClear(Constants.IdentCursed))
                {
                    changed = true;
                }
                oPtr.LegendaryFlags3.Set(ItemFlag3.Cursed);
                oPtr.IdentifyFlags.Set(Constants.IdentCursed);
            }
            if (changed)
            {
                Profile.Instance.MsgPrint("There is a malignant black aura surrounding you...");
                if (!string.IsNullOrEmpty(oPtr.Inscription))
                {
                    if (oPtr.Inscription == "uncursed")
                    {
                        oPtr.Inscription = string.Empty;
                    }
                }
            }
        }

        public bool DecreaseAbilityScore(int stat, int amount, bool permanent)
        {
            int loss;
            bool res = false;
            int cur = AbilityScores[stat].Innate;
            int max = AbilityScores[stat].InnateMax;
            bool same = cur == max;
            if (cur > 3)
            {
                if (cur <= 18)
                {
                    if (amount > 90)
                    {
                        cur--;
                    }
                    if (amount > 50)
                    {
                        cur--;
                    }
                    if (amount > 20)
                    {
                        cur--;
                    }
                    cur--;
                }
                else
                {
                    loss = ((((cur - 18) / 2) + 1) / 2) + 1;
                    if (loss < 1)
                    {
                        loss = 1;
                    }
                    loss = (Program.Rng.DieRoll(loss) + loss) * amount / 100;
                    if (loss < amount / 2)
                    {
                        loss = amount / 2;
                    }
                    cur -= loss;
                    if (cur < 18)
                    {
                        cur = amount <= 20 ? 18 : 17;
                    }
                }
                if (cur < 3)
                {
                    cur = 3;
                }
                if (cur != AbilityScores[stat].Innate)
                {
                    res = true;
                }
            }
            if (permanent && max > 3)
            {
                if (max <= 18)
                {
                    if (amount > 90)
                    {
                        max--;
                    }
                    if (amount > 50)
                    {
                        max--;
                    }
                    if (amount > 20)
                    {
                        max--;
                    }
                    max--;
                }
                else
                {
                    loss = ((((max - 18) / 2) + 1) / 2) + 1;
                    loss = (Program.Rng.DieRoll(loss) + loss) * amount / 100;
                    if (loss < amount / 2)
                    {
                        loss = amount / 2;
                    }
                    max -= loss;
                    if (max < 18)
                    {
                        max = amount <= 20 ? 18 : 17;
                    }
                }
                if (same || max < cur)
                {
                    max = cur;
                }
                if (max != AbilityScores[stat].InnateMax)
                {
                    res = true;
                }
            }
            if (res)
            {
                AbilityScores[stat].Innate = cur;
                AbilityScores[stat].InnateMax = max;
                UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
            }
            return res;
        }

        public string DescribeWieldLocation(int index)
        {
            string p;
            switch (index)
            {
                case InventorySlot.MeleeWeapon:
                    p = "attacking monsters with";
                    break;

                case InventorySlot.RangedWeapon:
                    p = "shooting missiles with";
                    break;

                case InventorySlot.LeftHand:
                    p = "wearing on your left hand";
                    break;

                case InventorySlot.RightHand:
                    p = "wearing on your right hand";
                    break;

                case InventorySlot.Neck:
                    p = "wearing around your neck";
                    break;

                case InventorySlot.Lightsource:
                    p = "using to light the way";
                    break;

                case InventorySlot.Body:
                    p = "wearing on your body";
                    break;

                case InventorySlot.Cloak:
                    p = "wearing on your back";
                    break;

                case InventorySlot.Arm:
                    p = "wearing on your arm";
                    break;

                case InventorySlot.Head:
                    p = "wearing on your head";
                    break;

                case InventorySlot.Hands:
                    p = "wearing on your hands";
                    break;

                case InventorySlot.Feet:
                    p = "wearing on your feet";
                    break;

                default:
                    p = "carrying in your pack";
                    break;
            }
            if (index == InventorySlot.MeleeWeapon)
            {
                Item oPtr = Inventory[index];
                if (AbilityScores[Ability.Strength].StrMaxWeaponWeight < oPtr.Weight / 10)
                {
                    p = "just lifting";
                }
            }
            if (index == InventorySlot.RangedWeapon)
            {
                Item oPtr = Inventory[index];
                if (AbilityScores[Ability.Strength].StrMaxWeaponWeight < oPtr.Weight / 10)
                {
                    p = "just holding";
                }
            }
            return p;
        }

        public void GainExperience(int amount)
        {
            ExperiencePoints += amount;
            if (ExperiencePoints < MaxExperienceGained)
            {
                MaxExperienceGained += amount / 5;
            }
            CheckExperience();
        }

        public void GainLevelReward()
        {
            GooPatron.GetReward(this, SaveGame.Instance.Level, SaveGame.Instance);
        }

        public void GetAbilitiesAsItemFlags(FlagSet f1, FlagSet f2, FlagSet f3)
        {
            PlayerStatus playerStatus = new PlayerStatus(this, SaveGame.Instance.Level);
            f1.Clear();
            f2.Clear();
            f3.Clear();
            PlayerClass.GetAbilitiesAsItemFlags(this, f1, f2, f3);
            Race.GetAbilitiesAsItemFlags(this, f1, f2, f3);

            if (Dna.Regen)
            {
                f3.Set(ItemFlag3.Regen);
            }
            if (Dna.SuppressRegen)
            {
                f3.Clear(ItemFlag3.Regen);
            }
            if (Dna.SpeedBonus != 0)
            {
                f1.Set(ItemFlag1.Speed);
            }
            if (Dna.ElecHit)
            {
                f3.Set(ItemFlag3.ShElec);
            }
            if (Dna.FireHit)
            {
                f3.Set(ItemFlag3.ShFire);
                f3.Set(ItemFlag3.Lightsource);
            }
            if (Dna.FeatherFall)
            {
                f3.Set(ItemFlag3.Feather);
            }
            if (Dna.ResFear)
            {
                f2.Set(ItemFlag2.ResFear);
            }
            if (Dna.Esp)
            {
                f3.Set(ItemFlag3.Telepathy);
            }
            if (Dna.FreeAction)
            {
                f2.Set(ItemFlag2.FreeAct);
            }
            if (Dna.SustainAll)
            {
                f2.Set(ItemFlag2.SustCon);
                if (Level > 9)
                {
                    f2.Set(ItemFlag2.SustStr);
                }
                if (Level > 19)
                {
                    f2.Set(ItemFlag2.SustDex);
                }
                if (Level > 29)
                {
                    f2.Set(ItemFlag2.SustWis);
                }
                if (Level > 39)
                {
                    f2.Set(ItemFlag2.SustInt);
                }
                if (Level > 49)
                {
                    f2.Set(ItemFlag2.SustCha);
                }
            }
        }

        public int GetScore(SaveGame saveGame)
        {
            int score = (MaxLevelGained - 1) * 100;
            for (int i = 0; i < Constants.MaxCaves; i++)
            {
                if (MaxDlv[i] > 0)
                {
                    score += ((MaxDlv[i] + saveGame.Dungeons[i].Offset) * 10);
                }
            }
            for (int i = 0; i < saveGame.Quests.Count; i++)
            {
                if (saveGame.Quests[i].Level == 0)
                {
                    score += 100;
                }
            }
            if (IsWinner)
            {
                score += 1000;
            }
            if (MaxLevelGained < 50)
            {
                int prev = 0;
                if (MaxLevelGained > 1)
                {
                    prev = GlobalData.PlayerExp[MaxLevelGained - 2] * ExperienceMultiplier / 100;
                }
                int next = GlobalData.PlayerExp[MaxLevelGained - 1] * ExperienceMultiplier / 100;
                int numerator = MaxExperienceGained - prev;
                int denominator = next - prev;
                int fraction = 100 * numerator / denominator;
                score += fraction;
            }
            return score;
        }

        public void InputPlayerName()
        {
            Gui.Clear(42);
            Gui.PrintLine(Colour.Orange, "[Enter your player's name above, or hit ESCAPE]", 43, 2);
            const int col = 15;
            while (true)
            {
                Gui.Goto(2, col);
                string tmp = Name;
                if (!Gui.AskforAux(out Name, tmp, 12))
                {
                    Name = tmp;
                }
                if (Name.Trim() != tmp.Trim())
                {
                    Generation = 1;
                }
                break;
            }
            Name = Name.PadRight(12);
            Gui.Print(Colour.Brown, Name, 2, col);
            Gui.Clear(22);
        }

        public void LoseExperience(int amount)
        {
            if (amount > ExperiencePoints)
            {
                amount = ExperiencePoints;
            }
            ExperiencePoints -= amount;
            CheckExperience();
        }

        public void PolymorphSelf()
        {
            int effects = Program.Rng.DieRoll(2);
            int tmp = 0;
            bool moreEffects = true;
            Profile.Instance.MsgPrint("You feel a change coming over you...");
            while (effects-- != 0 && moreEffects)
            {
                switch (Program.Rng.DieRoll(12))
                {
                    case 1:
                    case 2:
                    case 3:
                        PolymorphWounds();
                        break;

                    case 4:
                    case 5:
                    case 6:
                        Dna.GainMutation();
                        break;

                    case 7:
                        {
                            string newRace;
                            do
                            {
                                newRace = PlayerRaces.Instance.RandomRaceName();
                            } while (newRace == CurrentRace);
                            string an = PlayerRaces.Instance[newRace].Article;
                            Profile.Instance.MsgPrint($"You turn into {an} {PlayerRaces.Instance[newRace].Title}!");
                            ChangeRace(newRace);
                        }
                        SaveGame.Instance.Level.RedrawSingleLocation(MapY, MapX);
                        moreEffects = false;
                        break;

                    case 8:
                        Profile.Instance.MsgPrint("You polymorph into an abomination!");
                        while (tmp < 6)
                        {
                            DecreaseAbilityScore(tmp, Program.Rng.FixedSeed + 6, Program.Rng.DieRoll(3) == 1);
                            tmp++;
                        }
                        if (Program.Rng.DieRoll(6) == 1)
                        {
                            Profile.Instance.MsgPrint("You find living difficult in your present form!");
                            TakeHit(Program.Rng.DiceRoll(Program.Rng.DieRoll(Level), Level), "a lethal mutation");
                        }
                        ShuffleAbilityScores();
                        break;

                    default:
                        ShuffleAbilityScores();
                        break;
                }
            }
        }

        public void PolymorphWounds()
        {
            int wounds = TimedBleeding;
            int hitP = MaxHealth - Health;
            int change = Program.Rng.DiceRoll(Level, 5);
            bool nastyEffect = Program.Rng.DieRoll(5) == 1;
            if (!(wounds != 0 || hitP != 0 || nastyEffect))
            {
                return;
            }
            if (nastyEffect)
            {
                Profile.Instance.MsgPrint("A new wound was created!");
                TakeHit(change, "a polymorphed wound");
                SetTimedBleeding(change);
            }
            else
            {
                Profile.Instance.MsgPrint("Your wounds are polymorphed into less serious ones.");
                RestoreHealth(change);
                SetTimedBleeding(TimedBleeding - (change / 2));
            }
        }

        public void PrintSpells(int[] spells, int num, int y, int x, Realm realm)
        {
            int i;
            int set = realm == Realm1 ? 0 : 1;
            Gui.PrintLine("", y, x);
            Gui.Print("Name", y, x + 5);
            Gui.Print("Lv Vis Fail Info", y, x + 35);
            for (i = 0; i < num; i++)
            {
                int spell = spells[i];
                ISpell sPtr = Spellcasting.Spells[set][spell];
                Gui.PrintLine($"{i.IndexToLetter()}) {sPtr.SummaryLine(this)}", y + i + 1, x);
            }
            Gui.PrintLine("", y + i + 1, x);
        }

        public void RegenerateHealth(int percent)
        {
            int oldHealth = Health;
            int newHealth = (MaxHealth * percent) + Constants.PyRegenHpbase;
            Health += newHealth >> 16;
            if (Health < 0 && oldHealth > 0)
            {
                Health = Constants.MaxShort;
            }
            int newFractionalHealth = (newHealth & 0xFFFF) + FractionalHealth;
            if (newFractionalHealth >= 0x10000)
            {
                FractionalHealth = newFractionalHealth - 0x10000;
                Health++;
            }
            else
            {
                FractionalHealth = newFractionalHealth;
            }
            if (Health >= MaxHealth)
            {
                Health = MaxHealth;
                FractionalHealth = 0;
            }
            if (oldHealth != Health)
            {
                RedrawNeeded.Set(RedrawFlag.PrHp);
            }
        }

        public void RegenerateVis(int percent)
        {
            int oldVis = Vis;
            int newVis = (MaxVis * percent) + Constants.PyRegenMnbase;
            Vis += newVis >> 16;
            if (Vis < 0 && oldVis > 0)
            {
                Vis = Constants.MaxShort;
            }
            int newFractionalVis = (newVis & 0xFFFF) + FractionalVis;
            if (newFractionalVis >= 0x10000L)
            {
                FractionalVis = newFractionalVis - 0x10000;
                Vis++;
            }
            else
            {
                FractionalVis = newFractionalVis;
            }
            if (Vis >= MaxVis)
            {
                Vis = MaxVis;
                FractionalVis = 0;
            }
            if (oldVis != Vis)
            {
                RedrawNeeded.Set(RedrawFlag.PrVis);
            }
        }

        public void RerollHitPoints()
        {
            int i;
            PlayerHp[0] = HitDie;
            int lastroll = HitDie;
            for (i = 1; i < Constants.PyMaxLevel; i++)
            {
                PlayerHp[i] = lastroll;
                lastroll--;
                if (lastroll < 1)
                {
                    lastroll = HitDie;
                }
            }
            for (i = 1; i < Constants.PyMaxLevel; i++)
            {
                int j = Program.Rng.DieRoll(Constants.PyMaxLevel - 1);
                lastroll = PlayerHp[i];
                PlayerHp[i] = PlayerHp[j];
                PlayerHp[j] = lastroll;
            }
            for (i = 1; i < Constants.PyMaxLevel; i++)
            {
                PlayerHp[i] = PlayerHp[i - 1] + PlayerHp[i];
            }
            UpdatesNeeded.Set(UpdateFlags.UpdateHealth);
            RedrawNeeded.Set(RedrawFlag.PrHp);
            SaveGame.Instance.HandleStuff();
        }

        public bool RestoreAbilityScore(int stat)
        {
            if (AbilityScores[stat].Innate != AbilityScores[stat].InnateMax)
            {
                AbilityScores[stat].Innate = AbilityScores[stat].InnateMax;
                UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
                return true;
            }
            return false;
        }

        public bool RestoreHealth(int num)
        {
            if (Health < MaxHealth)
            {
                Health += num;
                if (Health >= MaxHealth)
                {
                    Health = MaxHealth;
                    FractionalHealth = 0;
                }
                RedrawNeeded.Set(RedrawFlag.PrHp);
                if (num < 5)
                {
                    Profile.Instance.MsgPrint("You feel a little better.");
                }
                else if (num < 15)
                {
                    Profile.Instance.MsgPrint("You feel better.");
                }
                else if (num < 35)
                {
                    Profile.Instance.MsgPrint("You feel much better.");
                }
                else
                {
                    Profile.Instance.MsgPrint("You feel very good.");
                }
                return true;
            }
            return false;
        }

        public bool RestoreLevel()
        {
            if (ExperiencePoints < MaxExperienceGained)
            {
                Profile.Instance.MsgPrint("You feel your life energies returning.");
                ExperiencePoints = MaxExperienceGained;
                CheckExperience();
                return true;
            }
            return false;
        }

        public void SenseInventory()
        {
            int playerLevel = Level;
            if (TimedConfusion != 0)
            {
                return;
            }
            if (!PlayerClass.TrySenseInventory(playerLevel))
            {
                return;
            }
            bool detailed = PlayerClass.HasDetailedSenseInventory;
            for (int i = 0; i < InventorySlot.Total; i++)
            {
                bool okay = false;
                Item item = Inventory[i];
                if (item.ItemType == null)
                {
                    continue;
                }
                switch (item.Category)
                {
                    case ItemCategory.Shot:
                    case ItemCategory.Arrow:
                    case ItemCategory.Bolt:
                    case ItemCategory.Bow:
                    case ItemCategory.Digging:
                    case ItemCategory.Hafted:
                    case ItemCategory.Polearm:
                    case ItemCategory.Sword:
                    case ItemCategory.Boots:
                    case ItemCategory.Gloves:
                    case ItemCategory.Helm:
                    case ItemCategory.Crown:
                    case ItemCategory.Shield:
                    case ItemCategory.Cloak:
                    case ItemCategory.SoftArmor:
                    case ItemCategory.HardArmor:
                    case ItemCategory.DragArmor:
                        {
                            okay = true;
                            break;
                        }
                    case ItemCategory.Light: // Only orbs
                        {
                            if (item.ItemSubCategory == LightType.Orb)
                            {
                                okay = true;
                            }
                            break;
                        }
                }
                if (!okay)
                {
                    continue;
                }
                if (item.IdentifyFlags.IsSet(Constants.IdentSense))
                {
                    continue;
                }
                if (item.IsKnown())
                {
                    continue;
                }
                if (i < InventorySlot.MeleeWeapon && 0 != Program.Rng.RandomLessThan(5))
                {
                    continue;
                }
                string feel = detailed ? item.GetDetailedFeeling() : item.GetVagueFeeling();
                if (string.IsNullOrEmpty(feel))
                {
                    continue;
                }
                string oName = item.Description(false, 0);
                if (i >= InventorySlot.MeleeWeapon)
                {
                    string isare = item.Count == 1 ? "is" : "are";
                    Profile.Instance.MsgPrint(
                        $"You feel the {oName} ({i.IndexToLabel()}) you are {DescribeWieldLocation(i)} {isare} {feel}...");
                }
                else
                {
                    string isare = item.Count == 1 ? "is" : "are";
                    Profile.Instance.MsgPrint(
                        $"You feel the {oName} ({i.IndexToLabel()}) in your pack {isare} {feel}...");
                }
                item.IdentifyFlags.Set(Constants.IdentSense);
                if (string.IsNullOrEmpty(item.Inscription))
                {
                    item.Inscription = feel;
                }
                NoticeFlags |= Constants.PnCombine | Constants.PnReorder;
            }
        }

        public bool SetFood(int v)
        {
            int oldAux, newAux;
            bool notice = false;
            v = v > 20000 ? 20000 : v < 0 ? 0 : v;
            if (Food < Constants.PyFoodFaint)
            {
                oldAux = 0;
            }
            else if (Food < Constants.PyFoodWeak)
            {
                oldAux = 1;
            }
            else if (Food < Constants.PyFoodAlert)
            {
                oldAux = 2;
            }
            else if (Food < Constants.PyFoodFull)
            {
                oldAux = 3;
            }
            else if (Food < Constants.PyFoodMax)
            {
                oldAux = 4;
            }
            else
            {
                oldAux = 5;
            }
            if (v < Constants.PyFoodFaint)
            {
                newAux = 0;
            }
            else if (v < Constants.PyFoodWeak)
            {
                newAux = 1;
            }
            else if (v < Constants.PyFoodAlert)
            {
                newAux = 2;
            }
            else if (v < Constants.PyFoodFull)
            {
                newAux = 3;
            }
            else if (v < Constants.PyFoodMax)
            {
                newAux = 4;
            }
            else
            {
                newAux = 5;
            }
            if (newAux > oldAux)
            {
                switch (newAux)
                {
                    case 1:
                        Profile.Instance.MsgPrint("You are still weak.");
                        break;

                    case 2:
                        Profile.Instance.MsgPrint("You are still hungry.");
                        break;

                    case 3:
                        Profile.Instance.MsgPrint("You are no longer hungry.");
                        break;

                    case 4:
                        Profile.Instance.MsgPrint("You are full!");
                        break;

                    case 5:
                        Profile.Instance.MsgPrint("You have gorged yourself!");
                        break;
                }
                notice = true;
            }
            else if (newAux < oldAux)
            {
                switch (newAux)
                {
                    case 0:
                        Profile.Instance.MsgPrint("You are getting faint from hunger!");
                        break;

                    case 1:
                        Profile.Instance.MsgPrint("You are getting weak from hunger!");
                        break;

                    case 2:
                        Profile.Instance.MsgPrint("You are getting hungry.");
                        break;

                    case 3:
                        Profile.Instance.MsgPrint("You are no longer full.");
                        break;

                    case 4:
                        Profile.Instance.MsgPrint("You are no longer gorged.");
                        break;
                }
                notice = true;
            }
            Food = v;
            if (!notice)
            {
                return false;
            }
            SaveGame.Instance.Disturb(false);
            UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
            RedrawNeeded.Set(RedrawFlag.PrHunger);
            SaveGame.Instance.HandleStuff();
            return true;
        }

        public void SetTimedAcidResistance(int v)
        {
            bool notice = false;
            v = v > 10000 ? 10000 : v < 0 ? 0 : v;
            if (v != 0)
            {
                if (TimedAcidResistance == 0)
                {
                    Profile.Instance.MsgPrint("You feel resistant to acid!");
                    notice = true;
                }
            }
            else
            {
                if (TimedAcidResistance != 0)
                {
                    Profile.Instance.MsgPrint("You feel less resistant to acid.");
                    notice = true;
                }
            }
            TimedAcidResistance = v;
            if (!notice)
            {
                return;
            }
            SaveGame.Instance.Disturb(false);
            SaveGame.Instance.HandleStuff();
        }

        public bool SetTimedBleeding(int v)
        {
            int oldAux, newAux;
            bool notice = false;
            v = v > 10000 ? 10000 : v < 0 ? 0 : v;
            if (Race.DoesntBleed(this))
            {
                v = 0;
            }
            if (TimedBleeding > 1000)
            {
                oldAux = 7;
            }
            else if (TimedBleeding > 200)
            {
                oldAux = 6;
            }
            else if (TimedBleeding > 100)
            {
                oldAux = 5;
            }
            else if (TimedBleeding > 50)
            {
                oldAux = 4;
            }
            else if (TimedBleeding > 25)
            {
                oldAux = 3;
            }
            else if (TimedBleeding > 10)
            {
                oldAux = 2;
            }
            else if (TimedBleeding > 0)
            {
                oldAux = 1;
            }
            else
            {
                oldAux = 0;
            }
            if (v > 1000)
            {
                newAux = 7;
            }
            else if (v > 200)
            {
                newAux = 6;
            }
            else if (v > 100)
            {
                newAux = 5;
            }
            else if (v > 50)
            {
                newAux = 4;
            }
            else if (v > 25)
            {
                newAux = 3;
            }
            else if (v > 10)
            {
                newAux = 2;
            }
            else if (v > 0)
            {
                newAux = 1;
            }
            else
            {
                newAux = 0;
            }
            if (newAux > oldAux)
            {
                switch (newAux)
                {
                    case 1:
                        Profile.Instance.MsgPrint("You have been given a graze.");
                        break;

                    case 2:
                        Profile.Instance.MsgPrint("You have been given a light cut.");
                        break;

                    case 3:
                        Profile.Instance.MsgPrint("You have been given a bad cut.");
                        break;

                    case 4:
                        Profile.Instance.MsgPrint("You have been given a nasty cut.");
                        break;

                    case 5:
                        Profile.Instance.MsgPrint("You have been given a severe cut.");
                        break;

                    case 6:
                        Profile.Instance.MsgPrint("You have been given a deep gash.");
                        break;

                    case 7:
                        Profile.Instance.MsgPrint("You have been given a mortal wound.");
                        break;
                }
                notice = true;
                if (Program.Rng.DieRoll(1000) < v || Program.Rng.DieRoll(16) == 1)
                {
                    if (!HasSustainCharisma)
                    {
                        Profile.Instance.MsgPrint("You have been horribly scarred.");
                        TryDecreasingAbilityScore(Ability.Charisma);
                    }
                }
            }
            else if (newAux < oldAux)
            {
                switch (newAux)
                {
                    case 0:
                        Profile.Instance.MsgPrint("You are no longer bleeding.");
                        SaveGame.Instance.Disturb(false);
                        break;
                }
                notice = true;
            }
            TimedBleeding = v;
            if (!notice)
            {
                return false;
            }
            SaveGame.Instance.Disturb(false);
            UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
            RedrawNeeded.Set(RedrawFlag.PrCut);
            SaveGame.Instance.HandleStuff();
            return true;
        }

        public bool SetTimedBlessing(int v)
        {
            bool notice = false;
            v = v > 10000 ? 10000 : v < 0 ? 0 : v;
            if (v != 0)
            {
                if (TimedBlessing == 0)
                {
                    Profile.Instance.MsgPrint("You feel righteous!");
                    notice = true;
                }
            }
            else
            {
                if (TimedBlessing != 0)
                {
                    Profile.Instance.MsgPrint("The prayer has expired.");
                    notice = true;
                }
            }
            TimedBlessing = v;
            if (!notice)
            {
                return false;
            }
            SaveGame.Instance.Disturb(false);
            UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
            SaveGame.Instance.HandleStuff();
            return true;
        }

        public bool SetTimedBlindness(int v)
        {
            bool notice = false;
            v = v > 10000 ? 10000 : v < 0 ? 0 : v;
            if (v != 0)
            {
                if (TimedBlindness == 0)
                {
                    Profile.Instance.MsgPrint("You are blind!");
                    notice = true;
                }
            }
            else
            {
                if (TimedBlindness != 0)
                {
                    Profile.Instance.MsgPrint("You can see again.");
                    notice = true;
                }
            }
            TimedBlindness = v;
            if (!notice)
            {
                return false;
            }
            SaveGame.Instance.Disturb(false);
            UpdatesNeeded.Set(UpdateFlags.UpdateRemoveView | UpdateFlags.UpdateRemoveLight);
            UpdatesNeeded.Set(UpdateFlags.UpdateView | UpdateFlags.UpdateLight);
            UpdatesNeeded.Set(UpdateFlags.UpdateMonsters);
            RedrawNeeded.Set(RedrawFlag.PrMap);
            RedrawNeeded.Set(RedrawFlag.PrBlind);
            SaveGame.Instance.HandleStuff();
            return true;
        }

        public bool SetTimedColdResistance(int v)
        {
            bool notice = false;
            v = v > 10000 ? 10000 : v < 0 ? 0 : v;
            if (v != 0)
            {
                if (TimedColdResistance == 0)
                {
                    Profile.Instance.MsgPrint("You feel resistant to cold!");
                    notice = true;
                }
            }
            else
            {
                if (TimedColdResistance != 0)
                {
                    Profile.Instance.MsgPrint("You feel less resistant to cold.");
                    notice = true;
                }
            }
            TimedColdResistance = v;
            if (!notice)
            {
                return false;
            }
            SaveGame.Instance.Disturb(false);
            SaveGame.Instance.HandleStuff();
            return true;
        }

        public bool SetTimedConfusion(int v)
        {
            bool notice = false;
            v = v > 10000 ? 10000 : v < 0 ? 0 : v;
            if (v != 0)
            {
                if (TimedConfusion == 0)
                {
                    Profile.Instance.MsgPrint("You are confused!");
                    notice = true;
                }
            }
            else
            {
                if (TimedConfusion != 0)
                {
                    Profile.Instance.MsgPrint("You feel less confused now.");
                    notice = true;
                }
            }
            TimedConfusion = v;
            if (!notice)
            {
                return false;
            }
            SaveGame.Instance.Disturb(false);
            RedrawNeeded.Set(RedrawFlag.PrConfused);
            SaveGame.Instance.HandleStuff();
            return true;
        }

        public void SetTimedEtherealness(int v)
        {
            bool notice = false;
            v = v > 10000 ? 10000 : v < 0 ? 0 : v;
            if (v != 0)
            {
                if (TimedEtherealness == 0)
                {
                    Profile.Instance.MsgPrint("You leave the physical world and turn into a wraith-being!");
                    notice = true;
                    {
                        RedrawNeeded.Set(RedrawFlag.PrMap);
                        UpdatesNeeded.Set(UpdateFlags.UpdateMonsters);
                    }
                }
            }
            else
            {
                if (TimedEtherealness != 0)
                {
                    Profile.Instance.MsgPrint("You feel opaque.");
                    notice = true;
                    {
                        RedrawNeeded.Set(RedrawFlag.PrMap);
                        UpdatesNeeded.Set(UpdateFlags.UpdateMonsters);
                    }
                }
            }
            TimedEtherealness = v;
            if (!notice)
            {
                return;
            }
            SaveGame.Instance.Disturb(false);
            UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
            SaveGame.Instance.HandleStuff();
        }

        public bool SetTimedFear(int v)
        {
            bool notice = false;
            v = v > 10000 ? 10000 : v < 0 ? 0 : v;
            if (v != 0)
            {
                if (TimedFear == 0)
                {
                    Profile.Instance.MsgPrint("You are terrified!");
                    notice = true;
                }
            }
            else
            {
                if (TimedFear != 0)
                {
                    Profile.Instance.MsgPrint("You feel bolder now.");
                    notice = true;
                }
            }
            TimedFear = v;
            if (!notice)
            {
                return false;
            }
            SaveGame.Instance.Disturb(false);
            RedrawNeeded.Set(RedrawFlag.PrAfraid);
            SaveGame.Instance.HandleStuff();
            return true;
        }

        public bool SetTimedFireResistance(int v)
        {
            bool notice = false;
            v = v > 10000 ? 10000 : v < 0 ? 0 : v;
            if (v != 0)
            {
                if (TimedFireResistance == 0)
                {
                    Profile.Instance.MsgPrint("You feel resistant to fire!");
                    notice = true;
                }
            }
            else
            {
                if (TimedFireResistance != 0)
                {
                    Profile.Instance.MsgPrint("You feel less resistant to fire.");
                    notice = true;
                }
            }
            TimedFireResistance = v;
            if (!notice)
            {
                return false;
            }
            SaveGame.Instance.Disturb(false);
            SaveGame.Instance.HandleStuff();
            return true;
        }

        public bool SetTimedHallucinations(int v)
        {
            bool notice = false;
            v = v > 10000 ? 10000 : v < 0 ? 0 : v;
            if (v != 0)
            {
                if (TimedHallucinations == 0)
                {
                    Profile.Instance.MsgPrint("Oh, wow! Everything looks so cosmic now!");
                    notice = true;
                }
            }
            else
            {
                if (TimedHallucinations != 0)
                {
                    Profile.Instance.MsgPrint("You can see clearly again.");
                    notice = true;
                }
            }
            TimedHallucinations = v;
            if (!notice)
            {
                return false;
            }
            SaveGame.Instance.Disturb(false);
            RedrawNeeded.Set(RedrawFlag.PrMap);
            UpdatesNeeded.Set(UpdateFlags.UpdateMonsters);
            SaveGame.Instance.HandleStuff();
            return true;
        }

        public bool SetTimedHaste(int v)
        {
            bool notice = false;
            v = v > 10000 ? 10000 : v < 0 ? 0 : v;
            if (v != 0)
            {
                if (TimedHaste == 0)
                {
                    Profile.Instance.MsgPrint("You feel yourself moving faster!");
                    notice = true;
                }
            }
            else
            {
                if (TimedHaste != 0)
                {
                    Profile.Instance.MsgPrint("You feel yourself slow down.");
                    notice = true;
                }
            }
            TimedHaste = v;
            if (!notice)
            {
                return false;
            }
            SaveGame.Instance.Disturb(false);
            UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
            SaveGame.Instance.HandleStuff();
            return true;
        }

        public bool SetTimedHeroism(int v)
        {
            bool notice = false;
            v = v > 10000 ? 10000 : v < 0 ? 0 : v;
            if (v != 0)
            {
                if (TimedHeroism == 0)
                {
                    Profile.Instance.MsgPrint("You feel like a hero!");
                    notice = true;
                }
            }
            else
            {
                if (TimedHeroism != 0)
                {
                    Profile.Instance.MsgPrint("The heroism wears off.");
                    notice = true;
                }
            }
            TimedHeroism = v;
            if (!notice)
            {
                return false;
            }
            SaveGame.Instance.Disturb(false);
            UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
            UpdatesNeeded.Set(UpdateFlags.UpdateHealth);
            SaveGame.Instance.HandleStuff();
            return true;
        }

        public bool SetTimedInfravision(int v)
        {
            bool notice = false;
            v = v > 10000 ? 10000 : v < 0 ? 0 : v;
            if (v != 0)
            {
                if (TimedInfravision == 0)
                {
                    Profile.Instance.MsgPrint("Your eyes begin to tingle!");
                    notice = true;
                }
            }
            else
            {
                if (TimedInfravision != 0)
                {
                    Profile.Instance.MsgPrint("Your eyes stop tingling.");
                    notice = true;
                }
            }
            TimedInfravision = v;
            if (!notice)
            {
                return false;
            }
            SaveGame.Instance.Disturb(false);
            UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
            UpdatesNeeded.Set(UpdateFlags.UpdateMonsters);
            SaveGame.Instance.HandleStuff();
            return true;
        }

        public void SetTimedInvulnerability(int v)
        {
            bool notice = false;
            v = v > 10000 ? 10000 : v < 0 ? 0 : v;
            if (v != 0)
            {
                if (TimedInvulnerability == 0)
                {
                    Profile.Instance.MsgPrint("Invulnerability!");
                    notice = true;
                    {
                        RedrawNeeded.Set(RedrawFlag.PrMap);
                        UpdatesNeeded.Set(UpdateFlags.UpdateMonsters);
                    }
                }
            }
            else
            {
                if (TimedInvulnerability != 0)
                {
                    Profile.Instance.MsgPrint("The invulnerability wears off.");
                    notice = true;
                    {
                        RedrawNeeded.Set(RedrawFlag.PrMap);
                        UpdatesNeeded.Set(UpdateFlags.UpdateMonsters);
                    }
                }
            }
            TimedInvulnerability = v;
            if (!notice)
            {
                return;
            }
            SaveGame.Instance.Disturb(false);
            UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
            SaveGame.Instance.HandleStuff();
        }

        public void SetTimedLightningResistance(int v)
        {
            bool notice = false;
            v = v > 10000 ? 10000 : v < 0 ? 0 : v;
            if (v != 0)
            {
                if (TimedLightningResistance == 0)
                {
                    Profile.Instance.MsgPrint("You feel resistant to electricity!");
                    notice = true;
                }
            }
            else
            {
                if (TimedLightningResistance != 0)
                {
                    Profile.Instance.MsgPrint("You feel less resistant to electricity.");
                    notice = true;
                }
            }
            TimedLightningResistance = v;
            if (!notice)
            {
                return;
            }
            SaveGame.Instance.Disturb(false);
            SaveGame.Instance.HandleStuff();
        }

        public bool SetTimedParalysis(int v)
        {
            bool notice = false;
            v = v > 10000 ? 10000 : v < 0 ? 0 : v;
            if (v != 0)
            {
                if (TimedParalysis == 0)
                {
                    Profile.Instance.MsgPrint("You are paralyzed!");
                    notice = true;
                }
            }
            else
            {
                if (TimedParalysis != 0)
                {
                    Profile.Instance.MsgPrint("You can move again.");
                    notice = true;
                }
            }
            TimedParalysis = v;
            if (!notice)
            {
                return false;
            }
            SaveGame.Instance.Disturb(false);
            RedrawNeeded.Set(RedrawFlag.PrState);
            SaveGame.Instance.HandleStuff();
            return true;
        }

        public bool SetTimedPoison(int v)
        {
            bool notice = false;
            v = v > 10000 ? 10000 : v < 0 ? 0 : v;
            if (v != 0)
            {
                if (TimedPoison == 0)
                {
                    Profile.Instance.MsgPrint("You are poisoned!");
                    notice = true;
                }
            }
            else
            {
                if (TimedPoison != 0)
                {
                    Profile.Instance.MsgPrint("You are no longer poisoned.");
                    notice = true;
                }
            }
            TimedPoison = v;
            if (!notice)
            {
                return false;
            }
            SaveGame.Instance.Disturb(false);
            RedrawNeeded.Set(RedrawFlag.PrPoisoned);
            SaveGame.Instance.HandleStuff();
            return true;
        }

        public void SetTimedPoisonResistance(int v)
        {
            bool notice = false;
            v = v > 10000 ? 10000 : v < 0 ? 0 : v;
            if (v != 0)
            {
                if (TimedPoisonResistance == 0)
                {
                    Profile.Instance.MsgPrint("You feel resistant to poison!");
                    notice = true;
                }
            }
            else
            {
                if (TimedPoisonResistance != 0)
                {
                    Profile.Instance.MsgPrint("You feel less resistant to poison.");
                    notice = true;
                }
            }
            TimedPoisonResistance = v;
            if (!notice)
            {
                return;
            }
            SaveGame.Instance.Disturb(false);
            SaveGame.Instance.HandleStuff();
        }

        public bool SetTimedProtectionFromEvil(int v)
        {
            bool notice = false;
            v = v > 10000 ? 10000 : v < 0 ? 0 : v;
            if (v != 0)
            {
                if (TimedProtectionFromEvil == 0)
                {
                    Profile.Instance.MsgPrint("You feel safe from evil!");
                    notice = true;
                }
            }
            else
            {
                if (TimedProtectionFromEvil != 0)
                {
                    Profile.Instance.MsgPrint("You no longer feel safe from evil.");
                    notice = true;
                }
            }
            TimedProtectionFromEvil = v;
            if (!notice)
            {
                return false;
            }
            SaveGame.Instance.Disturb(false);
            SaveGame.Instance.HandleStuff();
            return true;
        }

        public bool SetTimedSeeInvisibility(int v)
        {
            bool notice = false;
            v = v > 10000 ? 10000 : v < 0 ? 0 : v;
            if (v != 0)
            {
                if (TimedSeeInvisibility == 0)
                {
                    Profile.Instance.MsgPrint("Your eyes feel very sensitive!");
                    notice = true;
                }
            }
            else
            {
                if (TimedSeeInvisibility != 0)
                {
                    Profile.Instance.MsgPrint("Your eyes feel less sensitive.");
                    notice = true;
                }
            }
            TimedSeeInvisibility = v;
            if (!notice)
            {
                return false;
            }
            SaveGame.Instance.Disturb(false);
            UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
            UpdatesNeeded.Set(UpdateFlags.UpdateMonsters);
            SaveGame.Instance.HandleStuff();
            return true;
        }

        public bool SetTimedSlow(int v)
        {
            bool notice = false;
            v = v > 10000 ? 10000 : v < 0 ? 0 : v;
            if (v != 0)
            {
                if (TimedSlow == 0)
                {
                    Profile.Instance.MsgPrint("You feel yourself moving slower!");
                    notice = true;
                }
            }
            else
            {
                if (TimedSlow != 0)
                {
                    Profile.Instance.MsgPrint("You feel yourself speed up.");
                    notice = true;
                }
            }
            TimedSlow = v;
            if (!notice)
            {
                return false;
            }
            SaveGame.Instance.Disturb(false);
            UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
            SaveGame.Instance.HandleStuff();
            return true;
        }

        public void SetTimedStoneskin(int v)
        {
            bool notice = false;
            v = v > 10000 ? 10000 : v < 0 ? 0 : v;
            if (v != 0)
            {
                if (TimedStoneskin == 0)
                {
                    Profile.Instance.MsgPrint("Your skin turns to stone.");
                    notice = true;
                }
            }
            else
            {
                if (TimedStoneskin != 0)
                {
                    Profile.Instance.MsgPrint("Your skin returns to normal.");
                    notice = true;
                }
            }
            TimedStoneskin = v;
            if (!notice)
            {
                return;
            }
            SaveGame.Instance.Disturb(false);
            UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
            SaveGame.Instance.HandleStuff();
        }

        public bool SetTimedStun(int v)
        {
            int oldAux, newAux;
            bool notice = false;
            v = v > 10000 ? 10000 : v < 0 ? 0 : v;
            if (Race.DoesntStun(this))
            {
                v = 0;
            }
            if (TimedStun > 100)
            {
                oldAux = 3;
            }
            else if (TimedStun > 50)
            {
                oldAux = 2;
            }
            else if (TimedStun > 0)
            {
                oldAux = 1;
            }
            else
            {
                oldAux = 0;
            }
            if (v > 100)
            {
                newAux = 3;
            }
            else if (v > 50)
            {
                newAux = 2;
            }
            else if (v > 0)
            {
                newAux = 1;
            }
            else
            {
                newAux = 0;
            }
            if (newAux > oldAux)
            {
                switch (newAux)
                {
                    case 1:
                        Profile.Instance.MsgPrint("You have been stunned.");
                        break;

                    case 2:
                        Profile.Instance.MsgPrint("You have been heavily stunned.");
                        break;

                    case 3:
                        Profile.Instance.MsgPrint("You have been knocked out.");
                        break;
                }
                if (Program.Rng.DieRoll(1000) < v || Program.Rng.DieRoll(16) == 1)
                {
                    Profile.Instance.MsgPrint("A vicious Attack hits your head.");
                    if (Program.Rng.DieRoll(3) == 1)
                    {
                        if (!HasSustainIntelligence)
                        {
                            TryDecreasingAbilityScore(Ability.Intelligence);
                        }
                        if (!HasSustainWisdom)
                        {
                            TryDecreasingAbilityScore(Ability.Wisdom);
                        }
                    }
                    else if (Program.Rng.DieRoll(2) == 1)
                    {
                        if (!HasSustainIntelligence)
                        {
                            TryDecreasingAbilityScore(Ability.Intelligence);
                        }
                    }
                    else
                    {
                        if (!HasSustainWisdom)
                        {
                            TryDecreasingAbilityScore(Ability.Wisdom);
                        }
                    }
                }
                notice = true;
            }
            else if (newAux < oldAux)
            {
                switch (newAux)
                {
                    case 0:
                        Profile.Instance.MsgPrint("You are no longer stunned.");
                        SaveGame.Instance.Disturb(false);
                        break;
                }
                notice = true;
            }
            TimedStun = v;
            if (!notice)
            {
                return false;
            }
            SaveGame.Instance.Disturb(false);
            UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
            RedrawNeeded.Set(RedrawFlag.PrStun);
            SaveGame.Instance.HandleStuff();
            return true;
        }

        public bool SetTimedSuperheroism(int v)
        {
            bool notice = false;
            v = v > 10000 ? 10000 : v < 0 ? 0 : v;
            if (v != 0)
            {
                if (TimedSuperheroism == 0)
                {
                    Profile.Instance.MsgPrint("You feel like a killing machine!");
                    notice = true;
                }
            }
            else
            {
                if (TimedSuperheroism != 0)
                {
                    Profile.Instance.MsgPrint("You feel less Berserk.");
                    notice = true;
                }
            }
            TimedSuperheroism = v;
            if (!notice)
            {
                return false;
            }
            SaveGame.Instance.Disturb(false);
            UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
            UpdatesNeeded.Set(UpdateFlags.UpdateHealth);
            SaveGame.Instance.HandleStuff();
            return true;
        }

        public void SetTimedTelepathy(int v)
        {
            bool notice = false;
            v = v > 10000 ? 10000 : v < 0 ? 0 : v;
            if (v != 0)
            {
                if (TimedTelepathy == 0)
                {
                    Profile.Instance.MsgPrint("You feel your consciousness expand!");
                    notice = true;
                }
            }
            else
            {
                if (TimedTelepathy != 0)
                {
                    Profile.Instance.MsgPrint("Your consciousness contracts again.");
                    notice = true;
                }
            }
            TimedTelepathy = v;
            if (!notice)
            {
                return;
            }
            SaveGame.Instance.Disturb(false);
            UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
            UpdatesNeeded.Set(UpdateFlags.UpdateMonsters);
            SaveGame.Instance.HandleStuff();
        }

        public void ShuffleAbilityScores()
        {
            int jj;
            int ii = Program.Rng.RandomLessThan(6);
            for (jj = ii; jj != ii; jj = Program.Rng.RandomLessThan(6))
            {
            }
            int max1 = AbilityScores[ii].InnateMax;
            int cur1 = AbilityScores[ii].Innate;
            int max2 = AbilityScores[jj].InnateMax;
            int cur2 = AbilityScores[jj].Innate;
            AbilityScores[ii].InnateMax = max2;
            AbilityScores[ii].Innate = cur2;
            AbilityScores[jj].InnateMax = max1;
            AbilityScores[jj].Innate = cur1;
            UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
        }

        public bool SpellOkay(int spell, bool known, bool realm2)
        {
            int set = realm2 ? 1 : 0;
            ISpell sPtr = Spellcasting.Spells[set][spell % 32];
            if (sPtr.Level > Level)
            {
                return false;
            }
            if (sPtr.Forgotten)
            {
                return false;
            }
            if (sPtr.Learned)
            {
                return known;
            }
            return !known;
        }

        public void TakeHit(int damage, string hitFrom)
        {
            bool penInvuln = false;
            int warning = MaxHealth * GlobalData.HitpointWarn / 10;
            if (IsDead)
            {
                return;
            }
            SaveGame.Instance.Disturb(true);
            if (TimedInvulnerability != 0 && damage < 9000)
            {
                if (Program.Rng.DieRoll(Constants.PenetrateInvulnerability) == 1)
                {
                    penInvuln = true;
                }
                else
                {
                    return;
                }
            }
            if (TimedEtherealness != 0)
            {
                damage /= 10;
                if (damage == 0 && Program.Rng.DieRoll(10) == 1)
                {
                    damage = 1;
                }
            }
            Health -= damage;
            RedrawNeeded.Set(RedrawFlag.PrHp);
            if (penInvuln)
            {
                Profile.Instance.MsgPrint("The attack penetrates your shield of invulnerability!");
            }
            if (Health < 0)
            {
                if (Program.Rng.DieRoll(10) <= Religion.GetNamedDeity(GodName.Zo_Kalar).AdjustedFavour)
                {
                    Profile.Instance.MsgPrint("Zo-Kalar's favour saves you from death!");
                    Health += damage;
                }
                else
                {
                    if (IsWizard && !Gui.GetCheck("Die? "))
                    {
                        Health += damage;
                    }
                    else
                    {
                        Gui.PlaySound(SoundEffect.PlayerDeath);
                        Profile.Instance.MsgPrint("You die.");
                        Profile.Instance.MsgPrint(null);
                        SaveGame.Instance.DiedFrom = hitFrom;
                        if (TimedHallucinations != 0)
                        {
                            SaveGame.Instance.DiedFrom += "(?)";
                        }
                        IsWinner = false;
                        IsDead = true;
                        return;
                    }
                }
            }
            if (Health < warning)
            {
                Gui.PlaySound(SoundEffect.HealthWarning);
                Profile.Instance.MsgPrint("*** LOW HITPOINT WARNING! ***");
                Profile.Instance.MsgPrint(null);
            }
        }

        public void ToggleRecall()
        {
            if (WordOfRecallDelay == 0)
            {
                WordOfRecallDelay = Program.Rng.DieRoll(20) + 15;
                Profile.Instance.MsgPrint("The air about you becomes charged...");
            }
            else
            {
                WordOfRecallDelay = 0;
                Profile.Instance.MsgPrint("A tension leaves the air around you...");
            }
        }

        public bool TryDecreasingAbilityScore(int stat)
        {
            bool sust = false;
            switch (stat)
            {
                case Ability.Strength:
                    if (HasSustainStrength)
                    {
                        sust = true;
                    }
                    break;

                case Ability.Intelligence:
                    if (HasSustainIntelligence)
                    {
                        sust = true;
                    }
                    break;

                case Ability.Wisdom:
                    if (HasSustainWisdom)
                    {
                        sust = true;
                    }
                    break;

                case Ability.Dexterity:
                    if (HasSustainDexterity)
                    {
                        sust = true;
                    }
                    break;

                case Ability.Constitution:
                    if (HasSustainConstitution)
                    {
                        sust = true;
                    }
                    break;

                case Ability.Charisma:
                    if (HasSustainCharisma)
                    {
                        sust = true;
                    }
                    break;
            }
            if (sust)
            {
                Profile.Instance.MsgPrint(
                    $"You feel {GlobalData.DescStatNeg[stat]} for a moment, but the feeling passes.");
                return true;
            }
            if (Program.Rng.DieRoll(10) <= Religion.GetNamedDeity(GodName.Lobon).AdjustedFavour)
            {
                Profile.Instance.MsgPrint($"You feel {GlobalData.DescStatNeg[stat]} for a moment, but Lobon's favour protects you.");
                return true;
            }
            if (DecreaseAbilityScore(stat, 10, false))
            {
                Profile.Instance.MsgPrint($"You feel very {GlobalData.DescStatNeg[stat]}.");
                return true;
            }
            return false;
        }

        public bool TryIncreasingAbilityScore(int stat)
        {
            bool res = RestoreAbilityScore(stat);
            if (IncreaseAbilityScore(stat))
            {
                Profile.Instance.MsgPrint($"Wow!  You feel very {GlobalData.DescStatPos[stat]}!");
                return true;
            }
            if (res)
            {
                Profile.Instance.MsgPrint($"You feel less {GlobalData.DescStatNeg[stat]}.");
                return true;
            }
            return false;
        }

        public bool TryRestoringAbilityScore(int stat)
        {
            if (RestoreAbilityScore(stat))
            {
                Profile.Instance.MsgPrint($"You feel less {GlobalData.DescStatNeg[stat]}.");
                return true;
            }
            return false;
        }

        private bool IncreaseAbilityScore(int which)
        {
            int value = AbilityScores[which].Innate;
            if (value < 18 + 100)
            {
                int gain;
                if (value < 18)
                {
                    gain = Program.Rng.RandomLessThan(100) < 75 ? 1 : 2;
                    value += gain;
                }
                else if (value < 18 + 98)
                {
                    gain = (((18 + 100 - value) / 2) + 3) / 2;
                    if (gain < 1)
                    {
                        gain = 1;
                    }
                    value += Program.Rng.DieRoll(gain) + (gain / 2);
                    if (value > 18 + 99)
                    {
                        value = 18 + 99;
                    }
                }
                else
                {
                    value++;
                }
                AbilityScores[which].Innate = value;
                if (value > AbilityScores[which].InnateMax)
                {
                    AbilityScores[which].InnateMax = value;
                }
                UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
                return true;
            }
            return false;
        }
    }
}