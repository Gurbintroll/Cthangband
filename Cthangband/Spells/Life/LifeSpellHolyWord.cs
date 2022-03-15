// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Life
{
    [Serializable]
    internal class LifeSpellHolyWord : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.DispelEvil(player.Level * 4);
            player.RestoreHealth(1000);
            player.SetTimedFear(0);
            player.SetTimedPoison(0);
            player.SetTimedStun(0);
            player.SetTimedBleeding(0);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Holy Word";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 39;
                    ManaCost = 40;
                    BaseFailure = 80;
                    FirstCastExperience = 125;
                    break;

                case CharacterClass.Priest:
                    Level = 39;
                    ManaCost = 32;
                    BaseFailure = 90;
                    FirstCastExperience = 200;
                    break;

                case CharacterClass.Paladin:
                    Level = 39;
                    ManaCost = 38;
                    BaseFailure = 80;
                    FirstCastExperience = 125;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 45;
                    ManaCost = 45;
                    BaseFailure = 80;
                    FirstCastExperience = 125;
                    break;

                case CharacterClass.HighMage:
                    Level = 35;
                    ManaCost = 35;
                    BaseFailure = 60;
                    FirstCastExperience = 125;
                    break;

                default:
                    Level = 99;
                    ManaCost = 0;
                    BaseFailure = 0;
                    FirstCastExperience = 0;
                    break;
            }
        }

        protected override string Comment(Player player)
        {
            return $"d {4 * player.Level}/h 1000";
        }
    }
}