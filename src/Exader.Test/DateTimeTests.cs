using System;
using System.Diagnostics;
using System.Globalization;
using Xunit;

namespace Exader
{
    public class DateTimeTests
    {
        [Fact]
        public void AddDate()
        {
            Assert.Equal(new DateTime(3000, 1, 1), new DateTime(2000, 1, 1).Add(DatePart.Millennium, 1));
            Assert.Equal(new DateTime(5000, 1, 1), new DateTime(2000, 1, 1).Add(DatePart.Millennium, 3));
            Assert.Equal(new DateTime(7000, 1, 1), new DateTime(2000, 1, 1).Add(DatePart.Millennium, 5));

            Assert.Equal(new DateTime(6000, 1, 1), new DateTime(7000, 1, 1).Add(DatePart.Millennium, -1));
            Assert.Equal(new DateTime(4000, 1, 1), new DateTime(7000, 1, 1).Add(DatePart.Millennium, -3));
            Assert.Equal(new DateTime(2000, 1, 1), new DateTime(7000, 1, 1).Add(DatePart.Millennium, -5));

            Assert.Equal(new DateTime(2000, 1, 1), new DateTime(1900, 1, 1).Add(DatePart.Century, 1));
            Assert.Equal(new DateTime(2200, 1, 1), new DateTime(1900, 1, 1).Add(DatePart.Century, 3));
            Assert.Equal(new DateTime(2400, 1, 1), new DateTime(1900, 1, 1).Add(DatePart.Century, 5));

            Assert.Equal(new DateTime(2300, 1, 1), new DateTime(2400, 1, 1).Add(DatePart.Century, -1));
            Assert.Equal(new DateTime(2100, 1, 1), new DateTime(2400, 1, 1).Add(DatePart.Century, -3));
            Assert.Equal(new DateTime(1900, 1, 1), new DateTime(2400, 1, 1).Add(DatePart.Century, -5));

            Assert.Equal(new DateTime(2020, 1, 1), new DateTime(2010, 1, 1).Add(DatePart.Decade, 1));
            Assert.Equal(new DateTime(2040, 1, 1), new DateTime(2010, 1, 1).Add(DatePart.Decade, 3));
            Assert.Equal(new DateTime(2060, 1, 1), new DateTime(2010, 1, 1).Add(DatePart.Decade, 5));

            Assert.Equal(new DateTime(2050, 1, 1), new DateTime(2060, 1, 1).Add(DatePart.Decade, -1));
            Assert.Equal(new DateTime(2030, 1, 1), new DateTime(2060, 1, 1).Add(DatePart.Decade, -3));
            Assert.Equal(new DateTime(2010, 1, 1), new DateTime(2060, 1, 1).Add(DatePart.Decade, -5));

            Assert.Equal(new DateTime(2010, 1, 1), new DateTime(2009, 1, 1).Add(DatePart.Year, 1));
            Assert.Equal(new DateTime(2012, 1, 1), new DateTime(2009, 1, 1).Add(DatePart.Year, 3));
            Assert.Equal(new DateTime(2014, 1, 1), new DateTime(2009, 1, 1).Add(DatePart.Year, 5));

            Assert.Equal(new DateTime(2009, 1, 1), new DateTime(2010, 1, 1).Add(DatePart.Year, -1));
            Assert.Equal(new DateTime(2007, 1, 1), new DateTime(2010, 1, 1).Add(DatePart.Year, -3));
            Assert.Equal(new DateTime(2005, 1, 1), new DateTime(2010, 1, 1).Add(DatePart.Year, -5));

            Assert.Equal(new DateTime(2009, 4, 1), new DateTime(2009, 1, 1).Add(DatePart.Quarter, 1));
            Assert.Equal(new DateTime(2009, 7, 1), new DateTime(2009, 4, 1).Add(DatePart.Quarter, 1));
            Assert.Equal(new DateTime(2009, 10, 1), new DateTime(2009, 7, 1).Add(DatePart.Quarter, 1));
            Assert.Equal(new DateTime(2010, 1, 1), new DateTime(2009, 10, 1).Add(DatePart.Quarter, 1));
            Assert.Equal(new DateTime(2009, 7, 1), new DateTime(2009, 1, 1).Add(DatePart.Quarter, 2));
            Assert.Equal(new DateTime(2009, 10, 1), new DateTime(2009, 1, 1).Add(DatePart.Quarter, 3));
            Assert.Equal(new DateTime(2010, 1, 1), new DateTime(2009, 1, 1).Add(DatePart.Quarter, 4));

            Assert.Equal(new DateTime(2008, 10, 1), new DateTime(2009, 1, 1).Add(DatePart.Quarter, -1));
            Assert.Equal(new DateTime(2009, 1, 1), new DateTime(2009, 4, 1).Add(DatePart.Quarter, -1));
            Assert.Equal(new DateTime(2009, 4, 1), new DateTime(2009, 7, 1).Add(DatePart.Quarter, -1));
            Assert.Equal(new DateTime(2009, 7, 1), new DateTime(2009, 10, 1).Add(DatePart.Quarter, -1));
            Assert.Equal(new DateTime(2008, 7, 1), new DateTime(2009, 1, 1).Add(DatePart.Quarter, -2));
            Assert.Equal(new DateTime(2008, 4, 1), new DateTime(2009, 1, 1).Add(DatePart.Quarter, -3));
            Assert.Equal(new DateTime(2008, 1, 1), new DateTime(2009, 1, 1).Add(DatePart.Quarter, -4));

            Assert.Equal(new DateTime(2009, 2, 1), new DateTime(2009, 1, 1).Add(DatePart.Month, 1));
            Assert.Equal(new DateTime(2009, 6, 1), new DateTime(2009, 1, 1).Add(DatePart.Month, 5));
            Assert.Equal(new DateTime(2010, 1, 1), new DateTime(2009, 12, 1).Add(DatePart.Month, 1));
            Assert.Equal(new DateTime(2010, 1, 1), new DateTime(2009, 1, 1).Add(DatePart.Month, 12));

            Assert.Equal(new DateTime(2008, 12, 1), new DateTime(2009, 1, 1).Add(DatePart.Month, -1));
            Assert.Equal(new DateTime(2008, 8, 1), new DateTime(2009, 1, 1).Add(DatePart.Month, -5));
            Assert.Equal(new DateTime(2009, 11, 1), new DateTime(2009, 12, 1).Add(DatePart.Month, -1));
            Assert.Equal(new DateTime(2008, 1, 1), new DateTime(2009, 1, 1).Add(DatePart.Month, -12));
        }

        [Fact]
        public void AddWeekOfMonth()
        {
            Trace.WriteLine("Week of Month");
            CultureInfo russianCulture = CultureInfo.GetCultureInfo("ru-RU");

            Assert.Equal(new DateTime(2009, 1, 5), new DateTime(2009, 1, 1).Add(DatePart.WeekOfMonth, 1, russianCulture));
            Assert.Equal(new DateTime(2009, 1, 19), new DateTime(2009, 1, 1).Add(DatePart.WeekOfMonth, 3, russianCulture));
            Assert.Equal(new DateTime(2009, 2, 1), new DateTime(2009, 1, 1).Add(DatePart.WeekOfMonth, 5, russianCulture));
            Assert.Equal(new DateTime(2009, 3, 1), new DateTime(2009, 1, 1).Add(DatePart.WeekOfMonth, 10, russianCulture));

            var prevDate = new DateTime(2008, 12, 29);
            for (int value = 1; value < 100; value++)
            {
                DateTime date = new DateTime(2009, 1, 1).Add(DatePart.WeekOfMonth, value, russianCulture);

                Trace.WriteLine(value + ". " + date.ToShortDateString());

                Assert.NotEqual(prevDate, date);
                Assert.Equal(new DateTime(2009, 1, 1), date.Add(DatePart.WeekOfMonth, -value, russianCulture));
                Assert.True(russianCulture.DateTimeFormat.FirstDayOfWeek == date.DayOfWeek || 1 == date.Day);
                Assert.True(7 == (date - prevDate).Days
                    || date.IsFirstWeekOfMonth(russianCulture)
                    || date.IsLastWeekOfMonth(russianCulture)
                    || prevDate.IsFirstWeekOfMonth(russianCulture)
                    || prevDate.IsLastWeekOfMonth(russianCulture));

                prevDate = date;
            }
        }

        [Fact]
        public void AddWeekOfYear()
        {
            Trace.WriteLine("Week of Year");
            CultureInfo russianCulture = CultureInfo.GetCultureInfo("ru-RU");

            Assert.Equal(new DateTime(2009, 1, 5), new DateTime(2009, 1, 1).Add(DatePart.WeekOfYear, 1, russianCulture));
            Assert.Equal(new DateTime(2009, 1, 12), new DateTime(2009, 1, 5).Add(DatePart.WeekOfYear, 1, russianCulture));
            Assert.Equal(new DateTime(2009, 1, 19), new DateTime(2009, 1, 12).Add(DatePart.WeekOfYear, 1, russianCulture));
            Assert.Equal(new DateTime(2009, 2, 2), new DateTime(2009, 1, 26).Add(DatePart.WeekOfYear, 1, russianCulture));
            Assert.Equal(new DateTime(2009, 1, 19), new DateTime(2009, 1, 1).Add(DatePart.WeekOfYear, 3, russianCulture));
            Assert.Equal(new DateTime(2009, 2, 2), new DateTime(2009, 1, 1).Add(DatePart.WeekOfYear, 5, russianCulture));
            Assert.Equal(new DateTime(2010, 1, 4), new DateTime(2009, 1, 1).Add(DatePart.WeekOfYear, 53, russianCulture));
            Assert.Equal(new DateTime(2010, 1, 1), new DateTime(2009, 12, 28).Add(DatePart.WeekOfYear, 1, russianCulture));

            Assert.Equal(new DateTime(2008, 12, 29),
                new DateTime(2009, 1, 1)
                    .Add(DatePart.WeekOfYear, 52, russianCulture)
                    .Add(DatePart.WeekOfYear, -52, russianCulture));

            var prevDate = new DateTime(2008, 12, 29);
            for (int value = 1; value < 100; value++)
            {
                DateTime date = new DateTime(2009, 1, 1).Add(DatePart.WeekOfYear, value, russianCulture);

                Trace.WriteLine(value + ". " + date.ToShortDateString());

                Assert.NotEqual(prevDate, date);
                Assert.Equal(new DateTime(2008, 12, 29), date.Add(DatePart.WeekOfYear, -value, russianCulture));
                Assert.True(russianCulture.DateTimeFormat.FirstDayOfWeek == date.DayOfWeek || 1 == date.Day);
                Assert.True(7 == (date - prevDate).Days);

                prevDate = date;
            }
        }

        [Fact]
        public void DaysOfWeeks()
        {
            Assert.Equal(3, DateTimeExtensions.GetDaysInFirstWeek(2009, 1));
            Assert.Equal(5, DateTimeExtensions.GetDaysInLastWeek(2009, 12));

            Assert.Equal(2, DateTimeExtensions.GetDaysInFirstWeek(2010, 1));
            Assert.Equal(6, DateTimeExtensions.GetDaysInLastWeek(2010, 12));

            CultureInfo russianCulture = CultureInfo.GetCultureInfo("ru-RU");

            Assert.Equal(4, DateTimeExtensions.GetDaysInFirstWeek(2009, 1, russianCulture));
            Assert.Equal(4, DateTimeExtensions.GetDaysInLastWeek(2009, 12, russianCulture));

            Assert.Equal(3, DateTimeExtensions.GetDaysInFirstWeek(2010, 1, russianCulture));
            Assert.Equal(5, DateTimeExtensions.GetDaysInLastWeek(2010, 12, russianCulture));
        }

        [Fact]
        public void IsFirstAndLastWeek()
        {
            Assert.True(new DateTime(2009, 1, 1).IsFirstWeekOfMonth());
            Assert.False(new DateTime(2009, 1, 4).IsFirstWeekOfMonth());

            Assert.False(new DateTime(2009, 1, 24).IsLastWeekOfMonth());
            Assert.True(new DateTime(2009, 1, 25).IsLastWeekOfMonth());
            Assert.False(new DateTime(2009, 12, 26).IsLastWeekOfMonth());
            Assert.True(new DateTime(2009, 12, 27).IsLastWeekOfMonth());
            Assert.True(new DateTime(2009, 12, 28).IsLastWeekOfMonth());
            Assert.True(new DateTime(2009, 12, 29).IsLastWeekOfMonth());
            Assert.True(new DateTime(2009, 12, 31).IsLastWeekOfMonth());

            CultureInfo russianCulture = CultureInfo.GetCultureInfo("ru-RU");

            Assert.True(new DateTime(2009, 1, 1).IsFirstWeekOfMonth(russianCulture));
            Assert.True(new DateTime(2009, 1, 2).IsFirstWeekOfMonth(russianCulture));
            Assert.True(new DateTime(2009, 1, 3).IsFirstWeekOfMonth(russianCulture));
            Assert.True(new DateTime(2009, 1, 4).IsFirstWeekOfMonth(russianCulture));
            Assert.False(new DateTime(2009, 1, 5).IsFirstWeekOfMonth(russianCulture));

            Assert.False(new DateTime(2009, 12, 27).IsLastWeekOfMonth(russianCulture));
            Assert.True(new DateTime(2009, 12, 28).IsLastWeekOfMonth(russianCulture));
            Assert.True(new DateTime(2009, 12, 29).IsLastWeekOfMonth(russianCulture));
            Assert.True(new DateTime(2009, 12, 31).IsLastWeekOfMonth(russianCulture));
        }

        [Fact]
        public void WeeksInMonth()
        {
            Assert.Equal(5, DateTimeExtensions.GetWeeksInMonth(2009, 1));
            Assert.Equal(4, DateTimeExtensions.GetWeeksInMonth(2009, 2));
            Assert.Equal(6, DateTimeExtensions.GetWeeksInMonth(2009, 5));
        }
    }
}
