﻿// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.PlayerClass.Base;
using Cthangband.Projection;
using Cthangband.Talents.Base;
using System;

namespace Cthangband.Talents
{
    [Serializable]
    internal class TalentMindWave : BaseTalent
    {
        public override void Initialise(IPlayerClass playerClass)
        {
            Name = "Mind Wave";
            Level = 18;
            VisCost = 10;
            BaseFailure = 45;
        }

        public override void Use(Player player, Level level, SaveGame saveGame)
        {
            Profile.Instance.MsgPrint("Mind-warping forces emanate from your brain!");
            if (player.Level < 25)
            {
                saveGame.SpellEffects.Project(0, 2 + (player.Level / 10), player.MapY, player.MapX, player.Level * 3 / 2,
                    new ProjectPsi(SaveGame.Instance.SpellEffects), ProjectionFlag.ProjectKill);
            }
            else
            {
                saveGame.SpellEffects.MindblastMonsters(player.Level * (((player.Level - 5) / 10) + 1));
            }
        }

        protected override string Comment(Player player)
        {
            return player.Level < 25 ? $"dam {player.Level * 3 / 2}" : $"dam {player.Level * (((player.Level - 5) / 10) + 1)}";
        }
    }
}