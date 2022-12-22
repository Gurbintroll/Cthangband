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

namespace Cthangband.Spells.Chaos
{
    [Serializable]
    internal class ChaosSpellFlameStrike : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.FireBall(new ProjectFire(SaveGame.Instance.SpellEffects), 0, 150 + (2 * player.Level), 8);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Flame Strike";
            switch (characterClass)
            {
                case CharacterClassId.Mage:
                    Level = 37;
                    VisCost = 34;
                    BaseFailure = 75;
                    FirstCastExperience = 40;
                    break;

                case CharacterClassId.Priest:
                    Level = 39;
                    VisCost = 37;
                    BaseFailure = 75;
                    FirstCastExperience = 50;
                    break;

                case CharacterClassId.Ranger:
                    Level = 42;
                    VisCost = 42;
                    BaseFailure = 75;
                    FirstCastExperience = 42;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Monk:
                    Level = 42;
                    VisCost = 40;
                    BaseFailure = 75;
                    FirstCastExperience = 40;
                    break;

                case CharacterClassId.Fanatic:
                    Level = 37;
                    VisCost = 37;
                    BaseFailure = 75;
                    FirstCastExperience = 40;
                    break;

                case CharacterClassId.HighMage:
                case CharacterClassId.Cultist:
                    Level = 34;
                    VisCost = 32;
                    BaseFailure = 65;
                    FirstCastExperience = 40;
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
            return $"dam {150 + (player.Level * 2)}";
        }
    }
}