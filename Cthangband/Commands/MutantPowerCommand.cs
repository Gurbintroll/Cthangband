using Cthangband.Enumerations;
using Cthangband.StaticData;
using Cthangband.UI;
using System;

namespace Cthangband.Commands
{
    /// <summary>
    /// Use a mutant or racial power
    /// </summary>
    [Serializable]
    internal class MutantPowerCommand : ICommand
    {
        public char Key => 'p';

        public int? Repeat => 0;

        public bool IsEnabled => true;

        public void Execute(Player player, Level level)
        {
            int i = 0;
            int num;
            int[] powers = new int[36];
            string[] powerDesc = new string[36];
            int lvl = player.Level;
            int pets = 0;
            int petCtr;
            bool allPets = false;
            Monster monster;
            bool hasRacial = false;
            string racialPower = "(none)";
            for (num = 0; num < 36; num++)
            {
                powers[num] = 0;
                powerDesc[num] = "";
            }
            num = 0;
            if (player.TimedConfusion != 0)
            {
                Profile.Instance.MsgPrint("You are too confused to use any powers!");
                SaveGame.Instance.EnergyUse = 0;
                return;
            }
            switch (player.RaceIndex)
            {
                case RaceId.Dwarf:
                    racialPower = lvl < 5
                        ? "detect doors+traps (racial, unusable until level 5)"
                        : "detect doors+traps (racial, cost 5, WIS based)";
                    hasRacial = true;
                    break;

                case RaceId.Nibelung:
                    racialPower = lvl < 5
                        ? "detect doors+traps (racial, WIS based, unusable until level 5)"
                        : "detect doors+traps (racial, cost 5, WIS based)";
                    hasRacial = true;
                    break;

                case RaceId.Hobbit:
                    racialPower = lvl < 15
                        ? "create food        (racial, unusable until level 15)"
                        : "create food        (racial, cost 10, INT based)";
                    hasRacial = true;
                    break;

                case RaceId.Gnome:
                    racialPower = lvl < 5
                        ? "teleport           (racial, unusable until level 5)"
                        : "teleport           (racial, cost 5 + lvl/5, INT based)";
                    hasRacial = true;
                    break;

                case RaceId.HalfOrc:
                    racialPower = lvl < 3
                        ? "remove fear        (racial, unusable until level 3)"
                        : "remove fear        (racial, cost 5, WIS based)";
                    hasRacial = true;
                    break;

                case RaceId.HalfTroll:
                    racialPower = lvl < 10
                        ? "berserk            (racial, unusable until level 10)"
                        : "berserk            (racial, cost 12, WIS based)";
                    hasRacial = true;
                    break;

                case RaceId.TchoTcho:
                    racialPower = lvl < 8
                        ? "berserk            (racial, unusable until level 8)"
                        : "berserk            (racial, cost 10, WIS based)";
                    hasRacial = true;
                    break;

                case RaceId.Great:
                    racialPower = "dream powers    (unusable until level 30/40)";
                    hasRacial = true;
                    break;

                case RaceId.HalfOgre:
                    racialPower = lvl < 25
                        ? "Yellow Sign     (racial, unusable until level 25)"
                        : "Yellow Sign     (racial, cost 35, INT based)";
                    hasRacial = true;
                    break;

                case RaceId.HalfGiant:
                    racialPower = lvl < 20
                        ? "stone to mud       (racial, unusable until level 20)"
                        : "stone to mud       (racial, cost 10, STR based)";
                    hasRacial = true;
                    break;

                case RaceId.HalfTitan:
                    racialPower = lvl < 35
                        ? "probing            (racial, unusable until level 35)"
                        : "probing            (racial, cost 20, INT based)";
                    hasRacial = true;
                    break;

                case RaceId.Cyclops:
                    racialPower = lvl < 20
                        ? "throw boulder      (racial, unusable until level 20)"
                        : "throw boulder      (racial, cost 15, dam 3*lvl, STR based)";
                    hasRacial = true;
                    break;

                case RaceId.Yeek:
                    racialPower = lvl < 15
                        ? "scare monster      (racial, unusable until level 15)"
                        : "scare monster      (racial, cost 15, WIS based)";
                    hasRacial = true;
                    break;

                case RaceId.Spectre:
                    racialPower = lvl < 4
                        ? "scare monster      (racial, unusable until level 4)"
                        : "scare monster      (racial, cost 3, INT based)";
                    hasRacial = true;
                    break;

                case RaceId.Klackon:
                    racialPower = lvl < 9
                        ? "spit acid          (racial, unusable until level 9)"
                        : "spit acid          (racial, cost 9, dam lvl, DEX based)";
                    hasRacial = true;
                    break;

                case RaceId.Kobold:
                    racialPower = lvl < 12
                        ? "poison dart        (racial, unusable until level 12)"
                        : "poison dart        (racial, cost 8, dam lvl, DEX based)";
                    hasRacial = true;
                    break;

                case RaceId.DarkElf:
                    racialPower = lvl < 2
                        ? "magic missile      (racial, unusable until level 2)"
                        : "magic missile      (racial, cost 2, INT based)";
                    hasRacial = true;
                    break;

                case RaceId.Draconian:
                    racialPower = "breath weapon      (racial, cost lvl, dam 2*lvl, CON based)";
                    hasRacial = true;
                    break;

                case RaceId.MindFlayer:
                    racialPower = lvl < 15
                        ? "mind blast         (racial, unusable until level 15)"
                        : "mind blast         (racial, cost 12, dam lvl, INT based)";
                    hasRacial = true;
                    break;

                case RaceId.Imp:
                    racialPower = lvl < 9
                        ? "fire bolt/ball     (racial, unusable until level 9/30)"
                        : "fire bolt/ball(30) (racial, cost 15, dam lvl, WIS based)";
                    hasRacial = true;
                    break;

                case RaceId.Golem:
                    racialPower = lvl < 20
                        ? "stone skin         (racial, unusable until level 20)"
                        : "stone skin         (racial, cost 15, dur 30+d20, CON based)";
                    hasRacial = true;
                    break;

                case RaceId.Skeleton:
                case RaceId.Zombie:
                    racialPower = lvl < 30
                        ? "restore life       (racial, unusable until level 30)"
                        : "restore life       (racial, cost 30, WIS based)";
                    hasRacial = true;
                    break;

                case RaceId.Vampire:
                    racialPower = lvl < 2
                        ? "drain life         (racial, unusable until level 2)"
                        : "drain life         (racial, cost 1 + lvl/3, based)";
                    hasRacial = true;
                    break;

                case RaceId.Sprite:
                    racialPower = lvl < 12
                        ? "sleeping dust      (racial, unusable until level 12)"
                        : "sleeping dust      (racial, cost 12, INT based)";
                    hasRacial = true;
                    break;
            }
            for (petCtr = level.MMax - 1; petCtr >= 1; petCtr--)
            {
                monster = level.Monsters[petCtr];
                if ((monster.Mind & Constants.SmFriendly) != 0)
                {
                    pets++;
                }
            }
            System.Collections.Generic.List<Mutations.Mutation> activeMutations = player.Dna.ActivatableMutations(player);
            if (!hasRacial && activeMutations.Count == 0 && pets == 0)
            {
                Profile.Instance.MsgPrint("You have no powers to activate.");
                SaveGame.Instance.EnergyUse = 0;
                return;
            }
            if (hasRacial)
            {
                powers[0] = int.MaxValue;
                powerDesc[0] = racialPower;
                num++;
            }
            for (int j = 0; j < activeMutations.Count; j++)
            {
                powers[num] = j + 100;
                powerDesc[num] = activeMutations[j].ActivationSummary(player.Level);
                num++;
            }
            if (pets > 0)
            {
                powerDesc[num] = "dismiss pets";
                powers[num++] = 3;
            }
            bool flag = false;
            bool redraw = false;
            string outVal = $"(Powers {0.IndexToLetter()}-{(num - 1).IndexToLetter()}, *=List, ESC=exit) Use which power? ";
            while (!flag && Gui.GetCom(outVal, out char choice))
            {
                if (choice == ' ' || choice == '*' || choice == '?')
                {
                    if (!redraw)
                    {
                        int y = 1, x = 13;
                        int ctr = 0;
                        redraw = true;
                        Gui.Save();
                        Gui.PrintLine("", y++, x);
                        while (ctr < num)
                        {
                            string dummy = $"{ctr.IndexToLetter()}) {powerDesc[ctr]}";
                            Gui.PrintLine(dummy, y + ctr, x);
                            ctr++;
                        }
                        Gui.PrintLine("", y + ctr, x);
                    }
                    else
                    {
                        redraw = false;
                        Gui.Load();
                    }
                    continue;
                }
                if (choice == '\r' && num == 1)
                {
                    choice = 'a';
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
                    string tmpVal = $"Use {powerDesc[i]}? ";
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
                SaveGame.Instance.EnergyUse = 0;
                return;
            }
            if (powers[i] == int.MaxValue)
            {
                SaveGame.Instance.CommandEngine.UseRacialPower();
            }
            else if (powers[i] == 3)
            {
                int dismissed = 0;
                if (Gui.GetCheck("Dismiss all pets? "))
                {
                    allPets = true;
                }
                for (petCtr = level.MMax - 1; petCtr >= 1; petCtr--)
                {
                    monster = level.Monsters[petCtr];
                    if ((monster.Mind & Constants.SmFriendly) != 0)
                    {
                        bool deleteThis = false;
                        if (allPets)
                        {
                            deleteThis = true;
                        }
                        else
                        {
                            string friendName = monster.MonsterDesc(0x80);
                            string checkFriend = $"Dismiss {friendName}? ";
                            if (Gui.GetCheck(checkFriend))
                            {
                                deleteThis = true;
                            }
                        }
                        if (deleteThis)
                        {
                            level.Monsters.DeleteMonsterByIndex(petCtr, true);
                            dismissed++;
                        }
                    }
                }
                string s = dismissed == 1 ? "" : "s";
                Profile.Instance.MsgPrint($"You have dismissed {dismissed} pet{s}.");
            }
            else
            {
                SaveGame.Instance.EnergyUse = 100;
                activeMutations[powers[i] - 100].Activate(SaveGame.Instance, player, level);
            }
        }
    }
}
