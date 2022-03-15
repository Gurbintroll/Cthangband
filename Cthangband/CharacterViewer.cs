// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Spells;
using Cthangband.StaticData;
using Cthangband.UI;

namespace Cthangband
{
    /// <summary>
    /// A viewer to display a character sheet for the character
    /// </summary>
    internal class CharacterViewer
    {
        private readonly Player _player;

        public CharacterViewer(Player player)
        {
            _player = player;
        }

        /// <summary>
        /// Display the 'Equippy' characters (the visual representation of a characters' equipment)
        /// </summary>
        /// <param name="player"> The player whose equippy characters should be displayed </param>
        /// <param name="screenRow"> The row on which to print the characters </param>
        /// <param name="screenCol"> The column in which to start printing the characters </param>
        public static void DisplayPlayerEquippy(Player player, int screenRow, int screenCol)
        {
            for (int i = InventorySlot.MeleeWeapon; i < InventorySlot.Total; i++)
            {
                Item item = player.Inventory[i];
                Colour colour = Colour.Background;
                char character = ' ';
                // Only print items that exist
                if (item.ItemType != null)
                {
                    colour = item.ItemType.Colour;
                    character = item.ItemType.Character;
                }
                Gui.Print(colour, character, screenRow, screenCol + i - InventorySlot.MeleeWeapon);
            }
        }

        /// <summary>
        /// Display the 'Equippy' characters (the visual representation of a character's equipment)
        /// in the default location on the main game screen
        /// </summary>
        /// <param name="player"> The player whose equippy characters should be displayed </param>
        public static void PrintEquippy(Player player)
        {
            DisplayPlayerEquippy(player, ScreenLocation.RowEquippy, ScreenLocation.ColEquippy);
        }

        /// <summary>
        /// Display the player's entire character sheet
        /// </summary>
        public void DisplayPlayer()
        {
            Gui.Clear(0);
            DisplayPlayerTop();
            DisplayPlayerHistory();
            DisplayPlayerAbilityScoresWithEffects();
            DisplayPlayerAbilityScoresWithModifiers();
            DisplayPlayerEssentials();
            DisplayPlayerSkills();
        }

        /// <summary>
        /// Create a summary of the bonuses given by a specific ability score
        /// </summary>
        /// <param name="abilityIndex"> The index of the ability score to summarise </param>
        /// <returns> The summary of the score's bonuses </returns>
        private string AbilitySummary(int abilityIndex)
        {
            // The summary will have up to five sections
            string bonus1 = string.Empty;
            string bonus2 = string.Empty;
            string bonus3 = string.Empty;
            string bonus4 = string.Empty;
            string bonus5 = string.Empty;
            // Get the score
            AbilityScore ability = _player.AbilityScores[abilityIndex];
            // Fill in up to five pieces of bonus text
            switch (abilityIndex)
            {
                case Ability.Strength:
                    int toHit = ability.StrAttackBonus;
                    bonus1 = $"{toHit:+0;-0;+0} to hit";
                    int toDam = ability.StrDamageBonus;
                    bonus2 = $", {toDam:+0;-0;+0} damage";
                    int carry = ability.StrCarryingCapacity * 5;
                    bonus3 = $", carry {carry}lb";
                    int weap = ability.StrMaxWeaponWeight;
                    bonus4 = $", wield {weap}lb";
                    int dig = ability.StrDiggingBonus;
                    bonus5 = $", {dig}% digging";
                    break;

                case Ability.Intelligence:
                    int device = ability.IntUseDeviceBonus;
                    bonus1 = $"{device:+0;-0;+0} device";
                    int disarm = ability.IntDisarmBonus;
                    bonus2 = $", {disarm:+0;-0;+0}% disarm";
                    break;

                case Ability.Wisdom:
                    int save = ability.WisSavingThrowBonus;
                    bonus1 = $"{save:+0;-0;+0} save";
                    break;

                case Ability.Dexterity:
                    toHit = ability.DexAttackBonus;
                    bonus1 = $"{toHit:+0;-0;+0} to hit";
                    disarm = ability.DexDisarmBonus;
                    bonus2 = $", {disarm:+0;-0;+0}% disarm";
                    int ac = ability.DexArmourClassBonus;
                    bonus3 = $", {ac:+0;-0;+0} AC";
                    int theft = ability.DexTheftAvoidance;
                    bonus4 = $", {theft}% anti-theft";
                    break;

                case Ability.Constitution:
                    int hits = ability.ConHealthBonus;
                    if (hits == -1)
                    {
                        bonus1 = "-0.5 HP/lvl";
                    }
                    else
                    {
                        bonus1 = hits % 2 == 0 ? $"{hits / 2:+0;-0;+0} HP/lvl" : $"{hits / 2:+0;-0;+0}.5 HP/lvl";
                    }
                    int regen = ability.ConRecoverySpeed;
                    bonus2 = $", x{regen + 1} recovery";
                    break;

                case Ability.Charisma:
                    int haggle = ability.ChaPriceAdjustment;
                    bonus1 = $"{haggle}% prices";
                    break;
            }
            // Add the bonus text for spell casting abilities
            if (_player.Spellcasting.SpellStat == abilityIndex && abilityIndex != Ability.Strength)
            {
                int mana = ability.ManaBonus;
                // Casting abilities only have one or two inherent bonuses, so it's safe to start at three
                bonus3 = mana % 2 == 0 ? $", {mana / 2} SP/lvl" : $", {mana / 2}.5 SP/lvl";
                // Not all casting classes have actual spells
                if (_player.ProfessionIndex != CharacterClass.Mindcrafter && _player.ProfessionIndex != CharacterClass.Mystic
                    && _player.ProfessionIndex != CharacterClass.Channeler)
                {
                    int spells = ability.HalfSpellsPerLevel;
                    if (spells == 2)
                    {
                        bonus4 = ", 1 spell/lvl";
                    }
                    else if (spells % 2 == 0)
                    {
                        bonus4 = $", {spells / 2} spells/lvl";
                    }
                    else
                    {
                        bonus4 = $", {spells / 2}.5 spells/lvl";
                    }
                }
                // Almost all casting classes have a failure chance
                if (_player.ProfessionIndex != CharacterClass.Channeler)
                {
                    int fail = ability.SpellMinFailChance;
                    bonus5 = $", {fail}% min fail";
                }
            }
            // String together the bonuses and return the string
            return $"{bonus1}{bonus2}{bonus3}{bonus4}{bonus5}";
        }

        /// <summary>
        /// Display the player's ability scores including any bonuses or penalties these scores give them
        /// </summary>
        private void DisplayPlayerAbilityScoresWithEffects()
        {
            // Loop through the scores
            for (int i = 0; i < 6; i++)
            {
                string buf;
                // If they've been drained, make them visually distinct
                if (_player.AbilityScores[i].Innate < _player.AbilityScores[i].InnateMax)
                {
                    Gui.Print(Colour.Blue, GlobalData.StatNamesReduced[i], 14 + i, 1);
                    int value = _player.AbilityScores[i].Adjusted;
                    buf = value.StatToString();
                    Gui.Print(Colour.Grey, buf, 14 + i, 6);
                    buf = AbilitySummary(i);
                    Gui.Print(Colour.Grey, buf, i + 14, 13);
                }
                else
                {
                    Gui.Print(Colour.Blue, GlobalData.StatNames[i], 14 + i, 1);
                    buf = _player.AbilityScores[i].Adjusted.StatToString();
                    Gui.Print(Colour.Green, buf, 14 + i, 6);
                    buf = AbilitySummary(i);
                    Gui.Print(Colour.Green, buf, i + 14, 13);
                }
            }
        }

        /// <summary>
        /// Display the ability scores including details of any modifiers to them
        /// </summary>
        private void DisplayPlayerAbilityScoresWithModifiers()
        {
            int i;
            int stat;
            FlagSet f1 = new FlagSet();
            FlagSet f2 = new FlagSet();
            FlagSet f3 = new FlagSet();
            Colour a;
            char c;
            const int statCol = 1;
            const int row = 22;
            Gui.Print(Colour.Purple, "Initial", row - 1, statCol + 5);
            Gui.Print(Colour.Brown, "Race Class Mods", row - 1, statCol + 13);
            Gui.Print(Colour.Green, "Actual", row - 1, statCol + 29);
            Gui.Print(Colour.Red, "Reduced", row - 1, statCol + 36);
            // Loop through the scores
            for (i = 0; i < 6; i++)
            {
                // Reverse engineer our equipment bonuses from our score
                int equipmentBonuses = 0;
                if (_player.AbilityScores[i].InnateMax > 18 && _player.AbilityScores[i].AdjustedMax > 18)
                {
                    equipmentBonuses = (_player.AbilityScores[i].AdjustedMax - _player.AbilityScores[i].InnateMax) / 10;
                }
                if (_player.AbilityScores[i].InnateMax <= 18 && _player.AbilityScores[i].AdjustedMax <= 18)
                {
                    equipmentBonuses = _player.AbilityScores[i].AdjustedMax - _player.AbilityScores[i].InnateMax;
                }
                if (_player.AbilityScores[i].InnateMax <= 18 && _player.AbilityScores[i].AdjustedMax > 18)
                {
                    equipmentBonuses = ((_player.AbilityScores[i].AdjustedMax - 18) / 10) - _player.AbilityScores[i].InnateMax + 18;
                }
                if (_player.AbilityScores[i].InnateMax > 18 && _player.AbilityScores[i].AdjustedMax <= 18)
                {
                    equipmentBonuses = _player.AbilityScores[i].AdjustedMax - ((_player.AbilityScores[i].InnateMax - 18) / 10) - 19;
                }
                // Take out the bonuses we got for our our race and profession
                equipmentBonuses -= _player.Race.AbilityBonus[i];
                equipmentBonuses -= _player.Profession.AbilityBonus[i];
                // Print each of the scores and bonuses
                Gui.Print(Colour.Blue, GlobalData.StatNames[i], row + i, statCol);
                string buf = _player.AbilityScores[i].InnateMax.StatToString();
                Gui.Print(Colour.Purple, buf, row + i, statCol + 4);
                buf = _player.Race.AbilityBonus[i].ToString("+0;-0;+0").PadLeft(3);
                Gui.Print(Colour.Brown, buf, row + i, statCol + 13);
                buf = _player.Profession.AbilityBonus[i].ToString("+0;-0;+0").PadLeft(3);
                Gui.Print(Colour.Brown, buf, row + i, statCol + 19);
                buf = equipmentBonuses.ToString("+0;-0;+0").PadLeft(3);
                Gui.Print(Colour.Brown, buf, row + i, statCol + 24);
                buf = _player.AbilityScores[i].AdjustedMax.StatToString();
                Gui.Print(Colour.Green, buf, row + i, statCol + 27);
                if (_player.AbilityScores[i].Adjusted < _player.AbilityScores[i].AdjustedMax)
                {
                    buf = _player.AbilityScores[i].Adjusted.StatToString();
                    Gui.Print(Colour.Red, buf, row + i, statCol + 35);
                }
            }
            // Printe the bonuses for each score and each item we have
            int col = statCol + 44;
            Gui.Print(Colour.Blue, "abcdefghijklm@", row - 1, col);
            Gui.Print(Colour.Blue, "Modifications", row + 6, col);
            for (i = InventorySlot.MeleeWeapon; i < InventorySlot.Total; i++)
            {
                Item item = _player.Inventory[i];
                // Only extract known bonuses, not full bonuses
                item.ObjectFlagsKnown(f1, f2, f3);
                for (stat = 0; stat < 6; stat++)
                {
                    a = Colour.Grey;
                    c = '.';
                    if (f1.IsSet(1u << stat))
                    {
                        c = '*';
                        if (item.TypeSpecificValue > 0)
                        {
                            a = Colour.Green;
                            if (item.TypeSpecificValue < 10)
                            {
                                c = (char)('0' + (char)item.TypeSpecificValue);
                            }
                        }
                        if (item.TypeSpecificValue < 0)
                        {
                            a = Colour.Red;
                            if (item.TypeSpecificValue < 10)
                            {
                                c = (char)('0' - (char)item.TypeSpecificValue);
                            }
                        }
                    }
                    if (f2.IsSet(1u << stat))
                    {
                        a = Colour.Green;
                        c = 's';
                    }
                    Gui.Print(a, c, row + stat, col);
                }
                col++;
            }
            // Fake a set of item flags for our character to show along with those of the real items
            _player.GetAbilitiesAsItemFlags(f1, f2, f3);
            for (stat = 0; stat < 6; stat++)
            {
                a = Colour.Grey;
                c = '.';
                int extraModifier = 0;
                // We may have extra bonuses or penalties from mutations
                switch (stat)
                {
                    case Ability.Strength:
                        extraModifier += _player.Dna.StrengthBonus;
                        break;

                    case Ability.Intelligence:
                        extraModifier += _player.Dna.IntelligenceBonus;
                        break;

                    case Ability.Wisdom:
                        extraModifier += _player.Dna.WisdomBonus;
                        break;

                    case Ability.Dexterity:
                        extraModifier += _player.Dna.DexterityBonus;
                        break;

                    case Ability.Constitution:
                        extraModifier += _player.Dna.ConstitutionBonus;
                        break;

                    case Ability.Charisma:
                        extraModifier = _player.Dna.CharismaBonus;
                        if (_player.Dna.CharismaOverride)
                        {
                            extraModifier = 0;
                        }
                        break;
                }
                if (extraModifier != 0)
                {
                    c = '*';
                    if (extraModifier > 0)
                    {
                        a = Colour.Green;
                        if (extraModifier < 10)
                        {
                            c = (char)('0' + (char)extraModifier);
                        }
                    }
                    if (extraModifier < 0)
                    {
                        a = Colour.Red;
                        if (extraModifier < 10)
                        {
                            c = (char)('0' - (char)extraModifier);
                        }
                    }
                }
                if (f2.IsSet(1u << stat))
                {
                    a = Colour.Green;
                    c = 's';
                }
                Gui.Print(a, c, row + stat, col);
            }
        }

        /// <summary>
        /// Display essential player information such as level, experience, gold, health, mana
        /// </summary>
        private void DisplayPlayerEssentials()
        {
            int showTohit = _player.DisplayedAttackBonus;
            int showTodam = _player.DisplayedDamageBonus;
            Item item = _player.Inventory[InventorySlot.MeleeWeapon];
            // Only show bonuses if we know them
            if (item.IsKnown())
            {
                showTohit += item.BonusToHit;
            }
            if (item.IsKnown())
            {
                showTodam += item.BonusDamage;
            }
            // Print some basics
            PrintBonus("+ To Hit    ", showTohit, 30, 1, Colour.Brown);
            PrintBonus("+ To Damage ", showTodam, 31, 1, Colour.Brown);
            PrintBonus("+ To AC     ", _player.DisplayedArmourClassBonus, 32, 1, Colour.Brown);
            PrintShortScore("  Base AC   ", _player.DisplayedBaseArmourClass, 33, 1, Colour.Brown);
            PrintShortScore("Level      ", _player.Level, 30, 28, Colour.Green);
            PrintLongScore("Experience ", _player.ExperiencePoints, 31, 28,
                _player.ExperiencePoints >= _player.MaxExperienceGained ? Colour.Green : Colour.Red);
            PrintLongScore("Max Exp    ", _player.MaxExperienceGained, 32, 28, Colour.Green);
            // If we're max level we don't have any experience to advance
            if (_player.Level >= Constants.PyMaxLevel)
            {
                Gui.Print(Colour.Blue, "Exp to Adv.", 33, 28);
                Gui.Print(Colour.Green, "    *****", 33, 28 + 11);
            }
            else
            {
                PrintLongScore("Exp to Adv.", (int)(GlobalData.PlayerExp[_player.Level - 1] * _player.ExperienceMultiplier / 100L), 33, 28,
                    Colour.Green);
            }
            PrintLongScore("Exp Factor ", _player.ExperienceMultiplier, 34, 28, Colour.Green);
            PrintShortScore("Max Hit Points ", _player.MaxHealth, 30, 52, Colour.Green);
            if (_player.Health >= _player.MaxHealth)
            {
                PrintShortScore("Cur Hit Points ", _player.Health, 31, 52, Colour.Green);
            }
            else if (_player.Health > _player.MaxHealth * GlobalData.HitpointWarn / 10)
            {
                PrintShortScore("Cur Hit Points ", _player.Health, 31, 52, Colour.BrightYellow);
            }
            else
            {
                PrintShortScore("Cur Hit Points ", _player.Health, 31, 52, Colour.BrightRed);
            }
            PrintShortScore("Max SP (Mana)  ", _player.MaxMana, 32, 52, Colour.Green);
            if (_player.Mana >= _player.MaxMana)
            {
                PrintShortScore("Cur SP (Mana)  ", _player.Mana, 33, 52, Colour.Green);
            }
            else if (_player.Mana > _player.MaxMana * GlobalData.HitpointWarn / 10)
            {
                PrintShortScore("Cur SP (Mana)  ", _player.Mana, 33, 52, Colour.BrightYellow);
            }
            else
            {
                PrintShortScore("Cur SP (Mana)  ", _player.Mana, 33, 52, Colour.BrightRed);
            }
            PrintLongScore("Gold           ", _player.Gold, 34, 52, Colour.Green);
        }

        /// <summary>
        /// Display the player's character history
        /// </summary>
        private void DisplayPlayerHistory()
        {
            for (int i = 0; i < 4; i++)
            {
                Gui.Print(Colour.Brown, _player.History[i], i + 9, 10);
            }
        }

        /// <summary>
        /// Dislpay ther player's skills
        /// </summary>
        private void DisplayPlayerSkills()
        {
            Item item = _player.Inventory[InventorySlot.MeleeWeapon];
            int tmp = _player.AttackBonus + item.BonusToHit;
            int fighting = _player.SkillMelee + (tmp * Constants.BthPlusAdj);
            item = _player.Inventory[InventorySlot.RangedWeapon];
            tmp = _player.AttackBonus + item.BonusToHit;
            int shooting = _player.SkillRanged + (tmp * Constants.BthPlusAdj);
            item = _player.Inventory[InventorySlot.MeleeWeapon];
            int dambonus = _player.DisplayedDamageBonus;
            // Only include weapon damage if the player knows what it is
            if (item.IsKnown())
            {
                dambonus += item.BonusDamage;
            }
            int damdice = item.DamageDice;
            int damsides = item.DamageDiceSides;
            int attacksPerRound = _player.MeleeAttacksPerRound;
            int disarmTraps = _player.SkillDisarmTraps;
            int useDevice = _player.SkillUseDevice;
            int savingThrow = _player.SkillSavingThrow;
            int stealth = _player.SkillStealth;
            int searching = _player.SkillSearching;
            int searchFrequency = _player.SkillSearchFrequency;
            Gui.Print(Colour.Blue, "Fighting    :", 36, 1);
            PrintCategorisedNumber(fighting, 12, 36, 15);
            Gui.Print(Colour.Blue, "Shooting    :", 37, 1);
            PrintCategorisedNumber(shooting, 12, 37, 15);
            Gui.Print(Colour.Blue, "Saving Throw:", 38, 1);
            PrintCategorisedNumber(savingThrow, 6, 38, 15);
            Gui.Print(Colour.Blue, "Stealth     :", 39, 1);
            PrintCategorisedNumber(stealth, 1, 39, 15);
            Gui.Print(Colour.Blue, "Perception  :", 36, 28);
            PrintCategorisedNumber(searchFrequency, 6, 36, 42);
            Gui.Print(Colour.Blue, "Searching   :", 37, 28);
            PrintCategorisedNumber(searching, 6, 37, 42);
            Gui.Print(Colour.Blue, "Disarming   :", 38, 28);
            PrintCategorisedNumber(disarmTraps, 8, 38, 42);
            Gui.Print(Colour.Blue, "Magic Device:", 39, 28);
            PrintCategorisedNumber(useDevice, 6, 39, 42);
            Gui.Print(Colour.Blue, "Blows/Action:", 36, 55);
            Gui.Print(Colour.Green, $"{_player.MeleeAttacksPerRound}", 36, 69);
            Gui.Print(Colour.Blue, "Tot.Dmg./Act:", 37, 55);
            // Work out damage per action
            var buf = string.Empty;
            if (damdice == 0 || damsides == 0)
            {
                buf = dambonus <= 0 ? "nil!" : $"{attacksPerRound * dambonus}";
            }
            else
            {
                buf = dambonus == 0
                    ? $"{attacksPerRound * damdice}d{damsides}"
                    : $"{attacksPerRound * damdice}d{damsides}{attacksPerRound * dambonus:+0;-0;+0}";
            }
            Gui.Print(Colour.Green, buf, 37, 69);
            Gui.Print(Colour.Blue, "Shots/Action:", 38, 55);
            Gui.Print(Colour.Green, $"{_player.MissileAttacksPerRound}", 38, 69);
            Gui.Print(Colour.Blue, "Infra-Vision:", 39, 55);
            Gui.Print(Colour.Green, $"{_player.InfravisionRange * 10} feet", 39, 69);
        }

        /// <summary>
        /// Display the top panel of the character sheet with name, race, age, and so forth
        /// </summary>
        private void DisplayPlayerTop()
        {
            string realmBuff = "";
            Gui.Print(Colour.Blue, "Name        :", 2, 1);
            Gui.Print(Colour.Blue, "Gender      :", 3, 1);
            Gui.Print(Colour.Blue, "Race        :", 4, 1);
            Gui.Print(Colour.Blue, "Class       :", 5, 1);
            if (_player.Realm1 != 0 || _player.Realm2 != 0)
            {
                Gui.Print(Colour.Blue, "Magic       :", 6, 1);
            }
            Gui.Print(Colour.Brown, _player.Name, 2, 15);
            Gui.Print(Colour.Brown, _player.Gender.Title, 3, 15);
            Gui.Print(Colour.Brown, _player.Race.Title, 4, 15);
            Gui.Print(Colour.Brown, Profession.ClassSubName(_player.ProfessionIndex, _player.Realm1), 5, 15);
            // Only print realms if we have them
            if (_player.Realm1 != 0)
            {
                if (_player.Realm2 != 0)
                {
                    realmBuff = Spellcasting.RealmName(_player.Realm1) + "/" + Spellcasting.RealmName(_player.Realm2);
                }
                else
                {
                    realmBuff = Spellcasting.RealmName(_player.Realm1);
                }
            }
            if (_player.Realm1 != 0)
            {
                Gui.Print(Colour.Brown, realmBuff, 6, 15);
            }
            // Fanatics and Cultists get a patron
            if (_player.ProfessionIndex == CharacterClass.Fanatic || _player.ProfessionIndex == CharacterClass.Cultist)
            {
                Gui.Print(Colour.Blue, "Patron      :", 7, 1);
                Gui.Print(Colour.Brown, _player.GooPatron.LongName, 7, 15);
            }
            // Priests get a deity
            if (_player.Religion.Deity != Pantheon.GodName.None)
            {
                Gui.Print(Colour.Blue, "Deity       :", 7, 1);
                Gui.Print(Colour.Brown, _player.Religion.GetPatronDeity().LongName, 7, 15);
            }
            Gui.Print(Colour.Blue, "Birthday", 2, 32);
            string dateBuff = _player.GameTime.BirthdayText.PadLeft(8);
            Gui.Print(Colour.Brown, dateBuff, 2, 46);
            PrintShortScore("Age          ", _player.Age, 3, 32, Colour.Brown);
            PrintShortScore("Height       ", _player.Height, 4, 32, Colour.Brown);
            PrintShortScore("Weight       ", _player.Weight, 5, 32, Colour.Brown);
            PrintShortScore("Social Class ", _player.SocialClass, 6, 32, Colour.Brown);
            int i;
            // Print a quick summary of ability scores, but no detail
            for (i = 0; i < 6; i++)
            {
                string buf;
                if (_player.AbilityScores[i].Innate < _player.AbilityScores[i].InnateMax)
                {
                    Gui.Print(Colour.Blue, GlobalData.StatNamesReduced[i], 2 + i, 61);
                    int value = _player.AbilityScores[i].Adjusted;
                    buf = value.StatToString();
                    Gui.Print(Colour.Red, buf, 2 + i, 66);
                    value = _player.AbilityScores[i].AdjustedMax;
                    buf = value.StatToString();
                    Gui.Print(Colour.Green, buf, 2 + i, 73);
                }
                else
                {
                    Gui.Print(Colour.Blue, GlobalData.StatNames[i], 2 + i, 61);
                    buf = _player.AbilityScores[i].Adjusted.StatToString();
                    Gui.Print(Colour.Green, buf, 2 + i, 66);
                }
            }
        }

        /// <summary>
        /// Print a number with a title, with a plus or minus sign
        /// </summary>
        /// <param name="title"> The title to put with the number </param>
        /// <param name="number"> The number </param>
        /// <param name="row"> The row on which to print </param>
        /// <param name="col"> The column in which to start printing </param>
        /// <param name="colour"> The colour in which to print the number </param>
        private void PrintBonus(string header, int num, int row, int col, Colour colour)
        {
            int len = header.Length;
            Gui.Print(Colour.Blue, header, row, col);
            Gui.Print(Colour.Blue, "   ", row, col + len);
            string outVal = num.ToString("+0;-0;0").PadLeft(6);
            Gui.Print(colour, outVal, row, col + len + 3);
        }

        /// <summary>
        /// Use a scale to divide a score into categories to show how good the score is
        /// </summary>
        /// <param name="score"> The score to be displayed </param>
        /// <param name="divider"> The divider for categories </param>
        /// <param name="screenRow"> The row at which to print the text </param>
        /// <param name="screenCol"> The column at which to print the text </param>
        private void PrintCategorisedNumber(int score, int divider, int screenRow, int screenCol)
        {
            if (divider <= 0)
            {
                divider = 1;
            }
            Colour colour;
            string text;
            var scoreString = (score.ToString() + "%").PadLeft(4);
            if (score < 0)
            {
                colour = Colour.Black;
                text = $"Awful {score}%";
            }
            else
            {
                switch (score / divider)
                {
                    case 0:
                    case 1:
                        {
                            colour = Colour.Red;
                            text = $"Bad     {scoreString}";
                            break;
                        }
                    case 2:
                        {
                            colour = Colour.BrightRed;
                            text = $"Poor    {scoreString}";
                            break;
                        }
                    case 3:
                    case 4:
                        {
                            colour = Colour.Pink;
                            text = $"Medium  {scoreString}";
                            break;
                        }
                    case 5:
                        {
                            colour = Colour.Orange;
                            text = $"Fair    {scoreString}";
                            break;
                        }
                    case 6:
                        {
                            colour = Colour.BrightYellow;
                            text = $"Good    {scoreString}";
                            break;
                        }
                    case 7:
                    case 8:
                        {
                            colour = Colour.Green;
                            text = $"Great   {scoreString}";
                            break;
                        }
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                    case 13:
                        {
                            colour = Colour.BrightGreen;
                            text = $"Superb  {scoreString}";
                            break;
                        }
                    case 14:
                    case 15:
                    case 16:
                    case 17:
                        {
                            colour = Colour.Blue;
                            text = $"Amazing {scoreString}";
                            break;
                        }
                    default:
                        {
                            colour = Colour.Purple;
                            text = $"Stellar {scoreString}";
                            break;
                        }
                }
            }
            Gui.Print(colour, text, screenRow, screenCol);
        }

        /// <summary>
        /// Print a number with a title, justified to nine characters
        /// </summary>
        /// <param name="title"> The title to put with the number </param>
        /// <param name="number"> The number </param>
        /// <param name="row"> The row on which to print </param>
        /// <param name="col"> The column in which to start printing </param>
        /// <param name="colour"> The colour in which to print the number </param>
        private void PrintLongScore(string title, int number, int row, int col, Colour colour)
        {
            int len = title.Length;
            Gui.Print(Colour.Blue, title, row, col);
            string outVal = number.ToString().PadLeft(9);
            Gui.Print(colour, outVal, row, col + len);
        }

        /// <summary>
        /// Print a number with a title, justified to six characters
        /// </summary>
        /// <param name="title"> The title to put with the number </param>
        /// <param name="number"> The number </param>
        /// <param name="row"> The row on which to print </param>
        /// <param name="col"> The column in which to start printing </param>
        /// <param name="colour"> The colour in which to print the number </param>
        private void PrintShortScore(string header, int num, int row, int col, Colour colour)
        {
            int len = header.Length;
            Gui.Print(Colour.Blue, header, row, col);
            Gui.Print(Colour.Blue, "   ", row, col + len);
            string outVal = num.ToString().PadLeft(6);
            Gui.Print(colour, outVal, row, col + len + 3);
        }
    }
}