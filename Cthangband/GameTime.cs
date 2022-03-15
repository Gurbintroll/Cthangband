// Cthangband: © 1997 - 2022 Dean Anderson; Based on Angband: © 1997 Ben Harrison, James E. Wilson,
// Robert A. Koeneke; Based on Moria: © 1985 Robert Alan Koeneke and Umoria: © 1989 James E.Wilson
//
// This game is released under the “Angband License”, defined as: “© 1997 Ben Harrison, James E.
// Wilson, Robert A. Koeneke This software may be copied and distributed for educational, research,
// and not for profit purposes provided that this copyright and statement are included in all such
// copies. Other copyrights may also apply.”
using System;

namespace Cthangband
{
    [Serializable]
    internal class GameTime
    {
        public bool IsBirthday;
        public bool IsDawn;
        public bool IsDusk;
        public bool IsFeelTime;
        public bool IsHalloween;
        public bool IsMidnight;
        public bool IsNewYear;

        private const int LevelFeelDelay = 2500;
        private const int MillisecondsPerTurn = 800;
        private int _birthday;
        private DateTime _currentGameDateTime;
        private int _currentTurn;
        private DateTime _dawn;
        private DateTime _dusk;
        private DateTime _gameStartDateTime;
        private int _levelEntryTurn;
        private TimeSpan _tick = new TimeSpan(0, 0, 0, 0, MillisecondsPerTurn);

        public GameTime(int startDate, bool startAtDusk)
        {
            _currentGameDateTime = new DateTime(1297, 1, 1, 0, 0, 0, 0);
            _currentGameDateTime = _currentGameDateTime.AddDays(startDate - 1);
            _birthday = startDate;
            RecalculateDawnAndDusk();
            if (startAtDusk)
            {
                _gameStartDateTime = _dusk;
            }
            else
            {
                _gameStartDateTime = _dawn;
            }
            _currentGameDateTime = _gameStartDateTime;
            Tick();
        }

        public string BirthdayText
        {
            get
            {
                return _gameStartDateTime.ToString("MMM d");
            }
        }

        public string DateText
        {
            get
            {
                return _currentGameDateTime.ToString("MMM d");
            }
        }

        public bool IsLight
        {
            get
            {
                return (_currentGameDateTime >= _dawn) && (_currentGameDateTime <= _dusk);
            }
        }

        public bool IsTurnHundred
        {
            get
            {
                return _currentTurn % 100 == 0;
            }
        }

        public bool IsTurnTen
        {
            get
            {
                return _currentTurn % 10 == 0;
            }
        }

        public bool LevelFeel
        {
            get
            {
                return (_currentTurn - _levelEntryTurn) >= LevelFeelDelay;
            }
        }

        public string TimeText
        {
            get
            {
                return _currentGameDateTime.ToString("h:mmtt");
            }
        }

        public int Turn
        {
            get
            {
                return _currentTurn;
            }
        }

        public void MarkLevelEntry()
        {
            _levelEntryTurn = _currentTurn;
        }

        public void Tick()
        {
            var oldDay = _currentGameDateTime.DayOfYear;
            _currentTurn++;
            var oldDateTime = _currentGameDateTime;
            _currentGameDateTime += _tick;
            var newDay = _currentGameDateTime.DayOfYear;
            IsBirthday = false;
            IsDawn = false;
            IsDusk = false;
            IsFeelTime = false;
            IsHalloween = false;
            IsMidnight = false;
            IsNewYear = false;
            if (_currentTurn - _levelEntryTurn == LevelFeelDelay)
            {
                IsFeelTime = true;
            }
            if (oldDay != newDay)
            {
                IsMidnight = true;
                RecalculateDawnAndDusk();
                if (newDay == 1)
                {
                    IsNewYear = true;
                }
                if (newDay == _birthday)
                {
                    IsBirthday = true;
                }
                if (newDay == 305)
                {
                    IsHalloween = true;
                }
            }
            if (oldDateTime < _dawn && _currentGameDateTime >= _dawn)
            {
                IsDawn = true;
            }
            if (oldDateTime < _dusk && _currentGameDateTime >= _dusk)
            {
                IsDusk = true;
            }
            var year = _currentGameDateTime.Year;
            if (year > 1297)
            {
                _currentGameDateTime = _currentGameDateTime.AddYears(1297 - year);
            }
        }

        public void ToNextDawn()
        {
            var midnight = new DateTime(_currentGameDateTime.Year, _currentGameDateTime.Month, _currentGameDateTime.Day, 0, 0, 0);
            midnight += new TimeSpan(1, 0, 0, 0);
            _currentGameDateTime = midnight;
            RecalculateDawnAndDusk();
            _currentGameDateTime = _dawn;
            ReverseEngineerTurn();
            Tick();
        }

        public void ToNextDusk()
        {
            var midnight = new DateTime(_currentGameDateTime.Year, _currentGameDateTime.Month, _currentGameDateTime.Day, 0, 0, 0);
            midnight += new TimeSpan(1, 0, 0, 0);
            _currentGameDateTime = midnight;
            RecalculateDawnAndDusk();
            _currentGameDateTime = _dusk;
            ReverseEngineerTurn();
            Tick();
        }

        private void RecalculateDawnAndDusk()
        {
            var midnight = new DateTime(_currentGameDateTime.Year, _currentGameDateTime.Month, _currentGameDateTime.Day, 0, 0, 0);
            var n = midnight.DayOfYear;
            var delta = 23.45 * Math.Sin((360.0 / 365.0 * (n + 284)) * (Math.PI / 180.0)) * (Math.PI / 180.0);
            const double phi = 50.838 * (Math.PI / 180.0);
            var omega = Math.Acos(Math.Tan(delta) * -Math.Tan(phi)) * (180.0 / Math.PI);
            var sunriseHoursBeforeNoon = omega / 15.0;
            var sunrise = TimeSpan.FromHours(12.0 - sunriseHoursBeforeNoon);
            sunrise = new TimeSpan(0, 0, 0, (int)sunrise.TotalSeconds, 0);
            var sunset = TimeSpan.FromHours(12.0 + sunriseHoursBeforeNoon);
            sunset = new TimeSpan(0, 0, 0, (int)sunset.TotalSeconds, 0);
            _dawn = midnight + sunrise;
            _dusk = midnight + sunset;
        }

        private void ReverseEngineerTurn()
        {
            var totalTime = _currentGameDateTime - _gameStartDateTime;
            var milliseconds = totalTime.TotalMilliseconds;
            _currentTurn = (int)(milliseconds / MillisecondsPerTurn);
        }
    }
}