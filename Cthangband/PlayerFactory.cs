// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Pantheon;
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
        private readonly MenuItem[] _classMenu =
        {
            new MenuItem("Channeler", CharacterClass.Channeler), new MenuItem("Chosen One", CharacterClass.ChosenOne),
            new MenuItem("Cultist", CharacterClass.Cultist), new MenuItem("Druid", CharacterClass.Druid),
            new MenuItem("Fanatic", CharacterClass.Fanatic), new MenuItem("High Mage", CharacterClass.HighMage),
            new MenuItem("Mage", CharacterClass.Mage), new MenuItem("Monk", CharacterClass.Monk),
            new MenuItem("Mindcrafter", CharacterClass.Mindcrafter), new MenuItem("Mystic", CharacterClass.Mystic),
            new MenuItem("Paladin", CharacterClass.Paladin), new MenuItem("Priest", CharacterClass.Priest),
            new MenuItem("Ranger", CharacterClass.Ranger), new MenuItem("Rogue", CharacterClass.Rogue),
            new MenuItem("Warrior", CharacterClass.Warrior), new MenuItem("Warrior Mage", CharacterClass.WarriorMage)
        };

        private readonly string[] _menuItem = new string[32];

        private readonly ItemIdentifier[][] _playerInit =
        {
            new[]
            {
                new ItemIdentifier(ItemCategory.Ring, RingType.ResFear), new ItemIdentifier(ItemCategory.Sword, SwordType.BroadSword),
                new ItemIdentifier(ItemCategory.HardArmor, HardArmourType.ChainMail)
            },
            new[]
            {
                new ItemIdentifier(ItemCategory.SorceryBook, 0), new ItemIdentifier(ItemCategory.Sword, SwordType.Dagger),
                new ItemIdentifier(ItemCategory.DeathBook, 0)
            },
            new[]
            {
                new ItemIdentifier(ItemCategory.SorceryBook, 0), new ItemIdentifier(ItemCategory.Hafted, HaftedType.Mace),
                new ItemIdentifier(ItemCategory.DeathBook, 0)
            },
            new[]
            {
                new ItemIdentifier(ItemCategory.SorceryBook, 0), new ItemIdentifier(ItemCategory.Sword, SwordType.Dagger),
                new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.SoftLeather)
            },
            new[]
            {
                new ItemIdentifier(ItemCategory.NatureBook, 0), new ItemIdentifier(ItemCategory.Sword, SwordType.BroadSword),
                new ItemIdentifier(ItemCategory.DeathBook, 0)
            },
            new[]
            {
                new ItemIdentifier(ItemCategory.SorceryBook, 0), new ItemIdentifier(ItemCategory.Sword, SwordType.BroadSword),
                new ItemIdentifier(ItemCategory.Scroll, ScrollType.ProtectionFromEvil)
            },
            new[]
            {
                new ItemIdentifier(ItemCategory.SorceryBook, 0), new ItemIdentifier(ItemCategory.Sword, SwordType.ShortSword),
                new ItemIdentifier(ItemCategory.DeathBook, 0)
            },
            new[]
            {
                new ItemIdentifier(ItemCategory.SorceryBook, 0), new ItemIdentifier(ItemCategory.Sword, SwordType.BroadSword),
                new ItemIdentifier(ItemCategory.HardArmor, HardArmourType.MetalScaleMail)
            },
            new[]
            {
                new ItemIdentifier(ItemCategory.SorceryBook, 0), new ItemIdentifier(ItemCategory.Potion, PotionType.Healing),
                new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.SoftLeather)
            },
            new[]
            {
                new ItemIdentifier(ItemCategory.Sword, SwordType.SmallSword),
                new ItemIdentifier(ItemCategory.Potion, PotionType.RestoreVis),
                new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.SoftLeather)
            },
            new[]
            {
                new ItemIdentifier(ItemCategory.SorceryBook, 0), new ItemIdentifier(ItemCategory.Sword, SwordType.Dagger),
                new ItemIdentifier(ItemCategory.Ring, RingType.SustainInt)
            },
            new[]
            {
                new ItemIdentifier(ItemCategory.SorceryBook, 0), new ItemIdentifier(ItemCategory.Hafted, HaftedType.Quarterstaff),
                new ItemIdentifier(ItemCategory.Ring, RingType.SustainWis)
            },
            new[]
            {
                new ItemIdentifier(ItemCategory.SorceryBook, 0), new ItemIdentifier(ItemCategory.Ring, RingType.SustainInt),
                new ItemIdentifier(ItemCategory.DeathBook, 0)
            },
            new[]
            {
                new ItemIdentifier(ItemCategory.Wand, WandType.MagicMissile), new ItemIdentifier(ItemCategory.Sword, SwordType.Dagger),
                new ItemIdentifier(ItemCategory.Ring, RingType.SustainCha)
            },
            new[]
            {
                new ItemIdentifier(ItemCategory.Sword, SwordType.SmallSword),
                new ItemIdentifier(ItemCategory.Potion, PotionType.Healing),
                new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.SoftLeather)
            },
            new[]
            {
                new ItemIdentifier(ItemCategory.Ring, RingType.SustainWis),
                new ItemIdentifier(ItemCategory.Potion, PotionType.Healing),
                new ItemIdentifier(ItemCategory.SoftArmor, SoftArmourType.SoftLeather)
            }
        };

        private readonly MenuItem[] _raceMenu;

        private readonly int[] _realmChoices =
        {
            // Warrior
            RealmChoice.None,
            // Mage
            RealmChoice.Life | RealmChoice.Sorcery | RealmChoice.Nature | RealmChoice.Chaos | RealmChoice.Death |
            RealmChoice.Tarot | RealmChoice.Folk | RealmChoice.Corporeal,
            // Priest
            RealmChoice.Nature | RealmChoice.Chaos | RealmChoice.Tarot | RealmChoice.Folk | RealmChoice.Corporeal,
            // Rogue
            RealmChoice.Sorcery | RealmChoice.Death | RealmChoice.Tarot | RealmChoice.Folk,
            // Ranger
            RealmChoice.Chaos | RealmChoice.Death | RealmChoice.Tarot | RealmChoice.Folk | RealmChoice.Corporeal,
            // Paladin
            RealmChoice.Life | RealmChoice.Death,
            // Warrior Mage
            RealmChoice.Life | RealmChoice.Nature | RealmChoice.Chaos | RealmChoice.Death | RealmChoice.Tarot |
            RealmChoice.Folk | RealmChoice.Sorcery | RealmChoice.Corporeal,
            // Fanatic
            RealmChoice.Chaos,
            // Monk
            RealmChoice.Corporeal | RealmChoice.Tarot | RealmChoice.Chaos,
            // Mindcrafter
            RealmChoice.None,
            // High Mage
            RealmChoice.Life | RealmChoice.Sorcery | RealmChoice.Nature | RealmChoice.Chaos | RealmChoice.Death |
            RealmChoice.Tarot | RealmChoice.Folk | RealmChoice.Corporeal,
            // Druid
            RealmChoice.Nature,
            // Cultist
            RealmChoice.Life | RealmChoice.Sorcery | RealmChoice.Nature | RealmChoice.Death | RealmChoice.Tarot |
            RealmChoice.Folk | RealmChoice.Corporeal,
            // Channeler
            RealmChoice.None,
            // Chosen One
            RealmChoice.None,
            // Mystic
            RealmChoice.None
        };

        private readonly Gender[] _sexInfo = { new Gender("Female", "Queen"), new Gender("Male", "King"), new Gender("Other", "Monarch") };

        private int _menuLength;

        private Player _player;

        private int _prevClass;

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
            for (int i = 0; i < keys.Count; i++)
            {
                _raceMenu[i] = new MenuItem(keys[i], i);
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

        private Realm ChooseRealmRandomly(int choices)
        {
            Realm[] picks = new Realm[Constants.MaxRealm];
            int n = 0;
            if ((choices & RealmChoice.Chaos) != 0 && _player.Realm1 != Realm.Chaos)
            {
                picks[n] = Realm.Chaos;
                n++;
            }
            if ((choices & RealmChoice.Corporeal) != 0 && _player.Realm1 != Realm.Corporeal)
            {
                picks[n] = Realm.Corporeal;
                n++;
            }
            if ((choices & RealmChoice.Death) != 0 && _player.Realm1 != Realm.Death)
            {
                picks[n] = Realm.Death;
                n++;
            }
            if ((choices & RealmChoice.Folk) != 0 && _player.Realm1 != Realm.Folk)
            {
                picks[n] = Realm.Folk;
                n++;
            }
            if ((choices & RealmChoice.Life) != 0 && _player.Realm1 != Realm.Life)
            {
                picks[n] = Realm.Life;
                n++;
            }
            if ((choices & RealmChoice.Nature) != 0 && _player.Realm1 != Realm.Nature)
            {
                picks[n] = Realm.Nature;
                n++;
            }
            if ((choices & RealmChoice.Tarot) != 0 && _player.Realm1 != Realm.Tarot)
            {
                picks[n] = Realm.Tarot;
                n++;
            }
            if ((choices & RealmChoice.Sorcery) != 0 && _player.Realm1 != Realm.Sorcery)
            {
                picks[n] = Realm.Sorcery;
                n++;
            }
            int k = Program.Rng.RandomLessThan(n);
            return picks[k];
        }

        private void DisplayAPlusB(int x, int y, int initial, int bonus)
        {
            string buf = $"{initial:00}% + {bonus / 10}.{bonus % 10}%/lv";
            Gui.Print(Colour.Black, buf, y, x);
        }

        private void DisplayClassInfo(int pclass)
        {
            Gui.Print(Colour.Purple, "STR:", 36, 21);
            Gui.Print(Colour.Purple, "INT:", 37, 21);
            Gui.Print(Colour.Purple, "WIS:", 38, 21);
            Gui.Print(Colour.Purple, "DEX:", 39, 21);
            Gui.Print(Colour.Purple, "CON:", 40, 21);
            Gui.Print(Colour.Purple, "CHA:", 41, 21);
            for (int i = 0; i < 6; i++)
            {
                int bonus = Profession.ClassInfo[pclass].AbilityBonus[i];
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
            DisplayAPlusB(67, 36, Profession.ClassInfo[pclass].BaseDisarmBonus, Profession.ClassInfo[pclass].DisarmBonusPerLevel);
            DisplayAPlusB(67, 37, Profession.ClassInfo[pclass].BaseDeviceBonus, Profession.ClassInfo[pclass].DeviceBonusPerLevel);
            DisplayAPlusB(67, 38, Profession.ClassInfo[pclass].BaseSaveBonus, Profession.ClassInfo[pclass].SaveBonusPerLevel);
            DisplayAPlusB(67, 39, Profession.ClassInfo[pclass].BaseStealthBonus * 4, Profession.ClassInfo[pclass].StealthBonusPerLevel * 4);
            DisplayAPlusB(67, 40, Profession.ClassInfo[pclass].BaseMeleeAttackBonus, Profession.ClassInfo[pclass].MeleeAttackBonusPerLevel);
            DisplayAPlusB(67, 41, Profession.ClassInfo[pclass].BaseRangedAttackBonus, Profession.ClassInfo[pclass].RangedAttackBonusPerLevel);
            string buf = "+" + Profession.ClassInfo[pclass].ExperienceFactor + "%";
            Gui.Print(Colour.Black, buf, 36, 45);
            buf = "1d" + Profession.ClassInfo[pclass].HitDieBonus;
            Gui.Print(Colour.Black, buf, 37, 45);
            Gui.Print(Colour.Black, "-", 38, 45);
            buf = $"{Profession.ClassInfo[pclass].BaseSearchBonus:00}%";
            Gui.Print(Colour.Black, buf, 39, 45);
            buf = $"{Profession.ClassInfo[pclass].BaseSearchFrequency:00}%";
            Gui.Print(Colour.Black, buf, 40, 45);
            switch (pclass)
            {
                case CharacterClass.Cultist:
                    Gui.Print(Colour.Purple, "INT based spell casters, who use Chaos and another realm", 30, 20);
                    Gui.Print(Colour.Purple, "of their choice. Can't wield weapons except for powerful", 31, 20);
                    Gui.Print(Colour.Purple, "chaos blades. Learn to resist chaos (at lvl 20). Have a", 32, 20);
                    Gui.Print(Colour.Purple, "cult patron who will randomly give them rewards or", 33, 20);
                    Gui.Print(Colour.Purple, "punishments as they increase in level.", 34, 20);
                    break;

                case CharacterClass.Fanatic:
                    Gui.Print(Colour.Purple, "Warriors who dabble in INT based Chaos magic. Have a cult", 30, 20);
                    Gui.Print(Colour.Purple, "patron who will randomly give them rewards or punishments", 31, 20);
                    Gui.Print(Colour.Purple, "as they increase in level. Learn to resist chaos", 32, 20);
                    Gui.Print(Colour.Purple, "(at lvl 30) and fear (at lvl 40).", 33, 20);
                    break;

                case CharacterClass.ChosenOne:
                    Gui.Print(Colour.Purple, "Warriors of fate, who have no spell casting abilities but", 30, 20);
                    Gui.Print(Colour.Purple, "gain a large number of passive magical abilities (too long", 31, 20);
                    Gui.Print(Colour.Purple, "to list here) as they increase in level.", 32, 20);
                    break;

                case CharacterClass.Channeler:
                    Gui.Print(Colour.Purple, "Similar to a spell caster, but rather than casting spells", 30, 20);
                    Gui.Print(Colour.Purple, "from a book, they can use their CHA to channel vis into", 31, 20);
                    Gui.Print(Colour.Purple, "most types of item, powering the effects of the items", 32, 20);
                    Gui.Print(Colour.Purple, "without depleting them.", 33, 20);
                    break;

                case CharacterClass.Druid:
                    Gui.Print(Colour.Purple, "Nature priests who use WIS based spell casting and who are", 30, 20);
                    Gui.Print(Colour.Purple, "limited to the Nature realm. As priests, they can't use", 31, 20);
                    Gui.Print(Colour.Purple, "edged weapons unless those weapons are holy; but they can", 32, 20);
                    Gui.Print(Colour.Purple, "wear heavy armour without it disrupting their casting.", 33, 20);
                    break;

                case CharacterClass.HighMage:
                    Gui.Print(Colour.Purple, "INT based spell casters who specialise in a single realm", 30, 20);
                    Gui.Print(Colour.Purple, "of magic. They may choose any realm, and are better at", 31, 20);
                    Gui.Print(Colour.Purple, "casting spells from that realm than a normal mage. High", 32, 20);
                    Gui.Print(Colour.Purple, "mages also get more vis than other spell casters do.", 33, 20);
                    Gui.Print(Colour.Purple, "Wearing too much armour disrupts their casting.", 34, 20);
                    break;

                case CharacterClass.Mage:
                    Gui.Print(Colour.Purple, "Flexible INT based spell casters who can cast magic from", 30, 20);
                    Gui.Print(Colour.Purple, "any two realms of their choice. However, they can't wear", 31, 20);
                    Gui.Print(Colour.Purple, "much armour before it starts disrupting their casting.", 32, 20);
                    break;

                case CharacterClass.Monk:
                    Gui.Print(Colour.Purple, "Masters of unarmed combat. While wearing only light armour", 30, 20);
                    Gui.Print(Colour.Purple, "they can move faster and dodge blows and can learn to", 31, 20);
                    Gui.Print(Colour.Purple, "resist paralysis (at lvl 25). While not wielding a weapon", 32, 20);
                    Gui.Print(Colour.Purple, "they have extra attacks and do increased damage. They are", 33, 20);
                    Gui.Print(Colour.Purple, "WIS based casters using Chaos, Tarot or Corporeal magic.", 34, 20);
                    break;

                case CharacterClass.Mindcrafter:
                    Gui.Print(Colour.Purple, "Disciples of the psionic arts, Mindcrafters learn a range", 30, 20);
                    Gui.Print(Colour.Purple, "of mental abilities; which they power using WIS. As well", 31, 20);
                    Gui.Print(Colour.Purple, "as their powers, they learn to resist fear (at lvl 10),", 32, 20);
                    Gui.Print(Colour.Purple, "prevent wis drain (at lvl 20), resist confusion", 33, 20);
                    Gui.Print(Colour.Purple, "(at lvl 30), and gain telepathy (at lvl 40).", 34, 20);
                    break;

                case CharacterClass.Mystic:
                    Gui.Print(Colour.Purple, "Mystics master both martial and psionic arts, which they", 30, 20);
                    Gui.Print(Colour.Purple, "power using WIS. Can resist confusion (at lvl 10), fear", 31, 20);
                    Gui.Print(Colour.Purple, "(lvl 25), paralysis (lvl 30). Telepathy (lvl 40). While", 32, 20);
                    Gui.Print(Colour.Purple, "wearing only light armour they can move faster and dodge,", 33, 20);
                    Gui.Print(Colour.Purple, "and while not wielding a weapon they do increased damage.", 34, 20);
                    break;

                case CharacterClass.Paladin:
                    Gui.Print(Colour.Purple, "Holy warriors who use WIS based spell casting to supplement", 30, 20);
                    Gui.Print(Colour.Purple, "their fighting skills. Paladins can specialise in either", 31, 20);
                    Gui.Print(Colour.Purple, "Life or Death magic, but their spell casting is weak in", 32, 20);
                    Gui.Print(Colour.Purple, "comparison to a full priest. Paladins learn to resist fear", 33, 20);
                    Gui.Print(Colour.Purple, "(at lvl 40).", 34, 20);
                    break;

                case CharacterClass.Priest:
                    Gui.Print(Colour.Purple, "Devout followers of the Great Ones, Priests use WIS based", 30, 20);
                    Gui.Print(Colour.Purple, "spell casting. They may choose either Life or Death magic,", 31, 20);
                    Gui.Print(Colour.Purple, "and another realm of their choice. Priests can't use edged", 32, 20);
                    Gui.Print(Colour.Purple, "weapons unless they are blessed, but can use any armour.", 33, 20);
                    break;

                case CharacterClass.Ranger:
                    Gui.Print(Colour.Purple, "Masters of ranged combat, especiallly using bows. Rangers", 30, 20);
                    Gui.Print(Colour.Purple, "supplement their shooting and stealth with INT based spell", 31, 20);
                    Gui.Print(Colour.Purple, "casting from the Nature realm plus another realm of their", 32, 20);
                    Gui.Print(Colour.Purple, "choice from Death, Corporeal, Tarot, Chaos, and Folk.", 33, 20);
                    break;

                case CharacterClass.Rogue:
                    Gui.Print(Colour.Purple, "Stealth based characters who are adept at picking locks,", 30, 20);
                    Gui.Print(Colour.Purple, "searching, and disarming traps. Rogues can use stealth to", 31, 20);
                    Gui.Print(Colour.Purple, "their advantage in order to backstab sleeping or fleeing", 32, 20);
                    Gui.Print(Colour.Purple, "foes. They also dabble in INT based magic, learning spells", 33, 20);
                    Gui.Print(Colour.Purple, "from the Tarot, Sorcery, Death, or Folk realms.", 34, 20);
                    break;

                case CharacterClass.Warrior:
                    Gui.Print(Colour.Purple, "Straightforward, no-nonsense fighters. They are the best", 30, 20);
                    Gui.Print(Colour.Purple, "characters at melee combat, and require the least amount", 31, 20);
                    Gui.Print(Colour.Purple, "of experience to increase in level. They can learn to", 32, 20);
                    Gui.Print(Colour.Purple, "resist fear (at lvl 30). The ideal class for novices.", 33, 20);
                    break;

                case CharacterClass.WarriorMage:
                    Gui.Print(Colour.Purple, "A blend of both warrior and mage, getting the abilities of", 30, 20);
                    Gui.Print(Colour.Purple, "both but not being the best at either. They use INT based", 31, 20);
                    Gui.Print(Colour.Purple, "spell casting, getting access to the Folk realm plus a", 32, 20);
                    Gui.Print(Colour.Purple, "second realm of their choice. They pay for their extreme", 33, 20);
                    Gui.Print(Colour.Purple, "flexibility by increasing in level only slowly.", 34, 20);
                    break;
            }
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
                _player.Profession = Profession.ClassInfo[_prevClass];
                str = _player.Profession.Title;
            }
            else if (stage < 2)
            {
                str = spaces;
            }
            else
            {
                _player.Profession = Profession.ClassInfo[_player.ProfessionIndex];
                str = _player.Profession.Title;
            }
            Gui.Print(Colour.Brown, str, 5, 15);
            string buf = string.Empty;
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
                    buf = _player.Profession.AbilityBonus[i].ToString("+0;-0;+0").PadLeft(3);
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
            Gui.Print(Colour.Purple, "STR:", 36, 21);
            Gui.Print(Colour.Purple, "INT:", 37, 21);
            Gui.Print(Colour.Purple, "WIS:", 38, 21);
            Gui.Print(Colour.Purple, "DEX:", 39, 21);
            Gui.Print(Colour.Purple, "CON:", 40, 21);
            Gui.Print(Colour.Purple, "CHA:", 41, 21);
            for (int i = 0; i < 6; i++)
            {
                int bonus = PlayerRaces.Instance[race].AbilityBonus[i] + Profession.ClassInfo[_player.ProfessionIndex].AbilityBonus[i];
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
            DisplayAPlusB(67, 36, Profession.ClassInfo[_player.ProfessionIndex].BaseDisarmBonus + PlayerRaces.Instance[race].BaseDisarmBonus,
                Profession.ClassInfo[_player.ProfessionIndex].DisarmBonusPerLevel);
            DisplayAPlusB(67, 37, Profession.ClassInfo[_player.ProfessionIndex].BaseDeviceBonus + PlayerRaces.Instance[race].BaseDeviceBonus,
                Profession.ClassInfo[_player.ProfessionIndex].DeviceBonusPerLevel);
            DisplayAPlusB(67, 38, Profession.ClassInfo[_player.ProfessionIndex].BaseSaveBonus + PlayerRaces.Instance[race].BaseSaveBonus,
                Profession.ClassInfo[_player.ProfessionIndex].SaveBonusPerLevel);
            DisplayAPlusB(67, 39, (Profession.ClassInfo[_player.ProfessionIndex].BaseStealthBonus * 4) + (PlayerRaces.Instance[race].BaseStealthBonus * 4),
                Profession.ClassInfo[_player.ProfessionIndex].StealthBonusPerLevel * 4);
            DisplayAPlusB(67, 40, Profession.ClassInfo[_player.ProfessionIndex].BaseMeleeAttackBonus + PlayerRaces.Instance[race].BaseMeleeAttackBonus,
                Profession.ClassInfo[_player.ProfessionIndex].MeleeAttackBonusPerLevel);
            DisplayAPlusB(67, 41, Profession.ClassInfo[_player.ProfessionIndex].BaseRangedAttackBonus + PlayerRaces.Instance[race].BaseRangedAttackBonus,
                Profession.ClassInfo[_player.ProfessionIndex].RangedAttackBonusPerLevel);
            string buf = PlayerRaces.Instance[race].ExperienceFactor + Profession.ClassInfo[_player.ProfessionIndex].ExperienceFactor + "%";
            Gui.Print(Colour.Black, buf, 36, 45);
            buf = "1d" + (PlayerRaces.Instance[race].HitDieBonus + Profession.ClassInfo[_player.ProfessionIndex].HitDieBonus);
            Gui.Print(Colour.Black, buf, 37, 45);
            if (PlayerRaces.Instance[race].Infravision == 0)
            {
                Gui.Print(Colour.Black, "nil", 38, 45);
            }
            else
            {
                buf = PlayerRaces.Instance[race].Infravision + "0 feet";
                Gui.Print(Colour.Green, buf, 38, 45);
            }
            buf = $"{PlayerRaces.Instance[race].BaseSearchBonus + Profession.ClassInfo[_player.ProfessionIndex].BaseSearchBonus:00}%";
            Gui.Print(Colour.Black, buf, 39, 45);
            buf = $"{PlayerRaces.Instance[race].BaseSearchFrequency + Profession.ClassInfo[_player.ProfessionIndex].BaseSearchFrequency:00}%";
            Gui.Print(Colour.Black, buf, 40, 45);
            Gui.Print(Colour.Purple, PlayerRaces.Instance[race].Description1, 29, 20);
            Gui.Print(Colour.Purple, PlayerRaces.Instance[race].Description2, 30, 20);
            Gui.Print(Colour.Purple, PlayerRaces.Instance[race].Description3, 31, 20);
            Gui.Print(Colour.Purple, PlayerRaces.Instance[race].Description4, 32, 20);
            Gui.Print(Colour.Purple, PlayerRaces.Instance[race].Description5, 33, 20);
            Gui.Print(Colour.Purple, PlayerRaces.Instance[race].Description6, 34, 20);
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
            _player.ExperienceMultiplier = _player.Race.ExperienceFactor + _player.Profession.ExperienceFactor;
            _player.HitDie = _player.Race.HitDieBonus + _player.Profession.HitDieBonus;
            _player.MaxHealth = _player.HitDie;
            _player.PlayerHp[0] = _player.HitDie;
            int lastroll = _player.HitDie;
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
                int j = Program.Rng.DieRoll(Constants.PyMaxLevel - 1);
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
            int gold = (_player.SocialClass * 6) + Program.Rng.DieRoll(100) + 300;
            for (int i = 0; i < 6; i++)
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

        private void GetRealmsRandomly()
        {
            int pclas = _player.ProfessionIndex;
            _player.Realm1 = Realm.None;
            _player.Realm2 = Realm.None;
            if (_realmChoices[pclas] == RealmChoice.None)
            {
                return;
            }
            switch (pclas)
            {
                case CharacterClass.WarriorMage:
                    _player.Realm1 = Realm.Folk;
                    break;

                case CharacterClass.Fanatic:
                    _player.Realm1 = Realm.Chaos;
                    break;

                case CharacterClass.Priest:
                    _player.Realm1 = ChooseRealmRandomly(RealmChoice.Life | RealmChoice.Death);
                    break;

                case CharacterClass.Ranger:
                    _player.Realm1 = Realm.Nature;
                    break;

                case CharacterClass.Druid:
                    _player.Realm1 = Realm.Nature;
                    break;

                case CharacterClass.Cultist:
                    _player.Realm1 = Realm.Chaos;
                    break;

                default:
                    _player.Realm1 = ChooseRealmRandomly(_realmChoices[pclas]);
                    break;
            }
            if (pclas == CharacterClass.Paladin || pclas == CharacterClass.Rogue || pclas == CharacterClass.Fanatic ||
                pclas == CharacterClass.Monk || pclas == CharacterClass.HighMage ||
                pclas == CharacterClass.Druid)
            {
                return;
            }
            _player.Realm2 = ChooseRealmRandomly(_realmChoices[pclas]);
            if (_player.ProfessionIndex == CharacterClass.Priest)
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
                List<int> dice = new List<int>() { 17, 16, 14, 12, 11, 10 };
                for (i = 0; i < 6; i++)
                {
                    int index = Program.Rng.DieRoll(dice.Count) - 1;
                    j = dice[index];
                    dice.RemoveAt(index);
                    _player.AbilityScores[i].InnateMax = j;
                    int bonus = _player.Race.AbilityBonus[i] + _player.Profession.AbilityBonus[i];
                    _player.AbilityScores[i].Innate = _player.AbilityScores[i].InnateMax;
                    _player.AbilityScores[i].Adjusted = _player.AbilityScores[i]
                        .ModifyStatValue(_player.AbilityScores[i].InnateMax, bonus);
                }
                if (_player.AbilityScores[Profession.PrimeStat(_player.ProfessionIndex)].InnateMax > 13)
                {
                    break;
                }
            }
        }

        private void MenuDisplay(int current)
        {
            Gui.Clear(30);
            Gui.Print(Colour.Orange, "=>", 35, 0);
            for (int i = 0; i < _menuLength; i++)
            {
                int row = 35 + i - current;
                if (row >= 30 && row <= 40)
                {
                    Colour a = Colour.Purple;
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
                _prevClass = CharacterClass.Warrior;
                _prevRealm1 = Realm.None;
                _prevRealm2 = Realm.None;
                _prevName = "Xena";
                _prevGeneration = 0;
            }
            else
            {
                _prevSex = ex.GenderIndex;
                _prevRace = ex.BirthRace;
                _prevClass = ex.ProfessionIndex;
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
            int stage = 0;
            int[] menu = new int[9];
            bool[] autoChose = new bool[8];
            Realm[] realmChoice = new Realm[8];
            for (i = 0; i < 8; i++)
            {
                menu[i] = 0;
            }
            menu[BirthStage.ClassSelection] = 14;
            menu[BirthStage.RaceSelection] = 16;
            Gui.Clear();
            int viewMode = 1;
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
                            _player.ProfessionIndex = _prevClass;
                            _player.Profession = Profession.ClassInfo[_player.ProfessionIndex];
                            stage++;
                            break;
                        }
                        if (menu[0] == Constants.GenerateRandom)
                        {
                            autoChose[stage] = true;
                            _player.ProfessionIndex = Program.Rng.RandomLessThan(Constants.MaxClass);
                            _player.Profession = Profession.ClassInfo[_player.ProfessionIndex];
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
                        DisplayClassInfo(_classMenu[menu[stage]].Index);
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
                            _player.ProfessionIndex = _classMenu[menu[BirthStage.ClassSelection]].Index;
                            _player.Profession = Profession.ClassInfo[_player.ProfessionIndex];
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
                                string k = PlayerRaces.Instance.RandomRaceName();
                                _player.CurrentRace = k;
                                _player.Race = PlayerRaces.Instance[_player.CurrentRace];
                                _player.GetFirstLevelMutation = _player.Race.Mutates;
                            }
                            while ((_player.Race.Choice & (1L << _player.ProfessionIndex)) == 0);
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
                        switch (_player.ProfessionIndex)
                        {
                            case CharacterClass.Cultist:
                            case CharacterClass.Fanatic:
                                autoChose[stage] = true;
                                _player.Realm1 = Realm.Chaos;
                                stage++;
                                break;

                            case CharacterClass.WarriorMage:
                                autoChose[stage] = true;
                                _player.Realm1 = Realm.Folk;
                                stage++;
                                break;

                            case CharacterClass.Druid:
                            case CharacterClass.Ranger:
                                autoChose[stage] = true;
                                _player.Realm1 = Realm.Nature;
                                stage++;
                                break;

                            case CharacterClass.Paladin:
                            case CharacterClass.Priest:
                                realmChoice[0] = Realm.Life;
                                realmChoice[1] = Realm.Death;
                                _menuLength = 2;
                                break;

                            case CharacterClass.Rogue:
                                realmChoice[0] = Realm.Death;
                                realmChoice[1] = Realm.Sorcery;
                                realmChoice[2] = Realm.Tarot;
                                realmChoice[3] = Realm.Folk;
                                _menuLength = 4;
                                break;

                            case CharacterClass.HighMage:
                            case CharacterClass.Mage:
                                realmChoice[0] = Realm.Life;
                                realmChoice[1] = Realm.Death;
                                realmChoice[2] = Realm.Nature;
                                realmChoice[3] = Realm.Sorcery;
                                realmChoice[4] = Realm.Corporeal;
                                realmChoice[5] = Realm.Tarot;
                                realmChoice[6] = Realm.Chaos;
                                realmChoice[7] = Realm.Folk;
                                _menuLength = 8;
                                break;

                            case CharacterClass.Monk:
                                realmChoice[0] = Realm.Corporeal;
                                realmChoice[1] = Realm.Tarot;
                                realmChoice[2] = Realm.Chaos;
                                _menuLength = 3;
                                break;

                            case CharacterClass.ChosenOne:
                            case CharacterClass.Channeler:
                            case CharacterClass.Mindcrafter:
                            case CharacterClass.Mystic:
                            case CharacterClass.Warrior:
                                autoChose[stage] = true;
                                _player.Realm1 = Realm.None;
                                stage++;
                                break;
                        }
                        if (stage > BirthStage.RealmSelection1)
                        {
                            break;
                        }
                        autoChose[stage] = false;
                        for (i = 0; i < _menuLength; i++)
                        {
                            _menuItem[i] = Spellcasting.RealmName(realmChoice[i]);
                        }
                        DisplayPartialCharacter(stage);
                        if (menu[stage] >= _menuLength)
                        {
                            menu[stage] = 0;
                        }
                        MenuDisplay(menu[stage]);
                        DisplayRealmInfo(realmChoice[menu[stage]]);
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
                            _player.Realm1 = realmChoice[menu[BirthStage.RealmSelection1]];
                        }
                        break;

                    case BirthStage.RealmSelection2:
                        if (menu[0] == Constants.GenerateReplay)
                        {
                            autoChose[stage] = true;
                            _player.Realm2 = _prevRealm2;
                            if (_player.ProfessionIndex == CharacterClass.Priest)
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
                        switch (_player.ProfessionIndex)
                        {
                            case CharacterClass.ChosenOne:
                            case CharacterClass.Channeler:
                            case CharacterClass.Mindcrafter:
                            case CharacterClass.Warrior:
                            case CharacterClass.Fanatic:
                            case CharacterClass.HighMage:
                            case CharacterClass.Paladin:
                            case CharacterClass.Rogue:
                            case CharacterClass.Monk:
                            case CharacterClass.Mystic:
                            case CharacterClass.Druid:
                                autoChose[stage] = true;
                                _player.Realm2 = Realm.None;
                                stage++;
                                break;

                            case CharacterClass.Cultist:
                            case CharacterClass.WarriorMage:
                            case CharacterClass.Ranger:
                            case CharacterClass.Priest:
                            case CharacterClass.Mage:
                                _menuLength = 0;
                                int realmFilter = _realmChoices[_player.ProfessionIndex];
                                if ((realmFilter & RealmChoice.Life) != 0 && _player.Realm1 != Realm.Life)
                                {
                                    realmChoice[_menuLength] = Realm.Life;
                                    _menuLength++;
                                }
                                if ((realmFilter & RealmChoice.Death) != 0 && _player.Realm1 != Realm.Death)
                                {
                                    realmChoice[_menuLength] = Realm.Death;
                                    _menuLength++;
                                }
                                if ((realmFilter & RealmChoice.Nature) != 0 && _player.Realm1 != Realm.Nature)
                                {
                                    realmChoice[_menuLength] = Realm.Nature;
                                    _menuLength++;
                                }
                                if ((realmFilter & RealmChoice.Sorcery) != 0 && _player.Realm1 != Realm.Sorcery)
                                {
                                    realmChoice[_menuLength] = Realm.Sorcery;
                                    _menuLength++;
                                }
                                if ((realmFilter & RealmChoice.Corporeal) != 0 && _player.Realm1 != Realm.Corporeal)
                                {
                                    realmChoice[_menuLength] = Realm.Corporeal;
                                    _menuLength++;
                                }
                                if ((realmFilter & RealmChoice.Tarot) != 0 && _player.Realm1 != Realm.Tarot)
                                {
                                    realmChoice[_menuLength] = Realm.Tarot;
                                    _menuLength++;
                                }
                                if ((realmFilter & RealmChoice.Chaos) != 0 && _player.Realm1 != Realm.Chaos)
                                {
                                    realmChoice[_menuLength] = Realm.Chaos;
                                    _menuLength++;
                                }
                                if ((realmFilter & RealmChoice.Folk) != 0 && _player.Realm1 != Realm.Folk)
                                {
                                    realmChoice[_menuLength] = Realm.Folk;
                                    _menuLength++;
                                }
                                break;
                        }
                        if (stage > BirthStage.RealmSelection2)
                        {
                            break;
                        }
                        autoChose[stage] = false;
                        for (i = 0; i < _menuLength; i++)
                        {
                            _menuItem[i] = Spellcasting.RealmName(realmChoice[i]);
                        }
                        DisplayPartialCharacter(stage);
                        if (menu[stage] >= _menuLength)
                        {
                            menu[stage] = 0;
                        }
                        MenuDisplay(menu[stage]);
                        DisplayRealmInfo(realmChoice[menu[stage]]);
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
                            _player.Realm2 = realmChoice[menu[BirthStage.RealmSelection2]];
                            if (_player.ProfessionIndex == CharacterClass.Priest)
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
                        _player.GooPatron =
                            SaveGame.Instance.PatronList[Program.Rng.DieRoll(SaveGame.Instance.PatronList.Length) - 1];
                        _player.UpdatesNeeded.Set(UpdateFlags.UpdateBonuses | UpdateFlags.UpdateHealth);
                        SaveGame.Instance.Player = _player;
                        SaveGame.Instance.UpdateStuff();
                        SaveGame.Instance.Player = null;
                        _player.Health = _player.MaxHealth;
                        _player.Vis = _player.MaxVis;
                        _player.Energy = 150;
                        CharacterViewer characterViewer = new CharacterViewer(_player);
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
            Item item = new Item();
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
            if (_player.Race.Glows || _player.ProfessionIndex == CharacterClass.ChosenOne)
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
                Item carried = new Item(item) { Count = 1 };
                _player.Inventory[InventorySlot.Lightsource] = carried;
                _player.WeightCarried += carried.Weight;
            }
            for (int i = 0; i < 3; i++)
            {
                ItemCategory tv = _playerInit[_player.ProfessionIndex][i].Category;
                int sv = _playerInit[_player.ProfessionIndex][i].SubCategory;
                if (tv == ItemCategory.SorceryBook)
                {
                    tv = _player.Realm1.ToSpellBookItemCategory();
                }
                else if (tv == ItemCategory.DeathBook)
                {
                    tv = _player.Realm2.ToSpellBookItemCategory();
                }
                item = new Item();
                item.AssignItemType(Profile.Instance.ItemTypes.LookupKind(tv, sv));
                if (tv == ItemCategory.Sword && _player.ProfessionIndex == CharacterClass.Rogue && _player.Realm1 == Realm.Death)
                {
                    item.RareItemTypeIndex = Enumerations.RareItemType.WeaponOfPoisoning;
                }
                if (tv == ItemCategory.Wand)
                {
                    item.TypeSpecificValue = 1;
                }
                if (tv == ItemCategory.Ring && sv == RingType.ResFear && _player.Race.ResistsFear)
                {
                    sv = RingType.SustainStr;
                }
                item.IdentifyFlags.Set(Constants.IdentStoreb);
                item.BecomeFlavourAware();
                item.BecomeKnown();
                int slot = _player.Inventory.WieldSlot(item);
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