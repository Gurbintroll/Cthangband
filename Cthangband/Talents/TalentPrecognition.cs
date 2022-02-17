using System;

namespace Cthangband.Talents
{
    [Serializable]
    internal class TalentPrecognition : Talent
    {
        public override void Initialise(int characterClass)
        {
            Name = "Precognition";
            Level = 1;
            ManaCost = 1;
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