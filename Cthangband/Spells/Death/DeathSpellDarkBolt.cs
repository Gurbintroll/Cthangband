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

namespace Cthangband.Spells.Death
{
    [Serializable]
    internal class DeathSpellDarkBolt : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            int beam;
            switch (player.CharacterClassIndex)
            {
                case CharacterClassId.Mage:
                    beam = player.Level;
                    break;

                case CharacterClassId.HighMage:
                    beam = player.Level + 10;
                    break;

                default:
                    beam = player.Level / 2;
                    break;
            }
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetDirectionWithAim(out int dir))
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
                case CharacterClassId.Mage:
                    Level = 11;
                    VisCost = 11;
                    BaseFailure = 30;
                    FirstCastExperience = 15;
                    break;

                case CharacterClassId.Priest:
                    Level = 14;
                    VisCost = 15;
                    BaseFailure = 30;
                    FirstCastExperience = 15;
                    break;

                case CharacterClassId.Rogue:
                    Level = 28;
                    VisCost = 28;
                    BaseFailure = 75;
                    FirstCastExperience = 25;
                    break;

                case CharacterClassId.Ranger:
                    Level = 27;
                    VisCost = 27;
                    BaseFailure = 40;
                    FirstCastExperience = 40;
                    break;

                case CharacterClassId.Paladin:
                    Level = 18;
                    VisCost = 20;
                    BaseFailure = 30;
                    FirstCastExperience = 15;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Cultist:
                    Level = 14;
                    VisCost = 18;
                    BaseFailure = 30;
                    FirstCastExperience = 15;
                    break;

                case CharacterClassId.HighMage:
                    Level = 9;
                    VisCost = 9;
                    BaseFailure = 30;
                    FirstCastExperience = 15;
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
            return $"dam {4 + ((player.Level - 5) / 4)}d8";
        }
    }
}