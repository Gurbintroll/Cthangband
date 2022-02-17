using Cthangband.Enumerations;
using System;

namespace Cthangband
{
    [Serializable]
    internal class ExPlayer
    {
        public readonly int Generation;
        public readonly int Lev;
        public readonly string Name;
        public readonly int PBirthRace;
        public readonly int Pclass;
        public readonly int Prace;
        public readonly int Psex;
        public readonly Realm Realm1;
        public readonly Realm Realm2;

        public ExPlayer(Player player)
        {
            Psex = player.GenderIndex;
            Prace = player.RaceIndex;
            PBirthRace = player.RaceIndexAtBirth;
            Pclass = player.ProfessionIndex;
            Realm1 = player.Realm1;
            Realm2 = player.Realm2;
            Name = player.Name;
            Lev = player.Level;
            Generation = player.Generation;
        }
    }
}