using Cthangband.Enumerations;
using Cthangband.StaticData;
using System;

namespace Cthangband
{
    [Serializable]
    internal class Monster
    {
        public int ConfusionLevel;

        /// <summary>
        /// How far away from the player the monster is
        /// </summary>
        public int DistanceFromPlayer;

        public int Energy;

        public int FearLevel;

        public int FirstHeldItemIndex;
        public int Generation;
        public int Health;
        public int IndividualMonsterFlags;
        public bool IsVisible;
        public int MapX;
        public int MapY;
        public int MaxHealth;
        public uint Mind;
        public MonsterRace Race;

        /// <summary>
        /// How deeply the monster is sleeping
        /// </summary>
        public int SleepLevel;

        public int Speed;
        public int StolenGold;
        public int StunLevel;
        private static readonly string[] _funnyComments = { "Wow, cosmic, man!", "Rad!", "Groovy!", "Cool!", "Far out!" };

        private static readonly string[] _funnyDesc =
        {
            "silly", "hilarious", "absurd", "insipid", "ridiculous", "laughable", "ludicrous", "far-out", "groovy",
            "postmodern", "fantastic", "dadaistic", "cubistic", "cosmic", "awesome", "incomprehensible", "fabulous",
            "amazing", "incredible", "chaotic", "wild", "preposterous"
        };

        private static readonly string[] _horrorDesc =
        {
            "abominable", "abysmal", "appalling", "baleful", "blasphemous", "disgusting", "dreadful", "filthy",
            "grisly", "hideous", "hellish", "horrible", "infernal", "loathsome", "nightmarish", "repulsive",
            "sacrilegious", "terrible", "unclean", "unspeakable"
        };

        public string MonsterDesc(int mode)
        {
            if (Race == null)
            {
                return string.Empty;
            }
            string desc;
            string name = Race.Name;
            if (SaveGame.Instance.Player.TimedHallucinations != 0)
            {
                MonsterRace halluRace;
                do
                {
                    halluRace = Profile.Instance.MonsterRaces[Program.Rng.DieRoll(Profile.Instance.MonsterRaces.Count - 2)];
                } while ((halluRace.Flags1 & MonsterFlag1.Unique) != 0);
                string sillyName = halluRace.Name;
                name = sillyName;
            }
            bool seen = (mode & 0x80) != 0 || ((mode & 0x40) == 0 && IsVisible);
            bool pron = (seen && (mode & 0x20) != 0) || (!seen && (mode & 0x10) != 0);
            if (!seen || pron)
            {
                int kind = 0x00;
                if ((Race.Flags1 & MonsterFlag1.Female) != 0)
                {
                    kind = 0x20;
                }
                else if ((Race.Flags1 & MonsterFlag1.Male) != 0)
                {
                    kind = 0x10;
                }
                if (!pron)
                {
                    kind = 0x00;
                }
                string res = "it";
                switch (kind + (mode & 0x07))
                {
                    case 0x00:
                        res = "it";
                        break;

                    case 0x01:
                        res = "it";
                        break;

                    case 0x02:
                        res = "its";
                        break;

                    case 0x03:
                        res = "itself";
                        break;

                    case 0x04:
                        res = "something";
                        break;

                    case 0x05:
                        res = "something";
                        break;

                    case 0x06:
                        res = "something's";
                        break;

                    case 0x07:
                        res = "itself";
                        break;

                    case 0x10:
                        res = "he";
                        break;

                    case 0x11:
                        res = "him";
                        break;

                    case 0x12:
                        res = "his";
                        break;

                    case 0x13:
                        res = "himself";
                        break;

                    case 0x14:
                        res = "someone";
                        break;

                    case 0x15:
                        res = "someone";
                        break;

                    case 0x16:
                        res = "someone's";
                        break;

                    case 0x17:
                        res = "himself";
                        break;

                    case 0x20:
                        res = "she";
                        break;

                    case 0x21:
                        res = "her";
                        break;

                    case 0x22:
                        res = "her";
                        break;

                    case 0x23:
                        res = "herself";
                        break;

                    case 0x24:
                        res = "someone";
                        break;

                    case 0x25:
                        res = "someone";
                        break;

                    case 0x26:
                        res = "someone's";
                        break;

                    case 0x27:
                        res = "herself";
                        break;
                }
                desc = res;
            }
            else if ((mode & 0x02) != 0 && (mode & 0x01) != 0)
            {
                if ((Race.Flags1 & MonsterFlag1.Female) != 0)
                {
                    desc = "herself";
                }
                else if ((Race.Flags1 & MonsterFlag1.Male) != 0)
                {
                    desc = "himself";
                }
                else
                {
                    desc = "itself";
                }
            }
            else
            {
                if ((Race.Flags1 & MonsterFlag1.Unique) != 0 && SaveGame.Instance.Player.TimedHallucinations == 0)
                {
                    desc = name;
                }
                else if ((mode & 0x08) != 0)
                {
                    desc = name[0].IsVowel() ? "an " : "a ";
                    desc += name;
                }
                else
                {
                    desc = (Mind & Constants.SmFriendly) != 0 ? "your " : "the ";
                    desc += name;
                }
                if ((mode & 0x02) != 0)
                {
                    desc += "'s";
                }
            }
            return desc;
        }

        public void SanityBlast(bool necro)
        {
            Player player = SaveGame.Instance.Player;
            bool happened = false;
            int power = 100;
            if (necro)
            {
                Profile.Instance.MsgPrint("Your sanity is shaken by reading the Necronomicon!");
            }
            else
            {
                power = Race.Level + 10;
                string mName = MonsterDesc(0);
                if ((Race.Flags1 & MonsterFlag1.Unique) != 0)
                {
                    power *= 2;
                }
                else
                {
                    if ((Race.Flags1 & MonsterFlag1.Friends) != 0)
                    {
                        power /= 2;
                    }
                }
                if (!SaveGame.Instance.HackMind)
                {
                    return;
                }
                if (!IsVisible)
                {
                    return;
                }
                if ((Race.Flags2 & MonsterFlag2.EldritchHorror) == 0)
                {
                    return;
                }
                if ((Mind & Constants.SmFriendly) != 0 && Program.Rng.DieRoll(8) != 1)
                {
                    return;
                }
                if (Program.Rng.DieRoll(power) < player.SkillSavingThrow)
                {
                    return;
                }
                if (player.TimedHallucinations != 0)
                {
                    Profile.Instance.MsgPrint(
                        $"You behold the {_funnyDesc[Program.Rng.DieRoll(Constants.MaxFunny) - 1]} visage of {mName}!");
                    if (Program.Rng.DieRoll(3) == 1)
                    {
                        Profile.Instance.MsgPrint(_funnyComments[Program.Rng.DieRoll(Constants.MaxComment) - 1]);
                        player.TimedHallucinations += Program.Rng.DieRoll(Race.Level);
                    }
                    return;
                }
                Profile.Instance.MsgPrint(
                    $"You behold the {_horrorDesc[Program.Rng.DieRoll(Constants.MaxHorror) - 1]} visage of {mName}!");
                Race.Knowledge.RFlags2 |= MonsterFlag2.EldritchHorror;
                if (player.RaceIndex == RaceId.Imp || player.RaceIndex == RaceId.MindFlayer)
                {
                    return;
                }
                if (player.RaceIndex == RaceId.Skeleton || player.RaceIndex == RaceId.Zombie ||
                    player.RaceIndex == RaceId.Vampire || player.RaceIndex == RaceId.Spectre)
                {
                    if (Program.Rng.DieRoll(100) < 25 + player.Level)
                    {
                        return;
                    }
                }
            }
            if (Program.Rng.DieRoll(power) < player.SkillSavingThrow)
            {
                if (!player.HasConfusionResistance)
                {
                    player.SetTimedConfusion(player.TimedConfusion + Program.Rng.RandomLessThan(4) + 4);
                }
                if (!player.HasChaosResistance && Program.Rng.DieRoll(3) == 1)
                {
                    player.SetTimedHallucinations(player.TimedHallucinations + Program.Rng.RandomLessThan(250) + 150);
                }
                return;
            }
            if (Program.Rng.DieRoll(power) < player.SkillSavingThrow)
            {
                player.TryDecreasingAbilityScore(Ability.Intelligence);
                player.TryDecreasingAbilityScore(Ability.Wisdom);
                return;
            }
            if (Program.Rng.DieRoll(power) < player.SkillSavingThrow)
            {
                if (!player.HasConfusionResistance)
                {
                    player.SetTimedConfusion(player.TimedConfusion + Program.Rng.RandomLessThan(4) + 4);
                }
                if (!player.HasFreeAction)
                {
                    player.SetTimedParalysis(player.TimedParalysis + Program.Rng.RandomLessThan(4) + 4);
                }
                while (Program.Rng.RandomLessThan(100) > player.SkillSavingThrow)
                {
                    player.TryDecreasingAbilityScore(Ability.Intelligence);
                }
                while (Program.Rng.RandomLessThan(100) > player.SkillSavingThrow)
                {
                    player.TryDecreasingAbilityScore(Ability.Wisdom);
                }
                if (!player.HasChaosResistance)
                {
                    player.SetTimedHallucinations(player.TimedHallucinations + Program.Rng.RandomLessThan(250) + 150);
                }
                return;
            }
            if (Program.Rng.DieRoll(power) < player.SkillSavingThrow)
            {
                if (player.DecreaseAbilityScore(Ability.Intelligence, 10, true))
                {
                    happened = true;
                }
                if (player.DecreaseAbilityScore(Ability.Wisdom, 10, true))
                {
                    happened = true;
                }
                if (happened)
                {
                    Profile.Instance.MsgPrint("You feel much less sane than before.");
                }
                return;
            }
            if (Program.Rng.DieRoll(power) < player.SkillSavingThrow)
            {
                if (SaveGame.Instance.SpellEffects.LoseAllInfo())
                {
                    Profile.Instance.MsgPrint("You forget everything in your utmost terror!");
                }
                return;
            }
            Profile.Instance.MsgPrint("The exposure to eldritch forces warps you.");
            player.Dna.GainMutation();
            player.UpdatesNeeded |= UpdateFlags.PuBonus;
            SaveGame.Instance.HandleStuff();
        }
    }
}