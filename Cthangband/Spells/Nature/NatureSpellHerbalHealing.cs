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

namespace Cthangband.Spells.Nature
{
    [Serializable]
    internal class NatureSpellHerbalHealing : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.RestoreHealth(1000);
            player.SetTimedStun(0);
            player.SetTimedBleeding(0);
            player.SetTimedPoison(0);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Herbal Healing";
            switch (characterClass)
            {
                case CharacterClassId.Mage:
                    Level = 40;
                    VisCost = 100;
                    BaseFailure = 95;
                    FirstCastExperience = 50;
                    break;

                case CharacterClassId.Priest:
                    Level = 42;
                    VisCost = 100;
                    BaseFailure = 95;
                    FirstCastExperience = 50;
                    break;

                case CharacterClassId.Ranger:
                    Level = 40;
                    VisCost = 100;
                    BaseFailure = 95;
                    FirstCastExperience = 50;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Cultist:
                    Level = 45;
                    VisCost = 100;
                    BaseFailure = 95;
                    FirstCastExperience = 50;
                    break;

                case CharacterClassId.HighMage:
                case CharacterClassId.Druid:
                    Level = 35;
                    VisCost = 80;
                    BaseFailure = 85;
                    FirstCastExperience = 50;
                    break;

                default:
                    Level = 99;
                    VisCost = 0;
                    BaseFailure = 0;
                    FirstCastExperience = 0;
                    break;
            }
        }

        protected override string Comment(Player player)
        {
            return "heal 1000";
        }
    }
}