// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.PlayerClass.Base;
using Cthangband.Talents;
using Cthangband.Talents.Base;
using System;
using System.Collections.Generic;

namespace Cthangband.Spells
{
    [Serializable]
    internal class TalentList : List<ITalent>
    {
        public TalentList(IPlayerClass playerClass)
        {
            Add(new TalentPrecognition());
            Add(new TalentNeuralBlast());
            Add(new TalentMinorDisplacement());
            Add(new TalentMajorDisplacement());
            Add(new TalentDomination());
            Add(new TalentPulverise());
            Add(new TalentCharacterArmour());
            Add(new TalentPsychometry());
            Add(new TalentMindWave());
            Add(new TalentAdrenalineChannelling());
            Add(new TalentPsychicDrain());
            Add(new TalentTelekineticWave());
            foreach (var talent in this)
            {
                talent.Initialise(playerClass);
            }
        }
    }
}