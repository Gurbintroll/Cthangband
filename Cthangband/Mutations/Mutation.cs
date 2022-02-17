using Cthangband.Enumerations;
using System;

namespace Cthangband.Mutations
{
    [Serializable]
    internal abstract class Mutation
    {
        public string AtkDesc;
        public int Ddd;
        public int Dss;
        public int Frequency;
        public string GainMessage;
        public MutationGroup Group = MutationGroup.None;
        public string HaveMessage;
        public string LoseMessage;
        public MutationAttackType MutationAttackType = MutationAttackType.Physical;
        public int NWeight;

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