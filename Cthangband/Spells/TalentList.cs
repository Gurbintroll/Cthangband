// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
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