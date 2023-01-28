// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
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
    internal class ChaosSpellChainLightning : BaseSpell
    {
        public override int DefaultBaseFailure => 80;

        public override int DefaultLevel => 15;

        public override int DefaultVisCost => 15;

        public override int FirstCastExperience => 35;

        public override string Name => "Chain Lightning";

        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            for (var dir = 0; dir <= 9; dir++)
            {
                saveGame.SpellEffects.FireBeam(new ProjectElectricity(SaveGame.Instance.SpellEffects), dir,
                    Program.Rng.DiceRoll(5 + (player.Level / 10), 8));
            }
        }

        protected override string Comment(Player player)
        {
            return $"dam {5 + (player.Level / 10)}d8";
        }
    }
}