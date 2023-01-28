// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
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

        /// <summary>
        /// </summary>
        /// <param name="mode"> </param>
        /// <returns> </returns>
        public string MonsterDesc(int mode)
        {
            // * Mode Flags:
            // *   0x01 --&gt; Objective(or Reflexive)
            // *   0x02 --&gt; Possessive(or Reflexive)
            // *   0x04 --&gt; Use indefinites for hidden monsters("something")
            // *   0x08 --&gt; Use indefinites for visible monsters("a kobold")
            // *   0x10 --&gt; Pronominalize hidden monsters
            // *   0x20 --&gt; Pronominalize visible monsters
            // *   0x40 --&gt; Assume the monster is hidden
            // *   0x80 --&gt; Assume the monster is visible *
            // * Useful Modes:
            // *   0x00 --&gt; Full nominative name("the kobold") or "it"
            // *   0x04 --&gt; Full nominative name("the kobold") or "something"
            // *   0x80 --&gt; Genocide resistance name("the kobold")
            // *   0x88 --&gt; Killing name("a kobold")
            // *   0x22 --&gt; Possessive, genderized if visable("his") or "its"
            // *   0x23 --&gt; Reflexive, genderized if visable("himself") or "itself"
            if (Race == null)
            {
                return string.Empty;
            }
            string desc;
            var name = Race.Name;
            if (SaveGame.Instance.Player.TimedHallucinations != 0)
            {
                MonsterRace halluRace;
                do
                {
                    halluRace = Profile.Instance.MonsterRaces[Program.Rng.DieRoll(Profile.Instance.MonsterRaces.Count - 2)];
                } while ((halluRace.Flags1 & MonsterFlag1.Unique) != 0);
                var sillyName = halluRace.Name;
                name = sillyName;
            }
            var seen = (mode & 0x80) != 0 || ((mode & 0x40) == 0 && IsVisible);
            var pron = (seen && (mode & 0x20) != 0) || (!seen && (mode & 0x10) != 0);
            if (!seen || pron)
            {
                var kind = 0x00;
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
                var res = "it";
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
            var player = SaveGame.Instance.Player;
            var happened = false;
            var power = 100;
            if (necro)
            {
                Profile.Instance.MsgPrint("Your sanity is shaken by reading the Necronomicon!");
            }
            else
            {
                power = Race.Level + 10;
                var mName = MonsterDesc(0);
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
                if (player.Race.SanityImmune)
                {
                    Profile.Instance.MsgPrint("You've seen worse.");
                    return;
                }
                if (player.Race.SanityResistant)
                {
                    if (Program.Rng.DieRoll(100) < 25 + player.Level)
                    {
                        Profile.Instance.MsgPrint("You've seen worse.");
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
            if (Program.Rng.DieRoll(power) < player.SkillSavingThrow)
            {
                if (!player.HasChaosResistance)
                {
                    Profile.Instance.MsgPrint("The exposure to eldritch forces warps you.");
                    player.Dna.GainMutation();
                    player.UpdatesNeeded.Set(UpdateFlags.UpdateBonuses);
                    SaveGame.Instance.HandleStuff();
                }
            }
        }
    }
}