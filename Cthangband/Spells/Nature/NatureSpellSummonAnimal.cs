// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using Cthangband.Enumerations;
using Cthangband.Spells.Base;
using Cthangband.StaticData;
using System;

namespace Cthangband.Spells.Nature
{
    [Serializable]
    internal class NatureSpellSummonAnimal : BaseSpell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
            if (!level.Monsters.SummonSpecificFriendly(player.MapY, player.MapX, player.Level, Constants.SummonAnimalRanger,
                true))
            {
                Profile.Instance.MsgPrint("No-one ever turns up.");
            }
        }

        public override void Initialise(int characterClass)
        {
            Name = "Summon Animal";
            switch (characterClass)
            {
                case CharacterClass.Mage:
                    Level = 25;
                    VisCost = 25;
                    BaseFailure = 90;
                    FirstCastExperience = 50;
                    break;

                case CharacterClass.Priest:
                    Level = 30;
                    VisCost = 30;
                    BaseFailure = 55;
                    FirstCastExperience = 8;
                    break;

                case CharacterClass.Ranger:
                    Level = 23;
                    VisCost = 23;
                    BaseFailure = 65;
                    FirstCastExperience = 10;
                    break;

                case CharacterClass.WarriorMage:
                case CharacterClass.Cultist:
                    Level = 31;
                    VisCost = 31;
                    BaseFailure = 65;
                    FirstCastExperience = 10;
                    break;

                case CharacterClass.HighMage:
                case CharacterClass.Druid:
                    Level = 20;
                    VisCost = 20;
                    BaseFailure = 80;
                    FirstCastExperience = 50;
                    break;

                default:
                    Level = 99;
                    VisCost = 0;
                    BaseFailure = 0;
                    FirstCastExperience = 0;
                    break;
            }
        }

        protected override string Comment(Player player)
        {
            return "control 100%";
        }
    }
}