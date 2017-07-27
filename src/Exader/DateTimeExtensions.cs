using System;
using System.Diagnostics;
using System.Globalization;
using JetBrains.Annotations;

namespace Exader
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class DateTimeExtensions
    {
        public static DateTime Add(this DateTime date, DatePart part, int value, IFormatProvider provider = null)
        {
            switch (part)
            {
                case DatePart.Day:
                    return date.AddDays(value);

                case DatePart.WeekOfMonth:
                    {
                        if (provider == null) provider = CultureInfo.InvariantCulture;

                        int delta = 0;
                        if (value < 0)
                        {
                            int index = -1 * value;
                            DateTime current = date;
                            do
                            {
                                int startWeek = current.GetWeekOfMonth(provider);
                                if (1 < startWeek)
                                {
                                    for (int week = startWeek - 1; 0 < week; week--)
                                    {
                                        delta += GetDaysInWeek(current.Year, current.Month, week, provider);
                                        if (0 == --index) break;
                                    }
                                }

                                current = current.AddMonths(-1);
                                int weeksInMonth = GetWeeksInMonth(current.Year, current.Month, provider);
                                while (0 <= (index - weeksInMonth))
                                {
                                    delta += DateTime.DaysInMonth(current.Year, current.Month);
                                    weeksInMonth = GetWeeksInMonth(current.Year, current.Month, provider);
                                    current = current.AddMonths(-1);
                                    index -= weeksInMonth;
                                }
                            }
                            while (0 < index);

                            delta = -1 * delta;
                        }
                        else
                        {
                            int startWeek = date.GetWeekOfMonth(provider);
                            DateTime current = date;
                            do
                            {
                                int weeksInMonth = GetWeeksInMonth(current.Year, current.Month, provider);
                                for (int week = startWeek; week <= weeksInMonth; week++)
                                {
                                    delta += GetDaysInWeek(current.Year, current.Month, week, provider);
                                    if (0 == --value) break;
                                }

                                if (0 == value) break;

                                startWeek = 1;
                                current = current.AddMonths(1);
                            }
                            while (true);
                        }

                        return date.AddDays(delta);
                    }

                case DatePart.WeekOfYear:
                    {
                        if (provider == null) provider = CultureInfo.InvariantCulture;

                        int days;
                        if (value < 0)
                        {
                            int weekOfYear = date.GetWeekOfYear(provider);
                            days = 1 == weekOfYear ? GetDaysInWeek(date.Year - 1, 53, provider) : 7;
                        }
                        else
                        {
                            days = GetDaysInWeek(date.Year, date.GetWeekOfYear(provider), provider);
                        }

                        if (7 == days) return date.AddDays(7 * value);

                        return value < 0
                            ? date.AddDays(7 * (value + 1) - days)
                            : date.AddDays(7 * (value - 1) + days);
                    }

                case DatePart.Month:
                    Debug.Assert(1 == date.Day);
                    return date.AddMonths(value);

                case DatePart.Quarter:
                    int quarterMonth = (int)(3 * Math.Floor(date.Month / 3d)) + 1;
                    return new DateTime(date.Year, quarterMonth, 1).AddMonths(3 * value);

                case DatePart.Year:
                    return date.AddYears(value);

                case DatePart.Decade:
                    var decadeYear = (int)(10 * Math.Floor(date.Year / 10d));
                    decadeYear += 10 * value;
                    return new DateTime(decadeYear, 1, 1);

                case DatePart.Century:
                    var centuryYear = (int)(100 * Math.Floor(date.Year / 100d));
                    centuryYear += 100 * value;
                    return new DateTime(centuryYear, 1, 1);

                case DatePart.Millennium:
                    var millenniumYear = (int)(1000 * Math.Floor(date.Year / 1000d));
                    millenniumYear += 1000 * value;
                    return new DateTime(millenniumYear, 1, 1);

                default:
                    return date;
            }
        }

        public static DateTime Add(this DateTime date, TimePart part, int value)
        {
            switch (part)
            {
                case TimePart.Millisecond:
                    return date.AddMilliseconds(value);

                case TimePart.Second:
                    return date.AddSeconds(value);

                case TimePart.Minute:
                    return date.AddMinutes(value);

                case TimePart.Hour:
                    return date.AddHours(value);

                case TimePart.Day:
                    return date.AddDays(value);

                case TimePart.Week:
                    return date.AddDays(7 * value);

                case TimePart.Decade:
                    return date.AddDays(10 * value);

                case TimePart.Fortnight:
                    return date.AddDays(14 * value);

                default:
                    return date;
            }
        }

        public static int GetDaysInFirstWeek(int year, int month, IFormatProvider provider = null)
        {
            if (null == provider) provider = CultureInfo.InvariantCulture;
            DateTimeFormatInfo format = DateTimeFormatInfo.GetInstance(provider);
            int days = new DateTime(year, month, 1).DayOfWeek - format.FirstDayOfWeek;
            return 0 <= days ? (7 - days) : -1 * days;
        }

        public static int GetDaysInLastWeek(int year, int month, IFormatProvider provider = null)
        {
            if (null == provider) provider = CultureInfo.InvariantCulture;
            DateTimeFormatInfo format = DateTimeFormatInfo.GetInstance(provider);
            int days = new DateTime(year, month, DateTime.DaysInMonth(year, month)).DayOfWeek - format.FirstDayOfWeek;
            return 0 <= days ? (days + 1) : 7 - (days + 1);
        }

        public static int GetDaysInWeek(int year, int weekOfYear, IFormatProvider provider = null)
        {
            if (1 < weekOfYear && weekOfYear < 53) return 7;
            if (null == provider) provider = CultureInfo.InvariantCulture;
            return 1 == weekOfYear
                ? GetDaysInFirstWeek(year, 1, provider)
                : GetDaysInLastWeek(year, 12, provider);
        }

        public static int GetDaysInWeek(int year, int month, int weekOfMonth, IFormatProvider provider = null)
        {
            if (1 < weekOfMonth && (weekOfMonth < 4 || weekOfMonth < GetWeeksInMonth(year, month, provider))) return 7;
            if (null == provider) provider = CultureInfo.InvariantCulture;
            return 1 == weekOfMonth
                ? GetDaysInFirstWeek(year, month, provider)
                : GetDaysInLastWeek(year, month, provider);
        }

        public static int GetFirstDayOfLastWeekOfMonth(int year, int month, IFormatProvider provider)
        {
            int days = GetDaysInFirstWeek(year, month, provider);
            int rem = DateTime.DaysInMonth(year, month) - days;
            return (7 * ((rem - 1) / 7)) + days + 1;
        }

        public static int GetQuarter(this DateTime date)
        {
            return (date.Month + 2) / 3;
        }

        public static DateTime GetQuarterEnd(this DateTime date)
        {
            int month = 3 * date.GetQuarter();
            return new DateTime(date.Year, month, DateTime.DaysInMonth(date.Year, month));
        }

        public static DateTime GetQuarterStart(this DateTime date)
        {
            return new DateTime(date.Year, (3 * date.GetQuarter()) - 2, 1);
        }

        public static int GetWeekOfMonth(this DateTime date, IFormatProvider provider = null)
        {
            if (1 == date.Day) return 1;

            if (null == provider) provider = CultureInfo.InvariantCulture;

            int daysInFirstWeek = GetDaysInFirstWeek(date.Year, date.Month, provider);
            if (date.Day <= daysInFirstWeek) return 1;

            var num = (int)Math.Ceiling((date.Day - daysInFirstWeek) / 7d);
            int firstDayOfLastWeek = GetFirstDayOfLastWeekOfMonth(date.Year, date.Month, provider);
            if (date.Day <= firstDayOfLastWeek) num++;

            return num;
        }

        public static int GetWeekOfYear(this DateTime date, IFormatProvider provider = null)
        {
            DateTimeFormatInfo format = DateTimeFormatInfo.GetInstance(provider ?? CultureInfo.InvariantCulture);
            return format.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, format.FirstDayOfWeek);
        }

        public static int GetWeeksInMonth(int year, int month, IFormatProvider provider = null)
        {

            int days = GetDaysInFirstWeek(year, month, provider ?? CultureInfo.InvariantCulture);
            int rem = DateTime.DaysInMonth(year, month) - days;
            return (int)Math.Ceiling(rem / 7d) + 1;
        }

        public static bool IsFirstWeekOfMonth(this DateTime date, IFormatProvider provider = null)
        {
            if (7 <= date.Day) return false;
            if (null == provider) provider = CultureInfo.InvariantCulture;
            return date.Day <= GetDaysInFirstWeek(date.Year, date.Month, provider);
        }

        public static bool IsLastWeekOfMonth(this DateTime date, IFormatProvider provider = null)
        {
            if (date.Day <= 21) return false;
            if (null == provider) provider = CultureInfo.InvariantCulture;
            return GetFirstDayOfLastWeekOfMonth(date.Year, date.Month, provider) <= date.Day;
        }
    }
}