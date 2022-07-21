using Cthangband.Enumerations;
using Cthangband.Projection;
using Cthangband.Spells;
using Cthangband.StaticData;
using Cthangband.UI;
using System;

namespace Cthangband.Commands
{
    internal class CastCommand : ICommand
    {
        public char Key => 'm';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
            if (player.HasAntiMagic)
            {
                string whichMagicType = "magic";
                if (player.ProfessionIndex == CharacterClass.Mindcrafter || player.ProfessionIndex == CharacterClass.Mystic)
                {
                    whichMagicType = "psychic talents";
                }
                else if (player.Spellcasting.Type == CastingType.Divine)
                {
                    whichMagicType = "prayer";
                }
                Profile.Instance.MsgPrint($"An anti-magic shell disrupts your {whichMagicType}!");
                SaveGame.Instance.EnergyUse = 5;
            }
            else
            {
                if (player.Spellcasting.Type == CastingType.Mentalism)
                {
                    DoCmdMentalism(player, level);
                }
                else
                {
                    DoCmdCast(player, level);
                }
            }
        }

        public static bool GetSpell(out int sn, string prompt, int sval, bool known, bool realm2, Player player)
        {
            int i;
            int spell;
            int num = 0;
            int[] spells = new int[64];
            Realm useRealm = realm2 ? player.Realm2 : player.Realm1;
            string p = player.Spellcasting.Type == CastingType.Divine ? "prayer" : "spell";
            for (spell = 0; spell < 32; spell++)
            {
                if ((GlobalData.BookSpellFlags[sval] & (1u << spell)) != 0)
                {
                    spells[num++] = spell;
                }
            }
            bool okay = false;
            sn = -2;
            for (i = 0; i < num; i++)
            {
                if (player.SpellOkay(spells[i], known, realm2))
                {
                    okay = true;
                }
            }
            if (!okay)
            {
                return false;
            }
            sn = -1;
            bool flag = false;
            bool redraw = false;
            string outVal = $"({p}s {0.IndexToLetter()}-{(num - 1).IndexToLetter()}, *=List, ESC=exit) {prompt} which {p}? ";
            while (!flag && Gui.GetCom(outVal, out char choice))
            {
                if (choice == ' ' || choice == '*' || choice == '?')
                {
                    if (!redraw)
                    {
                        redraw = true;
                        Gui.Save();
                        player.PrintSpells(spells, num, 1, 20, useRealm);
                    }
                    else
                    {
                        redraw = false;
                        Gui.Load();
                    }
                    continue;
                }
                bool ask = char.IsUpper(choice);
                if (ask)
                {
                    choice = char.ToLower(choice);
                }
                i = char.IsLower(choice) ? choice.LetterToNumber() : -1;
                if (i < 0 || i >= num)
                {
                    continue;
                }
                spell = spells[i];
                if (!player.SpellOkay(spell, known, realm2))
                {
                    Profile.Instance.MsgPrint($"You may not {prompt} that {p}.");
                    continue;
                }
                if (ask)
                {
                    Spell sPtr = player.Spellcasting.Spells[realm2 ? 1 : 0][spell % 32];
                    string tmpVal = $"{prompt} {sPtr.Name} ({sPtr.ManaCost} mana, {sPtr.FailureChance(player)}% fail)? ";
                    if (!Gui.GetCheck(tmpVal))
                    {
                        continue;
                    }
                }
                flag = true;
            }
            if (redraw)
            {
                Gui.Load();
            }
            if (!flag)
            {
                return false;
            }
            sn = spell;
            return true;
        }

        private void DoCmdCast(Player player, Level level)
        {
            string prayer = player.Spellcasting.Type == CastingType.Divine ? "prayer" : "spell";
            if (player.Realm1 == 0)
            {
                Profile.Instance.MsgPrint("You cannot cast spells!");
                return;
            }
            if (player.TimedBlindness != 0 || level.NoLight())
            {
                Profile.Instance.MsgPrint("You cannot see!");
                return;
            }
            if (player.TimedConfusion != 0)
            {
                Profile.Instance.MsgPrint("You are too confused!");
                return;
            }
            Inventory.ItemFilterUseableSpellBook = true;
            if (!SaveGame.Instance.GetItem(out int item, "Use which book? ", false, true, true))
            {
                if (item == -2)
                {
                    Profile.Instance.MsgPrint($"You have no {prayer} books!");
                }
                Inventory.ItemFilterUseableSpellBook = false;
                return;
            }
            Inventory.ItemFilterUseableSpellBook = false;
            Item oPtr = item >= 0 ? player.Inventory[item] : level.Items[0 - item];
            int sval = oPtr.ItemSubCategory;
            bool useSetTwo = oPtr.Category == player.Realm2.ToSpellBookItemCategory();
            SaveGame.Instance.HandleStuff();
            if (!GetSpell(out int spell, player.Spellcasting.Type == CastingType.Divine ? "recite" : "cast", sval,
                true, useSetTwo, player))
            {
                if (spell == -2)
                {
                    Profile.Instance.MsgPrint($"You don't know any {prayer}s in that book.");
                }
                return;
            }
            Spell sPtr = useSetTwo ? player.Spellcasting.Spells[1][spell] : player.Spellcasting.Spells[0][spell];
            if (sPtr.ManaCost > player.Mana)
            {
                string cast = player.Spellcasting.Type == CastingType.Divine ? "recite" : "cast";
                Profile.Instance.MsgPrint($"You do not have enough mana to {cast} this {prayer}.");
                if (!Gui.GetCheck("Attempt it anyway? "))
                {
                    return;
                }
            }
            int chance = sPtr.FailureChance(player);
            if (Program.Rng.RandomLessThan(100) < chance)
            {
                Profile.Instance.MsgPrint($"You failed to get the {prayer} off!");
                if (oPtr.Category == ItemCategory.ChaosBook && Program.Rng.DieRoll(100) < spell)
                {
                    Profile.Instance.MsgPrint("You produce a chaotic effect!");
                    WildMagic(spell, player, level);
                }
                else if (oPtr.Category == ItemCategory.DeathBook && Program.Rng.DieRoll(100) < spell)
                {
                    if (sval == 3 && Program.Rng.DieRoll(2) == 1)
                    {
                        level.Monsters[0].SanityBlast(true);
                    }
                    else
                    {
                        Profile.Instance.MsgPrint("It hurts!");
                        player.TakeHit(Program.Rng.DiceRoll(oPtr.ItemSubCategory + 1, 6), "a miscast Death spell");
                        if (spell > 15 && Program.Rng.DieRoll(6) == 1 && !player.HasHoldLife)
                        {
                            player.LoseExperience(spell * 250);
                        }
                    }
                }
            }
            else
            {
                sPtr.Cast(SaveGame.Instance, player, level);
                if (!sPtr.Worked)
                {
                    int e = sPtr.FirstCastExperience;
                    sPtr.Worked = true;
                    player.GainExperience(e * sPtr.Level);
                }
            }
            SaveGame.Instance.EnergyUse = 100;
            if (sPtr.ManaCost <= player.Mana)
            {
                player.Mana -= sPtr.ManaCost;
            }
            else
            {
                int oops = sPtr.ManaCost - player.Mana;
                player.Mana = 0;
                player.FractionalMana = 0;
                Profile.Instance.MsgPrint("You faint from the effort!");
                player.SetTimedParalysis(player.TimedParalysis + Program.Rng.DieRoll((5 * oops) + 1));
                if (Program.Rng.RandomLessThan(100) < 50)
                {
                    bool perm = Program.Rng.RandomLessThan(100) < 25;
                    Profile.Instance.MsgPrint("You have damaged your health!");
                    player.DecreaseAbilityScore(Ability.Constitution, 15 + Program.Rng.DieRoll(10), perm);
                }
            }
            player.RedrawNeeded.Set(RedrawFlag.PrMana);
        }

        private void DoCmdMentalism(Player player, Level level)
        {
            int plev = player.Level;
            if (player.TimedConfusion != 0)
            {
                Profile.Instance.MsgPrint("You are too confused!");
                return;
            }
            if (!GetMentalismTalent(out int n, player))
            {
                return;
            }
            Talents.Talent talent = player.Spellcasting.Talents[n];
            if (talent.ManaCost > player.Mana)
            {
                Profile.Instance.MsgPrint("You do not have enough mana to use this talent.");
                if (!Gui.GetCheck("Attempt it anyway? "))
                {
                    return;
                }
            }
            int chance = talent.FailureChance(player);
            if (Program.Rng.RandomLessThan(100) < chance)
            {
                Profile.Instance.MsgPrint("You failed to concentrate hard enough!");
                if (Program.Rng.DieRoll(100) < chance / 2)
                {
                    int i = Program.Rng.DieRoll(100);
                    if (i < 5)
                    {
                        Profile.Instance.MsgPrint("Oh, no! Your mind has gone blank!");
                        SaveGame.Instance.SpellEffects.LoseAllInfo();
                    }
                    else if (i < 15)
                    {
                        Profile.Instance.MsgPrint("Weird visions seem to dance before your eyes...");
                        player.SetTimedHallucinations(player.TimedHallucinations + 5 + Program.Rng.DieRoll(10));
                    }
                    else if (i < 45)
                    {
                        Profile.Instance.MsgPrint("Your brain is addled!");
                        player.SetTimedConfusion(player.TimedConfusion + Program.Rng.DieRoll(8));
                    }
                    else if (i < 90)
                    {
                        player.SetTimedStun(player.TimedStun + Program.Rng.DieRoll(8));
                    }
                    else
                    {
                        Profile.Instance.MsgPrint("Your mind unleashes its power in an uncontrollable storm!");
                        SaveGame.Instance.SpellEffects.Project(1, 2 + (plev / 10), player.MapY, player.MapX, plev * 2,
                            new ProjectMana(SaveGame.Instance.SpellEffects),
                            ProjectionFlag.ProjectJump | ProjectionFlag.ProjectKill | ProjectionFlag.ProjectGrid |
                            ProjectionFlag.ProjectItem);
                        player.Mana = Math.Max(0, player.Mana - (plev * Math.Max(1, plev / 10)));
                    }
                }
            }
            else
            {
                talent.Use(player, level, SaveGame.Instance);
            }
            SaveGame.Instance.EnergyUse = 100;
            if (talent.ManaCost <= player.Mana)
            {
                player.Mana -= talent.ManaCost;
            }
            else
            {
                int oops = talent.ManaCost - player.Mana;
                player.Mana = 0;
                player.FractionalMana = 0;
                Profile.Instance.MsgPrint("You faint from the effort!");
                player.SetTimedParalysis(player.TimedParalysis + Program.Rng.DieRoll((5 * oops) + 1));
                if (Program.Rng.RandomLessThan(100) < 50)
                {
                    bool perm = Program.Rng.RandomLessThan(100) < 25;
                    Profile.Instance.MsgPrint("You have damaged your mind!");
                    player.DecreaseAbilityScore(Ability.Wisdom, 15 + Program.Rng.DieRoll(10), perm);
                }
            }
            player.RedrawNeeded.Set(RedrawFlag.PrMana);
        }

        private bool GetMentalismTalent(out int sn, Player player)
        {
            int i;
            int num = 0;
            int y = 1;
            int x = 20;
            int plev = player.Level;
            string p = "talent";
            sn = -1;
            bool flag = false;
            bool redraw = false;
            TalentList talents = player.Spellcasting.Talents;
            for (i = 0; i < talents.Count; i++)
            {
                if (talents[i].Level <= plev)
                {
                    num++;
                }
            }
            string outVal = $"({p}s {0.IndexToLetter()}-{(num - 1).IndexToLetter()}, *=List, ESC=exit) Use which {p}? ";
            while (!flag && Gui.GetCom(outVal, out char choice))
            {
                if (choice == ' ' || choice == '*' || choice == '?')
                {
                    if (!redraw)
                    {
                        redraw = true;
                        Gui.Save();
                        Gui.PrintLine("", y, x);
                        Gui.Print("Name", y, x + 5);
                        Gui.Print("Lv Mana Fail Info", y, x + 35);
                        for (i = 0; i < talents.Count; i++)
                        {
                            Talents.Talent talent = talents[i];
                            if (talent.Level > plev)
                            {
                                break;
                            }
                            string psiDesc = $"  {i.IndexToLetter()}) {talent.SummaryLine(player)}";
                            Gui.PrintLine(psiDesc, y + i + 1, x);
                        }
                        Gui.PrintLine("", y + i + 1, x);
                    }
                    else
                    {
                        redraw = false;
                        Gui.Load();
                    }
                    continue;
                }
                bool ask = char.IsUpper(choice);
                if (ask)
                {
                    choice = char.ToLower(choice);
                }
                i = char.IsLower(choice) ? choice.LetterToNumber() : -1;
                if (i < 0 || i >= num)
                {
                    continue;
                }
                if (ask)
                {
                    string tmpVal = $"Use {talents[i].Name}? ";
                    if (!Gui.GetCheck(tmpVal))
                    {
                        continue;
                    }
                }
                flag = true;
            }
            if (redraw)
            {
                Gui.Load();
            }
            if (!flag)
            {
                return false;
            }
            sn = i;
            return true;
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
                        level.Monsters.SummonSpecific(player.MapY, player.MapX, SaveGame.Instance.Difficulty * 3 / 2,
                            type);
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
