namespace ClearRiskApi.Services;

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

public class RiskScoringService
{
public object Evaluate(string address)
{
double O = 0.8;
double L = 0.6;
double D = 0.7;
double C = 0.4;
double A = 0.5;

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

    return new
    {
        report,
        reportHash
    };
}

private static string GetTier(double score)
{
    if (score >= 76) return "Extreme";
    if (score >= 51) return "High";
    if (score >= 26) return "Moderate";
    return "Low";
}

private static string HashSha256(object obj)
{
    var json = JsonSerializer.Serialize(obj);
    using var sha = SHA256.Create();
    var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(json));
    return Convert.ToHexString(bytes).ToLowerInvariant();
}

}