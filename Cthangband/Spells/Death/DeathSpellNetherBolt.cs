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
    internal class DeathSpellNetherBolt : BaseSpell
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
            saveGame.SpellEffects.FireBoltOrBeam(beam, new ProjectNether(SaveGame.Instance.SpellEffects), dir,
                Program.Rng.DiceRoll(6 + ((player.Level - 5) / 4), 8));
        }

        public override void Initialise(int characterClass)
        {
            Name = "Nether Bolt";
            switch (characterClass)
            {
                case CharacterClassId.Mage:
                    Level = 13;
                    VisCost = 12;
                    BaseFailure = 30;
                    FirstCastExperience = 4;
                    break;

                case CharacterClassId.Priest:
                    Level = 16;
                    VisCost = 16;
                    BaseFailure = 30;
                    FirstCastExperience = 4;
                    break;

                case CharacterClassId.Rogue:
                    Level = 23;
                    VisCost = 23;
                    BaseFailure = 75;
                    FirstCastExperience = 4;
                    break;

                case CharacterClassId.Ranger:
                    Level = 26;
                    VisCost = 26;
                    BaseFailure = 50;
                    FirstCastExperience = 3;
                    break;

                case CharacterClassId.Paladin:
                    Level = 19;
                    VisCost = 19;
                    BaseFailure = 30;
                    FirstCastExperience = 4;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Cultist:
                    Level = 16;
                    VisCost = 16;
                    BaseFailure = 30;
                    FirstCastExperience = 4;
                    break;

                case CharacterClassId.HighMage:
                    Level = 11;
                    VisCost = 10;
                    BaseFailure = 20;
                    FirstCastExperience = 5;
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
            return $"dam {6 + ((player.Level - 5) / 4)}d8";
        }
    }
}