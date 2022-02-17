using Cthangband.Enumerations;
using Cthangband.Projection;
using Cthangband.StaticData;
using System;

namespace Cthangband.Spells.Death
{
    [Serializable]
    internal class DeathSpellInvokeSpirits : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            int beam;
            switch (player.ProfessionIndex)
            {
                case CharacterClass.Mage:
                    beam = player.Level;
                    break;

                case CharacterClass.HighMage:
                    beam = player.Level + 10;
                    break;

                default:
                    beam = player.Level / 2;
                    break;
            }
            TargetEngine targetEngine = new TargetEngine(player, level);
            int die = Program.Rng.DieRoll(100) + (player.Level / 5);
            if (!targetEngine.GetAimDir(out int dir))
            {
                return;
            }
            Profile.Instance.MsgPrint("You call on the power of the dead...");
            if (die > 100)
            {
                Profile.Instance.MsgPrint("You feel a surge of eldritch force!");
            }
            if (die < 8)
            {
                Profile.Instance.MsgPrint("Oh no! Mouldering forms rise from the earth around you!");
                level.Monsters.SummonSpecific(player.MapY, player.MapX, SaveGame.Instance.Difficulty,
                    Constants.SummonUndead);
            }
            if (die < 14)
            {
                Profile.Instance.MsgPrint("An unnamable evil brushes against your mind...");
                player.SetTimedFear(player.TimedFear + Program.Rng.DieRoll(4) + 4);
            }
            if (die < 26)
            {
                Profile.Instance.MsgPrint("Your head is invaded by a horde of gibbering spectral voices...");
                player.SetTimedConfusion(player.TimedConfusion + Program.Rng.DieRoll(4) + 4);
            }
            if (die < 31)
            {
                saveGame.SpellEffects.PolyMonster(dir);
            }
            if (die < 36)
            {
                saveGame.SpellEffects.FireBoltOrBeam(beam - 10, new ProjectMissile(SaveGame.Instance.SpellEffects), dir,
                    Program.Rng.DiceRoll(3 + ((player.Level - 1) / 5), 4));
            }
            if (die < 41)
            {
                saveGame.SpellEffects.ConfuseMonster(dir, player.Level);
            }
            if (die < 46)
            {
                saveGame.SpellEffects.FireBall(new ProjectPois(SaveGame.Instance.SpellEffects), dir, 20 + (player.Level / 2), 3);
            }
            if (die < 51)
            {
                saveGame.SpellEffects.LightLine(dir);
            }
            if (die < 56)
            {
                saveGame.SpellEffects.FireBoltOrBeam(beam - 10, new ProjectElec(SaveGame.Instance.SpellEffects), dir,
                    Program.Rng.DiceRoll(3 + ((player.Level - 5) / 4), 8));
            }
            if (die < 61)
            {
                saveGame.SpellEffects.FireBoltOrBeam(beam - 10, new ProjectCold(SaveGame.Instance.SpellEffects), dir,
                    Program.Rng.DiceRoll(5 + ((player.Level - 5) / 4), 8));
            }
            if (die < 66)
            {
                saveGame.SpellEffects.FireBoltOrBeam(beam, new ProjectAcid(SaveGame.Instance.SpellEffects), dir,
                    Program.Rng.DiceRoll(6 + ((player.Level - 5) / 4), 8));
            }
            if (die < 71)
            {
                saveGame.SpellEffects.FireBoltOrBeam(beam, new ProjectFire(SaveGame.Instance.SpellEffects), dir,
                    Program.Rng.DiceRoll(8 + ((player.Level - 5) / 4), 8));
            }
            if (die < 76)
            {
                saveGame.SpellEffects.DrainLife(dir, 75);
            }
            if (die < 81)
            {
                saveGame.SpellEffects.FireBall(new ProjectElec(SaveGame.Instance.SpellEffects), dir, 30 + (player.Level / 2), 2);
            }
            if (die < 86)
            {
                saveGame.SpellEffects.FireBall(new ProjectAcid(SaveGame.Instance.SpellEffects), dir, 40 + player.Level, 2);
            }
            if (die < 91)
            {
                saveGame.SpellEffects.FireBall(new ProjectIce(SaveGame.Instance.SpellEffects), dir, 70 + player.Level, 3);
            }
            if (die < 96)
            {
                saveGame.SpellEffects.FireBall(new ProjectFire(SaveGame.Instance.SpellEffects), dir, 80 + player.Level, 3);
            }
            if (die < 101)
            {
                saveGame.SpellEffects.DrainLife(dir, 100 + player.Level);
            }
            if (die < 104)
            {
                saveGame.SpellEffects.Earthquake(player.MapY, player.MapX, 12);
            }
            if (die < 106)
            {
                saveGame.SpellEffects.DestroyArea(player.MapY, player.MapX, 15);
            }
            if (die < 108)
            {
                saveGame.SpellEffects.Carnage(true);
            }
            if (die < 110)
            {
                saveGame.SpellEffects.DispelMonsters(120);
            }
            saveGame.SpellEffects.DispelMonsters(150);
            saveGame.SpellEffects.SlowMonsters();
            saveGame.SpellEffects.SleepMonsters();
            player.RestoreHealth(300);
            if (die < 31)
            {
                Profile.Instance.MsgPrint("Sepulchral voices chuckle. 'Soon you will join us, mortal.'");
            }
        }

        public override void Initialise(int characterClass)
        {
            Name = "Invoke Spirits";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 10;
                    ManaCost = 15;
                    BaseFailure = 80;
                    FirstCastExperience = 30;
                    break;

                case CharacterClass.Priest:
                    Level = 13;
                    ManaCost = 15;
                    BaseFailure = 80;
                    FirstCastExperience = 30;
                    break;

                case CharacterClass.Rogue:
                    Level = 23;
                    ManaCost = 20;
                    BaseFailure = 40;
                    FirstCastExperience = 20;
                    break;

                case CharacterClass.Ranger:
                    Level = 25;
                    ManaCost = 25;
                    BaseFailure = 80;
                    FirstCastExperience = 100;
                    break;

                case CharacterClass.Paladin:
                    Level = 15;
                    ManaCost = 20;
                    BaseFailure = 80;
                    FirstCastExperience = 30;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 12;
                    ManaCost = 18;
                    BaseFailure = 80;
                    FirstCastExperience = 30;
                    break;

                case CharacterClass.HighMage:
                    Level = 8;
                    ManaCost = 10;
                    BaseFailure = 70;
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
            return "random";
        }
    }
}