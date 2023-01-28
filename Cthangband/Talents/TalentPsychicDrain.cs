// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.PlayerClass.Base;
using Cthangband.Projection;
using Cthangband.Talents.Base;
using System;

namespace Cthangband.Talents
{
    [Serializable]
    internal class TalentPsychicDrain : BaseTalent
    {
        public override void Initialise(IPlayerClass playerClass)
        {
            Name = "Psychic Drain";
            Level = 25;
            VisCost = 10;
            BaseFailure = 40;
        }

        public override void Use(Player player, Level level, SaveGame saveGame)
        {
            var targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetDirectionWithAim(out var dir))
            {
                return;
            }
            var i = Program.Rng.DiceRoll(player.Level / 2, 6);
            if (SaveGame.Instance.SpellEffects.FireBall(new ProjectPsiDrain(SaveGame.Instance.SpellEffects), dir, i, 0 + ((player.Level - 25) / 10)))
            {
                player.Energy -= Program.Rng.DieRoll(150);
            }
        }

        protected override string Comment(Player player)
        {
            return $"dam {player.Level / 2}d6";
        }
    }
}