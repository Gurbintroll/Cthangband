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
using System;

namespace Cthangband.Spells.Nature
{
    [Serializable]
    internal class NatureSpellCallSunlight : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.FireBall(new ProjectLight(SaveGame.Instance.SpellEffects), 0, 150, 8);
            level.WizLight();
            if (player.RaceIndex != RaceId.Vampire || player.HasLightResistance)
            {
                return;
            }
            Profile.Instance.MsgPrint("The sunlight scorches your flesh!");
            player.TakeHit(50, "sunlight");
        }

        public override void Initialise(int characterClass)
        {
            Name = "Whirlpool";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 37;
                    VrilCost = 35;
                    BaseFailure = 90;
                    FirstCastExperience = 100;
                    break;

                case CharacterClass.Priest:
                    Level = 39;
                    VrilCost = 38;
                    BaseFailure = 90;
                    FirstCastExperience = 100;
                    break;

                case CharacterClass.Ranger:
                    Level = 40;
                    VrilCost = 35;
                    BaseFailure = 75;
                    FirstCastExperience = 100;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 41;
                    VrilCost = 41;
                    BaseFailure = 90;
                    FirstCastExperience = 100;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Druid:
                    Level = 34;
                    VrilCost = 30;
                    BaseFailure = 80;
                    FirstCastExperience = 100;
                    break;

                default:
                    Level = 99;
                    VrilCost = 0;
                    BaseFailure = 0;
                    FirstCastExperience = 0;
                    break;
            }
        }

        protected override string Comment(Player player)
        {
            return "dam 150";
        }
    }
}