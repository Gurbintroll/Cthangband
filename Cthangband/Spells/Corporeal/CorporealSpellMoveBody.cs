// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using System;

namespace Cthangband.Spells.Corporeal
{
    [Serializable]
    internal class CorporealSpellMoveBody : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            Profile.Instance.MsgPrint("You focus your Chi. Choose a destination.");
            if (!targetEngine.TgtPt(out int ii, out int ij))
            {
                return;
            }
            player.Energy -= 60 - player.Level;
            if (!level.GridPassableNoCreature(ij, ii) || level.Grid[ij][ii].TileFlags.IsSet(GridTile.InVault) ||
                level.Distance(ij, ii, player.MapY, player.MapX) > player.Level + 2 ||
                Program.Rng.RandomLessThan(player.Level * player.Level / 2) == 0)
            {
                Profile.Instance.MsgPrint("You fail to concentrate correctly!");
                player.Energy -= 100;
                saveGame.SpellEffects.TeleportPlayer(10);
            }
            else
            {
                saveGame.SpellEffects.TeleportPlayerTo(ij, ii);
            }
        }

        public override void Initialise(int characterClass)
        {
            Name = "Move Body";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 12;
                    ManaCost = 12;
                    BaseFailure = 80;
                    FirstCastExperience = 40;
                    break;

                case CharacterClass.Priest:
                    Level = 15;
                    ManaCost = 15;
                    BaseFailure = 80;
                    FirstCastExperience = 40;
                    break;

                case CharacterClass.Ranger:
                    Level = 18;
                    ManaCost = 18;
                    BaseFailure = 80;
                    FirstCastExperience = 25;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Monk:
                case CharacterClass.Cultist:
                    Level = 15;
                    ManaCost = 12;
                    BaseFailure = 70;
                    FirstCastExperience = 30;
                    break;

                case CharacterClass.HighMage:
                    Level = 9;
                    ManaCost = 9;
                    BaseFailure = 70;
                    FirstCastExperience = 40;
                    break;

                default:
                    Level = 99;
                    ManaCost = 0;
                    BaseFailure = 0;
                    FirstCastExperience = 0;
                    break;
            }
        }

        protected override string Comment(Player player)
        {
            return $"range {player.Level + 2}";
        }
    }
}