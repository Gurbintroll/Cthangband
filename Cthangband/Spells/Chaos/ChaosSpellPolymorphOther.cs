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
    internal class ChaosSpellPolymorphOther : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetDirectionWithAim(out int dir))
            {
                return;
            }
            saveGame.SpellEffects.PolyMonster(dir);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Polymorph Other";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 11;
                    ManaCost = 7;
                    BaseFailure = 45;
                    FirstCastExperience = 9;
                    break;

                case CharacterClass.Priest:
                    Level = 14;
                    ManaCost = 11;
                    BaseFailure = 45;
                    FirstCastExperience = 9;
                    break;

                case CharacterClass.Ranger:
                    Level = 22;
                    ManaCost = 20;
                    BaseFailure = 60;
                    FirstCastExperience = 30;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                    Level = 12;
                    ManaCost = 12;
                    BaseFailure = 45;
                    FirstCastExperience = 9;
                    break;

                case CharacterClass.Fanatic:
                    Level = 11;
                    ManaCost = 11;
                    BaseFailure = 45;
                    FirstCastExperience = 9;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Cultist:
                    Level = 9;
                    ManaCost = 5;
                    BaseFailure = 35;
                    FirstCastExperience = 9;
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