# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name ActivationHandler
extends RefCounted

var dungeon_level: DungeonLevel
var player: Player
var savegame: SaveGame

const USE_DEVICE:int = 3


static func new_activation_handler(new_player: Player, new_dungeon_level: DungeonLevel, new_savegame: SaveGame) -> ActivationHandler:
	var activation_handler: ActivationHandler = ActivationHandler.new()
	activation_handler.player = new_player
	activation_handler.dungeon_level = new_dungeon_level
	activation_handler.savegame = new_savegame
	return activation_handler


func command_spike() -> void:
	# Get the location to be spiked
	var target_engine: TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	var dir: int = target_engine.get_direction_no_aim()
	if dir != -1:
		var row: int = player.map_row + dungeon_level.keypad_direction_row_offset[dir]
		var col: int = player.map_col + dungeon_level.keypad_direction_col_offset[dir]
		var tile: GridTile = dungeon_level.get_grid(row, col)
		# Make sure it can be spiked and we have spikes to do it with
		if not tile.feature_type.is_closed_door:
			Profile.instance.message_print("You see nothing there to spike.")
		else:
			var item_index: int = SaveGame.instance.command_engine.get_spike()
			if item_index == -1:
				Profile.instance.message_print("You have no spikes!")
			# Can't close a door if there's someone in the way
			elif tile.monster_index != 0:
				# Attempting costs a turn anyway
				SaveGame.instance.energy_use = 100
				Profile.instance.message_print("There is a monster in the way!")
				SaveGame.instance.command_engine.player_attack_monster(row, col)
			else:
				# Spiking a door costs a turn
				SaveGame.instance.energy_use = 100
				Profile.instance.message_print("You jam the door with a spike.")
				# Replace the door feature with a jammed door
				if tile.feature_type.category == FloorTileTypeCategory.LOCKED_DOOR:
					tile.set_feature(tile.feature_type.name.replace("Locked", "Jammed"))
				# If it's already jammed, strengthen it
				if tile.feature_type.category == FloorTileTypeCategory.LOCKED_DOOR:
					var strength: int = int(tile.feature_type.name.substr(10))
					if strength < 7:
						tile.set_feature("JammedDoor%d" % [strength + 1])
				# Use up the spike from the player's inventory
				player.inventory.inven_item_increase(item_index, -1)
				player.inventory.inven_item_describe(item_index)
				player.inventory.inven_item_optimise(item_index)


func command_activate(item_index:int)->void:
	var target_engine:TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	if item_index == -999:
		# No item passed in, so get one filtering to activatable items only
		savegame.item_filter_function = savegame.command_engine.ItemFilterActivatable
		item_index = savegame.get_item("Activate which item? ", true, false, false)
		if item_index == 99:
			return
		if item_index == -2:
			Profile.instance.message_print("You have nothing to activate.")
			return
	# Get the item from the index
	var item:Item = player.inventory.items[item_index] if item_index >= 0 else dungeon_level.dungeon_items.items[0 - item_index]
	# Check if the item is activatable
	savegame.item_filter_function = savegame.command_engine.ItemFilterActivatable
	if not player.inventory.item_matches_filter(item):
		Profile.instance.message_print("You can't activate that!")
		savegame.item_filter_function = Callable()
		return
	savegame.item_filter_function = Callable()
	# Activating an item uses 100 energy
	savegame.energy_use = 100
	# Get the level of the item
	var item_level:int = item.item_type.level
	if item.is_artifact():
		item_level = SaveGame.instance.artifacts[item.artifact_index].level
	# Work out the chance of using the item successfully based on its level and the
	# player's skill
	var chance: int = player.skill_use_device
	if player.timed_confusion != 0:
		chance /= 2
	chance -= 50 if item_level > 50 else item_level
	# Always give a slight chance of success
	if chance < USE_DEVICE and Rng.roll_die(USE_DEVICE - chance + 1) == 1:
		chance = USE_DEVICE
	# If we fail our use item roll just tell us and quit
	if chance < USE_DEVICE or Rng.roll_die(chance) < USE_DEVICE:
		Profile.instance.message_print("You failed to activate it properly.")
		return
	# If the item is still recharging, then just tell us and quit
	if item.recharge_time_left != 0:
		Profile.instance.message_print("It whines, glows and fades...")
		return
	# We passed the checks, so the item is activated
	Profile.instance.message_print("You activate it...")
	# If it is a legendary item then use its ability and quit
	if item.is_legendary():
		savegame.command_engine.activate_legendary(item)
		return
	# If it's an artifact then use its ability
	if item.artifact_index != 0:
		match item.artifact_index:
			# Star Essence of Polaris lights the area
			ArtifactId.STAR_ESSENCE_OF_POLARIS:
					Profile.instance.message_print("The essence wells with clear light...")
					savegame.spell_effects_handler.light_area(Rng.roll_dice(2, 15), 3)
					item.recharge_time_left = Rng.roll_die(10) + 9
			# Star essence of Xoth lights and maps the area
			ArtifactId.STAR_ESSENCE_OF_XOTH:
					Profile.instance.message_print("The essence shines brightly...")
					dungeon_level.map_area()
					savegame.spell_effects_handler.light_area(Rng.roll_dice(2, 15), 3)
					item.recharge_time_left = Rng.roll_die(50) + 49
			# Shining Trapezohedron lights the entire level and recalls us, but drains
			# health to do so
			ArtifactId.SHINING_TRAPEZOHEDRON:
					Profile.instance.message_print("The gemstone flashes bright red!")
					dungeon_level.wiz_light()
					Profile.instance.message_print("The gemstone drains your vitality...")
					player.take_hit(Rng.roll_dice(3, 8), "the shining trapezohedron")
					savegame.spell_effects_handler.detect_traps()
					savegame.spell_effects_handler.detect_doors()
					savegame.spell_effects_handler.detect_stairs()
					if User.get_check("Activate recall? "):
						player.toggle_recall()
					item.recharge_time_left = Rng.roll_die(20) + 19
			# Amulet of Lobon protects us from evil
			ArtifactId.AMULET_OF_LOBON:
					Profile.instance.message_print("The amulet lets out a shrill wail...")
					var k: int = 3 * player.level
					player.set_timed_protection_from_evil(player.timed_protection_from_evil + Rng.roll_die(25) + k)
					item.recharge_time_left = Rng.roll_die(225) + 224
			# Amulet of Abdul Alhazred dispels evil
			ArtifactId.AMULET_OF_ABDUL_ALHAZRED:
					Profile.instance.message_print("The amulet floods the area with goodness...")
					savegame.spell_effects_handler.dispel_evil(player.level * 5)
					item.recharge_time_left = Rng.roll_die(300) + 299
			# Ring of Magic has a djinn in it that drains life from an opponent
			ArtifactId.RING_OF_MAGIC:
					Profile.instance.message_print("You order Frakir to strangle your opponent.")
					var dir:int = target_engine.get_direction_with_aim()
					if dir == -1:
						return
					if savegame.spell_effects_handler.drain_life(dir, 100):
						item.recharge_time_left = Rng.roll_die(100) + 99
			# Ring of Bast hastes you
			ArtifactId.RING_OF_BAST:
					Profile.instance.message_print("The ring glows brightly...")
					if player.timed_haste == 0:
						player.set_timed_haste(Rng.roll_die(75) + 75)
					else:
						player.set_timed_haste(player.timed_haste + 5)
					item.recharge_time_left = Rng.roll_die(150) + 149
			# Ring of Elemental Fire casts a fireball
			ArtifactId.RING_OF_ELEMENTAL_POWER_FIRE:
					Profile.instance.message_print("The ring glows deep red...")
					var dir:int = target_engine.get_direction_with_aim()
					if dir == -1:
						return
					savegame.spell_effects_handler.fire_ball(ProjectileFire.new(savegame.spell_effects_handler), dir, 120, 3)
					item.recharge_time_left = Rng.roll_die(225) + 224
			# Ring of Elemental Ice casts a coldball
			ArtifactId.RING_OF_ELEMENTAL_POWER_ICE:
					Profile.instance.message_print("The ring glows bright white...")
					var dir:int = target_engine.get_direction_with_aim()
					if dir == -1:
						return
					savegame.spell_effects_handler.fire_ball(ProjectileCold.new(savegame.spell_effects_handler), dir, 200, 3)
					item.recharge_time_left = Rng.roll_die(325) + 324
			# Ring of Elemental Lightning casts a lightning ball
			ArtifactId.RING_OF_ELEMENTAL_POWER_STORM:
					Profile.instance.message_print("The ring glows deep blue...")
					var dir:int = target_engine.get_direction_with_aim()
					if dir == -1:
						return
					savegame.spell_effects_handler.fire_ball(ProjectileElectricity.new(savegame.spell_effects_handler), dir, 250, 3)
					item.recharge_time_left = Rng.roll_die(425) + 424
			# Ring of Set has a random effect
			ArtifactId.RING_OF_SET:
					Profile.instance.message_print("The ring glows intensely black...")
					var dir:int = target_engine.get_direction_with_aim()
					if dir == -1:
						return
					savegame.command_engine.ring_of_set_power(dir)
					item.recharge_time_left = Rng.roll_die(450) + 449
			# Razorback gives you a point-blank lightning ball
			ArtifactId.DRAGONSCALE_RAZORBACK:
					Profile.instance.message_print("Your armor is surrounded by lightning...")
					for i:int in 8:
						savegame.spell_effects_handler.fire_ball(ProjectileElectricity.new(savegame.spell_effects_handler), dungeon_level.ordered_direction[i], 150, 3)
					item.recharge_time_left = 1000
			# Bladeturner heals you and gives you timed resistances
			ArtifactId.DRAGONSCALE_BLADETURNER:
					var dir:int = target_engine.get_direction_with_aim()
					if dir == -1:
						return
					Profile.instance.message_print("You breathe the elements.")
					savegame.spell_effects_handler.fire_ball(ProjectileMissile.new(savegame.spell_effects_handler), dir, 300, 4)
					Profile.instance.message_print("Your armor glows many colors...")
					player.set_timed_fear(0)
					player.set_timed_superheroism(player.timed_super_heroism + Rng.roll_die(50) + 50)
					player.restore_health(30)
					player.set_timed_blessing(player.timed_blessing + Rng.roll_die(50) + 50)
					player.set_timed_acid_resistance(player.timed_acid_resistance + Rng.roll_die(50) + 50)
					player.set_timed_lightning_resistance(player.timed_lightning_resistance + Rng.roll_die(50) + 50)
					player.set_timed_fire_resistance(player.timed_fire_resistance + Rng.roll_die(50) + 50)
					player.set_timed_cold_resistance(player.timed_cold_resistance + Rng.roll_die(50) + 50)
					player.set_timed_poison_resistance(player.timed_poison_resistance + Rng.roll_die(50) + 50)
					item.recharge_time_left = 400
			# Soulkeeper heals you a lot
			ArtifactId.PLATE_MAIL_SOULKEEPER:
					Profile.instance.message_print("Your armor glows a bright white...")
					Profile.instance.message_print("You feel much better...")
					player.restore_health(1000)
					player.set_timed_bleeding(0)
					item.recharge_time_left = 888
			# Vampire Hunter cures most ailments
			ArtifactId.ARMOUR_OF_THE_VAMPIRE_HUNTER:
					Profile.instance.message_print("A heavenly choir sings...")
					player.set_timed_poison(0)
					player.set_timed_bleeding(0)
					player.set_timed_stun(0)
					player.set_timed_confusion(0)
					player.set_timed_blindness(0)
					player.set_timed_heroism(player.timed_heroism + Rng.roll_die(25) + 25)
					player.restore_health(777)
					item.recharge_time_left = 300
			# Orc does carnage
			ArtifactId.ARMOUR_OF_THE_ORCS:
					Profile.instance.message_print("Your armor glows deep blue...")
					savegame.spell_effects_handler.carnage(true)
					item.recharge_time_left = 500
			# Ogre Lords destroys doors
			ArtifactId.ARMOUR_OF_THE_OGRE_LORDS:
					Profile.instance.message_print("Your armor glows bright red...")
					savegame.spell_effects_handler.destroy_doors_touch()
					item.recharge_time_left = 10
			# Dragon Helm and Terror Mask cause fear
			ArtifactId.DRAGON_HELM_OF_POWER, ArtifactId.HELM_TERROR_MASK:
					savegame.spell_effects_handler.turn_monsters(40 + player.level)
					item.recharge_time_left = 3 * (player.level + 10)
			# Skull Keeper detects everything
			ArtifactId.HELM_SKULLKEEPER:
					Profile.instance.message_print("Your helm glows bright white...")
					Profile.instance.message_print("An image forms in your mind...")
					savegame.spell_effects_handler.detect_all()
					item.recharge_time_left = Rng.roll_die(55) + 54
			# Sun Crown heals
			ArtifactId.CROWN_OF_THE_SUN:
					Profile.instance.message_print("Your crown glows deep yellow...")
					Profile.instance.message_print("You feel a warm tingling inside...")
					player.restore_health(700)
					player.set_timed_bleeding(0)
					item.recharge_time_left = 250
			# Cloak of Barzai gives resistances
			ArtifactId.CLOAK_OF_BARZAI:
					Profile.instance.message_print("Your cloak glows many colours...")
					player.set_timed_acid_resistance(player.timed_acid_resistance + Rng.roll_die(20) + 20)
					player.set_timed_lightning_resistance(player.timed_lightning_resistance + Rng.roll_die(20) + 20)
					player.set_timed_fire_resistance(player.timed_fire_resistance + Rng.roll_die(20) + 20)
					player.set_timed_cold_resistance(player.timed_cold_resistance + Rng.roll_die(20) + 20)
					player.set_timed_poison_resistance(player.timed_poison_resistance + Rng.roll_die(20) + 20)
					item.recharge_time_left = 111
			# Darkness sends monsters to sleep
			ArtifactId.CLOAK_DARKNESS:
					Profile.instance.message_print("Your cloak glows deep blue...")
					savegame.spell_effects_handler.sleep_monsters_touch()
					item.recharge_time_left = 55
			# Swashbuckler recharges items
			ArtifactId.CLOAK_OF_THE_SWASHBUCKLER:
					Profile.instance.message_print("Your cloak glows bright yellow...")
					savegame.spell_effects_handler.recharge(60)
					item.recharge_time_left = 70
			# Shifter teleports you
			ArtifactId.CLOAK_SHIFTER:
					Profile.instance.message_print("Your cloak twists space around you...")
					savegame.spell_effects_handler.teleport_player(100)
					item.recharge_time_left = 45
			# Nyogtha restores experience
			ArtifactId.SHADOW_CLOAK_OF_NYOGTHA:
					Profile.instance.message_print("Your cloak glows a deep red...")
					player.restore_level()
					item.recharge_time_left = 450
			# Light shoots magic missiles
			ArtifactId.GLOVES_OF_LIGHT:
					Profile.instance.message_print("Your gloves glow extremely brightly...")
					var dir:int = target_engine.get_direction_with_aim()
					if dir == -1:
						return
					savegame.spell_effects_handler.fire_bolt(ProjectileMissile.new(savegame.spell_effects_handler), dir,
						Rng.roll_dice(2, 6))
					item.recharge_time_left = 2
			# Iron Fist shoots fire bolts
			ArtifactId.GAUNTLET_IRON_FIST:
					Profile.instance.message_print("Your gauntlets are covered in fire...")
					var dir:int = target_engine.get_direction_with_aim()
					if dir == -1:
						return
					savegame.spell_effects_handler.fire_bolt(ProjectileFire.new(savegame.spell_effects_handler), dir, Rng.roll_dice(9, 8))
					item.recharge_time_left = Rng.roll_die(8) + 7
			# Ghouls shoot cold bolts
			ArtifactId.GAUNTLETS_OF_GHOULS:
					Profile.instance.message_print("Your gauntlets are covered in frost...")
					var dir:int = target_engine.get_direction_with_aim()
					if dir == -1:
						return
					savegame.spell_effects_handler.fire_bolt(ProjectileCold.new(savegame.spell_effects_handler), dir, Rng.roll_dice(6, 8))
					item.recharge_time_left = Rng.roll_die(7) + 6
			# White Spark shoot lightning bolts
			ArtifactId.GAUNTLETS_WHITESPARK:
					Profile.instance.message_print("Your gauntlets are covered in sparks...")
					var dir:int = target_engine.get_direction_with_aim()
					if dir == -1:
						return
					savegame.spell_effects_handler.fire_bolt(ProjectileElectricity.new(savegame.spell_effects_handler), dir, Rng.roll_dice(4, 8))
					item.recharge_time_left = Rng.roll_die(6) + 5
			# The Dead shoot acid bolts
			ArtifactId.GAUNTLETS_OF_THE_DEAD:
					Profile.instance.message_print("Your gauntlets are covered in acid...")
					var dir:int = target_engine.get_direction_with_aim()
					if dir == -1:
						return
					savegame.spell_effects_handler.fire_bolt(ProjectileAcid.new(savegame.spell_effects_handler), dir, Rng.roll_dice(5, 8))
					item.recharge_time_left = Rng.roll_die(5) + 4
			# Cesti shoot arrows
			ArtifactId.CESTI_OF_COMBAT:
					Profile.instance.message_print("Your cesti grows magical spikes...")
					var dir:int = target_engine.get_direction_with_aim()
					if dir == -1:
						return
					savegame.spell_effects_handler.fire_bolt(ProjectileArrow.new(savegame.spell_effects_handler), dir, 150)
					item.recharge_time_left = Rng.roll_die(90) + 89
			# Boots haste you
			ArtifactId.BOOTS_OF_ITHAQUA:
					Profile.instance.message_print("A wind swirls around your boots...")
					if player.timed_haste == 0:
						player.set_timed_haste(Rng.roll_die(20) + 20)
					else:
						player.set_timed_haste(player.timed_haste + 5)
					item.recharge_time_left = 200
			# Dancing heal poison and fear
			ArtifactId.BOOTS_OF_DANCING:
					Profile.instance.message_print("Your boots glow deep blue...")
					player.set_timed_fear(0)
					player.set_timed_poison(0)
					item.recharge_time_left = 5
			# Faith shoots a fire bolt
			ArtifactId.DAGGER_OF_FAITH:
					Profile.instance.message_print("Your dagger is covered in fire...")
					var dir:int = target_engine.get_direction_with_aim()
					if dir == -1:
						return
					savegame.spell_effects_handler.fire_bolt(ProjectileFire.new(savegame.spell_effects_handler), dir, Rng.roll_dice(9, 8))
					item.recharge_time_left = Rng.roll_die(8) + 7
			# Hope shoots a frost bolt
			ArtifactId.DAGGER_OF_HOPE:
					Profile.instance.message_print("Your dagger is covered in frost...")
					var dir:int = target_engine.get_direction_with_aim()
					if dir == -1:
						return
					savegame.spell_effects_handler.fire_bolt(ProjectileCold.new(savegame.spell_effects_handler), dir, Rng.roll_dice(6, 8))
					item.recharge_time_left = Rng.roll_die(7) + 6
			# Charity shoots a lightning bolt
			ArtifactId.DAGGER_OF_CHARITY:
					Profile.instance.message_print("Your dagger is covered in sparks...")
					var dir:int = target_engine.get_direction_with_aim()
					if dir == -1:
						return
					savegame.spell_effects_handler.fire_bolt(ProjectileElectricity.new(savegame.spell_effects_handler), dir, Rng.roll_dice(4, 8))
					item.recharge_time_left = Rng.roll_die(6) + 5
			# Thoth shoots a poison ball
			ArtifactId.DAGGER_OF_THOTH:
					Profile.instance.message_print("Your dagger throbs deep green...")
					var dir:int = target_engine.get_direction_with_aim()
					if dir == -1:
						return
					savegame.spell_effects_handler.fire_ball(ProjectilePoison.new(savegame.spell_effects_handler), dir, 12, 3)
					item.recharge_time_left = Rng.roll_die(4) + 3
			# Icicle shoots a cold ball
			ArtifactId.DAGGER_ICICLE:
					Profile.instance.message_print("Your dagger is covered in frost...")
					var dir:int = target_engine.get_direction_with_aim()
					if dir == -1:
						return
					savegame.spell_effects_handler.fire_ball(ProjectileCold.new(savegame.spell_effects_handler), dir, 48, 2)
					item.recharge_time_left = Rng.roll_die(5) + 4
			# Karakal teleports you randomly
			ArtifactId.SWORD_OF_KARAKAL:
					match Rng.roll_die(13):
						1, 2, 3, 4, 5:
							savegame.spell_effects_handler.teleport_player(10)
						6, 7, 8, 9, 10:
							savegame.spell_effects_handler.teleport_player(222)
						11, 12:
							savegame.spell_effects_handler.stair_creation()
						_:
							if User.get_check("Leave this level? "):
								savegame.is_auto_save = true
								savegame.command_save_game()
								savegame.is_auto_save = false
								savegame.new_level_flag = true
								savegame.came_from = LevelStart.START_RANDOM
					item.recharge_time_left = 35
			# Excalibur shoots a frost ball
			ArtifactId.SWORD_EXCALIBUR:
					Profile.instance.message_print("Your sword glows an intense blue...")
					var dir:int = target_engine.get_direction_with_aim()
					if dir == -1:
						return
					savegame.spell_effects_handler.fire_ball(ProjectileCold.new(savegame.spell_effects_handler), dir, 100, 2)
					item.recharge_time_left = 300
			# Dawn Sword summons a reaver
			ArtifactId.SWORD_OF_THE_DAWN:
					Profile.instance.message_print("Your sword flickers black for a moment...")
					dungeon_level.dungeon_monsters.summon_specific_friendly(player.map_row, player.map_col, savegame.total_difficulty(), SummonType.SUMMON_REAVER, true)
					item.recharge_time_left = 500 + Rng.roll_die(500)
			# Everflame shoots a fire ball
			ArtifactId.SWORD_OF_EVERFLAME:
					Profile.instance.message_print("Your sword glows an intense red...")
					var dir:int = target_engine.get_direction_with_aim()
					if dir == -1:
						return
					savegame.spell_effects_handler.fire_ball(ProjectileFire.new(savegame.spell_effects_handler), dir, 72, 2)
					item.recharge_time_left = 400
			# Theoden drains life
			ArtifactId.AXE_OF_THEODEN:
					Profile.instance.message_print("Your axe blade glows black...")
					var dir:int = target_engine.get_direction_with_aim()
					if dir == -1:
						return
					savegame.spell_effects_handler.drain_life(dir, 120)
					item.recharge_time_left = 400
			# Grungnir shoots a lightning ball
			ArtifactId.SPEAR_GUNGNIR:
					Profile.instance.message_print("Your spear crackles with electricity...")
					var dir:int = target_engine.get_direction_with_aim()
					if dir == -1:
						return
					savegame.spell_effects_handler.fire_ball(ProjectileElectricity.new(savegame.spell_effects_handler), dir, 100, 3)
					item.recharge_time_left = 500
			# Destiny does rock to mud
			ArtifactId.SPEAR_OF_DESTINY:
					Profile.instance.message_print("Your spear pulsates...")
					var dir:int = target_engine.get_direction_with_aim()
					if dir == -1:
						return
					savegame.spell_effects_handler.wall_to_mud(dir)
					item.recharge_time_left = 5
			# Trolls does mass carnage
			ArtifactId.AXE_OF_THE_TROLLS:
					Profile.instance.message_print("Your axe lets out a long, shrill note...")
					savegame.spell_effects_handler.mass_carnage(true)
					item.recharge_time_left = 1000
			# Spleens Slicer heals you
			ArtifactId.AXE_SPLEENSLICER:
					Profile.instance.message_print("Your battle axe radiates deep purple...")
					player.restore_health(Rng.roll_dice(4, 8))
					player.set_timed_bleeding((player.timed_bleeding / 2) - 50)
					item.recharge_time_left = Rng.roll_die(3) + 2
			# Gnorri teleports monsters away
			ArtifactId.TRIDENT_OF_THE_GNORRI:
					Profile.instance.message_print("Your trident glows deep red...")
					var dir:int = target_engine.get_direction_with_aim()
					if dir == -1:
						return
					savegame.spell_effects_handler.teleport_monster(dir)
					item.recharge_time_left = 150
			# G'Harne does Word of Recall
			ArtifactId.SCYTHE_OF_GHARNE:
					Profile.instance.message_print("Your scythe glows soft white...")
					player.toggle_recall()
					item.recharge_time_left = 200
			# Totila does confusion
			ArtifactId.FLAIL_TOTILA:
					Profile.instance.message_print("Your flail glows in scintillating colours...")
					var dir:int = target_engine.get_direction_with_aim()
					if dir == -1:
						return
					savegame.spell_effects_handler.confuse_monster(dir, 20)
					item.recharge_time_left = 15
			# Firestarter does fire ball
			ArtifactId.MORNING_STAR_FIRESTARTER:
					Profile.instance.message_print("Your morning star rages in fire...")
					var dir:int = target_engine.get_direction_with_aim()
					if dir == -1:
						return
					savegame.spell_effects_handler.fire_ball(ProjectileFire.new(savegame.spell_effects_handler), dir, 72, 3)
					item.recharge_time_left = 100
			# Thunder does haste
			ArtifactId.MACE_THUNDER:
					Profile.instance.message_print("Your mace glows bright green...")
					if player.timed_haste == 0:
						player.set_timed_haste(Rng.roll_die(20) + 20)
					else:
						player.set_timed_haste(player.timed_haste + 5)
					item.recharge_time_left = Rng.roll_die(100) + 99
			# Ereril does identify
			ArtifactId.QUARTERSTAFF_ERIRIL:
					Profile.instance.message_print("Your quarterstaff glows yellow...")
					if not savegame.spell_effects_handler.identify_item():
						return
					item.recharge_time_left = 10
			# Atal does full identify
			ArtifactId.QUARTERSTAFF_OF_ATAL:
					Profile.instance.message_print("Your quarterstaff glows brightly...")
					savegame.spell_effects_handler.detect_all()
					savegame.spell_effects_handler.probing()
					savegame.spell_effects_handler.identify_fully()
					item.recharge_time_left = 1000
			# Justice drains life
			ArtifactId.HAMMER_JUSTICE:
					Profile.instance.message_print("Your hammer glows white...")
					var dir:int = target_engine.get_direction_with_aim()
					if dir == -1:
						return
					savegame.spell_effects_handler.drain_life(dir, 90)
					item.recharge_time_left = 70
			# Death brands your bolts
			ArtifactId.CROSSBOW_OF_DEATH:
					Profile.instance.message_print("Your crossbow glows deep red...")
					savegame.command_engine.brand_bolts()
					item.recharge_time_left = 999
		return
	# If it wasn't an artifact, then check the other types of activatable item Planar
	# weapon teleports you
	if item.rare_index == RareItemId.WEAPON_PLANAR_WEAPON:
		savegame.spell_effects_handler.teleport_player(100)
		item.recharge_time_left = 50 + Rng.roll_die(50)
		return
	# Dragon armour gives you a ball of the relevant damage type
	if item.category == ItemCategory.DRAGON_ARMOUR:
		var dir:int = target_engine.get_direction_with_aim()
		if dir == -1:
			return
		match item.sub_category:
			DragonArmour.BLUE:
					Profile.instance.message_print("You breathe lightning.")
					savegame.spell_effects_handler.fire_ball(ProjectileElectricity.new(savegame.spell_effects_handler), dir, 100, -2)
					item.recharge_time_left = Rng.roll_die(450) + 449
			DragonArmour.WHITE:
					Profile.instance.message_print("You breathe frost.")
					savegame.spell_effects_handler.fire_ball(ProjectileCold.new(savegame.spell_effects_handler), dir, 110, -2)
					item.recharge_time_left = Rng.roll_die(450) + 449
			DragonArmour.BLACK:
					Profile.instance.message_print("You breathe acid.")
					savegame.spell_effects_handler.fire_ball(ProjectileAcid.new(savegame.spell_effects_handler), dir, 130, -2)
					item.recharge_time_left = Rng.roll_die(450) + 449
			DragonArmour.GREEN:
					Profile.instance.message_print("You breathe poison gas.")
					savegame.spell_effects_handler.fire_ball(ProjectilePoison.new(savegame.spell_effects_handler), dir, 150, -2)
					item.recharge_time_left = Rng.roll_die(450) + 449
			DragonArmour.RED:
					Profile.instance.message_print("You breathe fire.")
					savegame.spell_effects_handler.fire_ball(ProjectileFire.new(savegame.spell_effects_handler), dir, 200, -2)
					item.recharge_time_left = Rng.roll_die(450) + 449
			DragonArmour.MULTI_HUED:
					match Rng.roll_die(5) - 1:
						0:
							Profile.instance.message_print("You breathe fire.")
							savegame.spell_effects_handler.fire_ball(ProjectileFire.new(savegame.spell_effects_handler), dir, 250, -2)
						1:
							Profile.instance.message_print("You breathe lightning.")
							savegame.spell_effects_handler.fire_ball(ProjectileElectricity.new(savegame.spell_effects_handler), dir, 250, -2)
						2:
							Profile.instance.message_print("You breathe cold.")
							savegame.spell_effects_handler.fire_ball(ProjectileCold.new(savegame.spell_effects_handler), dir, 250, -2)
						3:
							Profile.instance.message_print("You breathe acid.")
							savegame.spell_effects_handler.fire_ball(ProjectileAcid.new(savegame.spell_effects_handler), dir, 250, -2)
						4:
							Profile.instance.message_print("You breathe poison.")
							savegame.spell_effects_handler.fire_ball(ProjectilePoison.new(savegame.spell_effects_handler), dir, 250, -2)
					item.recharge_time_left = Rng.roll_die(225) + 224
			DragonArmour.BRONZE:
					Profile.instance.message_print("You breathe confusion.")
					savegame.spell_effects_handler.fire_ball(ProjectileConfusion.new(savegame.spell_effects_handler), dir, 120, -2)
					item.recharge_time_left = Rng.roll_die(450) + 449
			DragonArmour.GOLD:
					Profile.instance.message_print("You breathe sound.")
					savegame.spell_effects_handler.fire_ball(ProjectileSound.new(savegame.spell_effects_handler), dir, 130, -2)
					item.recharge_time_left = Rng.roll_die(450) + 449
			DragonArmour.CHAOS:
					chance = Rng.roll_die(2)
					var element:String = "chaos" if chance == 1 else "disenchantment"
					Profile.instance.message_print("You breathe %s." % [element])
					var projectile:Projectile = ProjectileChaos.new(savegame.spell_effects_handler) if chance == 1 else ProjectileDisenchantment.new(savegame.spell_effects_handler)
					savegame.spell_effects_handler.fire_ball(projectile, dir, 220, -2)
					item.recharge_time_left = Rng.roll_die(300) + 299
			DragonArmour.LAW:
					chance = Rng.roll_die(2)
					var element:String = "sound" if chance == 1 else "shards"
					Profile.instance.message_print("You breathe %s." % [element])
					var projectile:Projectile = ProjectileChaos.new(savegame.spell_effects_handler) if chance == 1 else ProjectileDisenchantment.new(savegame.spell_effects_handler)
					savegame.spell_effects_handler.fire_ball(projectile, dir, 230, -2)
					item.recharge_time_left = Rng.roll_die(300) + 299
			DragonArmour.BALANCE:
					chance = Rng.roll_die(4)
					var element:String = "chaos" if chance == 1 else "disenchantment" if chance == 2 else "sound" if chance == 3 else "shards"
					Profile.instance.message_print("You breathe %s." % [element])
					var projectile:Projectile
					match chance:
						1:
							projectile = ProjectileChaos.new(savegame.spell_effects_handler)
						2:
							projectile = ProjectileDisenchantment.new(savegame.spell_effects_handler)
						3:
							projectile = ProjectileSound.new (savegame.spell_effects_handler)
						4:
							projectile = ProjectileExplode.new (savegame.spell_effects_handler)
					savegame.spell_effects_handler.fire_ball(projectile, dir, 250, -2)
					item.recharge_time_left = Rng.roll_die(300) + 299
			DragonArmour.PSEUDO:
					chance = Rng.roll_die(2)
					var element:String = "light" if chance == 1 else "darkness"
					Profile.instance.message_print("You breathe %s." % [element])
					var projectile:Projectile = ProjectileLight.new(savegame.spell_effects_handler) if chance == 1 else ProjectileDark.new(savegame.spell_effects_handler)
					savegame.spell_effects_handler.fire_ball(projectile, dir, 200, -2)
					item.recharge_time_left = Rng.roll_die(300) + 299
			DragonArmour.POWER:
					Profile.instance.message_print("You breathe the elements.")
					savegame.spell_effects_handler.fire_ball(ProjectileMissile.new(savegame.spell_effects_handler), dir, 300, -3)
					item.recharge_time_left = Rng.roll_die(300) + 299
		return
	# Elemental rings give you a ball of the appropriate element
	if item.category == ItemCategory.RING:
		var dir:int = target_engine.get_direction_with_aim()
		if dir == -1:
			return
		match item.sub_category:
			RingType.ACID:
					savegame.spell_effects_handler.fire_ball(ProjectileAcid.new(savegame.spell_effects_handler), dir, 50, 2)
					player.set_timed_acid_resistance(player.timed_acid_resistance + Rng.roll_die(20) + 20)
					item.recharge_time_left = Rng.roll_die(50) + 49
			RingType.ICE:
					savegame.spell_effects_handler.fire_ball(ProjectileCold.new(savegame.spell_effects_handler), dir, 50, 2)
					player.set_timed_cold_resistance(player.timed_cold_resistance + Rng.roll_die(20) + 20)
					item.recharge_time_left = Rng.roll_die(50) + 49
			RingType.FLAMES:
					savegame.spell_effects_handler.fire_ball(ProjectileFire.new(savegame.spell_effects_handler), dir, 50, 2)
					player.set_timed_fire_resistance(player.timed_fire_resistance + Rng.roll_die(20) + 20)
					item.recharge_time_left = Rng.roll_die(50) + 49
		return
	# We ran out of item types
	Profile.instance.message_print("Oops. That object cannot be activated.")

func command_aim_wand(item_index:int)->void:
	if item_index == -999:
		# Prompt for an item, showing only wands
		Inventory.item_filter_category = ItemCategory.WAND
		item_index = savegame.get_item("Aim which wand? ", true, true, true)
		if item_index == 99:
			return
		if item_index == -2:
			Profile.instance.message_print("You have no wand to aim.")
			return
	# Get the item and check if it is really a wand
	var item:Item = player.inventory.items[item_index] if item_index >= 0 else dungeon_level.dungeon_items.items[0 - item_index]
	Inventory.item_filter_category = ItemCategory.WAND
	if not player.inventory.item_matches_filter(item):
		Profile.instance.message_print("That is not a wand!")
		Inventory.item_filter_category = 0
		return
	Inventory.item_filter_category = 0
	# We can't use wands directly from the floor, since we need to aim them
	if item_index < 0 and item.count > 1:
		Profile.instance.message_print("You must first pick up the wands.")
		return
	# Aim the wand
	var target_engine:TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	var dir:int = target_engine.get_direction_with_aim()
	if dir == -1:
		return
	# Using a wand takes 100 energy
	savegame.energy_use = 100
	var ident:bool = false
	var item_level:int = item.item_type.level
	# Chance of success is your skill - item level, with item level capped at 50 and your
	# skill halved if you're confused
	var chance:int = player.skill_use_device
	if player.timed_confusion != 0:
		chance /= 2
	chance -= 50 if item_level > 50 else item_level
	# Always a small chance of success
	if chance < USE_DEVICE and Rng.roll_die(USE_DEVICE - chance + 1) == 1:
		chance = USE_DEVICE
	if chance < USE_DEVICE or Rng.roll_die(chance) < USE_DEVICE:
		Profile.instance.message_print("You failed to use the wand properly.")
		return
	# Make sure we have charges
	if item.type_specific_value <= 0:
		Profile.instance.message_print("The wand has no charges left.")
		item.identify_flags.set_bits(IdentifyFlags.EMPTY)
		return
	var sub_category:int = item.sub_category
	# Wand of wonder just chooses another type of wand less than its own index
	if sub_category == WandType.WONDER:
		sub_category = Rng.roll_die(WandType.WONDER)-1
	match sub_category:
		WandType.HEAL_MONSTER:
				if savegame.spell_effects_handler.heal_monster(dir):
					ident = true
		WandType.HASTE_MONSTER:
				if savegame.spell_effects_handler.speed_monster(dir):
					ident = true
		WandType.CLONE_MONSTER:
				if savegame.spell_effects_handler.clone_monster(dir):
					ident = true
		WandType.TELEPORT_AWAY:
				if savegame.spell_effects_handler.teleport_monster(dir):
					ident = true
		WandType.DISARMING:
				if savegame.spell_effects_handler.disarm_trap(dir):
					ident = true
		WandType.TRAP_DOOR_DEST:
				if savegame.spell_effects_handler.destroy_door(dir):
					ident = true
		WandType.STONE_TO_MUD:
				if savegame.spell_effects_handler.wall_to_mud(dir):
					ident = true
		WandType.LIGHT:
				Profile.instance.message_print("A line of blue shimmering light appears.")
				savegame.spell_effects_handler.light_line(dir)
				ident = true
		WandType.SLEEP_MONSTER:
				if savegame.spell_effects_handler.sleep_monster(dir):
					ident = true
		WandType.SLOW_MONSTER:
				if savegame.spell_effects_handler.slow_monster(dir):
					ident = true
		WandType.CONFUSE_MONSTER:
				if savegame.spell_effects_handler.confuse_monster(dir, 10):
					ident = true
		WandType.FEAR_MONSTER:
				if savegame.spell_effects_handler.fear_monster(dir, 10):
					ident = true
		WandType.DRAIN_LIFE:
				if savegame.spell_effects_handler.drain_life(dir, 75):
					ident = true
		WandType.POLYMORPH:
				if savegame.spell_effects_handler.poly_monster(dir):
					ident = true
		WandType.STINKING_CLOUD:
				savegame.spell_effects_handler.fire_ball(ProjectilePoison.new(savegame.spell_effects_handler), dir, 12, 2)
				ident = true
		WandType.MAGIC_MISSILE:
				savegame.spell_effects_handler.fire_bolt_or_beam(20, ProjectileMissile.new(savegame.spell_effects_handler), dir,
					Rng.roll_dice(2, 6))
				ident = true
		WandType.ACID_BOLT:
				savegame.spell_effects_handler.fire_bolt_or_beam(20, ProjectileAcid.new(savegame.spell_effects_handler), dir,
					Rng.roll_dice(3, 8))
				ident = true
		WandType.CHARM_MONSTER:
				if savegame.spell_effects_handler.charm_monster(dir, 45):
					ident = true
		WandType.FIRE_BOLT:
				savegame.spell_effects_handler.fire_bolt_or_beam(20, ProjectileFire.new(savegame.spell_effects_handler), dir,
					Rng.roll_dice(6, 8))
				ident = true
		WandType.COLD_BOLT:
				savegame.spell_effects_handler.fire_bolt_or_beam(20, ProjectileCold.new(savegame.spell_effects_handler), dir,
					Rng.roll_dice(3, 8))
				ident = true
		WandType.ACID_BALL:
				savegame.spell_effects_handler.fire_ball(ProjectileAcid.new(savegame.spell_effects_handler), dir, 60, 2)
				ident = true
		WandType.ELEC_BALL:
				savegame.spell_effects_handler.fire_ball(ProjectileElectricity.new(savegame.spell_effects_handler), dir, 32, 2)
				ident = true
		WandType.FIRE_BALL:
				savegame.spell_effects_handler.fire_ball(ProjectileFire.new(savegame.spell_effects_handler), dir, 72, 2)
				ident = true
		WandType.COLD_BALL:
				savegame.spell_effects_handler.fire_ball(ProjectileCold.new(savegame.spell_effects_handler), dir, 48, 2)
				ident = true
		WandType.WONDER:
				Profile.instance.message_print("Oops. Wand of wonder activated.")
		WandType.DRAGON_FIRE:
				savegame.spell_effects_handler.fire_ball(ProjectileFire.new(savegame.spell_effects_handler), dir, 100, 3)
				ident = true
		WandType.DRAGON_BREATH:
				savegame.spell_effects_handler.fire_ball(ProjectileCold.new(savegame.spell_effects_handler), dir, 80, 3)
				ident = true
		WandType.DRAGON_BREATH:
				match Rng.roll_die(5):
					1:
							savegame.spell_effects_handler.fire_ball(ProjectileAcid.new(savegame.spell_effects_handler), dir, 100, -3)
					2:
							savegame.spell_effects_handler.fire_ball(ProjectileElectricity.new(savegame.spell_effects_handler), dir, 80, -3)
					3:
							savegame.spell_effects_handler.fire_ball(ProjectileFire.new(savegame.spell_effects_handler), dir, 100, -3)
					4:
							savegame.spell_effects_handler.fire_ball(ProjectileCold.new(savegame.spell_effects_handler), dir, 80, -3)
					_:
							savegame.spell_effects_handler.fire_ball(ProjectilePoison.new(savegame.spell_effects_handler), dir, 60, -3)
				ident = true
		WandType.ANNIHILATION:
				if savegame.spell_effects_handler.drain_life(dir, 125):
					ident = true
		WandType.SHARD:
				savegame.spell_effects_handler.fire_ball(ProjectileShard.new(savegame.spell_effects_handler), dir, 75 + Rng.roll_die(50),2)
				ident = true
	player.notice_flags.set_bits(NoticeFlags.NOTICE_COMBINE | NoticeFlags.NOTICE_REORDER)
	# Mark the wand as having been tried
	item.item_tried()
	# If we just discovered the item's flavour, mark it as so
	if ident and not item.is_flavour_aware():
		item.become_flavour_aware()
		player.gain_experience((item_level + (player.level >> 1)) / player.level)
	# If we're a channeler then we should be using vis instead of charges
	var channeled:bool = false
	if player.spellcasting.casting_type == CastingType.CHANNELLING:
		channeled = savegame.command_engine.command_channel(item)
	# We didn't use vis, so decrease the wand's charges
	if not channeled:
		item.type_specific_value -= 1
		# If the wand is part of a stack, split it off from the others
		if item_index >= 0 and item.count > 1:
			var split_item:Item = item.get_copy()
			split_item.count = 1
			item.type_specific_value += 1
			item.count -= 1
			player.weight_carried -= split_item.weight
			item_index = player.inventory.inven_carry(split_item)
			Profile.instance.message_print("You unstack your wand.")
		# Let us know we have used a charge
		if item_index >= 0:
			player.inventory.report_charge_usage_from_inventory(item_index)
		else:
			dungeon_level.report_charge_usage_from_floor(0 - item_index)


func command_eat_food(item_index:int)->void:
	# Get a food item from the inventory if one wasn't already specified
	Inventory.item_filter_category = ItemCategory.FOOD
	if item_index == -999:
		item_index = savegame.get_item("Eat which item? ", false, true, true)
		if item_index == 99:
			return
		if item_index == -2:
			Profile.instance.message_print("You have nothing to eat.")
			return
	var item:Item = player.inventory.items[item_index] if item_index >= 0 else dungeon_level.dungeon_items.items[0 - item_index]
	# Make sure the item is edible
	Inventory.item_filter_category = ItemCategory.FOOD
	if not player.inventory.item_matches_filter(item):
		Profile.instance.message_print("You can't eat that!")
		Inventory.item_filter_category = 0
		return
	Inventory.item_filter_category = 0
	# Eating costs 100 energy
	savegame.energy_use = 100
	var ident:bool = false
	var item_level:int = item.item_type.level
	match item.sub_category:
		FoodType.POISON:
				if not (player.HasPoisonResistance or player.timed_poison_resistance != 0):
					# Hagarg Ryonis may protect us from poison
					if Rng.roll_die(10) <= player.religion.gods[GodName.HAGARG_RYONIS].adjusted_favour():
						Profile.instance.message_print("Hagarg Ryonis's favour protects you!")
					elif player.set_timed_poison(player.timed_poison + Rng.roll_die(10) + 9):
						ident = true
		FoodType.BLINDNESS:
				if not player.has_blindness_resistance:
					if player.set_timed_blindness(player.timed_blindness + Rng.roll_die(200) + 199):
						ident = true
		FoodType.PARANOIA:
				if not player.has_fear_resistance:
					if player.set_timed_fear(player.timed_fear + Rng.roll_die(10) + 9):
						ident = true
		FoodType.CONFUSION:
				if not player.has_confusion_resistance:
					if player.set_timed_confusion(player.timed_confusion + Rng.roll_die(10) + 9):
						ident = true
		FoodType.HALLUCINATION:
				if not player.has_chaos_resistance:
					if player.set_timed_hallucinations(player.timed_hallucinations + Rng.roll_die(250) + 249):
						ident = true
		FoodType.PARALYSIS:
				if not player.has_free_action:
					if player.set_timed_paralysis(player.timed_paralysis + Rng.roll_die(10) + 9):
						ident = true
		FoodType.WEAKNESS:
				player.take_hit(Rng.roll_dice(6, 6), "poisonous food.")
				player.try_decreasing_ability_score(Ability.STRENGTH)
				ident = true
		FoodType.SICKNESS:
				player.take_hit(Rng.roll_dice(6, 6), "poisonous food.")
				player.try_decreasing_ability_score(Ability.CONSTITUTION)
				ident = true
		FoodType.STUPIDITY:
				player.take_hit(Rng.roll_dice(8, 8), "poisonous food.")
				player.try_decreasing_ability_score(Ability.INTELLIGENCE)
				ident = true
		FoodType.NAIVETY:
				player.take_hit(Rng.roll_dice(8, 8), "poisonous food.")
				player.try_decreasing_ability_score(Ability.WISDOM)
				ident = true
		FoodType.UNHEALTH:
				player.take_hit(Rng.roll_dice(10, 10), "poisonous food.")
				player.try_decreasing_ability_score(Ability.CONSTITUTION)
				ident = true
		FoodType.DISEASE:
				player.take_hit(Rng.roll_dice(10, 10), "poisonous food.")
				player.try_decreasing_ability_score(Ability.STRENGTH)
				ident = true
		FoodType.CUREPOISON:
				if player.set_timed_poison(0):
					ident = true
		FoodType.CURE_BLINDNESS:
				if player.set_timed_blindness(0):
					ident = true
		FoodType.PARANOIA:
				if player.set_timed_fear(0):
					ident = true
		FoodType.CONFUSION:
				if player.set_timed_confusion(0):
					ident = true
		FoodType.CURE_SERIOUS:
				if player.restore_health(Rng.roll_dice(4, 8)):
					ident = true
		FoodType.RESTORE_STR:
				if player.try_restoring_ability_score(Ability.STRENGTH):
					ident = true
		FoodType.RESTORE_CON:
				if player.try_restoring_ability_score(Ability.CONSTITUTION):
					ident = true
		FoodType.RESTORING:
				if player.try_restoring_ability_score(Ability.STRENGTH):
					ident = true
				if player.try_restoring_ability_score(Ability.INTELLIGENCE):
					ident = true
				if player.try_restoring_ability_score(Ability.WISDOM):
					ident = true
				if player.try_restoring_ability_score(Ability.DEXTERITY):
					ident = true
				if player.try_restoring_ability_score(Ability.CONSTITUTION):
					ident = true
				if player.try_restoring_ability_score(Ability.CHARISMA):
					ident = true
		FoodType.RATION, FoodType.BISCUIT, FoodType.VENISON:
				Profile.instance.message_print("That tastes good.")
				ident = true
		FoodType.DWARFBREAD:
				Profile.instance.message_print("You look at the dwarf bread, and don't feel quite so hungry anymore.")
				ident = true
		FoodType.SLIME_MOLD:
				savegame.command_engine.potion_effect(PotionType.SLIME_MOLD)
				ident = true
		FoodType.WAYBREAD:
				Profile.instance.message_print("That tastes good.")
				player.set_timed_poison(0)
				player.restore_health(Rng.roll_dice(4, 8))
				ident = true
		FoodType.PINT_OF_ALE, FoodType.PINT_OF_WINE:
				Profile.instance.message_print("That tastes good.")
				ident = true
		FoodType.WARPSTONE:
				Profile.instance.message_print("That tastes... funky.")
				player.genome.gain_mutation()
				if Rng.roll_die(3) == 1:
					player.genome.gain_mutation()
				if Rng.roll_die(3) == 1:
					player.genome.gain_mutation()
				ident = true
	player.notice_flags.set_bits(NoticeFlags.NOTICE_COMBINE | NoticeFlags.NOTICE_REORDER)
	# We've tried this type of object
	item.item_tried()
	# Learn its flavour if necessary
	if ident and !item.is_flavour_aware():
		item.become_flavour_aware()
		player.gain_experience((item_level + (player.level >> 1)) / player.level)
	# Dwarf bread isn't actually eaten so reduce our hunger and return early
	if item.sub_category == FoodType.DWARFBREAD:
		player.set_food(player.food + item.type_specific_value)
		return
	player.ancestry.consume_food(player, item)
	# Use up the item (if it fell to the floor this will have already been dealt with)
	if item_index >= 0:
		player.inventory.inven_item_increase(item_index, -1)
		player.inventory.inven_item_describe(item_index)
		player.inventory.inven_item_optimise(item_index)
	else:
		dungeon_level.floor_item_increase(0 - item_index, -1)
		dungeon_level.floor_item_describe(0 - item_index)
		dungeon_level.floor_item_optimise(0 - item_index)


func command_fire()->void:
	# Check that we're actually wielding a ranged weapon
	var missile_weapon:Item = player.inventory.items[InventorySlot.RANGED_WEAPON]
	if missile_weapon.category == 0:
		Profile.instance.message_print("You have nothing to fire with.")
		return
	# Get the ammunition to fire
	Inventory.item_filter_category = player.ammunition_item_category
	var item_index: int = savegame.get_item("Fire which item? ", false, true, true)
	if item_index == 99:
		return
	if item_index == -2:
		Profile.instance.message_print("You have nothing to fire.")
		return
	var ammunition_stack:Item = player.inventory.items[item_index] if item_index >= 0 else dungeon_level.dungeon_items.items[0 - item_index]
	var target_engine:TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	# Find out where we're aiming at
	var dir:int = target_engine.get_direction_with_aim()
	if dir == -1:
		return
	# Copy an ammunition piece from the stack...
	var individual_ammunition:Item = ammunition_stack.get_copy()
	individual_ammunition.count = 1
	# ...and reduced the amount in the stack
	if item_index >= 0:
		player.inventory.inven_item_increase(item_index, -1)
		player.inventory.inven_item_describe(item_index)
		player.inventory.inven_item_optimise(item_index)
	else:
		dungeon_level.floor_item_increase(0 - item_index, -1)
		dungeon_level.floor_item_optimise(0 - item_index)
	# Get the details of the shot
	var missile_name:String = individual_ammunition.description(false, 3)
	var missile_colour:int = individual_ammunition.item_type.colour
	var missile_character:String = individual_ammunition.item_type.character
	var shot_speed:int = player.MissileAttacksPerRound
	var shot_damage:int = Rng.roll_dice(individual_ammunition.damage_dice, individual_ammunition.damage_dice_sides) + individual_ammunition.bonus_damage + missile_weapon.bonus_damage
	var attack_bonus:int = player.attack_bonus + individual_ammunition.bonus_to_hit + missile_weapon.bonus_to_hit
	var chance_to_hit:int = player.skill_ranged + (attack_bonus * Constants.ITEM_TO_HIT_MULTIPLIER)
	# Damage multiplier depends on weapon
	var damage_multiplier:int = 1
	match missile_weapon.sub_category:
		BowType.SLING:
				damage_multiplier = 2
		BowType.SHORTBOW:
				damage_multiplier = 2
		BowType.LONGBOW:
				damage_multiplier = 3
		BowType.LIGHT_CROSSBOW:
				damage_multiplier = 3
		BowType.HEAVY_CROSSBOW:
				damage_multiplier = 4
	# Extra might gives us an increased multiplier
	if player.has_extra_might:
		damage_multiplier += 1
	shot_damage *= damage_multiplier
	# We're actually going to track the shot and draw it square by square
	var shot_distance: int = 10 + (5 * damage_multiplier)
	# Divide by our shot speed to give the equivalent of col shots per turn
	savegame.energy_use = 100 / shot_speed
	var row:int = player.map_row
	var col:int = player.map_col
	var target_col:int = player.map_col + (99 * dungeon_level.keypad_direction_col_offset[dir])
	var target_row:int = player.map_row + (99 * dungeon_level.keypad_direction_row_offset[dir])
	# Special for if we're hitting our own square
	if dir == 5 and target_engine.target_okay():
		target_col = savegame.target_col
		target_row = savegame.target_row
	savegame.notice_update_redraw()
	var hit_body:bool = false
	# Loop until we've reached our distance or hit something
	for cur_dis:int in shot_distance + 1:
		if row == target_row and col == target_col:
			break
		# Move a step towards the target
		var new_location:Location = dungeon_level.move_one_step_towards(row, col, player.map_row, player.map_col, target_row, target_col)
		var new_row:int = new_location.row
		var new_col:int = new_location.col
		# If we were blocked by a wall or something then stop short
		if not dungeon_level.grid_passable(new_row, new_col):
			break
		cur_dis += 1
		col = new_col
		row = new_row
		# If we can see the current projectile location, show it briefly
		if dungeon_level.panel_contains(row, col) and dungeon_level.player_can_see_bold(row, col):
			dungeon_level.print_character_at_map_location(missile_character, missile_colour, row, col)
			User.wait_for_animation()
			dungeon_level.redraw_single_location(row, col)
		else:
			# Pause even if we can't see it so it doesn't look weird if it goes in and out
			# of sight
			User.wait_for_animation()
		# Check if we might hit a monster (not necessarily the one we were aiming at)
		if dungeon_level.get_grid(row,col).monster_index != 0:
			var tile:GridTile = dungeon_level.get_grid(row,col)
			var monster:Monster = dungeon_level.dungeon_monsters.monsters[tile.monster_index]
			var monster_type:MonsterType = monster.monster_type
			var visible:bool = monster.is_visible
			hit_body = true
			# Check if we actually hit it
			if savegame.command_engine.player_check_ranged_hit_on_monster(chance_to_hit - cur_dis, monster_type.ac, monster.is_visible):
				var note_dies:String = " dies."
				if (monster_type.flags_3.is_set(MonsterFlags3.DEMON) or monster_type.flags_3.is_set(MonsterFlags3.UNDEAD) or
					monster_type.flags_3.is_set(MonsterFlags3.CTHULOID) or monster_type.flags_3.is_set(MonsterFlags2.STUPID) or
					monster_type.flags_3.is_set(MonsterFlags3.NON_LIVING) or "Evg".contains(monster_type.character) ):
					note_dies = " is destroyed."
				if not visible:
					Profile.instance.message_print("The %s finds a mark." % [missile_name])
				else:
					var monster_name:String = monster.describe(0)
					Profile.instance.message_print("The %s hits %s." % [missile_name, monster_name])
					if monster.is_visible:
						savegame.health_track(tile.monster_index)
					# Note that pets only get angry if they see us and we see them
					if monster.mind.is_set(MonsterMind.FRIENDLY):
						monster_name = monster.describe(0)
						Profile.instance.message_print("%s gets angry!" % [monster_name])
						monster.mind.clear_bits(MonsterMind.FRIENDLY)
				# Work out the damage done
				shot_damage = individual_ammunition.adjust_damage_for_monster_type(shot_damage, monster)
				shot_damage = savegame.command_engine.player_critical_ranged(individual_ammunition.weight, individual_ammunition.bonus_to_hit, shot_damage)
				if shot_damage < 0:
					shot_damage = 0
				if dungeon_level.dungeon_monsters.damage_monster(tile.monster_index, shot_damage, note_dies):
					# The monster is dead, so don't add further statuses or messages
					pass
				else:
					dungeon_level.dungeon_monsters.message_pain(tile.monster_index, shot_damage)
					if CommandEngine.fear and monster.is_visible:
						var monster_name:String = monster.describe(0)
						Profile.instance.message_print("%s flees in terror!" % [monster_name])
			# Stop the ammo's travel since we hit something
			break
	# If we hit something we have a chance to break the ammo, otherwise it just drops at
	# the end of its travel
	var j:int = individual_ammunition.breakage_chance() if hit_body else 0
	dungeon_level.drop_near(individual_ammunition, j, row, col)

func command_quaff_potion(item_index:int)->void:
	# Get an item if we didn't already have one
	Inventory.item_filter_category = ItemCategory.POTION
	if item_index == -999:
		item_index = savegame.get_item("Quaff which potion? ", true, true, true)
		if item_index == 99:
			return
		if item_index == -2:
			Profile.instance.message_print("You have no potions to quaff.")
			return
	var item:Item = player.inventory.items[item_index] if item_index >= 0 else dungeon_level.dungeon_items.items[0 - item_index]
	# Make sure the item is a potion
	Inventory.item_filter_category = ItemCategory.POTION
	if not player.inventory.item_matches_filter(item):
		Profile.instance.message_print("That is not a potion!")
		return
	Inventory.item_filter_category = 0
	# Drinking a potion costs a whole turn
	savegame.energy_use = 100
	var item_level:int = item.item_type.level
	# Do the actual potion effect
	var identified:bool = savegame.command_engine.potion_effect(item.sub_category)
	# Skeletons are messy drinkers
	if player.ancestry.spills_potions and Rng.roll_die(12) == 1:
		Profile.instance.message_print("Some of the fluid falls through your jaws!")
		savegame.spell_effects_handler.potion_smash_effect(0, player.map_row, player.map_col, item.sub_category)
	player.notice_flags.set_bits(NoticeFlags.NOTICE_COMBINE | NoticeFlags.NOTICE_REORDER)
	# We may now know the potion's type
	item.item_tried()
	if identified and !item.is_flavour_aware():
		item.become_flavour_aware()
		player.gain_experience((item_level + (player.level >> 1)) / player.level)
	# Most potions give us a bit of food too
	player.set_food(player.food + item.type_specific_value)
	var channeled: bool = false
	# If we're a channeler, we might be able to spend vis instead of using it up
	if player.spellcasting.casting_type == CastingType.CHANNELLING:
		channeled = savegame.command_engine.command_channel(item)
	if !channeled:
		# We didn't channel it, so use up one potion from the stack
		if item_index >= 0:
			player.inventory.inven_item_increase(item_index, -1)
			player.inventory.inven_item_describe(item_index)
			player.inventory.inven_item_optimise(item_index)
		else:
			dungeon_level.floor_item_increase(0 - item_index, -1)
			dungeon_level.floor_item_describe(0 - item_index)
			dungeon_level.floor_item_optimise(0 - item_index)

func command_read_scroll(item_index:int)->void:
	# Make sure we're in a situation where we can read
	if player.timed_blindness != 0:
		Profile.instance.message_print("You can't see anything.")
		return
	if dungeon_level.no_light():
		Profile.instance.message_print("You have no light to read by.")
		return
	if player.timed_confusion != 0:
		Profile.instance.message_print("You are too confused!")
		return
	# If we weren't passed in an item, prompt for one
	Inventory.item_filter_category = ItemCategory.SCROLL
	if item_index == -999:
		item_index = savegame.get_item("Read which scroll? ", true, true, true)
		if item_index == 99:
			return
		if item_index == -2:
			Profile.instance.message_print("You have no scrolls to read.")
			return
	var item:Item = player.inventory.items[item_index] if item_index >= 0 else dungeon_level.dungeon_items.items[0 - item_index]
	# Make sure the item is actually a scroll
	Inventory.item_filter_category = ItemCategory.SCROLL
	if not player.inventory.item_matches_filter(item):
		Profile.instance.message_print("That is not a scroll!")
		Inventory.item_filter_category = 0
		return
	Inventory.item_filter_category = 0
	# Scrolls use a full turn
	savegame.energy_use = 100
	var identified: bool = false
	var item_level: int = item.item_type.level
	var used_up: int = true
	# Most types of scroll are obvious
	match item.sub_category:
		ScrollType.DARKNESS:
				if not player.has_blindness_resistance and !player.has_dark_resistance:
					player.set_timed_blindness(player.timed_blindness + 3 + Rng.roll_die(5))
				if savegame.spell_effects_handler.unlight_area(10, 3):
					identified = true
		ScrollType.AGGRAVATE_MONSTER:
				Profile.instance.message_print("There is a high pitched humming noise.")
				savegame.spell_effects_handler.aggravate_monsters(1)
				identified = true
		ScrollType.CURSE_ARMOUR:
				if savegame.command_engine.curse_armour():
					identified = true
		ScrollType.CURSE_WEAPON:
				if savegame.command_engine.curse_weapon():
					identified = true
		ScrollType.SUMMON_MONSTER:
				for i:int in Rng.roll_die(3):
					if dungeon_level.dungeon_monsters.summon_specific(player.map_row, player.map_col, savegame.total_difficulty(), 0):
						identified = true
		ScrollType.SUMMON_UNDEAD:
				for i:int in Rng.roll_die(3):
					if (dungeon_level.dungeon_monsters.summon_specific(player.map_row, player.map_col, savegame.total_difficulty(),SummonType.SUMMON_UNDEAD)):
						identified = true
		ScrollType.TRAP_CREATION:
				if savegame.spell_effects_handler.trap_creation():
					identified = true
		ScrollType.PHASE_DOOR:
				savegame.spell_effects_handler.teleport_player(10)
				identified = true
		ScrollType.TELEPORT:
				savegame.spell_effects_handler.teleport_player(100)
				identified = true
		ScrollType.TELEPORT_LEVEL:
				savegame.spell_effects_handler.TeleportPlayerLevel()
				identified = true
		ScrollType.WORD_OF_RECALL:
				player.toggle_recall()
				identified = true
		ScrollType.IDENTIFY:
				identified = true
				if not savegame.spell_effects_handler.identify_item():
					used_up = false
		ScrollType.STAR_IDENTIFY:
				identified = true
				if not savegame.spell_effects_handler.identify_fully():
					used_up = false
		ScrollType.REMOVE_CURSE:
				if savegame.spell_effects_handler.remove_curse():
					Profile.instance.message_print("You feel as if someone is watching over you.")
					identified = true
		ScrollType.STAR_REMOVE_CURSE:
				savegame.spell_effects_handler.remove_all_curse()
				identified = true
		ScrollType.ENCHANT_ARMOR:
				identified = true
				if not savegame.spell_effects_handler.enchant_spell(0, 0, 1):
					used_up = false
		ScrollType.ENCHANT_WEAPON_TO_HIT:
				if not savegame.spell_effects_handler.enchant_spell(1, 0, 0):
					used_up = false
				identified = true
		ScrollType.ENCHANT_WEAPON_TO_DAM:
				if not savegame.spell_effects_handler.enchant_spell(0, 1, 0):
					used_up = false
				identified = true
		ScrollType.STAR_ENCHANT_ARMOR:
				if not savegame.spell_effects_handler.enchant_spell(0, 0, Rng.roll_die(3) + 2):
					used_up = false
				identified = true
		ScrollType.STAR_ENCHAN_TWEAPON:
				if not savegame.spell_effects_handler.enchant_spell(Rng.roll_die(3), Rng.roll_die(3), 0):
					used_up = false
				identified = true
		ScrollType.RECHARGING:
				if not savegame.spell_effects_handler.recharge(60):
					used_up = false
				identified = true
		ScrollType.LIGHT:
				if savegame.spell_effects_handler.light_area(Rng.roll_dice(2, 8), 2):
					identified = true
		ScrollType.MAPPING:
				dungeon_level.map_area()
				identified = true
		ScrollType.DETECT_GOLD:
				if savegame.spell_effects_handler.detect_treasure():
					identified = true
				if savegame.spell_effects_handler.detect_objects_gold():
					identified = true
		ScrollType.DETECT_ITEM:
				if savegame.spell_effects_handler.detect_items_normal():
					identified = true
		ScrollType.DETECT_TRAP:
				if savegame.spell_effects_handler.detect_traps():
					identified = true
		ScrollType.DETECT_DOOR:
				if savegame.spell_effects_handler.detect_doors():
					identified = true
				if savegame.spell_effects_handler.detect_stairs():
					identified = true
		ScrollType.DETECT_INVIS:
				if savegame.spell_effects_handler.detect_monsters_invis():
					identified = true
		ScrollType.SATISFY_HUNGER:
				if player.set_food(Constants.FOOD_GORGED - 1):
					identified = true
		ScrollType.BLESSING:
				if player.set_timed_blessing(player.timed_blessing + Rng.roll_die(12) + 6):
					identified = true
		ScrollType.HOLY_CHANT:
				if player.set_timed_blessing(player.timed_blessing + Rng.roll_die(24) + 12):
					identified = true
		ScrollType.HOLYP_RAYER:
				if player.set_timed_blessing(player.timed_blessing + Rng.roll_die(48) + 24):
					identified = true
		ScrollType.MONSTER_CONFUSION:
				if not player.has_confusing_touch:
					Profile.instance.message_print("Your hands begin to glow.")
					player.has_confusing_touch = true
					identified = true
		ScrollType.PROTECTION_FROM_EVIL:
				var i:int = 3 * player.level
				if player.set_timed_protection_from_evil(player.timed_protection_from_evil + Rng.roll_die(25) + i):
					identified = true
		ScrollType.RUNE_OF_PROTECTION:
				savegame.spell_effects_handler.elder_sign()
				identified = true
		ScrollType.TRAP_DOOR_DESTRUCTION:
				if savegame.spell_effects_handler.destroy_doors_touch():
					identified = true
		ScrollType.STAR_DESTRUCTION:
				savegame.spell_effects_handler.destroy_area(player.map_row, player.map_col, 15)
				identified = true
		ScrollType.DISPEL_UNDEAD:
				if savegame.spell_effects_handler.dispel_undead(60):
					identified = true
		ScrollType.CARNAGE:
				savegame.spell_effects_handler.carnage(true)
				identified = true
		ScrollType.MASS_CARNAGE:
				savegame.spell_effects_handler.mass_carnage(true)
				identified = true
		ScrollType.ACQUIREMENT:
				dungeon_level.acquirement(player.map_row, player.map_col, 1, true)
				identified = true
		ScrollType.STAR_ACQUIREMENT:
				dungeon_level.acquirement(player.map_row, player.map_col, Rng.roll_die(2) + 1, true)
				identified = true
		ScrollType.FIRE:
				savegame.spell_effects_handler.fire_ball(ProjectileFire.new(savegame.spell_effects_handler), 0, 150, 4)
				if not (player.timed_fire_resistance != 0 or player.has_fire_resistance or player.has_fire_immunity):
					player.take_hit(50 + Rng.roll_die(50), "a Scroll of Fire")
				identified = true
		ScrollType.ICE:
				savegame.spell_effects_handler.fire_ball(ProjectileIce.new(savegame.spell_effects_handler), 0, 175, 4)
				if not (player.timed_cold_resistance != 0 or player.has_cold_resistance or player.has_cold_immunity):
					player.take_hit(100 + Rng.roll_die(100), "a Scroll of Ice")
				identified = true
		ScrollType.CHAOS:
				savegame.spell_effects_handler.fire_ball(ProjectileChaos.new(savegame.spell_effects_handler), 0, 222, 4)
				if not player.has_chaos_resistance:
					player.take_hit(111 + Rng.roll_die(111), "a Scroll of Chaos")
				identified = true
		ScrollType.RUMOR:
				Profile.instance.message_print("There is message on the scroll. It says:")
				Profile.instance.message_print("")
				savegame.command_engine.get_rumour()
				identified = true
		ScrollType.INVOCATION:
				var patron:Patron = Patron.random_patron()
				Profile.instance.message_print("You invoke the secret name of %s." % [patron.long_name])
				patron.get_reward(savegame, player, dungeon_level)
				identified = true
		ScrollType.ARTIFACT:
				savegame.spell_effects_handler.artifact_scroll()
				identified = true
	player.notice_flags.set_bits(NoticeFlags.NOTICE_COMBINE | NoticeFlags.NOTICE_REORDER)
	# We might have just identified the scroll
	item.item_tried()
	if identified and !item.is_flavour_aware():
		item.become_flavour_aware()
		player.gain_experience((item_level + (player.level >> 1)) / player.level)
	var channeled: bool = false
	# Channellers can use vis instead of the scroll being used up
	if player.spellcasting.casting_type == CastingType.CHANNELLING:
		channeled = savegame.command_engine.command_channel(item)
	if not channeled:
		if not used_up:
			return
		# If it wasn't used up then decrease the amount in the stack
		if item_index >= 0:
			player.inventory.inven_item_increase(item_index, -1)
			player.inventory.inven_item_describe(item_index)
			player.inventory.inven_item_optimise(item_index)
		else:
			dungeon_level.floor_item_increase(0 - item_index, -1)
			dungeon_level.floor_item_describe(0 - item_index)
			dungeon_level.floor_item_optimise(0 - item_index)

func command_refill(item_index:int)->void:
	# Make sure we actually have a light source to refuel
	var light_source:Item = player.inventory.items[InventorySlot.LIGHT_SOURCE]
	if light_source.category != ItemCategory.LIGHT:
		Profile.instance.message_print("You are not wielding a light.")
	elif light_source.sub_category == LightType.LANTERN:
		refill_lamp(item_index)
	elif light_source.sub_category == LightType.TORCH:
		refill_torch(item_index)
	else:
		Profile.instance.message_print("Your light cannot be refilled.")

func command_throw(damage_multiplier:int)->void:
	# Get an item to throw
	var item_index: int = savegame.get_item("Throw which item? ", false, true, true)
	if item_index == 99:
		return
	if item_index == -2:
		Profile.instance.message_print("You have nothing to throw.")
		return
	var item:Item = player.inventory.items[item_index] if item_index >= 0 else dungeon_level.dungeon_items.items[0 - item_index]
	var target_engine:TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
	var dir:int = target_engine.get_direction_with_aim()
	if dir == -1:
		return
	# Copy a single item from the item stack as the thrown item
	var missile:Item = item.get_copy()
	missile.count = 1
	if item_index >= 0:
		player.inventory.inven_item_increase(item_index, -1)
		player.inventory.inven_item_describe(item_index)
		player.inventory.inven_item_optimise(item_index)
	else:
		dungeon_level.floor_item_increase(0 - item_index, -1)
		dungeon_level.floor_item_optimise(0 - item_index)
	var missile_name:String = missile.description(false, 3)
	var missile_colour:int = missile.item_type.colour
	var missile_character:String = missile.item_type.character
	# Thrown distance is based on the weight of the missile
	var multiplier:int = 10 + (2 * (damage_multiplier - 1))
	var divider:int = missile.weight if missile.weight > 10 else 10
	var throw_distance:int = (player.ability_scores[Ability.STRENGTH].str_attack_speed_component() + 20) * multiplier / divider
	if throw_distance > 10:
		throw_distance = 10
	# Work out the damage done
	var damage: int = Rng.roll_dice(missile.damage_dice, missile.damage_dice_sides) + missile.bonus_damage
	damage *= damage_multiplier
	var chance: int = player.skill_throwing + (player.attack_bonus * Constants.ITEM_TO_HIT_MULTIPLIER)
	# Throwing something always uses a full turn, even if you can make multiple missile attacks
	savegame.energy_use = 100
	var row: int = player.map_row
	var col: int = player.map_col
	var target_col: int = player.map_col + (99 * dungeon_level.keypad_direction_col_offset[dir])
	var target_row: int = player.map_row + (99 * dungeon_level.keypad_direction_row_offset[dir])
	if dir == 5 and target_engine.target_okay():
		target_col = savegame.target_col
		target_row = savegame.target_row
	savegame.notice_update_redraw()
	var new_row: int = player.map_row
	var new_col: int = player.map_col
	var hit_body: bool = false
	# Send the thrown object in the right direction one square at a time
	for cur_dis:int in throw_distance + 1:
		# If we reach our limit, stop the object moving
		if row == target_row and col == target_col:
			break
		var new_location:Location = dungeon_level.move_one_step_towards(row, col, player.map_row, player.map_col, target_row, target_col)
		new_row = new_location.row
		new_col = new_location.col
		# If we hit a wall or something stop moving
		if not dungeon_level.grid_passable(new_row, new_col):
			break
		cur_dis += 1
		col = new_col
		row = new_row
		# If we can see, display the thrown item with a suitable delay
		if dungeon_level.panel_contains(row, col) and dungeon_level.player_can_see_bold(row, col):
			dungeon_level.print_character_at_map_location(missile_character, missile_colour, row, col)
			User.wait_for_animation()
			dungeon_level.redraw_single_location(row, col)
		else:
			# Delay even if we don't see it, so it doesn't look weird when it passes behind something
			User.wait_for_animation()
		# If there's a monster in the way, we might hit it regardless of whether or not it
		# is our intended target
		if dungeon_level.get_grid(row,col).monster_index != 0:
			var tile:GridTile = dungeon_level.get_grid(row,col)
			var monster:Monster = dungeon_level.dungeon_monsters.monsters[tile.monster_index]
			var monster_type:MonsterType = monster.monster_type
			var visible:bool = monster.is_visible
			hit_body = true
			# See if it actually hit the monster
			if savegame.command_engine.player_check_ranged_hit_on_monster(chance - cur_dis, monster_type.ac, monster.is_visible):
				var note_dies: String = " dies."
				if (monster_type.flags_3.is_set(MonsterFlags3.DEMON) or monster_type.flags_3.is_set(MonsterFlags3.UNDEAD) or
					monster_type.flags_3.is_set(MonsterFlags3.CTHULOID) or monster_type.flags_3.is_set(MonsterFlags2.STUPID) or
					monster_type.flags_3.is_set(MonsterFlags3.NON_LIVING) or "Evg".contains(monster_type.character) ):
					note_dies = " is destroyed."
				# Let the player know what happened
				if not visible:
					Profile.instance.message_print("The %s finds a mark." % [missile_name])
				else:
					var monster_name: String = monster.describe(0)
					Profile.instance.message_print("The %s hits %s." % [missile_name, monster_name])
					if monster.is_visible:
						savegame.health_track(tile.monster_index)
				# Adjust the damage for the particular monster type
				damage = missile.adjust_damage_for_monster_type(damage, monster)
				damage = savegame.command_engine.player_critical_ranged(missile.weight, missile.bonus_to_hit, damage)
				if damage < 0:
					damage = 0
				if dungeon_level.dungeon_monsters.damage_monster(tile.monster_index, damage, note_dies):
					# The monster is dead, so don't add further statuses or messages
					pass
				else:
					# Let the player know what happens to the monster
					dungeon_level.dungeon_monsters.message_pain(tile.monster_index, damage)
					if monster.mind.is_set(MonsterMind.FRIENDLY) and missile.item_type.item_category != ItemCategory.POTION:
						var monster_name: String = monster.describe(0)
						Profile.instance.message_print("%s gets angry!" % [monster_name])
						monster.mind.clear_bits(MonsterMind.FRIENDLY)
					if CommandEngine.fear and monster.is_visible:
						var monster_name: String = monster.describe(0)
						Profile.instance.message_print("%s flees in terror!" % [monster_name])
	var chance_to_break:int = missile.breakage_chance() if hit_body else 0
	if missile.item_type.item_category == ItemCategory.POTION:
		if hit_body or !dungeon_level.grid_passable(new_row, new_col) or Rng.roll_die(100) < chance_to_break:
			Profile.instance.message_print("The %s shatters!" % [missile_name])
			if savegame.spell_effects_handler.potion_smash_effect(1, row, col, missile.sub_category):
				if dungeon_level.get_grid(row,col).monster_index != 0 and (dungeon_level.dungeon_monsters.monsters[dungeon_level.get_grid(row,col).monster_index].mind.is_set(MonsterMind.FRIENDLY)):
					var monster_name: String = dungeon_level.dungeon_monsters.monsters[dungeon_level.get_grid(row,col).monster_index].describe(0)
					Profile.instance.message_print("%s gets angry!" % [monster_name])
					dungeon_level.dungeon_monsters.monsters[dungeon_level.get_grid(row,col).monster_index].mind.clear_bits(MonsterMind.FRIENDLY)
			return
		chance_to_break = 0
	# Drop the item on the floor
	dungeon_level.drop_near(missile, chance_to_break, row, col)

func command_use_staff(item_index:int)->void:
	# Get an item if we weren't passed one
	Inventory.item_filter_category = ItemCategory.STAFF
	if item_index == -999:
		item_index = savegame.get_item("Use which staff? ", false, true, true)
		if item_index == 99:
			return
		if item_index == -2:
			Profile.instance.message_print("You have no staff to use.")
			return
	var item:Item = player.inventory.items[item_index] if item_index >= 0 else dungeon_level.dungeon_items.items[0 - item_index]
	# Make sure the item is actually a staff
	Inventory.item_filter_category = ItemCategory.STAFF
	if not player.inventory.item_matches_filter(item):
		Profile.instance.message_print("That is not a staff!")
		Inventory.item_filter_category = 0
		return
	Inventory.item_filter_category = 0
	# We can't use a staff from the floor
	if item_index < 0 and item.count > 1:
		Profile.instance.message_print("You must first pick up the staffs.")
		return
	# Using a staff costs a full turn
	savegame.energy_use = 100
	var identified: bool = false
	var item_level: int = item.item_type.level
	# We have a chance of the device working equal to skill (halved if confused) - item
	# level (capped at 50)
	var chance: int = player.skill_use_device
	if player.timed_confusion != 0:
		chance /= 2
	chance -= 50 if item_level > 50 else item_level
	# Always a small chance of it working
	if chance < USE_DEVICE and Rng.roll_die(USE_DEVICE - chance + 1) == 1:
		chance = USE_DEVICE
	# Check to see if we use it properly
	if chance < USE_DEVICE or Rng.roll_die(chance) < USE_DEVICE:
		Profile.instance.message_print("You failed to use the staff properly.")
		return
	# Make sure it has charges left
	if item.type_specific_value <= 0:
		Profile.instance.message_print("The staff has no charges left.")
		item.identify_flags.set_bits(IdentifyFlags.EMPTY)
		return
	var use_charge:bool = true
	# Do the specific effect for the type of staff
	match item.sub_category:
		StaffType.DARKNESS:
				if not player.has_blindness_resistance and !player.has_dark_resistance:
					if player.set_timed_blindness(player.timed_blindness + 3 + Rng.roll_die(5)):
						identified = true
				if savegame.spell_effects_handler.unlight_area(10, 3):
					identified = true
		StaffType.SLOWNESS:
				if player.set_timed_slow(player.timed_slow + Rng.roll_die(30) + 15):
					identified = true
		StaffType.HASTE_MONSTERS:
				if savegame.spell_effects_handler.haste_monsters():
					identified = true
		StaffType.SUMMONING:
				for k:int in Rng.roll_die(4):
					if dungeon_level.dungeon_monsters.summon_specific(player.map_row, player.map_col, savegame.total_difficulty(), 0):
						identified = true
		StaffType.TELEPORTATION:
				savegame.spell_effects_handler.teleport_player(100)
				identified = true
		StaffType.IDENTIFY:
				if not savegame.spell_effects_handler.itentify_item():
					use_charge = false
				identified = true
		StaffType.REMOVE_CURSE:
				if savegame.spell_effects_handler.remove_curse():
					if player.timed_blindness == 0:
						Profile.instance.message_print("The staff glows blue for a moment...")
					identified = true
		StaffType.STARLIGHT:
				if player.timed_blindness == 0:
					Profile.instance.message_print("The end of the staff glows brightly...")
				for k:int in 8:
					savegame.spell_effects_handler.light_line(dungeon_level.ordered_direction[k])
				identified = true
		StaffType.LIGHT:
				if savegame.spell_effects_handler.light_area(Rng.roll_dice(2, 8), 2):
					identified = true
		StaffType.MAPPING:
				dungeon_level.map_area()
				identified = true
		StaffType.DETECT_GOLD:
				if savegame.spell_effects_handler.detect_treasure():
					identified = true
				if savegame.spell_effects_handler.detect_objects_gold():
					identified = true
		StaffType.DETECT_ITEM:
				if savegame.spell_effects_handler.detect_items_normal():
					identified = true
		StaffType.DETECT_TRAP:
				if savegame.spell_effects_handler.detect_traps():
					identified = true
		StaffType.DETECT_DOOR:
				if savegame.spell_effects_handler.detect_doors():
					identified = true
				if savegame.spell_effects_handler.detect_stairs():
					identified = true
		StaffType.DETECT_INVIS:
				if savegame.spell_effects_handler.detect_monsters_invis():
					identified = true
		StaffType.DETECT_EVIL:
				if savegame.spell_effects_handler.detect_monsters_evil():
					identified = true
		StaffType.CURE_LIGHT:
				if player.restore_health(Rng.roll_die(8)):
					identified = true
		StaffType.CURING:
				if player.set_timed_blindness(0):
					identified = true
				if player.set_timed_poison(0):
					identified = true
				if player.set_timed_confusion(0):
					identified = true
				if player.set_timed_stun(0):
					identified = true
				if player.set_timed_bleeding(0):
					identified = true
				if player.set_timed_hallucinations(0):
					identified = true
		StaffType.HEALING:
				if player.restore_health(300):
					identified = true
				if player.set_timed_stun(0):
					identified = true
				if player.set_timed_bleeding(0):
					identified = true
		StaffType.THE_MAGI:
				if player.try_restoring_ability_score(Ability.INTELLIGENCE):
					identified = true
				if player.power < player.max_power:
					player.power = player.max_power
					player.fractional_power = 0
					identified = true
					Profile.instance.message_print("Your feel your head clear.")
					player.redraw_needed.set_bits(RedrawFlags.REDRAW_POWER)
		StaffType.SLEEP_MONSTERS:
				if savegame.spell_effects_handler.sleep_monsters():
					identified = true
		StaffType.SLOW_MONSTERS:
				if savegame.spell_effects_handler.slow_monsters():
					identified = true
		StaffType.SPEED:
				if player.timed_haste == 0:
					if player.set_timed_haste(Rng.roll_die(30) + 15):
						identified = true
				else:
					player.set_timed_haste(player.timed_haste + 5)
		StaffType.PROBING:
				savegame.spell_effects_handler.probing()
				identified = true
		StaffType.DISPEL_EVIL:
				if savegame.spell_effects_handler.dispel_evil(60):
					identified = true
		StaffType.POWER:
				if savegame.spell_effects_handler.dispel_monsters(120):
					identified = true
		StaffType.HOLINESS:
				if savegame.spell_effects_handler.dispel_evil(120):
					identified = true
				var k:int = 3 * player.level
				if player.set_timed_protection_from_evil(player.timed_protection_from_evil + Rng.roll_die(25) + k):
					identified = true
				if player.set_timed_poison(0):
					identified = true
				if player.set_timed_fear(0):
					identified = true
				if player.restore_health(50):
					identified = true
				if player.set_timed_stun(0):
					identified = true
				if player.set_timed_bleeding(0):
					identified = true
		StaffType.CARNAGE:
				savegame.spell_effects_handler.carnage(true)
				identified = true
		StaffType.EARTHQUAKES:
				savegame.spell_effects_handler.earthquake(player.map_row, player.map_col, 10)
				identified = true
		StaffType.DESTRUCTION:
				savegame.spell_effects_handler.destroy_area(player.map_row, player.map_col, 15)
				identified = true
	player.notice_flags.set_bits(NoticeFlags.NOTICE_COMBINE | NoticeFlags.NOTICE_REORDER)
	# We might now know what the staff does
	item.item_tried()
	if identified and !item.is_flavour_aware():
		item.become_flavour_aware()
	if not use_charge:
		return
	# Channellers can use vis instead of a charge
	var channeled:bool = false
	if player.spellcasting.casting_type == CastingType.CHANNELLING:
		channeled = savegame.command_engine.command_channel(item)
	if not channeled:
		# Use the actual charge
		item.type_specific_value -= 1
		# If the staff was part of a stack, separate it from the rest
		if item_index >= 0 and item.count > 1:
			var single_staff:Item = item.get_copy()
			single_staff.count = 1
			item.type_specific_value += 1
			item.count -= 1
			player.weight_carried -= single_staff.weight
			item_index = player.inventory.inven_carry(single_staff)
			Profile.instance.message_print("You unstack your staff.")
		# Let the player know what happened
		if item_index >= 0:
			player.inventory.report_charge_usage_from_inventory(item_index)
		else:
			dungeon_level.report_charge_usage_from_floor(0 - item_index)

func command_zap_rod(item_index:int)->void:
	# Get the item if we weren't passed it
	Inventory.item_filter_category = ItemCategory.ROD
	if item_index == -999:
		item_index = savegame.get_item("Zap which rod? ", false, true, true)
		if item_index == 99:
			return
		if item_index == -2:
			Profile.instance.message_print("You have no rod to zap.")
			return
	var item:Item = player.inventory.items[item_index] if item_index >= 0 else dungeon_level.dungeon_items.items[0 - item_index]
	# Make sure the item is actually a rod
	Inventory.item_filter_category = ItemCategory.ROD
	if not player.inventory.item_matches_filter(item):
		Profile.instance.message_print("That is not a rod!")
		Inventory.item_filter_category = 0
		return
	Inventory.item_filter_category = 0
	# Rods can't be used from the floor
	if item_index < 0 and item.count > 1:
		Profile.instance.message_print("You must first pick up the rods.")
		return
	# We may need to aim the rod
	var dir:int = 5
	if (item.sub_category >= RodType.MINIMUM_AIMED and item.sub_category != RodType.HAVOC) or not item.is_flavour_aware():
		var target_engine:TargetEngine = TargetEngine.new_target_engine(player, dungeon_level, savegame)
		dir = target_engine.get_direction_with_aim()
		if dir == -1:
			return
	# Using a rod takes a whole turn
	savegame.energy_use = 100
	var identified:bool = false
	var item_level:int = item.item_type.level
	# Chance to successfully use it is skill (halved if confused) - rod level (capped at 50)
	var chance:int = player.skill_use_device
	if player.timed_confusion != 0:
		chance /= 2
	chance -= 50 if item_level > 50 else item_level
	# There's always a small chance of success
	if chance < USE_DEVICE and Rng.roll_die(USE_DEVICE - chance + 1) == 1:
		chance = USE_DEVICE
	# Do the actual check
	if chance < USE_DEVICE or Rng.roll_die(chance) < USE_DEVICE:
		Profile.instance.message_print("You failed to use the rod properly.")
		return
	# Rods only have a single charge but recharge over time
	if item.type_specific_value != 0:
		Profile.instance.message_print("The rod is still charging.")
		return
	# Do the rod-specific effect
	var use_charge: bool = true
	match item.sub_category:
		RodType.DETECT_TRAP:
				if savegame.spell_effects_handler.detect_traps():
					identified = true
				item.type_specific_value = 10 + Rng.roll_die(10)
		RodType.DETECT_DOOR:
				if savegame.spell_effects_handler.detect_doors():
					identified = true
				if savegame.spell_effects_handler.detect_stairs():
					identified = true
				item.type_specific_value = 70
		RodType.IDENTIFY:
				identified = true
				if not savegame.spell_effects_handler.itentify_item():
					use_charge = false
				item.type_specific_value = 10
		RodType.RECALL:
				player.toggle_recall()
				identified = true
				item.type_specific_value = 60
		RodType.ILLUMINATION:
				if savegame.spell_effects_handler.light_area(Rng.roll_dice(2, 8), 2):
					identified = true
				item.type_specific_value = 10 + Rng.roll_die(11)
		RodType.MAPPING:
				dungeon_level.map_area()
				identified = true
				item.type_specific_value = 99
		RodType.DETECTION:
				savegame.spell_effects_handler.detect_all()
				identified = true
				item.type_specific_value = 99
		RodType.PROBING:
				savegame.spell_effects_handler.probing()
				identified = true
				item.type_specific_value = 50
		RodType.CURING:
				if player.set_timed_blindness(0):
					identified = true
				if player.set_timed_poison(0):
					identified = true
				if player.set_timed_confusion(0):
					identified = true
				if player.set_timed_stun(0):
					identified = true
				if player.set_timed_bleeding(0):
					identified = true
				if player.set_timed_hallucinations(0):
					identified = true
				item.type_specific_value = 999
		RodType.HEALING:
				if player.restore_health(500):
					identified = true
				if player.set_timed_stun(0):
					identified = true
				if player.set_timed_bleeding(0):
					identified = true
				item.type_specific_value = 999
		RodType.RESTORATION:
				if player.restore_level():
					identified = true
				if player.try_restoring_ability_score(Ability.STRENGTH):
					identified = true
				if player.try_restoring_ability_score(Ability.INTELLIGENCE):
					identified = true
				if player.try_restoring_ability_score(Ability.WISDOM):
					identified = true
				if player.try_restoring_ability_score(Ability.DEXTERITY):
					identified = true
				if player.try_restoring_ability_score(Ability.CONSTITUTION):
					identified = true
				if player.try_restoring_ability_score(Ability.CHARISMA):
					identified = true
				item.type_specific_value = 999
		RodType.SPEED:
				if player.timed_haste == 0:
					if player.set_timed_haste(Rng.roll_die(30) + 15):
						identified = true
				else:
					player.set_timed_haste(player.timed_haste + 5)
				item.type_specific_value = 99
		RodType.TELEPORT_AWAY:
				if savegame.spell_effects_handler.teleport_monster(dir):
					identified = true
				item.type_specific_value = 25
		RodType.DISARMING:
				if savegame.spell_effects_handler.disarm_trap(dir):
					identified = true
				item.type_specific_value = 15 + Rng.roll_die(15)
		RodType.LIGHT:
				Profile.instance.message_print("A line of blue shimmering light appears.")
				savegame.spell_effects_handler.light_line(dir)
				identified = true
				item.type_specific_value = 9
		RodType.SLEEP_MONSTER:
				if savegame.spell_effects_handler.sleep_monster(dir):
					identified = true
				item.type_specific_value = 18
		RodType.SLOW_MONSTER:
				if savegame.spell_effects_handler.slow_monster(dir):
					identified = true
				item.type_specific_value = 20
		RodType.DRAIN_LIFE:
				if savegame.spell_effects_handler.drain_life(dir, 75):
					identified = true
				item.type_specific_value = 23
		RodType.POLYMORPH:
				if savegame.spell_effects_handler.poly_monster(dir):
					identified = true
				item.type_specific_value = 25
		RodType.ACID_BOLT:
				savegame.spell_effects_handler.fire_bolt_or_beam(10, ProjectileAcid.new(savegame.spell_effects_handler), dir, Rng.roll_dice(6, 8))
				identified = true
				item.type_specific_value = 12
		RodType.ELEC_BOLT:
				savegame.spell_effects_handler.fire_bolt_or_beam(10, ProjectileElectricity.new(savegame.spell_effects_handler), dir, Rng.roll_dice(3, 8))
				identified = true
				item.type_specific_value = 11
		RodType.FIRE_BOLT:
				savegame.spell_effects_handler.fire_bolt_or_beam(10, ProjectileFire.new(savegame.spell_effects_handler), dir, Rng.roll_dice(8, 8))
				identified = true
				item.type_specific_value = 15
		RodType.COLD_BOLT:
				savegame.spell_effects_handler.fire_bolt_or_beam(10, ProjectileCold.new(savegame.spell_effects_handler), dir, Rng.roll_dice(5, 8))
				identified = true
				item.type_specific_value = 13
		RodType.ACID_BALL:
				savegame.spell_effects_handler.fire_ball(ProjectileAcid.new(savegame.spell_effects_handler), dir, 60, 2)
				identified = true
				item.type_specific_value = 27
		RodType.ELEC_BALL:
				savegame.spell_effects_handler.fire_ball(ProjectileElectricity.new(savegame.spell_effects_handler), dir, 32, 2)
				identified = true
				item.type_specific_value = 23
		RodType.FIRE_BALL:
				savegame.spell_effects_handler.fire_ball(ProjectileFire.new(savegame.spell_effects_handler), dir, 72, 2)
				identified = true
				item.type_specific_value = 30
		RodType.COLD_BALL:
				savegame.spell_effects_handler.fire_ball(ProjectileCold.new(savegame.spell_effects_handler), dir, 48, 2)
				identified = true
				item.type_specific_value = 25
		RodType.HAVOC:
				savegame.spell_effects_handler.call_chaos()
				identified = true
				item.type_specific_value = 250
	player.notice_flags.set_bits(NoticeFlags.NOTICE_COMBINE | NoticeFlags.NOTICE_REORDER)
	# We may have just discovered what the rod does
	item.item_tried()
	if identified and !item.is_flavour_aware():
		item.become_flavour_aware()
		player.gain_experience((item_level + (player.level >> 1)) / player.level)
	# We may not have actually used a charge
	if not use_charge:
		item.type_specific_value = 0
		return
	# Channellers can spend vis instead of a charge
	var channeled: bool = false
	if player.spellcasting.casting_type == CastingType.CHANNELLING:
		channeled = savegame.command_engine.command_channel(item)
		if channeled:
			item.type_specific_value = 0
	if not channeled:
		# If the rod was part of a stack, remove it
		if item_index >= 0 and item.count > 1:
			var single_rod:Item = item.get_copy()
			single_rod.count = 1
			item.type_specific_value = 0
			item.count -= 1
			player.weight_carried -= single_rod.weight
			player.inventory.inven_carry(single_rod)
			Profile.instance.message_print("You unstack your rod.")

func refill_lamp(item_index:int)->void:
	# Get an item if we don't already have one
	savegame.item_filter_function = savegame.command_engine.ItemFilterLanternFuel
	if item_index == -999:
		item_index = savegame.get_item("Refill with which flask? ", true, true, true)
		if item_index == 99:
			return
		if item_index == -2:
			Profile.instance.message_print("You have no flasks of oil.")
			return
	var fuel_source:Item = player.inventory.items[item_index] if item_index >= 0 else dungeon_level.dungeon_items.items[0 - item_index]
	# Make sure our item is suitable fuel
	savegame.item_filter_function = savegame.command_engine.item_filter_lantern_fuel
	if not player.inventory.item_matches_filter(fuel_source):
		Profile.instance.message_print("You can't refill a lantern from that!")
		savegame.item_filter_function = Callable()
		return
	savegame.item_filter_function = Callable()
	# Refilling takes half a turn
	savegame.energy_use = 50
	var lamp:Item = player.inventory.items[InventorySlot.LIGHT_SOURCE]
	# Add the fuel
	lamp.type_specific_value += fuel_source.type_specific_value
	Profile.instance.message_print("You fuel your lamp.")
	# Check for overfilling
	if lamp.type_specific_value >= Constants.FUEL_LANTERN:
		lamp.type_specific_value = Constants.FUEL_LANTERN
		Profile.instance.message_print("Your lamp is full.")
	# Update the inventory
	if item_index >= 0:
		player.inventory.inven_item_increase(item_index, -1)
		player.inventory.inven_item_describe(item_index)
		player.inventory.inven_item_optimise(item_index)
	else:
		dungeon_level.floor_item_increase(0 - item_index, -1)
		dungeon_level.floor_item_describe(0 - item_index)
		dungeon_level.floor_item_optimise(0 - item_index)
	player.updates_needed.set_bits(UpdateFlags.UPDATE_TORCH_RADIUS)

func refill_torch(item_index:int)->void:
# Get an item if we don't already have one
	savegame.item_filter_function = savegame.command_engine.item_filter_torch_fuel
	if item_index == -999:
		item_index = savegame.get_item("Refuel with which torch? ", false, true, true)
		if item_index == 99:
			return
		if item_index == -2:
			Profile.instance.message_print("You have no extra torches.")
			return
	var fuel_source:Item = player.inventory.items[item_index] if item_index >= 0 else dungeon_level.dungeon_items.items[0 - item_index]
	# Check that our fuel is suitable
	savegame.item_filter_function = savegame.command_engine.item_filter_torch_fuel
	if not player.inventory.item_matches_filter(fuel_source):
		Profile.instance.message_print("You can't refill a torch with that!")
		savegame.item_filter_function = Callable()
		return
	savegame.item_filter_function = Callable()
	# Refueling takes half a turn
	savegame.energy_use = 50
	var torch: Item = player.inventory.items[InventorySlot.LIGHT_SOURCE]
	# Add the fuel
	torch.type_specific_value += fuel_source.type_specific_value + 5
	Profile.instance.message_print("You combine the torches.")
	# Check for overfilling
	if torch.type_specific_value >= Constants.FUEL_TORCH:
		torch.type_specific_value = Constants.FUEL_TORCH
		Profile.instance.message_print("Your torch is fully fueled.")
	else:
		Profile.instance.message_print("Your torch glows more brightly.")
	# Update the player's inventory
	if item_index >= 0:
		player.inventory.inven_item_increase(item_index, -1)
		player.inventory.inven_item_describe(item_index)
		player.inventory.inven_item_optimise(item_index)
	else:
		dungeon_level.floor_item_increase(0 - item_index, -1)
		dungeon_level.floor_item_describe(0 - item_index)
		dungeon_level.floor_item_optimise(0 - item_index)
	player.updates_needed.set_bits(UpdateFlags.UPDATE_TORCH_RADIUS)
