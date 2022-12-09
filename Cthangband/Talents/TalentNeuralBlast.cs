// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Projection;
using Cthangband.Talents.Base;
using System;

namespace Cthangband.Talents
{
    [Serializable]
    internal class TalentNeuralBlast : BaseTalent
    {
        public override void Initialise(int characterClass)
        {
            Name = "Neural Blast";
            Level = 2;
            VrilCost = 1;
            BaseFailure = 20;
        }

        public override void Use(Player player, Level level, SaveGame saveGame)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetDirectionWithAim(out int dir))
            {
                return;
            }
            if (Program.Rng.DieRoll(100) < player.Level * 2)
            {
                saveGame.SpellEffects.FireBeam(new ProjectPsi(SaveGame.Instance.SpellEffects), dir,
                    Program.Rng.DiceRoll(3 + ((player.Level - 1) / 4), 3 + (player.Level / 15)));
            }
            else
            {
                saveGame.SpellEffects.FireBall(new ProjectPsi(SaveGame.Instance.SpellEffects), dir,
                    Program.Rng.DiceRoll(3 + ((player.Level - 1) / 4), 3 + (player.Level / 15)), 0);
            }
        }

        protected override string Comment(Player player)
        {
            return $"dam {3 + ((player.Level - 1) / 4)}d{3 + (player.Level / 15)}";
        }
    }
}