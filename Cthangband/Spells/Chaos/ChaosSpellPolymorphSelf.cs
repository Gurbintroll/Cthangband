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

namespace Cthangband.Spells.Chaos
{
    [Serializable]
    internal class ChaosSpellPolymorphSelf : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            player.PolymorphSelf();
        }

        public override void Initialise(int characterClass)
        {
            Name = "Polymorph Self";
            switch (characterClass)
            {
                case CharacterClassId.Mage:
                    Level = 42;
                    VisCost = 50;
                    BaseFailure = 85;
                    FirstCastExperience = 250;
                    break;

                case CharacterClassId.Priest:
                    Level = 45;
                    VisCost = 55;
                    BaseFailure = 95;
                    FirstCastExperience = 250;
                    break;

                case CharacterClassId.Ranger:
                    Level = 42;
                    VisCost = 75;
                    BaseFailure = 95;
                    FirstCastExperience = 250;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Monk:
                    Level = 45;
                    VisCost = 55;
                    BaseFailure = 85;
                    FirstCastExperience = 250;
                    break;

                case CharacterClassId.Fanatic:
                    Level = 42;
                    VisCost = 50;
                    BaseFailure = 85;
                    FirstCastExperience = 250;
                    break;

                case CharacterClassId.HighMage:
                case CharacterClassId.Cultist:
                    Level = 39;
                    VisCost = 40;
                    BaseFailure = 75;
                    FirstCastExperience = 250;
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
            return string.Empty;
        }
    }
}