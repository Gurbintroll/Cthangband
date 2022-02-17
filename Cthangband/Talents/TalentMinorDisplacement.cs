using System;

namespace Cthangband.Talents
{
    [Serializable]
    internal class TalentMinorDisplacement : Talent
    {
        public override void Initialise(int characterClass)
        {
            Name = "Minor Displacement";
            Level = 3;
            ManaCost = 2;
            BaseFailure = 25;
        }

        public override void Use(Player player, Level level, SaveGame saveGame)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (player.Level < 25)
            {
                saveGame.SpellEffects.TeleportPlayer(10);
            }
            else
            {
                Profile.Instance.MsgPrint("Choose a destination.");
                if (!targetEngine.TgtPt(out int i, out int j))
                {
                    return;
                }
                player.Energy -= 60 - player.Level;
                if (!level.GridPassableNoCreature(j, i) || level.Grid[j][i].TileFlags.IsSet(GridTile.InVault) ||
                    level.Grid[j][i].FeatureType.Name != "Water" ||
                    level.Distance(j, i, player.MapY, player.MapX) > player.Level + 2 ||
                    Program.Rng.RandomLessThan(player.Level * player.Level / 2) == 0)
                {
                    Profile.Instance.MsgPrint("Something disrupts your concentration!");
                    player.Energy -= 100;
                    SaveGame.Instance.SpellEffects.TeleportPlayer(20);
                }
                else
                {
                    SaveGame.Instance.SpellEffects.TeleportPlayerTo(j, i);
                }
            }
        }

        protected override string Comment(Player player)
        {
            return $"range {(player.Level < 25 ? 10 : player.Level + 2)}";
        }
    }
}