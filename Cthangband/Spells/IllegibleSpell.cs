using System;

namespace Cthangband.Spells
{
    [Serializable]
    internal class IllegibleSpell : Spell
    {
        public override void Cast(SaveGame saveGame, Player player, Level level)
        {
        }

        public override void Initialise(int characterClass)
        {
            Name = "(illegible)";
            Level = 99;
            ManaCost = 0;
            BaseFailure = 0;
            FirstCastExperience = 0;
        }

        protected override string Comment(Player player)
        {
            return string.Empty;
        }
    }
}