# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name Journal extends RefCounted

const ITEM_FLAG_NAMES: Array[String] = [
"Add Str", "Add Int", "Add Wis", "Add Dex", "Add Con", "Add Cha", "", "", "Stealth", "Searching",
"Infravis", "Tunnelling", "Speed", "Blows", "Chaotic", "Vampiric", "Slay Anim.", "Slay Evil",
"Slay Und.", "Slay Demon", "Slay Orc", "Slay Troll", "Slay Giant", "Slay Drag.", "Kill Drag.", "Sharpness",
"Impact", "Poison Brd", "Acid Brand", "Elec Brand", "Fire Brand", "Cold Brand", "Sust Str", "Sust Int",
"Sust Wis", "Sust Dex", "Sust Con", "Sust Cha", "", "", "Imm Acid", "Imm Elec", "Imm Fire", "Imm Cold",
"", "Reflect", "Free Act", "Hold Life", "Res Acid", "Res Elec", "Res Fire", "Res Cold", "Res Pois",
"Res Fear", "Res Light", "Res Dark", "Res Blind", "Res Conf", "Res Sound", "Res Shard", "Res Neth",
"Res Nexus", "Res Chaos", "Res Disen", "Aura Fire", "Aura Elec", "", "Anti-Theft", "Anti-Tele", "Anti-Magic",
"Wraithform", "Evil Curse", "", "", "", "", "Levitate", "Light",
"See Invis", "Telepathy", "Digestion", "Regen", "Xtra Might", "Xtra Shots", "Ign Acid", "Ign Elec",
"Ign Fire", "Ign Cold", "Activate", "Drain Exp", "Teleport", "Aggravate", "Blessed", "Cursed", "Hvy Curse",
"Perm Curse"
]

var player:Player
var dungeon_level:DungeonLevel
var savegame:SaveGame

var menu_colours:Array[int]
var menu_indices:Array[int]
var menu_item:Array[String]
var menu_length:int

func _init(the_player:Player, the_level:DungeonLevel, the_savegame:SaveGame) -> void:
	player = the_player
	dungeon_level = the_level
	savegame = the_savegame


func show_menu() -> void:
	Console.save_screen()
	Console.set_background(BackgroundImage.PAPER)
	Console.set_mask (-1, -1, -1, -1)
	while true:
		Console.clear_screen()
		Console.print_at( 0, 1, "Journal",Ink.BLUE)
		Console.print_at( 1, 1, "=======",Ink.BLUE)
		Console.print_at( 3, 0, "(a) Abilities",Ink.BLUE)
		Console.print_at( 4, 0, "(d) Deities",Ink.BLUE)
		Console.print_at( 5, 0, "(k) Kill Count",Ink.BLUE)
		Console.print_at( 6, 0, "(m) Mutations",Ink.BLUE)
		Console.print_at( 7, 0, "(p) Pets",Ink.BLUE)
		Console.print_at( 8, 0, "(q) Quests",Ink.BLUE)
		Console.print_at( 9, 0, "(r) Word of Recall",Ink.BLUE)
		Console.print_at(10, 0, "(s) Monsters Seen",Ink.BLUE)
		Console.print_at(11, 0, "(u) Uniques", Ink.BLUE)
		Console.print_at(12, 0, "(w) Worthless Items", Ink.BLUE)
		Console.print_at(43, 1, "[Select a journal section, or Escape to finish.]", Ink.ORANGE)
		var k:String = User.get_key()
		if k == "Escape":
			break
		match k:
			'a', 'A':
				journal_abilities()
			'd', 'D':
				journal_deities()
			'p', 'P':
				journal_pets()
			'q', 'Q':
				journal_quests()
			's', 'S':
				journal_monsters()
			'u', 'U':
				journal_uniques()
			'k', 'K':
				journal_kills()
			'm', 'M':
				journal_mutations()
			'r', 'R':
				journal_recall()
			'w', 'W':
				journal_worthless_items()
	Console.set_background(BackgroundImage.DEFAULT)
	Console.set_mask (1,13,42,66)
	Console.restore_screen()

func display_flags(mode:int) -> void:
	var n:int
	var top:int = 20 * mode
	var f_1:FlagSet = FlagSet.new()
	var f_2:FlagSet = FlagSet.new()
	var f_3:FlagSet = FlagSet.new()
	var b:Array[FlagSet]
	for i:int in 84:
		b.append(FlagSet.new())
	for i:int in range(InventorySlot.MELEE_WEAPON,InventorySlot.TOTAL):
		n = i - InventorySlot.MELEE_WEAPON
		var item:Item = player.inventory.items[i]
		item.get_known_flags(f_1, f_2, f_3)
		b[n*6+0].set_bits(f_1.low_order_bits())
		b[n*6+1].set_bits(f_1.high_order_bits())
		b[n*6+2].set_bits(f_2.low_order_bits())
		b[n*6+3].set_bits(f_2.high_order_bits())
		b[n*6+4].set_bits(f_3.low_order_bits())
		b[n*6+5].set_bits(f_3.high_order_bits())
	n = 13
	player.get_abilities_as_item_flags(f_1, f_2, f_3)
	b[n*6+0].set_bits(f_1.low_order_bits())
	b[n*6+1].set_bits(f_1.high_order_bits())
	b[n*6+2].set_bits(f_2.low_order_bits())
	b[n*6+3].set_bits(f_2.high_order_bits())
	b[n*6+4].set_bits(f_3.low_order_bits())
	b[n*6+5].set_bits(f_3.high_order_bits())
	for col:int in 3:
		CharacterViewer.print_equippy(player, top + 2, (col * 26) + 12)
		Console.print_at(top + 3, (col * 26) + 12, "abcdefghijklm@", Ink.BLUE)
		for row: int in 16:
			var name:String = ITEM_FLAG_NAMES[(48 * mode) + (16 * col) + row]
			if name == "":
				continue
			var base_colour:int = Ink.BLUE
			for i:int in 14:
				if b[i*6+(3 * mode) + col].is_set(1 << row):
					base_colour = Ink.GREEN
			Console.print_at(top + row + 4, (col * 26) + 1, name, base_colour)
			Console.print_at(top + row + 4, (col * 26) + 11, ':', base_colour)
			for i: int in 14:
				var a:int = Ink.GREY
				var c:String = "."
				if b[i*6+(3 * mode) + col].is_set(1 << row):
					a = base_colour
					c = "+"
				Console.print_at(top + row + 4, (col * 26) + 12 + i, c, a)

func display_monster(monster_type_index:int, num:int, of:int)->void:
	for identifier:String in SymbolDisplay.SYMBOL_IDENTIFICATION:
		if identifier.left(1) == Profile.instance.monster_types.types[monster_type_index].character:
			var name:String = identifier.substr(2)
			var buf:String = "Monster Type: %s (%d of %d)" % [name, num + 1, of]
			Console.print_at(3, 0, buf, Ink.BLUE)
			break
	display_monster_header(5, 0, monster_type_index)
	var monster_type: MonsterType = Profile.instance.monster_types.types[monster_type_index]
	monster_type.knowledge.display_body(monster_type, 6,0,Ink.BROWN)

func display_monster_header(row:int, col:int, monster_type_index:int)->void:
	var monster_type:MonsterType = Profile.instance.monster_types.types[monster_type_index]
	var c1:String = monster_type.character
	var a1:int = monster_type.colour
	var name: String = ""
	if monster_type.flags_1.is_clear(MonsterFlags1.UNIQUE):
		name += "The "
	name += monster_type.name
	name += " ('"
	Console.print_at(row, col, name, Ink.BROWN)
	Console.print_following(c1, a1)
	Console.print_following("')", Ink.BROWN)

func journal_abilities()->void:
	Console.clear_screen()
	display_flags(0)
	display_flags(1)
	Console.print_at(43, 1, "[Press any key to finish.]", Ink.ORANGE)
	User.get_key()

func journal_deities()->void:
	Console.clear_screen()
	Console.print_at(0, 1, "Standings with Deities", Ink.BLUE)
	Console.print_at(1, 1, "======================", Ink.BLUE)
	var row:int = 3
	var patron:God = null
	for deity_name:int in player.religion.gods:
		var deity:God = player.religion.gods[deity_name]
		var text:String = deity.short_name
		if deity.is_patron:
			patron = deity
		var adjusted:int = deity.adjusted_favour()
		match adjusted:
			0:
				if deity.favour < -1000:
					text += " hates you"
				elif deity.favour < -100:
					text += " dislikes you"
				elif deity.favour < -20:
					text += " is annoyed by you"
				else:
					text += " is indifferent to you"
			1:
				text += " has noticed you"
			2:
				text += " is watching you"
			3:
				text += " approves of you"
			4:
				text += " likes you"
			5:
				text += " holds you dear"
			6:
				text += " loves you"
			7:
				text += " adores you"
			8:
				text += " prizes you"
			9:
				text += " dotes on you"
			10:
				text += " cherishes you"
		if adjusted > 0:
			match deity.god_name:
				GodName.LOBON:
					text += " (%d%% chance to avoid ability drain)" % [adjusted * 10]
				GodName.NATH_HORTHATH:
					text += " (+%d%% max health)" % [adjusted * 10]
				GodName.HAGARG_RYONIS:
					text += " (%d%% chance to avoid poison/life drain)" % [adjusted * 10]
				GodName.TAMASH:
					text += " (+%d%% max power)" % [adjusted * 10]
				GodName.ZO_KALAR:
					text += " (%d%% chance to avoid death)" % [adjusted * 10]
		text += "."
		Console.print_at(row, 1, text, Ink.BLUE)
		row += 1
	if patron != null:
		Console.print_at(12, 1, "You are a follower of %s." % [patron.long_name], Ink.BLUE)
		Console.print_at(13, 1, "Over time, your standing with %s will revert to approval." % [patron.short_name], Ink.BLUE)
		Console.print_at(14, 1, "Your standing with other deities will revert to annoyance.", Ink.BLUE)
	else:
		Console.print_at(12, 1, "Over time, your standing with all deities will revert back to indifference.", Ink.BLUE)
	Console.print_at(43, 1, "[Press any key to finish.]", Ink.ORANGE)
	User.get_key()

func journal_kills()->void:
	var names:Array[String]
	var counts:Array[int]
	var unique:Array[bool]
	var max_count:int = 0
	var total:int = 0
	for monster_type: MonsterType in Profile.instance.monster_types.types:
		if monster_type.flags_1.is_set(MonsterFlags1.UNIQUE):
			var dead:bool = monster_type.max_num == 0
			if dead:
				total += 1
				names.append(monster_type.name)
				counts.append(1)
				unique.append(true)
		else:
			if monster_type.knowledge.r_kills > 0:
				total += monster_type.knowledge.r_kills
				names.append(monster_type.name)
				counts.append(monster_type.knowledge.r_kills)
				unique.append(false)
	# Not the most efficient sort, but we're sorting fewer than a thousand values
	# and we want to keep the original order as much as possible
	for i:int in names.size()-1:
		for j: int in range(names.size()-1, i, -1):
			if counts[j] <= counts[j - 1]:
				continue
			var temp_name:String = names[j]
			names[j] = names[j - 1]
			names[j - 1] = temp_name
			var temp_count:int = counts[j]
			counts[j] = counts[j - 1]
			counts[j - 1] = temp_count
			var temp_unique:int = unique[j]
			unique[j] = unique[j - 1]
			unique[j - 1] = temp_unique
	var first:int = 0
	while true:
		var buf:String
		Console.clear_screen()
		Console.print_at(0, 1, "Kill Count", Ink.BLUE)
		Console.print_at(1, 1, "==========", Ink.BLUE)
		if names.size() == 0:
			Console.print_at(3, 0, "You haven't killed anything yet!", Ink.BLUE)
		else:
			for i:int in range (first, first+38):
				if i < names.size():
					if unique[i]:
						buf = "You have killed %s" % [names[i]]
					else:
						if counts[i] > 1:
							var plural:String = Util.pluralise_monster_name(names[i])
							buf = "You have killed %d %s" % [counts[i], plural]
						else:
							buf = "You have killed 1 %s" % [names[i]]
					Console.print_at(i - first + 3, 0, buf, Ink.BLUE)
		buf = "Total Kills: %d" % [total]
		Console.print_at(41, 0, buf, Ink.BLUE)
		Console.print_at(43, 1, "[Use up and down to navigate list, and Escape to finish.]", Ink.ORANGE)
		var c:String = User.get_key(true)
		if c == "Escape":
			break
		if c == "8" or c == "4":
			first -= 1
			if first < 0:
				first = 0
		if c == "2" or c == "6":
			first += 1
			if first > max_count - 38:
				first = max_count - 38
			if first < 0:
				first = 0

func journal_monsters() -> void:
	var seen:Array[int]
	var filtered:Array[int]
	var max_seen:int = 0
	var filter_mask:Array[bool]
	var filter_lookup:Array[int]
	var used_filters:Array[int]
	var max_used_filter:int = 0
	# First set up our filter mask for characters
	filter_mask.resize(256)
	filter_mask.fill(false)
	filter_lookup.resize(256)
	filter_lookup.fill(0)
	used_filters.resize(256)
	used_filters.fill(0)
	filtered.resize(256)
	filtered.fill(0)
	# Run through our seen monsters, noting that we've seen them and setting our filter for them
	for i:int in Profile.instance.monster_types.types.size():
		if Profile.instance.monster_types.types[i].knowledge.r_sights != 0 or player.is_wizard:
			seen.append(i)
			max_seen += 1
			var symbol:int = ord(Profile.instance.monster_types.types[i].character)
			if symbol < 256:
				if not filter_mask[symbol]:
					filter_mask[symbol] = true
	# Run through our filters, gathering which have been used
	for i:int in 256:
		if not filter_mask[i]:
			continue
		used_filters[max_used_filter] = i
		filter_lookup[i] = max_used_filter
		max_used_filter += 1
	# If we've not seen any monsters, report that and quit
	if max_seen == 0:
		Console.clear_screen()
		Console.print_at(0, 1, "Monsters Seen", Ink.BLUE)
		Console.print_at(1, 1, "=============", Ink.BLUE)
		Console.print_at(3, 0, "You haven't seen any monsters yet!", Ink.BLUE)
		Console.print_at(43, 1, "[Press any key to finish]", Ink.ORANGE)
		User.get_key()
		return
	# Start at the first filter used
	var current_filter_index:int = 0
	var current_filter:int = used_filters[0]
	var use_max:int = false
	# Loop until we quit
	while true:
		var max_filtered:int = 0
		# Find all the seen monsters matching our current filter and add them to the filtered array
		for i:int in max_seen:
			if Profile.instance.monster_types.types[seen[i]].character == char(current_filter):
				filtered[max_filtered] = seen[i]
				max_filtered += 1
		var current_index:int = 0
		if use_max:
			current_index = max_filtered - 1
		var c:String
		while true:
			Console.clear_screen()
			Console.print_at(0, 1, "Monsters Seen", Ink.BLUE)
			Console.print_at(1, 1, "=============", Ink.BLUE)
			display_monster(filtered[current_index], current_index, max_filtered)
			Console.print_at(43, 1, "[Up and down to change type, left and right to change monster, Esc to finish]", Ink.ORANGE)
			c = User.get_key(true)
			if c == '4':
				if current_index > 0:
					current_index -=1
				else:
					c = '8'
					break
			elif c == '6':
				if current_index < max_filtered - 1:
					current_index += 1
				else:
					c = '2'
					break
			elif c == "Escape" or c == '2' or c == '8':
				break
			else:
				if filter_mask[ord(c)]:
					current_filter_index = filter_lookup[ord(c)]
					current_filter = used_filters[current_filter_index]
					break
		if c == '8':
			if current_filter_index > 0:
				current_filter_index -= 1
				current_filter = used_filters[current_filter_index]
				use_max = true
			else:
				use_max = false
		if c == '2':
			if current_filter_index < max_used_filter - 1:
				current_filter_index += 1
				current_filter = used_filters[current_filter_index]
				use_max = false
			else:
				use_max = true
		if c == "Escape":
			break


func journal_mutations()-> void:
	var features:Array[String] = player.genome.get_mutation_list()
	var max_feature:int = features.size()
	var first:int = 0
	while true:
		Console.clear_screen()
		Console.print_at(0, 1, "Mutations", Ink.BLUE)
		Console.print_at(1, 1, "=========", Ink.BLUE)
		if max_feature == 0:
			Console.print_at(3, 0, "You have no mutations.", Ink.BLUE)
		else:
			for i:int in range (first, first + 38):
				if i < max_feature:
					Console.print_at(i - first + 3, 0, features[i], Ink.BLUE)
		Console.print_at(43, 1, "[Use up and down to navigate list, and Escape to finish.]", Ink.ORANGE)
		var c:String = User.get_key(true)
		if c == "Escape":
			break
		if c == '8' or c == '4':
			first -= 1
			if first < 0:
				first = 0
		if c == '2' or c == '6':
			first += 1
			if first > max_feature - 38:
				first = max_feature - 38
			if first < 0:
				first = 0

func journal_pets() -> void:
	var pet_names:Array[String]
	var pets:int = 0
	for monster_index:int in range (dungeon_level.monster_count_max, 0, -1):
		var monster:Monster = dungeon_level.dungeon_monsters.monsters[monster_index]
		if monster.mind.is_clear(MonsterMind.FRIENDLY):
			continue
		pet_names.append(monster.monster_type.name)
		pets += 1
	var first: int = 0
	while true:
		Console.clear_screen()
		Console.print_at(0, 1, "Pets", Ink.BLUE)
		Console.print_at(1, 1, "====", Ink.BLUE)
		if pets == 0:
			Console.print_at(3, 0, "You have no pets.", Ink.BLUE)
		else:
			for i:int in range(first, first+38):
				if i < pets:
					Console.print_at(i - first + 3, 0, pet_names[i], Ink.BLUE)
		Console.print_at(43, 1, "[Use up and down to navigate list, and Escape to finish.]", Ink.ORANGE)
		var c:String = User.get_key(true)
		if c == "Escape":
			break
		if c == '8' or c == '4':
			first -= 1
			if first < 0:
				first = 0
		if c == '2' or c == '6':
			first += 1
			if first > pets - 38:
				first = pets - 38
			if first < 0:
				first = 0

func journal_quests()->void:
	Console.clear_screen()
	Console.print_at(0, 1, "Outstanding Quests", Ink.BLUE)
	Console.print_at(1, 1, "==================", Ink.BLUE)
	var lev:Array[int]
	var first:Array[int]
	for i:int in savegame.dungeons.size():
		first.append(-1)
		lev.append(-1)
	for i:int in savegame.quests.quests.size():
		var q:Quest = savegame.quests.quests[i]
		if q.depth > 0:
			var dungeon:int = q.dungeon
			if first[dungeon] == -1 or q.depth < lev[dungeon]:
				first[dungeon] = i
				lev[dungeon] = q.depth
	var row:int = 3
	for i:int in savegame.dungeons.size():
		if first[i] != -1:
			var line:String = savegame.quests.describe_quest(first[i])
			Console.print_at(row, 0, line, Ink.BLUE)
			row += 1
	if row == 3:
		Console.print_at(row, 0, "Congratulations! You have completed all the quests.", Ink.BLUE)
	Console.print_at(43, 1, "[Press any key to finish.]", Ink.ORANGE)
	User.get_key()

func journal_recall() -> void:
	Console.clear_screen()
	Console.print_at(0, 1, "Word of Recall", Ink.BLUE)
	Console.print_at(1, 1, "==============", Ink.BLUE)
	var recall_town:String = savegame.towns[player.town_with_house].name if player.town_with_house > -1 else  savegame.current_town.name
	var recall_dungeon:String = savegame.dungeons[savegame.recall_dungeon].name
	var recall_depth:int = player.max_dlv[savegame.recall_dungeon]
	Console.print_at(3, 0, "Your Word of Recall position is level %d of %s." % [recall_depth, recall_dungeon], Ink.BLUE)
	Console.print_at(4, 0, "Your home town is %s." % [recall_town], Ink.BLUE)
	if player.town_with_house > -1:
		recall_town = "your house in " + savegame.dungeons[player.town_with_house].shortname
	if savegame.current_depth == 0:
		Console.print_at(6, 0, "If you recall now, you will return to level %d of %s." % [recall_depth, recall_dungeon], Ink.BROWN)
	else:
		Console.print_at(6, 0, "If you recall now, you will return to %s." % [recall_town], Ink.BROWN)
	var description:String = "(If you own a house, your home town is always considered to be the town containing that house "
	description += "and you will be transported directly to your house. "
	description += "If not, your home town is updated each time you visit a new town, and you will be transported to a random location in that town. "
	description += "Your Word of Recall position has its dungeon location updated "
	description += "only when you recall from a new dungeon or tower but has its level updated "
	description += "each time you reach a new level within that dungeon or tower. In either case, you will be transported to a random location on the dungeon or tower level.)"
	Console.print_wrap(8,0, description, Ink.BLUE)
	Console.print_at(43, 1, "[Press any key to finish.]", Ink.ORANGE)
	User.get_key()

func journal_uniques()->void:
	var names:Array[String]
	var alive:Array[bool]
	var max_count:int = 0
	for monster_type:MonsterType in Profile.instance.monster_types.types:
		if monster_type.flags_1.is_set(MonsterFlags1.UNIQUE) and (monster_type.knowledge.r_sights > 0 or player.is_wizard):
			names.append(monster_type.name)
			var dead:bool = monster_type.max_num == 0
			alive.append(not dead)
			max_count += 1
	var first:int = 0
	while true:
		Console.clear_screen()
		Console.print_at(0, 1, "Unique Foes", Ink.BLUE)
		Console.print_at(1, 1, "===========", Ink.BLUE)
		if max_count == 0:
			Console.print_at(3, 0, "You know of no unique foes!", Ink.BLUE)
		for i:int in range(first, first + 38):
			if i < max_count:
				var buf:String = ("%s is alive." % [names[i]]) if alive[i] else ("%s is dead." % [names[i]])
				Console.print_at(i - first + 3, 0, buf, Ink.BLUE)
		Console.print_at(43, 1, "[Use up and down to navigate list, and Escape to finish.]", Ink.ORANGE)
		var c:String = User.get_key(true)
		if c == "Escape":
			break
		if c == '8' or c == '4':
			first -= 1
			if first < 0:
				first = 0
		if c == '2' or c == '6':
			first += 1
			if first > max_count - 38:
				first = max_count - 38
			if first < 0:
				first = 0

func journal_worthless_items() -> void:
	Console.clear_screen()
	Console.print_at(0, 1, "Worthless Items", Ink.BLUE)
	Console.print_at(1, 1, "===============", Ink.BLUE)
	var text: String = "Items marked in red "
	text += "will be considered 'worthless' and you will stomp on them (destroying them) rather than "
	text += "picking them up. Destroying (using 'k' or 'K') a worthless object will be done automatically "
	text += "without you being prompted. Items will only be destroyed if they are on the floor or in your "
	text += "inventory. Items you are wielding will never be destroyed (giving you chance to improve their "
	text += "quality to a non-worthless level)."
	Console.print_wrap(3,0, text, Ink.BLUE)
	menu_item = []
	menu_colours = []
	menu_indices = []
	for pair:ItemCategoryDescription in Tables.item_category_descriptions:
		menu_item.append(pair.name)
		menu_colours.append(Ink.BLUE)
	menu_length = menu_item.size()
	var menu:int = menu_length / 2
	while true:
		menu_display(menu)
		Console.print_at(43, 1, "[Up/Down = select item type, Left/Right = forward/back.]", Ink.ORANGE)
		while true:
			var c:String = User.get_key(true)
			if c == '8' and menu > 0:
				menu -= 1
				break
			if c == '2' and menu < menu_length - 1:
				menu += 1
				break
			if c == '6':
				worthless_item_type_selection(Tables.item_category_descriptions[menu].item_category)
				menu_item = []
				menu_colours = []
				menu_indices = []
				for pair:ItemCategoryDescription in Tables.item_category_descriptions:
					menu_item.append(pair.name)
					menu_colours.append(Ink.BLUE)
				menu_length = menu_item.size()
				break
			if c == '4':
				return

func menu_display(current:int)->void:
	Console.clear_from(9)
	Console.print_at(25, 0, "=>", Ink.ORANGE)
	var desc:String = ""
	var desc_colour: int = Ink.BROWN
	for i:int in menu_length:
		var row:int = 25 + i - current
		if row < 10 or row > 40:
			continue
		var a:int = menu_colours[i]
		if i == current:
			match a:
				Ink.BLUE:
					a = Ink.BRIGHT_BLUE
					desc = "(This type of item has further sub-types.)"
				Ink.GREEN:
					a = Ink.BRIGHT_GREEN
					desc = "(This type of item has value to you.)"
				_:
					a = Ink.BRIGHT_RED
					desc = "(This type of item is worthless to you.)"
			desc_colour = a
		Console.print_at(row, 2, menu_item[i], a)
	Console.print_at(25, 33, desc, desc_colour)

func strip_down_name(name:String)->String:
	var val:String = name.replace("~", "")
	val = val.replace("%", "")
	val = val.replace("&", "")
	return val.strip_edges()

func worthless_item_chest_selection(item_type:ItemType) -> void:
	var quality_text:Array[String] = ["Empty", "Unlocked", "Locked", "Trapped"]
	menu_length = 0
	menu_item = []
	menu_colours = []
	for i:int in 4:
		menu_item.append(quality_text[i])
		menu_colours.append(Ink.RED if item_type.stompable[i] else Ink.GREEN)
	menu_length = 4
	var menu:int = 1
	while true:
		menu_display(menu)
		Console.print_at(43, 1, "[Up/Down = select item type, Left/Right = forward/back.]", Ink.ORANGE)
		while true:
			var c:String = User.get_key(true)
			if c == '8' and menu > 0:
				menu -= 1
				break
			if c == '2' and menu < menu_length - 1:
				menu += 1
				break
			if c == '6':
				item_type.stompable[menu] = not item_type.stompable[menu]
				menu_colours[menu] = Ink.RED if item_type.stompable[menu] else Ink.GREEN
				break
			if c == '4':
				return

func worthless_item_quality_selection(item_type:ItemType)->void:
	var quality_text:Array[String] = ["Bad", "Average", "Good", "Excellent"]
	menu_length = 0
	menu_item = []
	menu_colours = []
	for i:int in 4:
		menu_item.append(quality_text[i])
		menu_colours.append(Ink.RED if item_type.stompable[i] else Ink.GREEN)
	menu_length = 4
	var menu:int = 1
	while true:
		menu_display(menu)
		Console.print_at(43, 1, "[Up/Down = select item type, Left/Right = forward/back.]", Ink.ORANGE)
		while true:
			var c:String = User.get_key(true)
			if c == '8' and menu > 0:
				menu -= 1
				break
			if c == '2' and menu < menu_length - 1:
				menu += 1
				break
			if c == '6':
				item_type.stompable[menu] = not item_type.stompable[menu]
				menu_colours[menu] = Ink.RED if item_type.stompable[menu] else Ink.GREEN
				break
			if c == '4':
				return

func worthless_item_type_selection(item_category:int)->void:
	menu_length = 0
	menu_item = []
	menu_colours = []
	for item_type_index:int in savegame.item_types.types.size():
		var item_type:ItemType = savegame.item_types.types[item_type_index]
		if item_type.item_category == item_category:
			if item_type.flags_3.is_set(ItemFlags3.INSTA_ART):
				continue
			menu_item.append(strip_down_name(item_type.name))
			if item_type.has_quality() or item_type.item_category == ItemCategory.CHEST:
				menu_colours.append(Ink.BLUE)
			else:
				menu_colours.append (Ink.RED if item_type.stompable[0] else Ink.GREEN)
			menu_indices.append(item_type_index)
			menu_length += 1
	var menu:int = menu_length / 2
	while true:
		menu_display(menu)
		Console.print_at(43, 1, "[Up/Down = select item type, Left/Right = forward/back.]", Ink.ORANGE)
		while true:
			var c:String = User.get_key(true)
			if c == '8' and menu > 0:
				menu -= 1
				break
			if c == '2' and menu < menu_length - 1:
				menu += 1
				break
			if c == '6':
				var item_type:ItemType = savegame.item_types.types[menu_indices[menu]]
				if item_type.has_quality():
					worthless_item_quality_selection(item_type)
					menu_length = 0
					menu_item = []
					menu_colours = []
					for item_type_index:int in savegame.item_types.types.size():
						item_type = savegame.item_types.types[item_type_index]
						if item_type.item_category == item_category:
							if item_type.flags_3.is_set(ItemFlags3.INSTA_ART):
								continue
							menu_item.append(strip_down_name(item_type.name))
							if item_type.has_quality() or item_type.item_category == ItemCategory.CHEST:
								menu_colours.append(Ink.BLUE)
							else:
								menu_colours.append (Ink.RED if item_type.stompable[0] else Ink.GREEN)
							menu_indices.append(item_type_index)
							menu_length += 1
				elif item_type.item_category == ItemCategory.CHEST:
					worthless_item_chest_selection(item_type)
					menu_length = 0
					menu_item = []
					menu_colours = []
					for item_type_index:int in savegame.item_types.types.size():
						item_type = savegame.item_types.types[item_type_index]
						if item_type.item_category == item_category:
							if item_type.flags_3.is_set(ItemFlags3.INSTA_ART):
								continue
							menu_item.append(strip_down_name(item_type.name))
							if item_type.has_quality() or item_type.item_category == ItemCategory.CHEST:
								menu_colours.append(Ink.BLUE)
							else:
								menu_colours.append (Ink.RED if item_type.stompable[0] else Ink.GREEN)
							menu_indices.append(item_type_index)
							menu_length += 1
				else:
					item_type.stompable[0] = not item_type.stompable[0]
					menu_colours[menu] = Ink.RED if item_type.stompable[0] else Ink.GREEN
				break
			if c == '4':
				return
