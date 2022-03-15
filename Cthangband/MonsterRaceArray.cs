// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.StaticData;
using System;
using System.Collections.Generic;

namespace Cthangband
{
    [Serializable]
    internal class MonsterRaceArray : List<MonsterRace>
    {
        public MonsterRaceArray()
        {
            Dictionary<string, BaseMonsterRace> baseRaces = StaticResources.Instance.BaseMonsterRaces;
            int index = 0;
            for (int level = -1; level < 128; level++)
            {
                foreach (KeyValuePair<string, BaseMonsterRace> baseMonsterRace in baseRaces)
                {
                    if (baseMonsterRace.Value.Level != level)
                    {
                        continue;
                    }
                    Add(new MonsterRace(baseMonsterRace.Value, index));
                    index++;
                }
            }
        }

        public void AddKnowledge()
        {
            foreach (MonsterRace monsterType in this)
            {
                monsterType.Knowledge = new MonsterKnowledge(monsterType);
            }
        }

        public int IndexFromName(string name)
        {
            foreach (MonsterRace race in this)
            {
                if (race.Name == name)
                {
                    return race.Index;
                }
            }
            return 0;
        }

        public void ResetGuardians()
        {
            foreach (MonsterRace race in this)
            {
                race.Flags1 &= ~MonsterFlag1.Guardian;
            }
        }

        public void ResetUniqueOnlyGuardianStatus()
        {
            foreach (MonsterRace race in this)
            {
                race.Flags1 &= ~MonsterFlag1.OnlyGuardian;
            }
        }
    }
}