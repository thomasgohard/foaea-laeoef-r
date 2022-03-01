using System;
using System.Collections.Generic;
using System.Text;

namespace TestData.Helpers
{
    public static class PeriodHelper
    {
        public static DateTime AdjustNowForPeriod(DateTime now, int periodCount, string periodicPaymentCode)
        {
            if (periodCount <= 0)
                periodCount = 1;

            var result = periodicPaymentCode switch
            {
                "A" => now.AddDays(-7 * periodCount), // weekly
                "B" => now.AddDays(-14 * periodCount), // every 2 weeks
                "C" => now.AddMonths(-1 * periodCount), // monthly
                "D" => now.AddMonths(-3 * periodCount), // quaterly
                "E" => now.AddMonths(-6 * periodCount), // semi-annually
                "F" => now.AddYears(-1 * periodCount), // annually
                "G" => AdjustSemiMonthly(now, periodCount), // semi-monthly
                _ => throw new Exception("Invalid period!")
            };

            return result; //.AddDays(1);
        }

        private static DateTime AdjustSemiMonthly(DateTime now, int periodCount)
        {
            DateTime result = now;

            if (periodCount > 0)
            {
                for (int i = 0; i < periodCount; i++)
                {
                    if (result.Day >= 15)
                        result = new DateTime(result.Year, result.Month, 1);
                    else
                    {
                        if (result.Month > 1)
                            result = new DateTime(result.Year, result.Month - 1, 15);
                        else
                            result = new DateTime(result.Year - 1, 12, 15);
                    }
                }

            }

            return result;
        }
    }
}
