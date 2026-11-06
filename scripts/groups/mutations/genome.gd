# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name Genome
extends Resource

@export var natural_attacks: Array[Mutation]
@export var armour_class_bonus: int
@export var chaos_gift: bool
@export var cha_bonus: int
@export var cha_override: int
@export var con_bonus: int
@export var dex_bonus: int
@export var elec_hit: bool
@export var esp: bool
@export var feather_fall: bool
@export var fire_hit: bool
@export var free_action: bool
@export var infravision_bonus: int
@export var int_bonus: int
@export var magic_resistance: bool
@export var regen: bool
@export var res_fear: bool
@export var res_time: bool
@export var search_bonus: int
@export var speed_bonus: int
@export var stealth_bonus: int
@export var str_bonus: int
@export var suppress_regen: bool
@export var sustain_all: bool
@export var vulnerable: bool
@export var wis_bonus: int
@export var not_possessed: Array[Mutation]
@export var possessed: Array[Mutation]


func _init() -> void:
	possessed = []
	not_possessed = []

	# active mutations
	not_possessed.append(MutationBanish.new())
	not_possessed.append(MutationBerserk.new())
	not_possessed.append(MutationBlink.new())
	not_possessed.append(MutationBreatheFire.new())
	not_possessed.append(MutationColdTouch.new())
	not_possessed.append(MutationDazzle.new())
	not_possessed.append(MutationDetectCurses.new())
	not_possessed.append(MutationEarthquake.new())
	not_possessed.append(MutationEatMagic.new())
	not_possessed.append(MutationEatRock.new())
	not_possessed.append(MutationGrowMould.new())
	not_possessed.append(MutationHypnoGaze.new())
	not_possessed.append(MutationIllumination.new())
	not_possessed.append(MutationLaserEyes.new())
	not_possessed.append(MutationLauncher.new())
	not_possessed.append(MutationMidasTouch.new())
	not_possessed.append(MutationMindBlast.new())
	not_possessed.append(MutationPanicHit.new())
	not_possessed.append(MutationPolymorph.new())
	not_possessed.append(MutationRadiation.new())
	not_possessed.append(MutationRecall.new())
	not_possessed.append(MutationResist.new())
	not_possessed.append(MutationShriek.new())
	not_possessed.append(MutationSmellMetal.new())
	not_possessed.append(MutationSmellMonsters.new())
	not_possessed.append(MutationSpitAcid.new())
	not_possessed.append(MutationSterility.new())
	not_possessed.append(MutationSwapPosition.new())
	not_possessed.append(MutationTelekinesis.new())
	not_possessed.append(MutationVampirism.new())
	not_possessed.append(MutationVolTeleport.new())
	not_possessed.append(MutationWeighMagic.new())
	# Passive Mutations
	not_possessed.append(MutationAthritis.new())
	not_possessed.append(MutationBlankFace.new())
	not_possessed.append(MutationBlubber.new())
	not_possessed.append(MutationEsp.new())
	not_possessed.append(MutationExtraEyes.new())
	not_possessed.append(MutationExtraLegs.new())
	not_possessed.append(MutationExtraNoise.new())
	not_possessed.append(MutationFearless.new())
	not_possessed.append(MutationFireBody.new())
	not_possessed.append(MutationFleshRot.new())
	not_possessed.append(MutationHyperInt.new())
	not_possessed.append(MutationHyperStrength.new())
	not_possessed.append(MutationInfravision.new())
	not_possessed.append(MutationIronSkin.new())
	not_possessed.append(MutationLightningTouch.new())
	not_possessed.append(MutationLimber.new())
	not_possessed.append(MutationMagicResistant.new())
	not_possessed.append(MutationMotion.new())
	not_possessed.append(MutationNormality.new())
	not_possessed.append(MutationPaperSkin.new())
	not_possessed.append(MutationPuny.new())
	not_possessed.append(MutationRegen.new())
	not_possessed.append(MutationResiliant.new())
	not_possessed.append(MutationScales.new())
	not_possessed.append(MutationShortLeg.new())
	not_possessed.append(MutationSillyVoice.new())
	not_possessed.append(MutationSustain.new())
	not_possessed.append(MutationTimeResistance.new())
	not_possessed.append(MutationVulnerability.new())
	not_possessed.append(MutationWartySkin.new())
	not_possessed.append(MutationWings.new())
	not_possessed.append(MutationWitheredBrain.new())
	# Random Mutations
	not_possessed.append(MutationAlcohol.new())
	not_possessed.append(MutationAttractAnimals.new())
	not_possessed.append(MutationAttractDemons.new())
	not_possessed.append(MutationAttractDragons.new())
	not_possessed.append(MutationBanishAll.new())
	not_possessed.append(MutationBeak.new())
	not_possessed.append(MutationBerserkRage.new())
	not_possessed.append(MutationChaosGift.new())
	not_possessed.append(MutationCowardice.new())
	not_possessed.append(MutationDisarm.new())
	not_possessed.append(MutationEatLight.new())
	not_possessed.append(MutationFlatulence.new())
	not_possessed.append(MutationHallucinate.new())
	not_possessed.append(MutationHealthToPower.new())
	not_possessed.append(MutationHorns.new())
	not_possessed.append(MutationInvuln.new())
	not_possessed.append(MutationLoseMutations.new())
	not_possessed.append(MutationNausea.new())
	not_possessed.append(MutationPolyWound.new())
	not_possessed.append(MutationPowerToHealth.new())
	not_possessed.append(MutationProduceChaos.new())
	not_possessed.append(MutationProducePower.new())
	not_possessed.append(MutationRandomTeleport.new())
	not_possessed.append(MutationRealityDrift.new())
	not_possessed.append(MutationSpeedFlux.new())
	not_possessed.append(MutationTail.new())
	not_possessed.append(MutationTentacles.new())
	not_possessed.append(MutationTrunk.new())
	not_possessed.append(MutationWarning.new())
	not_possessed.append(MutationWasting.new())
	not_possessed.append(MutationWeirdMind.new())
	not_possessed.append(MutationWraith.new())

	# Initialise all the mutations
	for mutation: Mutation in not_possessed:
		mutation.initialise()


func has_mutations() -> bool:
	return (possessed.size() > 0)


func activatable_mutations(player: Player) -> Array[Mutation]:
	var list: Array[Mutation] = []
	for mutation: Mutation in possessed:
		if mutation.activation_summary(player.level) != "":
			list.append(mutation)
	return list


func gain_mutation() -> void:
	if not_possessed.size() == 0:
		return
	Profile.instance.message_print("You change...")
	var total: int = 0
	for mutation: Mutation in not_possessed:
		total += mutation.frequency
	var roll: int = Rng.roll_die(total)
	for i: int in not_possessed.size():
		roll -= not_possessed[i].frequency
		if roll > 0:
			continue
		var mutation: Mutation = not_possessed[i]
		not_possessed.remove_at(i)
		if possessed.size() > 0 and mutation.group != MutationGroup.NONE:
			var j: int = 0
			while j < possessed.size():
				if possessed[j].group == mutation.group:
					var other: Mutation = possessed[j]
					possessed.remove_at(j)
					other.on_lose(self)
					Profile.instance.message_print(other.lose_message)
					not_possessed.append(other)
				else:
					j += 1
		possessed.append(mutation)
		mutation.on_gain(self)
		Profile.instance.message_print(mutation.gain_message)
		SaveGame.instance.notice_update_redraw()
		return
	Profile.instance.message_print("Oops! Fell out of mutation list!")


func get_mutation_list() -> Array[String]:
	var list: Array[String] = []
	for mutation: Mutation in possessed:
		list.append(mutation.have_message)
	return list


func lose_all_mutations() -> void:
	if possessed.size() == 0:
		return
	Profile.instance.message_print("You change...")
	while possessed.size() > 0:
		var mutation: Mutation = possessed[0]
		possessed.remove_at(0)
		mutation.on_lose(self)
		not_possessed.append(mutation)
		Profile.instance.message_print(mutation.lose_message)
	SaveGame.instance.notice_update_redraw()


func lose_mutation() -> void:
	if possessed.size() == 0:
		return
	Profile.instance.message_print("You change...")
	var total: int = 0
	for mutation: Mutation in possessed:
		total += mutation.frequency
	var roll: int = Rng.roll_die(total)
	for i: int in possessed.size():
		roll -= possessed[i].frequency
		if roll > 0:
			continue
		var mutation: Mutation = possessed[i]
		possessed.remove_at(i)
		mutation.on_lose(self)
		not_possessed.append(mutation)
		Profile.instance.message_print(mutation.lose_message)
		SaveGame.instance.notice_update_redraw()
		return
	Profile.instance.message_print("Oops! Fell out of mutation list!")


func on_process_world(save_game: SaveGame, player: Player, level: DungeonLevel) -> void:
	for mutation: Mutation in possessed:
		mutation.on_process_world(save_game, player, level)
