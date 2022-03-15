// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using System;

namespace Cthangband.Mutations.RandomMutations
{
    [Serializable]
    internal class MutationEatLight : Mutation
    {
        public override void Initialise()
        {
            Frequency = 1;
            GainMessage = "You feel a strange kinship with Nyogtha.";
            HaveMessage = "You sometimes feed off of the light around you.";
            LoseMessage = "You feel the world's a brighter place.";
        }

        public override void OnProcessWorld(SaveGame saveGame, Player player, Level level)
        {
            if (Program.Rng.DieRoll(3000) != 1)
            {
                return;
            }
            Profile.Instance.MsgPrint("A shadow passes over you.");
            Profile.Instance.MsgPrint(null);
            if (level.Grid[player.MapY][player.MapX].TileFlags.IsSet(GridTile.SelfLit))
            {
                player.RestoreHealth(10);
            }
            Item oPtr = player.Inventory[InventorySlot.Lightsource];
            if (oPtr.Category == ItemCategory.Light)
            {
                if ((oPtr.ItemSubCategory == LightType.Torch || oPtr.ItemSubCategory == LightType.Lantern) &&
                    oPtr.TypeSpecificValue > 0)
                {
                    player.RestoreHealth(oPtr.TypeSpecificValue / 20);
                    oPtr.TypeSpecificValue /= 2;
                    Profile.Instance.MsgPrint("You absorb energy from your light!");
                    if (player.TimedBlindness != 0)
                    {
                        if (oPtr.TypeSpecificValue == 0)
                        {
                            oPtr.TypeSpecificValue++;
                        }
                    }
                    else if (oPtr.TypeSpecificValue == 0)
                    {
                        saveGame.Disturb(false);
                        Profile.Instance.MsgPrint("Your light has gone out!");
                    }
                    else if (oPtr.TypeSpecificValue < 100 && oPtr.TypeSpecificValue % 10 == 0)
                    {
                        Profile.Instance.MsgPrint("Your light is growing faint.");
                    }
                }
            }
            saveGame.SpellEffects.UnlightArea(50, 10);
        }
    }
}