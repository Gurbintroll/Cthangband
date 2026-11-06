# Copyright © 1997-2026 Gurbintroll Games
#
# This program is free software: you can redistribute it and/or modify it under the terms of the
# GNU General Public License as published by the Free Software Foundation, either version 3 of the
# License, or (at your option) any later version.
# This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
# without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
# See the GNU General Public License for more details.
class_name TalentList
extends Resource

@export var talents: Array[Talent]


func init_talent_list(profession: Profession) -> void:
	talents.append(TalentPrecognition.new()) # Precognition
	talents.append(TalentNeuralBlast.new()) # Neural Blast
	talents.append(TalentMinorDisplacement.new()) # Minor Displacement
	talents.append(TalentMajorDisplacement.new()) # Major Displacement
	talents.append(TalentDomination.new()) # Domination
	talents.append(TalentPulverise.new()) # Pulverise
	talents.append(TalentCharacterArmour.new()) # Character Armour
	talents.append(TalentPsychometry.new()) # Psychometry
	talents.append(TalentMindWave.new()) # Mind Wave
	talents.append(TalentAdrenalineChannelling.new()) # Adrenaline Channelling
	talents.append(TalentPsychicDrain.new()) # Psychic Drain
	talents.append(TalentTelekineticWave.new()) # Telekinetic Wave
	for talent: Talent in talents:
		talent.initialise(profession)
