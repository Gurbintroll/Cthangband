// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using System;

namespace Cthangband.Mutations.RandomMutations
{
    [Serializable]
    internal class MutationHorns : Mutation
    {
        public override void Initialise()
        {
            Frequency = 2;
            GainMessage = "Horns pop forth into your forehead!";
            HaveMessage = "You have horns (dam. 2d6).";
            LoseMessage = "Your horns vanish from your forehead!";
            DamageDiceSize = 2;
            DamageDiceNumber = 6;
            EquivalentWeaponWeight = 15;
            AttackDescription = "horns";
        }

        public override void OnGain(Genome genome)
        {
            genome.NaturalAttacks.Add(this);
        }

        public override void OnLose(Genome genome)
        {
            genome.NaturalAttacks.Remove(this);
        }
    }
}