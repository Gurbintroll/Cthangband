﻿// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
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
    internal class TalentDomination : BaseTalent
    {
        public override void Initialise(IPlayerClass playerClass)
        {
            Name = "Domination";
            Level = 9;
            VisCost = 7;
            BaseFailure = 50;
        }

        public override void Use(Player player, Level level, SaveGame saveGame)
        {
            var targetEngine = new TargetEngine(player, level);
            if (player.Level < 30)
            {
                if (!targetEngine.GetDirectionWithAim(out var dir))
                {
                    return;
                }
                saveGame.SpellEffects.FireBall(new ProjectDomination(SaveGame.Instance.SpellEffects), dir, player.Level, 0);
            }
            else
            {
                saveGame.SpellEffects.CharmMonsters(player.Level * 2);
            }
        }

        protected override string Comment(Player player)
        {
            return string.Empty;
        }
    }
}