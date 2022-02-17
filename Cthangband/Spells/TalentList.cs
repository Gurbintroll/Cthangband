using Cthangband.Talents;
using System;
using System.Collections.Generic;

namespace Cthangband.Spells
{
    [Serializable]
    internal class TalentList : List<Talent>
    {
        public TalentList(int characterClass)
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
            Add(new TalentAdrenalineChanneling());
            Add(new TalentPsychicDrain());
            Add(new TalentTelekineticWave());
            foreach (Talent talent in this)
            {
                talent.Initialise(characterClass);
            }
        }
    }
}