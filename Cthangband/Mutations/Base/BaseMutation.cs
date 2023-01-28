// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using System;

namespace Cthangband.Mutations.Base
{
    [Serializable]
    internal abstract class BaseMutation : IMutation
    {
        public string AttackDescription { get; set; }
        public int DamageDiceNumber { get; set; }
        public int DamageDiceSize { get; set; }
        public int EquivalentWeaponWeight { get; set; }
        public int Frequency { get; set; }
        public string GainMessage { get; set; }
        public string GainMessage1 { get; set; }
        public MutationGroup Group { get; set; }
        public string HaveMessage { get; set; }
        public string LoseMessage { get; set; }
        public MutationAttackType MutationAttackType { get; set; }

        public virtual void Activate(SaveGame saveGame, Player player, Level level)
        {
        }

        public virtual string ActivationSummary(int lvl)
        {
            return string.Empty;
        }

        public abstract void Initialise();

        public virtual void OnGain(Genome genome)
        {
        }

        public virtual void OnLose(Genome genome)
        {
        }

        public virtual void OnProcessWorld(SaveGame saveGame, Player player, Level level)
        {
        }
    }
}