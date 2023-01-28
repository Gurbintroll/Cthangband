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
    internal class TalentPrecognition : BaseTalent
    {
        public override void Initialise(IPlayerClass playerClass)
        {
            Name = "Precognition";
            Level = 1;
            VisCost = 1;
            BaseFailure = 15;
        }

        public override void Use(Player player, Level level, SaveGame saveGame)
        {
            if (player.Level > 44)
            {
                level.WizLight();
            }
            else if (player.Level > 19)
            {
                level.MapArea();
            }
            bool b;
            if (player.Level < 30)
            {
                b = saveGame.SpellEffects.DetectMonstersNormal();
                if (player.Level > 14)
                {
                    b |= saveGame.SpellEffects.DetectMonstersInvis();
                }
                if (player.Level > 4)
                {
                    b |= saveGame.SpellEffects.DetectTraps();
                }
            }
            else
            {
                b = saveGame.SpellEffects.DetectAll();
            }
            if (player.Level > 24 && player.Level < 40)
            {
                player.SetTimedTelepathy(player.TimedTelepathy + player.Level);
            }
            if (!b)
            {
                Profile.Instance.MsgPrint("You feel safe.");
            }
        }

        protected override string Comment(Player player)
        {
            return string.Empty;
        }
    }
}