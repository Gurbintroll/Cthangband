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
    internal class DeathSpellDarkBolt : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            int beam;
            switch (player.ProfessionIndex)
            {
                case CharacterClass.Mage:
                    beam = player.Level;
                    break;

                case CharacterClass.HighMage:
                    beam = player.Level + 10;
                    break;

                default:
                    beam = player.Level / 2;
                    break;
            }
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetAimDir(out int dir))
            {
                return;
            }
            saveGame.SpellEffects.FireBoltOrBeam(beam, new ProjectDark(SaveGame.Instance.SpellEffects), dir,
                Program.Rng.DiceRoll(4 + ((player.Level - 5) / 4), 8));
        }

        public override void Initialise(int characterClass)
        {
            Name = "Dark Bolt";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 11;
                    ManaCost = 11;
                    BaseFailure = 30;
                    FirstCastExperience = 15;
                    break;

                case CharacterClass.Priest:
                    Level = 14;
                    ManaCost = 15;
                    BaseFailure = 30;
                    FirstCastExperience = 15;
                    break;

                case CharacterClass.Rogue:
                    Level = 28;
                    ManaCost = 28;
                    BaseFailure = 75;
                    FirstCastExperience = 25;
                    break;

                case CharacterClass.Ranger:
                    Level = 27;
                    ManaCost = 27;
                    BaseFailure = 40;
                    FirstCastExperience = 40;
                    break;

                case CharacterClass.Paladin:
                    Level = 18;
                    ManaCost = 20;
                    BaseFailure = 30;
                    FirstCastExperience = 15;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 14;
                    ManaCost = 18;
                    BaseFailure = 30;
                    FirstCastExperience = 15;
                    break;

                case CharacterClass.HighMage:
                    Level = 9;
                    ManaCost = 9;
                    BaseFailure = 30;
                    FirstCastExperience = 15;
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
            return $"dam {4 + ((player.Level - 5) / 4)}d8";
        }
    }
}