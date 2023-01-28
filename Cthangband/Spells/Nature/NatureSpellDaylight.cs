// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Spells.Base;
using System;

namespace Cthangband.Spells.Nature
{
    [Serializable]
    internal class NatureSpellDaylight : BaseSpell
    {
        public override int DefaultBaseFailure => 50;

        public override int DefaultLevel => 4;

        public override int DefaultVisCost => 4;

        public override int FirstCastExperience => 5;

        public override string Name => "Daylight";

        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.LightArea(Program.Rng.DiceRoll(2, player.Level / 2), (player.Level / 10) + 1);
            if (!player.Race.IsSunlightSensitive || player.HasLightResistance)
            {
                return;
            }
            Profile.Instance.MsgPrint("The daylight scorches your flesh!");
            player.TakeHit(Program.Rng.DiceRoll(2, 2), "daylight");
        }

        protected override string Comment(Player player)
        {
            return $"dam 2d{player.Level / 2}";
        }
    }
}