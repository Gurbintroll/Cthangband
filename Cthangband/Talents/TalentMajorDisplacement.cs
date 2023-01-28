// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.PlayerClass.Base;
using Cthangband.Talents.Base;
using System;

namespace Cthangband.Talents
{
    [Serializable]
    internal class TalentMajorDisplacement : BaseTalent
    {
        public override void Initialise(IPlayerClass playerClass)
        {
            Name = "Major Displacement";
            Level = 7;
            VisCost = 6;
            BaseFailure = 35;
        }

        public override void Use(Player player, Level level, SaveGame saveGame)
        {
            SaveGame.Instance.SpellEffects.TeleportPlayer(player.Level * 5);
            if (player.Level > 29)
            {
                SaveGame.Instance.SpellEffects.BanishMonsters(player.Level);
            }
        }

        protected override string Comment(Player player)
        {
            return $"range {player.Level * 5}";
        }
    }
}