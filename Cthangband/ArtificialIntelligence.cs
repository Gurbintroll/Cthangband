using Cthangband.Enumerations;
using Cthangband.Projection;
using Cthangband.StaticData;
using Cthangband.UI;
using System;

namespace Cthangband
{
    /// <summary>
    /// A class for generating monster behaviour
    /// </summary>
    internal class ArtificialIntelligence
    {
        private readonly Level _level;
        private readonly Player _player;
        private readonly SaveGame _saveGame = SaveGame.Instance;

        public ArtificialIntelligence(Player player, Level level)
        {
            _player = player;
            _level = level;
        }

        /// <summary>
        /// Process all the monsters on the level
        /// </summary>
        public void ProcessAllMonsters()
        {
            // The noise the player is making is based on their stealth score
            uint noise = 1u << (30 - _player.SkillStealth);
            // Go through all the monster slots on the level
            for (int i = _level.MMax - 1; i >= 1; i--)
            {
                Monster monster = _level.Monsters[i];
                // If the monster slot is empty, skip it
                if (monster.Race == null)
                {
                    continue;
                }
                // Keep count of how many are our friends
                if ((monster.Mind & Constants.SmFriendly) != 0)
                {
                    _saveGame.TotalFriends++;
                    _saveGame.TotalFriendLevels += monster.Race.Level;
                }
                // Monsters that have just been spawned don't act in their first turn
                if ((monster.IndividualMonsterFlags & Constants.MflagBorn) != 0)
                {
                    monster.IndividualMonsterFlags &= ~Constants.MflagBorn;
                    continue;
                }
                // Check the monster's speed to see if it should get a turn
                monster.Energy += GlobalData.ExtractEnergy[monster.Speed];
                if (monster.Energy < 100)
                {
                    continue;
                }
                // The monster is going to take a turn, even if it does nothing
                monster.Energy -= 100;
                // If the monster is too far away, don't even bother
                if (monster.DistanceFromPlayer >= 100)
                {
                    continue;
                }
                MonsterRace race = monster.Race;
                int monsterX = monster.MapX;
                int monsterY = monster.MapY;
                bool test = false;
                // Check to see if the monster notices the player
                // 1) We're in range
                if (monster.DistanceFromPlayer <= race.NoticeRange)
                {
                    test = true;
                }
                // 2) We're aggravating
                else if (monster.DistanceFromPlayer <= Constants.MaxSight && (_level.PlayerHasLosBold(monsterY, monsterX) || _player.HasAggravation))
                {
                    test = true;
                }
                // 3) We've left scent where the monster is so it can smell us
                else if (_level.Grid[_player.MapY][_player.MapX].ScentAge == _level.Grid[monsterY][monsterX].ScentAge &&
                         _level.Grid[monsterY][monsterX].ScentStrength < Constants.MonsterFlowDepth &&
                         _level.Grid[monsterY][monsterX].ScentStrength < race.NoticeRange)
                {
                    test = true;
                }
                // If it didn't notice us, skip to the next monster
                if (!test)
                {
                    continue;
                }
                _level.Monsters.CurrentlyActingMonster = i;
                // Process the individual monster
                ProcessMonster(i, noise);
                // If the monster killed the player or sent us to a new level, then stop processing
                if (!_saveGame.Playing || _player.IsDead || _saveGame.NewLevelFlag)
                {
                    break;
                }
            }
            _level.Monsters.CurrentlyActingMonster = 0;
        }

        /// <summary>
        /// Make a set of attacks on another monster
        /// </summary>
        /// <param name="monsterIndex"> The index of the monster making the attack </param>
        /// <param name="targetIndex"> The index of the target monster </param>
        /// <returns> True if the attack happened, false if not </returns>
        private bool AttackAnotherMonster(int monsterIndex, int targetIndex)
        {
            Monster monster = _level.Monsters[monsterIndex];
            Monster target = _level.Monsters[targetIndex];
            MonsterRace race = monster.Race;
            MonsterRace targetRace = target.Race;
            bool touched = false;
            int ySaver = target.MapY;
            int xSaver = target.MapX;
            // If we never attack then we shouldn't this time
            if ((race.Flags1 & MonsterFlag1.NeverAttack) != 0)
            {
                return false;
            }
            int armourClass = targetRace.ArmourClass;
            int monsterLevel = race.Level >= 1 ? race.Level : 1;
            string monsterName = monster.MonsterDesc(0);
            string targetName = target.MonsterDesc(0);
            monster.MonsterDesc(0x88);
            bool blinked = false;
            // If the player can't see either monster, they just hear noise
            if (!(monster.IsVisible || target.IsVisible))
            {
                Profile.Instance.MsgPrint("You hear noise.");
            }
            // We have up to four attacks
            for (int attackNumber = 0; attackNumber < 4; attackNumber++)
            {
                bool visible = false;
                bool obvious = false;
                int power = 0;
                int damage = 0;
                string act = null;
                AttackEffect effect = race.Attack[attackNumber].Effect;
                AttackType method = race.Attack[attackNumber].Method;
                int dDice = race.Attack[attackNumber].DDice;
                int dSide = race.Attack[attackNumber].DSide;
                // Can't attack ourselves
                if (target == monster)
                {
                    break;
                }
                // If the target has moved, abort
                if (target.MapX != xSaver || target.MapY != ySaver)
                {
                    break;
                }
                // If we don't have an attack in this attack slot, abort
                if (method == 0)
                {
                    break;
                }
                // If we blinked away after stealing on a previous attack, abort
                if (blinked)
                {
                    break;
                }
                if (monster.IsVisible)
                {
                    visible = true;
                }
                // Get the power of the attack based on the attack type
                switch (effect)
                {
                    case AttackEffect.Hurt:
                        power = 60;
                        break;

                    case AttackEffect.Poison:
                        power = 5;
                        break;

                    case AttackEffect.UnBonus:
                        power = 20;
                        break;

                    case AttackEffect.UnPower:
                        power = 15;
                        break;

                    case AttackEffect.EatGold:
                        power = 5;
                        break;

                    case AttackEffect.EatItem:
                        power = 5;
                        break;

                    case AttackEffect.EatFood:
                        power = 5;
                        break;

                    case AttackEffect.EatLight:
                        power = 5;
                        break;

                    case AttackEffect.Acid:
                        power = 0;
                        break;

                    case AttackEffect.Electricity:
                        power = 10;
                        break;

                    case AttackEffect.Fire:
                        power = 10;
                        break;

                    case AttackEffect.Cold:
                        power = 10;
                        break;

                    case AttackEffect.Blind:
                        power = 2;
                        break;

                    case AttackEffect.Confuse:
                        power = 10;
                        break;

                    case AttackEffect.Terrify:
                        power = 10;
                        break;

                    case AttackEffect.Paralyze:
                        power = 2;
                        break;

                    case AttackEffect.LoseStr:
                        power = 0;
                        break;

                    case AttackEffect.LoseDex:
                        power = 0;
                        break;

                    case AttackEffect.LoseCon:
                        power = 0;
                        break;

                    case AttackEffect.LoseInt:
                        power = 0;
                        break;

                    case AttackEffect.LoseWis:
                        power = 0;
                        break;

                    case AttackEffect.LoseCha:
                        power = 0;
                        break;

                    case AttackEffect.LoseAll:
                        power = 2;
                        break;

                    case AttackEffect.Shatter:
                        power = 60;
                        break;

                    case AttackEffect.Exp10:
                        power = 5;
                        break;

                    case AttackEffect.Exp20:
                        power = 5;
                        break;

                    case AttackEffect.Exp40:
                        power = 5;
                        break;

                    case AttackEffect.Exp80:
                        power = 5;
                        break;
                }
                // If we hit the monster, describe the type of hit
                if (effect == 0 || CheckHitMonsterVersusMonster(power, monsterLevel, armourClass))
                {
                    _saveGame.Disturb(true);
                    switch (method)
                    {
                        case AttackType.Hit:
                            act = "hits {0}.";
                            touched = true;
                            break;

                        case AttackType.Touch:
                            act = "touches {0}.";
                            touched = true;
                            break;

                        case AttackType.Punch:
                            act = "punches {0}.";
                            touched = true;
                            break;

                        case AttackType.Kick:
                            act = "kicks {0}.";
                            touched = true;
                            break;

                        case AttackType.Claw:
                            act = "claws {0}.";
                            touched = true;
                            break;

                        case AttackType.Bite:
                            act = "bites {0}.";
                            touched = true;
                            break;

                        case AttackType.Sting:
                            act = "stings {0}.";
                            touched = true;
                            break;

                        case AttackType.Butt:
                            act = "butts {0}.";
                            touched = true;
                            break;

                        case AttackType.Crush:
                            act = "crushes {0}.";
                            touched = true;
                            break;

                        case AttackType.Engulf:
                            act = "engulfs {0}.";
                            touched = true;
                            break;

                        case AttackType.Charge:
                            act = "charges {0}.";
                            touched = true;
                            break;

                        case AttackType.Crawl:
                            act = "crawls on {0}.";
                            touched = true;
                            break;

                        case AttackType.Drool:
                            act = "drools on {0}.";
                            touched = false;
                            break;

                        case AttackType.Spit:
                            act = "spits on {0}.";
                            touched = false;
                            break;

                        case AttackType.Gaze:
                            act = "gazes at {0}.";
                            touched = false;
                            break;

                        case AttackType.Wail:
                            act = "wails at {0}.";
                            touched = false;
                            break;

                        case AttackType.Spore:
                            act = "releases spores at {0}.";
                            touched = false;
                            break;

                        case AttackType.Worship:
                            act = "hero worships {0}.";
                            touched = false;
                            break;

                        case AttackType.Beg:
                            act = "begs {0} for money.";
                            touched = false;
                            target.SleepLevel = 0;
                            break;

                        case AttackType.Insult:
                            act = "insults {0}.";
                            touched = false;
                            target.SleepLevel = 0;
                            break;

                        case AttackType.Moan:
                            act = "moans at {0}.";
                            touched = false;
                            target.SleepLevel = 0;
                            break;

                        case AttackType.Show:
                            act = "sings to {0}.";
                            touched = false;
                            target.SleepLevel = 0;
                            break;
                    }
                    // Display the attack description
                    if (!string.IsNullOrEmpty(act))
                    {
                        string temp = string.Format(act, targetName);
                        if (monster.IsVisible || target.IsVisible)
                        {
                            Profile.Instance.MsgPrint($"{monsterName} {temp}");
                        }
                    }
                    obvious = true;
                    damage = Program.Rng.DiceRoll(dDice, dSide);
                    // Default to a missile attack
                    Projectile pt = new ProjectMissile(SaveGame.Instance.SpellEffects);
                    // Choose the correct type of attack to display, as well as any other special
                    // effects for the attack
                    switch (effect)
                    {
                        case 0:
                            damage = 0;
                            pt = null;
                            break;

                        case AttackEffect.Hurt:
                            damage -= damage * (armourClass < 150 ? armourClass : 150) / 250;
                            break;

                        case AttackEffect.Poison:
                            pt = new ProjectPois(SaveGame.Instance.SpellEffects);
                            break;

                        case AttackEffect.UnBonus:
                        case AttackEffect.UnPower:
                            pt = new ProjectDisenchant(SaveGame.Instance.SpellEffects);
                            break;

                        case AttackEffect.EatFood:
                        case AttackEffect.EatLight:
                            pt = null;
                            damage = 0;
                            break;

                        case AttackEffect.EatItem:
                        case AttackEffect.EatGold:
                            // Monsters don't actually steal from other monsters
                            pt = null;
                            damage = 0;
                            if (Program.Rng.DieRoll(2) == 1)
                            {
                                blinked = true;
                            }
                            break;

                        case AttackEffect.Acid:
                            pt = new ProjectAcid(SaveGame.Instance.SpellEffects);
                            break;

                        case AttackEffect.Electricity:
                            pt = new ProjectElec(SaveGame.Instance.SpellEffects);
                            break;

                        case AttackEffect.Fire:
                            pt = new ProjectFire(SaveGame.Instance.SpellEffects);
                            break;

                        case AttackEffect.Cold:
                            pt = new ProjectCold(SaveGame.Instance.SpellEffects);
                            break;

                        case AttackEffect.Blind:
                            break;

                        case AttackEffect.Confuse:
                            pt = new ProjectConfusion(SaveGame.Instance.SpellEffects);
                            break;

                        case AttackEffect.Terrify:
                            pt = new ProjectTurnAll(SaveGame.Instance.SpellEffects);
                            break;

                        case AttackEffect.Paralyze:
                            pt = new ProjectOldSleep(SaveGame.Instance.SpellEffects);
                            damage = race.Level;
                            break;

                        case AttackEffect.LoseStr:
                        case AttackEffect.LoseInt:
                        case AttackEffect.LoseWis:
                        case AttackEffect.LoseDex:
                        case AttackEffect.LoseCon:
                        case AttackEffect.LoseCha:
                        case AttackEffect.LoseAll:
                            break;

                        case AttackEffect.Shatter:
                            if (damage > 23)
                            {
                                _saveGame.SpellEffects.Earthquake(monster.MapY, monster.MapX, 8);
                            }
                            break;

                        case AttackEffect.Exp10:
                        case AttackEffect.Exp20:
                        case AttackEffect.Exp40:
                        case AttackEffect.Exp80:
                            pt = new ProjectNether(SaveGame.Instance.SpellEffects);
                            break;

                        default:
                            pt = null;
                            break;
                    }
                    // Implement the attack as a projectile
                    if (pt != null)
                    {
                        _saveGame.SpellEffects.Project(monsterIndex, 0, target.MapY, target.MapX, damage, pt,
                            ProjectionFlag.ProjectKill | ProjectionFlag.ProjectStop);
                        // If we touched the target we might get burned or zapped
                        if (touched)
                        {
                            if ((targetRace.Flags2 & MonsterFlag2.FireAura) != 0 && (race.Flags3 & MonsterFlag3.ImmuneFire) == 0)
                            {
                                if (monster.IsVisible || target.IsVisible)
                                {
                                    // Auras prevent us blinking away
                                    blinked = false;
                                    // The player may see and learn that the target has the aura
                                    Profile.Instance.MsgPrint($"{monsterName} is suddenly very hot!");
                                    if (target.IsVisible)
                                    {
                                        targetRace.Knowledge.RFlags2 |= MonsterFlag2.FireAura;
                                    }
                                }
                                _saveGame.SpellEffects.Project(targetIndex, 0, monster.MapY, monster.MapX,
                                    Program.Rng.DiceRoll(1 + (targetRace.Level / 26), 1 + (targetRace.Level / 17)),
                                    new ProjectFire(SaveGame.Instance.SpellEffects), ProjectionFlag.ProjectKill | ProjectionFlag.ProjectStop);
                            }
                            if ((targetRace.Flags2 & MonsterFlag2.LightningAura) != 0 && (race.Flags3 & MonsterFlag3.ImmuneLightning) == 0)
                            {
                                if (monster.IsVisible || target.IsVisible)
                                {
                                    // Auras prevent us blinking away
                                    blinked = false;
                                    // The player may see and learn that the target has the aura
                                    Profile.Instance.MsgPrint($"{monsterName} gets zapped!");
                                    if (target.IsVisible)
                                    {
                                        targetRace.Knowledge.RFlags2 |= MonsterFlag2.LightningAura;
                                    }
                                }
                                _saveGame.SpellEffects.Project(targetIndex, 0, monster.MapY, monster.MapX,
                                    Program.Rng.DiceRoll(1 + (targetRace.Level / 26), 1 + (targetRace.Level / 17)),
                                    new ProjectElec(SaveGame.Instance.SpellEffects), ProjectionFlag.ProjectKill | ProjectionFlag.ProjectStop);
                            }
                        }
                    }
                }
                else
                {
                    // We didn't hit, so just let the player know that if we are visible
                    switch (method)
                    {
                        case AttackType.Hit:
                        case AttackType.Touch:
                        case AttackType.Punch:
                        case AttackType.Kick:
                        case AttackType.Claw:
                        case AttackType.Bite:
                        case AttackType.Sting:
                        case AttackType.Butt:
                        case AttackType.Crush:
                        case AttackType.Engulf:
                        case AttackType.Charge:
                            if (monster.IsVisible)
                            {
                                _saveGame.Disturb(false);
                                Profile.Instance.MsgPrint($"{monsterName} misses {targetName}.");
                            }
                            break;
                    }
                }
                // If the player saw what happened, they know more abouyt what attacks we have
                if (visible)
                {
                    if (obvious || damage != 0 || race.Knowledge.RBlows[attackNumber] > 10)
                    {
                        if (race.Knowledge.RBlows[attackNumber] < Constants.MaxUchar)
                        {
                            race.Knowledge.RBlows[attackNumber]++;
                        }
                    }
                }
            }
            // If we stole and should therefore blink away, then do so.
            if (blinked)
            {
                Profile.Instance.MsgPrint(monster.IsVisible ? "The thief flees laughing!" : "You hear laughter!");
                _saveGame.SpellEffects.TeleportAway(monsterIndex, (Constants.MaxSight * 2) + 5);
            }
            // We made the attack
            return true;
        }

        /// <summary>
        /// Modify a monster's move away from the player based on where their scent is coming from
        /// </summary>
        /// <param name="monsterIndex"> The index of the monster </param>
        /// <param name="coord"> The location we're moving to </param>
        private void AvoidPlayersScent(int monsterIndex, MapCoordinate coord)
        {
            Monster monster = _level.Monsters[monsterIndex];
            MonsterRace race = monster.Race;
            int monsterY = monster.MapY;
            int monsterX = monster.MapX;
            int dY = monsterY - coord.Y;
            int dX = monsterX - coord.X;
            // If the scent too strong, keep going where we were going
            if (_level.Grid[monsterY][monsterX].ScentAge < _level.Grid[_player.MapY][_player.MapX].ScentAge)
            {
                return;
            }
            if (_level.Grid[monsterY][monsterX].ScentStrength > Constants.MonsterFlowDepth)
            {
                return;
            }
            if (_level.Grid[monsterY][monsterX].ScentStrength > race.NoticeRange)
            {
                return;
            }
            int gy = 0;
            int gx = 0;
            int when = 0;
            int score = -1;
            // Check each of the eight directions
            for (int i = 7; i >= 0; i--)
            {
                int y = monsterY + _level.OrderedDirectionYOffset[i];
                int x = monsterX + _level.OrderedDirectionXOffset[i];
                // If we have no scent there, or the scent is too recent, ignore it
                if (_level.Grid[y][x].ScentAge == 0)
                {
                    continue;
                }
                if (_level.Grid[y][x].ScentAge < when)
                {
                    continue;
                }
                // If the scent is weaker than in the other directions, go that way
                int dis = _level.Distance(y, x, dY, dX);
                int s = (5000 / (dis + 3)) - (500 / (_level.Grid[y][x].ScentStrength + 1));
                if (s < 0)
                {
                    s = 0;
                }
                if (s < score)
                {
                    continue;
                }
                when = _level.Grid[y][x].ScentAge;
                score = s;
                gy = y;
                gx = x;
            }
            // If we didn't find any scent at all, keep going the way we were going
            if (when == 0)
            {
                return;
            }
            // Go towards the weakest scent
            coord.Y = monsterY - gy;
            coord.X = monsterX - gx;
        }

        /// <summary>
        /// Use a breath weapon on another monster
        /// </summary>
        /// <param name="monsterIndex"> The monster doing the breathing </param>
        /// <param name="targetY"> The y coordinate of the target </param>
        /// <param name="targetX"> The x coordinate of the target </param>
        /// <param name="projectile"> The type of breath being used </param>
        /// <param name="damage"> The damage the breath will do </param>
        /// <param name="radius"> The radius of the attack, or zero for the default radius </param>
        private void BreatheAtMonster(int monsterIndex, int targetY, int targetX, Projectile projectile, int damage, int radius)
        {
            const ProjectionFlag projectionFlags = ProjectionFlag.ProjectGrid | ProjectionFlag.ProjectItem | ProjectionFlag.ProjectKill;
            Monster monster = _level.Monsters[monsterIndex];
            MonsterRace race = monster.Race;
            // Radius 0 means use the default radius
            if (radius < 1)
            {
                radius = (race.Flags2 & MonsterFlag2.Powerful) != 0 ? 3 : 2;
            }
            // Make the radius negative to indicate we need a cone instead of a ball
            radius = 0 - radius;
            _saveGame.SpellEffects.Project(monsterIndex, radius, targetY, targetX, damage, projectile, projectionFlags);
        }

        /// <summary>
        /// Breathe on the player
        /// </summary>
        /// <param name="monsterIndex"> The index of the monster doing the breathing </param>
        /// <param name="projectile"> The projectile that is being breathed </param>
        /// <param name="damage"> The damage done by the breath </param>
        /// <param name="radius">
        /// The (positive) radius of the breath weapon, or zero for the default radius
        /// </param>
        private void BreatheAtPlayer(int monsterIndex, Projectile projectile, int damage, int radius)
        {
            const ProjectionFlag projectionFlags = ProjectionFlag.ProjectGrid | ProjectionFlag.ProjectItem | ProjectionFlag.ProjectKill;
            Monster monster = _level.Monsters[monsterIndex];
            MonsterRace race = monster.Race;
            // Radius 0 means use the default radius
            if (radius < 1)
            {
                radius = (race.Flags2 & MonsterFlag2.Powerful) != 0 ? 3 : 2;
            }
            // Make the radius negative to indicate we need a cone instead of a ball
            radius = 0 - radius;
            _saveGame.SpellEffects.Project(monsterIndex, radius, _player.MapY, _player.MapX, damage, projectile, projectionFlags);
        }

        /// <summary>
        /// Check whether a monster hits another monster with an attack
        /// </summary>
        /// <param name="power"> The power of the attack type </param>
        /// <param name="level"> The level of the attacking monster </param>
        /// <param name="armourClass"> The armour class of the defending monster </param>
        /// <returns> True for a hit or false for a miss </returns>
        private bool CheckHitMonsterVersusMonster(int power, int level, int armourClass)
        {
            // Base 5% chance to hit and 5% chance to miss
            int k = Program.Rng.RandomLessThan(100);
            if (k < 10)
            {
                return k < 5;
            }
            // If we didn't auto hit or miss, use the standard formula for attacking
            int i = power + (level * 3);
            return i > 0 && Program.Rng.DieRoll(i) > armourClass * 3 / 4;
        }

        /// <summary>
        /// Chooses a spell for the monster to attack the player with
        /// </summary>
        /// <param name="monsterIndex"> The index of the monster casting the spell </param>
        /// <param name="spells">
        /// A list of the 'magic numbers' of the spells the monster can cast
        /// </param>
        /// <param name="listSize"> The number of spells in the spell list </param>
        /// <returns> The 'magic number' of the spell the monster will cast </returns>
        private int ChooseSpellAgainstPlayer(int monsterIndex, int[] spells, int listSize)
        {
            Monster monster = _level.Monsters[monsterIndex];
            MonsterRace race = monster.Race;
            // If the monster is stupid, cast a random spell
            if ((race.Flags2 & MonsterFlag2.Stupid) != 0)
            {
                return spells[Program.Rng.RandomLessThan(listSize)];
            }
            // Deposit the spells into a number of buckets based on what they are used for
            int[] escape = new int[96];
            int escapeNum = 0;
            int[] attack = new int[96];
            int attackNum = 0;
            int[] summon = new int[96];
            int summonNum = 0;
            int[] tactic = new int[96];
            int tacticNum = 0;
            int[] annoy = new int[96];
            int annoyNum = 0;
            int[] haste = new int[96];
            int hasteNum = 0;
            int[] heal = new int[96];
            int healNum = 0;
            for (int i = 0; i < listSize; i++)
            {
                if (SpellIsForEscape(spells[i]))
                {
                    escape[escapeNum++] = spells[i];
                }
                if (SpellIsForAttack(spells[i]))
                {
                    attack[attackNum++] = spells[i];
                }
                if (SpellIsForSummoning(spells[i]))
                {
                    summon[summonNum++] = spells[i];
                }
                if (SpellIsTactical(spells[i]))
                {
                    tactic[tacticNum++] = spells[i];
                }
                if (SpellIsForAnnoyance(spells[i]))
                {
                    annoy[annoyNum++] = spells[i];
                }
                if (SpellIsForHaste(spells[i]))
                {
                    haste[hasteNum++] = spells[i];
                }
                if (SpellIsForHealing(spells[i]))
                {
                    heal[healNum++] = spells[i];
                }
            }
            // Priority One: If we're afraid or hurt, always use a random escape spell if we have one
            if (monster.Health < monster.MaxHealth / 3 || monster.FearLevel != 0)
            {
                if (escapeNum != 0)
                {
                    return escape[Program.Rng.RandomLessThan(escapeNum)];
                }
            }
            // Priority Two: If we're hurt, always use a random healing spell if we have one
            if (monster.Health < monster.MaxHealth / 3)
            {
                if (healNum != 0)
                {
                    return heal[Program.Rng.RandomLessThan(healNum)];
                }
            }
            // Priority Three: If we're near the player and have no attack spells, probably use a
            // tactical spell
            if (_level.Distance(_player.MapY, _player.MapX, monster.MapY, monster.MapX) < 4 && attackNum != 0 &&
                Program.Rng.RandomLessThan(100) < 75)
            {
                if (tacticNum != 0)
                {
                    return tactic[Program.Rng.RandomLessThan(tacticNum)];
                }
            }
            // Priority Four: If we're at less than full health, probably use a healing spell
            if (monster.Health < monster.MaxHealth * 3 / 4 && Program.Rng.RandomLessThan(100) < 75)
            {
                if (healNum != 0)
                {
                    return heal[Program.Rng.RandomLessThan(healNum)];
                }
            }
            // Priority Five: If we have a summoning spell, maybe use it
            if (summonNum != 0 && Program.Rng.RandomLessThan(100) < 50)
            {
                return summon[Program.Rng.RandomLessThan(summonNum)];
            }
            // Priority Six: If we have a direct attack spell, probably use it
            if (attackNum != 0 && Program.Rng.RandomLessThan(100) < 85)
            {
                return attack[Program.Rng.RandomLessThan(attackNum)];
            }
            // Priority Seven: If we have a tactical spell, maybe use it
            if (tacticNum != 0 && Program.Rng.RandomLessThan(100) < 50)
            {
                return tactic[Program.Rng.RandomLessThan(tacticNum)];
            }
            // Priority Eight: If we have a haste spell, maybe use it
            if (hasteNum != 0 && Program.Rng.RandomLessThan(100) < 20 + race.Speed - monster.Speed)
            {
                return haste[Program.Rng.RandomLessThan(hasteNum)];
            }
            // Priority Nine: If we have an annoying spell, probably use it
            if (annoyNum != 0 && Program.Rng.RandomLessThan(100) < 85)
            {
                return annoy[Program.Rng.RandomLessThan(annoyNum)];
            }
            // Priority Ten: Give up on using a spell
            return 0;
        }

        /// <summary>
        /// Use the same algorithm that we use for missiles to see if we have a clean shot between
        /// two locations, but without actually shooting anything
        /// </summary>
        /// <param name="startY"> The start Y </param>
        /// <param name="startX"> The start X </param>
        /// <param name="targetY"> The Target Y </param>
        /// <param name="targetX"> The Target X </param>
        /// <returns> </returns>
        private bool CleanShot(int startY, int startX, int targetY, int targetX)
        {
            int y = startY;
            int x = startX;
            // Loop from the start to the maximumm range allowed
            for (int distance = 0; distance <= Constants.MaxRange; distance++)
            {
                // If our location is blocked, give up
                if (distance != 0 && !_level.GridPassable(y, x))
                {
                    break;
                }
                // If there's another monster in the way and it is friendly, give up
                if (distance != 0 && _level.Grid[y][x].Monster > 0)
                {
                    if ((_level.Monsters[_level.Grid[y][x].Monster].Mind & Constants.SmFriendly) == 0)
                    {
                        break;
                    }
                }
                // If we've reached the target then we have a clean shot
                if (x == targetX && y == targetY)
                {
                    return true;
                }
                // Move closer
                _level.MoveOneStepTowards(out y, out x, y, x, startY, startX, targetY, targetX);
            }
            // If we gave up or ran out of distance we don't have a clean shot
            return false;
        }

        /// <summary>
        /// Try to find a position where the player can't see you from which to ambush them
        /// </summary>
        /// <param name="monsterIndex"> The index of the monster that is trying to hide </param>
        /// <param name="relativeTarget">
        /// A map location, which will be amended to contain the hiding spot
        /// </param>
        /// <returns> True if a hiding spot was found, or false if it wasn't </returns>
        private bool FindAmbushSpot(int monsterIndex, MapCoordinate relativeTarget)
        {
            Monster monster = _level.Monsters[monsterIndex];
            int fy = monster.MapY;
            int fx = monster.MapX;
            int hidingSpotY = 0;
            int hidingSpotX = 0;
            int shortestDistance = 999;
            int tooCloseToPlayer = (_level.Distance(_player.MapY, _player.MapX, fy, fx) * 3 / 4) + 2;
            // Start with a short search radius and slowly increase
            for (int d = 1; d < 10; d++)
            {
                int y;
                for (y = fy - d; y <= fy + d; y++)
                {
                    int x;
                    for (x = fx - d; x <= fx + d; x++)
                    {
                        // Make sure the spot is valid
                        if (!_level.InBounds(y, x))
                        {
                            continue;
                        }
                        if (!_level.GridPassable(y, x))
                        {
                            continue;
                        }
                        // Make sure the spot is the right distance
                        if (_level.Distance(y, x, fy, fx) != d)
                        {
                            continue;
                        }
                        // Make sure the spot is actually hidden
                        if (!_level.PlayerCanSeeBold(y, x) && CleanShot(fy, fx, y, x))
                        {
                            // If the spot is closer to the player than any previously found spot
                            // (but not too close), remember it
                            int dis = _level.Distance(y, x, _player.MapY, _player.MapX);
                            if (dis < shortestDistance && dis >= tooCloseToPlayer)
                            {
                                hidingSpotY = y;
                                hidingSpotX = x;
                                shortestDistance = dis;
                            }
                        }
                    }
                }
                // If we found at least one spot, return true with the coordinates
                if (shortestDistance < 999)
                {
                    relativeTarget.Y = fy - hidingSpotY;
                    relativeTarget.X = fx - hidingSpotX;
                    return true;
                }
            }
            // We didn't find a spot
            return false;
        }

        /// <summary>
        /// Find a spot that as far from the player as possible an safe from attack
        /// </summary>
        /// <param name="monsterIndex"> The index of the monster trying to find safety </param>
        /// <param name="relativeTarget">
        /// A map location, which will be amended to contain the safe spot
        /// </param>
        /// <returns> True if a safe spot was found, or false if it wasn't </returns>
        private bool FindSafeSpot(int monsterIndex, MapCoordinate relativeTarget)
        {
            Monster monster = _level.Monsters[monsterIndex];
            int fy = monster.MapY;
            int fx = monster.MapX;
            int safeSpotY = 0;
            int safeSpotX = 0;
            int longestDistance = 0;
            // Start with a short search radius and slowly increase
            for (int d = 1; d < 10; d++)
            {
                int y;
                for (y = fy - d; y <= fy + d; y++)
                {
                    int x;
                    for (x = fx - d; x <= fx + d; x++)
                    {
                        // Make sure the spot is valid
                        if (!_level.InBounds(y, x))
                        {
                            continue;
                        }
                        if (!_level.GridPassable(y, x))
                        {
                            continue;
                        }
                        // Make sure the spot is the right distance
                        if (_level.Distance(y, x, fy, fx) != d)
                        {
                            continue;
                        }
                        // Reject spots that smell too strongly of the player
                        if (_level.Grid[y][x].ScentAge < _level.Grid[_player.MapY][_player.MapX].ScentAge)
                        {
                            continue;
                        }
                        if (_level.Grid[y][x].ScentStrength > _level.Grid[fy][fx].ScentStrength + (2 * d))
                        {
                            continue;
                        }
                        // Make sure the spot is actually hidden
                        if (!_level.Projectable(y, x, _player.MapY, _player.MapX))
                        {
                            // If the spot is further from the player than any previously found
                            // spot, remember it
                            int dis = _level.Distance(y, x, _player.MapY, _player.MapX);
                            if (dis > longestDistance)
                            {
                                safeSpotY = y;
                                safeSpotX = x;
                                longestDistance = dis;
                            }
                        }
                    }
                }
                // If we found a spot then save its coordinates and return true
                if (longestDistance > 0)
                {
                    relativeTarget.Y = fy - safeSpotY;
                    relativeTarget.X = fx - safeSpotX;
                    return true;
                }
            }
            // We found nowhere suitable
            return false;
        }

        /// <summary>
        /// Fire some kind of ball attack at the player
        /// </summary>
        /// <param name="monsterIndex"> The index of the monster firing the attack </param>
        /// <param name="projectile"> The type of effect the ball has </param>
        /// <param name="damage"> The damage done by the ball </param>
        /// <param name="radius"> The radius of the ball, or zero to use the default radius </param>
        private void FireBallAtPlayer(int monsterIndex, Projectile projectile, int damage, int radius)
        {
            const ProjectionFlag projectionFlag = ProjectionFlag.ProjectGrid | ProjectionFlag.ProjectItem | ProjectionFlag.ProjectKill;
            Monster monster = _level.Monsters[monsterIndex];
            MonsterRace race = monster.Race;
            if (radius < 1)
            {
                radius = (race.Flags2 & MonsterFlag2.Powerful) != 0 ? 3 : 2;
            }
            _saveGame.SpellEffects.Project(monsterIndex, radius, _player.MapY, _player.MapX, damage, projectile, projectionFlag);
        }

        /// <summary>
        /// Fire a bolt of some kind at another monster
        /// </summary>
        /// <param name="monsterIndex"> The index of the monster doing the firing </param>
        /// <param name="targetY"> The y coordinate of the target </param>
        /// <param name="targetX"> The x coordinate of the target </param>
        /// <param name="projectile"> The projectile to be fired </param>
        /// <param name="damage"> The damage the projectile should do </param>
        private void FireBoltAtMonster(int monsterIndex, int targetY, int targetX, Projectile projectile, int damage)
        {
            const ProjectionFlag projectionFlags = ProjectionFlag.ProjectStop | ProjectionFlag.ProjectKill;
            _saveGame.SpellEffects.Project(monsterIndex, 0, targetY, targetX, damage, projectile, projectionFlags);
        }

        /// <summary>
        /// Cast a bolt spell at the player
        /// </summary>
        /// <param name="monsterIndex"> The index of the monster casting the bolt </param>
        /// <param name="projectile"> The projectile being used for the bolt </param>
        /// <param name="damage"> The damage that the bolt will do </param>
        private void FireBoltAtPlayer(int monsterIndex, Projectile projectile, int damage)
        {
            const ProjectionFlag projectionFlags = ProjectionFlag.ProjectStop | ProjectionFlag.ProjectKill;
            _saveGame.SpellEffects.Project(monsterIndex, 0, _player.MapY, _player.MapX, damage, projectile, projectionFlags);
        }

        /// <summary>
        /// Get a list of potential moves towards an enemy monster if there is one
        /// </summary>
        /// <param name="monster"> The monster making the moves </param>
        /// <param name="movesList"> The list in which to insert the moves </param>
        private void GetMovesTowardsEnemyMonsters(Monster monster, PotentialMovesList movesList)
        {
            int[][] spiralGridOffsets =
            {
                new[] {-1, 0}, new[] {0, -1}, new[] {1, 0}, new[] {0, +1}, new[] {-1, -1}, new[] {1, -1}, new[] {1, 1},
                new[] {-1, 1}, new[] {-2, 0}, new[] {0, -2}, new[] {2, 0}, new[] {0, 2}, new[] {-2, -1}, new[] {1, -2},
                new[] {2, 1}, new[] {-1, 2}, new[] {-1, -2}, new[] {2, -1}, new[] {1, 2}, new[] {-2, 1}, new[] {-2, -2},
                new[] {2, -2}, new[] {2, 2}, new[] {-2, 2}, new[] {-3, 0}, new[] {0, -3}, new[] {3, 0}, new[] {0, 3},
                new[] {-3, -1}, new[] {1, -3}, new[] {3, 1}, new[] {-1, 3}, new[] {-3, 1}, new[] {-1, -3}, new[] {3, -1},
                new[] {1, 3}, new[] {-3, -2}, new[] {2, -3}, new[] {3, 2}, new[] {-2, 3}, new[] {-3, 2}, new[] {-2, -3},
                new[] {3, -2}, new[] {2, 3}, new[] {-3, -3}, new[] {3, -3}, new[] {3, 3}, new[] {-3, 3}, new[] {-4, 0},
                new[] {0, -4}, new[] {4, 0}, new[] {0, 4}, new[] {1, -4}, new[] {4, 1}, new[] {-1, 4}, new[] {-4, -1},
                new[] {-4, 1}, new[] {-1, -4}, new[] {4, -1}, new[] {1, 4}, new[] {-4, -2}, new[] {2, -4}, new[] {4, 2},
                new[] {-2, 4}, new[] {-4, 2}, new[] {-2, -4}, new[] {4, -2}, new[] {2, 4}, new[] {-4, -3}, new[] {3, -4},
                new[] {4, 3}, new[] {-3, 4}, new[] {-4, 3}, new[] {-3, -4}, new[] {4, -3}, new[] {3, 4}, new[] {-4, -4},
                new[] {4, -4}, new[] {4, 4}, new[] {-4, 4}
            };
            // Go through the possible places an enemy could be
            for (int i = 0; i < 80; i++)
            {
                // Get the location of the place (these are ordered so they start close and spiral out)
                int y = monster.MapY + spiralGridOffsets[i][0];
                int x = monster.MapX + spiralGridOffsets[i][1];
                // Check if we're in bounds and have a monster
                if (_level.InBounds(y, x))
                {
                    if (_level.Grid[y][x].Monster != 0)
                    {
                        Monster enemy = _level.Monsters[_level.Grid[y][x].Monster];
                        // Only go for monsters who are awake and on the opposing side
                        if ((enemy.Mind & Constants.SmFriendly) != (monster.Mind & Constants.SmFriendly) &&
                            enemy.SleepLevel == 0)
                        {
                            // Add moves directly towards and either side of the enemy based on its
                            // relative location
                            x -= monster.MapX;
                            y -= monster.MapY;
                            if (y < 0 && x == 0)
                            {
                                movesList[0] = 8;
                                movesList[1] = 7;
                                movesList[2] = 9;
                            }
                            else if (y > 0 && x == 0)
                            {
                                movesList[0] = 2;
                                movesList[1] = 1;
                                movesList[2] = 3;
                            }
                            else if (x > 0 && y == 0)
                            {
                                movesList[0] = 6;
                                movesList[1] = 9;
                                movesList[2] = 3;
                            }
                            else if (x < 0 && y == 0)
                            {
                                movesList[0] = 4;
                                movesList[1] = 7;
                                movesList[2] = 1;
                            }
                            if (y < 0 && x < 0)
                            {
                                movesList[0] = 7;
                                movesList[1] = 4;
                                movesList[2] = 8;
                            }
                            else if (y < 0 && x > 0)
                            {
                                movesList[0] = 9;
                                movesList[1] = 6;
                                movesList[2] = 8;
                            }
                            else if (y > 0 && x < 0)
                            {
                                movesList[0] = 1;
                                movesList[1] = 4;
                                movesList[2] = 2;
                            }
                            else if (y > 0 && x > 0)
                            {
                                movesList[0] = 3;
                                movesList[1] = 6;
                                movesList[2] = 2;
                            }
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Populate a list with possible moves towards the player. Note that we won't always move
        /// directly towards the player, based on factors such as if we're in a group in a room or
        /// if we're afraid of the player.
        /// </summary>
        /// <param name="monsterIndex"> the index of the monster who is moving </param>
        /// <param name="movesList"> The list to be populated with moves </param>
        /// <returns> True if we have potential moves or false if we don't </returns>
        private bool GetMovesTowardsPlayer(int monsterIndex, PotentialMovesList movesList)
        {
            Monster monster = _level.Monsters[monsterIndex];
            MonsterRace race = monster.Race;
            int moveVal = 0;
            bool done = false;
            // Default to moving towards the player's exact location
            MapCoordinate targetLocation = new MapCoordinate(_player.MapX, _player.MapY);
            // Adjust our target based on the player's scent if we can't move in a straight line to them
            TrackPlayerByScent(monsterIndex, targetLocation);
            // Get the relative move needed to reach our target location
            MapCoordinate desiredRelativeMovement = new MapCoordinate();
            desiredRelativeMovement.Y = monster.MapY - targetLocation.Y;
            desiredRelativeMovement.X = monster.MapX - targetLocation.X;
            if ((monster.Mind & Constants.SmFriendly) == 0)
            {
                // If we're a pack animal that can't go through walls
                if ((race.Flags1 & MonsterFlag1.Friends) != 0 && (race.Flags3 & MonsterFlag3.Animal) != 0 &&
                    (race.Flags2 & MonsterFlag2.PassWall) == 0 && (race.Flags2 & MonsterFlag2.KillWall) == 0)
                {
                    int room = 0;
                    // Check if the player is in a room by counting the room tiles around them
                    for (int i = 0; i < 8; i++)
                    {
                        if (_level.Grid[_player.MapY + _level.OrderedDirectionYOffset[i]][_player.MapX + _level.OrderedDirectionXOffset[i]].TileFlags
                            .IsSet(GridTile.InRoom))
                        {
                            room++;
                        }
                    }
                    // If the player isn't in a room and they're healthy, wait to ambush them rather
                    // than running headlong into the corridor after them and queueing up to get hit
                    if (room < 8 && _player.Health > _player.MaxHealth * 3 / 4)
                    {
                        if (FindAmbushSpot(monsterIndex, desiredRelativeMovement))
                        {
                            done = true;
                        }
                    }
                }
                // If we're not done and we have friends make our movement slightly depend on our
                // index so we spread out and don't block each other
                if (!done && (race.Flags1 & MonsterFlag1.Friends) != 0)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        targetLocation.Y = _player.MapY + _level.OrderedDirectionYOffset[(monsterIndex + i) & 7];
                        targetLocation.X = _player.MapX + _level.OrderedDirectionXOffset[(monsterIndex + i) & 7];
                        // We might have got a '5' meaning stay where we are, so replace that with
                        // moving towards the player
                        if (monster.MapY == targetLocation.Y && monster.MapX == targetLocation.X)
                        {
                            targetLocation.Y = _player.MapY;
                            targetLocation.X = _player.MapX;
                            break;
                        }
                        // Repeat till we get a direction we can move in
                        if (!_level.GridPassableNoCreature(targetLocation.Y, targetLocation.X))
                        {
                            continue;
                        }
                        break;
                    }
                    desiredRelativeMovement.Y = monster.MapY - targetLocation.Y;
                    desiredRelativeMovement.X = monster.MapX - targetLocation.X;
                    done = true;
                }
            }
            // If we're an ally then check if we should retreat
            if ((monster.Mind & Constants.SmFriendly) != 0)
            {
                if (MonsterShouldRetreat(monsterIndex))
                {
                    // If we should be scared, simply move the opposite way to the player
                    desiredRelativeMovement.Y = -desiredRelativeMovement.Y;
                    desiredRelativeMovement.X = -desiredRelativeMovement.X;
                }
            }
            else
            {
                // If we're not an ally then check if we should retreat
                if (!done && MonsterShouldRetreat(monsterIndex))
                {
                    // If we should retreat, then try to find a safe spot where the player can't
                    // shoot or see us
                    if (!FindSafeSpot(monsterIndex, desiredRelativeMovement))
                    {
                        // If we failed to find one, just back off
                        desiredRelativeMovement.Y = -desiredRelativeMovement.Y;
                        desiredRelativeMovement.X = -desiredRelativeMovement.X;
                    }
                    else
                    {
                        // We found a safe spot, so head there, but prioritise avoiding the player's scent
                        AvoidPlayersScent(monsterIndex, desiredRelativeMovement);
                    }
                }
            }
            // If our best move is to stand still, don't move
            if (desiredRelativeMovement.X == 0 && desiredRelativeMovement.Y == 0)
            {
                return false;
            }
            // Convert the target location to an actual direction
            int ax = Math.Abs(desiredRelativeMovement.X);
            int ay = Math.Abs(desiredRelativeMovement.Y);
            if (desiredRelativeMovement.Y < 0)
            {
                moveVal += 8;
            }
            if (desiredRelativeMovement.X > 0)
            {
                moveVal += 4;
            }
            if (ay > ax << 1)
            {
                moveVal++;
                moveVal++;
            }
            else if (ax > ay << 1)
            {
                moveVal++;
            }
            // Add some potential moves to the list based on the direction we've decided to go in
            switch (moveVal)
            {
                case 0:
                    movesList[0] = 9;
                    if (ay > ax)
                    {
                        movesList[1] = 8;
                        movesList[2] = 6;
                        movesList[3] = 7;
                        movesList[4] = 3;
                    }
                    else
                    {
                        movesList[1] = 6;
                        movesList[2] = 8;
                        movesList[3] = 3;
                        movesList[4] = 7;
                    }
                    break;

                case 1:
                case 9:
                    movesList[0] = 6;
                    if (desiredRelativeMovement.Y < 0)
                    {
                        movesList[1] = 3;
                        movesList[2] = 9;
                        movesList[3] = 2;
                        movesList[4] = 8;
                    }
                    else
                    {
                        movesList[1] = 9;
                        movesList[2] = 3;
                        movesList[3] = 8;
                        movesList[4] = 2;
                    }
                    break;

                case 2:
                case 6:
                    movesList[0] = 8;
                    if (desiredRelativeMovement.X < 0)
                    {
                        movesList[1] = 9;
                        movesList[2] = 7;
                        movesList[3] = 6;
                        movesList[4] = 4;
                    }
                    else
                    {
                        movesList[1] = 7;
                        movesList[2] = 9;
                        movesList[3] = 4;
                        movesList[4] = 6;
                    }
                    break;

                case 4:
                    movesList[0] = 7;
                    if (ay > ax)
                    {
                        movesList[1] = 8;
                        movesList[2] = 4;
                        movesList[3] = 9;
                        movesList[4] = 1;
                    }
                    else
                    {
                        movesList[1] = 4;
                        movesList[2] = 8;
                        movesList[3] = 1;
                        movesList[4] = 9;
                    }
                    break;

                case 5:
                case 13:
                    movesList[0] = 4;
                    if (desiredRelativeMovement.Y < 0)
                    {
                        movesList[1] = 1;
                        movesList[2] = 7;
                        movesList[3] = 2;
                        movesList[4] = 8;
                    }
                    else
                    {
                        movesList[1] = 7;
                        movesList[2] = 1;
                        movesList[3] = 8;
                        movesList[4] = 2;
                    }
                    break;

                case 8:
                    movesList[0] = 3;
                    if (ay > ax)
                    {
                        movesList[1] = 2;
                        movesList[2] = 6;
                        movesList[3] = 1;
                        movesList[4] = 9;
                    }
                    else
                    {
                        movesList[1] = 6;
                        movesList[2] = 2;
                        movesList[3] = 9;
                        movesList[4] = 1;
                    }
                    break;

                case 10:
                case 14:
                    movesList[0] = 2;
                    if (desiredRelativeMovement.X < 0)
                    {
                        movesList[1] = 3;
                        movesList[2] = 1;
                        movesList[3] = 6;
                        movesList[4] = 4;
                    }
                    else
                    {
                        movesList[1] = 1;
                        movesList[2] = 3;
                        movesList[3] = 4;
                        movesList[4] = 6;
                    }
                    break;

                case 12:
                    movesList[0] = 1;
                    if (ay > ax)
                    {
                        movesList[1] = 2;
                        movesList[2] = 4;
                        movesList[3] = 3;
                        movesList[4] = 7;
                    }
                    else
                    {
                        movesList[1] = 4;
                        movesList[2] = 2;
                        movesList[3] = 7;
                        movesList[4] = 3;
                    }
                    break;
            }
            return true;
        }

        /// <summary>
        /// Check if a monster will decide to run away from the player. Note that this doesn't
        /// decide whether the monster should become scared, only whether or not it will move away
        /// (if it is scared this is likely)
        /// </summary>
        /// <param name="monsterIndex"> The index of the monster </param>
        /// <returns> True if the monster should run away, false if not </returns>
        private bool MonsterShouldRetreat(int monsterIndex)
        {
            Monster monster = _level.Monsters[monsterIndex];
            MonsterRace race = monster.Race;
            // Don't move away if we're already too far away to see the player
            if (monster.DistanceFromPlayer > Constants.MaxSight + 5)
            {
                return false;
            }
            // If we're the player's friend then don't move away from them
            if ((monster.Mind & Constants.SmFriendly) != 0)
            {
                return false;
            }
            // If we're scared then definitely move away
            if (monster.FearLevel != 0)
            {
                return true;
            }
            // If we're very close, stay close for potential melee
            if (monster.DistanceFromPlayer <= 5)
            {
                return false;
            }
            int playerLevel = _player.Level;
            int monsterLevel = race.Level + (monsterIndex & 0x08) + 25;
            // If we're tougher than the player, don't move away
            if (monsterLevel > playerLevel + 4)
            {
                return false;
            }
            // If we're significantly weaker than the player, move away
            if (monsterLevel + 4 <= playerLevel)
            {
                return true;
            }
            // If we're significantly less healthy than the player, move away
            int playerHealth = _player.Health;
            int playerMaxHealth = _player.MaxHealth;
            int monsterHealth = monster.Health;
            int monsterMaxHealth = monster.MaxHealth;
            int playerHealthFactor = (playerLevel * playerMaxHealth) + (playerHealth << 2);
            int monsterHealthFactor = (monsterLevel * monsterMaxHealth) + (monsterHealth << 2);
            return playerHealthFactor * monsterMaxHealth > monsterHealthFactor * playerMaxHealth;
        }

        /// <summary>
        /// Have an individual monster take its turn
        /// </summary>
        /// <param name="monsterIndex"> The index of the monster </param>
        /// <param name="noise"> The amount of noise the player is making </param>
        private void ProcessMonster(int monsterIndex, uint noise)
        {
            Monster monster = _level.Monsters[monsterIndex];
            MonsterRace race = monster.Race;
            // Is the monster asleep?
            if (monster.SleepLevel != 0)
            {
                // if the player aggravates, notice them more
                uint notice = 0;
                if (!_player.HasAggravation)
                {
                    notice = (uint)Program.Rng.RandomLessThan(1024);
                }
                // If the player makes too much noise (or aggravates)
                if (notice * notice * notice <= noise)
                {
                    int wakeAmount = 1;
                    if (monster.DistanceFromPlayer < 50)
                    {
                        wakeAmount = 100 / monster.DistanceFromPlayer;
                    }
                    // Aggravate wakes the monster fully, if it notices at all
                    if (_player.HasAggravation)
                    {
                        wakeAmount = monster.SleepLevel;
                    }
                    // If we're not awake yet, just sleep less deeply
                    if (monster.SleepLevel > wakeAmount)
                    {
                        monster.SleepLevel -= wakeAmount;
                        // The player may notice
                        if (monster.IsVisible && race.Knowledge.RIgnore < Constants.MaxUchar)
                        {
                            race.Knowledge.RIgnore++;
                        }
                    }
                    else
                    {
                        // Wake us up
                        monster.SleepLevel = 0;
                        // If the player sees us wake up, let them know
                        if (monster.IsVisible)
                        {
                            string monsterName = monster.MonsterDesc(0);
                            Profile.Instance.MsgPrint($"{monsterName} wakes up.");
                            // And let the player notice how easily we wake
                            if (race.Knowledge.RWake < Constants.MaxUchar)
                            {
                                race.Knowledge.RWake++;
                            }
                        }
                    }
                }
                // If we're still asleep after all that, do nothing else for our turn
                if (monster.SleepLevel != 0)
                {
                    return;
                }
            }
            // If we're stunned, then reduce our stun level
            if (monster.StunLevel != 0)
            {
                int stunRelief = 1;
                // We have a level-based chance of shaking off the stun completely
                if (Program.Rng.RandomLessThan(5000) <= race.Level * race.Level)
                {
                    stunRelief = monster.StunLevel;
                }
                // Reduce our stun if the relief is not enough to get rid of it
                if (monster.StunLevel > stunRelief)
                {
                    monster.StunLevel -= stunRelief;
                }
                else
                {
                    // Get rid of all of our stun
                    monster.StunLevel = 0;
                    // If the player sees us, let them know we're no longer stunned
                    if (monster.IsVisible)
                    {
                        string monsterName = monster.MonsterDesc(0);
                        Profile.Instance.MsgPrint($"{monsterName} is no longer stunned.");
                    }
                }
                // If we are still stunned, don't take a turn
                if (monster.StunLevel != 0)
                {
                    return;
                }
            }
            // If we're confused
            if (monster.ConfusionLevel != 0)
            {
                // Reduce our confusion by an amount based on our level
                int confusionRelief = Program.Rng.DieRoll((race.Level / 10) + 1);
                if (monster.ConfusionLevel > confusionRelief)
                {
                    monster.ConfusionLevel -= confusionRelief;
                }
                else
                {
                    // If we're no longer confused, the player will see this
                    monster.ConfusionLevel = 0;
                    if (monster.IsVisible)
                    {
                        string monsterName = monster.MonsterDesc(0);
                        Profile.Instance.MsgPrint($"{monsterName} is no longer confused.");
                    }
                }
            }
            // If we're curently friendly and the player aggravates, then stop being friendly
            bool getsAngry = false;
            if ((monster.Mind & Constants.SmFriendly) != 0 && _player.HasAggravation)
            {
                getsAngry = true;
            }
            // If we're unique, don't stay friendly
            if ((monster.Mind & Constants.SmFriendly) != 0 && !_player.IsWizard && (race.Flags1 & MonsterFlag1.Unique) != 0)
            {
                getsAngry = true;
            }
            // If we got angry, let the player see that
            if (getsAngry)
            {
                string monsterName = monster.MonsterDesc(0);
                Profile.Instance.MsgPrint($"{monsterName} suddenly becomes hostile!");
                monster.Mind &= ~Constants.SmFriendly;
            }
            // Are we afraid?
            if (monster.FearLevel != 0)
            {
                // Reduce our fear by an amount based on our level
                int fearRelief = Program.Rng.DieRoll((race.Level / 10) + 1);
                if (monster.FearLevel > fearRelief)
                {
                    monster.FearLevel -= fearRelief;
                }
                else
                {
                    monster.FearLevel = 0;
                    // If the player can see us, they can see we're no longer afraid
                    if (monster.IsVisible)
                    {
                        string monsterName = monster.MonsterDesc(0);
                        string monsterPossessive = monster.MonsterDesc(0x22);
                        Profile.Instance.MsgPrint($"{monsterName} recovers {monsterPossessive} courage.");
                    }
                }
            }
            int oldY = monster.MapY;
            int oldX = monster.MapX;
            // If it's suitable for us to reproduce
            if ((race.Flags2 & MonsterFlag2.Multiply) != 0 && _level.Monsters.NumRepro < Constants.MaxRepro &&
                monster.Generation < 10)
            {
                // Find how many spaces we've got near us
                int k;
                int y;
                for (k = 0, y = oldY - 1; y <= oldY + 1; y++)
                {
                    for (int x = oldX - 1; x <= oldX + 1; x++)
                    {
                        if (_level.Grid[y][x].Monster != 0)
                        {
                            k++;
                        }
                    }
                }
                // If we're friendly, then our babies are friendly too
                bool isFriend = (monster.Mind & Constants.SmFriendly) != 0;
                // If there's lots of space, then pop out a baby
                if (k < 4 && (k == 0 || Program.Rng.RandomLessThan(k * Constants.MonMultAdj) == 0))
                {
                    if (_level.Monsters.MultiplyMonster(monsterIndex, isFriend, false))
                    {
                        // If the player saw this, they now know we can multiply
                        if (monster.IsVisible)
                        {
                            race.Knowledge.RFlags2 |= MonsterFlag2.Multiply;
                        }
                        // Having a baby takes our entire turn
                        return;
                    }
                }
            }
            // If we can usefully cast a spell against the player, then that's our turn
            if (TryCastingASpellAgainstPlayer(monsterIndex))
            {
                return;
            }
            // If we can usefully cast a spell against another monster, then that's our turn
            if (TryCastingASpellAgainstAnotherMonster(monsterIndex))
            {
                return;
            }
            // Initialise our possible moves
            PotentialMovesList potentialMoves = new PotentialMovesList();
            potentialMoves[0] = 0;
            potentialMoves[1] = 0;
            potentialMoves[2] = 0;
            potentialMoves[3] = 0;
            potentialMoves[4] = 0;
            potentialMoves[5] = 0;
            potentialMoves[6] = 0;
            potentialMoves[7] = 0;
            // If we're confused, have four attempts to move randomly
            if (monster.ConfusionLevel != 0)
            {
                potentialMoves[0] = 5;
                potentialMoves[1] = 5;
                potentialMoves[2] = 5;
                potentialMoves[3] = 5;
            }
            // If we move randomly most of the time, have a high chance of putting four random moves
            // in the matrix
            else if ((race.Flags1 & MonsterFlag1.RandomMove50) != 0 && (race.Flags1 & MonsterFlag1.RandomMove25) != 0 &&
                     Program.Rng.RandomLessThan(100) < 75)
            {
                // If the player sees us, then they learn about our random movement
                if (monster.IsVisible)
                {
                    race.Knowledge.RFlags1 |= MonsterFlag1.RandomMove50;
                    race.Knowledge.RFlags1 |= MonsterFlag1.RandomMove25;
                }
                potentialMoves[0] = 5;
                potentialMoves[1] = 5;
                potentialMoves[2] = 5;
                potentialMoves[3] = 5;
            }
            // If we have a moderate chance of moving randomly, maybe put four random moves in our matrix
            else if ((race.Flags1 & MonsterFlag1.RandomMove50) != 0 && Program.Rng.RandomLessThan(100) < 50)
            {
                // If the player sees us, then they learn about our random movement
                if (monster.IsVisible)
                {
                    race.Knowledge.RFlags1 |= MonsterFlag1.RandomMove50;
                }
                potentialMoves[0] = 5;
                potentialMoves[1] = 5;
                potentialMoves[2] = 5;
                potentialMoves[3] = 5;
            }
            // If we have a low chance of moving randomly, maybe put four random moves in our matrix
            else if ((race.Flags1 & MonsterFlag1.RandomMove25) != 0 && Program.Rng.RandomLessThan(100) < 25)
            {
                // If the player sees us, then they learn about our random movement
                if (monster.IsVisible)
                {
                    race.Knowledge.RFlags1 |= MonsterFlag1.RandomMove25;
                }
                potentialMoves[0] = 5;
                potentialMoves[1] = 5;
                potentialMoves[2] = 5;
                potentialMoves[3] = 5;
            }
            // If we're the player's friend and we're too far away, add sensible moves to our matrix
            else if ((monster.Mind & Constants.SmFriendly) != 0)
            {
                if (monster.DistanceFromPlayer > Constants.FollowDistance)
                {
                    GetMovesTowardsPlayer(monsterIndex, potentialMoves);
                }
                else
                {
                    // If we're close to the player (and friendly) just use random moves
                    potentialMoves[0] = 5;
                    potentialMoves[1] = 5;
                    potentialMoves[2] = 5;
                    potentialMoves[3] = 5;
                    // Possibly override these random moves with attacks on enemies
                    GetMovesTowardsEnemyMonsters(monster, potentialMoves);
                }
            }
            // If all the above fail, we must be a hostile monster who wants to move towards the player
            else
            {
                // If we fail to get sensible moves, give up on our turn
                if (!GetMovesTowardsPlayer(monsterIndex, potentialMoves))
                {
                    return;
                }
            }
            bool doTurn = false;
            bool doMove = false;
            bool doView = false;
            bool didOpenDoor = false;
            bool didBashDoor = false;
            bool didTakeItem = false;
            bool didKillItem = false;
            bool didMoveBody = false;
            bool didKillBody = false;
            bool didPassWall = false;
            bool didKillWall = false;
            // Go through our possible moves until we come to the limit of the ones we've had suggested
            for (int i = 0; potentialMoves[i] != 0; i++)
            {
                int d = potentialMoves[i];
                // Moves of '5' (i.e. 'stay still') are placeholders for random moves
                if (d == 5)
                {
                    d = _level.OrderedDirection[Program.Rng.RandomLessThan(8)];
                }
                // Work out where the move will take us
                int newY = oldY + _level.KeypadDirectionYOffset[d];
                int newX = oldX + _level.KeypadDirectionXOffset[d];
                GridTile tile = _level.Grid[newY][newX];
                Monster monsterInTargetTile = _level.Monsters[tile.Monster];
                // If we can simply move there, then we will do so
                if (_level.GridPassable(newY, newX))
                {
                    doMove = true;
                }
                // Bushes don't actually block us, so we can move there too
                else if (_level.Grid[newY][newX].FeatureType.Name == "Bush")
                {
                    doMove = true;
                }
                // We can always attack the player, even if the move would otherwse not be allowed
                else if (newY == _player.MapY && newX == _player.MapX)
                {
                    doMove = true;
                }
                // We can always attack another monster, even if the move would otherwise not be allowed
                else if (tile.Monster != -0)
                {
                    doMove = true;
                }
                // We can never go through permanent walls even if we can phase through or destroy
                // normal walls
                else if (tile.FeatureType.IsPermanent)
                {
                }
                // If we can phase through walls then they don't block us
                else if ((race.Flags2 & MonsterFlag2.PassWall) != 0)
                {
                    doMove = true;
                    didPassWall = true;
                }
                // If we can tunnel through walls then they don't block us
                else if ((race.Flags2 & MonsterFlag2.KillWall) != 0)
                {
                    doMove = true;
                    didKillWall = true;
                    // Occasionally make a noise if we're going to tunnel
                    if (Program.Rng.DieRoll(20) == 1)
                    {
                        Profile.Instance.MsgPrint("There is a grinding sound.");
                    }
                    // Remove the wall (and the player's memory of it) and remind ourselves to
                    // update the view if the player can see it
                    tile.TileFlags.Clear(GridTile.PlayerMemorised);
                    _level.CaveRemoveFeat(newY, newX);
                    if (_level.PlayerHasLosBold(newY, newX))
                    {
                        doView = true;
                    }
                }
                // If we're trying to get through a door
                else if (tile.FeatureType.IsClosedDoor)
                {
                    bool mayBash = true;
                    doTurn = true;
                    // If we can open the door then try to do so
                    if ((race.Flags2 & MonsterFlag2.OpenDoor) != 0)
                    {
                        // We can always open unlocked doors
                        if (tile.FeatureType.Name == "LockedDoor0" || tile.FeatureType.Name == "SecretDoor")
                        {
                            didOpenDoor = true;
                            mayBash = false;
                        }
                        // We have a chance to unlock locked doors
                        else if (tile.FeatureType.Name.Contains("Locked"))
                        {
                            int k = int.Parse(tile.FeatureType.Name.Substring(10));
                            if (Program.Rng.RandomLessThan(monster.Health / 10) > k)
                            {
                                _level.CaveSetFeat(newY, newX, "LockedDoor0");
                                mayBash = false;
                            }
                        }
                    }
                    // If we can't open doors (or failed to unlock the door), then we can bash it down
                    if (mayBash && (race.Flags2 & MonsterFlag2.BashDoor) != 0)
                    {
                        int k = int.Parse(tile.FeatureType.Name.Substring(10));
                        // If we succeeded, let the player hear it
                        if (Program.Rng.RandomLessThan(monster.Health / 10) > k)
                        {
                            Profile.Instance.MsgPrint("You hear a door burst open!");
                            didBashDoor = true;
                            doMove = true;
                        }
                    }
                    // If we opened it or bashed it, replace the closed door with the relevant open
                    // or broken one
                    if (didOpenDoor || didBashDoor)
                    {
                        if (didBashDoor && Program.Rng.RandomLessThan(100) < 50)
                        {
                            _level.CaveSetFeat(newY, newX, "BrokenDoor");
                        }
                        else
                        {
                            _level.CaveSetFeat(newY, newX, "OpenDoor");
                        }
                        // If the player can see, remind ourselves to update the view later
                        if (_level.PlayerHasLosBold(newY, newX))
                        {
                            doView = true;
                        }
                    }
                }
                // If we're going to move onto an Elder Sign and we're capable of doing attacks
                if (doMove && tile.FeatureType.Name == "ElderSign" && (race.Flags1 & MonsterFlag1.NeverAttack) == 0)
                {
                    // Assume we're not moving
                    doMove = false;
                    // We have a chance of breaking the sign based on our level
                    if (Program.Rng.DieRoll(Constants.BreakElderSign) < race.Level)
                    {
                        // If the player knows the sign is there, let them know it was broken
                        if (tile.TileFlags.IsSet(GridTile.PlayerMemorised))
                        {
                            Profile.Instance.MsgPrint("The Elder Sign is broken!");
                        }
                        tile.TileFlags.Clear(GridTile.PlayerMemorised);
                        _level.CaveRemoveFeat(newY, newX);
                        // Breaking the sign means we can move after all
                        doMove = true;
                    }
                }
                // If we're going to move onto a Yellow Sign and we can attack
                else if (doMove && tile.FeatureType.Name == "YellowSign" &&
                         (race.Flags1 & MonsterFlag1.NeverAttack) == 0)
                {
                    // Assume we're not moving
                    doMove = false;
                    // We have a chance to break the sign
                    if (Program.Rng.DieRoll(Constants.BreakYellowSign) < race.Level)
                    {
                        // If the player knows about the sign, let them know it was broken
                        if (tile.TileFlags.IsSet(GridTile.PlayerMemorised))
                        {
                            // If the player was on the sign, hurt them
                            if (newY == _player.MapY && newX == _player.MapX)
                            {
                                Profile.Instance.MsgPrint("The rune explodes!");
                                _saveGame.SpellEffects.FireBall(new ProjectMana(SaveGame.Instance.SpellEffects), 0,
                                    2 * ((_player.Level / 2) + Program.Rng.DiceRoll(7, 7)), 2);
                            }
                            else
                            {
                                Profile.Instance.MsgPrint("An Yellow Sign was disarmed.");
                            }
                        }
                        tile.TileFlags.Clear(GridTile.PlayerMemorised);
                        _level.CaveRemoveFeat(newY, newX);
                        // We can do the move after all
                        doMove = true;
                    }
                }
                // If we're going to attack the player, but our race never attacks, then cancel the move
                if (doMove && newY == _player.MapY && newX == _player.MapX && (race.Flags1 & MonsterFlag1.NeverAttack) != 0)
                {
                    doMove = false;
                }
                // If we're trying to move onto the player, then attack them instead
                if (doMove && newY == _player.MapY && newX == _player.MapX)
                {
                    _saveGame.CombatEngine.MonsterAttackPlayer(monsterIndex);
                    doMove = false;
                    doTurn = true;
                }
                // If we're trying to move onto another monster
                if (doMove && tile.Monster != 0)
                {
                    MonsterRace targetMonsterRace = monsterInTargetTile.Race;
                    // Assume for the moment we're not doing the move
                    doMove = false;
                    // If we can trample other monsters on our team and we're tougher than the one
                    // that's in our way...
                    if ((race.Flags2 & MonsterFlag2.KillBody) != 0 && race.Mexp > targetMonsterRace.Mexp &&
                        _level.GridPassable(newY, newX) &&
                        !((monster.Mind & Constants.SmFriendly) != 0 && (monsterInTargetTile.Mind & Constants.SmFriendly) != 0))
                    {
                        // Remove the other monster and replace it
                        doMove = true;
                        didKillBody = true;
                        _level.DeleteMonster(newY, newX);
                        monsterInTargetTile = _level.Monsters[tile.Monster];
                    }
                    // If we're not on the same team as the other monster or we're confused
                    else if ((monster.Mind & Constants.SmFriendly) != (monsterInTargetTile.Mind & Constants.SmFriendly) || monster.ConfusionLevel != 0)
                    {
                        doMove = false;
                        // Attack the monster in the target tile
                        if (monsterInTargetTile.Race != null && monsterInTargetTile.Health >= 0)
                        {
                            if (AttackAnotherMonster(monsterIndex, tile.Monster))
                            {
                                return;
                            }
                        }
                    }
                    // If the other monster is on our team and we can't trample it, maybe we can
                    // push past
                    else if ((race.Flags2 & MonsterFlag2.MoveBody) != 0 && race.Mexp > targetMonsterRace.Mexp &&
                             _level.GridPassable(newY, newX) && _level.GridPassable(monster.MapY, monster.MapX))
                    {
                        doMove = true;
                        didMoveBody = true;
                    }
                }
                // If we're going to do a move but we can't move, cancel it
                if (doMove && (race.Flags1 & MonsterFlag1.NeverMove) != 0)
                {
                    doMove = false;
                }
                // If we're going to do a move
                if (doMove)
                {
                    int nextItemIndex;
                    doTurn = true;
                    // Swap positions with the monster that is in the tile we're aiming for
                    _level.Grid[oldY][oldX].Monster = tile.Monster;
                    // If it was actually a monster then update it accordingly
                    if (tile.Monster != 0)
                    {
                        monsterInTargetTile.MapY = oldY;
                        monsterInTargetTile.MapX = oldX;
                        _level.Monsters.UpdateMonsterVisibility(tile.Monster, true);
                        // Pushing past something wakes it up
                        _level.Monsters[tile.Monster].SleepLevel = 0;
                    }
                    // Update our position
                    tile.Monster = monsterIndex;
                    monster.MapY = newY;
                    monster.MapX = newX;
                    _level.Monsters.UpdateMonsterVisibility(monsterIndex, true);
                    _level.RedrawSingleLocation(oldY, oldX);
                    _level.RedrawSingleLocation(newY, newX);
                    // If we are hostile and the player saw us move, then disturb them
                    if (monster.IsVisible && (monster.IndividualMonsterFlags & Constants.MflagView) != 0)
                    {
                        if ((monster.Mind & Constants.SmFriendly) == 0)
                        {
                            _saveGame.Disturb(false);
                        }
                    }
                    // Check through the items in the tile we just entered
                    for (int thisItemIndex = tile.Item; thisItemIndex != 0; thisItemIndex = nextItemIndex)
                    {
                        Item item = _level.Items[thisItemIndex];
                        nextItemIndex = item.NextInStack;
                        // We ignore gold
                        if (item.Category == ItemCategory.Gold)
                        {
                            continue;
                        }
                        // If we pick up or stomp on items, check the item type
                        if (((race.Flags2 & MonsterFlag2.TakeItem) != 0 ||
                             (race.Flags2 & MonsterFlag2.KillItem) != 0) && (monster.Mind & Constants.SmFriendly) == 0)
                        {
                            FlagSet f1 = new FlagSet();
                            FlagSet f2 = new FlagSet();
                            FlagSet f3 = new FlagSet();
                            uint flg3 = 0;
                            item.GetMergedFlags(f1, f2, f3);
                            string itemName = item.Description(true, 3);
                            string monsterName = monster.MonsterDesc(0x04);
                            if (f1.IsSet(ItemFlag1.KillDragon))
                            {
                                flg3 |= MonsterFlag3.Dragon;
                            }
                            if (f1.IsSet(ItemFlag1.SlayDragon))
                            {
                                flg3 |= MonsterFlag3.Dragon;
                            }
                            if (f1.IsSet(ItemFlag1.SlayTroll))
                            {
                                flg3 |= MonsterFlag3.Troll;
                            }
                            if (f1.IsSet(ItemFlag1.SlayGiant))
                            {
                                flg3 |= MonsterFlag3.Giant;
                            }
                            if (f1.IsSet(ItemFlag1.SlayOrc))
                            {
                                flg3 |= MonsterFlag3.Orc;
                            }
                            if (f1.IsSet(ItemFlag1.SlayDemon))
                            {
                                flg3 |= MonsterFlag3.Demon;
                            }
                            if (f1.IsSet(ItemFlag1.SlayUndead))
                            {
                                flg3 |= MonsterFlag3.Undead;
                            }
                            if (f1.IsSet(ItemFlag1.SlayAnimal))
                            {
                                flg3 |= MonsterFlag3.Animal;
                            }
                            if (f1.IsSet(ItemFlag1.SlayEvil))
                            {
                                flg3 |= MonsterFlag3.Evil;
                            }
                            // Monsters won't pick up artifacts or items that hurt them
                            if (item.IsFixedArtifact() || (race.Flags3 & flg3) != 0 ||
                                !string.IsNullOrEmpty(item.RandartName))
                            {
                                if ((race.Flags2 & MonsterFlag2.TakeItem) != 0)
                                {
                                    didTakeItem = true;
                                    if (monster.IsVisible && _level.PlayerHasLosBold(newY, newX))
                                    {
                                        Profile.Instance.MsgPrint($"{monsterName} tries to pick up {itemName}, but fails.");
                                    }
                                }
                            }
                            // If we picked up the item and the player saw, let them know
                            else if ((race.Flags2 & MonsterFlag2.TakeItem) != 0)
                            {
                                didTakeItem = true;
                                if (_level.PlayerHasLosBold(newY, newX))
                                {
                                    Profile.Instance.MsgPrint($"{monsterName} picks up {itemName}.");
                                }
                                // And pick up the actual item
                                _saveGame.Level.ExciseObjectIdx(thisItemIndex);
                                item.Marked = false;
                                item.Y = 0;
                                item.X = 0;
                                item.HoldingMonsterIndex = monsterIndex;
                                item.NextInStack = monster.FirstHeldItemIndex;
                                monster.FirstHeldItemIndex = thisItemIndex;
                            }
                            else
                            {
                                // We can't pick up the item, so just stomp on it
                                didKillItem = true;
                                // If the player saw us, let them know
                                if (_level.PlayerHasLosBold(newY, newX))
                                {
                                    Profile.Instance.MsgPrint($"{monsterName} crushes {itemName}.");
                                }
                                _saveGame.Level.DeleteObjectIdx(thisItemIndex);
                            }
                        }
                    }
                }
                // If we did something, then don't try another potential move
                if (doTurn)
                {
                    break;
                }
            }
            // If all our moves failed, have another go at casting a spell at the player
            if (!doTurn && !doMove && monster.FearLevel == 0 && (monster.Mind & Constants.SmFriendly) == 0)
            {
                if (TryCastingASpellAgainstPlayer(monsterIndex))
                {
                    return;
                }
            }
            // Update the view if necessary
            if (doView)
            {
                _player.UpdatesNeeded.Set(UpdateFlags.UpdateView | UpdateFlags.UpdateLight | UpdateFlags.UpdateScent | UpdateFlags.UpdateMonsters);
            }
            // If we did something unusual and the player saw, let them remember we can do that
            if (monster.IsVisible)
            {
                if (didOpenDoor)
                {
                    race.Knowledge.RFlags2 |= MonsterFlag2.OpenDoor;
                }
                if (didBashDoor)
                {
                    race.Knowledge.RFlags2 |= MonsterFlag2.BashDoor;
                }
                if (didTakeItem)
                {
                    race.Knowledge.RFlags2 |= MonsterFlag2.TakeItem;
                }
                if (didKillItem)
                {
                    race.Knowledge.RFlags2 |= MonsterFlag2.KillItem;
                }
                if (didMoveBody)
                {
                    race.Knowledge.RFlags2 |= MonsterFlag2.MoveBody;
                }
                if (didKillBody)
                {
                    race.Knowledge.RFlags2 |= MonsterFlag2.KillBody;
                }
                if (didPassWall)
                {
                    race.Knowledge.RFlags2 |= MonsterFlag2.PassWall;
                }
                if (didKillWall)
                {
                    race.Knowledge.RFlags2 |= MonsterFlag2.KillWall;
                }
            }
            // If we couldn't do anything because we were afraid and cornered, lose our fear
            if (!doTurn && !doMove && monster.FearLevel != 0)
            {
                monster.FearLevel = 0;
                if (monster.IsVisible)
                {
                    string monsterName = monster.MonsterDesc(0);
                    Profile.Instance.MsgPrint($"{monsterName} turns to fight!");
                }
            }
        }

        /// <summary>
        /// Roll a percentage change of a monster realising it shouldn't do something because the
        /// player resisted it last time. Monsters that are not smart have half the passed chance.
        /// </summary>
        /// <param name="race"> The monster's race </param>
        /// <param name="percentage"> The chance of it happening </param>
        /// <returns> True if it should happen, or false if it should not </returns>
        private bool RealiseSpellIsUseless(MonsterRace race, int percentage)
        {
            if ((race.Flags2 & MonsterFlag2.Smart) == 0)
            {
                percentage /= 2;
            }
            return Program.Rng.RandomLessThan(100) < percentage;
        }

        /// <summary>
        /// Remove flags for ineffective spells from the monster's flags and return them.
        /// </summary>
        /// <param name="monsterIndex"> The index of the monster </param>
        /// <param name="modifiedFlags4"> Flags4 after having spells removed </param>
        /// <param name="initialFlags4"> Flags4 before having spells removed </param>
        /// <param name="modifiedFlags5"> Flags5 after having spells removed </param>
        /// <param name="initialFlags5"> Flags5 before having spells removed </param>
        /// <param name="modifiedFlags6"> Flags6 after having spells removed </param>
        /// <param name="initialFlags6"> Flags6 before having spells removed </param>
        private void RemoveIneffectiveSpells(int monsterIndex, out uint modifiedFlags4, uint initialFlags4,
            out uint modifiedFlags5, uint initialFlags5, out uint modifiedFlags6, uint initialFlags6)
        {
            Monster monster = _level.Monsters[monsterIndex];
            MonsterRace race = monster.Race;
            uint flags4 = initialFlags4;
            uint flags5 = initialFlags5;
            uint flags6 = initialFlags6;
            modifiedFlags4 = initialFlags4;
            modifiedFlags5 = initialFlags5;
            modifiedFlags6 = initialFlags6;
            // If we're stupid, we won't realise how ineffective things are
            if ((race.Flags2 & MonsterFlag2.Stupid) != 0)
            {
                return;
            }
            // Tiny chance of forgetting what we've seen, clearing all smart flags except for ally
            // and clone
            if (monster.Mind != 0 && Program.Rng.RandomLessThan(100) < 1)
            {
                monster.Mind &= (Constants.SmFriendly | Constants.SmCloned);
            }
            uint mindFlags = monster.Mind;
            // If we're not aware of any of the player's resistances, don't bother going through them
            if (mindFlags == 0)
            {
                return;
            }
            // If we know the player is immune to acid, don't do acid spells
            if ((mindFlags & Constants.SmImmAcid) != 0)
            {
                if (RealiseSpellIsUseless(race, 100))
                {
                    flags4 &= ~MonsterFlag4.BreatheAcid;
                }
                if (RealiseSpellIsUseless(race, 100))
                {
                    flags5 &= ~MonsterFlag5.AcidBall;
                }
                if (RealiseSpellIsUseless(race, 100))
                {
                    flags5 &= ~MonsterFlag5.AcidBolt;
                }
            }
            // If we know the player resists acid both temporarily and permanently, probably don't
            // do acid spells
            else if ((mindFlags & Constants.SmOppAcid) != 0 && (mindFlags & Constants.SmResAcid) != 0)
            {
                if (RealiseSpellIsUseless(race, 80))
                {
                    flags4 &= ~MonsterFlag4.BreatheAcid;
                }
                if (RealiseSpellIsUseless(race, 80))
                {
                    flags5 &= ~MonsterFlag5.AcidBall;
                }
                if (RealiseSpellIsUseless(race, 80))
                {
                    flags5 &= ~MonsterFlag5.AcidBolt;
                }
            }
            // If we know the player resists acid at all, maybe don't do acid spells
            else if ((mindFlags & Constants.SmOppAcid) != 0 || (mindFlags & Constants.SmResAcid) != 0)
            {
                if (RealiseSpellIsUseless(race, 30))
                {
                    flags4 &= ~MonsterFlag4.BreatheAcid;
                }
                if (RealiseSpellIsUseless(race, 30))
                {
                    flags5 &= ~MonsterFlag5.AcidBall;
                }
                if (RealiseSpellIsUseless(race, 30))
                {
                    flags5 &= ~MonsterFlag5.AcidBolt;
                }
            }
            // If we know the player is immune to lightning, don't do lightning spells
            if ((mindFlags & Constants.SmImmElec) != 0)
            {
                if (RealiseSpellIsUseless(race, 100))
                {
                    flags4 &= ~MonsterFlag4.BreatheLightning;
                }
                if (RealiseSpellIsUseless(race, 100))
                {
                    flags5 &= ~MonsterFlag5.LightningBall;
                }
                if (RealiseSpellIsUseless(race, 100))
                {
                    flags5 &= ~MonsterFlag5.LightningBolt;
                }
            }
            // If we know the player resists lightning both temporarily and permanently, probably
            // don't do lightning spells
            else if ((mindFlags & Constants.SmOppElec) != 0 && (mindFlags & Constants.SmResElec) != 0)
            {
                if (RealiseSpellIsUseless(race, 80))
                {
                    flags4 &= ~MonsterFlag4.BreatheLightning;
                }
                if (RealiseSpellIsUseless(race, 80))
                {
                    flags5 &= ~MonsterFlag5.LightningBall;
                }
                if (RealiseSpellIsUseless(race, 80))
                {
                    flags5 &= ~MonsterFlag5.LightningBolt;
                }
            }
            // If we know the player resists lightning at all, maybe don't do lightning spells
            else if ((mindFlags & Constants.SmOppElec) != 0 || (mindFlags & Constants.SmResElec) != 0)
            {
                if (RealiseSpellIsUseless(race, 30))
                {
                    flags4 &= ~MonsterFlag4.BreatheLightning;
                }
                if (RealiseSpellIsUseless(race, 30))
                {
                    flags5 &= ~MonsterFlag5.LightningBall;
                }
                if (RealiseSpellIsUseless(race, 30))
                {
                    flags5 &= ~MonsterFlag5.LightningBolt;
                }
            }
            // If we know the player is immune to fire, don't do fire spells
            if ((mindFlags & Constants.SmImmFire) != 0)
            {
                if (RealiseSpellIsUseless(race, 100))
                {
                    flags4 &= ~MonsterFlag4.BreatheFire;
                }
                if (RealiseSpellIsUseless(race, 100))
                {
                    flags5 &= ~MonsterFlag5.FireBall;
                }
                if (RealiseSpellIsUseless(race, 100))
                {
                    flags5 &= ~MonsterFlag5.FireBolt;
                }
            }
            // If we know the player resists fire both temporarily and permanently, probably don't
            // do fire spells
            else if ((mindFlags & Constants.SmOppFire) != 0 && (mindFlags & Constants.SmResFire) != 0)
            {
                if (RealiseSpellIsUseless(race, 80))
                {
                    flags4 &= ~MonsterFlag4.BreatheFire;
                }
                if (RealiseSpellIsUseless(race, 80))
                {
                    flags5 &= ~MonsterFlag5.FireBall;
                }
                if (RealiseSpellIsUseless(race, 80))
                {
                    flags5 &= ~MonsterFlag5.FireBolt;
                }
            }
            // If we know the player resists fire at all, maybe don't do fire spells
            else if ((mindFlags & Constants.SmOppFire) != 0 || (mindFlags & Constants.SmResFire) != 0)
            {
                if (RealiseSpellIsUseless(race, 30))
                {
                    flags4 &= ~MonsterFlag4.BreatheFire;
                }
                if (RealiseSpellIsUseless(race, 30))
                {
                    flags5 &= ~MonsterFlag5.FireBall;
                }
                if (RealiseSpellIsUseless(race, 30))
                {
                    flags5 &= ~MonsterFlag5.FireBolt;
                }
            }
            // If we know the player is immune to cold, don't do fire spells
            if ((mindFlags & Constants.SmImmCold) != 0)
            {
                if (RealiseSpellIsUseless(race, 100))
                {
                    flags4 &= ~MonsterFlag4.BreatheCold;
                }
                if (RealiseSpellIsUseless(race, 100))
                {
                    flags5 &= ~MonsterFlag5.ColdBall;
                }
                if (RealiseSpellIsUseless(race, 100))
                {
                    flags5 &= ~MonsterFlag5.ColdBolt;
                }
                if (RealiseSpellIsUseless(race, 100))
                {
                    flags5 &= ~MonsterFlag5.IceBolt;
                }
            }
            // If we know the player resists cold both temporarily and permanently, probably don't
            // do cold spells
            else if ((mindFlags & Constants.SmOppCold) != 0 && (mindFlags & Constants.SmResCold) != 0)
            {
                if (RealiseSpellIsUseless(race, 80))
                {
                    flags4 &= ~MonsterFlag4.BreatheCold;
                }
                if (RealiseSpellIsUseless(race, 80))
                {
                    flags5 &= ~MonsterFlag5.ColdBall;
                }
                if (RealiseSpellIsUseless(race, 80))
                {
                    flags5 &= ~MonsterFlag5.ColdBolt;
                }
                if (RealiseSpellIsUseless(race, 80))
                {
                    flags5 &= ~MonsterFlag5.IceBolt;
                }
            }
            // If we know the player resists cold at all, maybe don't do cold spells
            else if ((mindFlags & Constants.SmOppCold) != 0 || (mindFlags & Constants.SmResCold) != 0)
            {
                if (RealiseSpellIsUseless(race, 30))
                {
                    flags4 &= ~MonsterFlag4.BreatheCold;
                }
                if (RealiseSpellIsUseless(race, 30))
                {
                    flags5 &= ~MonsterFlag5.ColdBall;
                }
                if (RealiseSpellIsUseless(race, 30))
                {
                    flags5 &= ~MonsterFlag5.ColdBolt;
                }
                if (RealiseSpellIsUseless(race, 30))
                {
                    flags5 &= ~MonsterFlag5.IceBolt;
                }
            }
            // If we know the player resists poison both temporarily and permanently, probably don't
            // do poison spells
            if ((mindFlags & Constants.SmOppPois) != 0 && (mindFlags & Constants.SmResPois) != 0)
            {
                if (RealiseSpellIsUseless(race, 80))
                {
                    flags4 &= ~MonsterFlag4.BreathePoison;
                }
                if (RealiseSpellIsUseless(race, 80))
                {
                    flags5 &= ~MonsterFlag5.PoisonBall;
                }
                if (RealiseSpellIsUseless(race, 40))
                {
                    flags4 &= ~MonsterFlag4.RadiationBall;
                }
                if (RealiseSpellIsUseless(race, 40))
                {
                    flags4 &= ~MonsterFlag4.BreatheRadiation;
                }
            }
            // If we know the player resists poison at all, maybe don't do cold spells
            else if ((mindFlags & Constants.SmOppPois) != 0 || (mindFlags & Constants.SmResPois) != 0)
            {
                if (RealiseSpellIsUseless(race, 30))
                {
                    flags4 &= ~MonsterFlag4.BreathePoison;
                }
                if (RealiseSpellIsUseless(race, 30))
                {
                    flags5 &= ~MonsterFlag5.PoisonBall;
                }
            }
            // If we know the player resists nether, maybe don't do nether spells
            if ((mindFlags & Constants.SmResNeth) != 0)
            {
                if (RealiseSpellIsUseless(race, 50))
                {
                    flags4 &= ~MonsterFlag4.BreatheNether;
                }
                if (RealiseSpellIsUseless(race, 50))
                {
                    flags5 &= ~MonsterFlag5.NetherBall;
                }
                if (RealiseSpellIsUseless(race, 50))
                {
                    flags5 &= ~MonsterFlag5.NetherBolt;
                }
            }
            // If we know the player resists light, maybe don't do light spells
            if ((mindFlags & Constants.SmResLight) != 0)
            {
                if (RealiseSpellIsUseless(race, 50))
                {
                    flags4 &= ~MonsterFlag4.BreatheLight;
                }
            }
            // If we know the player resists darkness, maybe don't do darkness spells
            if ((mindFlags & Constants.SmResDark) != 0)
            {
                if (RealiseSpellIsUseless(race, 50))
                {
                    flags4 &= ~MonsterFlag4.BreatheDark;
                }
                if (RealiseSpellIsUseless(race, 50))
                {
                    flags5 &= ~MonsterFlag5.DarkBall;
                }
            }
            // If we know the player resists fear, don't do fear spells
            if ((mindFlags & Constants.SmResFear) != 0)
            {
                if (RealiseSpellIsUseless(race, 100))
                {
                    flags5 &= ~MonsterFlag5.Scare;
                }
            }
            // If we know the player resists confiusion, maybe don't do confusion spells
            if ((mindFlags & Constants.SmResConf) != 0)
            {
                if (RealiseSpellIsUseless(race, 100))
                {
                    flags5 &= ~MonsterFlag5.Confuse;
                }
                if (RealiseSpellIsUseless(race, 50))
                {
                    flags4 &= ~MonsterFlag4.BreatheConfusion;
                }
            }
            // If we know the player resists chaos, maybe don't do chaos or confusion spells
            if ((mindFlags & Constants.SmResChaos) != 0)
            {
                if (RealiseSpellIsUseless(race, 100))
                {
                    flags5 &= ~MonsterFlag5.Confuse;
                }
                if (RealiseSpellIsUseless(race, 50))
                {
                    flags4 &= ~MonsterFlag4.BreatheConfusion;
                }
                if (RealiseSpellIsUseless(race, 50))
                {
                    flags4 &= ~MonsterFlag4.BreatheChaos;
                }
                if (RealiseSpellIsUseless(race, 50))
                {
                    flags4 &= ~MonsterFlag4.ChaosBall;
                }
            }
            // If we know the player resists disenchantment, don't do disenchantment spells
            if ((mindFlags & Constants.SmResDisen) != 0)
            {
                if (RealiseSpellIsUseless(race, 100))
                {
                    flags4 &= ~MonsterFlag4.BreatheDisenchant;
                }
            }
            // If we know the player resists blindness, don't do blindness spells
            if ((mindFlags & Constants.SmResBlind) != 0)
            {
                if (RealiseSpellIsUseless(race, 100))
                {
                    flags5 &= ~MonsterFlag5.Blindness;
                }
            }
            // If we know the player resists nexus, maybe don't do nexus or teleport spells
            if ((mindFlags & Constants.SmResNexus) != 0)
            {
                if (RealiseSpellIsUseless(race, 50))
                {
                    flags4 &= ~MonsterFlag4.BreatheNexus;
                }
                if (RealiseSpellIsUseless(race, 50))
                {
                    flags6 &= ~MonsterFlag6.TeleportLevel;
                }
            }
            // If we know the player resists sound, maybe don't do sound spells
            if ((mindFlags & Constants.SmResSound) != 0)
            {
                if (RealiseSpellIsUseless(race, 50))
                {
                    flags4 &= ~MonsterFlag4.BreatheSound;
                }
            }
            // If we know the player resists shards, maybe don't do shard spells
            if ((mindFlags & Constants.SmResShard) != 0)
            {
                if (RealiseSpellIsUseless(race, 50))
                {
                    flags4 &= ~MonsterFlag4.BreatheShards;
                }
                if (RealiseSpellIsUseless(race, 20))
                {
                    flags4 &= ~MonsterFlag4.ShardBall;
                }
            }
            // If we know the player reflects bolts, don't do bolt spells
            if ((mindFlags & Constants.SmImmReflect) != 0)
            {
                if (RealiseSpellIsUseless(race, 100))
                {
                    flags5 &= ~MonsterFlag5.ColdBolt;
                }
                if (RealiseSpellIsUseless(race, 100))
                {
                    flags5 &= ~MonsterFlag5.FireBolt;
                }
                if (RealiseSpellIsUseless(race, 100))
                {
                    flags5 &= ~MonsterFlag5.AcidBolt;
                }
                if (RealiseSpellIsUseless(race, 100))
                {
                    flags5 &= ~MonsterFlag5.LightningBolt;
                }
                if (RealiseSpellIsUseless(race, 100))
                {
                    flags5 &= ~MonsterFlag5.PoisonBolt;
                }
                if (RealiseSpellIsUseless(race, 100))
                {
                    flags5 &= ~MonsterFlag5.NetherBolt;
                }
                if (RealiseSpellIsUseless(race, 100))
                {
                    flags5 &= ~MonsterFlag5.WaterBolt;
                }
                if (RealiseSpellIsUseless(race, 100))
                {
                    flags5 &= ~MonsterFlag5.ManaBolt;
                }
                if (RealiseSpellIsUseless(race, 100))
                {
                    flags5 &= ~MonsterFlag5.PlasmaBolt;
                }
                if (RealiseSpellIsUseless(race, 100))
                {
                    flags5 &= ~MonsterFlag5.IceBolt;
                }
                if (RealiseSpellIsUseless(race, 100))
                {
                    flags5 &= ~MonsterFlag5.MagicMissile;
                }
                if (RealiseSpellIsUseless(race, 100))
                {
                    flags4 &= ~MonsterFlag4.Arrow1D6;
                }
                if (RealiseSpellIsUseless(race, 100))
                {
                    flags4 &= ~MonsterFlag4.Arrow3D6;
                }
                if (RealiseSpellIsUseless(race, 100))
                {
                    flags4 &= ~MonsterFlag4.Arrow5D6;
                }
                if (RealiseSpellIsUseless(race, 100))
                {
                    flags4 &= ~MonsterFlag4.Arrow7D6;
                }
            }
            // If we know the player has free action, don't do slow or hold spells
            if ((mindFlags & Constants.SmImmFree) != 0)
            {
                if (RealiseSpellIsUseless(race, 100))
                {
                    flags5 &= ~MonsterFlag5.Hold;
                }
                if (RealiseSpellIsUseless(race, 100))
                {
                    flags5 &= ~MonsterFlag5.Slow;
                }
            }
            // If we know the player has no mana, don't do mana drain
            if ((mindFlags & Constants.SmImmMana) != 0)
            {
                if (RealiseSpellIsUseless(race, 100))
                {
                    flags5 &= ~MonsterFlag5.DrainMana;
                }
            }
            modifiedFlags4 = flags4;
            modifiedFlags5 = flags5;
            modifiedFlags6 = flags6;
        }

        /// <summary>
        /// Return whether or not a spell is suitable for annoying the player
        /// </summary>
        /// <param name="spell"> The spell's number (32*flags + bit in flag) </param>
        /// <returns> True if the spell is annoying, false otherwise </returns>
        private bool SpellIsForAnnoyance(int spell)
        {
            // MonsterFlag4.Shriek
            if (spell == 96 + 0)
            {
                return true;
            }
            // MonsterFlag5.DrainMana MonsterFlag5.MindBlast MonsterFlag5.BrainSmash
            // MonsterFlag5.CauseLightWounds MonsterFlag5.CauseSeriousWounds MonsterFlag5.CauseCriticalWounds
            if (spell >= 128 + 9 && spell <= 128 + 14)
            {
                return true;
            }
            // MonsterFlag5.Scare MonsterFlag5.Blindness MonsterFlag5.Confuse MonsterFlag5.Slow MonsterFlag5.Hold
            if (spell >= 128 + 27 && spell <= 128 + 31)
            {
                return true;
            }
            // MonsterFlag6.TeleportTo
            if (spell == 160 + 8)
            {
                return true;
            }
            // MonsterFlag6.Darkness MonsterFlag6.CreateTraps MonsterFlag6.Forget
            if (spell >= 160 + 12 && spell <= 160 + 14)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Return whether a spell is primarily an attack spell
        /// </summary>
        /// <param name="spell"> The spell's number (32*flags + bit in flag) </param>
        /// <returns> True if the spell is an attack spell, false otherwise </returns>
        private bool SpellIsForAttack(int spell)
        {
            // MonsterFlag4.ShardBall MonsterFlag4.Arrow1D6 MonsterFlag4.Arrow3D6
            // MonsterFlag4.Arrow5D6 MonsterFlag4.Arrow7D6 MonsterFlag4.BreatheAcid
            // MonsterFlag4.BreatheLightning MonsterFlag4.BreatheFire MonsterFlag4.BreatheCold
            // MonsterFlag4.BreathePoison MonsterFlag4.BreatheNether MonsterFlag4.BreatheLight
            // MonsterFlag4.BreatheDark MonsterFlag4.BreatheConfusion MonsterFlag4.BreatheSound
            // MonsterFlag4.BreatheChaos MonsterFlag4.BreatheDisenchant MonsterFlag4.BreatheNexus
            // MonsterFlag4.BreatheTime MonsterFlag4.BreatheInertia MonsterFlag4.BreatheGravity
            // MonsterFlag4.BreatheShards MonsterFlag4.BreathePlasma MonsterFlag4.BreatheForce
            // MonsterFlag4.BreatheMana MonsterFlag4.RadiationBall MonsterFlag4.BreatheRadiation
            // MonsterFlag4.ChaosBall MonsterFlag4.BreatheDisintegration
            if (spell < 128 && spell > 96)
            {
                return true;
            }
            // MonsterFlag5.AcidBall MonsterFlag5.LightningBall MonsterFlag5.FireBall
            // MonsterFlag5.ColdBall MonsterFlag5.PoisonBall MonsterFlag5.NetherBall
            // MonsterFlag5.WaterBall MonsterFlag5.ManaBall MonsterFlag5.DarkBall
            if (spell >= 128 && spell <= 128 + 8)
            {
                return true;
            }
            // MonsterFlag5.CauseLightWounds MonsterFlag5.CauseSeriousWounds
            // MonsterFlag5.CauseCriticalWounds MonsterFlag5.CauseMortalWounds MonsterFlag5.AcidBolt
            // MonsterFlag5.LightningBolt MonsterFlag5.FireBolt MonsterFlag5.ColdBolt
            // MonsterFlag5.PoisonBolt MonsterFlag5.NetherBolt MonsterFlag5.WaterBolt
            // MonsterFlag5.ManaBolt MonsterFlag5.PlasmaBolt MonsterFlag5.IceBolt
            // MonsterFlag5.MagicMissile MonsterFlag5.Scare
            if (spell >= 128 + 12 && spell <= 128 + 27)
            {
                return true;
            }
            // MonsterFlag6.DreadCurse
            if (spell == 160 + 1)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Return whether or not a spell is suitable for escaping the player
        /// </summary>
        /// <param name="spell"> The spell's number (32*flags + bit in flag) </param>
        /// <returns> True if the spell is good for escaping, false otherwise </returns>
        private bool SpellIsForEscape(int spell)
        {
            // MonsterFlag6.Blink MonsterFlag6.TeleportSelf
            if (spell == 160 + 4 || spell == 160 + 5)
            {
                return true;
            }
            // MonsterFlag6.TeleportAway MonsterFlag6.TeleportLevel
            if (spell == 160 + 9 || spell == 160 + 10)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Return whether or not a spell is suitable for hasting oneself
        /// </summary>
        /// <param name="spell"> The spell's number (32*flags + bit in flag) </param>
        /// <returns> True if the spell is good for hasting, false otherwise </returns>
        private bool SpellIsForHaste(int spell)
        {
            // MonsterFlag6.Haste
            return spell == 160 + 0;
        }

        /// <summary>
        /// Return whether or not a spell is suitable for healing oneself
        /// </summary>
        /// <param name="spell"> The spell's number (32*flags + bit in flag) </param>
        /// <returns> True if the spell is good for healing, false otherwise </returns>
        private bool SpellIsForHealing(int spell)
        {
            // MonsterFlag6.Heal
            return spell == 160 + 2;
        }

        /// <summary>
        /// Return whether or not a spell is a summoning spell
        /// </summary>
        /// <param name="spell"> The spell's number (32*flags + bit in flag) </param>
        /// <returns> True if the spell is a summoning spell, false otherwise </returns>
        private bool SpellIsForSummoning(int spell)
        {
            // MonsterFlag6.SummonKin MonsterFlag6.SummonReaver MonsterFlag6.SummonMonster
            // MonsterFlag6.SummonMonsters MonsterFlag6.SummonAnt MonsterFlag6.SummonSpider
            // MonsterFlag6.SummonHound MonsterFlag6.SummonHydra MonsterFlag6.SummonCthuloid
            // MonsterFlag6.SummonDemon MonsterFlag6.SummonUndead MonsterFlag6.SummonDragon
            // MonsterFlag6.SummonHiUndead MonsterFlag6.SummonHiDragon
            // MonsterFlag6.SummonGreatOldOne MonsterFlag6.SummonUnique
            return spell >= 160 + 16;
        }

        /// <summary>
        /// Return whether or not a spell gives a tactical advantage
        /// </summary>
        /// <param name="spell"> The spell's number (32*flags + bit in flag) </param>
        /// <returns> True if the spell gives a tactical advantage, false otherwise </returns>
        private bool SpellIsTactical(int spell)
        {
            // MonsterFlag6.Blink
            return spell == 160 + 4;
        }

        /// <summary>
        /// Check whether there's room to summon something next to a location
        /// </summary>
        /// <param name="targetY"> The target Y coordinate </param>
        /// <param name="targetX"> The target X coordinate </param>
        /// <returns> True if there is room, or false if there is not </returns>
        private bool SummonPossible(int targetY, int targetX)
        {
            int y;
            for (y = targetY - 2; y <= targetY + 2; y++)
            {
                int x;
                for (x = targetX - 2; x <= targetX + 2; x++)
                {
                    // Can't summon out of bounds
                    if (!_level.InBounds(y, x))
                    {
                        continue;
                    }
                    // Can't summon too far away from the target location
                    if (_level.Distance(targetY, targetX, y, x) > 2)
                    {
                        continue;
                    }
                    // Can't summon onto an Elder Sign
                    if (_level.Grid[y][x].FeatureType.Name == "ElderSign")
                    {
                        continue;
                    }
                    // Can't summon onto a Yellow Sign
                    if (_level.Grid[y][x].FeatureType.Name == "YellowSign")
                    {
                        continue;
                    }
                    // An empty tile in line of sight is acceptable
                    if (_level.GridPassableNoCreature(y, x) && _level.Los(targetY, targetX, y, x))
                    {
                        return true;
                    }
                }
            }
            // We didn't find anywhere and ran out of places to look
            return false;
        }

        /// <summary>
        /// Take damage after being hit by another monster
        /// </summary>
        /// <param name="monsterIndex"> The index of the monster taking the damage </param>
        /// <param name="damage"> The damage taken </param>
        /// <param name="fear"> Whether the damage makes us afraid </param>
        /// <param name="note"> A special descriptive note that replaces the normal death message </param>
        private void TakeDamageFromAnotherMonster(int monsterIndex, int damage, out bool fear, string note)
        {
            fear = false;
            Monster monster = _level.Monsters[monsterIndex];
            MonsterRace race = monster.Race;
            // Track the monster that has just taken damage
            if (_saveGame.TrackedMonsterIndex == monsterIndex)
            {
                _player.RedrawNeeded.Set(RedrawFlag.PrHealth);
            }
            monster.SleepLevel = 0;
            // Take the damage
            monster.Health -= damage;
            // Did it kill us?
            if (monster.Health < 0)
            {
                // Uniques and guardians can never be killed by other monsters
                if ((race.Flags1 & MonsterFlag1.Unique) != 0 || (race.Flags1 & MonsterFlag1.Guardian) != 0)
                {
                    monster.Health = 1;
                }
                else
                {
                    // Construct a message telling the player what happened
                    string monsterName = monster.MonsterDesc(0);
                    Gui.PlaySound(SoundEffect.MonsterDies);
                    // Append the note if there is one
                    if (!string.IsNullOrEmpty(note))
                    {
                        Profile.Instance.MsgPrint(monsterName + note);
                    }
                    // Don't tell the player if the monster is not visible
                    else if (!monster.IsVisible)
                    {
                    }
                    // Some non-living things are destroyed rather than dying
                    else if ((race.Flags3 & MonsterFlag3.Demon) != 0 || (race.Flags3 & MonsterFlag3.Undead) != 0 ||
                             (race.Flags3 & MonsterFlag3.Cthuloid) != 0 || (race.Flags2 & MonsterFlag2.Stupid) != 0 ||
                             (race.Flags3 & MonsterFlag3.Nonliving) != 0 || "Evg".Contains(race.Character.ToString()))
                    {
                        Profile.Instance.MsgPrint($"{monsterName} is destroyed.");
                    }
                    else
                    {
                        Profile.Instance.MsgPrint($"{monsterName} is killed.");
                    }
                    // Let the save game know we've died
                    _saveGame.MonsterDeath(monsterIndex);
                    // Delete us from the monster list
                    _level.Monsters.DeleteMonsterByIndex(monsterIndex, true);
                    fear = false;
                    return;
                }
            }
            // Assuming we survived, check if the attack made us afraid
            if (monster.FearLevel != 0 && damage > 0)
            {
                // If we're already afraid, we might get desperate and overcome our fear
                int tmp = Program.Rng.DieRoll(damage);
                if (tmp < monster.FearLevel)
                {
                    monster.FearLevel -= tmp;
                }
                else
                {
                    monster.FearLevel = 0;
                    fear = false;
                }
            }
            // If we weren't already afraid, this attack might make us afraid
            if (monster.FearLevel == 0 && (race.Flags3 & MonsterFlag3.ImmuneFear) == 0)
            {
                int percentage = 100 * monster.Health / monster.MaxHealth;
                if ((percentage <= 10 && Program.Rng.RandomLessThan(10) < percentage) ||
                    (damage >= monster.Health && Program.Rng.RandomLessThan(100) < 80))
                {
                    fear = true;
                    monster.FearLevel = Program.Rng.DieRoll(10) +
                                   (damage >= monster.Health && percentage > 7 ? 20 : (11 - percentage) * 5);
                }
            }
        }

        /// <summary>
        /// Adjust the coordinates we're trying to move to if we can't get directly there for some reason
        /// </summary>
        /// <param name="monsterIndex"> The index of the monster trying to move </param>
        /// <param name="target"> The target location we're moving to </param>
        private void TrackPlayerByScent(int monsterIndex, MapCoordinate target)
        {
            Monster monster = _level.Monsters[monsterIndex];
            MonsterRace race = monster.Race;
            // If we can move through walls then we don't need to adjust anything
            if ((race.Flags2 & MonsterFlag2.PassWall) != 0)
            {
                return;
            }
            if ((race.Flags2 & MonsterFlag2.KillWall) != 0)
            {
                return;
            }
            int y1 = monster.MapY;
            int x1 = monster.MapX;
            GridTile cPtr = _level.Grid[y1][x1];
            // If we have no scent of the player then don't change where we were going
            if (cPtr.ScentAge < _level.Grid[_player.MapY][_player.MapX].ScentAge)
            {
                if (cPtr.ScentAge == 0)
                {
                    return;
                }
            }
            if (cPtr.ScentStrength > Constants.MonsterFlowDepth)
            {
                return;
            }
            if (cPtr.ScentStrength > race.NoticeRange)
            {
                return;
            }
            // If we can actually see the player then don't change where we are going
            if (_level.PlayerHasLosBold(y1, x1))
            {
                return;
            }
            int when = 0;
            int cost = 999;
            // Check the eight directions we can move to see which has the most recent or strongest scent
            for (int i = 7; i >= 0; i--)
            {
                int y = y1 + _level.OrderedDirectionYOffset[i];
                int x = x1 + _level.OrderedDirectionXOffset[i];
                if (_level.Grid[y][x].ScentAge == 0)
                {
                    continue;
                }
                if (_level.Grid[y][x].ScentAge < when)
                {
                    continue;
                }
                if (_level.Grid[y][x].ScentStrength > cost)
                {
                    continue;
                }
                when = _level.Grid[y][x].ScentAge;
                cost = _level.Grid[y][x].ScentStrength;
                // Give us a target in the general direction of the strongest scent
                target.Y = _player.MapY + (16 * _level.OrderedDirectionYOffset[i]);
                target.X = _player.MapX + (16 * _level.OrderedDirectionXOffset[i]);
            }
        }

        /// <summary>
        /// Try to cast a spell on another monster
        /// </summary>
        /// <param name="monsterIndex"> The index of the monster casting the spell </param>
        /// <returns> True if we cast a spell, false otherwise </returns>
        private bool TryCastingASpellAgainstAnotherMonster(int monsterIndex)
        {
            int count = 0;
            int[] spell = new int[96];
            int num = 0;
            Monster monster = _level.Monsters[monsterIndex];
            MonsterRace race = monster.Race;
            bool wakeUp = false;
            bool blind = _player.TimedBlindness != 0;
            bool seen = !blind && monster.IsVisible;
            bool friendly = (monster.Mind & Constants.SmFriendly) != 0;
            // Can't cast a spell if we're confused
            if (monster.ConfusionLevel != 0)
            {
                return false;
            }
            // We have a chance to cast a spell based on our race
            int chance = (race.FreqInate + race.FreqSpell) / 2;
            if (chance == 0)
            {
                return false;
            }
            if (Program.Rng.RandomLessThan(100) >= chance)
            {
                return false;
            }
            // Look through the monster list to find a potential target
            for (int i = 1; i < _level.MMax; i++)
            {
                int targetIndex = i;
                Monster target = _level.Monsters[targetIndex];
                MonsterRace targetRace = target.Race;
                // Can't cast spells on ourself
                if (target == monster)
                {
                    continue;
                }
                // Can't cast spells on empty monster slots
                if (target.Race == null)
                {
                    continue;
                }
                // Don't cast spells on monsters from the same team
                if ((monster.Mind & Constants.SmFriendly) == (target.Mind & Constants.SmFriendly))
                {
                    continue;
                }
                // If the target is too far away from the player, don't cast a spell
                if (target.DistanceFromPlayer > Constants.MaxRange)
                {
                    continue;
                }
                // If we don't have line of sight to the target, don't cast a spell
                if (!_level.Projectable(monster.MapY, monster.MapX, target.MapY, target.MapX))
                {
                    continue;
                }
                int y = target.MapY;
                int x = target.MapX;
                int rlev = race.Level >= 1 ? race.Level : 1;
                uint f4 = race.Flags4;
                uint f5 = race.Flags5;
                uint f6 = race.Flags6;
                // If we're smart and badly injured, we may want to prioritise spells that disable
                // the target, summon help, or let us escape over spells that do direct damage
                if ((race.Flags2 & MonsterFlag2.Smart) != 0 && monster.Health < monster.MaxHealth / 10 &&
                    Program.Rng.RandomLessThan(100) < 50)
                {
                    f4 &= MonsterFlag4.IntMask;
                    f5 &= MonsterFlag5.IntMask;
                    f6 &= MonsterFlag6.IntMask;
                    // If we just got rid of all our spells then abort
                    if (f4 == 0 && f5 == 0 && f6 == 0)
                    {
                        return false;
                    }
                }
                // If there's nowhere around the target to put a summoned creature, then remove
                // summoning spells
                if (((f4 & MonsterFlag4.SummonMask) != 0 || (f5 & MonsterFlag5.SummonMask) != 0 ||
                     (f6 & MonsterFlag6.SummonMask) != 0) && (race.Flags2 & MonsterFlag2.Stupid) == 0 &&
                    !SummonPossible(target.MapY, target.MapX))
                {
                    f4 &= ~MonsterFlag4.SummonMask;
                    f5 &= ~MonsterFlag5.SummonMask;
                    f6 &= ~MonsterFlag6.SummonMask;
                }
                // If we've removed all our spells, don't cast anything
                if (f4 == 0 && f5 == 0 && f6 == 0)
                {
                    return false;
                }
                // Gather up all the spells that are left and put them in an array Flags4 spells are 96
                // + bit number (0-31)
                int k;
                for (k = 0; k < 32; k++)
                {
                    if ((f4 & (1u << k)) != 0)
                    {
                        spell[num++] = k + (32 * 3);
                    }
                }
                for (k = 0; k < 32; k++)
                {
                    if ((f5 & (1u << k)) != 0)
                    {
                        spell[num++] = k + (32 * 4);
                    }
                }
                for (k = 0; k < 32; k++)
                {
                    if ((f6 & (1u << k)) != 0)
                    {
                        spell[num++] = k + (32 * 5);
                    }
                }
                // If we ended up with no spells, don't cast
                if (num == 0)
                {
                    return false;
                }
                // If the player's already dead or off the level, don't cast
                if (!_saveGame.Playing || _player.IsDead || _saveGame.NewLevelFlag)
                {
                    return false;
                }
                string monsterName = monster.MonsterDesc(0x00);
                string monsterPossessive = monster.MonsterDesc(0x22);
                string targetName = target.MonsterDesc(0x00);
                monster.MonsterDesc(0x88);
                // Against other monsters we pick spells randomly
                int thrownSpell = spell[Program.Rng.RandomLessThan(num)];
                bool seeMonster = seen;
                bool seeTarget = !blind && target.IsVisible;
                bool seeEither = seeMonster || seeTarget;
                bool seeBoth = seeMonster && seeTarget;
                // If we decided not to cast, don't
                switch (thrownSpell)
                {
                    // MonsterFlag4.Shriek
                    case 96 + 0:
                        _saveGame.Disturb(true);
                        Profile.Instance.MsgPrint(!seeMonster ? "You hear a shriek." : $"{monsterName} shrieks at {targetName}.");
                        wakeUp = true;
                        break;

                    // MonsterFlag4.Xxx2
                    case 96 + 1:
                    // MonsterFlag4.Xxx3
                    case 96 + 2:
                        break;

                    // MonsterFlag4.ShardBall
                    case 96 + 3:
                        _saveGame.Disturb(true);
                        Profile.Instance.MsgPrint(!seeEither
                            ? "You hear someone mumble."
                            : $"{monsterName} casts a shard ball at {targetName}");
                        BreatheAtMonster(monsterIndex, y, x, new ProjectShard(SaveGame.Instance.SpellEffects), monster.Health / 4 > 800 ? 800 : monster.Health / 4, 2);
                        break;

                    // MonsterFlag4.Arrow1D6
                    case 96 + 4:
                        _saveGame.Disturb(true);
                        Profile.Instance.MsgPrint(!seeEither
                            ? "You hear a strange noise."
                            : $"{monsterName} fires an arrow at {targetName}");
                        FireBoltAtMonster(monsterIndex, y, x, new ProjectArrow(SaveGame.Instance.SpellEffects), Program.Rng.DiceRoll(1, 6));
                        break;

                    // MonsterFlag4.Arrow3D6
                    case 96 + 5:
                        _saveGame.Disturb(true);
                        Profile.Instance.MsgPrint(!seeEither
                            ? "You hear a strange noise."
                            : $"{monsterName} fires an arrow at {targetName}");
                        FireBoltAtMonster(monsterIndex, y, x, new ProjectArrow(SaveGame.Instance.SpellEffects), Program.Rng.DiceRoll(3, 6));
                        break;

                    // MonsterFlag4.Arrow5D6
                    case 96 + 6:
                        _saveGame.Disturb(true);
                        Profile.Instance.MsgPrint(!seeEither
                            ? "You hear a strange noise."
                            : $"{monsterName} fires a missile at {targetName}");
                        FireBoltAtMonster(monsterIndex, y, x, new ProjectArrow(SaveGame.Instance.SpellEffects), Program.Rng.DiceRoll(5, 6));
                        break;

                    // MonsterFlag4.Arrow7D6
                    case 96 + 7:
                        if (!seeEither)
                        {
                            Profile.Instance.MsgPrint("You hear a strange noise.");
                        }
                        else
                        {
                            _saveGame.Disturb(true);
                        }
                        Profile.Instance.MsgPrint(blind
                            ? $"{monsterName} makes a strange noise."
                            : $"{monsterName} fires a missile at {targetName}");
                        FireBoltAtMonster(monsterIndex, y, x, new ProjectArrow(SaveGame.Instance.SpellEffects), Program.Rng.DiceRoll(7, 6));
                        break;

                    // MonsterFlag4.BreatheAcid
                    case 96 + 8:
                        _saveGame.Disturb(true);
                        Profile.Instance.MsgPrint(!seeEither
                            ? "You hear breathing noise."
                            : $"{monsterName} breathes acid at {targetName}");
                        BreatheAtMonster(monsterIndex, y, x, new ProjectAcid(SaveGame.Instance.SpellEffects), monster.Health / 3 > 1600 ? 1600 : monster.Health / 3, 0);
                        break;

                    // MonsterFlag4.BreatheLightning
                    case 96 + 9:
                        _saveGame.Disturb(true);
                        Profile.Instance.MsgPrint(!seeEither
                            ? "You hear breathing noise."
                            : $"{monsterName} breathes lightning at {targetName}");
                        BreatheAtMonster(monsterIndex, y, x, new ProjectElec(SaveGame.Instance.SpellEffects), monster.Health / 3 > 1600 ? 1600 : monster.Health / 3, 0);
                        break;

                    // MonsterFlag4.BreatheFire
                    case 96 + 10:
                        _saveGame.Disturb(true);
                        Profile.Instance.MsgPrint(!seeEither
                            ? "You hear breathing noise."
                            : $"{monsterName} breathes fire at {targetName}");
                        BreatheAtMonster(monsterIndex, y, x, new ProjectFire(SaveGame.Instance.SpellEffects), monster.Health / 3 > 1600 ? 1600 : monster.Health / 3, 0);
                        break;

                    // MonsterFlag4.BreatheCold
                    case 96 + 11:
                        _saveGame.Disturb(true);
                        Profile.Instance.MsgPrint(!seeEither
                            ? "You hear breathing noise."
                            : $"{monsterName} breathes frost at {targetName}");
                        BreatheAtMonster(monsterIndex, y, x, new ProjectCold(SaveGame.Instance.SpellEffects), monster.Health / 3 > 1600 ? 1600 : monster.Health / 3, 0);
                        break;

                    // MonsterFlag4.BreathePoison
                    case 96 + 12:
                        _saveGame.Disturb(true);
                        Profile.Instance.MsgPrint(!seeEither
                            ? "You hear breathing noise."
                            : $"{monsterName} breathes gas at {targetName}");
                        BreatheAtMonster(monsterIndex, y, x, new ProjectPois(SaveGame.Instance.SpellEffects), monster.Health / 3 > 800 ? 800 : monster.Health / 3, 0);
                        break;

                    // MonsterFlag4.BreatheNether
                    case 96 + 13:
                        _saveGame.Disturb(true);
                        Profile.Instance.MsgPrint(!seeEither
                            ? "You hear breathing noise."
                            : $"{monsterName} breathes nether at {targetName}");
                        BreatheAtMonster(monsterIndex, y, x, new ProjectNether(SaveGame.Instance.SpellEffects), monster.Health / 6 > 550 ? 550 : monster.Health / 6, 0);
                        break;

                    // MonsterFlag4.BreatheLight
                    case 96 + 14:
                        _saveGame.Disturb(true);
                        Profile.Instance.MsgPrint(!seeEither
                            ? "You hear breathing noise."
                            : $"{monsterName} breathes light at {targetName}");
                        BreatheAtMonster(monsterIndex, y, x, new ProjectLight(SaveGame.Instance.SpellEffects), monster.Health / 6 > 400 ? 400 : monster.Health / 6, 0);
                        break;

                    // MonsterFlag4.BreatheDark
                    case 96 + 15:
                        _saveGame.Disturb(true);
                        Profile.Instance.MsgPrint(!seeEither
                            ? "You hear breathing noise."
                            : $"{monsterName} breathes darkness at {targetName}");
                        BreatheAtMonster(monsterIndex, y, x, new ProjectDark(SaveGame.Instance.SpellEffects), monster.Health / 6 > 400 ? 400 : monster.Health / 6, 0);
                        break;

                    // MonsterFlag4.BreatheConfusion
                    case 96 + 16:
                        _saveGame.Disturb(true);
                        Profile.Instance.MsgPrint(!seeEither
                            ? "You hear breathing noise."
                            : $"{monsterName} breathes confusion at {targetName}");
                        BreatheAtMonster(monsterIndex, y, x, new ProjectConfusion(SaveGame.Instance.SpellEffects), monster.Health / 6 > 400 ? 400 : monster.Health / 6,
                            0);
                        break;

                    // MonsterFlag4.BreatheSound
                    case 96 + 17:
                        _saveGame.Disturb(true);
                        Profile.Instance.MsgPrint(!seeEither
                            ? "You hear breathing noise."
                            : $"{monsterName} breathes sound at {targetName}");
                        BreatheAtMonster(monsterIndex, y, x, new ProjectSound(SaveGame.Instance.SpellEffects), monster.Health / 6 > 400 ? 400 : monster.Health / 6, 0);
                        break;

                    // MonsterFlag4.BreatheChaos
                    case 96 + 18:
                        _saveGame.Disturb(true);
                        Profile.Instance.MsgPrint(!seeEither
                            ? "You hear breathing noise."
                            : $"{monsterName} breathes chaos at {targetName}");
                        BreatheAtMonster(monsterIndex, y, x, new ProjectChaos(SaveGame.Instance.SpellEffects), monster.Health / 6 > 600 ? 600 : monster.Health / 6, 0);
                        break;

                    // MonsterFlag4.BreatheDisenchant
                    case 96 + 19:
                        _saveGame.Disturb(true);
                        Profile.Instance.MsgPrint(!seeEither
                            ? "You hear breathing noise."
                            : $"{monsterName} breathes disenchantment at {targetName}");
                        BreatheAtMonster(monsterIndex, y, x, new ProjectDisenchant(SaveGame.Instance.SpellEffects), monster.Health / 6 > 500 ? 500 : monster.Health / 6,
                            0);
                        break;

                    // MonsterFlag4.BreatheNexus
                    case 96 + 20:
                        _saveGame.Disturb(true);
                        Profile.Instance.MsgPrint(!seeEither
                            ? "You hear breathing noise."
                            : $"{monsterName} breathes nexus at {targetName}");
                        BreatheAtMonster(monsterIndex, y, x, new ProjectNexus(SaveGame.Instance.SpellEffects), monster.Health / 3 > 250 ? 250 : monster.Health / 3, 0);
                        break;

                    // MonsterFlag4.BreatheTime
                    case 96 + 21:
                        _saveGame.Disturb(true);
                        Profile.Instance.MsgPrint(!seeEither
                            ? "You hear breathing noise."
                            : $"{monsterName} breathes time at {targetName}");
                        BreatheAtMonster(monsterIndex, y, x, new ProjectTime(SaveGame.Instance.SpellEffects), monster.Health / 3 > 150 ? 150 : monster.Health / 3, 0);
                        break;

                    // MonsterFlag4.BreatheInertia
                    case 96 + 22:
                        _saveGame.Disturb(true);
                        Profile.Instance.MsgPrint(!seeEither
                            ? "You hear breathing noise."
                            : $"{monsterName} breathes inertia at {targetName}");
                        BreatheAtMonster(monsterIndex, y, x, new ProjectInertia(SaveGame.Instance.SpellEffects), monster.Health / 6 > 200 ? 200 : monster.Health / 6,
                            0);
                        break;

                    // MonsterFlag4.BreatheGravity
                    case 96 + 23:
                        _saveGame.Disturb(true);
                        Profile.Instance.MsgPrint(!seeEither
                            ? "You hear breathing noise."
                            : $"{monsterName} breathes gravity at {targetName}");
                        BreatheAtMonster(monsterIndex, y, x, new ProjectGravity(SaveGame.Instance.SpellEffects), monster.Health / 3 > 200 ? 200 : monster.Health / 3,
                            0);
                        break;

                    // MonsterFlag4.BreatheShards
                    case 96 + 24:
                        _saveGame.Disturb(true);
                        Profile.Instance.MsgPrint(!seeEither
                            ? "You hear breathing noise."
                            : $"{monsterName} breathes shards at {targetName}");
                        BreatheAtMonster(monsterIndex, y, x, new ProjectExplode(SaveGame.Instance.SpellEffects), monster.Health / 6 > 400 ? 400 : monster.Health / 6, 0);
                        break;

                    // MonsterFlag4.BreathePlasma
                    case 96 + 25:
                        _saveGame.Disturb(true);
                        Profile.Instance.MsgPrint(!seeEither
                            ? "You hear breathing noise."
                            : $"{monsterName} breathes plasma at {targetName}");
                        BreatheAtMonster(monsterIndex, y, x, new ProjectPlasma(SaveGame.Instance.SpellEffects), monster.Health / 6 > 150 ? 150 : monster.Health / 6, 0);
                        break;

                    // MonsterFlag4.BreatheForce
                    case 96 + 26:
                        _saveGame.Disturb(true);
                        Profile.Instance.MsgPrint(!seeEither
                            ? "You hear breathing noise."
                            : $"{monsterName} breathes force at {targetName}");
                        BreatheAtMonster(monsterIndex, y, x, new ProjectForce(SaveGame.Instance.SpellEffects), monster.Health / 6 > 200 ? 200 : monster.Health / 6, 0);
                        break;

                    // MonsterFlag4.BreatheMana
                    case 96 + 27:
                        _saveGame.Disturb(true);
                        Profile.Instance.MsgPrint(!seeEither
                            ? "You hear breathing noise."
                            : $"{monsterName} breathes magical energy at {targetName}");
                        BreatheAtMonster(monsterIndex, y, x, new ProjectMana(SaveGame.Instance.SpellEffects), monster.Health / 3 > 250 ? 250 : monster.Health / 3, 0);
                        break;

                    // MonsterFlag4.RadiationBall
                    case 96 + 28:
                        _saveGame.Disturb(true);
                        Profile.Instance.MsgPrint(!seeEither
                            ? "You hear someone mumble."
                            : $"{monsterName} casts a ball of radiation at {targetName}");
                        BreatheAtMonster(monsterIndex, y, x, new ProjectNuke(SaveGame.Instance.SpellEffects), rlev + Program.Rng.DiceRoll(10, 6), 2);
                        break;

                    // MonsterFlag4.BreatheRadiation
                    case 96 + 29:
                        _saveGame.Disturb(true);
                        Profile.Instance.MsgPrint(!seeEither
                            ? "You hear breathing noise."
                            : $"{monsterName} breathes toxic waste at {targetName}");
                        BreatheAtMonster(monsterIndex, y, x, new ProjectNuke(SaveGame.Instance.SpellEffects), monster.Health / 3 > 800 ? 800 : monster.Health / 3, 0);
                        break;

                    // MonsterFlag4.ChaosBall
                    case 96 + 30:
                        _saveGame.Disturb(true);
                        Profile.Instance.MsgPrint(!seeEither
                            ? "You hear someone mumble frighteningly."
                            : $"{monsterName} invokes raw chaos upon {targetName}");
                        BreatheAtMonster(monsterIndex, y, x, new ProjectChaos(SaveGame.Instance.SpellEffects), (rlev * 2) + Program.Rng.DiceRoll(10, 10),
                            4);
                        break;

                    // MonsterFlag4.BreatheDisintegration
                    case 96 + 31:
                        _saveGame.Disturb(true);
                        Profile.Instance.MsgPrint(!seeEither
                            ? "You hear breathing noise."
                            : $"{monsterName} breathes disintegration at {targetName}");
                        BreatheAtMonster(monsterIndex, y, x, new ProjectDisintegrate(SaveGame.Instance.SpellEffects),
                            monster.Health / 3 > 300 ? 300 : monster.Health / 3, 0);
                        break;

                    // MonsterFlag5.AcidBall
                    case 128 + 0:
                        _saveGame.Disturb(true);
                        Profile.Instance.MsgPrint(!seeEither
                            ? "You hear someone mumble."
                            : $"{monsterName} casts an acid ball at {targetName}");
                        BreatheAtMonster(monsterIndex, y, x, new ProjectAcid(SaveGame.Instance.SpellEffects), Program.Rng.DieRoll(rlev * 3) + 15, 2);
                        break;

                    // MonsterFlag5.LightningBall
                    case 128 + 1:
                        _saveGame.Disturb(true);
                        Profile.Instance.MsgPrint(!seeEither
                            ? "You hear someone mumble."
                            : $"{monsterName} casts a lightning ball at {targetName}");
                        BreatheAtMonster(monsterIndex, y, x, new ProjectElec(SaveGame.Instance.SpellEffects), Program.Rng.DieRoll(rlev * 3 / 2) + 8, 2);
                        break;

                    // MonsterFlag5.FireBall
                    case 128 + 2:
                        _saveGame.Disturb(true);
                        Profile.Instance.MsgPrint(!seeEither
                            ? "You hear someone mumble."
                            : $"{monsterName} casts a fire ball at {targetName}");
                        BreatheAtMonster(monsterIndex, y, x, new ProjectFire(SaveGame.Instance.SpellEffects), Program.Rng.DieRoll(rlev * 7 / 2) + 10, 2);
                        break;

                    // MonsterFlag5.ColdBall
                    case 128 + 3:
                        _saveGame.Disturb(true);
                        Profile.Instance.MsgPrint(!seeEither
                            ? "You hear someone mumble."
                            : $"{monsterName} casts a frost ball at {targetName}");
                        BreatheAtMonster(monsterIndex, y, x, new ProjectCold(SaveGame.Instance.SpellEffects), Program.Rng.DieRoll(rlev * 3 / 2) + 10, 2);
                        break;

                    // MonsterFlag5.PoisonBall
                    case 128 + 4:
                        _saveGame.Disturb(true);
                        Profile.Instance.MsgPrint(!seeEither
                            ? "You hear someone mumble."
                            : $"{monsterName} casts a stinking cloud at {targetName}");
                        BreatheAtMonster(monsterIndex, y, x, new ProjectPois(SaveGame.Instance.SpellEffects), Program.Rng.DiceRoll(12, 2), 2);
                        break;

                    // MonsterFlag5.NetherBall
                    case 128 + 5:
                        _saveGame.Disturb(true);
                        Profile.Instance.MsgPrint(!seeEither
                            ? "You hear someone mumble."
                            : $"{monsterName} casts a nether ball at {targetName}");
                        BreatheAtMonster(monsterIndex, y, x, new ProjectNether(SaveGame.Instance.SpellEffects), 50 + Program.Rng.DiceRoll(10, 10) + rlev,
                            2);
                        break;

                    // MonsterFlag5.WaterBall
                    case 128 + 6:
                        _saveGame.Disturb(true);
                        Profile.Instance.MsgPrint(!seeEither
                            ? "You hear someone mumble."
                            : $"{monsterName} gestures fluidly at {targetName}");
                        Profile.Instance.MsgPrint($"{targetName} is engulfed in a whirlpool.");
                        BreatheAtMonster(monsterIndex, y, x, new ProjectWater(SaveGame.Instance.SpellEffects), Program.Rng.DieRoll(rlev * 5 / 2) + 50, 4);
                        break;

                    // MonsterFlag5.ManaBall
                    case 128 + 7:
                        _saveGame.Disturb(true);
                        Profile.Instance.MsgPrint(!seeEither
                            ? "You hear someone mumble powerfully."
                            : $"{monsterName} invokes a mana storm upon {targetName}");
                        BreatheAtMonster(monsterIndex, y, x, new ProjectMana(SaveGame.Instance.SpellEffects), (rlev * 5) + Program.Rng.DiceRoll(10, 10), 4);
                        break;

                    // MonsterFlag5.DarkBall
                    case 128 + 8:
                        _saveGame.Disturb(true);
                        Profile.Instance.MsgPrint(!seeEither
                            ? "You hear someone mumble powerfully."
                            : $"{monsterName} invokes a darkness storm upon {targetName}");
                        BreatheAtMonster(monsterIndex, y, x, new ProjectDark(SaveGame.Instance.SpellEffects), (rlev * 5) + Program.Rng.DiceRoll(10, 10), 4);
                        break;

                    // MonsterFlag5.DrainMana
                    case 128 + 9:
                        int r1 = (Program.Rng.DieRoll(rlev) / 2) + 1;
                        if (seeMonster)
                        {
                            Profile.Instance.MsgPrint($"{monsterName} draws psychic energy from {targetName}");
                        }
                        if (monster.Health < monster.MaxHealth)
                        {
                            if (!(targetRace.Flags4 != 0 || targetRace.Flags5 != 0 || targetRace.Flags6 != 0))
                            {
                                if (seeBoth)
                                {
                                    Profile.Instance.MsgPrint($"{targetName} is unaffected!");
                                }
                            }
                            else
                            {
                                monster.Health += 6 * r1;
                                if (monster.Health > monster.MaxHealth)
                                {
                                    monster.Health = monster.MaxHealth;
                                }
                                if (_saveGame.TrackedMonsterIndex == monsterIndex)
                                {
                                    _player.RedrawNeeded.Set(RedrawFlag.PrHealth);
                                }
                                if (seen)
                                {
                                    Profile.Instance.MsgPrint($"{monsterName} appears healthier.");
                                }
                            }
                        }
                        wakeUp = true;
                        break;

                    // MonsterFlag5.MindBlast
                    case 128 + 10:
                        _saveGame.Disturb(true);
                        if (seen)
                        {
                            Profile.Instance.MsgPrint($"{monsterName} gazes intently at {targetName}");
                        }
                        if ((targetRace.Flags1 & MonsterFlag1.Unique) != 0 || (targetRace.Flags3 & MonsterFlag3.ImmuneConfusion) != 0 ||
                            targetRace.Level > Program.Rng.DieRoll(rlev - 10 < 1 ? 1 : rlev - 10) + 10)
                        {
                            if ((targetRace.Flags3 & MonsterFlag3.ImmuneConfusion) != 0 && seen)
                            {
                                targetRace.Knowledge.RFlags3 |= MonsterFlag3.ImmuneConfusion;
                            }
                            if (seeTarget)
                            {
                                Profile.Instance.MsgPrint($"{targetName} is unaffected!");
                            }
                        }
                        else
                        {
                            Profile.Instance.MsgPrint($"{targetName} is blasted by psionic energy.");
                            target.ConfusionLevel += Program.Rng.RandomLessThan(4) + 4;
                            TakeDamageFromAnotherMonster(targetIndex, Program.Rng.DiceRoll(8, 8), out _, " collapses, a mindless husk.");
                        }
                        wakeUp = true;
                        break;

                    // MonsterFlag5.BrainSmash
                    case 128 + 11:
                        _saveGame.Disturb(true);
                        if (seen)
                        {
                            Profile.Instance.MsgPrint($"{monsterName} gazes intently at {targetName}");
                        }
                        if ((targetRace.Flags1 & MonsterFlag1.Unique) != 0 || (targetRace.Flags3 & MonsterFlag3.ImmuneConfusion) != 0 ||
                            targetRace.Level > Program.Rng.DieRoll(rlev - 10 < 1 ? 1 : rlev - 10) + 10)
                        {
                            if ((targetRace.Flags3 & MonsterFlag3.ImmuneConfusion) != 0 && seen)
                            {
                                targetRace.Knowledge.RFlags3 |= MonsterFlag3.ImmuneConfusion;
                            }
                            if (seeTarget)
                            {
                                Profile.Instance.MsgPrint($"{targetName} is unaffected!");
                            }
                        }
                        else
                        {
                            if (seeTarget)
                            {
                                Profile.Instance.MsgPrint($"{targetName} is blasted by psionic energy.");
                            }
                            target.ConfusionLevel += Program.Rng.RandomLessThan(4) + 4;
                            target.Speed -= Program.Rng.RandomLessThan(4) + 4;
                            target.StunLevel += Program.Rng.RandomLessThan(4) + 4;
                            TakeDamageFromAnotherMonster(targetIndex, Program.Rng.DiceRoll(12, 15), out _, " collapses, a mindless husk.");
                        }
                        wakeUp = true;
                        break;

                    // MonsterFlag5.CauseLightWounds
                    case 128 + 12:
                        _saveGame.Disturb(true);
                        if (blind || !seeMonster)
                        {
                            Profile.Instance.MsgPrint($"{monsterName} mumbles.");
                        }
                        else
                        {
                            Profile.Instance.MsgPrint($"{monsterName} points at {targetName} and curses.");
                        }
                        if (targetRace.Level > Program.Rng.DieRoll(rlev - 10 < 1 ? 1 : rlev - 10) + 10)
                        {
                            if (seeTarget)
                            {
                                Profile.Instance.MsgPrint($"{targetName} resists!");
                            }
                        }
                        else
                        {
                            TakeDamageFromAnotherMonster(targetIndex, Program.Rng.DiceRoll(3, 8), out _, " is destroyed.");
                        }
                        wakeUp = true;
                        break;

                    // MonsterFlag5.CauseSeriousWounds
                    case 128 + 13:
                        _saveGame.Disturb(true);
                        if (blind || !seeMonster)
                        {
                            Profile.Instance.MsgPrint($"{monsterName} mumbles.");
                        }
                        else
                        {
                            Profile.Instance.MsgPrint($"{monsterName} points at {targetName} and curses horribly.");
                        }
                        if (targetRace.Level > Program.Rng.DieRoll(rlev - 10 < 1 ? 1 : rlev - 10) + 10)
                        {
                            if (seeTarget)
                            {
                                Profile.Instance.MsgPrint($"{targetName} resists!");
                            }
                        }
                        else
                        {
                            TakeDamageFromAnotherMonster(targetIndex, Program.Rng.DiceRoll(8, 8), out _, " is destroyed.");
                        }
                        wakeUp = true;
                        break;

                    // MonsterFlag5.CauseCriticalWounds
                    case 128 + 14:
                        _saveGame.Disturb(true);
                        if (blind || !seeMonster)
                        {
                            Profile.Instance.MsgPrint($"{monsterName} mumbles.");
                        }
                        else
                        {
                            Profile.Instance.MsgPrint($"{monsterName} points at {targetName}, incanting terribly!");
                        }
                        if (targetRace.Level > Program.Rng.DieRoll(rlev - 10 < 1 ? 1 : rlev - 10) + 10)
                        {
                            if (seeTarget)
                            {
                                Profile.Instance.MsgPrint($"{targetName} resists!");
                            }
                        }
                        else
                        {
                            TakeDamageFromAnotherMonster(targetIndex, Program.Rng.DiceRoll(10, 15), out _, " is destroyed.");
                        }
                        wakeUp = true;
                        break;

                    // MonsterFlag5.CauseMortalWounds
                    case 128 + 15:
                        _saveGame.Disturb(true);
                        if (blind || !seeMonster)
                        {
                            Profile.Instance.MsgPrint($"{monsterName} mumbles.");
                        }
                        else
                        {
                            Profile.Instance.MsgPrint($"{monsterName} points at {targetName}, screaming the word 'DIE!'");
                        }
                        if (targetRace.Level > Program.Rng.DieRoll(rlev - 10 < 1 ? 1 : rlev - 10) + 10)
                        {
                            if (seeTarget)
                            {
                                Profile.Instance.MsgPrint($"{targetName} resists!");
                            }
                        }
                        else
                        {
                            TakeDamageFromAnotherMonster(targetIndex, Program.Rng.DiceRoll(15, 15), out _, " is destroyed.");
                        }
                        wakeUp = true;
                        break;

                    // MonsterFlag5.AcidBolt
                    case 128 + 16:
                        _saveGame.Disturb(true);
                        if (blind || !seeMonster)
                        {
                            Profile.Instance.MsgPrint($"{monsterName} mumbles.");
                        }
                        else
                        {
                            Profile.Instance.MsgPrint($"{monsterName} casts a acid bolt at {targetName}");
                        }
                        FireBoltAtMonster(monsterIndex, y, x, new ProjectAcid(SaveGame.Instance.SpellEffects), Program.Rng.DiceRoll(7, 8) + (rlev / 3));
                        break;

                    // MonsterFlag5.LightningBolt
                    case 128 + 17:
                        _saveGame.Disturb(true);
                        if (blind || !seeMonster)
                        {
                            Profile.Instance.MsgPrint($"{monsterName} mumbles.");
                        }
                        else
                        {
                            Profile.Instance.MsgPrint($"{monsterName} casts a lightning bolt at {targetName}");
                        }
                        FireBoltAtMonster(monsterIndex, y, x, new ProjectElec(SaveGame.Instance.SpellEffects), Program.Rng.DiceRoll(4, 8) + (rlev / 3));
                        break;

                    // MonsterFlag5.FireBolt
                    case 128 + 18:
                        _saveGame.Disturb(true);
                        if (blind || !seeMonster)
                        {
                            Profile.Instance.MsgPrint($"{monsterName} mumbles.");
                        }
                        else
                        {
                            Profile.Instance.MsgPrint($"{monsterName} casts a fire bolt at {targetName}");
                        }
                        FireBoltAtMonster(monsterIndex, y, x, new ProjectFire(SaveGame.Instance.SpellEffects), Program.Rng.DiceRoll(9, 8) + (rlev / 3));
                        break;

                    // MonsterFlag5.ColdBolt
                    case 128 + 19:
                        _saveGame.Disturb(true);
                        if (blind || !seeMonster)
                        {
                            Profile.Instance.MsgPrint($"{monsterName} mumbles.");
                        }
                        else
                        {
                            Profile.Instance.MsgPrint($"{monsterName} casts a frost bolt at {targetName}");
                        }
                        FireBoltAtMonster(monsterIndex, y, x, new ProjectCold(SaveGame.Instance.SpellEffects), Program.Rng.DiceRoll(6, 8) + (rlev / 3));
                        break;

                    // MonsterFlag5.PoisonBolt
                    case 128 + 20:
                        break;

                    // MonsterFlag5.NetherBolt
                    case 128 + 21:
                        _saveGame.Disturb(true);
                        if (blind || !seeMonster)
                        {
                            Profile.Instance.MsgPrint($"{monsterName} mumbles.");
                        }
                        else
                        {
                            Profile.Instance.MsgPrint($"{monsterName} casts a nether bolt at {targetName}");
                        }
                        FireBoltAtMonster(monsterIndex, y, x, new ProjectNether(SaveGame.Instance.SpellEffects),
                            30 + Program.Rng.DiceRoll(5, 5) + (rlev * 3 / 2));
                        break;

                    // MonsterFlag5.WaterBolt
                    case 128 + 22:
                        _saveGame.Disturb(true);
                        if (blind || !seeMonster)
                        {
                            Profile.Instance.MsgPrint($"{monsterName} mumbles.");
                        }
                        else
                        {
                            Profile.Instance.MsgPrint($"{monsterName} casts a water bolt at {targetName}");
                        }
                        FireBoltAtMonster(monsterIndex, y, x, new ProjectWater(SaveGame.Instance.SpellEffects), Program.Rng.DiceRoll(10, 10) + rlev);
                        break;

                    // MonsterFlag5.ManaBolt
                    case 128 + 23:
                        _saveGame.Disturb(true);
                        if (blind || !seeMonster)
                        {
                            Profile.Instance.MsgPrint($"{monsterName} mumbles.");
                        }
                        else
                        {
                            Profile.Instance.MsgPrint($"{monsterName} casts a mana bolt at {targetName}");
                        }
                        FireBoltAtMonster(monsterIndex, y, x, new ProjectMana(SaveGame.Instance.SpellEffects), Program.Rng.DieRoll(rlev * 7 / 2) + 50);
                        break;

                    // MonsterFlag5.PlasmaBolt
                    case 128 + 24:
                        _saveGame.Disturb(true);
                        if (blind || !seeMonster)
                        {
                            Profile.Instance.MsgPrint($"{monsterName} mumbles.");
                        }
                        else
                        {
                            Profile.Instance.MsgPrint($"{monsterName} casts a plasma bolt at {targetName}");
                        }
                        FireBoltAtMonster(monsterIndex, y, x, new ProjectPlasma(SaveGame.Instance.SpellEffects), 10 + Program.Rng.DiceRoll(8, 7) + rlev);
                        break;

                    // MonsterFlag5.IceBolt
                    case 128 + 25:
                        _saveGame.Disturb(true);
                        if (blind || !seeMonster)
                        {
                            Profile.Instance.MsgPrint($"{monsterName} mumbles.");
                        }
                        else
                        {
                            Profile.Instance.MsgPrint($"{monsterName} casts an ice bolt at {targetName}");
                        }
                        FireBoltAtMonster(monsterIndex, y, x, new ProjectIce(SaveGame.Instance.SpellEffects), Program.Rng.DiceRoll(6, 6) + rlev);
                        break;

                    // MonsterFlag5.MagicMissile
                    case 128 + 26:
                        _saveGame.Disturb(true);
                        if (blind || !seeMonster)
                        {
                            Profile.Instance.MsgPrint($"{monsterName} mumbles.");
                        }
                        else
                        {
                            Profile.Instance.MsgPrint($"{monsterName} casts a magic missile at {targetName}");
                        }
                        FireBoltAtMonster(monsterIndex, y, x, new ProjectMissile(SaveGame.Instance.SpellEffects), Program.Rng.DiceRoll(2, 6) + (rlev / 3));
                        break;

                    // MonsterFlag5.Scare
                    case 128 + 27:
                        _saveGame.Disturb(true);
                        if (blind || !seeMonster)
                        {
                            Profile.Instance.MsgPrint($"{monsterName} mumbles, and you hear scary noises.");
                        }
                        else
                        {
                            Profile.Instance.MsgPrint($"{monsterName} casts a fearful illusion at {targetName}");
                        }
                        if ((targetRace.Flags3 & MonsterFlag3.ImmuneFear) != 0)
                        {
                            if (seeTarget)
                            {
                                Profile.Instance.MsgPrint($"{targetName} refuses to be frightened.");
                            }
                        }
                        else if (targetRace.Level > Program.Rng.DieRoll(rlev - 10 < 1 ? 1 : rlev - 10) + 10)
                        {
                            if (seeTarget)
                            {
                                Profile.Instance.MsgPrint($"{targetName} refuses to be frightened.");
                            }
                        }
                        else
                        {
                            if (target.FearLevel == 0 && seeTarget)
                            {
                                Profile.Instance.MsgPrint($"{targetName} flees in terror!");
                            }
                            target.FearLevel += Program.Rng.RandomLessThan(4) + 4;
                        }
                        wakeUp = true;
                        break;

                    // MonsterFlag5.Blindness
                    case 128 + 28:
                        _saveGame.Disturb(true);
                        if (blind || !seeMonster)
                        {
                            Profile.Instance.MsgPrint($"{monsterName} mumbles.");
                        }
                        else
                        {
                            string it = targetName != "it" ? "s" : "'s";
                            Profile.Instance.MsgPrint($"{monsterName} casts a spell, burning {targetName}{it} eyes.");
                        }
                        if ((targetRace.Flags3 & MonsterFlag3.ImmuneConfusion) != 0)
                        {
                            if (seeTarget)
                            {
                                Profile.Instance.MsgPrint($"{targetName} is unaffected.");
                            }
                        }
                        else if (targetRace.Level > Program.Rng.DieRoll(rlev - 10 < 1 ? 1 : rlev - 10) + 10)
                        {
                            if (seeTarget)
                            {
                                Profile.Instance.MsgPrint($"{targetName} is unaffected.");
                            }
                        }
                        else
                        {
                            if (seeTarget)
                            {
                                Profile.Instance.MsgPrint($"{targetName} is blinded!");
                            }
                            target.ConfusionLevel += 12 + Program.Rng.RandomLessThan(4);
                        }
                        wakeUp = true;
                        break;

                    // MonsterFlag5.Confuse
                    case 128 + 29:
                        _saveGame.Disturb(true);
                        if (blind || !seeMonster)
                        {
                            Profile.Instance.MsgPrint($"{monsterName} mumbles, and you hear puzzling noises.");
                        }
                        else
                        {
                            Profile.Instance.MsgPrint($"{monsterName} creates a mesmerising illusion in front of {targetName}");
                        }
                        if ((targetRace.Flags3 & MonsterFlag3.ImmuneConfusion) != 0)
                        {
                            if (seeTarget)
                            {
                                Profile.Instance.MsgPrint($"{targetName} disbelieves the feeble spell.");
                            }
                        }
                        else if (targetRace.Level > Program.Rng.DieRoll(rlev - 10 < 1 ? 1 : rlev - 10) + 10)
                        {
                            if (seeTarget)
                            {
                                Profile.Instance.MsgPrint($"{targetName} disbelieves the feeble spell.");
                            }
                        }
                        else
                        {
                            if (seeTarget)
                            {
                                Profile.Instance.MsgPrint($"{targetName} seems confused.");
                            }
                            target.ConfusionLevel += 12 + Program.Rng.RandomLessThan(4);
                        }
                        wakeUp = true;
                        break;

                    // MonsterFlag5.Slow
                    case 128 + 30:
                        _saveGame.Disturb(true);
                        if (!blind && seeEither)
                        {
                            string it = targetName == "it" ? "s" : "'s";
                            Profile.Instance.MsgPrint($"{monsterName} drains power from {targetName}{it} muscles.");
                        }
                        if ((targetRace.Flags1 & MonsterFlag1.Unique) != 0)
                        {
                            if (seeTarget)
                            {
                                Profile.Instance.MsgPrint($"{targetName} is unaffected.");
                            }
                        }
                        else if (targetRace.Level > Program.Rng.DieRoll(rlev - 10 < 1 ? 1 : rlev - 10) + 10)
                        {
                            if (seeTarget)
                            {
                                Profile.Instance.MsgPrint($"{targetName} is unaffected.");
                            }
                        }
                        else
                        {
                            target.Speed -= 10;
                            if (seeTarget)
                            {
                                Profile.Instance.MsgPrint($"{targetName} starts moving slower.");
                            }
                        }
                        wakeUp = true;
                        break;

                    // MonsterFlag5.Hold
                    case 128 + 31:
                        _saveGame.Disturb(true);
                        if (!blind && seeMonster)
                        {
                            Profile.Instance.MsgPrint($"{monsterName} stares intently at {targetName}");
                        }
                        if ((targetRace.Flags1 & MonsterFlag1.Unique) != 0 || (targetRace.Flags3 & MonsterFlag3.ImmuneStun) != 0)
                        {
                            if (seeTarget)
                            {
                                Profile.Instance.MsgPrint($"{targetName} is unaffected.");
                            }
                        }
                        else if (targetRace.Level > Program.Rng.DieRoll(rlev - 10 < 1 ? 1 : rlev - 10) + 10)
                        {
                            if (seeTarget)
                            {
                                Profile.Instance.MsgPrint($"{targetName} is unaffected.");
                            }
                        }
                        else
                        {
                            target.StunLevel += Program.Rng.DieRoll(4) + 4;
                            if (seeTarget)
                            {
                                Profile.Instance.MsgPrint($"{targetName} is paralyzed!");
                            }
                        }
                        wakeUp = true;
                        break;

                    // MonsterFlag6.Haste
                    case 160 + 0:
                        _saveGame.Disturb(true);
                        if (blind || !seeMonster)
                        {
                            Profile.Instance.MsgPrint($"{monsterName} mumbles.");
                        }
                        else
                        {
                            Profile.Instance.MsgPrint($"{monsterName} concentrates on {monsterPossessive} body.");
                        }
                        if (monster.Speed < race.Speed + 10)
                        {
                            if (seeMonster)
                            {
                                Profile.Instance.MsgPrint($"{monsterName} starts moving faster.");
                            }
                            monster.Speed += 10;
                        }
                        else if (monster.Speed < race.Speed + 20)
                        {
                            if (seeMonster)
                            {
                                Profile.Instance.MsgPrint($"{monsterName} starts moving faster.");
                            }
                            monster.Speed += 2;
                        }
                        break;

                    // MonsterFlag6.DreadCurse
                    case 160 + 1:
                        _saveGame.Disturb(false);
                        Profile.Instance.MsgPrint(!seeMonster
                            ? "You hear someone invoke the Dread Curse of Azathoth!"
                            : $"{monsterName} invokes the Dread Curse of Azathoth on {targetName}");
                        if ((targetRace.Flags1 & MonsterFlag1.Unique) != 0)
                        {
                            if (!blind && seeTarget)
                            {
                                Profile.Instance.MsgPrint($"{targetName} is unaffected!");
                            }
                        }
                        else
                        {
                            if (race.Level + Program.Rng.DieRoll(20) > targetRace.Level + 10 + Program.Rng.DieRoll(20))
                            {
                                target.Health -= (65 + Program.Rng.DieRoll(25)) * target.Health / 100;
                                if (target.Health < 1)
                                {
                                    target.Health = 1;
                                }
                            }
                            else
                            {
                                if (seeTarget)
                                {
                                    Profile.Instance.MsgPrint($"{targetName} resists!");
                                }
                            }
                        }
                        wakeUp = true;
                        break;

                    // MonsterFlag6.Heal
                    case 160 + 2:
                        _saveGame.Disturb(true);
                        if (blind || !seeMonster)
                        {
                            Profile.Instance.MsgPrint($"{monsterName} mumbles.");
                        }
                        else
                        {
                            Profile.Instance.MsgPrint($"{monsterName} concentrates on {monsterPossessive} wounds.");
                        }
                        monster.Health += rlev * 6;
                        if (monster.Health >= monster.MaxHealth)
                        {
                            monster.Health = monster.MaxHealth;
                            Profile.Instance.MsgPrint(seen
                                ? $"{monsterName} looks completely healed!"
                                : $"{monsterName} sounds completely healed!");
                        }
                        else
                        {
                            Profile.Instance.MsgPrint(seen
                                ? $"{monsterName} looks healthier."
                                : $"{monsterName} sounds healthier.");
                        }
                        if (_saveGame.TrackedMonsterIndex == monsterIndex)
                        {
                            _player.RedrawNeeded.Set(RedrawFlag.PrHealth);
                        }
                        if (monster.FearLevel != 0)
                        {
                            monster.FearLevel = 0;
                            if (seeMonster)
                            {
                                Profile.Instance.MsgPrint($"{monsterName} recovers {monsterPossessive} courage.");
                            }
                        }
                        break;

                    // MonsterFlag6.Xxx2
                    case 160 + 3:
                        break;

                    // MonsterFlag6.Blink
                    case 160 + 4:
                        _saveGame.Disturb(true);
                        if (seeMonster)
                        {
                            Profile.Instance.MsgPrint($"{monsterName} blinks away.");
                        }
                        _saveGame.SpellEffects.TeleportAway(monsterIndex, 10);
                        break;

                    // MonsterFlag6.TeleportSelf
                    case 160 + 5:
                        _saveGame.Disturb(true);
                        if (seeMonster)
                        {
                            Profile.Instance.MsgPrint($"{monsterName} teleports away.");
                        }
                        _saveGame.SpellEffects.TeleportAway(monsterIndex, (Constants.MaxSight * 2) + 5);
                        break;

                    // MonsterFlag6.Xxx3
                    case 160 + 6:
                    // MonsterFlag6.Xxx4
                    case 160 + 7:
                    // MonsterFlag6.TeleportTo
                    case 160 + 8:
                        break;

                    // MonsterFlag6.TeleportAway
                    case 160 + 9:
                        bool resistsTele = false;
                        _saveGame.Disturb(true);
                        Profile.Instance.MsgPrint($"{monsterName} teleports {targetName} away.");
                        if ((targetRace.Flags3 & MonsterFlag3.ResistTeleport) != 0)
                        {
                            if ((targetRace.Flags1 & MonsterFlag1.Unique) != 0)
                            {
                                if (seeTarget)
                                {
                                    targetRace.Knowledge.RFlags3 |= MonsterFlag3.ResistTeleport;
                                    Profile.Instance.MsgPrint($"{targetName} is unaffected!");
                                }
                                resistsTele = true;
                            }
                            else if (targetRace.Level > Program.Rng.DieRoll(100))
                            {
                                if (seeTarget)
                                {
                                    targetRace.Knowledge.RFlags3 |= MonsterFlag3.ResistTeleport;
                                    Profile.Instance.MsgPrint($"{targetName} resists!");
                                }
                                resistsTele = true;
                            }
                        }
                        if (!resistsTele)
                        {
                            _saveGame.SpellEffects.TeleportAway(targetIndex, (Constants.MaxSight * 2) + 5);
                        }
                        break;

                    // MonsterFlag6.TeleportLevel
                    case 160 + 10:
                    // MonsterFlag6.Xxx5
                    case 160 + 11:
                        break;

                    // MonsterFlag6.Darkness
                    case 160 + 12:
                        _saveGame.Disturb(true);
                        Profile.Instance.MsgPrint(blind ? $"{monsterName} mumbles." : $"{monsterName} gestures in shadow.");
                        if (seen)
                        {
                            Profile.Instance.MsgPrint($"{targetName} is surrounded by darkness.");
                        }
                        _saveGame.SpellEffects.Project(monsterIndex, 3, y, x, 0, new ProjectDarkWeak(SaveGame.Instance.SpellEffects),
                            ProjectionFlag.ProjectGrid | ProjectionFlag.ProjectKill);
                        _saveGame.SpellEffects.UnlightRoom(y, x);
                        break;

                    // MonsterFlag6.CreateTraps
                    case 160 + 13:
                    // MonsterFlag6.Forget
                    case 160 + 14:
                    // MonsterFlag6.Xxx6
                    case 160 + 15:
                        break;

                    // MonsterFlag6.SummonKin
                    case 160 + 16:
                        _saveGame.Disturb(true);
                        if (blind || !seeMonster)
                        {
                            Profile.Instance.MsgPrint($"{monsterName} mumbles.");
                        }
                        else
                        {
                            string kin = (race.Flags1 & MonsterFlag1.Unique) != 0 ? "minions" : "kin";
                            Profile.Instance.MsgPrint($"{monsterName} magically summons {monsterPossessive} {kin}.");
                        }
                        _level.Monsters.SummonKinType = race.Character;
                        for (k = 0; k < 6; k++)
                        {
                            if (friendly)
                            {
                                if (_level.Monsters.SummonSpecificFriendly(y, x, rlev, Constants.SummonKin, true))
                                {
                                    count++;
                                }
                            }
                            else
                            {
                                if (_level.Monsters.SummonSpecific(y, x, rlev, Constants.SummonKin))
                                {
                                    count++;
                                }
                            }
                        }
                        if (blind && count != 0)
                        {
                            Profile.Instance.MsgPrint("You hear many things appear nearby.");
                        }
                        break;

                    // MonsterFlag6.SummonReaver
                    case 160 + 17:
                        _saveGame.Disturb(true);
                        if (blind || !seeMonster)
                        {
                            Profile.Instance.MsgPrint($"{monsterName} mumbles.");
                        }
                        else
                        {
                            Profile.Instance.MsgPrint($"{monsterName} magically summons Black Reavers!");
                        }
                        if (blind && count != 0)
                        {
                            Profile.Instance.MsgPrint("You hear heavy steps nearby.");
                        }
                        if (friendly)
                        {
                            _level.Monsters.SummonSpecificFriendly(y, x, rlev, Constants.SummonReaver, true);
                        }
                        else
                        {
                            _saveGame.SpellEffects.SummonReaver();
                        }
                        break;

                    // MonsterFlag6.SummonMonster
                    case 160 + 18:
                        _saveGame.Disturb(true);
                        if (blind || !seeMonster)
                        {
                            Profile.Instance.MsgPrint($"{monsterName} mumbles.");
                        }
                        else
                        {
                            Profile.Instance.MsgPrint($"{monsterName} magically summons help!");
                        }
                        for (k = 0; k < 1; k++)
                        {
                            if (friendly)
                            {
                                if (_level.Monsters.SummonSpecificFriendly(y, x, rlev, Constants.SummonNoUniques, true))
                                {
                                    count++;
                                }
                            }
                            else
                            {
                                if (_level.Monsters.SummonSpecific(y, x, rlev, 0))
                                {
                                    count++;
                                }
                            }
                        }
                        if (blind && count != 0)
                        {
                            Profile.Instance.MsgPrint("You hear something appear nearby.");
                        }
                        break;

                    // MonsterFlag6.SummonMonsters
                    case 160 + 19:
                        _saveGame.Disturb(true);
                        if (blind || !seeMonster)
                        {
                            Profile.Instance.MsgPrint($"{monsterName} mumbles.");
                        }
                        else
                        {
                            Profile.Instance.MsgPrint($"{monsterName} magically summons monsters!");
                        }
                        for (k = 0; k < 8; k++)
                        {
                            if (friendly)
                            {
                                if (_level.Monsters.SummonSpecificFriendly(y, x, rlev, Constants.SummonNoUniques, true))
                                {
                                    count++;
                                }
                            }
                            else
                            {
                                if (_level.Monsters.SummonSpecific(y, x, rlev, 0))
                                {
                                    count++;
                                }
                            }
                        }
                        if (blind && count != 0)
                        {
                            Profile.Instance.MsgPrint("You hear many things appear nearby.");
                        }
                        break;

                    // MonsterFlag6.SummonAnt
                    case 160 + 20:
                        _saveGame.Disturb(true);
                        if (blind || !seeMonster)
                        {
                            Profile.Instance.MsgPrint($"{monsterName} mumbles.");
                        }
                        else
                        {
                            Profile.Instance.MsgPrint($"{monsterName} magically summons ants.");
                        }
                        for (k = 0; k < 6; k++)
                        {
                            if (friendly)
                            {
                                if (_level.Monsters.SummonSpecificFriendly(y, x, rlev, Constants.SummonAnt, true))
                                {
                                    count++;
                                }
                            }
                            else
                            {
                                if (_level.Monsters.SummonSpecific(y, x, rlev, Constants.SummonAnt))
                                {
                                    count++;
                                }
                            }
                        }
                        if (blind && count != 0)
                        {
                            Profile.Instance.MsgPrint("You hear many things appear nearby.");
                        }
                        break;

                    // MonsterFlag6.SummonSpider
                    case 160 + 21:
                        _saveGame.Disturb(true);
                        if (blind || !seeMonster)
                        {
                            Profile.Instance.MsgPrint($"{monsterName} mumbles.");
                        }
                        else
                        {
                            Profile.Instance.MsgPrint($"{monsterName} magically summons spiders.");
                        }
                        for (k = 0; k < 6; k++)
                        {
                            if (friendly)
                            {
                                if (_level.Monsters.SummonSpecificFriendly(y, x, rlev, Constants.SummonSpider, true))
                                {
                                    count++;
                                }
                            }
                            else
                            {
                                if (_level.Monsters.SummonSpecific(y, x, rlev, Constants.SummonSpider))
                                {
                                    count++;
                                }
                            }
                        }
                        if (blind && count != 0)
                        {
                            Profile.Instance.MsgPrint("You hear many things appear nearby.");
                        }
                        break;

                    // MonsterFlag6.SummonHound
                    case 160 + 22:
                        _saveGame.Disturb(true);
                        if (blind || !seeMonster)
                        {
                            Profile.Instance.MsgPrint($"{monsterName} mumbles.");
                        }
                        else
                        {
                            Profile.Instance.MsgPrint($"{monsterName} magically summons hounds.");
                        }
                        for (k = 0; k < 6; k++)
                        {
                            if (friendly)
                            {
                                if (_level.Monsters.SummonSpecificFriendly(y, x, rlev, Constants.SummonHound, true))
                                {
                                    count++;
                                }
                            }
                            else
                            {
                                if (_level.Monsters.SummonSpecific(y, x, rlev, Constants.SummonHound))
                                {
                                    count++;
                                }
                            }
                        }
                        if (blind && count != 0)
                        {
                            Profile.Instance.MsgPrint("You hear many things appear nearby.");
                        }
                        break;

                    // MonsterFlag6.SummonHydra
                    case 160 + 23:
                        _saveGame.Disturb(true);
                        if (blind || !seeMonster)
                        {
                            Profile.Instance.MsgPrint($"{monsterName} mumbles.");
                        }
                        else
                        {
                            Profile.Instance.MsgPrint($"{monsterName} magically summons hydras.");
                        }
                        for (k = 0; k < 6; k++)
                        {
                            if (friendly)
                            {
                                if (_level.Monsters.SummonSpecificFriendly(y, x, rlev, Constants.SummonHydra, true))
                                {
                                    count++;
                                }
                            }
                            else
                            {
                                if (_level.Monsters.SummonSpecific(y, x, rlev, Constants.SummonHydra))
                                {
                                    count++;
                                }
                            }
                        }
                        if (blind && count != 0)
                        {
                            Profile.Instance.MsgPrint("You hear many things appear nearby.");
                        }
                        break;

                    // MonsterFlag6.SummonCthuloid
                    case 160 + 24:
                        _saveGame.Disturb(true);
                        if (blind || !seeMonster)
                        {
                            Profile.Instance.MsgPrint($"{monsterName} mumbles.");
                        }
                        else
                        {
                            Profile.Instance.MsgPrint($"{monsterName} magically summons a Cthuloid entity!");
                        }
                        for (k = 0; k < 1; k++)
                        {
                            if (friendly)
                            {
                                if (_level.Monsters.SummonSpecificFriendly(y, x, rlev, Constants.SummonCthuloid, true))
                                {
                                    count++;
                                }
                            }
                            else
                            {
                                if (_level.Monsters.SummonSpecific(y, x, rlev, Constants.SummonCthuloid))
                                {
                                    count++;
                                }
                            }
                        }
                        if (blind && count != 0)
                        {
                            Profile.Instance.MsgPrint("You hear something appear nearby.");
                        }
                        break;

                    // MonsterFlag6.SummonDemon
                    case 160 + 25:
                        _saveGame.Disturb(true);
                        if (blind || !seeMonster)
                        {
                            Profile.Instance.MsgPrint($"{monsterName} mumbles.");
                        }
                        else
                        {
                            Profile.Instance.MsgPrint($"{monsterName} magically summons a demon!");
                        }
                        for (k = 0; k < 1; k++)
                        {
                            if (friendly)
                            {
                                if (_level.Monsters.SummonSpecificFriendly(y, x, rlev, Constants.SummonDemon, true))
                                {
                                    count++;
                                }
                            }
                            else
                            {
                                if (_level.Monsters.SummonSpecific(y, x, rlev, Constants.SummonDemon))
                                {
                                    count++;
                                }
                            }
                        }
                        if (blind && count != 0)
                        {
                            Profile.Instance.MsgPrint("You hear something appear nearby.");
                        }
                        break;

                    // MonsterFlag6.SummonUndead
                    case 160 + 26:
                        _saveGame.Disturb(true);
                        if (blind || !seeMonster)
                        {
                            Profile.Instance.MsgPrint($"{monsterName} mumbles.");
                        }
                        else
                        {
                            Profile.Instance.MsgPrint($"{monsterName} magically summons an undead adversary!");
                        }
                        for (k = 0; k < 1; k++)
                        {
                            if (friendly)
                            {
                                if (_level.Monsters.SummonSpecificFriendly(y, x, rlev, Constants.SummonUndead, true))
                                {
                                    count++;
                                }
                            }
                            else
                            {
                                if (_level.Monsters.SummonSpecific(y, x, rlev, Constants.SummonUndead))
                                {
                                    count++;
                                }
                            }
                        }
                        if (blind && count != 0)
                        {
                            Profile.Instance.MsgPrint("You hear something appear nearby.");
                        }
                        break;

                    // MonsterFlag6.SummonDragon
                    case 160 + 27:
                        _saveGame.Disturb(true);
                        if (blind || !seeMonster)
                        {
                            Profile.Instance.MsgPrint($"{monsterName} mumbles.");
                        }
                        else
                        {
                            Profile.Instance.MsgPrint($"{monsterName} magically summons a dragon!");
                        }
                        for (k = 0; k < 1; k++)
                        {
                            if (friendly)
                            {
                                if (_level.Monsters.SummonSpecificFriendly(y, x, rlev, Constants.SummonDragon, true))
                                {
                                    count++;
                                }
                            }
                            else
                            {
                                if (_level.Monsters.SummonSpecific(y, x, rlev, Constants.SummonDragon))
                                {
                                    count++;
                                }
                            }
                        }
                        if (blind && count != 0)
                        {
                            Profile.Instance.MsgPrint("You hear something appear nearby.");
                        }
                        break;

                    // MonsterFlag6.SummonHiUndead
                    case 160 + 28:
                        _saveGame.Disturb(true);
                        if (blind || !seeMonster)
                        {
                            Profile.Instance.MsgPrint($"{monsterName} mumbles.");
                        }
                        else
                        {
                            Profile.Instance.MsgPrint($"{monsterName} magically summons greater undead!");
                        }
                        for (k = 0; k < 8; k++)
                        {
                            if (friendly)
                            {
                                if (_level.Monsters.SummonSpecificFriendly(y, x, rlev,
                                    Constants.SummonHiUndeadNoUniques, true))
                                {
                                    count++;
                                }
                            }
                            else
                            {
                                if (_level.Monsters.SummonSpecific(y, x, rlev, Constants.SummonHiUndead))
                                {
                                    count++;
                                }
                            }
                        }
                        if (blind && count != 0)
                        {
                            Profile.Instance.MsgPrint("You hear many creepy things appear nearby.");
                        }
                        break;

                    // MonsterFlag6.SummonHiDragon
                    case 160 + 29:
                        _saveGame.Disturb(true);
                        if (blind || !seeMonster)
                        {
                            Profile.Instance.MsgPrint($"{monsterName} mumbles.");
                        }
                        else
                        {
                            Profile.Instance.MsgPrint($"{monsterName} magically summons ancient dragons!");
                        }
                        for (k = 0; k < 8; k++)
                        {
                            if (friendly)
                            {
                                if (_level.Monsters.SummonSpecificFriendly(y, x, rlev,
                                    Constants.SummonHiDragonNoUniques, true))
                                {
                                    count++;
                                }
                            }
                            else
                            {
                                if (_level.Monsters.SummonSpecific(y, x, rlev, Constants.SummonHiDragon))
                                {
                                    count++;
                                }
                            }
                        }
                        if (blind && count != 0)
                        {
                            Profile.Instance.MsgPrint("You hear many powerful things appear nearby.");
                        }
                        break;

                    // MonsterFlag6.SummonGreatOldOne
                    case 160 + 30:
                        _saveGame.Disturb(true);
                        if (blind || !seeMonster)
                        {
                            Profile.Instance.MsgPrint($"{monsterName} mumbles.");
                        }
                        else
                        {
                            Profile.Instance.MsgPrint($"{monsterName} magically summons Great Old Ones!");
                        }
                        for (k = 0; k < 8; k++)
                        {
                            if (_level.Monsters.SummonSpecific(y, x, rlev, Constants.SummonGoo))
                            {
                                count++;
                            }
                        }
                        if (blind && count != 0)
                        {
                            Profile.Instance.MsgPrint("You hear immortal beings appear nearby.");
                        }
                        break;

                    // MonsterFlag6.SummonUnique
                    case 160 + 31:
                        _saveGame.Disturb(true);
                        if (blind || !seeMonster)
                        {
                            Profile.Instance.MsgPrint($"{monsterName} mumbles.");
                        }
                        else
                        {
                            Profile.Instance.MsgPrint($"{monsterName} magically summons special opponents!");
                        }
                        for (k = 0; k < 8; k++)
                        {
                            if (!friendly)
                            {
                                if (_level.Monsters.SummonSpecific(y, x, rlev, Constants.SummonUnique))
                                {
                                    count++;
                                }
                            }
                        }
                        for (k = 0; k < 8; k++)
                        {
                            if (friendly)
                            {
                                if (_level.Monsters.SummonSpecificFriendly(y, x, rlev,
                                    Constants.SummonHiUndeadNoUniques, true))
                                {
                                    count++;
                                }
                            }
                            else
                            {
                                if (_level.Monsters.SummonSpecific(y, x, rlev, Constants.SummonHiUndead))
                                {
                                    count++;
                                }
                            }
                        }
                        if (blind && count != 0)
                        {
                            Profile.Instance.MsgPrint("You hear many powerful things appear nearby.");
                        }
                        break;
                }
                // Most spells will wake up the target if it's asleep
                if (wakeUp)
                {
                    target.SleepLevel = 0;
                }
                // If the player saw us cast the spell, let them learn we can do that
                if (seen)
                {
                    if (thrownSpell < 32 * 4)
                    {
                        race.Knowledge.RFlags4 |= 1u << (thrownSpell - (32 * 3));
                        if (race.Knowledge.RCastInate < Constants.MaxUchar)
                        {
                            race.Knowledge.RCastInate++;
                        }
                    }
                    else if (thrownSpell < 32 * 5)
                    {
                        race.Knowledge.RFlags5 |= 1u << (thrownSpell - (32 * 4));
                        if (race.Knowledge.RCastSpell < Constants.MaxUchar)
                        {
                            race.Knowledge.RCastSpell++;
                        }
                    }
                    else if (thrownSpell < 32 * 6)
                    {
                        race.Knowledge.RFlags6 |= 1u << (thrownSpell - (32 * 5));
                        if (race.Knowledge.RCastSpell < Constants.MaxUchar)
                        {
                            race.Knowledge.RCastSpell++;
                        }
                    }
                }
                // If we killed the player, let their descendants remember that
                if (_player.IsDead && race.Knowledge.RDeaths < Constants.MaxShort)
                {
                    race.Knowledge.RDeaths++;
                }
                // We did cast a spell
                return true;
            }
            return false;
        }

        /// <summary>
        /// Choose and use an attack spell to cast when in combat with the player
        /// </summary>
        /// <param name="monsterIndex"> The index of the monster </param>
        /// <returns> True if a spell was cast, false if not </returns>
        private bool TryCastingASpellAgainstPlayer(int monsterIndex)
        {
            int k;
            int[] spell = new int[96];
            int num = 0;
            Monster monster = _level.Monsters[monsterIndex];
            MonsterRace race = monster.Race;
            bool noInnate = false;
            int playerX = _player.MapX;
            int playerY = _player.MapY;
            int count = 0;
            bool playerIsBlind = _player.TimedBlindness != 0;
            bool seenByPlayer = !playerIsBlind && monster.IsVisible;
            // No spells if we're confused
            if (monster.ConfusionLevel != 0)
            {
                return false;
            }
            // No spells if we're being nice because we've just been summoned
            if ((monster.IndividualMonsterFlags & Constants.MflagNice) != 0)
            {
                return false;
            }
            // No spells on the player if they're our friend
            if ((monster.Mind & Constants.SmFriendly) != 0)
            {
                return false;
            }
            // We aren't guaranteed to cast a spell even if we can
            int chance = (race.FreqInate + race.FreqSpell) / 2;
            if (chance == 0)
            {
                return false;
            }
            if (Program.Rng.RandomLessThan(100) >= chance)
            {
                return false;
            }
            // Innate abilities are inherently less likely than actual spells
            if (Program.Rng.RandomLessThan(100) >= chance * 2)
            {
                noInnate = true;
            }
            // If we're too far from the player don't cast a spell
            if (monster.DistanceFromPlayer > Constants.MaxRange)
            {
                return false;
            }
            // If we have no line of sight to the player, don't cast a spell
            if (!_level.Projectable(monster.MapY, monster.MapX, _player.MapY, _player.MapX))
            {
                return false;
            }
            // Gather together the abilities we have
            int monsterLevel = race.Level >= 1 ? race.Level : 1;
            uint f4 = race.Flags4;
            uint f5 = race.Flags5;
            uint f6 = race.Flags6;
            // If we're not allowed innate abilities, then things on Flags4 can't be used
            if (noInnate)
            {
                f4 = 0;
            }
            // If we're smart and badly injured, we may want to prioritise spells that disable the
            // player, summon help, or let us escape over spells that do direct damage
            if ((race.Flags2 & MonsterFlag2.Smart) != 0 && monster.Health < monster.MaxHealth / 10 &&
                Program.Rng.RandomLessThan(100) < 50)
            {
                f4 &= MonsterFlag4.IntMask;
                f5 &= MonsterFlag5.IntMask;
                f6 &= MonsterFlag6.IntMask;
                // If we just got rid of all our spells then don't cast
                if (f4 == 0 && f5 == 0 && f6 == 0)
                {
                    return false;
                }
            }
            // Ditch any spells that we've seen the player resist before so we know they'll be ineffective
            RemoveIneffectiveSpells(monsterIndex, out f4, f4, out f5, f5, out f6, f6);
            // If we just got rid of all our spells then don't cast
            if (f4 == 0 && f5 == 0 && f6 == 0)
            {
                return false;
            }
            // If we don't have a clean shot, and we're stupid, remove bolt spells
            if (((f4 & MonsterFlag4.BoltMask) != 0 || (f5 & MonsterFlag5.BoltMask) != 0 ||
                 (f6 & MonsterFlag6.BoltMask) != 0) && (race.Flags2 & MonsterFlag2.Stupid) == 0 &&
                !CleanShot(monster.MapY, monster.MapX, _player.MapY, _player.MapX))
            {
                f4 &= ~MonsterFlag4.BoltMask;
                f5 &= ~MonsterFlag5.BoltMask;
                f6 &= ~MonsterFlag6.BoltMask;
            }
            // If there's nowhere around the player to put a summoned creature, then remove
            // summoning spells
            if (((f4 & MonsterFlag4.SummonMask) != 0 || (f5 & MonsterFlag5.SummonMask) != 0 ||
                 (f6 & MonsterFlag6.SummonMask) != 0) && (race.Flags2 & MonsterFlag2.Stupid) == 0 &&
                !SummonPossible(_player.MapY, _player.MapX))
            {
                f4 &= ~MonsterFlag4.SummonMask;
                f5 &= ~MonsterFlag5.SummonMask;
                f6 &= ~MonsterFlag6.SummonMask;
            }
            // If we've removed all our spells, don't cast anything
            if (f4 == 0 && f5 == 0 && f6 == 0)
            {
                return false;
            }
            // Gather up all the spells that are left and put them in an array Flags4 spells are 96
            // + bit number (0-31)
            for (k = 0; k < 32; k++)
            {
                if ((f4 & (1u << k)) != 0)
                {
                    spell[num++] = k + (32 * 3);
                }
            }
            // Flags5 spells are 128 + bit number (0-31)
            for (k = 0; k < 32; k++)
            {
                if ((f5 & (1u << k)) != 0)
                {
                    spell[num++] = k + (32 * 4);
                }
            }
            // Flags6 spells are 160 + bit number (0-31)
            for (k = 0; k < 32; k++)
            {
                if ((f6 & (1 << k)) != 0)
                {
                    spell[num++] = k + (32 * 5);
                }
            }
            // If we ended up with no spells, don't cast
            if (num == 0)
            {
                return false;
            }
            // If the player's already dead or off the level, don't cast
            if (!_saveGame.Playing || _player.IsDead || _saveGame.NewLevelFlag)
            {
                return false;
            }
            string monsterName = monster.MonsterDesc(0x00);
            string monsterPossessive = monster.MonsterDesc(0x22);
            string monsterDescription = monster.MonsterDesc(0x88);
            // Pick one of our spells to cast, based on our priorities
            int thrownSpell = ChooseSpellAgainstPlayer(monsterIndex, spell, num);
            // If we decided not to cast, don't
            if (thrownSpell == 0)
            {
                return false;
            }
            // Stupid monsters may fail spells
            int failrate = 25 - ((monsterLevel + 3) / 4);
            if ((race.Flags2 & MonsterFlag2.Stupid) != 0)
            {
                failrate = 0;
            }
            // Only check for actual spells - nothing is so stupid it fails to breathe
            if (thrownSpell >= 128 && Program.Rng.RandomLessThan(100) < failrate)
            {
                Profile.Instance.MsgPrint($"{monsterName} tries to cast a spell, but fails.");
                return true;
            }
            // Now do whatever the actual spell does
            switch (thrownSpell)
            {
                // MonsterFlag4.Shriek
                case 96 + 0:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint($"{monsterName} makes a high pitched shriek.");
                    _saveGame.SpellEffects.AggravateMonsters(monsterIndex);
                    break;

                // MonsterFlag4.Xxx2
                case 96 + 1:
                // MonsterFlag4.Xxx3
                case 96 + 2:
                    break;

                // MonsterFlag4.ShardBall
                case 96 + 3:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} mumbles something." : $"{monsterName} fires a shard ball.");
                    FireBallAtPlayer(monsterIndex, new ProjectShard(SaveGame.Instance.SpellEffects), monster.Health / 4 > 800 ? 800 : monster.Health / 4, 2);
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsShard);
                    break;

                // MonsterFlag4.Arrow1D6
                case 96 + 4:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} makes a strange noise." : $"{monsterName} fires an arrow.");
                    FireBoltAtPlayer(monsterIndex, new ProjectArrow(SaveGame.Instance.SpellEffects), Program.Rng.DiceRoll(1, 6));
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsReflect);
                    break;

                // MonsterFlag4.Arrow3D6
                case 96 + 5:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} makes a strange noise." : $"{monsterName} fires an arrow!");
                    FireBoltAtPlayer(monsterIndex, new ProjectArrow(SaveGame.Instance.SpellEffects), Program.Rng.DiceRoll(3, 6));
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsReflect);
                    break;

                // MonsterFlag4.Arrow5D6
                case 96 + 6:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName}s makes a strange noise." : $"{monsterName} fires a missile.");
                    FireBoltAtPlayer(monsterIndex, new ProjectArrow(SaveGame.Instance.SpellEffects), Program.Rng.DiceRoll(5, 6));
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsReflect);
                    break;

                // MonsterFlag4.Arrow7D6
                case 96 + 7:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} makes a strange noise." : $"{monsterName} fires a missile!");
                    FireBoltAtPlayer(monsterIndex, new ProjectArrow(SaveGame.Instance.SpellEffects), Program.Rng.DiceRoll(7, 6));
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsReflect);
                    break;

                // MonsterFlag4.BreatheAcid
                case 96 + 8:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} breathes." : $"{monsterName} breathes acid.");
                    BreatheAtPlayer(monsterIndex, new ProjectAcid(SaveGame.Instance.SpellEffects), monster.Health / 3 > 1600 ? 1600 : monster.Health / 3, 0);
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsAcid);
                    break;

                // MonsterFlag4.BreatheLightning
                case 96 + 9:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} breathes." : $"{monsterName} breathes lightning.");
                    BreatheAtPlayer(monsterIndex, new ProjectElec(SaveGame.Instance.SpellEffects), monster.Health / 3 > 1600 ? 1600 : monster.Health / 3, 0);
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsElec);
                    break;

                // MonsterFlag4.BreatheFire
                case 96 + 10:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} breathes." : $"{monsterName} breathes fire.");
                    BreatheAtPlayer(monsterIndex, new ProjectFire(SaveGame.Instance.SpellEffects), monster.Health / 3 > 1600 ? 1600 : monster.Health / 3, 0);
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsFire);
                    break;

                // MonsterFlag4.BreatheCold
                case 96 + 11:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} breathes." : $"{monsterName} breathes frost.");
                    BreatheAtPlayer(monsterIndex, new ProjectCold(SaveGame.Instance.SpellEffects), monster.Health / 3 > 1600 ? 1600 : monster.Health / 3, 0);
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsCold);
                    break;

                // MonsterFlag4.BreathePoison
                case 96 + 12:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} breathes." : $"{monsterName} breathes gas.");
                    BreatheAtPlayer(monsterIndex, new ProjectPois(SaveGame.Instance.SpellEffects), monster.Health / 3 > 800 ? 800 : monster.Health / 3, 0);
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsPois);
                    break;

                // MonsterFlag4.BreatheNether
                case 96 + 13:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} breathes." : $"{monsterName} breathes nether.");
                    BreatheAtPlayer(monsterIndex, new ProjectNether(SaveGame.Instance.SpellEffects), monster.Health / 6 > 550 ? 550 : monster.Health / 6, 0);
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsNeth);
                    break;

                // MonsterFlag4.BreatheLight
                case 96 + 14:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} breathes." : $"{monsterName} breathes light.");
                    BreatheAtPlayer(monsterIndex, new ProjectLight(SaveGame.Instance.SpellEffects), monster.Health / 6 > 400 ? 400 : monster.Health / 6, 0);
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsLight);
                    break;

                // MonsterFlag4.BreatheDark
                case 96 + 15:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} breathes." : $"{monsterName} breathes darkness.");
                    BreatheAtPlayer(monsterIndex, new ProjectDark(SaveGame.Instance.SpellEffects), monster.Health / 6 > 400 ? 400 : monster.Health / 6, 0);
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsDark);
                    break;

                // MonsterFlag4.BreatheConfusion
                case 96 + 16:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} breathes." : $"{monsterName} breathes confusion.");
                    BreatheAtPlayer(monsterIndex, new ProjectConfusion(SaveGame.Instance.SpellEffects), monster.Health / 6 > 400 ? 400 : monster.Health / 6, 0);
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsConf);
                    break;

                // MonsterFlag4.BreatheSound
                case 96 + 17:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} breathes." : $"{monsterName} breathes sound.");
                    BreatheAtPlayer(monsterIndex, new ProjectSound(SaveGame.Instance.SpellEffects), monster.Health / 6 > 400 ? 400 : monster.Health / 6, 0);
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsSound);
                    break;

                // MonsterFlag4.BreatheChaos
                case 96 + 18:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} breathes." : $"{monsterName} breathes chaos.");
                    BreatheAtPlayer(monsterIndex, new ProjectChaos(SaveGame.Instance.SpellEffects), monster.Health / 6 > 600 ? 600 : monster.Health / 6, 0);
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsChaos);
                    break;

                // MonsterFlag4.BreatheDisenchant
                case 96 + 19:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} breathes." : $"{monsterName} breathes disenchantment.");
                    BreatheAtPlayer(monsterIndex, new ProjectDisenchant(SaveGame.Instance.SpellEffects), monster.Health / 6 > 500 ? 500 : monster.Health / 6, 0);
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsDisen);
                    break;

                // MonsterFlag4.BreatheNexus
                case 96 + 20:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} breathes." : $"{monsterName} breathes nexus.");
                    BreatheAtPlayer(monsterIndex, new ProjectNexus(SaveGame.Instance.SpellEffects), monster.Health / 3 > 250 ? 250 : monster.Health / 3, 0);
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsNexus);
                    break;

                // MonsterFlag4.BreatheTime
                case 96 + 21:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} breathes." : $"{monsterName} breathes time.");
                    BreatheAtPlayer(monsterIndex, new ProjectTime(SaveGame.Instance.SpellEffects), monster.Health / 3 > 150 ? 150 : monster.Health / 3, 0);
                    break;

                // MonsterFlag4.BreatheInertia
                case 96 + 22:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} breathes." : $"{monsterName} breathes inertia.");
                    BreatheAtPlayer(monsterIndex, new ProjectInertia(SaveGame.Instance.SpellEffects), monster.Health / 6 > 200 ? 200 : monster.Health / 6, 0);
                    break;

                // MonsterFlag4.BreatheGravity
                case 96 + 23:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} breathes." : $"{monsterName} breathes gravity.");
                    BreatheAtPlayer(monsterIndex, new ProjectGravity(SaveGame.Instance.SpellEffects), monster.Health / 3 > 200 ? 200 : monster.Health / 3, 0);
                    break;

                // MonsterFlag4.BreatheShards
                case 96 + 24:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} breathes." : $"{monsterName} breathes shards.");
                    BreatheAtPlayer(monsterIndex, new ProjectExplode(SaveGame.Instance.SpellEffects), monster.Health / 6 > 400 ? 400 : monster.Health / 6, 0);
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsShard);
                    break;

                // MonsterFlag4.BreathePlasma
                case 96 + 25:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} breathes." : $"{monsterName} breathes plasma.");
                    BreatheAtPlayer(monsterIndex, new ProjectPlasma(SaveGame.Instance.SpellEffects), monster.Health / 6 > 150 ? 150 : monster.Health / 6, 0);
                    break;

                // MonsterFlag4.BreatheForce
                case 96 + 26:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} breathes." : $"{monsterName} breathes force.");
                    BreatheAtPlayer(monsterIndex, new ProjectForce(SaveGame.Instance.SpellEffects), monster.Health / 6 > 200 ? 200 : monster.Health / 6, 0);
                    break;

                // MonsterFlag4.BreatheMana
                case 96 + 27:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} breathes." : $"{monsterName} breathes magical energy.");
                    BreatheAtPlayer(monsterIndex, new ProjectMana(SaveGame.Instance.SpellEffects), monster.Health / 3 > 250 ? 250 : monster.Health / 3, 0);
                    break;

                // MonsterFlag4.RadiationBall
                case 96 + 28:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} mumbles." : $"{monsterName} casts a ball of radiation.");
                    FireBallAtPlayer(monsterIndex, new ProjectNuke(SaveGame.Instance.SpellEffects), monsterLevel + Program.Rng.DiceRoll(10, 6), 2);
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsPois);
                    break;

                // MonsterFlag4.BreatheRadiation
                case 96 + 29:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} breathes." : $"{monsterName} breathes toxic waste.");
                    BreatheAtPlayer(monsterIndex, new ProjectNuke(SaveGame.Instance.SpellEffects), monster.Health / 3 > 800 ? 800 : monster.Health / 3, 0);
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsPois);
                    break;

                // MonsterFlag4.ChaosBall
                case 96 + 30:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind
                        ? $"{monsterName} mumbles frighteningly."
                        : $"{monsterName} invokes raw chaos.");
                    FireBallAtPlayer(monsterIndex, new ProjectChaos(SaveGame.Instance.SpellEffects), (monsterLevel * 2) + Program.Rng.DiceRoll(10, 10), 4);
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsChaos);
                    break;

                // MonsterFlag4.BreatheDisintegration
                case 96 + 31:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} breathes." : $"{monsterName} breathes disintegration.");
                    BreatheAtPlayer(monsterIndex, new ProjectDisintegrate(SaveGame.Instance.SpellEffects), monster.Health / 3 > 300 ? 300 : monster.Health / 3, 0);
                    break;

                // MonsterFlag5.AcidBall
                case 128 + 0:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} mumbles." : $"{monsterName} casts an acid ball.");
                    FireBallAtPlayer(monsterIndex, new ProjectAcid(SaveGame.Instance.SpellEffects), Program.Rng.DieRoll(monsterLevel * 3) + 15, 2);
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsAcid);
                    break;

                // MonsterFlag5.LightningBall
                case 128 + 1:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} mumbles." : $"{monsterName} casts a lightning ball.");
                    FireBallAtPlayer(monsterIndex, new ProjectElec(SaveGame.Instance.SpellEffects), Program.Rng.DieRoll(monsterLevel * 3 / 2) + 8, 2);
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsElec);
                    break;

                // MonsterFlag5.FireBall
                case 128 + 2:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} mumbles." : $"{monsterName} casts a fire ball.");
                    FireBallAtPlayer(monsterIndex, new ProjectFire(SaveGame.Instance.SpellEffects), Program.Rng.DieRoll(monsterLevel * 7 / 2) + 10, 2);
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsFire);
                    break;

                // MonsterFlag5.ColdBall
                case 128 + 3:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} mumbles." : $"{monsterName} casts a frost ball.");
                    FireBallAtPlayer(monsterIndex, new ProjectCold(SaveGame.Instance.SpellEffects), Program.Rng.DieRoll(monsterLevel * 3 / 2) + 10, 2);
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsCold);
                    break;

                // MonsterFlag5.PoisonBall
                case 128 + 4:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} mumbles." : $"{monsterName} casts a stinking cloud.");
                    FireBallAtPlayer(monsterIndex, new ProjectPois(SaveGame.Instance.SpellEffects), Program.Rng.DiceRoll(12, 2), 2);
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsPois);
                    break;

                // MonsterFlag5.NetherBall
                case 128 + 5:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} mumbles." : $"{monsterName} casts a nether ball.");
                    FireBallAtPlayer(monsterIndex, new ProjectNether(SaveGame.Instance.SpellEffects), 50 + Program.Rng.DiceRoll(10, 10) + monsterLevel, 2);
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsNeth);
                    break;

                // MonsterFlag5.WaterBall
                case 128 + 6:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} mumbles." : $"{monsterName} gestures fluidly.");
                    Profile.Instance.MsgPrint("You are engulfed in a whirlpool.");
                    FireBallAtPlayer(monsterIndex, new ProjectWater(SaveGame.Instance.SpellEffects), Program.Rng.DieRoll(monsterLevel * 5 / 2) + 50, 4);
                    break;

                // MonsterFlag5.ManaBall
                case 128 + 7:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind
                        ? $"{monsterName} mumbles powerfully."
                        : $"{monsterName} invokes a mana storm.");
                    FireBallAtPlayer(monsterIndex, new ProjectMana(SaveGame.Instance.SpellEffects), (monsterLevel * 5) + Program.Rng.DiceRoll(10, 10), 4);
                    break;

                // MonsterFlag5.DarkBall
                case 128 + 8:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind
                        ? $"{monsterName} mumbles powerfully."
                        : $"{monsterName} invokes a darkness storm.");
                    FireBallAtPlayer(monsterIndex, new ProjectDark(SaveGame.Instance.SpellEffects), (monsterLevel * 5) + Program.Rng.DiceRoll(10, 10), 4);
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsDark);
                    break;

                // MonsterFlag5.DrainMana
                case 128 + 9:
                    if (_player.Mana != 0)
                    {
                        _saveGame.Disturb(true);
                        Profile.Instance.MsgPrint($"{monsterName} draws psychic energy from you!");
                        int r1 = (Program.Rng.DieRoll(monsterLevel) / 2) + 1;
                        if (r1 >= _player.Mana)
                        {
                            r1 = _player.Mana;
                            _player.Mana = 0;
                            _player.FractionalMana = 0;
                        }
                        else
                        {
                            _player.Mana -= r1;
                        }
                        _player.RedrawNeeded.Set(RedrawFlag.PrMana);
                        if (monster.Health < monster.MaxHealth)
                        {
                            monster.Health += 6 * r1;
                            if (monster.Health > monster.MaxHealth)
                            {
                                monster.Health = monster.MaxHealth;
                            }
                            if (_saveGame.TrackedMonsterIndex == monsterIndex)
                            {
                                _player.RedrawNeeded.Set(RedrawFlag.PrHealth);
                            }
                            if (seenByPlayer)
                            {
                                Profile.Instance.MsgPrint($"{monsterName} appears healthier.");
                            }
                        }
                    }
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsMana);
                    break;

                // MonsterFlag5.MindBlast
                case 128 + 10:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(!seenByPlayer
                        ? "You feel something focusing on your mind."
                        : $"{monsterName} gazes deep into your eyes.");
                    if (Program.Rng.RandomLessThan(100) < _player.SkillSavingThrow)
                    {
                        Profile.Instance.MsgPrint("You resist the effects!");
                    }
                    else
                    {
                        Profile.Instance.MsgPrint("Your mind is blasted by psionic energy.");
                        if (!_player.HasConfusionResistance)
                        {
                            _player.SetTimedConfusion(_player.TimedConfusion + Program.Rng.RandomLessThan(4) + 4);
                        }
                        if (!_player.HasChaosResistance && Program.Rng.DieRoll(3) == 1)
                        {
                            _player.SetTimedHallucinations(_player.TimedHallucinations + Program.Rng.RandomLessThan(250) + 150);
                        }
                        _player.TakeHit(Program.Rng.DiceRoll(8, 8), monsterDescription);
                    }
                    break;

                // MonsterFlag5.BrainSmash
                case 128 + 11:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(!seenByPlayer
                        ? "You feel something focusing on your mind."
                        : $"{monsterName} looks deep into your eyes.");
                    if (Program.Rng.RandomLessThan(100) < _player.SkillSavingThrow)
                    {
                        Profile.Instance.MsgPrint("You resist the effects!");
                    }
                    else
                    {
                        Profile.Instance.MsgPrint("Your mind is blasted by psionic energy.");
                        _player.TakeHit(Program.Rng.DiceRoll(12, 15), monsterDescription);
                        if (!_player.HasBlindnessResistance)
                        {
                            _player.SetTimedBlindness(_player.TimedBlindness + 8 + Program.Rng.RandomLessThan(8));
                        }
                        if (!_player.HasConfusionResistance)
                        {
                            _player.SetTimedConfusion(_player.TimedConfusion + Program.Rng.RandomLessThan(4) + 4);
                        }
                        if (!_player.HasFreeAction)
                        {
                            _player.SetTimedParalysis(_player.TimedParalysis + Program.Rng.RandomLessThan(4) + 4);
                        }
                        _player.SetTimedSlow(_player.TimedSlow + Program.Rng.RandomLessThan(4) + 4);
                        while (Program.Rng.RandomLessThan(100) > _player.SkillSavingThrow)
                        {
                            _player.TryDecreasingAbilityScore(Ability.Intelligence);
                        }
                        while (Program.Rng.RandomLessThan(100) > _player.SkillSavingThrow)
                        {
                            _player.TryDecreasingAbilityScore(Ability.Wisdom);
                        }
                        if (!_player.HasChaosResistance)
                        {
                            _player.SetTimedHallucinations(_player.TimedHallucinations + Program.Rng.RandomLessThan(250) + 150);
                        }
                    }
                    break;

                // MonsterFlag5.CauseLightWounds
                case 128 + 12:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} mumbles." : $"{monsterName} points at you and curses.");
                    if (Program.Rng.RandomLessThan(100) < _player.SkillSavingThrow)
                    {
                        Profile.Instance.MsgPrint("You resist the effects!");
                    }
                    else
                    {
                        _player.CurseEquipment(33, 0);
                        _player.TakeHit(Program.Rng.DiceRoll(3, 8), monsterDescription);
                    }
                    break;

                // MonsterFlag5.CauseSeriousWounds
                case 128 + 13:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind
                        ? $"{monsterName} mumbles."
                        : $"{monsterName} points at you and curses horribly.");
                    if (Program.Rng.RandomLessThan(100) < _player.SkillSavingThrow)
                    {
                        Profile.Instance.MsgPrint("You resist the effects!");
                    }
                    else
                    {
                        _player.CurseEquipment(50, 5);
                        _player.TakeHit(Program.Rng.DiceRoll(8, 8), monsterDescription);
                    }
                    break;

                // MonsterFlag5.CauseCriticalWounds
                case 128 + 14:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind
                        ? $"{monsterName} mumbles loudly."
                        : $"{monsterName} points at you, incanting terribly!");
                    if (Program.Rng.RandomLessThan(100) < _player.SkillSavingThrow)
                    {
                        Profile.Instance.MsgPrint("You resist the effects!");
                    }
                    else
                    {
                        _player.CurseEquipment(80, 15);
                        _player.TakeHit(Program.Rng.DiceRoll(10, 15), monsterDescription);
                    }
                    break;

                // MonsterFlag5.CauseMortalWounds
                case 128 + 15:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind
                        ? $"{monsterName} screams the word 'DIE!'"
                        : $"{monsterName} points at you, screaming the word DIE!");
                    if (Program.Rng.RandomLessThan(100) < _player.SkillSavingThrow)
                    {
                        Profile.Instance.MsgPrint("You resist the effects!");
                    }
                    else
                    {
                        _player.TakeHit(Program.Rng.DiceRoll(15, 15), monsterDescription);
                        _player.SetTimedBleeding(_player.TimedBleeding + Program.Rng.DiceRoll(10, 10));
                    }
                    break;

                // MonsterFlag5.AcidBolt
                case 128 + 16:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} mumbles." : $"{monsterName} casts a acid bolt.");
                    FireBoltAtPlayer(monsterIndex, new ProjectAcid(SaveGame.Instance.SpellEffects), Program.Rng.DiceRoll(7, 8) + (monsterLevel / 3));
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsAcid);
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsReflect);
                    break;

                // MonsterFlag5.LightningBolt
                case 128 + 17:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} mumbles." : $"{monsterName} casts a lightning bolt.");
                    FireBoltAtPlayer(monsterIndex, new ProjectElec(SaveGame.Instance.SpellEffects), Program.Rng.DiceRoll(4, 8) + (monsterLevel / 3));
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsElec);
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsReflect);
                    break;

                // MonsterFlag5.FireBolt
                case 128 + 18:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} mumbles." : $"{monsterName} casts a fire bolt.");
                    FireBoltAtPlayer(monsterIndex, new ProjectFire(SaveGame.Instance.SpellEffects), Program.Rng.DiceRoll(9, 8) + (monsterLevel / 3));
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsFire);
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsReflect);
                    break;

                // MonsterFlag5.ColdBolt
                case 128 + 19:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} mumbles." : $"{monsterName} casts a frost bolt.");
                    FireBoltAtPlayer(monsterIndex, new ProjectCold(SaveGame.Instance.SpellEffects), Program.Rng.DiceRoll(6, 8) + (monsterLevel / 3));
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsCold);
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsReflect);
                    break;

                // MonsterFlag5.PoisonBolt
                case 128 + 20:
                    break;

                // MonsterFlag5.NetherBolt
                case 128 + 21:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} mumbles." : $"{monsterName} casts a nether bolt.");
                    FireBoltAtPlayer(monsterIndex, new ProjectNether(SaveGame.Instance.SpellEffects), 30 + Program.Rng.DiceRoll(5, 5) + (monsterLevel * 3 / 2));
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsNeth);
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsReflect);
                    break;

                // MonsterFlag5.WaterBolt
                case 128 + 22:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} mumbles." : $"{monsterName} casts a water bolt.");
                    FireBoltAtPlayer(monsterIndex, new ProjectWater(SaveGame.Instance.SpellEffects), Program.Rng.DiceRoll(10, 10) + monsterLevel);
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsReflect);
                    break;

                // MonsterFlag5.ManaBolt
                case 128 + 23:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} mumbles." : $"{monsterName} casts a mana bolt.");
                    FireBoltAtPlayer(monsterIndex, new ProjectMana(SaveGame.Instance.SpellEffects), Program.Rng.DieRoll(monsterLevel * 7 / 2) + 50);
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsReflect);
                    break;

                // MonsterFlag5.PlasmaBolt
                case 128 + 24:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} mumbles." : $"{monsterName} casts a plasma bolt.");
                    FireBoltAtPlayer(monsterIndex, new ProjectPlasma(SaveGame.Instance.SpellEffects), 10 + Program.Rng.DiceRoll(8, 7) + monsterLevel);
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsReflect);
                    break;

                // MonsterFlag5.IceBolt
                case 128 + 25:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} mumbles." : $"{monsterName} casts an ice bolt.");
                    FireBoltAtPlayer(monsterIndex, new ProjectIce(SaveGame.Instance.SpellEffects), Program.Rng.DiceRoll(6, 6) + monsterLevel);
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsCold);
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsReflect);
                    break;

                // MonsterFlag5.MagicMissile
                case 128 + 26:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} mumbles." : $"{monsterName} casts a magic missile.");
                    FireBoltAtPlayer(monsterIndex, new ProjectMissile(SaveGame.Instance.SpellEffects), Program.Rng.DiceRoll(2, 6) + (monsterLevel / 3));
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsReflect);
                    break;

                // MonsterFlag5.Scare
                case 128 + 27:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind
                        ? $"{monsterName} mumbles, and you hear scary noises."
                        : $"{monsterName} casts a fearful illusion.");
                    if (_player.HasFearResistance)
                    {
                        Profile.Instance.MsgPrint("You refuse to be frightened.");
                    }
                    else if (Program.Rng.RandomLessThan(100) < _player.SkillSavingThrow)
                    {
                        Profile.Instance.MsgPrint("You refuse to be frightened.");
                    }
                    else
                    {
                        _player.SetTimedFear(_player.TimedFear + Program.Rng.RandomLessThan(4) + 4);
                    }
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsFear);
                    break;

                // MonsterFlag5.Blindness
                case 128 + 28:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(
                        playerIsBlind ? $"{monsterName} mumbles." : $"{monsterName} casts a spell, burning your eyes!");
                    if (_player.HasBlindnessResistance)
                    {
                        Profile.Instance.MsgPrint("You are unaffected!");
                    }
                    else if (Program.Rng.RandomLessThan(100) < _player.SkillSavingThrow)
                    {
                        Profile.Instance.MsgPrint("You resist the effects!");
                    }
                    else
                    {
                        _player.SetTimedBlindness(12 + Program.Rng.RandomLessThan(4));
                    }
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsBlind);
                    break;

                // MonsterFlag5.Confuse
                case 128 + 29:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind
                        ? $"{monsterName} mumbles, and you hear puzzling noises."
                        : $"{monsterName} creates a mesmerising illusion.");
                    if (_player.HasConfusionResistance)
                    {
                        Profile.Instance.MsgPrint("You disbelieve the feeble spell.");
                    }
                    else if (Program.Rng.RandomLessThan(100) < _player.SkillSavingThrow)
                    {
                        Profile.Instance.MsgPrint("You disbelieve the feeble spell.");
                    }
                    else
                    {
                        _player.SetTimedConfusion(_player.TimedConfusion + Program.Rng.RandomLessThan(4) + 4);
                    }
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsConf);
                    break;

                // MonsterFlag5.Slow
                case 128 + 30:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint($"{monsterName} drains power from your muscles!");
                    if (_player.HasFreeAction)
                    {
                        Profile.Instance.MsgPrint("You are unaffected!");
                    }
                    else if (Program.Rng.RandomLessThan(100) < _player.SkillSavingThrow)
                    {
                        Profile.Instance.MsgPrint("You resist the effects!");
                    }
                    else
                    {
                        _player.SetTimedSlow(_player.TimedSlow + Program.Rng.RandomLessThan(4) + 4);
                    }
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsFree);
                    break;

                // MonsterFlag5.Hold
                case 128 + 31:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} mumbles." : $"{monsterName} stares deep into your eyes!");
                    if (_player.HasFreeAction)
                    {
                        Profile.Instance.MsgPrint("You are unaffected!");
                    }
                    else if (Program.Rng.RandomLessThan(100) < _player.SkillSavingThrow)
                    {
                        Profile.Instance.MsgPrint("You resist the effects!");
                    }
                    else
                    {
                        _player.SetTimedParalysis(_player.TimedParalysis + Program.Rng.RandomLessThan(4) + 4);
                    }
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsFree);
                    break;

                // MonsterFlag6.Haste
                case 160 + 0:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} mumbles." : $"{monsterName} concentrates on {monsterPossessive} body.");
                    if (monster.Speed < race.Speed + 10)
                    {
                        Profile.Instance.MsgPrint($"{monsterName} starts moving faster.");
                        monster.Speed += 10;
                    }
                    else if (monster.Speed < race.Speed + 20)
                    {
                        Profile.Instance.MsgPrint($"{monsterName} starts moving faster.");
                        monster.Speed += 2;
                    }
                    break;

                // MonsterFlag6.DreadCurse
                case 160 + 1:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint($"{monsterName} invokes the Dread Curse of Azathoth!");
                    if (Program.Rng.RandomLessThan(100) < _player.SkillSavingThrow)
                    {
                        Profile.Instance.MsgPrint("You resist the effects!");
                    }
                    else
                    {
                        int dummy = (65 + Program.Rng.DieRoll(25)) * _player.Health / 100;
                        Profile.Instance.MsgPrint("Your feel your life fade away!");
                        _player.TakeHit(dummy, monsterName);
                        _player.CurseEquipment(100, 20);
                        if (_player.Health < 1)
                        {
                            _player.Health = 1;
                        }
                    }
                    break;

                // MonsterFlag6.Heal
                case 160 + 2:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} mumbles." : $"{monsterName} concentrates on {monsterPossessive} wounds.");
                    monster.Health += monsterLevel * 6;
                    if (monster.Health >= monster.MaxHealth)
                    {
                        monster.Health = monster.MaxHealth;
                        Profile.Instance.MsgPrint(seenByPlayer
                            ? $"{monsterName} looks completely healed!"
                            : $"{monsterName} sounds completely healed!");
                    }
                    else
                    {
                        Profile.Instance.MsgPrint(seenByPlayer ? $"{monsterName} looks healthier." : $"{monsterName} sounds healthier.");
                    }
                    if (_saveGame.TrackedMonsterIndex == monsterIndex)
                    {
                        _player.RedrawNeeded.Set(RedrawFlag.PrHealth);
                    }
                    if (monster.FearLevel != 0)
                    {
                        monster.FearLevel = 0;
                        Profile.Instance.MsgPrint($"{monsterName} recovers {monsterPossessive} courage.");
                    }
                    break;

                // MonsterFlag6.Xxx2
                case 160 + 3:
                    break;

                // MonsterFlag6.Blink
                case 160 + 4:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint($"{monsterName} blinks away.");
                    _saveGame.SpellEffects.TeleportAway(monsterIndex, 10);
                    break;

                // MonsterFlag6.TeleportSelf
                case 160 + 5:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint($"{monsterName} teleports away.");
                    _saveGame.SpellEffects.TeleportAway(monsterIndex, (Constants.MaxSight * 2) + 5);
                    break;

                // MonsterFlag6.Xxx3
                case 160 + 6:
                // MonsterFlag6.Xxx4
                case 160 + 7:
                    break;

                // MonsterFlag6.TeleportTo
                case 160 + 8:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint($"{monsterName} commands you to return.");
                    _saveGame.SpellEffects.TeleportPlayerTo(monster.MapY, monster.MapX);
                    break;

                // MonsterFlag6.TeleportAway
                case 160 + 9:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint($"{monsterName} teleports you away.");
                    _saveGame.SpellEffects.TeleportPlayer(100);
                    break;

                // MonsterFlag6.TeleportLevel
                case 160 + 10:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind
                        ? $"{monsterName} mumbles strangely."
                        : $"{monsterName} gestures at your feet.");
                    if (_player.HasNexusResistance)
                    {
                        Profile.Instance.MsgPrint("You are unaffected!");
                    }
                    else if (Program.Rng.RandomLessThan(100) < _player.SkillSavingThrow)
                    {
                        Profile.Instance.MsgPrint("You resist the effects!");
                    }
                    else
                    {
                        _saveGame.SpellEffects.TeleportPlayerLevel();
                    }
                    _level.Monsters.UpdateSmartLearn(monsterIndex, Constants.DrsNexus);
                    break;

                // MonsterFlag6.Xxx5
                case 160 + 11:
                    break;

                // MonsterFlag6.Darkness
                case 160 + 12:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} mumbles." : $"{monsterName} gestures in shadow.");
                    _saveGame.SpellEffects.UnlightArea(0, 3);
                    break;

                // MonsterFlag6.CreateTraps
                case 160 + 13:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind
                        ? $"{monsterName} mumbles, and then cackles evilly."
                        : $"{monsterName} casts a spell and cackles evilly.");
                    _saveGame.SpellEffects.TrapCreation();
                    break;

                // MonsterFlag6.Forget
                case 160 + 14:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint($"{monsterName} tries to blank your mind.");
                    if (Program.Rng.RandomLessThan(100) < _player.SkillSavingThrow)
                    {
                        Profile.Instance.MsgPrint("You resist the effects!");
                    }
                    else if (_saveGame.SpellEffects.LoseAllInfo())
                    {
                        Profile.Instance.MsgPrint("Your memories fade away.");
                    }
                    break;

                // MonsterFlag6.Xxx6
                case 160 + 15:
                    break;

                // MonsterFlag6.SummonKin
                case 160 + 16:
                    _saveGame.Disturb(true);
                    if (playerIsBlind)
                    {
                        Profile.Instance.MsgPrint($"{monsterName} mumbles.");
                    }
                    else
                    {
                        string kin = (race.Flags1 & MonsterFlag1.Unique) != 0 ? "minions" : "kin";
                        Profile.Instance.MsgPrint($"{monsterName} magically summons {monsterPossessive} {kin}.");
                    }
                    _level.Monsters.SummonKinType = race.Character;
                    for (k = 0; k < 6; k++)
                    {
                        if (_level.Monsters.SummonSpecific(playerY, playerX, monsterLevel, Constants.SummonKin))
                        {
                            count++;
                        }
                    }
                    if (playerIsBlind && count != 0)
                    {
                        Profile.Instance.MsgPrint("You hear many things appear nearby.");
                    }
                    break;

                // MonsterFlag6.SummonReaver
                case 160 + 17:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind
                        ? $"{monsterName} mumbles."
                        : $"{monsterName} magically summons Black Reavers!");
                    if (playerIsBlind && count != 0)
                    {
                        Profile.Instance.MsgPrint("You hear heavy steps nearby.");
                    }
                    _saveGame.SpellEffects.SummonReaver();
                    break;

                // MonsterFlag6.SummonMonster
                case 160 + 18:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} mumbles." : $"{monsterName} magically summons help!");
                    for (k = 0; k < 1; k++)
                    {
                        if (_level.Monsters.SummonSpecific(playerY, playerX, monsterLevel, 0))
                        {
                            count++;
                        }
                    }
                    if (playerIsBlind && count != 0)
                    {
                        Profile.Instance.MsgPrint("You hear something appear nearby.");
                    }
                    break;

                // MonsterFlag6.SummonMonsters
                case 160 + 19:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} mumbles." : $"{monsterName} magically summons monsters!");
                    for (k = 0; k < 8; k++)
                    {
                        if (_level.Monsters.SummonSpecific(playerY, playerX, monsterLevel, 0))
                        {
                            count++;
                        }
                    }
                    if (playerIsBlind && count != 0)
                    {
                        Profile.Instance.MsgPrint("You hear many things appear nearby.");
                    }
                    break;

                // MonsterFlag6.SummonAnt
                case 160 + 20:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} mumbles." : $"{monsterName} magically summons ants.");
                    for (k = 0; k < 6; k++)
                    {
                        if (_level.Monsters.SummonSpecific(playerY, playerX, monsterLevel, Constants.SummonAnt))
                        {
                            count++;
                        }
                    }
                    if (playerIsBlind && count != 0)
                    {
                        Profile.Instance.MsgPrint("You hear many things appear nearby.");
                    }
                    break;

                // MonsterFlag6.SummonSpider
                case 160 + 21:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} mumbles." : $"{monsterName} magically summons spiders.");
                    for (k = 0; k < 6; k++)
                    {
                        if (_level.Monsters.SummonSpecific(playerY, playerX, monsterLevel, Constants.SummonSpider))
                        {
                            count++;
                        }
                    }
                    if (playerIsBlind && count != 0)
                    {
                        Profile.Instance.MsgPrint("You hear many things appear nearby.");
                    }
                    break;

                // MonsterFlag6.SummonHound
                case 160 + 22:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} mumbles." : $"{monsterName} magically summons hounds.");
                    for (k = 0; k < 6; k++)
                    {
                        if (_level.Monsters.SummonSpecific(playerY, playerX, monsterLevel, Constants.SummonHound))
                        {
                            count++;
                        }
                    }
                    if (playerIsBlind && count != 0)
                    {
                        Profile.Instance.MsgPrint("You hear many things appear nearby.");
                    }
                    break;

                // MonsterFlag6.SummonHydra
                case 160 + 23:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} mumbles." : $"{monsterName} magically summons hydras.");
                    for (k = 0; k < 6; k++)
                    {
                        if (_level.Monsters.SummonSpecific(playerY, playerX, monsterLevel, Constants.SummonHydra))
                        {
                            count++;
                        }
                    }
                    if (playerIsBlind && count != 0)
                    {
                        Profile.Instance.MsgPrint("You hear many things appear nearby.");
                    }
                    break;

                // MonsterFlag6.SummonCthuloid
                case 160 + 24:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind
                        ? $"{monsterName} mumbles."
                        : $"{monsterName} magically summons a Cthuloid entity!");
                    for (k = 0; k < 1; k++)
                    {
                        if (_level.Monsters.SummonSpecific(playerY, playerX, monsterLevel, Constants.SummonCthuloid))
                        {
                            count++;
                        }
                    }
                    if (playerIsBlind && count != 0)
                    {
                        Profile.Instance.MsgPrint("You hear something appear nearby.");
                    }
                    break;

                // MonsterFlag6.SummonDemon
                case 160 + 25:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} mumbles." : $"{monsterName} magically summons a demon!");
                    for (k = 0; k < 1; k++)
                    {
                        if (_level.Monsters.SummonSpecific(playerY, playerX, monsterLevel, Constants.SummonDemon))
                        {
                            count++;
                        }
                    }
                    if (playerIsBlind && count != 0)
                    {
                        Profile.Instance.MsgPrint("You hear something appear nearby.");
                    }
                    break;

                // MonsterFlag6.SummonUndead
                case 160 + 26:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind
                        ? $"{monsterName} mumbles."
                        : $"{monsterName} magically summons an undead adversary!");
                    for (k = 0; k < 1; k++)
                    {
                        if (_level.Monsters.SummonSpecific(playerY, playerX, monsterLevel, Constants.SummonUndead))
                        {
                            count++;
                        }
                    }
                    if (playerIsBlind && count != 0)
                    {
                        Profile.Instance.MsgPrint("You hear something appear nearby.");
                    }
                    break;

                // MonsterFlag6.SummonDragon
                case 160 + 27:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind ? $"{monsterName} mumbles." : $"{monsterName} magically summons a dragon!");
                    for (k = 0; k < 1; k++)
                    {
                        if (_level.Monsters.SummonSpecific(playerY, playerX, monsterLevel, Constants.SummonDragon))
                        {
                            count++;
                        }
                    }
                    if (playerIsBlind && count != 0)
                    {
                        Profile.Instance.MsgPrint("You hear something appear nearby.");
                    }
                    break;

                // MonsterFlag6.SummonHiUndead
                case 160 + 28:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(
                        playerIsBlind ? $"{monsterName} mumbles." : $"{monsterName} magically summons greater undead!");
                    for (k = 0; k < 8; k++)
                    {
                        if (_level.Monsters.SummonSpecific(playerY, playerX, monsterLevel, Constants.SummonHiUndead))
                        {
                            count++;
                        }
                    }
                    if (playerIsBlind && count != 0)
                    {
                        Profile.Instance.MsgPrint("You hear many creepy things appear nearby.");
                    }
                    break;

                // MonsterFlag6.SummonHiDragon
                case 160 + 29:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind
                        ? $"{monsterName} mumbles."
                        : $"{monsterName} magically summons ancient dragons!");
                    for (k = 0; k < 8; k++)
                    {
                        if (_level.Monsters.SummonSpecific(playerY, playerX, monsterLevel, Constants.SummonHiDragon))
                        {
                            count++;
                        }
                    }
                    if (playerIsBlind && count != 0)
                    {
                        Profile.Instance.MsgPrint("You hear many powerful things appear nearby.");
                    }
                    break;

                // MonsterFlag6.SummonGreatOldOne
                case 160 + 30:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(
                        playerIsBlind ? $"{monsterName} mumbles." : $"{monsterName} magically summons Great Old Ones!");
                    for (k = 0; k < 8; k++)
                    {
                        if (_level.Monsters.SummonSpecific(playerY, playerX, monsterLevel, Constants.SummonGoo))
                        {
                            count++;
                        }
                    }
                    if (playerIsBlind && count != 0)
                    {
                        Profile.Instance.MsgPrint("You hear immortal beings appear nearby.");
                    }
                    break;

                // MonsterFlag6.SummonUnique
                case 160 + 31:
                    _saveGame.Disturb(true);
                    Profile.Instance.MsgPrint(playerIsBlind
                        ? $"{monsterName} mumbles."
                        : $"{monsterName} magically summons special opponents!");
                    for (k = 0; k < 8; k++)
                    {
                        if (_level.Monsters.SummonSpecific(playerY, playerX, monsterLevel, Constants.SummonUnique))
                        {
                            count++;
                        }
                    }
                    for (k = 0; k < 8; k++)
                    {
                        if (_level.Monsters.SummonSpecific(playerY, playerX, monsterLevel, Constants.SummonHiUndead))
                        {
                            count++;
                        }
                    }
                    if (playerIsBlind && count != 0)
                    {
                        Profile.Instance.MsgPrint("You hear many powerful things appear nearby.");
                    }
                    break;
            }
            // If the player saw us cast the spell, let them learn we can do that
            if (seenByPlayer)
            {
                if (thrownSpell < 32 * 4)
                {
                    race.Knowledge.RFlags4 |= 1u << (thrownSpell - (32 * 3));
                    if (race.Knowledge.RCastInate < Constants.MaxUchar)
                    {
                        race.Knowledge.RCastInate++;
                    }
                }
                else if (thrownSpell < 32 * 5)
                {
                    race.Knowledge.RFlags5 |= 1u << (thrownSpell - (32 * 4));
                    if (race.Knowledge.RCastSpell < Constants.MaxUchar)
                    {
                        race.Knowledge.RCastSpell++;
                    }
                }
                else if (thrownSpell < 32 * 6)
                {
                    race.Knowledge.RFlags6 |= 1u << (thrownSpell - (32 * 5));
                    if (race.Knowledge.RCastSpell < Constants.MaxUchar)
                    {
                        race.Knowledge.RCastSpell++;
                    }
                }
            }
            // If we killed the player, let their descendants remember that
            if (_player.IsDead && race.Knowledge.RDeaths < Constants.MaxShort)
            {
                race.Knowledge.RDeaths++;
            }
            // We did cast a spell
            return true;
        }
    }
}