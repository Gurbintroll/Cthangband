﻿// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Projection;
using Cthangband.Spells.Base;
using System;

namespace Cthangband.Spells.Chaos
{
    [Serializable]
    internal class ChaosSpellMagicMissile : BaseSpell
    {
        public override int DefaultBaseFailure => 20;

        public override int DefaultLevel => 1;

        public override int DefaultVisCost => 1;

        public override int FirstCastExperience => 4;

        public override string Name => "Magic Missile";

        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            var beam = player.PlayerClass.SpellBeamChance(player.Level);
            var targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetDirectionWithAim(out var dir))
            {
                return;
            }
            saveGame.SpellEffects.FireBoltOrBeam(beam - 10, new ProjectMissile(SaveGame.Instance.SpellEffects), dir,
                Program.Rng.DiceRoll(3 + ((player.Level - 1) / 5), 4));
        }

        protected override string Comment(Player player)
        {
            return $"dam {3 + ((player.Level - 1) / 5)}d4";
        }
    }
}