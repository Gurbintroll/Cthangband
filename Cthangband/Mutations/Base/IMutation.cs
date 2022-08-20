// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;

namespace Cthangband.Mutations.Base
{
    internal interface IMutation
    {
        string AttackDescription { get; set; }
        int DamageDiceNumber { get; set; }
        int DamageDiceSize { get; set; }
        int EquivalentWeaponWeight { get; set; }
        int Frequency { get; set; }
        string GainMessage { get; set; }
        MutationGroup Group { get; set; }
        string HaveMessage { get; set; }
        string LoseMessage { get; set; }
        MutationAttackType MutationAttackType { get; set; }

        void Activate(SaveGame saveGame, Player player, Level level);

        string ActivationSummary(int lvl);

        void Initialise();

        void OnGain(Genome genome);

        void OnLose(Genome genome);

        void OnProcessWorld(SaveGame saveGame, Player player, Level level);
    }
}