using Cthangband.Enumerations;
using System;

namespace Cthangband.Mutations.RandomMutations
{
    [Serializable]
    internal class MutationWasting : Mutation
    {
        public override void Initialise()
        {
            Frequency = 1;
            GainMessage = "You suddenly contract a horrible wasting disease.";
            HaveMessage = "You have a horrible wasting disease.";
            LoseMessage = "You are cured of the horrible wasting disease!";
        }

        public override void OnProcessWorld(SaveGame saveGame, Player player, Level level)
        {
            if (Program.Rng.DieRoll(3000) != 13)
            {
                return;
            }
            int whichStat = Program.Rng.RandomLessThan(6);
            bool sustained = false;
            switch (whichStat)
            {
                case Ability.Strength:
                    if (player.HasSustainStrength)
                    {
                        sustained = true;
                    }
                    break;

                case Ability.Intelligence:
                    if (player.HasSustainIntelligence)
                    {
                        sustained = true;
                    }
                    break;

                case Ability.Wisdom:
                    if (player.HasSustainWisdom)
                    {
                        sustained = true;
                    }
                    break;

                case Ability.Dexterity:
                    if (player.HasSustainDexterity)
                    {
                        sustained = true;
                    }
                    break;

                case Ability.Constitution:
                    if (player.HasSustainConstitution)
                    {
                        sustained = true;
                    }
                    break;

                case Ability.Charisma:
                    if (player.HasSustainCharisma)
                    {
                        sustained = true;
                    }
                    break;

                default:
                    Profile.Instance.MsgPrint("Invalid stat chosen!");
                    sustained = true;
                    break;
            }
            if (sustained)
            {
                return;
            }
            saveGame.Disturb(false);
            if (Program.Rng.DieRoll(10) <= player.Religion.GetNamedDeity(Pantheon.GodName.Lobon).AdjustedFavour)
            {
                Profile.Instance.MsgPrint("Lobon's favour protects you from wasting away!");
                Profile.Instance.MsgPrint(null);
                return;
            }
            Profile.Instance.MsgPrint("You can feel yourself wasting away!");
            Profile.Instance.MsgPrint(null);
            player.DecreaseAbilityScore(whichStat, Program.Rng.DieRoll(6) + 6, Program.Rng.DieRoll(3) == 1);
        }
    }
}