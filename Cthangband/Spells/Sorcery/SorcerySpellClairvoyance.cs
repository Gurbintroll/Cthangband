// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Sorcery
{
    [Serializable]
    internal class SorcerySpellClairvoyance : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            level.WizLight();
            if (!player.HasTelepathy)
            {
                player.SetTimedTelepathy(player.TimedTelepathy + Program.Rng.DieRoll(30) + 25);
            }
        }

        public override void Initialise(int characterClass)
        {
            Name = "Clairvoyance";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 30;
                    ManaCost = 40;
                    BaseFailure = 80;
                    FirstCastExperience = 120;
                    break;

                case CharacterClass.Rogue:
                    Level = 37;
                    ManaCost = 40;
                    BaseFailure = 80;
                    FirstCastExperience = 100;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 35;
                    ManaCost = 45;
                    BaseFailure = 80;
                    FirstCastExperience = 120;
                    break;

                case CharacterClass.HighMage:
                    Level = 25;
                    ManaCost = 30;
                    BaseFailure = 70;
                    FirstCastExperience = 120;
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
            return "dur 25+d30";
        }
    }
}