namespace TollFeeCalculator.Models;

public class TollFeeCalculatorConfig
{
    public int DailyMaxFee { get; init; }
    public int ChargeIntervalMinutes { get; init; }

    public IReadOnlyCollection<TollFeeInterval> FeeSchedule { get; init; } = null!;
}