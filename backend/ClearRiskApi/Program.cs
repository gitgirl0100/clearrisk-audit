using ClearRiskApi.Services;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddSingleton<RiskScoringService>();

var app = builder.Build();

app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapPost("/api/evaluate", (EvalRequest req, RiskScoringService scoringService) =>
{
    var address = (req.Address ?? "").Trim();

    if (string.IsNullOrWhiteSpace(address))
    {
        return Results.BadRequest(new
        {
            error = "A contract address is required."
        });
    }

    if (!address.StartsWith("0x"))
    {
        return Results.BadRequest(new
        {
            error = "Invalid contract address. Ethereum addresses must begin with 0x."
        });
    }

    if (address.Length != 42)
    {
        return Results.BadRequest(new
        {
            error = "Invalid contract address. Ethereum addresses must be exactly 42 characters long."
        });
    }

    var hexPart = address.Substring(2);

    if (!Regex.IsMatch(hexPart, "^[0-9a-fA-F]{40}$"))
    {
        return Results.BadRequest(new
        {
            error = "Invalid contract address. Only hexadecimal characters are allowed after 0x."
        });
    }

    var result = scoringService.Evaluate(address);
    return Results.Ok(result);
});

app.Run("http://localhost:5000");

record EvalRequest(string Address);
