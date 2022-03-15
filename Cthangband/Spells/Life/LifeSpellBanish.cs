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
    internal class LifeSpellBanish : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            if (saveGame.SpellEffects.BanishEvil(100))
            {
                Profile.Instance.MsgPrint("The power of your god banishes evil!");
            }
        }

        public override void Initialise(int characterClass)
        {
            Name = "Banish";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 35;
                    ManaCost = 55;
                    BaseFailure = 80;
                    FirstCastExperience = 115;
                    break;

                case CharacterClass.Priest:
                    Level = 25;
                    ManaCost = 25;
                    BaseFailure = 80;
                    FirstCastExperience = 250;
                    break;

                case CharacterClass.Paladin:
                    Level = 35;
                    ManaCost = 55;
                    BaseFailure = 80;
                    FirstCastExperience = 115;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 42;
                    ManaCost = 55;
                    BaseFailure = 80;
                    FirstCastExperience = 115;
                    break;

                case CharacterClass.HighMage:
                    Level = 33;
                    ManaCost = 44;
                    BaseFailure = 60;
                    FirstCastExperience = 115;
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
            return string.Empty;
        }
    }
}