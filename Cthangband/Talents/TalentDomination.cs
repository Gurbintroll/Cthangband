using Cthangband.Projection;
using System;

namespace Cthangband.Talents
{
    [Serializable]
    internal class TalentDomination : Talent
    {
        public override void Initialise(int characterClass)
        {
            Name = "Domination";
            Level = 9;
            ManaCost = 7;
            BaseFailure = 50;
        }

        public override void Use(Player player, Level level, SaveGame saveGame)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (player.Level < 30)
            {
                if (!targetEngine.GetAimDir(out int dir))
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