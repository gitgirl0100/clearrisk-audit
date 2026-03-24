using System.Security.Cryptography;
using System.Text;

namespace ClearRiskApi.Services
{
    public class RiskScoringService
    {
        public RiskEvaluationResult Evaluate(string contractAddress)
        {
            int seed = GetDeterministicSeed(contractAddress);
            Random random = new Random(seed);

            double ownershipRisk = NextRiskValue(random);
            double liquidityRisk = NextRiskValue(random);
            double distributionRisk = NextRiskValue(random);
            double codeTransparencyRisk = NextRiskValue(random);
            double activityRisk = NextRiskValue(random);

            double finalScore =
                ((0.25 * ownershipRisk) +
                 (0.30 * liquidityRisk) +
                 (0.20 * distributionRisk) +
                 (0.15 * codeTransparencyRisk) +
                 (0.10 * activityRisk)) * 100;

            finalScore = Math.Round(finalScore, 2);

            string riskTier = finalScore switch
            {
                < 30 => "Low",
                < 60 => "Moderate",
                _ => "High"
            };

            var breakdown = new RiskBreakdown
            {
                OwnershipRisk = ownershipRisk,
                LiquidityRisk = liquidityRisk,
                DistributionRisk = distributionRisk,
                CodeTransparencyRisk = codeTransparencyRisk,
                ActivityRisk = activityRisk
            };

            string reportHash = GenerateReportHash(contractAddress, finalScore, riskTier, breakdown);

            return new RiskEvaluationResult
            {
                FinalScore = finalScore,
                RiskTier = riskTier,
                Breakdown = breakdown,
                ReportHash = reportHash
            };
        }

        private static int GetDeterministicSeed(string contractAddress)
        {
            using SHA256 sha256 = SHA256.Create();
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(contractAddress));

            return BitConverter.ToInt32(hashBytes, 0);
        }

        private static double NextRiskValue(Random random)
        {
            return Math.Round(random.NextDouble(), 2);
        }

        private static string GenerateReportHash(
            string contractAddress,
            double finalScore,
            string riskTier,
            RiskBreakdown breakdown)
        {
            string rawReport =
                $"ContractAddress:{contractAddress}|" +
                $"FinalScore:{finalScore}|" +
                $"RiskTier:{riskTier}|" +
                $"OwnershipRisk:{breakdown.OwnershipRisk}|" +
                $"LiquidityRisk:{breakdown.LiquidityRisk}|" +
                $"DistributionRisk:{breakdown.DistributionRisk}|" +
                $"CodeTransparencyRisk:{breakdown.CodeTransparencyRisk}|" +
                $"ActivityRisk:{breakdown.ActivityRisk}";

            using SHA256 sha256 = SHA256.Create();
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawReport));
            return Convert.ToHexString(hashBytes).ToLower();
        }
    }

    public class RiskEvaluationResult
    {
        public double FinalScore { get; set; }
        public string RiskTier { get; set; } = string.Empty;
        public RiskBreakdown Breakdown { get; set; } = new();
        public string ReportHash { get; set; } = string.Empty;
    }

    public class RiskBreakdown
    {
        public double OwnershipRisk { get; set; }
        public double LiquidityRisk { get; set; }
        public double DistributionRisk { get; set; }
        public double CodeTransparencyRisk { get; set; }
        public double ActivityRisk { get; set; }
    }
}