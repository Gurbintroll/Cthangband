// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Projection;
using Cthangband.Spells.Base;
using Cthangband.StaticData;
using Cthangband.Talents.Base;
using Cthangband.UI;
using System;

namespace Cthangband.Spells
{
    [Serializable]
    internal class CastingHandler
    {
        private readonly Level _level;
        private readonly Player _player;

        public CastingHandler(Player player, Level level)
        {
            _player = player;
            _level = level;
        }

        public void Cast()
        {
            if (_player.HasAntiMagic)
            {
                string whichMagicType = "magic";
                if (_player.CharacterClassIndex == CharacterClassId.Mindcrafter || _player.CharacterClassIndex == CharacterClassId.Mystic)
                {
                    whichMagicType = "psychic talents";
                }
                else if (_player.Spellcasting.Type == CastingType.Divine)
                {
                    whichMagicType = "prayer";
                }
                Profile.Instance.MsgPrint($"An anti-magic shell disrupts your {whichMagicType}!");
                SaveGame.Instance.EnergyUse = 5;
            }
            else
            {
                if (_player.Spellcasting.Type == CastingType.Mentalism)
                {
                    DoCmdMentalism();
                }
                else
                {
                    DoCmdCast();
                }
            }
        }

        public bool GetSpell(out int sn, string prompt, int sval, bool known, bool realm2)
        {
            int i;
            int spell;
            int num = 0;
            int[] spells = new int[64];
            Realm useRealm = realm2 ? _player.Realm2 : _player.Realm1;
            string p = _player.Spellcasting.Type == CastingType.Divine ? "prayer" : "spell";
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
                if (_player.SpellOkay(spells[i], known, realm2))
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
                        _player.PrintSpells(spells, num, 1, 20, useRealm);
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
                if (!_player.SpellOkay(spell, known, realm2))
                {
                    Profile.Instance.MsgPrint($"You may not {prompt} that {p}.");
                    continue;
                }
                if (ask)
                {
                    ISpell sPtr = _player.Spellcasting.Spells[realm2 ? 1 : 0][spell % 32];
                    string tmpVal = $"{prompt} {sPtr.Name} ({sPtr.VisCost} vis, {sPtr.FailureChance(_player)}% fail)? ";
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

        private void DoCmdCast()
        {
            string prayer = _player.Spellcasting.Type == CastingType.Divine ? "prayer" : "spell";
            if (_player.Realm1 == 0)
            {
                Profile.Instance.MsgPrint("You cannot cast spells!");
                return;
            }
            if (_player.TimedBlindness != 0 || _level.NoLight())
            {
                Profile.Instance.MsgPrint("You cannot see!");
                return;
            }
            if (_player.TimedConfusion != 0)
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
            Item oPtr = item >= 0 ? _player.Inventory[item] : _level.Items[0 - item];
            int sval = oPtr.ItemSubCategory;
            bool useSetTwo = oPtr.Category == _player.Realm2.ToSpellBookItemCategory();
            SaveGame.Instance.HandleStuff();
            if (!GetSpell(out int spell, _player.Spellcasting.Type == CastingType.Divine ? "recite" : "cast", sval,
                true, useSetTwo))
            {
                if (spell == -2)
                {
                    Profile.Instance.MsgPrint($"You don't know any {prayer}s in that book.");
                }
                return;
            }
            ISpell sPtr = useSetTwo ? _player.Spellcasting.Spells[1][spell] : _player.Spellcasting.Spells[0][spell];
            if (sPtr.VisCost > _player.Vis)
            {
                string cast = _player.Spellcasting.Type == CastingType.Divine ? "recite" : "cast";
                Profile.Instance.MsgPrint($"You do not have enough vis to {cast} this {prayer}.");
                if (!Gui.GetCheck("Attempt it anyway? "))
                {
                    return;
                }
            }
            int chance = sPtr.FailureChance(_player);
            if (Program.Rng.RandomLessThan(100) < chance)
            {
                Profile.Instance.MsgPrint($"You failed to get the {prayer} off!");
                if (oPtr.Category == ItemCategory.ChaosBook && Program.Rng.DieRoll(100) < spell)
                {
                    Profile.Instance.MsgPrint("You produce a chaotic effect!");
                    WildMagic(spell);
                }
                else if (oPtr.Category == ItemCategory.DeathBook && Program.Rng.DieRoll(100) < spell)
                {
                    if (sval == 3 && Program.Rng.DieRoll(2) == 1)
                    {
                        _level.Monsters[0].SanityBlast(true);
                    }
                    else
                    {
                        Profile.Instance.MsgPrint("It hurts!");
                        _player.TakeHit(Program.Rng.DiceRoll(oPtr.ItemSubCategory + 1, 6), "a miscast Death spell");
                        if (spell > 15 && Program.Rng.DieRoll(6) == 1 && !_player.HasHoldLife)
                        {
                            _player.LoseExperience(spell * 250);
                        }
                    }
                }
            }
            else
            {
                sPtr.Cast(SaveGame.Instance, _player, _level);
                if (!sPtr.Worked)
                {
                    int e = sPtr.FirstCastExperience;
                    sPtr.Worked = true;
                    _player.GainExperience(e * sPtr.Level);
                }
            }
            SaveGame.Instance.EnergyUse = 100;
            if (sPtr.VisCost <= _player.Vis)
            {
                _player.Vis -= sPtr.VisCost;
            }
            else
            {
                int oops = sPtr.VisCost - _player.Vis;
                _player.Vis = 0;
                _player.FractionalVis = 0;
                Profile.Instance.MsgPrint("You faint from the effort!");
                _player.SetTimedParalysis(_player.TimedParalysis + Program.Rng.DieRoll((5 * oops) + 1));
                if (Program.Rng.RandomLessThan(100) < 50)
                {
                    bool perm = Program.Rng.RandomLessThan(100) < 25;
                    Profile.Instance.MsgPrint("You have damaged your health!");
                    _player.DecreaseAbilityScore(Ability.Constitution, 15 + Program.Rng.DieRoll(10), perm);
                }
            }
            _player.RedrawNeeded.Set(RedrawFlag.PrVis);
        }

        private void DoCmdMentalism()
        {
            int plev = _player.Level;
            if (_player.TimedConfusion != 0)
            {
                Profile.Instance.MsgPrint("You are too confused!");
                return;
            }
            if (!GetMentalismTalent(out int n))
            {
                return;
            }
            ITalent talent = _player.Spellcasting.Talents[n];
            if (talent.VisCost > _player.Vis)
            {
                Profile.Instance.MsgPrint("You do not have enough vis to use this talent.");
                if (!Gui.GetCheck("Attempt it anyway? "))
                {
                    return;
                }
            }
            int chance = talent.FailureChance(_player);
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
                        _player.SetTimedHallucinations(_player.TimedHallucinations + 5 + Program.Rng.DieRoll(10));
                    }
                    else if (i < 45)
                    {
                        Profile.Instance.MsgPrint("Your brain is addled!");
                        _player.SetTimedConfusion(_player.TimedConfusion + Program.Rng.DieRoll(8));
                    }
                    else if (i < 90)
                    {
                        _player.SetTimedStun(_player.TimedStun + Program.Rng.DieRoll(8));
                    }
                    else
                    {
                        Profile.Instance.MsgPrint("Your mind unleashes its power in an uncontrollable storm!");
                        SaveGame.Instance.SpellEffects.Project(1, 2 + (plev / 10), _player.MapY, _player.MapX, plev * 2,
                            new ProjectVis(SaveGame.Instance.SpellEffects),
                            ProjectionFlag.ProjectJump | ProjectionFlag.ProjectKill | ProjectionFlag.ProjectGrid |
                            ProjectionFlag.ProjectItem);
                        _player.Vis = Math.Max(0, _player.Vis - (plev * Math.Max(1, plev / 10)));
                    }
                }
            }
            else
            {
                talent.Use(_player, _level, SaveGame.Instance);
            }
            SaveGame.Instance.EnergyUse = 100;
            if (talent.VisCost <= _player.Vis)
            {
                _player.Vis -= talent.VisCost;
            }
            else
            {
                int oops = talent.VisCost - _player.Vis;
                _player.Vis = 0;
                _player.FractionalVis = 0;
                Profile.Instance.MsgPrint("You faint from the effort!");
                _player.SetTimedParalysis(_player.TimedParalysis + Program.Rng.DieRoll((5 * oops) + 1));
                if (Program.Rng.RandomLessThan(100) < 50)
                {
                    bool perm = Program.Rng.RandomLessThan(100) < 25;
                    Profile.Instance.MsgPrint("You have damaged your mind!");
                    _player.DecreaseAbilityScore(Ability.Wisdom, 15 + Program.Rng.DieRoll(10), perm);
                }
            }
            _player.RedrawNeeded.Set(RedrawFlag.PrVis);
        }

        private bool GetMentalismTalent(out int sn)
        {
            int i;
            int num = 0;
            int y = 1;
            int x = 20;
            int plev = _player.Level;
            string p = "talent";
            sn = -1;
            bool flag = false;
            bool redraw = false;
            TalentList talents = _player.Spellcasting.Talents;
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
                        Gui.Print("Lv Vis Fail Info", y, x + 35);
                        for (i = 0; i < talents.Count; i++)
                        {
                            ITalent talent = talents[i];
                            if (talent.Level > plev)
                            {
                                break;
                            }
                            string psiDesc = $"  {i.IndexToLetter()}) {talent.SummaryLine(_player)}";
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

        private void WildMagic(int spell)
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
                    SaveGame.Instance.SpellEffects.Earthquake(_player.MapY, _player.MapX, 5);
                    break;

                case 27:
                case 28:
                    _player.Dna.GainMutation();
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
                        _level.Monsters.SummonSpecific(_player.MapY, _player.MapX, SaveGame.Instance.Difficulty * 3 / 2,
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