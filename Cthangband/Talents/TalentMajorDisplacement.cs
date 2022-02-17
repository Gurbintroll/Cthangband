using System;

namespace Cthangband.Talents
{
    [Serializable]
    internal class TalentMajorDisplacement : Talent
    {
        public override void Initialise(int characterClass)
        {
            Name = "Major Displacement";
            Level = 7;
            ManaCost = 6;
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