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

namespace Cthangband.Spells.Sorcery
{
    [Serializable]
    internal class SorcerySpellSlowMonster : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetDirectionWithAim(out int dir))
            {
                return;
            }
            saveGame.SpellEffects.SlowMonster(dir);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Slow Monster";
            switch (characterClass)
            {
                case CharacterClassId.Mage:
                    Level = 11;
                    VisCost = 7;
                    BaseFailure = 75;
                    FirstCastExperience = 7;
                    break;

                case CharacterClassId.Rogue:
                    Level = 29;
                    VisCost = 17;
                    BaseFailure = 75;
                    FirstCastExperience = 2;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Cultist:
                    Level = 12;
                    VisCost = 11;
                    BaseFailure = 75;
                    FirstCastExperience = 7;
                    break;

                case CharacterClassId.HighMage:
                    Level = 9;
                    VisCost = 5;
                    BaseFailure = 65;
                    FirstCastExperience = 7;
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