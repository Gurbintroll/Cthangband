// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Projection;
using System;

namespace Cthangband.Spells.Death
{
    [Serializable]
    internal class DeathSpellOrbOfEntropy : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetDirectionWithAim(out int dir))
            {
                return;
            }
            saveGame.SpellEffects.FireBall(new ProjectOldDrain(SaveGame.Instance.SpellEffects), dir,
                Program.Rng.DiceRoll(3, 6) + player.Level + (player.Level /
                (player.ProfessionIndex == CharacterClass.Mage || player.ProfessionIndex == CharacterClass.HighMage ? 2 : 4)),
                player.Level < 30 ? 2 : 3);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Orb of Entropy";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 12;
                    ManaCost = 12;
                    BaseFailure = 40;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.Priest:
                    Level = 14;
                    ManaCost = 14;
                    BaseFailure = 40;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.Rogue:
                    Level = 21;
                    ManaCost = 21;
                    BaseFailure = 60;
                    FirstCastExperience = 3;
                    break;

                case CharacterClass.Ranger:
                    Level = 24;
                    ManaCost = 24;
                    BaseFailure = 55;
                    FirstCastExperience = 3;
                    break;

                case CharacterClass.Paladin:
                    Level = 17;
                    ManaCost = 17;
                    BaseFailure = 40;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 14;
                    ManaCost = 14;
                    BaseFailure = 40;
                    FirstCastExperience = 5;
                    break;

                case CharacterClass.HighMage:
                    Level = 10;
                    ManaCost = 10;
                    BaseFailure = 30;
                    FirstCastExperience = 5;
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
            int s = player.Level + (player.Level /
                    (player.ProfessionIndex == CharacterClass.Mage || player.ProfessionIndex == CharacterClass.HighMage ? 2 : 4));
            return $"dam 3d6+{s}";
        }
    }
}