using System;
using System.Globalization;

namespace Kogler.Framework
{
    public interface IDateSpan
    {
        DateTime Start { get; }
        DateTime End { get; }
        TimeSpan Duration { get; }
    }

    public struct DateSpan : IDateSpan, IComparable, IComparable<IDateSpan>, IEquatable<IDateSpan>, IFormattable
    {
        #region << Construstors >>

        private readonly Calendar Calendar;

        public DateSpan(DateTime start, DateTime end, bool ignoreLastDay = false, Calendar calendar = null)
        {
            IgnoreLastDay = ignoreLastDay;
            Calendar = calendar ?? CultureInfo.CurrentCulture.Calendar;
            Start = start;
            End = end;
        }

        public DateSpan(IDateSpan other, bool ignoreLastDay = false, Calendar calendar = null) : this(other.Start, other.End, ignoreLastDay, calendar) { }

        public DateSpan(DateTime start, TimeSpan duration, bool ignoreLastDay = false, Calendar calendar = null) : this(start, start.Add(duration), ignoreLastDay, calendar) { }

        /// <summary>
        /// Add days to DateTime
        /// </summary>
        /// <param name="start">start date</param>
        /// <param name="days">days to add</param>
        /// <param name="ignoreLastDay"></param>
        /// <param name="calendar"></param>
        public DateSpan(DateTime start, int days, bool ignoreLastDay = false, Calendar calendar = null) : this(start, start.AddDays(days), ignoreLastDay, calendar) { }

        #endregion

        #region << Properties >>

        public TimeSpan Duration => End - Start;
        public int Days => Duration.Days;
        public bool IgnoreLastDay { get; }

        public DateTime EndIgnored
        {
            get
            {
                if (!IgnoreLastDay) return End;
                var endIgnored = End.AddDays(-1);
                return endIgnored < Start ? Start : endIgnored;
            }
        }

        public bool IsSameYear => Start.Year.Equals(EndIgnored.Year);
        public bool IsSameMonth => IsSameYear && Start.Month.Equals(EndIgnored.Month);

        public bool IsSameWeek =>
            Calendar.GetWeekOfYear(Start, CalendarWeekRule.FirstDay, DayOfWeek.Monday)
                .Equals(
                    Calendar.GetWeekOfYear(EndIgnored, CalendarWeekRule.FirstDay, DayOfWeek.Monday));

        public bool IsSameDay => Start.Equals(EndIgnored);

        #region IDateSpan Members
        
        public DateTime Start { get; }
        public DateTime End { get; }

        #endregion

        #endregion

        #region << Overrides of Object Type >>>

        public override bool Equals(object obj)
        {
            var other = obj as IDateSpan;
            return (other != null) && (Start == other.Start) && (End == other.End);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = 0;
                result = (result * 397) ^ Start.GetHashCode();
                result = (result * 397) ^ End.GetHashCode();
                return result;
            }
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(IDateSpan other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IDateSpan other)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return String.Format(CultureInfo.CurrentCulture, "Start: {0}, End: {1}", new object[] { Start, End });
        }

        #endregion

        #region << Operators >>

        public static DateSpan operator +(DateSpan date, TimeSpan time)
        {
            date.End.Add(time);
            return date;
        }

        public static DateSpan operator -(DateSpan date, TimeSpan time)
        {
            date.End.Add(-time);
            return date;
        }

        public static DateSpan operator +(DateSpan date, DateSpan date1)
        {
            date.End.Add(date1.Duration);
            return date;
        }

        public static DateSpan operator -(DateSpan date, DateSpan date1)
        {
            date.End.Add(-date1.Duration);
            return date;
        }

        public static bool operator ==(DateSpan d1, DateSpan d2)
        {
            return d1.Start == d2.Start &&
                   d1.End == d2.End &&
                   d1.IgnoreLastDay == d2.IgnoreLastDay;
        }

        public static bool operator !=(DateSpan d1, DateSpan d2)
        {
            return !(d1 == d2);
        }

        public static bool operator <(DateSpan d1, DateSpan d2)
        {
            return d1.Duration < d2.Duration;
        }

        public static bool operator <=(DateSpan d1, DateSpan d2)
        {
            return d1.Duration <= d2.Duration;
        }

        public static bool operator >(DateSpan d1, DateSpan d2)
        {
            return d1.Duration > d2.Duration;
        }

        public static bool operator >=(DateSpan d1, DateSpan d2)
        {
            return d1.Duration >= d2.Duration;
        }

        #endregion

        public static DateSpan ThisWeek => new DateSpan(DateTime.Today.BeginOfWeek(), 7);

        public static DateSpan NextWeek => ThisWeek.ShiftDays(7);

        public static DateSpan PreviousWeek => ThisWeek.ShiftDays(-7);

        public static DateSpan ThisMonth => DateTime.Today.FullMonths();

        public static DateSpan NextMonth => DateTime.Today.EndOfMonth().AddDays(1).FullMonths();
    }
}
