﻿using IrrKlang;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Cthangband.UI
{
    internal class Mixer
    {
        public float MusicVolume = 1;
        public float SoundVolume = 1;
        private readonly Assembly _assembly = Assembly.GetExecutingAssembly();
        private readonly Dictionary<MusicTrack, ISoundSource> _musicSources = new Dictionary<MusicTrack, ISoundSource>();
        private readonly Dictionary<SoundEffect, List<string>> _soundResourceLists = new Dictionary<SoundEffect, List<string>>();
        private readonly Dictionary<string, ISoundSource> _soundSources = new Dictionary<string, ISoundSource>();
        private ISound _currentMusicISound;
        private MusicTrack _currentMusicTrack = MusicTrack.None;
        private ISoundEngine _engine = new ISoundEngine();

        public Mixer()
        {
            _musicSources.Add(MusicTrack.Chargen, MusicTrackFromResource("umbrella-pants.flac"));
            _musicSources.Add(MusicTrack.Death, MusicTrackFromResource("final-count.flac"));
            _musicSources.Add(MusicTrack.Dungeon, MusicTrackFromResource("bent-and-broken.flac"));
            _musicSources.Add(MusicTrack.Menu, MusicTrackFromResource("anxiety.flac"));
            _musicSources.Add(MusicTrack.QuestLevel, MusicTrackFromResource("the-house-of-leaves.flac"));
            _musicSources.Add(MusicTrack.Town, MusicTrackFromResource("midnight-tale.flac"));
            _musicSources.Add(MusicTrack.Victory, MusicTrackFromResource("take-a-chance.flac"));
            _musicSources.Add(MusicTrack.Wilderness, MusicTrackFromResource("hush.flac"));
            _soundResourceLists.Add(SoundEffect.ActivateArtifact, new List<string> { "plm_aim_wand.wav" });
            _soundResourceLists.Add(SoundEffect.Bell, new List<string> { "plm_jar_ding.wav" });
            _soundResourceLists.Add(SoundEffect.BreathWeapon, new List<string> { "mco_attack_breath.wav" });
            _soundResourceLists.Add(SoundEffect.BumpWall, new List<string> { "plm_wood_thud.wav" });
            _soundResourceLists.Add(SoundEffect.CastSpell, new List<string> { "plm_spell1.wav", "plm_spell2.wav", "plm_spell3.wav" });
            _soundResourceLists.Add(SoundEffect.Cursed, new List<string> { "pls_man_oooh.wav" });
            _soundResourceLists.Add(SoundEffect.DestroyItem, new List<string> { "plm_bang_metal.wav", "plm_break_canister.wav", "plm_break_glass.wav", "plm_break_glass2.wav", "plm_break_plates.wav", "plm_break_shatter.wav", "plm_break_smash.wav", "plm_glass_breaking.wav", "plm_glass_break.wav", "plm_glass_smashing.wav" });
            _soundResourceLists.Add(SoundEffect.Dig, new List<string> { "plm_metal_clank.wav" });
            _soundResourceLists.Add(SoundEffect.DisarmTrap, new List<string> { "plm_bang_ceramic.wav", "plm_chest_latch.wav", "plm_click_switch3.wav" });
            _soundResourceLists.Add(SoundEffect.Drop, new List<string> { "plm_drop_boot.wav" });
            _soundResourceLists.Add(SoundEffect.Eat, new List<string> { "plm_eat_bite.wav" });
            _soundResourceLists.Add(SoundEffect.EnterDungeon1, new List<string> { "amb_door_iron.wav", "amb_bell_metal1.wav" });
            _soundResourceLists.Add(SoundEffect.EnterDungeon2, new List<string> { "amb_bell_tibet1.wav", "amb_bell_metal2.wav", "amb_gong_strike.wav" });
            _soundResourceLists.Add(SoundEffect.EnterDungeon3, new List<string> { "amb_bell_tibet2.wav", "amb_dungeon_echo.wav", "amb_pulse_low.wav" });
            _soundResourceLists.Add(SoundEffect.EnterDungeon4, new List<string> { "amb_bell_tibet3.wav", "amb_dungeon_echowet.wav", "amb_gong_undertone.wav" });
            _soundResourceLists.Add(SoundEffect.EnterDungeon5, new List<string> { "amb_door_doom.wav", "amb_gong_chinese.wav", "amb_gong_low.wav" });
            _soundResourceLists.Add(SoundEffect.EnterStore, new List<string> { "sto_bell_desk.wav", "sto_bell_ding.wav", "sto_bell_dingaling.wav", "sto_bell_jingles.wav", "sto_bell_ringing.wav", "sto_bell_shop.wav" });
            _soundResourceLists.Add(SoundEffect.EnterTownDay, new List<string> { "amb_thunder_rain.wav" });
            _soundResourceLists.Add(SoundEffect.EnterTownNight, new List<string> { "amb_guitar_chord.wav", "amb_thunder_roll.wav" });
            _soundResourceLists.Add(SoundEffect.FindSecretDoor, new List<string> { "id_bad_hmm.wav" });
            _soundResourceLists.Add(SoundEffect.HealthWarning, new List<string> { "plc_bell_warn.wav" });
            _soundResourceLists.Add(SoundEffect.HitGood, new List<string> { "plc_hit_anvil.wav" });
            _soundResourceLists.Add(SoundEffect.HitGreat, new List<string> { "plc_hit_groan.wav" });
            _soundResourceLists.Add(SoundEffect.HitHiGreat, new List<string> { "plc_hit_grunt.wav" });
            _soundResourceLists.Add(SoundEffect.HitHiSuperb, new List<string> { "plc_hit_anvil2.wav" });
            _soundResourceLists.Add(SoundEffect.HitSuperb, new List<string> { "plc_hit_grunt2.wav" });
            _soundResourceLists.Add(SoundEffect.Hungry, new List<string> { "id_bad_hmm.wav" });
            _soundResourceLists.Add(SoundEffect.IdentifyBad, new List<string> { "id_bad_hmm.wav" });
            _soundResourceLists.Add(SoundEffect.IdentifyGood, new List<string> { "id_bad_hmm.wav" });
            _soundResourceLists.Add(SoundEffect.IdentifySpecial, new List<string> { "id_bad_hmm.wav" });
            _soundResourceLists.Add(SoundEffect.LeaveStore, new List<string> { "plm_door_bolt.wav" });
            _soundResourceLists.Add(SoundEffect.LevelGain, new List<string> { "plm_levelup.wav" });
            _soundResourceLists.Add(SoundEffect.LockpickFail, new List<string> { "plm_click_dry.wav", "plm_click_switch.wav", "plm_click_wood.wav", "plm_door_echolock.wav", "plm_door_wooden.wav" });
            _soundResourceLists.Add(SoundEffect.LockpickSuccess, new List<string> { "plm_break_wood.wav", "plm_cabinet_open.wav", "plm_chest_unlatch.wav", "plm_lock_case.wav", "plm_lock_distant.wav", "plm_open_case.wav" });
            _soundResourceLists.Add(SoundEffect.MeleeHit, new List<string> { "plc_hit_hay.wav", "plc_hit_body.wav" });
            _soundResourceLists.Add(SoundEffect.Miss, new List<string> { "plc_miss_arrow2.wav" });
            _soundResourceLists.Add(SoundEffect.MonsterBashes, new List<string> { "mco_squish_snap.wav" });
            _soundResourceLists.Add(SoundEffect.MonsterBegs, new List<string> { "mco_man_mumble.wav" });
            _soundResourceLists.Add(SoundEffect.MonsterBites, new List<string> { "mco_snarl_short.wav", "mco_bite_soft.wav", "mco_bite_munch.wav", "mco_bite_long.wav", "mco_bite_short.wav", "mco_bite_gnash.wav", "mco_bite_chomp.wav", "mco_bite_regular.wav", "mco_bite_small.wav", "mco_bite_dainty.wav", "mco_bite_hard.wav", "mco_bite_chew.wav" });
            _soundResourceLists.Add(SoundEffect.MonsterBreeds, new List<string> { "mco_frog_trill.wav" });
            _soundResourceLists.Add(SoundEffect.MonsterButts, new List<string> { "mco_cuica_rubbing.wav", "mco_thud_crash.wav" });
            _soundResourceLists.Add(SoundEffect.MonsterCausesFear, new List<string> { "mco_creature_groan.wav", "mco_dino_slur.wav" });
            _soundResourceLists.Add(SoundEffect.MonsterClaws, new List<string> { "mco_ceramic_trill.wav", "mco_scurry_dry.wav" });
            _soundResourceLists.Add(SoundEffect.MonsterCrawls, new List<string> { "mco_card_shuffle.wav", "mco_shake_roll.wav" });
            _soundResourceLists.Add(SoundEffect.MonsterCreatesTraps, new List<string> { "mco_thoing_deep.wav" });
            _soundResourceLists.Add(SoundEffect.MonsterCrushes, new List<string> { "mco_dino_low.wav", "mco_squish_hit.wav" });
            _soundResourceLists.Add(SoundEffect.MonsterDies, new List<string> { "mco_howl_croak.wav", "mco_howl_deep.wav", "mco_howl_distressed.wav", "mco_howl_high.wav", "mco_howl_long.wav" });
            _soundResourceLists.Add(SoundEffect.MonsterDrools, new List<string> { "mco_creature_choking.wav", "mco_liquid_squirt.wav" });
            _soundResourceLists.Add(SoundEffect.MonsterEngulfs, new List<string> { "mco_dino_talk.wav", "mco_dino_yawn.wav" });
            _soundResourceLists.Add(SoundEffect.MonsterFlees, new List<string> { "mco_creature_yelp.wav" });
            _soundResourceLists.Add(SoundEffect.MonsterGazes, new List<string> { "mco_thoing_backwards.wav" });
            _soundResourceLists.Add(SoundEffect.MonsterHits, new List<string> { "mco_hit_whip.wav" });
            _soundResourceLists.Add(SoundEffect.MonsterInsults, new List<string> { "mco_strange_thwoink.wav" });
            _soundResourceLists.Add(SoundEffect.MonsterKicks, new List<string> { "mco_rubber_thud.wav" });
            _soundResourceLists.Add(SoundEffect.MonsterMoans, new List<string> { "mco_strange_music.wav" });
            _soundResourceLists.Add(SoundEffect.MonsterShrieks, new List<string> { "mco_mouse_squeaks.wav" });
            _soundResourceLists.Add(SoundEffect.MonsterSpits, new List<string> { "mco_attack_spray.wav" });
            _soundResourceLists.Add(SoundEffect.MonsterSpores, new List<string> { "mco_dub_wobble.wav", "mco_spray_long.wav" });
            _soundResourceLists.Add(SoundEffect.MonsterStings, new List<string> { "mco_castanet_trill.wav", "mco_tube_hit.wav" });
            _soundResourceLists.Add(SoundEffect.MonsterTouches, new List<string> { "mco_click_vibra.wav" });
            _soundResourceLists.Add(SoundEffect.MonsterWails, new List<string> { "mco_dino_low.wav" });
            _soundResourceLists.Add(SoundEffect.NothingToOpen, new List<string> { "plm_click_switch2.wav", "plm_door_knob.wav" });
            _soundResourceLists.Add(SoundEffect.OpenDoor, new List<string> { "plm_door_bolt.wav", "plm_door_creak.wav", "plm_door_dungeon.wav", "plm_door_entrance.wav", "plm_door_open.wav", "plm_door_opening.wav", "plm_door_rusty.wav", "plm_door_squeaky.wav" });
            _soundResourceLists.Add(SoundEffect.PickUpMoney1, new List<string> { "plm_coins_light.wav", "plm_coins_shake.wav" });
            _soundResourceLists.Add(SoundEffect.PickUpMoney2, new List<string> { "plm_chain_light.wav", "plm_coins_pour.wav" });
            _soundResourceLists.Add(SoundEffect.PickUpMoney3, new List<string> { "plm_coins_dump.wav" });
            _soundResourceLists.Add(SoundEffect.PlayerAfraid, new List<string> { "pls_man_yell.wav" });
            _soundResourceLists.Add(SoundEffect.PlayerBerserk, new List<string> { "pls_man_scream2.wav" });
            _soundResourceLists.Add(SoundEffect.PlayerBlessed, new List<string> { "sum_angel_song.wav" });
            _soundResourceLists.Add(SoundEffect.PlayerBlind, new List<string> { "pls_tone_conk.wav" });
            _soundResourceLists.Add(SoundEffect.Playerconfused, new List<string> { "pls_man_ugh.wav" });
            _soundResourceLists.Add(SoundEffect.PlayerCut, new List<string> { "pls_man_argoh.wav" });
            _soundResourceLists.Add(SoundEffect.PlayerDeath, new List<string> { "plc_die_laugh.wav" });
            _soundResourceLists.Add(SoundEffect.PlayerDrugged, new List<string> { "pls_breathe_in.wav" });
            _soundResourceLists.Add(SoundEffect.PlayerHaste, new List<string> { "pls_bell_sustain.wav" });
            _soundResourceLists.Add(SoundEffect.PlayerHeroism, new List<string> { "pls_tone_goblet.wav" });
            _soundResourceLists.Add(SoundEffect.PlayerInfravision, new List<string> { "pls_tone_clavelo8.wav" });
            _soundResourceLists.Add(SoundEffect.PlayerInvulnerable, new List<string> { "pls_tone_blurk.wav" });
            _soundResourceLists.Add(SoundEffect.PlayerParalysed, new List<string> { "pls_man_gulp_new.wav" });
            _soundResourceLists.Add(SoundEffect.PlayerPoisoned, new List<string> { "pls_tone_guiro.wav" });
            _soundResourceLists.Add(SoundEffect.PlayerProtEvil, new List<string> { "pls_bell_glass.wav" });
            _soundResourceLists.Add(SoundEffect.PlayerRecover, new List<string> { "pls_bell_chime_new.wav" });
            _soundResourceLists.Add(SoundEffect.PlayerResistAcid, new List<string> { "pls_man_sniff.wav" });
            _soundResourceLists.Add(SoundEffect.PlayerResistCold, new List<string> { "pls_tone_stick.wav" });
            _soundResourceLists.Add(SoundEffect.PlayerResistElectric, new List<string> { "pls_tone_elec.wav" });
            _soundResourceLists.Add(SoundEffect.PlayerResistFire, new List<string> { "pls_tone_scrape.wav" });
            _soundResourceLists.Add(SoundEffect.PlayerResistPoison, new List<string> { "pls_man_spit.wav" });
            _soundResourceLists.Add(SoundEffect.PlayerSeeInvisible, new List<string> { "pls_tone_clave6.wav" });
            _soundResourceLists.Add(SoundEffect.PlayerShield, new List<string> { "pls_bell_bowl.wav" });
            _soundResourceLists.Add(SoundEffect.PlayerSlow, new List<string> { "pls_man_sigh.wav" });
            _soundResourceLists.Add(SoundEffect.PlayerStatDrain, new List<string> { "pls_tone_headstock.wav" });
            _soundResourceLists.Add(SoundEffect.Pray, new List<string> { "sum_angel_song.wav" });
            _soundResourceLists.Add(SoundEffect.PseudoId, new List<string> { "id_good_hmm.wav" });
            _soundResourceLists.Add(SoundEffect.Quaff, new List<string> { "plm_bottle_clinks.wav", "plm_cork_pop.wav", "plm_cork_squeak.wav" });
            _soundResourceLists.Add(SoundEffect.QuestMonsterDies, new List<string> { "amb_guitar_chord.wav" });
            _soundResourceLists.Add(SoundEffect.RangedHit, new List<string> { "plc_hit_arrow.wav" });
            _soundResourceLists.Add(SoundEffect.Shoot, new List<string> { "plc_miss_swish.wav", "plc_miss_arrow.wav" });
            _soundResourceLists.Add(SoundEffect.ShutDoor, new List<string> { "plm_bang_dumpster.wav", "plm_cabinet_shut.wav", "plm_close_hatch.wav", "plm_door_creakshut.wav", "plm_door_latch.wav", "plm_door_shut.wav", "plm_door_slam.wav" });
            _soundResourceLists.Add(SoundEffect.StairsDown, new List<string> { "plm_floor_creak.wav" });
            _soundResourceLists.Add(SoundEffect.StairsUp, new List<string> { "plm_floor_creak2.wav" });
            _soundResourceLists.Add(SoundEffect.StoreSoldBargain, new List<string> { "id_bad_dang.wav" });
            _soundResourceLists.Add(SoundEffect.StoreSoldCheaply, new List<string> { "sto_man_haha.wav" });
            _soundResourceLists.Add(SoundEffect.StoreSoldExtraCheaply, new List<string> { "sto_man_whoohaha.wav" });
            _soundResourceLists.Add(SoundEffect.StoreSoldWorthless, new List<string> { "sto_man_hey.wav" });
            _soundResourceLists.Add(SoundEffect.StoreTransaction, new List<string> { "sto_coins_countertop.wav", "sto_bell_register1.wav", "sto_bell_register2.wav" });
            _soundResourceLists.Add(SoundEffect.Study, new List<string> { "plm_book_pageturn.wav" });
            _soundResourceLists.Add(SoundEffect.SummonAngel, new List<string> { "sum_angel_song.wav" });
            _soundResourceLists.Add(SoundEffect.SummonAnimals, new List<string> { "sum_lion_growl.wav" });
            _soundResourceLists.Add(SoundEffect.SummonDemons, new List<string> { "sum_ghost_wail.wav", "sum_laugh_evil2.wav" });
            _soundResourceLists.Add(SoundEffect.SummonDragons, new List<string> { "sum_piano_scrape.wav" });
            _soundResourceLists.Add(SoundEffect.SummonGreaterDemons, new List<string> { "sum_ghost_moan.wav" });
            _soundResourceLists.Add(SoundEffect.SummonGreaterDragons, new List<string> { "sum_gong_temple.wav" });
            _soundResourceLists.Add(SoundEffect.SummonGreaterUndead, new List<string> { "sum_ghost_moan.wav" });
            _soundResourceLists.Add(SoundEffect.SummonHounds, new List<string> { "sum_lion_growl.wav" });
            _soundResourceLists.Add(SoundEffect.SummonHydras, new List<string> { "sum_piano_scrape.wav" });
            _soundResourceLists.Add(SoundEffect.SummonMonster, new List<string> { "sum_chime_jangle.wav" });
            _soundResourceLists.Add(SoundEffect.SummonRingwraiths, new List<string> { "sum_bell_hand.wav" });
            _soundResourceLists.Add(SoundEffect.SummonSpiders, new List<string> { "sum_piano_scrape.wav" });
            _soundResourceLists.Add(SoundEffect.SummonUndead, new List<string> { "sum_ghost_oooo.wav" });
            _soundResourceLists.Add(SoundEffect.SummonUniques, new List<string> { "sum_bell_tone.wav" });
            _soundResourceLists.Add(SoundEffect.Teleport, new List<string> { "plm_chimes_jangle.wav" });
            _soundResourceLists.Add(SoundEffect.TeleportLevel, new List<string> { "sum_bell_crystal.wav" });
            _soundResourceLists.Add(SoundEffect.UniqueDies, new List<string> { "sum_ghost_wail.wav" });
            _soundResourceLists.Add(SoundEffect.UseStaff, new List<string> { "plm_use_staff.wav" });
            _soundResourceLists.Add(SoundEffect.WieldWeapon, new List<string> { "plm_metal_sharpen.wav" });
            _soundResourceLists.Add(SoundEffect.ZapRod, new List<string> { "plm_zap_rod.wav" });
            foreach (var soundResourceList in _soundResourceLists)
            {
                foreach (var resourceName in soundResourceList.Value)
                {
                    if (!_soundSources.ContainsKey(resourceName))
                    {
                        var stream = _assembly.GetManifestResourceStream("Cthangband.Sounds." + resourceName);
                        _soundSources.Add(resourceName, _engine.AddSoundSourceFromIOStream(stream, resourceName));
                        stream.Close();
                    }
                }
            }
        }

        public void Play(MusicTrack musicTrack)
        {
            if (musicTrack == _currentMusicTrack)
            {
                return;
            }
            if (MusicVolume == 0)
            {
                return;
            }
            _currentMusicISound?.Stop();
            _currentMusicISound = null;
            if (musicTrack == MusicTrack.None)
            {
                return;
            }
            _musicSources[musicTrack].DefaultVolume = MusicVolume;
            _currentMusicISound = _engine.Play2D(_musicSources[musicTrack], true, false, false);
            _currentMusicTrack = musicTrack;
        }

        public void Play(SoundEffect sound)
        {
            if (SoundVolume == 0)
            {
                return;
            }
            var list = _soundResourceLists[sound];
            var soundResourceName = list[Program.Rng.DieRoll(list.Count) - 1];
            var soundSource = _soundSources[soundResourceName];
            soundSource.DefaultVolume = SoundVolume;
            _engine.Play2D(soundSource, false, false, false);
        }

        private ISoundSource MusicTrackFromResource(string resourceName)
        {
            var stream = _assembly.GetManifestResourceStream("Cthangband.Music." + resourceName);
            var soundSource = _engine.AddSoundSourceFromIOStream(stream, resourceName);
            stream.Close();
            return soundSource;
        }
    }
}