// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Tarot
{
    [Serializable]
    internal class TarotSpellSummonObject : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetDirectionWithAim(out int dir))
            {
                return;
            }
            saveGame.CommandEngine.SummonItem(dir, player.Level * 15, true);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Summon Object";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 20;
                    ManaCost = 20;
                    BaseFailure = 80;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.Priest:
                case CharacterClass.Monk:
                    Level = 22;
                    ManaCost = 22;
                    BaseFailure = 80;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.Rogue:
                    Level = 25;
                    ManaCost = 22;
                    BaseFailure = 80;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.Ranger:
                    Level = 24;
                    ManaCost = 22;
                    BaseFailure = 80;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 24;
                    ManaCost = 23;
                    BaseFailure = 80;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.HighMage:
                    Level = 16;
                    ManaCost = 16;
                    BaseFailure = 70;
                    FirstCastExperience = 8;
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
            return $"max wgt {player.Level * 15 / 10}";
        }
    }
}