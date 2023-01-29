// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Pantheon;
using Cthangband.Patron.Base;
using Cthangband.PlayerClass.Base;
using Cthangband.PlayerRace.Base;
using Cthangband.Spells;
using Cthangband.StaticData;
using Cthangband.UI;
using System.Collections.Generic;
using System.Linq;

namespace Cthangband
{
    internal class PlayerFactory
    {
        private readonly MenuItem[] _classMenu;

        private readonly string[] _menuItem = new string[32];

        private readonly MenuItem[] _raceMenu;

        private readonly Gender[] _sexInfo = { new Gender("Female", "Queen"), new Gender("Male", "King"), new Gender("Other", "Monarch") };

        private int _menuLength;

        private Player _player;

        private string _prevClass;

        private int _prevGeneration;

        private string _prevName;

        private string _prevRace;

        private Realm _prevRealm1;

        private Realm _prevRealm2;

        private int _prevSex;

        internal PlayerFactory()
        {
            var keys = PlayerRaces.Instance.Keys.ToList();
            keys.Sort();
            _raceMenu = new MenuItem[keys.Count];
            for (var i = 0; i < keys.Count; i++)
            {
                _raceMenu[i] = new MenuItem(keys[i], i);
            }
            keys = PlayerClasses.Instance.Keys.ToList();
            keys.Sort();
            _classMenu = new MenuItem[keys.Count];
            for (var i = 0; i < keys.Count; i++)
            {
                _classMenu[i] = new MenuItem(keys[i], i);
            }
        }

        public Player CharacterGeneration(ExPlayer ex)
        {
            Gui.SetBackground(Terminal.BackgroundImage.Paper);
            Gui.Mixer.Play(MusicTrack.Chargen);
            _player = new Player();
            if (PlayerBirth(ex))
            {
                return _player;
            }
            return null;
        }

        private void DisplayAPlusB(int x, int y, int initial, int bonus)
        {
            var buf = $"{initial:00}% + {bonus / 10}.{bonus % 10}%/lv";
            Gui.Print(Colour.Black, buf, y, x);
        }

        private void DisplayClassInfo(string pclass)
        {
            var playerClass = PlayerClasses.Instance[pclass];
            Gui.Print(Colour.Purple, "STR:", 36, 21);
            Gui.Print(Colour.Purple, "INT:", 37, 21);
            Gui.Print(Colour.Purple, "WIS:", 38, 21);
            Gui.Print(Colour.Purple, "DEX:", 39, 21);
            Gui.Print(Colour.Purple, "CON:", 40, 21);
            Gui.Print(Colour.Purple, "CHA:", 41, 21);
            for (var i = 0; i < 6; i++)
            {
                var bonus = playerClass.AbilityBonus[i];
                DisplayStatBonus(26, 36 + i, bonus);
            }
            Gui.Print(Colour.Purple, "Disarming   :", 36, 53);
            Gui.Print(Colour.Purple, "Magic Device:", 37, 53);
            Gui.Print(Colour.Purple, "Saving Throw:", 38, 53);
            Gui.Print(Colour.Purple, "Stealth     :", 39, 53);
            Gui.Print(Colour.Purple, "Fighting    :", 40, 53);
            Gui.Print(Colour.Purple, "Shooting    :", 41, 53);
            Gui.Print(Colour.Purple, "Experience  :", 36, 31);
            Gui.Print(Colour.Purple, "Hit Dice    :", 37, 31);
            Gui.Print(Colour.Purple, "Infravision :", 38, 31);
            Gui.Print(Colour.Purple, "Searching   :", 39, 31);
            Gui.Print(Colour.Purple, "Perception  :", 40, 31);
            DisplayAPlusB(67, 36, playerClass.BaseDisarmBonus, playerClass.DisarmBonusPerLevel);
            DisplayAPlusB(67, 37, playerClass.BaseDeviceBonus, playerClass.DeviceBonusPerLevel);
            DisplayAPlusB(67, 38, playerClass.BaseSaveBonus, playerClass.SaveBonusPerLevel);
            DisplayAPlusB(67, 39, playerClass.BaseStealthBonus * 4, playerClass.StealthBonusPerLevel * 4);
            DisplayAPlusB(67, 40, playerClass.BaseMeleeAttackBonus, playerClass.MeleeAttackBonusPerLevel);
            DisplayAPlusB(67, 41, playerClass.BaseRangedAttackBonus, playerClass.RangedAttackBonusPerLevel);
            var buf = "+" + playerClass.ExperienceFactor + "%";
            Gui.Print(Colour.Black, buf, 36, 45);
            buf = "1d" + playerClass.HitDieBonus;
            Gui.Print(Colour.Black, buf, 37, 45);
            Gui.Print(Colour.Black, "-", 38, 45);
            buf = $"{playerClass.BaseSearchBonus:00}%";
            Gui.Print(Colour.Black, buf, 39, 45);
            buf = $"{playerClass.BaseSearchFrequency:00}%";
            Gui.Print(Colour.Black, buf, 40, 45);
            Gui.Print(Colour.Purple, playerClass.DescriptionLine1, 29, 20);
            Gui.Print(Colour.Purple, playerClass.DescriptionLine2, 30, 20);
            Gui.Print(Colour.Purple, playerClass.DescriptionLine3, 31, 20);
            Gui.Print(Colour.Purple, playerClass.DescriptionLine4, 32, 20);
            Gui.Print(Colour.Purple, playerClass.DescriptionLine5, 33, 20);
            Gui.Print(Colour.Purple, playerClass.DescriptionLine6, 34, 20);
        }

        private void DisplayPartialCharacter(int stage)
        {
            int i;
            string str;
            const string spaces = "                 ";
            Gui.Clear(0);
            Gui.Print(Colour.Blue, "Name        :", 2, 1);
            Gui.Print(Colour.Brown, stage == 0 ? _prevName : spaces, 2, 15);
            Gui.Print(Colour.Blue, "Gender      :", 3, 1);
            if (stage == 0)
            {
                _player.Gender = _sexInfo[_prevSex];
                str = _player.Gender.Title;
            }
            else if (stage < 6)
            {
                str = spaces;
            }
            else
            {
                _player.Gender = _sexInfo[_player.GenderIndex];
                str = _player.Gender.Title;
            }
            Gui.Print(Colour.Brown, str, 3, 15);
            Gui.Print(Colour.Blue, "Race        :", 4, 1);
            if (stage == 0)
            {
                _player.Race = PlayerRaces.Instance[_prevRace];
                str = _player.Race.Title;
            }
            else if (stage < 3)
            {
                str = spaces;
            }
            else
            {
                _player.Race = PlayerRaces.Instance[_player.CurrentRace];
                str = _player.Race.Title;
            }
            Gui.Print(Colour.Brown, str, 4, 15);
            Gui.Print(Colour.Blue, "Class       :", 5, 1);
            if (stage == 0)
            {
                _player.PlayerClass = PlayerClasses.Instance[_prevClass];
                str = _player.PlayerClass.Title;
            }
            else if (stage < 2)
            {
                str = spaces;
            }
            else
            {
                _player.PlayerClass = PlayerClasses.Instance[_player.CurrentClass];
                str = _player.PlayerClass.Title;
            }
            Gui.Print(Colour.Brown, str, 5, 15);
            var buf = string.Empty;
            if (stage == 0)
            {
                if (_prevRealm1 != Realm.None)
                {
                    if (_prevRealm2 != Realm.None)
                    {
                        buf = Spellcasting.RealmName(_prevRealm1) + "/" + Spellcasting.RealmName(_prevRealm2);
                    }
                    else
                    {
                        buf = Spellcasting.RealmName(_prevRealm1);
                    }
                }
                if (_prevRealm1 != Realm.None || _prevRealm2 != Realm.None)
                {
                    Gui.Print(Colour.Blue, "Magic       :", 6, 1);
                }
                if (_prevRealm1 != Realm.None)
                {
                    Gui.Print(Colour.Brown, buf, 6, 15);
                }
            }
            else if (stage < 4)
            {
                str = spaces;
                Gui.Print(Colour.Blue, str, 6, 0);
                Gui.Print(Colour.Brown, str, 6, 15);
            }
            else
            {
                buf = string.Empty;
                if (_player.Realm1 != Realm.None)
                {
                    if (_player.Realm2 != Realm.None)
                    {
                        buf = Spellcasting.RealmName(_player.Realm1) + "/" + Spellcasting.RealmName(_player.Realm2);
                    }
                    else
                    {
                        buf = Spellcasting.RealmName(_player.Realm1);
                    }
                }
                if (_player.Realm1 != Realm.None || _player.Realm2 != Realm.None)
                {
                    Gui.Print(Colour.Blue, "Magic       :", 6, 1);
                }
                if (_player.Realm1 != Realm.None)
                {
                    Gui.Print(Colour.Brown, buf, 6, 15);
                }
            }
            Gui.Print(Colour.Blue, "Birthday", 2, 32);
            Gui.Print(Colour.Blue, "Age          ", 3, 32);
            Gui.Print(Colour.Blue, "Height       ", 4, 32);
            Gui.Print(Colour.Blue, "Weight       ", 5, 32);
            Gui.Print(Colour.Blue, "Social Class ", 6, 32);
            Gui.Print(Colour.Blue, "STR:", 2 + Ability.Strength, 61);
            Gui.Print(Colour.Blue, "INT:", 2 + Ability.Intelligence, 61);
            Gui.Print(Colour.Blue, "WIS:", 2 + Ability.Wisdom, 61);
            Gui.Print(Colour.Blue, "DEX:", 2 + Ability.Dexterity, 61);
            Gui.Print(Colour.Blue, "CON:", 2 + Ability.Constitution, 61);
            Gui.Print(Colour.Blue, "CHA:", 2 + Ability.Charisma, 61);
            Gui.Print(Colour.Blue, "STR:", 14 + Ability.Strength, 1);
            Gui.Print(Colour.Blue, "INT:", 14 + Ability.Intelligence, 1);
            Gui.Print(Colour.Blue, "WIS:", 14 + Ability.Wisdom, 1);
            Gui.Print(Colour.Blue, "DEX:", 14 + Ability.Dexterity, 1);
            Gui.Print(Colour.Blue, "CON:", 14 + Ability.Constitution, 1);
            Gui.Print(Colour.Blue, "CHA:", 14 + Ability.Charisma, 1);
            Gui.Print(Colour.Blue, "STR:", 22 + Ability.Strength, 1);
            Gui.Print(Colour.Blue, "INT:", 22 + Ability.Intelligence, 1);
            Gui.Print(Colour.Blue, "WIS:", 22 + Ability.Wisdom, 1);
            Gui.Print(Colour.Blue, "DEX:", 22 + Ability.Dexterity, 1);
            Gui.Print(Colour.Blue, "CON:", 22 + Ability.Constitution, 1);
            Gui.Print(Colour.Blue, "CHA:", 22 + Ability.Charisma, 1);
            Gui.Print(Colour.Purple, "Initial", 21, 6);
            Gui.Print(Colour.Brown, "Race Class Mods", 21, 14);
            Gui.Print(Colour.Green, "Actual", 21, 30);
            Gui.Print(Colour.Red, "Reduced", 21, 37);
            Gui.Print(Colour.Blue, "abcdefghijklm@", 21, 45);
            Gui.Print(Colour.Grey, "..............", 22, 45);
            Gui.Print(Colour.Grey, "..............", 23, 45);
            Gui.Print(Colour.Grey, "..............", 24, 45);
            Gui.Print(Colour.Grey, "..............", 25, 45);
            Gui.Print(Colour.Grey, "..............", 26, 45);
            Gui.Print(Colour.Grey, "..............", 27, 45);
            Gui.Print(Colour.Blue, "Modifications", 28, 45);

            if (stage < 2)
            {
                for (i = 0; i < 6; i++)
                {
                    Gui.Print(Colour.Brown, "   ", 22 + i, 20);
                }
            }
            else
            {
                for (i = 0; i < 6; i++)
                {
                    buf = _player.PlayerClass.AbilityBonus[i].ToString("+0;-0;+0").PadLeft(3);
                    Gui.Print(Colour.Brown, buf, 22 + i, 20);
                }
            }
            if (stage < 3)
            {
                for (i = 0; i < 6; i++)
                {
                    Gui.Print(Colour.Brown, "   ", 22 + i, 14);
                }
            }
            else
            {
                for (i = 0; i < 6; i++)
                {
                    buf = (_player.Race.AbilityBonus[i]).ToString("+0;-0;+0").PadLeft(3);
                    Gui.Print(Colour.Brown, buf, 22 + i, 14);
                }
            }
        }

        private void DisplayRaceInfo(string race)
        {
            var playerRace = PlayerRaces.Instance[race];
            Gui.Print(Colour.Purple, "STR:", 36, 21);
            Gui.Print(Colour.Purple, "INT:", 37, 21);
            Gui.Print(Colour.Purple, "WIS:", 38, 21);
            Gui.Print(Colour.Purple, "DEX:", 39, 21);
            Gui.Print(Colour.Purple, "CON:", 40, 21);
            Gui.Print(Colour.Purple, "CHA:", 41, 21);
            for (var i = 0; i < 6; i++)
            {
                var bonus = playerRace.AbilityBonus[i] + _player.PlayerClass.AbilityBonus[i];
                DisplayStatBonus(26, 36 + i, bonus);
            }
            Gui.Print(Colour.Purple, "Disarming   :", 36, 53);
            Gui.Print(Colour.Purple, "Magic Device:", 37, 53);
            Gui.Print(Colour.Purple, "Saving Throw:", 38, 53);
            Gui.Print(Colour.Purple, "Stealth     :", 39, 53);
            Gui.Print(Colour.Purple, "Fighting    :", 40, 53);
            Gui.Print(Colour.Purple, "Shooting    :", 41, 53);
            Gui.Print(Colour.Purple, "Experience  :", 36, 31);
            Gui.Print(Colour.Purple, "Hit Dice    :", 37, 31);
            Gui.Print(Colour.Purple, "Infravision :", 38, 31);
            Gui.Print(Colour.Purple, "Searching   :", 39, 31);
            Gui.Print(Colour.Purple, "Perception  :", 40, 31);
            DisplayAPlusB(67, 36, _player.PlayerClass.BaseDisarmBonus + playerRace.BaseDisarmBonus,
                _player.PlayerClass.DisarmBonusPerLevel);
            DisplayAPlusB(67, 37, _player.PlayerClass.BaseDeviceBonus + playerRace.BaseDeviceBonus,
                _player.PlayerClass.DeviceBonusPerLevel);
            DisplayAPlusB(67, 38, _player.PlayerClass.BaseSaveBonus + playerRace.BaseSaveBonus,
                _player.PlayerClass.SaveBonusPerLevel);
            DisplayAPlusB(67, 39, (_player.PlayerClass.BaseStealthBonus * 4) + (playerRace.BaseStealthBonus * 4),
                _player.PlayerClass.StealthBonusPerLevel * 4);
            DisplayAPlusB(67, 40, _player.PlayerClass.BaseMeleeAttackBonus + playerRace.BaseMeleeAttackBonus,
                _player.PlayerClass.MeleeAttackBonusPerLevel);
            DisplayAPlusB(67, 41, _player.PlayerClass.BaseRangedAttackBonus + playerRace.BaseRangedAttackBonus,
                _player.PlayerClass.RangedAttackBonusPerLevel);
            var buf = playerRace.ExperienceFactor + _player.PlayerClass.ExperienceFactor + "%";
            Gui.Print(Colour.Black, buf, 36, 45);
            buf = "1d" + (playerRace.HitDieBonus + _player.PlayerClass.HitDieBonus);
            Gui.Print(Colour.Black, buf, 37, 45);
            if (playerRace.Infravision == 0)
            {
                Gui.Print(Colour.Black, "nil", 38, 45);
            }
            else
            {
                buf = playerRace.Infravision + "0 feet";
                Gui.Print(Colour.Green, buf, 38, 45);
            }
            buf = $"{playerRace.BaseSearchBonus + _player.PlayerClass.BaseSearchBonus:00}%";
            Gui.Print(Colour.Black, buf, 39, 45);
            buf = $"{playerRace.BaseSearchFrequency + _player.PlayerClass.BaseSearchFrequency:00}%";
            Gui.Print(Colour.Black, buf, 40, 45);
            Gui.Print(Colour.Purple, playerRace.Description1, 29, 20);
            Gui.Print(Colour.Purple, playerRace.Description2, 30, 20);
            Gui.Print(Colour.Purple, playerRace.Description3, 31, 20);
            Gui.Print(Colour.Purple, playerRace.Description4, 32, 20);
            Gui.Print(Colour.Purple, playerRace.Description5, 33, 20);
            Gui.Print(Colour.Purple, playerRace.Description6, 34, 20);
        }

        private void DisplayRealmInfo(Realm prealm)
        {
            switch (prealm)
            {
                case Realm.Chaos:
                    Gui.Print(Colour.Purple, "The Chaos realm is the most destructive realm. It focuses", 30, 20);
                    Gui.Print(Colour.Purple, "on combat spells. It is a very good choice for anyone who", 31, 20);
                    Gui.Print(Colour.Purple, "wants to be able to damage their foes directly, but is ", 32, 20);
                    Gui.Print(Colour.Purple, "somewhat lacking in non-combat spells.", 33, 20);
                    break;

                case Realm.Corporeal:
                    Gui.Print(Colour.Purple, "The Corporeal realm contains spells that exclusively affect", 30, 20);
                    Gui.Print(Colour.Purple, "the caster's body, although some spells also indirectly", 31, 20);
                    Gui.Print(Colour.Purple, "affect other creatures or objects. The corporeal realm is", 32, 20);
                    Gui.Print(Colour.Purple, "particularly good at sensing spells.", 33, 20);
                    break;

                case Realm.Death:
                    Gui.Print(Colour.Purple, "The Death realm has a combination of life-draining spells,", 30, 20);
                    Gui.Print(Colour.Purple, "curses, and undead summoning. Like chaos, it is a very", 31, 20);
                    Gui.Print(Colour.Purple, "offensive realm.", 32, 20);
                    break;

                case Realm.Folk:
                    Gui.Print(Colour.Purple, "The Folk realm is the least specialised of all the realms.", 30, 20);
                    Gui.Print(Colour.Purple, "Folk magic is capable of doing any effect that is possible", 31, 20);
                    Gui.Print(Colour.Purple, "in other realms - but usually less effectively than the", 32, 20);
                    Gui.Print(Colour.Purple, "specialist realms.", 33, 20);
                    break;

                case Realm.Life:
                    Gui.Print(Colour.Purple, "The Life realm is devoted to healing and buffing, with some", 30, 20);
                    Gui.Print(Colour.Purple, "offensive capability against undead and demons. It is the", 31, 20);
                    Gui.Print(Colour.Purple, "most defensive of the realms.", 32, 20);
                    break;

                case Realm.Nature:
                    Gui.Print(Colour.Purple, "The Nature realm has a large number of summoning spells and", 30, 20);
                    Gui.Print(Colour.Purple, "miscellaneous spells, but little in the way of offensive", 31, 20);
                    Gui.Print(Colour.Purple, "and defensive capabilities.", 32, 20);
                    break;

                case Realm.Sorcery:
                    Gui.Print(Colour.Purple, "The Sorcery realm contains spells dealing with raw magic", 30, 20);
                    Gui.Print(Colour.Purple, "itself, for example spells dealing with magical items.", 31, 20);
                    Gui.Print(Colour.Purple, "It is the premier source of miscellaneous non-combat", 32, 20);
                    Gui.Print(Colour.Purple, "utility spells.", 33, 20);
                    break;

                case Realm.Tarot:
                    Gui.Print(Colour.Purple, "The Tarot realm is one of the most specialised realms of", 30, 20);
                    Gui.Print(Colour.Purple, "all, almost exclusively containing summoning and transport", 31, 20);
                    Gui.Print(Colour.Purple, "spells.", 32, 20);
                    break;
            }
        }

        private void DisplayStatBonus(int x, int y, int bonus)
        {
            string buf;
            if (bonus == 0)
            {
                Gui.Print(Colour.Black, "+0", y, x);
            }
            else if (bonus < 0)
            {
                buf = bonus.ToString();
                Gui.Print(Colour.BrightRed, buf, y, x);
            }
            else
            {
                buf = "+" + bonus;
                Gui.Print(Colour.Green, buf, y, x);
            }
        }

        private void GetAhw()
        {
            _player.Age = _player.Race.BaseAge + Program.Rng.DieRoll(_player.Race.AgeRange);
            _player.GameTime = new GameTime(Program.Rng.DieRoll(365), _player.Race.IsNocturnal);
            if (_player.GenderIndex == Constants.SexMale)
            {
                _player.Height = Program.Rng.RandomNormal(_player.Race.MaleBaseHeight, _player.Race.MaleHeightRange);
                _player.Weight = Program.Rng.RandomNormal(_player.Race.MaleBaseWeight, _player.Race.MaleWeightRange);
            }
            else if (_player.GenderIndex == Constants.SexFemale)
            {
                _player.Height = Program.Rng.RandomNormal(_player.Race.FemaleBaseHeight, _player.Race.FemaleHeightRange);
                _player.Weight = Program.Rng.RandomNormal(_player.Race.FemaleBaseWeight, _player.Race.FemaleWeightRange);
            }
            else
            {
                if (Program.Rng.DieRoll(2) == 1)
                {
                    _player.Height = Program.Rng.RandomNormal(_player.Race.MaleBaseHeight, _player.Race.MaleHeightRange);
                    _player.Weight = Program.Rng.RandomNormal(_player.Race.MaleBaseWeight, _player.Race.MaleWeightRange);
                }
                else
                {
                    _player.Height = Program.Rng.RandomNormal(_player.Race.FemaleBaseHeight, _player.Race.FemaleHeightRange);
                    _player.Weight = Program.Rng.RandomNormal(_player.Race.FemaleBaseWeight, _player.Race.FemaleWeightRange);
                }
            }
        }

        private void GetExtra()
        {
            int i;
            _player.MaxLevelGained = 1;
            _player.Level = 1;
            _player.ExperienceMultiplier = _player.Race.ExperienceFactor + _player.PlayerClass.ExperienceFactor;
            _player.HitDie = _player.Race.HitDieBonus + _player.PlayerClass.HitDieBonus;
            _player.MaxHealth = _player.HitDie;
            _player.PlayerHp[0] = _player.HitDie;
            var lastroll = _player.HitDie;
            for (i = 1; i < Constants.PyMaxLevel; i++)
            {
                _player.PlayerHp[i] = lastroll;
                lastroll--;
                if (lastroll < 1)
                {
                    lastroll = _player.HitDie;
                }
            }
            for (i = 1; i < Constants.PyMaxLevel; i++)
            {
                var j = Program.Rng.DieRoll(Constants.PyMaxLevel - 1);
                lastroll = _player.PlayerHp[i];
                _player.PlayerHp[i] = _player.PlayerHp[j];
                _player.PlayerHp[j] = lastroll;
            }
            for (i = 1; i < Constants.PyMaxLevel; i++)
            {
                _player.PlayerHp[i] = _player.PlayerHp[i - 1] + _player.PlayerHp[i];
            }
        }

        private void GetMoney()
        {
            var gold = (_player.SocialClass * 6) + Program.Rng.DieRoll(100) + 300;
            for (var i = 0; i < 6; i++)
            {
                if (_player.AbilityScores[i].Adjusted >= 18 + 50)
                {
                    gold -= 300;
                }
                else if (_player.AbilityScores[i].Adjusted >= 18 + 20)
                {
                    gold -= 200;
                }
                else if (_player.AbilityScores[i].Adjusted > 18)
                {
                    gold -= 150;
                }
                else
                {
                    gold -= (_player.AbilityScores[i].Adjusted - 8) * 10;
                }
            }
            if (gold < 100)
            {
                gold = 100;
            }
            _player.Gold = gold;
        }

        private List<Realm> GetRealmList(Realm realm)
        {
            if (realm == Realm.None)
            {
                return new List<Realm> { Realm.None };
            }
            var list = new List<Realm>();
            if ((realm & Realm.Chaos) != 0 && _player.Realm1 != Realm.Chaos)
            {
                list.Add(Realm.Chaos);
            }
            if ((realm & Realm.Corporeal) != 0 && _player.Realm1 != Realm.Corporeal)
            {
                list.Add(Realm.Corporeal);
            }
            if ((realm & Realm.Death) != 0 && _player.Realm1 != Realm.Death)
            {
                list.Add(Realm.Death);
            }
            if ((realm & Realm.Folk) != 0 && _player.Realm1 != Realm.Folk)
            {
                list.Add(Realm.Folk);
            }
            if ((realm & Realm.Life) != 0 && _player.Realm1 != Realm.Life)
            {
                list.Add(Realm.Life);
            }
            if ((realm & Realm.Nature) != 0 && _player.Realm1 != Realm.Nature)
            {
                list.Add(Realm.Nature);
            }
            if ((realm & Realm.Sorcery) != 0 && _player.Realm1 != Realm.Sorcery)
            {
                list.Add(Realm.Sorcery);
            }
            if ((realm & Realm.Tarot) != 0 && _player.Realm1 != Realm.Tarot)
            {
                list.Add(Realm.Tarot);
            }
            return list;
        }

        private void GetRealmsRandomly()
        {
            var pclas = _player.PlayerClass;
            _player.Realm1 = Realm.None;
            _player.Realm2 = Realm.None;
            _player.Realm1 = pclas.ChooseRealmRandomly(pclas.FirstRealmChoice, _player);
            _player.Realm2 = pclas.ChooseRealmRandomly(pclas.SecondRealmChoice, _player);
            if (pclas.HasDeity)
            {
                switch (_player.Realm2)
                {
                    case Realm.Nature:
                        _player.Religion.Deity = GodName.Hagarg_Ryonis;
                        break;

                    case Realm.Folk:
                        _player.Religion.Deity = GodName.Zo_Kalar;
                        break;

                    case Realm.Chaos:
                        _player.Religion.Deity = GodName.Nath_Horthah;
                        break;

                    case Realm.Corporeal:
                        _player.Religion.Deity = GodName.Lobon;
                        break;

                    case Realm.Tarot:
                        _player.Religion.Deity = GodName.Tamash;
                        break;

                    default:
                        _player.Religion.Deity = GodName.None;
                        break;
                }
            }
            else
            {
                _player.Religion.Deity = GodName.None;
            }
        }

        private void GetStats()
        {
            int i, j;
            while (true)
            {
                var dice = new List<int>() { 17, 16, 14, 12, 11, 10 };
                for (i = 0; i < 6; i++)
                {
                    var index = Program.Rng.DieRoll(dice.Count) - 1;
                    j = dice[index];
                    dice.RemoveAt(index);
                    _player.AbilityScores[i].InnateMax = j;
                    var bonus = _player.Race.AbilityBonus[i] + _player.PlayerClass.AbilityBonus[i];
                    _player.AbilityScores[i].Innate = _player.AbilityScores[i].InnateMax;
                    _player.AbilityScores[i].Adjusted = _player.AbilityScores[i]
                        .ModifyStatValue(_player.AbilityScores[i].InnateMax, bonus);
                }
                if (_player.AbilityScores[_player.PlayerClass.PrimeAbilityScore].InnateMax > 13)
                {
                    break;
                }
            }
        }

        private void MenuDisplay(int current)
        {
            Gui.Clear(30);
            Gui.Print(Colour.Orange, "=>", 35, 0);
            for (var i = 0; i < _menuLength; i++)
            {
                var row = 35 + i - current;
                if (row >= 30 && row <= 40)
                {
                    var a = Colour.Purple;
                    if (i == current)
                    {
                        a = Colour.Pink;
                    }
                    Gui.Print(a, _menuItem[i], row, 2);
                }
            }
        }

        private bool PlayerBirth(ExPlayer ex)
        {
            if (ex == null)
            {
                _prevSex = Constants.SexFemale;
                _prevRace = "Human";
                _prevClass = "Warrior";
                _prevRealm1 = Realm.None;
                _prevRealm2 = Realm.None;
                _prevName = "Xena";
                _prevGeneration = 0;
            }
            else
            {
                _prevSex = ex.GenderIndex;
                _prevRace = ex.BirthRace;
                _prevClass = ex.CurrentClass;
                _prevRealm1 = ex.Realm1;
                _prevRealm2 = ex.Realm2;
                _prevName = ex.Name;
                _prevGeneration = ex.Generation;
            }
            if (!PlayerBirthAux())
            {
                return false;
            }
            _player.BirthRace = _player.CurrentRace;
            SaveGame.Instance.Quests.PlayerBirthQuests();
            Profile.Instance.MessageAdd(" ");
            Profile.Instance.MessageAdd("  ");
            Profile.Instance.MessageAdd("====================");
            Profile.Instance.MessageAdd("  ");
            Profile.Instance.MessageAdd(" ");
            _player.IsDead = false;
            PlayerOutfit();
            return true;
        }

        private bool PlayerBirthAux()
        {
            int i;
            var stage = 0;
            var menu = new int[9];
            var autoChose = new bool[8];
            for (i = 0; i < 8; i++)
            {
                menu[i] = 0;
            }
            menu[BirthStage.ClassSelection] = 14;
            menu[BirthStage.RaceSelection] = 16;
            Gui.Clear();
            var viewMode = 1;
            while (true)
            {
                char c;
                switch (stage)
                {
                    case BirthStage.Introduction:
                        _player.Religion.Deity = GodName.None;
                        for (i = 0; i < 8; i++)
                        {
                            autoChose[i] = false;
                        }
                        _menuItem[0] = "Choose";
                        _menuItem[1] = "Random";
                        _menuItem[2] = "Re-use";
                        _menuLength = 3;
                        DisplayPartialCharacter(stage);
                        if (menu[stage] >= _menuLength)
                        {
                            menu[stage] = 0;
                        }
                        MenuDisplay(menu[stage]);
                        switch (menu[stage])
                        {
                            case 0:
                                Gui.Print(Colour.Purple, "Choose your character's race, sex, and class; and select", 35,
                                    20);
                                Gui.Print(Colour.Purple, "which realms of magic your character will use.", 36, 20);
                                break;

                            case 1:
                                Gui.Print(Colour.Purple, "Let the game generate a character for you randomly.", 35, 20);
                                break;

                            case 2:
                                Gui.Print(Colour.Purple, "Re-play with a character similar to the one you played", 35,
                                    20);
                                Gui.Print(Colour.Purple, "last time.", 36, 20);
                                break;
                        }
                        Gui.Print(Colour.Orange,
                            "[Use up and down to select an option, right to confirm, or left to go back.]", 43, 1);
                        while (true)
                        {
                            c = Gui.Inkey();
                            if (c == '8')
                            {
                                if (menu[stage] > 0)
                                {
                                    menu[stage]--;
                                    break;
                                }
                            }
                            if (c == '2')
                            {
                                if (menu[stage] < _menuLength - 1)
                                {
                                    menu[stage]++;
                                    break;
                                }
                            }
                            if (c == '6')
                            {
                                stage++;
                                break;
                            }
                            if (c == '4')
                            {
                                return false;
                            }
                            if (c == 'h')
                            {
                                Gui.ShowManual();
                            }
                        }
                        break;

                    case BirthStage.ClassSelection:
                        _player.Religion.Deity = GodName.None;
                        if (menu[0] == Constants.GenerateReplay)
                        {
                            autoChose[stage] = true;
                            _player.CurrentClass = _prevClass;
                            _player.PlayerClass = PlayerClasses.Instance[_player.CurrentClass];
                            stage++;
                            break;
                        }
                        if (menu[0] == Constants.GenerateRandom)
                        {
                            autoChose[stage] = true;
                            _player.CurrentClass = PlayerClasses.Instance.RandomClassName();
                            _player.PlayerClass = PlayerClasses.Instance[_player.CurrentClass];
                            stage++;
                            break;
                        }
                        autoChose[stage] = false;
                        _menuLength = Constants.MaxClass;
                        for (i = 0; i < Constants.MaxClass; i++)
                        {
                            _menuItem[i] = _classMenu[i].Text;
                        }
                        DisplayPartialCharacter(stage);
                        if (menu[stage] >= _menuLength)
                        {
                            menu[stage] = 0;
                        }
                        MenuDisplay(menu[stage]);
                        DisplayClassInfo(_classMenu[menu[stage]].Text);
                        Gui.Print(Colour.Orange,
                            "[Use up and down to select an option, right to confirm, or left to go back.]", 43, 1);
                        while (true)
                        {
                            c = Gui.Inkey();
                            if (c == '8')
                            {
                                if (menu[stage] > 0)
                                {
                                    menu[stage]--;
                                    break;
                                }
                            }
                            if (c == '2')
                            {
                                if (menu[stage] < _menuLength - 1)
                                {
                                    menu[stage]++;
                                    break;
                                }
                            }
                            if (c == '6')
                            {
                                stage++;
                                break;
                            }
                            if (c == '4')
                            {
                                do
                                {
                                    stage--;
                                }
                                while (autoChose[stage]);
                                break;
                            }
                            if (c == 'h')
                            {
                                Gui.ShowManual();
                            }
                        }
                        if (stage > BirthStage.ClassSelection)
                        {
                            _player.CurrentClass = _classMenu[menu[BirthStage.ClassSelection]].Text;
                            _player.PlayerClass = PlayerClasses.Instance[_player.CurrentClass];
                        }
                        break;

                    case BirthStage.RaceSelection:
                        if (menu[0] == Constants.GenerateReplay)
                        {
                            autoChose[stage] = true;
                            _player.CurrentRace = _prevRace;
                            _player.Race = PlayerRaces.Instance[_player.CurrentRace];
                            _player.GetFirstLevelMutation = _player.Race.Mutates;
                            stage++;
                            break;
                        }
                        if (menu[0] == Constants.GenerateRandom)
                        {
                            autoChose[stage] = true;
                            do
                            {
                                var k = PlayerRaces.Instance.RandomRaceName();
                                _player.CurrentRace = k;
                                _player.Race = PlayerRaces.Instance[_player.CurrentRace];
                                _player.GetFirstLevelMutation = _player.Race.Mutates;
                            }
                            while (_player.Race.AbilityBonus[_player.PlayerClass.PrimeAbilityScore] < 0);
                            stage++;
                            break;
                        }
                        autoChose[stage] = false;
                        _menuLength = PlayerRaces.Instance.Count;
                        for (i = 0; i < _menuLength; i++)
                        {
                            _menuItem[i] = _raceMenu[i].Text;
                        }
                        DisplayPartialCharacter(stage);
                        if (menu[stage] >= _menuLength)
                        {
                            menu[stage] = 0;
                        }
                        MenuDisplay(menu[stage]);
                        DisplayRaceInfo(_raceMenu[menu[stage]].Text);
                        Gui.Print(Colour.Orange,
                            "[Use up and down to select an option, right to confirm, or left to go back.]", 43, 1);
                        while (true)
                        {
                            c = Gui.Inkey();
                            if (c == '8')
                            {
                                if (menu[stage] > 0)
                                {
                                    menu[stage]--;
                                    break;
                                }
                            }
                            if (c == '2')
                            {
                                if (menu[stage] < _menuLength - 1)
                                {
                                    menu[stage]++;
                                    break;
                                }
                            }
                            if (c == '6')
                            {
                                stage++;
                                break;
                            }
                            if (c == '4')
                            {
                                do
                                {
                                    stage--;
                                }
                                while (autoChose[stage]);
                                break;
                            }
                            if (c == 'h')
                            {
                                Gui.ShowManual();
                            }
                        }
                        if (stage > BirthStage.RaceSelection)
                        {
                            _player.CurrentRace = _raceMenu[menu[BirthStage.RaceSelection]].Text;
                            _player.Race = PlayerRaces.Instance[_player.CurrentRace];
                            _player.GetFirstLevelMutation = _player.Race.Mutates;
                        }
                        break;

                    case BirthStage.RealmSelection1:
                        if (menu[0] == Constants.GenerateReplay)
                        {
                            autoChose[stage] = true;
                            _player.Realm1 = _prevRealm1;
                            stage++;
                            break;
                        }
                        if (menu[0] == Constants.GenerateRandom)
                        {
                            autoChose[stage] = true;
                            GetRealmsRandomly();
                            stage++;
                            break;
                        }
                        _player.Realm1 = Realm.None;
                        var realmList = GetRealmList(_player.PlayerClass.FirstRealmChoice);
                        _menuLength = realmList.Count;
                        if (_menuLength == 1)
                        {
                            autoChose[stage] = true;
                            _player.Realm1 = realmList[0];
                            stage++;
                            break;
                        }
                        autoChose[stage] = false;
                        for (i = 0; i < _menuLength; i++)
                        {
                            _menuItem[i] = Spellcasting.RealmName(realmList[i]);
                        }
                        DisplayPartialCharacter(stage);
                        if (menu[stage] >= _menuLength)
                        {
                            menu[stage] = 0;
                        }
                        MenuDisplay(menu[stage]);
                        DisplayRealmInfo(realmList[menu[stage]]);
                        Gui.Print(Colour.Orange,
                            "[Use up and down to select an option, right to confirm, or left to go back.]", 43, 1);
                        while (true)
                        {
                            c = Gui.Inkey();
                            if (c == '8')
                            {
                                if (menu[stage] > 0)
                                {
                                    menu[stage]--;
                                    break;
                                }
                            }
                            if (c == '2')
                            {
                                if (menu[stage] < _menuLength - 1)
                                {
                                    menu[stage]++;
                                    break;
                                }
                            }
                            if (c == '6')
                            {
                                stage++;
                                break;
                            }
                            if (c == '4')
                            {
                                do
                                {
                                    stage--;
                                }
                                while (autoChose[stage]);
                                break;
                            }
                            if (c == 'h')
                            {
                                Gui.ShowManual();
                            }
                        }
                        if (stage > BirthStage.RealmSelection1)
                        {
                            _player.Realm1 = realmList[menu[BirthStage.RealmSelection1]];
                        }
                        break;

                    case BirthStage.RealmSelection2:
                        if (menu[0] == Constants.GenerateReplay)
                        {
                            autoChose[stage] = true;
                            _player.Realm2 = _prevRealm2;
                            if (_player.PlayerClass.HasDeity)
                            {
                                switch (_player.Realm2)
                                {
                                    case Realm.Nature:
                                        _player.Religion.Deity = GodName.Hagarg_Ryonis;
                                        break;

                                    case Realm.Folk:
                                        _player.Religion.Deity = GodName.Zo_Kalar;
                                        break;

                                    case Realm.Chaos:
                                        _player.Religion.Deity = GodName.Nath_Horthah;
                                        break;

                                    case Realm.Corporeal:
                                        _player.Religion.Deity = GodName.Lobon;
                                        break;

                                    case Realm.Tarot:
                                        _player.Religion.Deity = GodName.Tamash;
                                        break;

                                    default:
                                        _player.Religion.Deity = GodName.None;
                                        break;
                                }
                            }
                            else
                            {
                                _player.Religion.Deity = GodName.None;
                            }
                            stage++;
                            break;
                        }
                        if (menu[0] == Constants.GenerateRandom)
                        {
                            autoChose[stage] = true;
                            stage++;
                            break;
                        }
                        _player.Realm2 = Realm.None;
                        realmList = GetRealmList(_player.PlayerClass.SecondRealmChoice);
                        _menuLength = realmList.Count;
                        if (_menuLength == 1)
                        {
                            autoChose[stage] = true;
                            _player.Realm2 = realmList[0];
                            stage++;
                            break;
                        }
                        autoChose[stage] = false;
                        for (i = 0; i < _menuLength; i++)
                        {
                            _menuItem[i] = Spellcasting.RealmName(realmList[i]);
                        }
                        DisplayPartialCharacter(stage);
                        if (menu[stage] >= _menuLength)
                        {
                            menu[stage] = 0;
                        }
                        MenuDisplay(menu[stage]);
                        DisplayRealmInfo(realmList[menu[stage]]);
                        Gui.Print(Colour.Orange,
                            "[Use up and down to select an option, right to confirm, or left to go back.]", 43, 1);
                        while (true)
                        {
                            c = Gui.Inkey();
                            if (c == '8')
                            {
                                if (menu[stage] > 0)
                                {
                                    menu[stage]--;
                                    break;
                                }
                            }
                            if (c == '2')
                            {
                                if (menu[stage] < _menuLength - 1)
                                {
                                    menu[stage]++;
                                    break;
                                }
                            }
                            if (c == '6')
                            {
                                stage++;
                                break;
                            }
                            if (c == '4')
                            {
                                do
                                {
                                    stage--;
                                }
                                while (autoChose[stage]);
                                break;
                            }
                            if (c == 'h')
                            {
                                Gui.ShowManual();
                            }
                        }
                        if (stage > BirthStage.RealmSelection2)
                        {
                            _player.Realm2 = realmList[menu[BirthStage.RealmSelection2]];
                            if (_player.PlayerClass.HasDeity)
                            {
                                switch (_player.Realm2)
                                {
                                    case Realm.Nature:
                                        _player.Religion.Deity = GodName.Hagarg_Ryonis;
                                        break;

                                    case Realm.Folk:
                                        _player.Religion.Deity = GodName.Zo_Kalar;
                                        break;

                                    case Realm.Chaos:
                                        _player.Religion.Deity = GodName.Nath_Horthah;
                                        break;

                                    case Realm.Corporeal:
                                        _player.Religion.Deity = GodName.Lobon;
                                        break;

                                    case Realm.Tarot:
                                        _player.Religion.Deity = GodName.Tamash;
                                        break;

                                    default:
                                        _player.Religion.Deity = GodName.None;
                                        break;
                                }
                            }
                            else
                            {
                                _player.Religion.Deity = GodName.None;
                            }
                        }
                        break;

                    case BirthStage.GenderSelection:
                        if (menu[0] == Constants.GenerateReplay)
                        {
                            autoChose[stage] = true;
                            _player.GenderIndex = _prevSex;
                            _player.Gender = _sexInfo[_player.GenderIndex];
                            stage++;
                            break;
                        }
                        if (menu[0] == Constants.GenerateRandom)
                        {
                            autoChose[stage] = true;
                            _player.GenderIndex = Program.Rng.RandomBetween(0, 1);
                            _player.Gender = _sexInfo[_player.GenderIndex];
                            stage++;
                            break;
                        }
                        _menuLength = Constants.MaxGenders;
                        for (i = 0; i < Constants.MaxGenders; i++)
                        {
                            _menuItem[i] = _sexInfo[i].Title;
                        }
                        DisplayPartialCharacter(stage);
                        autoChose[stage] = false;
                        if (menu[stage] >= _menuLength)
                        {
                            menu[stage] = 0;
                        }
                        MenuDisplay(menu[stage]);
                        Gui.Print(Colour.Purple, "Your sex has no effect on gameplay.", 35, 21);
                        Gui.Print(Colour.Orange,
                            "[Use up and down to select an option, right to confirm, or left to go back.]", 43, 1);
                        while (true)
                        {
                            c = Gui.Inkey();
                            if (c == '8')
                            {
                                if (menu[stage] > 0)
                                {
                                    menu[stage]--;
                                    break;
                                }
                            }
                            if (c == '2')
                            {
                                if (menu[stage] < _menuLength - 1)
                                {
                                    menu[stage]++;
                                    break;
                                }
                            }
                            if (c == '6')
                            {
                                stage++;
                                break;
                            }
                            if (c == '4')
                            {
                                do
                                {
                                    stage--;
                                }
                                while (autoChose[stage]);
                                break;
                            }
                            if (c == 'h')
                            {
                                Gui.ShowManual();
                            }
                        }
                        if (stage > BirthStage.GenderSelection)
                        {
                            _player.GenderIndex = menu[BirthStage.GenderSelection];
                            _player.Gender = _sexInfo[_player.GenderIndex];
                        }
                        break;

                    case BirthStage.Confirmation:
                        if (menu[0] != Constants.GenerateReplay)
                        {
                            _player.Name = _player.Race.CreateRandomName();
                            _player.Generation = 1;
                        }
                        else
                        {
                            _player.Name = string.IsNullOrEmpty(_prevName)
                                ? _player.Race.CreateRandomName()
                                : _prevName;
                            _player.Generation = _prevGeneration + 1;
                        }
                        GetStats();
                        GetExtra();
                        GetAhw();
                        _player.Race.GetHistory(_player);
                        GetMoney();
                        _player.Spellcasting = new Spellcasting(_player);
                        _player.GooPatron = Patrons.Instance[Patrons.Instance.RandomPatronName()];
                        _player.UpdatesNeeded.Set(UpdateFlags.UpdateBonuses | UpdateFlags.UpdateHealth);
                        SaveGame.Instance.Player = _player;
                        SaveGame.Instance.UpdateStuff();
                        SaveGame.Instance.Player = null;
                        _player.Health = _player.MaxHealth;
                        _player.Vis = _player.MaxVis;
                        _player.Energy = 150;
                        var characterViewer = new CharacterViewer(_player);
                        while (true)
                        {
                            characterViewer.DisplayPlayer();
                            Gui.Print(Colour.Orange,
                                "[Use return to confirm, or left to go back.]", 43, 1);
                            c = Gui.Inkey();
                            if (c == 13)
                            {
                                stage++;
                                break;
                            }
                            if (c == '4')
                            {
                                do
                                {
                                    stage--;
                                }
                                while (autoChose[stage]);
                                break;
                            }
                            if (c == '8')
                            {
                                viewMode++;
                                if (viewMode > 1)
                                {
                                    viewMode = 0;
                                }
                            }
                            if (c == '2')
                            {
                                viewMode--;
                                if (viewMode < 0)
                                {
                                    viewMode = 1;
                                }
                            }
                            if (c == 'h')
                            {
                                Gui.ShowManual();
                            }
                        }
                        break;

                    case BirthStage.Naming:
                        _player.InputPlayerName();
                        return true;
                }
            }
        }

        private void PlayerOutfit()
        {
            var item = new Item();
            if (_player.Race.DoesntEat)
            {
                item.AssignItemType(
                    Profile.Instance.ItemTypes.LookupKind(ItemCategory.Scroll, ScrollType.SatisfyHunger));
                item.Count = (char)Program.Rng.RandomBetween(2, 5);
                item.BecomeFlavourAware();
                item.BecomeKnown();
                item.IdentifyFlags.Set(Constants.IdentStoreb);
                _player.Inventory.InvenCarry(item, false);
                item = new Item();
            }
            else
            {
                item.AssignItemType(Profile.Instance.ItemTypes.LookupKind(ItemCategory.Food, FoodType.Ration));
                item.Count = Program.Rng.RandomBetween(3, 7);
                item.BecomeFlavourAware();
                item.BecomeKnown();
                _player.Inventory.InvenCarry(item, false);
                item = new Item();
            }
            if (_player.Race.Glows || _player.PlayerClass.Glows)
            {
                item.AssignItemType(
                    Profile.Instance.ItemTypes.LookupKind(ItemCategory.Scroll, ScrollType.Light));
                item.Count = Program.Rng.RandomBetween(3, 7);
                item.BecomeFlavourAware();
                item.BecomeKnown();
                item.IdentifyFlags.Set(Constants.IdentStoreb);
                _player.Inventory.InvenCarry(item, false);
            }
            else
            {
                item.AssignItemType(Profile.Instance.ItemTypes.LookupKind(ItemCategory.Light, LightType.Torch));
                item.Count = Program.Rng.RandomBetween(3, 7);
                item.TypeSpecificValue = Program.Rng.RandomBetween(3, 7) * 500;
                item.BecomeFlavourAware();
                item.BecomeKnown();
                _player.Inventory.InvenCarry(item, false);
                var carried = new Item(item) { Count = 1 };
                _player.Inventory[InventorySlot.Lightsource] = carried;
                _player.WeightCarried += carried.Weight;
            }
            var identifiers = _player.PlayerClass.StartingItems;
            for (var i = 0; i < 3; i++)
            {
                var identifier = identifiers[i];
                var tv = identifier.Category;
                var sv = identifier.SubCategory;
                if (tv == ItemCategory.SorceryBook)
                {
                    tv = _player.Realm1.ToSpellBookItemCategory();
                }
                else if (tv == ItemCategory.DeathBook)
                {
                    tv = _player.Realm2.ToSpellBookItemCategory();
                }
                if (tv == ItemCategory.Ring && sv == RingType.ResFear && _player.Race.ResistsFear)
                {
                    sv = RingType.SustainStr;
                }
                item = new Item();
                item.AssignItemType(Profile.Instance.ItemTypes.LookupKind(tv, sv));
                if (tv == ItemCategory.Wand)
                {
                    item.TypeSpecificValue = 1;
                }
                item.IdentifyFlags.Set(Constants.IdentStoreb);
                item.BecomeFlavourAware();
                item.BecomeKnown();
                var slot = _player.Inventory.WieldSlot(item);
                if (slot == -1)
                {
                    _player.Inventory.InvenCarry(item, false);
                }
                else
                {
                    _player.Inventory[slot] = item;
                    _player.WeightCarried += item.Weight;
                }
            }
        }
    }
}