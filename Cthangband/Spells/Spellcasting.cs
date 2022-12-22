// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Spells.Base;
using System;

namespace Cthangband.Spells
{
    [Serializable]
    internal class Spellcasting
    {
        public readonly int SpellFirst;
        public readonly int[] SpellOrder = new int[64];
        public readonly SpellList[] Spells = new SpellList[2];
        public readonly int SpellStat;
        public readonly int SpellWeight;
        public readonly TalentList Talents;
        public readonly CastingType Type;

        public Spellcasting(Player player)
        {
            Spells[0] = new SpellList(player.Realm1, player.CharacterClassIndex);
            Spells[1] = new SpellList(player.Realm2, player.CharacterClassIndex);
            Talents = new TalentList(player.CharacterClassIndex);
            switch (player.CharacterClassIndex)
            {
                case CharacterClassId.Mage:
                case CharacterClassId.HighMage:
                case CharacterClassId.WarriorMage:
                case CharacterClassId.Rogue:
                case CharacterClassId.Cultist:
                    Type = CastingType.Arcane;
                    break;

                case CharacterClassId.Priest:
                case CharacterClassId.Paladin:
                case CharacterClassId.Ranger:
                case CharacterClassId.Druid:
                case CharacterClassId.Fanatic:
                case CharacterClassId.Monk:
                    Type = CastingType.Divine;
                    break;

                case CharacterClassId.Mindcrafter:
                case CharacterClassId.Mystic:
                    Type = CastingType.Mentalism;
                    break;

                case CharacterClassId.Channeler:
                    Type = CastingType.Channeling;
                    break;

                default:
                    Type = CastingType.None;
                    break;
            }
            switch (player.CharacterClassIndex)
            {
                case CharacterClassId.Mage:
                case CharacterClassId.HighMage:
                case CharacterClassId.WarriorMage:
                case CharacterClassId.Rogue:
                case CharacterClassId.Ranger:
                case CharacterClassId.Cultist:
                case CharacterClassId.Fanatic:
                    SpellStat = Ability.Intelligence;
                    break;

                case CharacterClassId.Priest:
                case CharacterClassId.Paladin:
                case CharacterClassId.Druid:
                case CharacterClassId.Monk:
                case CharacterClassId.Mindcrafter:
                case CharacterClassId.Mystic:
                    SpellStat = Ability.Wisdom;
                    break;

                case CharacterClassId.Channeler:
                    SpellStat = Ability.Charisma;
                    break;

                default:
                    SpellStat = Ability.Strength;
                    break;
            }
            switch (player.CharacterClassIndex)
            {
                case CharacterClassId.Mage:
                case CharacterClassId.Monk:
                case CharacterClassId.Mindcrafter:
                case CharacterClassId.Mystic:
                case CharacterClassId.HighMage:
                case CharacterClassId.Cultist:
                    SpellWeight = 300;
                    break;

                case CharacterClassId.Priest:
                case CharacterClassId.Rogue:
                case CharacterClassId.WarriorMage:
                case CharacterClassId.Druid:
                    SpellWeight = 350;
                    break;

                case CharacterClassId.Ranger:
                case CharacterClassId.Paladin:
                case CharacterClassId.Fanatic:
                case CharacterClassId.Channeler:
                    SpellWeight = 400;
                    break;

                default:
                    SpellWeight = 0;
                    break;
            }
            SpellFirst = 100;
            foreach (SpellList bookset in Spells)
            {
                foreach (ISpell spell in bookset)
                {
                    if (spell.Level < SpellFirst)
                    {
                        SpellFirst = spell.Level;
                    }
                }
            }
            for (int i = 0; i < 64; i++)
            {
                SpellOrder[i] = 99;
            }
        }

        public static string RealmName(Realm realm)
        {
            switch (realm)
            {
                case Realm.None:
                    return "None";

                case Realm.Life:
                    return "Life";

                case Realm.Sorcery:
                    return "Sorcery";

                case Realm.Nature:
                    return "Nature";

                case Realm.Chaos:
                    return "Chaos";

                case Realm.Death:
                    return "Death";

                case Realm.Tarot:
                    return "Tarot";

                case Realm.Folk:
                    return "Folk";

                case Realm.Corporeal:
                    return "Corporeal";

                default:
                    return "Unknown Realm";
            }
        }
    }
}