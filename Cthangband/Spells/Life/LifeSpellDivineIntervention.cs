// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Projection;
using Cthangband.Spells.Base;
using Cthangband.StaticData;
using System;

namespace Cthangband.Spells.Life
{
    [Serializable]
    internal class LifeSpellDivineIntervention : BaseSpell
    {
        public override int DefaultBaseFailure => 80;

        public override int DefaultLevel => 48;

        public override int DefaultVisCost => 50;

        public override int FirstCastExperience => 100;

        public override string Name => "Divine Intervention";

        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            saveGame.SpellEffects.Project(0, 1, player.MapY, player.MapX, 777, new ProjectHolyFire(SaveGame.Instance.SpellEffects),
                ProjectionFlag.ProjectKill);
            saveGame.SpellEffects.DispelMonsters(player.Level * 4);
            saveGame.SpellEffects.SlowMonsters();
            saveGame.SpellEffects.StunMonsters(player.Level * 4);
            saveGame.SpellEffects.ConfuseMonsters(player.Level * 4);
            saveGame.SpellEffects.TurnMonsters(player.Level * 4);
            saveGame.SpellEffects.StasisMonsters(player.Level * 4);
            level.Monsters.SummonSpecificFriendly(player.MapY, player.MapX, player.Level, Constants.SummonCthuloid, true);
            player.SetTimedSuperheroism(player.TimedSuperheroism + Program.Rng.DieRoll(25) + 25);
            player.RestoreHealth(300);
            if (player.TimedHaste == 0)
            {
                player.SetTimedHaste(Program.Rng.DieRoll(20 + player.Level) + player.Level);
            }
            else
            {
                player.SetTimedHaste(player.TimedHaste + Program.Rng.DieRoll(5));
            }
            player.SetTimedFear(0);
        }

        protected override string Comment(Player player)
        {
            return $"h300/d{player.Level * 4}+777";
        }
    }
}