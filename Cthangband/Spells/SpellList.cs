// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Spells.Chaos;
using Cthangband.Spells.Corporeal;
using Cthangband.Spells.Death;
using Cthangband.Spells.Folk;
using Cthangband.Spells.Life;
using Cthangband.Spells.Nature;
using Cthangband.Spells.Sorcery;
using Cthangband.Spells.Tarot;
using System;
using System.Collections.Generic;

namespace Cthangband.Spells
{
    /// <summary>
    /// A set of four spell books from a single realm, containing a total of thirty two spells
    /// </summary>
    [Serializable]
    internal class SpellList : List<Spell>
    {
        public SpellList(Realm realm, int characterClass)
        {
            switch (realm)
            {
                case Realm.Chaos:
                    // Sign of Chaos
                    Add(new ChaosSpellMagicMissile());
                    Add(new ChaosSpellTrapAndDoorDestruction());
                    Add(new ChaosSpellFlashOfLight());
                    Add(new ChaosSpellTouchOfConfusion());
                    Add(new ChaosSpellManaBurst());
                    Add(new ChaosSpellFireBolt());
                    Add(new ChaosSpellFistOfForce());
                    Add(new ChaosSpellTeleportSelf());
                    // Chaos Mastery
                    Add(new ChaosSpellWonder());
                    Add(new ChaosSpellChaosBolt());
                    Add(new ChaosSpellSonicBoom());
                    Add(new ChaosSpellDoomBolt());
                    Add(new ChaosSpellFireBall());
                    Add(new ChaosSpellTeleportOther());
                    Add(new ChaosSpellWordOfDestruction());
                    Add(new ChaosSpellInvokeChaos());
                    // G'harne Fragments
                    Add(new ChaosSpellPolymorphOther());
                    Add(new ChaosSpellChainLightning());
                    Add(new ChaosSpellArcaneBinding());
                    Add(new ChaosSpellDisintegrate());
                    Add(new ChaosSpellAlterReality());
                    Add(new ChaosSpellPolymorphSelf());
                    Add(new ChaosSpellChaosBranding());
                    Add(new ChaosSpellSummonDemon());
                    // Book of Azathoth
                    Add(new ChaosSpellGravityBeam());
                    Add(new ChaosSpellMeteorSwarm());
                    Add(new ChaosSpellFlameStrike());
                    Add(new ChaosSpellCallChaos());
                    Add(new ChaosSpellShardBall());
                    Add(new ChaosSpellManaStorm());
                    Add(new ChaosSpellBreatheChaos());
                    Add(new ChaosSpellCallTheVoid());
                    break;

                case Realm.Corporeal:
                    // Basic Chi Flow
                    Add(new CorporealSpellCureLightWounds());
                    Add(new CorporealSpellBlink());
                    Add(new CorporealSpellBravery());
                    Add(new CorporealSpellBatsSense());
                    Add(new CorporealSpellEaglesVision());
                    Add(new CorporealSpellMindVision());
                    Add(new CorporealSpellCureMediumWounds());
                    Add(new CorporealSpellSatisfyHunger());
                    // Yogic Mastery
                    Add(new CorporealSpellBurnResistance());
                    Add(new CorporealSpellDetoxify());
                    Add(new CorporealSpellCureCriticalWounds());
                    Add(new CorporealSpellSeeInvisible());
                    Add(new CorporealSpellTeleport());
                    Add(new CorporealSpellHaste());
                    Add(new CorporealSpellHealing());
                    Add(new CorporealSpellResistTrue());
                    // De Vermis Mysteriis
                    Add(new CorporealSpellHorrificVisage());
                    Add(new CorporealSpellSeeMagic());
                    Add(new CorporealSpellStoneSkin());
                    Add(new CorporealSpellMoveBody());
                    Add(new CorporealSpellMutateBody());
                    Add(new CorporealSpellKnowSelf());
                    Add(new CorporealSpellTeleportLevel());
                    Add(new CorporealSpellWordOfRecall());
                    // Pnakotic Manuscript
                    Add(new CorporealSpellHeroism());
                    Add(new CorporealSpellWraithform());
                    Add(new CorporealSpellAttunement());
                    Add(new CorporealSpellRestoreBody());
                    Add(new CorporealSpellHealingTrue());
                    Add(new CorporealSpellHypnoticEyes());
                    Add(new CorporealSpellRestoreSoul());
                    Add(new CorporealSpellInvulnerability());
                    break;

                case Realm.Death:
                    // Black Prayers
                    Add(new DeathSpellDetectUnlife());
                    Add(new DeathSpellMalediction());
                    Add(new DeathSpellDetectEvil());
                    Add(new DeathSpellStinkingCloud());
                    Add(new DeathSpellBlackSleep());
                    Add(new DeathSpellResistPoison());
                    Add(new DeathSpellHorrify());
                    Add(new DeathSpellEnslaveUndead());
                    // Black Mass
                    Add(new DeathSpellOrbOfEntropy());
                    Add(new DeathSpellNetherBolt());
                    Add(new DeathSpellTerror());
                    Add(new DeathSpellVampiricDrain());
                    Add(new DeathSpellPoisonBranding());
                    Add(new DeathSpellDispelGood());
                    Add(new DeathSpellCarnage());
                    Add(new DeathSpellRestoreLife());
                    // Cultes des Goules
                    Add(new DeathSpellBerserk());
                    Add(new DeathSpellInvokeSpirits());
                    Add(new DeathSpellDarkBolt());
                    Add(new DeathSpellBattleFrenzy());
                    Add(new DeathSpellVampirismTrue());
                    Add(new DeathSpellVampiricBranding());
                    Add(new DeathSpellDarknessStorm());
                    Add(new DeathSpellMassCarnage());
                    // Necronomicon
                    Add(new DeathSpellDeathRay());
                    Add(new DeathSpellRaiseTheDead());
                    Add(new DeathSpellEsoteria());
                    Add(new DeathSpellWordOfDeath());
                    Add(new DeathSpellEvocation());
                    Add(new DeathSpellHellfire());
                    Add(new DeathSpellAnnihilation());
                    Add(new DeathSpellWraithform());
                    break;

                case Realm.Folk:
                    // Cantrips for Beginners
                    Add(new FolkSpellZap());
                    Add(new FolkSpellWizardLock());
                    Add(new FolkSpellDetectInvisibility());
                    Add(new FolkSpellDetectMonsters());
                    Add(new FolkSpellBlink());
                    Add(new FolkSpellLightArea());
                    Add(new FolkSpellTrapAndDoorDestruction());
                    Add(new FolkSpellCureLightWounds());
                    // Minor Magicks
                    Add(new FolkSpellDetectDoorsAndTraps());
                    Add(new FolkSpellPhlogiston());
                    Add(new FolkSpellDetectTreasure());
                    Add(new FolkSpellDetectEnchantment());
                    Add(new FolkSpellDetectObjects());
                    Add(new FolkSpellCurePoison());
                    Add(new FolkSpellResistCold());
                    Add(new FolkSpellResistFire());
                    // Major Magicks
                    Add(new FolkSpellResistLightning());
                    Add(new FolkSpellResistAcid());
                    Add(new FolkSpellCureMediumWounds());
                    Add(new FolkSpellTeleport());
                    Add(new FolkSpellStoneToMud());
                    Add(new FolkSpellRayOfLight());
                    Add(new FolkSpellSatisfyHunger());
                    Add(new FolkSpellSeeInvisible());
                    // Magicks of Mastery
                    Add(new FolkSpellRecharging());
                    Add(new FolkSpellTeleportLevel());
                    Add(new FolkSpellIdentify());
                    Add(new FolkSpellTeleportAway());
                    Add(new FolkSpellElementalBall());
                    Add(new FolkSpellDetection());
                    Add(new FolkSpellWordOfRecall());
                    Add(new FolkSpellClairvoyance());
                    break;

                case Realm.Life:
                    // Book of Common Prayer
                    Add(new LifeSpellDetectEvil());
                    Add(new LifeSpellCureLightWounds());
                    Add(new LifeSpellBless());
                    Add(new LifeSpellRemoveFear());
                    Add(new LifeSpellCallLight());
                    Add(new LifeSpellDetectTrapsAndSecretDoors());
                    Add(new LifeSpellCureMediumWounds());
                    Add(new LifeSpellSatisfyHunger());
                    // High Mass
                    Add(new LifeSpellRemoveCurse());
                    Add(new LifeSpellCurePoison());
                    Add(new LifeSpellCureCriticalWounds());
                    Add(new LifeSpellSenseUnseen());
                    Add(new LifeSpellHolyOrb());
                    Add(new LifeSpellProtectionFromEvil());
                    Add(new LifeSpellHealing());
                    Add(new LifeSpellElderSign());
                    // Dhol Chants
                    Add(new LifeSpellExorcism());
                    Add(new LifeSpellDispelCurse());
                    Add(new LifeSpellDispelUndeadAndDemons());
                    Add(new LifeSpellDayOfTheDove());
                    Add(new LifeSpellDispelEvil());
                    Add(new LifeSpellBanish());
                    Add(new LifeSpellHolyWord());
                    Add(new LifeSpellWardingTrue());
                    // Ponape Scriptures
                    Add(new LifeSpellHeroism());
                    Add(new LifeSpellPrayer());
                    Add(new LifeSpellBlessWeapon());
                    Add(new LifeSpellRestoration());
                    Add(new LifeSpellHealingTrue());
                    Add(new LifeSpellHolyVision());
                    Add(new LifeSpellDivineIntervention());
                    Add(new LifeSpellHolyInvulnerability());
                    break;

                case Realm.Nature:
                    // Call of the Wild
                    Add(new NatureSpellDetectCreatures());
                    Add(new NatureSpellFirstAid());
                    Add(new NatureSpellDetectDoorsAndTraps());
                    Add(new NatureSpellForaging());
                    Add(new NatureSpellDaylight());
                    Add(new NatureSpellAnimalTaming());
                    Add(new NatureSpellResistEnvironment());
                    Add(new NatureSpellCureWoundsAndPoison());
                    // Nature Mastery
                    Add(new NatureSpellStoneToMud());
                    Add(new NatureSpellLightningBolt());
                    Add(new NatureSpellNatureAwareness());
                    Add(new NatureSpellFrostBolt());
                    Add(new NatureSpellRayOfSunlight());
                    Add(new NatureSpellEntangle());
                    Add(new NatureSpellSummonAnimal());
                    Add(new NatureSpellHerbalHealing());
                    // Revelations of Glaaki
                    Add(new NatureSpellDoorCreation());
                    Add(new NatureSpellStairBuilding());
                    Add(new NatureSpellStoneSkin());
                    Add(new NatureSpellResistanceTrue());
                    Add(new NatureSpellAnimalFriendship());
                    Add(new NatureSpellStoneTell());
                    Add(new NatureSpellWallOfStone());
                    Add(new NatureSpellProtectFromCorrosion());
                    // Cthaat Aquadingen
                    Add(new NatureSpellEarthquake());
                    Add(new NatureSpellWhirlwindAttack());
                    Add(new NatureSpellBlizzard());
                    Add(new NatureSpellLightningStorm());
                    Add(new NatureSpellWhirlpool());
                    Add(new NatureSpellCallSunlight());
                    Add(new NatureSpellElementalBranding());
                    Add(new NatureSpellNaturesWrath());
                    break;

                case Realm.Sorcery:
                    // Beginner's Handbook
                    Add(new SorcerySpellDetectMonsters());
                    Add(new SorcerySpellPhaseDoor());
                    Add(new SorcerySpellDetectDoorsAndTraps());
                    Add(new SorcerySpellLightArea());
                    Add(new SorcerySpellConfuseMonster());
                    Add(new SorcerySpellTeleport());
                    Add(new SorcerySpellSleepMonster());
                    Add(new SorcerySpellRecharging());
                    // Master Sorcerer's Handbook
                    Add(new SorcerySpellMagicMapping());
                    Add(new SorcerySpellIdentify());
                    Add(new SorcerySpellSlowMonster());
                    Add(new SorcerySpellMassSleep());
                    Add(new SorcerySpellTeleportAway());
                    Add(new SorcerySpellHasteSelf());
                    Add(new SorcerySpellDetectionTrue());
                    Add(new SorcerySpellIdentifyTrue());
                    // Unaussprechlichen Kulten
                    Add(new SorcerySpellDetectObjectsAndTreasure());
                    Add(new SorcerySpellDetectEnchantment());
                    Add(new SorcerySpellCharmMonster());
                    Add(new SorcerySpellDimensionDoor());
                    Add(new SorcerySpellSenseMinds());
                    Add(new SorcerySpellSelfKnowledge());
                    Add(new SorcerySpellTeleportLevel());
                    Add(new SorcerySpellWordOfRecall());
                    // Liber Ivonis
                    Add(new SorcerySpellStasis());
                    Add(new SorcerySpellTelekinesis());
                    Add(new SorcerySpellYellowSign());
                    Add(new SorcerySpellClairvoyance());
                    Add(new SorcerySpellEnchantWeapon());
                    Add(new SorcerySpellEnchantArmour());
                    Add(new SorcerySpellAlchemy());
                    Add(new SorcerySpellGlobeOfInvulnerability());
                    break;

                case Realm.Tarot:
                    // Conjurings and Tricks
                    Add(new TarotSpellPhaseDoor());
                    Add(new TarotSpellMindBlast());
                    Add(new TarotSpellTarotDraw());
                    Add(new TarotSpellResetRecall());
                    Add(new TarotSpellTeleport());
                    Add(new TarotSpellDimensionDoor());
                    Add(new TarotSpellAstralSpying());
                    Add(new TarotSpellTeleportAway());
                    // Card Mastery
                    Add(new TarotSpellSummonObject());
                    Add(new TarotSpellSummonAnimal());
                    Add(new TarotSpellPhantasmalServant());
                    Add(new TarotSpellSummonMonster());
                    Add(new TarotSpellConjureElemental());
                    Add(new TarotSpellTeleportLevel());
                    Add(new TarotSpellWordOfRecall());
                    Add(new TarotSpellBanish());
                    // Eltdown Shards
                    Add(new TarotSpellTheFool());
                    Add(new TarotSpellSummonSpiders());
                    Add(new TarotSpellSummonReptiles());
                    Add(new TarotSpellSummonHounds());
                    Add(new TarotSpellAstralBranding());
                    Add(new TarotSpellExtradimensionalBeing());
                    Add(new TarotSpellDeathDealing());
                    Add(new TarotSpellSummonReaver());
                    // Celeano Fragments
                    Add(new TarotSpellEtherealDivination());
                    Add(new TarotSpellAstralLore());
                    Add(new TarotSpellSummonUndead());
                    Add(new TarotSpellSummonDragon());
                    Add(new TarotSpellMassSummons());
                    Add(new TarotSpellSummonDemon());
                    Add(new TarotSpellSummonAncientDragon());
                    Add(new TarotSpellSummonGreaterUndead());
                    break;

                default:
                    for (int i = 0; i < 32; i++)
                    {
                        Add(new IllegibleSpell());
                    }
                    break;
            }
            foreach (Spell spell in this)
            {
                spell.Initialise(characterClass);
            }
        }
    }
}