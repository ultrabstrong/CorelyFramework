using System;

namespace Corely.Data.Dates
{
    public static class DayOfMonthCalculator
    {
        #region Methods

        /// <summary>
        /// Get last day of month for date
        /// </summary>
        /// <returns></returns>
        public static DateTime GetLastBusinessDayOfMonth(DateTime date)
        {
            DateTime lastWorkingDay = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
            if (lastWorkingDay.DayOfWeek == DayOfWeek.Sunday)
            {
                lastWorkingDay = lastWorkingDay.AddDays(-2);
            }
            else if (lastWorkingDay.DayOfWeek == DayOfWeek.Saturday)
            {
                lastWorkingDay = lastWorkingDay.AddDays(-1);
            }
            return lastWorkingDay;
        }

        #endregion

    }
}
