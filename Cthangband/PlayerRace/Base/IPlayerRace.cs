// Cthangband: © 1997 - 2023 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using System.Collections.Generic;

namespace Cthangband.PlayerRace.Base
{
    internal interface IPlayerRace
    {
        int[] AbilityBonus { get; }
        int AgeRange { get; }

        string Article { get; }
        int BaseAge { get; }
        int BaseDeviceBonus { get; }
        int BaseDisarmBonus { get; }
        int BaseMeleeAttackBonus { get; }
        int BaseRangedAttackBonus { get; }
        int BaseSaveBonus { get; }
        int BaseSearchBonus { get; }
        int BaseSearchFrequency { get; }
        int BaseStealthBonus { get; }
        string Description1 { get; }
        string Description2 { get; }
        string Description3 { get; }
        string Description4 { get; }
        string Description5 { get; }
        string Description6 { get; }
        bool DoesntEat { get; }
        int ExperienceFactor { get; }
        bool FeedsOnNether { get; }
        int FemaleBaseHeight { get; }
        int FemaleBaseWeight { get; }
        int FemaleHeightRange { get; }
        int FemaleWeightRange { get; }
        bool Glows { get; }
        bool HasSpeedBonus { get; }
        int HitDieBonus { get; }
        int Infravision { get; }
        bool IsIncorporeal { get; }
        bool IsNocturnal { get; }
        bool IsSunlightSensitive { get; }
        int MaleBaseHeight { get; }
        int MaleBaseWeight { get; }
        int MaleHeightRange { get; }
        int MaleWeightRange { get; }
        bool Mutates { get; }
        bool ResistsFear { get; }
        bool SanityImmune { get; }

        bool SanityResistant { get; }
        bool SpillsPotions { get; }
        string Title { get; }

        void ApplyArmourBonus(Player player);

        void ApplyRacialStatus(Player player);

        void ConsumeFood(Player player, Item item);

        string CreateRandomName();

        bool DoesntBleed(Player player);

        bool DoesntStun(Player player);

        void GetAbilitiesAsItemFlags(Player player, FlagSet f1, FlagSet f2, FlagSet f3);

        void GetHistory(Player player);

        string GetRacialPowerText(Player player);

        List<string> SelfKnowledge(Player player);

        void UseRacialPower(SaveGame saveGame, Player player, Level level);
    }
}