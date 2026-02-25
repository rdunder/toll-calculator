using TollFeeCalculator.Factories;
using TollFeeCalculator.Interfaces;
using TollFeeCalculator.Models;
using TollFeeCalculator.Services;

var builder = WebApplication.CreateBuilder(args);

var tollConfig = builder.Configuration
    .GetSection("TollConfig")
    .Get<TollFeeCalculatorConfig>() ?? TollFeeCalculatorConfigFactory.CreateDefault();

builder.Services.AddSingleton(tollConfig);
builder.Services.AddSingleton<IHolidayProvider, HolidayProvider2026>();
builder.Services.AddScoped<ITollFeeCalculator, TollFeeCalculatorService>();

builder.Services.AddOpenApi();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapPost("/api/toll/calculate", (
    CalculateFeeRequest request,
    ITollFeeCalculator calculator) =>
    {
        if (request.Passages == null! || !request.Passages.Any())
        {
            return Results.BadRequest("Passages are required");
        }

        var fee = calculator.CalculateFee(request.Passages);

        return Results.Ok(new CalculateFeeResponse(TotalFee: fee, Currency: "SEK"));
    });


app.Run();

record CalculateFeeRequest(List<DateTime> Passages);
record CalculateFeeResponse(int TotalFee, string Currency);