// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
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
                if (!targetEngine.GetDirectionWithAim(out int dir))
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