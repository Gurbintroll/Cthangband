// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Projection;
using Cthangband.Talents.Base;
using System;

namespace Cthangband.Talents
{
    [Serializable]
    internal class TalentTelekineticWave : BaseTalent
    {
        public override void Initialise(int characterClass)
        {
            Name = "Telekinetic Wave";
            Level = 28;
            VrilCost = 20;
            BaseFailure = 45;
        }

        public override void Use(Player player, Level level, SaveGame saveGame)
        {
            Profile.Instance.MsgPrint("A wave of pure physical force radiates out from your body!");
            SaveGame.Instance.SpellEffects.Project(0, 3 + (player.Level / 10), player.MapY, player.MapX,
                player.Level * (player.Level > 39 ? 4 : 3), new ProjectTelekinesis(SaveGame.Instance.SpellEffects),
                ProjectionFlag.ProjectKill | ProjectionFlag.ProjectItem | ProjectionFlag.ProjectGrid);
        }

        protected override string Comment(Player player)
        {
            return $"dam {player.Level * (player.Level > 39 ? 4 : 3)}";
        }
    }
}