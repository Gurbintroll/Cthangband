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

namespace Cthangband.Spells.Nature
{
    [Serializable]
    internal class NatureSpellCallSunlight : BaseSpell
    {
        public override int DefaultBaseFailure => 90;

        public override int DefaultLevel => 37;

        public override int DefaultVisCost => 35;

        public override int FirstCastExperience => 100;

        public override string Name => "Call Sunlight";

        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.FireBall(new ProjectLight(SaveGame.Instance.SpellEffects), 0, 150, 8);
            level.WizLight();
            if (!player.Race.IsSunlightSensitive || player.HasLightResistance)
            {
                return;
            }
            Profile.Instance.MsgPrint("The sunlight scorches your flesh!");
            player.TakeHit(50, "sunlight");
        }

        protected override string Comment(Player player)
        {
            return "dam 150";
        }
    }
}