using ClearRiskApi.Services;

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

if (!address.StartsWith("0x") || address.Length != 42)
{
    return Results.BadRequest(new
    {
        error = "Invalid contract address. Expected 0x + 40 hex characters."
    });
}

var result = scoringService.Evaluate(address);
return Results.Ok(result);

});

app.Run("http://localhost:5000");

record EvalRequest(string Address);
