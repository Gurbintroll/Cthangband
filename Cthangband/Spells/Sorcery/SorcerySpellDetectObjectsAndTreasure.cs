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
    internal class SorcerySpellDetectObjectsAndTreasure : BaseSpell
    {
        public override int DefaultBaseFailure => 25;

        public override int DefaultLevel => 5;

        public override int DefaultVisCost => 5;

        public override int FirstCastExperience => 15;

        public override string Name => "Detect Objects and Treasure";

        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.DetectObjectsNormal();
            saveGame.SpellEffects.DetectTreasure();
            saveGame.SpellEffects.DetectObjectsGold();
        }

        protected override string Comment(Player player)
        {
            return string.Empty;
        }
    }
}