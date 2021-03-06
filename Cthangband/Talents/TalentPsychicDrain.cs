// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Projection;
using System;

namespace Cthangband.Talents
{
    [Serializable]
    internal class TalentPsychicDrain : Talent
    {
        public override void Initialise(int characterClass)
        {
            Name = "Psychic Drain";
            Level = 25;
            ManaCost = 10;
            BaseFailure = 40;
        }

        public override void Use(Player player, Level level, SaveGame saveGame)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetDirectionWithAim(out int dir))
            {
                return;
            }
            int i = Program.Rng.DiceRoll(player.Level / 2, 6);
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