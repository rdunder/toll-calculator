using TollFeeCalculator.Models;

namespace TollFeeCalculator.Interfaces;

public interface ITollFeeConfigurationProvider
{
    TollFeeCalculatorConfig GetConfiguration();
}