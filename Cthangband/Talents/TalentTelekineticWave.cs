using Cthangband.Enumerations;
using Cthangband.Projection;
using System;

namespace Cthangband.Talents
{
    [Serializable]
    internal class TalentTelekineticWave : Talent
    {
        public override void Initialise(int characterClass)
        {
            Name = "Telekinetic Wave";
            Level = 28;
            ManaCost = 20;
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