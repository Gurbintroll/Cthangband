﻿// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Spells.Base;
using System;

namespace Cthangband.Spells.Sorcery
{
    [Serializable]
    internal class SorcerySpellClairvoyance : BaseSpell
    {
        public override int DefaultBaseFailure => 80;

        public override int DefaultLevel => 30;

        public override int DefaultVisCost => 40;

        public override int FirstCastExperience => 120;

        public override string Name => "Clairvoyance";

        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            level.WizLight();
            if (!player.HasTelepathy)
            {
                player.SetTimedTelepathy(player.TimedTelepathy + Program.Rng.DieRoll(30) + 25);
            }
        }

        protected override string Comment(Player player)
        {
            return "dur 25+d30";
        }
    }
}