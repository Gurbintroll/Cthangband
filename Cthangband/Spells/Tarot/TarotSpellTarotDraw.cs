using Cthangband.Enumerations;
using Cthangband.Projection;
using Cthangband.StaticData;
using System;

namespace Cthangband.Spells.Tarot
{
    [Serializable]
    internal class TarotSpellTarotDraw : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            bool noneCame = false;
            int die = Program.Rng.DieRoll(120);
            if (player.ProfessionIndex == CharacterClass.Rogue || player.ProfessionIndex == CharacterClass.HighMage)
            {
                die = Program.Rng.DieRoll(110) + (player.Level / 5);
            }
            Profile.Instance.MsgPrint("You shuffle your Tarot deck and draw a card...");
            if (die < 7)
            {
                Profile.Instance.MsgPrint("Oh no! It's the Blasted Tower!");
                for (int dummy = 0; dummy < Program.Rng.DieRoll(3); dummy++)
                {
                    SaveGame.Instance.SpellEffects.ActivateHiSummon();
                }
            }
            else if (die < 14)
            {
                Profile.Instance.MsgPrint("Oh no! It's the Devil!");
                level.Monsters.SummonSpecific(player.MapY, player.MapX, SaveGame.Instance.Difficulty,
                    Constants.SummonDemon);
            }
            else if (die < 18)
            {
                Profile.Instance.MsgPrint("Oh no! It's the Hanged Man.");
                saveGame.ActivateDreadCurse();
            }
            else if (die < 22)
            {
                Profile.Instance.MsgPrint("It's the swords of discord.");
                saveGame.SpellEffects.AggravateMonsters(1);
            }
            else if (die < 26)
            {
                Profile.Instance.MsgPrint("It's the Fool.");
                player.TryDecreasingAbilityScore(Ability.Intelligence);
                player.TryDecreasingAbilityScore(Ability.Wisdom);
            }
            else if (die < 30)
            {
                Profile.Instance.MsgPrint("It's a picture of a strange monster.");
                if (!level.Monsters.SummonSpecific(player.MapY, player.MapX, saveGame.Difficulty * 3 / 2,
                    32 + Program.Rng.DieRoll(6)))
                {
                    noneCame = true;
                }
            }
            else if (die < 33)
            {
                Profile.Instance.MsgPrint("It's the Moon.");
                saveGame.SpellEffects.UnlightArea(10, 3);
            }
            else if (die < 38)
            {
                Profile.Instance.MsgPrint("It's the Wheel of Fortune.");
                WildMagic(Program.Rng.DieRoll(32) - 1, player, level);
            }
            else if (die < 40)
            {
                Profile.Instance.MsgPrint("It's a teleport card.");
                saveGame.SpellEffects.TeleportPlayer(10);
            }
            else if (die < 42)
            {
                Profile.Instance.MsgPrint("It's the Star.");
                player.SetTimedBlessing(player.TimedBlessing + player.Level);
            }
            else if (die < 47)
            {
                Profile.Instance.MsgPrint("It's a teleport card.");
                SaveGame.Instance.SpellEffects.TeleportPlayer(100);
            }
            else if (die < 52)
            {
                Profile.Instance.MsgPrint("It's a teleport card.");
                SaveGame.Instance.SpellEffects.TeleportPlayer(200);
            }
            else if (die < 60)
            {
                Profile.Instance.MsgPrint("It's the Tower.");
                SaveGame.Instance.SpellEffects.WallBreaker();
            }
            else if (die < 72)
            {
                Profile.Instance.MsgPrint("It's Temperance.");
                SaveGame.Instance.SpellEffects.SleepMonstersTouch();
            }
            else if (die < 80)
            {
                Profile.Instance.MsgPrint("It's the Tower.");
                SaveGame.Instance.SpellEffects.Earthquake(player.MapY, player.MapX, 5);
            }
            else if (die < 82)
            {
                Profile.Instance.MsgPrint("It's a picture of a friendly monster.");
                if (!level.Monsters.SummonSpecificFriendly(player.MapY, player.MapX, SaveGame.Instance.Difficulty * 3 / 2,
                    Constants.SummonBizarre1, false))
                {
                    noneCame = true;
                }
            }
            else if (die < 84)
            {
                Profile.Instance.MsgPrint("It's a picture of a friendly monster.");
                if (!level.Monsters.SummonSpecificFriendly(player.MapY, player.MapX, SaveGame.Instance.Difficulty * 3 / 2,
                    Constants.SummonBizarre2, false))
                {
                    noneCame = true;
                }
            }
            else if (die < 86)
            {
                Profile.Instance.MsgPrint("It's a picture of a friendly monster.");
                if (!level.Monsters.SummonSpecificFriendly(player.MapY, player.MapX, SaveGame.Instance.Difficulty * 3 / 2,
                    Constants.SummonBizarre4, false))
                {
                    noneCame = true;
                }
            }
            else if (die < 88)
            {
                Profile.Instance.MsgPrint("It's a picture of a friendly monster.");
                if (!level.Monsters.SummonSpecificFriendly(player.MapY, player.MapX, SaveGame.Instance.Difficulty * 3 / 2,
                    Constants.SummonBizarre5, false))
                {
                    noneCame = true;
                }
            }
            else if (die < 96)
            {
                Profile.Instance.MsgPrint("It's the Lovers.");
                if (!targetEngine.GetAimDir(out int dir))
                {
                    return;
                }
                SaveGame.Instance.SpellEffects.CharmMonster(dir, Math.Min(player.Level, 20));
            }
            else if (die < 101)
            {
                Profile.Instance.MsgPrint("It's the Hermit.");
                SaveGame.Instance.SpellEffects.WallStone();
            }
            else if (die < 111)
            {
                Profile.Instance.MsgPrint("It's the Judgement.");
                player.RerollHitPoints();
                if (player.Dna.HasMutations)
                {
                    Profile.Instance.MsgPrint("You are cured of all mutations.");
                    player.Dna.LoseAllMutations();
                    player.UpdatesNeeded |= UpdateFlags.PuBonus;
                    SaveGame.Instance.HandleStuff();
                }
            }
            else if (die < 120)
            {
                Profile.Instance.MsgPrint("It's the Sun.");
                level.WizLight();
            }
            else
            {
                Profile.Instance.MsgPrint("It's the World.");
                if (player.ExperiencePoints < Constants.PyMaxExp)
                {
                    int ee = (player.ExperiencePoints / 25) + 1;
                    if (ee > 5000)
                    {
                        ee = 5000;
                    }
                    Profile.Instance.MsgPrint("You feel more experienced.");
                    player.GainExperience(ee);
                }
            }
            if (noneCame)
            {
                Profile.Instance.MsgPrint("No-one ever turns up.");
            }
        }

        public override void Initialise(int characterClass)
        {
            Name = "Tarot Draw";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 5;
                    ManaCost = 5;
                    BaseFailure = 75;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.Priest:
                case CharacterClass.Monk:
                    Level = 6;
                    ManaCost = 5;
                    BaseFailure = 75;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.Rogue:
                    Level = 9;
                    ManaCost = 7;
                    BaseFailure = 75;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.Ranger:
                    Level = 9;
                    ManaCost = 7;
                    BaseFailure = 75;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 7;
                    ManaCost = 7;
                    BaseFailure = 75;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.HighMage:
                    Level = 4;
                    ManaCost = 4;
                    BaseFailure = 65;
                    FirstCastExperience = 8;
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
            return "random";
        }

        private void WildMagic(int spell, Player player, Level level)
        {
            int counter = 0;
            int type = Constants.SummonBizarre1 - 1 + Program.Rng.DieRoll(6);
            if (type < Constants.SummonBizarre1)
            {
                type = Constants.SummonBizarre1;
            }
            else if (type > Constants.SummonBizarre6)
            {
                type = Constants.SummonBizarre6;
            }
            switch (Program.Rng.DieRoll(spell) + Program.Rng.DieRoll(8) + 1)
            {
                case 1:
                case 2:
                case 3:
                    SaveGame.Instance.SpellEffects.TeleportPlayer(10);
                    break;

                case 4:
                case 5:
                case 6:
                    SaveGame.Instance.SpellEffects.TeleportPlayer(100);
                    break;

                case 7:
                case 8:
                    SaveGame.Instance.SpellEffects.TeleportPlayer(200);
                    break;

                case 9:
                case 10:
                case 11:
                    SaveGame.Instance.SpellEffects.UnlightArea(10, 3);
                    break;

                case 12:
                case 13:
                case 14:
                    SaveGame.Instance.SpellEffects.LightArea(Program.Rng.DiceRoll(2, 3), 2);
                    break;

                case 15:
                    SaveGame.Instance.SpellEffects.DestroyDoorsTouch();
                    break;

                case 16:
                case 17:
                    SaveGame.Instance.SpellEffects.WallBreaker();
                    break;

                case 18:
                    SaveGame.Instance.SpellEffects.SleepMonstersTouch();
                    break;

                case 19:
                case 20:
                    SaveGame.Instance.SpellEffects.TrapCreation();
                    break;

                case 21:
                case 22:
                    SaveGame.Instance.SpellEffects.DoorCreation();
                    break;

                case 23:
                case 24:
                case 25:
                    SaveGame.Instance.SpellEffects.AggravateMonsters(1);
                    break;

                case 26:
                    SaveGame.Instance.SpellEffects.Earthquake(player.MapY, player.MapX, 5);
                    break;

                case 27:
                case 28:
                    player.Dna.GainMutation();
                    break;

                case 29:
                case 30:
                    SaveGame.Instance.SpellEffects.ApplyDisenchant();
                    break;

                case 31:
                    SaveGame.Instance.SpellEffects.LoseAllInfo();
                    break;

                case 32:
                    SaveGame.Instance.SpellEffects.FireBall(new ProjectChaos(SaveGame.Instance.SpellEffects), 0, spell + 5, 1 + (spell / 10));
                    break;

                case 33:
                    SaveGame.Instance.SpellEffects.WallStone();
                    break;

                case 34:
                case 35:
                    while (counter++ < 8)
                    {
                        level.Monsters.SummonSpecific(player.MapY, player.MapX, SaveGame.Instance.Difficulty * 3 / 2, type);
                    }
                    break;

                case 36:
                case 37:
                    SaveGame.Instance.SpellEffects.ActivateHiSummon();
                    break;

                case 38:
                    SaveGame.Instance.SpellEffects.SummonReaver();
                    break;

                default:
                    SaveGame.Instance.ActivateDreadCurse();
                    break;
            }
        }
    }
}