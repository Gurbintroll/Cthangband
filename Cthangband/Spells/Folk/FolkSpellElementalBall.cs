// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Projection;
using System;

namespace Cthangband.Spells.Folk
{
    [Serializable]
    internal class FolkSpellElementalBall : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            TargetEngine targetEngine = new TargetEngine(player, level);
            if (!targetEngine.GetDirectionWithAim(out int dir))
            {
                return;
            }
            Projectile dummy;
            switch (Program.Rng.DieRoll(4))
            {
                case 1:
                    dummy = new ProjectFire(SaveGame.Instance.SpellEffects);
                    break;

                case 2:
                    dummy = new ProjectElec(SaveGame.Instance.SpellEffects);
                    break;

                case 3:
                    dummy = new ProjectCold(SaveGame.Instance.SpellEffects);
                    break;

                default:
                    dummy = new ProjectAcid(SaveGame.Instance.SpellEffects);
                    break;
            }
            saveGame.SpellEffects.FireBall(dummy, dir, 75 + player.Level, 2);
        }

        public override void Initialise(int characterClass)
        {
            Name = "Teleport Away";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 41;
                    ManaCost = 30;
                    BaseFailure = 66;
                    FirstCastExperience = 30;
                    break;

                case CharacterClass.Priest:
                    Level = 44;
                    ManaCost = 39;
                    BaseFailure = 66;
                    FirstCastExperience = 30;
                    break;

                case CharacterClass.Rogue:
                    Level = 47;
                    ManaCost = 42;
                    BaseFailure = 66;
                    FirstCastExperience = 30;
                    break;

                case CharacterClass.Ranger:
                    Level = 47;
                    ManaCost = 42;
                    BaseFailure = 66;
                    FirstCastExperience = 30;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 45;
                    ManaCost = 44;
                    BaseFailure = 66;
                    FirstCastExperience = 30;
                    break;

                case CharacterClass.HighMage:
                    Level = 40;
                    ManaCost = 28;
                    BaseFailure = 55;
                    FirstCastExperience = 30;
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
            return $"dam {75 + player.Level}";
        }
    }
}