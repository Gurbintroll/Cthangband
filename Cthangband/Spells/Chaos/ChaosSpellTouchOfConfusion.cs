// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Chaos
{
    [Serializable]
    internal class ChaosSpellTouchOfConfusion : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            if (!player.HasConfusingTouch)
            {
                Profile.Instance.MsgPrint("Your hands start glowing.");
                player.HasConfusingTouch = true;
            }
        }

        public override void Initialise(int characterClass)
        {
            Name = "Touch of Confusion";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 5;
                    ManaCost = 5;
                    BaseFailure = 50;
                    FirstCastExperience = 1;
                    break;

                case CharacterClass.Priest:
                    Level = 5;
                    ManaCost = 4;
                    BaseFailure = 30;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Ranger:
                    Level = 7;
                    ManaCost = 5;
                    BaseFailure = 45;
                    FirstCastExperience = 2;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                    Level = 5;
                    ManaCost = 5;
                    BaseFailure = 30;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.Fanatic:
                    Level = 5;
                    ManaCost = 4;
                    BaseFailure = 30;
                    FirstCastExperience = 6;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Cultist:
                    Level = 4;
                    ManaCost = 2;
                    BaseFailure = 20;
                    FirstCastExperience = 1;
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