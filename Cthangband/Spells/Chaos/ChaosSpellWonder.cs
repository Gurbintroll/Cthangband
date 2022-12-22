// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Projection;
using Cthangband.Spells.Base;
using System;

namespace Cthangband.Spells.Chaos
{
    [Serializable]
    internal class ChaosSpellWonder : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            int beam;
            switch (player.CharacterClassIndex)
            {
                case CharacterClassId.Mage:
                    beam = player.Level;
                    break;

                case CharacterClassId.HighMage:
                    beam = player.Level + 10;
                    break;

                default:
                    beam = player.Level / 2;
                    break;
            }
            TargetEngine targetEngine = new TargetEngine(player, level);
            int die = Program.Rng.DieRoll(100) + (player.Level / 5);
            if (!targetEngine.GetDirectionWithAim(out int dir))
            {
                return;
            }
            if (die > 100)
            {
                Profile.Instance.MsgPrint("You feel a surge of power!");
            }
            if (die < 8)
            {
                saveGame.SpellEffects.CloneMonster(dir);
            }
            else if (die < 14)
            {
                saveGame.SpellEffects.SpeedMonster(dir);
            }
            else if (die < 26)
            {
                saveGame.SpellEffects.HealMonster(dir);
            }
            else if (die < 31)
            {
                saveGame.SpellEffects.PolyMonster(dir);
            }
            else if (die < 36)
            {
                saveGame.SpellEffects.FireBoltOrBeam(beam - 10, new ProjectMissile(SaveGame.Instance.SpellEffects), dir,
                    Program.Rng.DiceRoll(3 + ((player.Level - 1) / 5), 4));
            }
            else if (die < 41)
            {
                saveGame.SpellEffects.ConfuseMonster(dir, player.Level);
            }
            else if (die < 46)
            {
                saveGame.SpellEffects.FireBall(new ProjectPoison(SaveGame.Instance.SpellEffects), dir, 20 + (player.Level / 2), 3);
            }
            else if (die < 51)
            {
                saveGame.SpellEffects.LightLine(dir);
            }
            else if (die < 56)
            {
                saveGame.SpellEffects.FireBoltOrBeam(beam - 10, new ProjectElectricity(SaveGame.Instance.SpellEffects), dir,
                    Program.Rng.DiceRoll(3 + ((player.Level - 5) / 4), 8));
            }
            else if (die < 61)
            {
                saveGame.SpellEffects.FireBoltOrBeam(beam - 10, new ProjectCold(SaveGame.Instance.SpellEffects), dir,
                    Program.Rng.DiceRoll(5 + ((player.Level - 5) / 4), 8));
            }
            else if (die < 66)
            {
                saveGame.SpellEffects.FireBoltOrBeam(beam, new ProjectAcid(SaveGame.Instance.SpellEffects), dir,
                    Program.Rng.DiceRoll(6 + ((player.Level - 5) / 4), 8));
            }
            else if (die < 71)
            {
                saveGame.SpellEffects.FireBoltOrBeam(beam, new ProjectFire(SaveGame.Instance.SpellEffects), dir,
                    Program.Rng.DiceRoll(8 + ((player.Level - 5) / 4), 8));
            }
            else if (die < 76)
            {
                saveGame.SpellEffects.DrainLife(dir, 75);
            }
            else if (die < 81)
            {
                saveGame.SpellEffects.FireBall(new ProjectElectricity(SaveGame.Instance.SpellEffects), dir, 30 + (player.Level / 2), 2);
            }
            else if (die < 86)
            {
                saveGame.SpellEffects.FireBall(new ProjectAcid(SaveGame.Instance.SpellEffects), dir, 40 + player.Level, 2);
            }
            else if (die < 91)
            {
                saveGame.SpellEffects.FireBall(new ProjectIce(SaveGame.Instance.SpellEffects), dir, 70 + player.Level, 3);
            }
            else if (die < 96)
            {
                saveGame.SpellEffects.FireBall(new ProjectFire(SaveGame.Instance.SpellEffects), dir, 80 + player.Level, 3);
            }
            else if (die < 101)
            {
                saveGame.SpellEffects.DrainLife(dir, 100 + player.Level);
            }
            else if (die < 104)
            {
                saveGame.SpellEffects.Earthquake(player.MapY, player.MapX, 12);
            }
            else if (die < 106)
            {
                saveGame.SpellEffects.DestroyArea(player.MapY, player.MapX, 15);
            }
            else if (die < 108)
            {
                saveGame.SpellEffects.Carnage(true);
            }
            else if (die < 110)
            {
                saveGame.SpellEffects.DispelMonsters(120);
            }
            else
            {
                saveGame.SpellEffects.DispelMonsters(150);
                saveGame.SpellEffects.SlowMonsters();
                saveGame.SpellEffects.SleepMonsters();
                player.RestoreHealth(300);
            }
        }

        public override void Initialise(int characterClass)
        {
            Name = "Wonder";
            switch (characterClass)
            {
                case CharacterClassId.Mage:
                    Level = 17;
                    VisCost = 10;
                    BaseFailure = 25;
                    FirstCastExperience = 5;
                    break;

                case CharacterClassId.Priest:
                    Level = 19;
                    VisCost = 15;
                    BaseFailure = 50;
                    FirstCastExperience = 7;
                    break;

                case CharacterClassId.Ranger:
                    Level = 27;
                    VisCost = 23;
                    BaseFailure = 60;
                    FirstCastExperience = 5;
                    break;

                case CharacterClassId.WarriorMage:
                case CharacterClassId.Monk:
                    Level = 21;
                    VisCost = 21;
                    BaseFailure = 45;
                    FirstCastExperience = 7;
                    break;

                case CharacterClassId.Fanatic:
                    Level = 19;
                    VisCost = 12;
                    BaseFailure = 45;
                    FirstCastExperience = 7;
                    break;

                case CharacterClassId.HighMage:
                case CharacterClassId.Cultist:
                    Level = 15;
                    VisCost = 9;
                    BaseFailure = 20;
                    FirstCastExperience = 5;
                    break;

                default:
                    Level = 99;
                    VisCost = 0;
                    BaseFailure = 0;
                    FirstCastExperience = 0;
                    break;
            }
        }

        protected override string Comment(Player player)
        {
            return "random";
        }
    }
}