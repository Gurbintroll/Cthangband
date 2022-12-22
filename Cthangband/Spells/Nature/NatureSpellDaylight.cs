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
    internal class NatureSpellDaylight : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.LightArea(Program.Rng.DiceRoll(2, player.Level / 2), (player.Level / 10) + 1);
            if (!player.Race.IsSunlightSensitive || player.HasLightResistance)
            {
                return;
            }
            Profile.Instance.MsgPrint("The daylight scorches your flesh!");
            player.TakeHit(Program.Rng.DiceRoll(2, 2), "daylight");
        }

        public override void Initialise(int characterClass)
        {
            Name = "Daylight";
            switch (characterClass)
            {
                case CharacterClassId.Mage:
                    Level = 4;
                    VisCost = 4;
                    BaseFailure = 50;
                    FirstCastExperience = 5;
                    break;

                case CharacterClassId.Priest:
                    Level = 6;
                    VisCost = 5;
                    BaseFailure = 50;
                    FirstCastExperience = 5;
                    break;

                case CharacterClassId.Ranger:
                    Level = 6;
                    VisCost = 7;
                    BaseFailure = 50;
                    FirstCastExperience = 3;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Cultist:
                    Level = 6;
                    VisCost = 6;
                    BaseFailure = 50;
                    FirstCastExperience = 5;
                    break;

                case CharacterClassId.HighMage:
                case CharacterClassId.Druid:
                    Level = 3;
                    VisCost = 3;
                    BaseFailure = 40;
                    FirstCastExperience = 5;
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
            return $"dam 2d{player.Level / 2}";
        }
    }
}