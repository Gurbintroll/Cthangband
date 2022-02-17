using Cthangband.Enumerations;
using Cthangband.Projection;
using System;

namespace Cthangband.Talents
{
    [Serializable]
    internal class TalentMindWave : Talent
    {
        public override void Initialise(int characterClass)
        {
            Name = "Mind Wave";
            Level = 18;
            ManaCost = 10;
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