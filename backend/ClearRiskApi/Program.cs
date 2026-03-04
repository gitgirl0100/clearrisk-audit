using System.Security.Cryptography;
using System.Text;
using System.Text.Json;


var builder = WebApplication.CreateBuilder(args);

// Allow local frontend to call backend during development
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();
app.UseCors();

app.UseDefaultFiles();
app.UseStaticFiles();


// --- Helpers ---
static string GetTier(double score)
{
    if (score >= 76) return "Extreme";
    if (score >= 51) return "High";
    if (score >= 26) return "Moderate";
    return "Low";
}

static string HashSha256(object obj)
{
    var json = JsonSerializer.Serialize(obj);
    using var sha = SHA256.Create();
    var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(json));
    return Convert.ToHexString(bytes).ToLowerInvariant();
}

// --- API Endpoint ---
RouteHandlerBuilder routeHandlerBuilder = app.MapPost("/api/evaluate", (EvalRequest req) =>
{
    var address = (req.Address ?? "").Trim();

    // Basic Ethereum address validation
if (!address.StartsWith("0x") || address.Length != 42)
{
    return Results.BadRequest(new
    {
        error = "Invalid contract address. Expected 0x + 40 hex characters."
    });
}

// Deterministic mock risk inputs (MVP phase)
double O = 0.8; // Ownership Privilege Risk
double L = 0.6; // Liquidity Risk
double D = 0.7; // Distribution Risk
double C = 0.4; // Code Transparency Risk
double A = 0.5; // Activity Risk

var finalScore = Math.Round((0.25 * O + 0.30 * L + 0.20 * D + 0.15 * C + 0.10 * A) * 100, 2);
var tier = GetTier(finalScore);

var report = new
{
    tokenAddress = address,
    finalScore,
    tier,
    breakdown = new
    {
        ownershipRisk = O,
        liquidityRisk = L,
        distributionRisk = D,
        codeTransparencyRisk = C,
        activityRisk = A
    }
};

var reportHash = HashSha256(report);

return Results.Ok(new
{
    report,
    reportHash
});
});

app.Run("http://localhost:5000");

record EvalRequest(string Address);
