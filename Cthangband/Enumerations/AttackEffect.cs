// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
namespace Cthangband.Enumerations
{
    /// <summary>
    /// The effect that a monster attack has when it hits
    /// </summary>
    internal enum AttackEffect
    {
        Nothing,
        Hurt,
        Poison,
        UnBonus,
        UnPower,
        EatGold,
        EatItem,
        EatFood,
        EatLight,
        Acid,
        Electricity,
        Fire,
        Cold,
        Blind,
        Confuse,
        Terrify,
        Paralyze,
        LoseStr,
        LoseInt,
        LoseWis,
        LoseDex,
        LoseCon,
        LoseCha,
        LoseAll,
        Exp10,
        Exp20,
        Exp40,
        Exp80,
        Shatter,
    }
}