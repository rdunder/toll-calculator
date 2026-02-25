using TollFeeCalculator.Interfaces;

namespace TollFeeCalculator.Services;

public class TollFeeCalculatorHardCodedRequirements : ITollFeeCalculator
{
    private const int DailyMaxFee = 60;
    private const int ChargeIntervalMinutes = 60;

    public int CalculateFee(IEnumerable<DateTime> timeStamps)
    {
        if (timeStamps == null!) return 0;
    
        var passages = timeStamps.ToList();
        if (passages.Count == 0) return 0;

        return passages
            .GroupBy(dt => dt.Date)
            .Sum(dayGroup => CalculateDailyFee(dayGroup.OrderBy(dt => dt)));
    }

    private int CalculateDailyFee(IEnumerable<DateTime> sortedPassages)
    {
        int dailyTotal = 0;
        DateTime? windowStart = null;
        int windowMaxFee = 0;

        foreach (var passage in sortedPassages)
        {
            int fee = GetPassageFee(passage);

            if (windowStart == null || (passage - windowStart.Value).TotalMinutes > ChargeIntervalMinutes)
            {
                dailyTotal += windowMaxFee;
                windowStart = passage;
                windowMaxFee = fee;
            }
            else if (fee > windowMaxFee)
            {
                windowMaxFee = fee;
            }
        }

        dailyTotal += windowMaxFee;
    
        return Math.Min(dailyTotal, DailyMaxFee);
    }

    private int GetPassageFee(DateTime passage)
    {
        return passage switch
        {
            // Toll-free: weekends
            { DayOfWeek: DayOfWeek.Saturday or DayOfWeek.Sunday } => 0,
        
            // Toll-free: 2026 holidays
            { Year: 2026, Month: 1, Day: 1 or 6 } => 0,
            { Year: 2026, Month: 4, Day: 3 or 5 or 6 } => 0,
            { Year: 2026, Month: 5, Day: 1 or 14 or 24 } => 0,
            { Year: 2026, Month: 6, Day: 6 or 19 or 20 } => 0,
            { Year: 2026, Month: 10, Day: 31 } => 0,
            { Year: 2026, Month: 12, Day: 24 or 25 or 26 } => 0,
        
            // 8 SEK
            { Hour: 6, Minute: >= 0 and <= 29 } or
                { Hour: 8, Minute: >= 30 } or
                { Hour: >= 9 and <= 14 } or
                { Hour: 18, Minute: >= 0 and <= 29 } => 8,
        
            // 13 SEK
            { Hour: 6, Minute: >= 30 and <= 59 } or
                { Hour: 8, Minute: >= 0 and <= 29 } or
                { Hour: 15, Minute: >= 0 and <= 29 } or
                { Hour: 17 } => 13,
        
            // 18 SEK
            { Hour: 7 } or
                { Hour: 15, Minute: >= 30 } or
                { Hour: 16 } => 18,
        
            // Outside toll hours
            _ => 0
        };
    }
}