using System.ComponentModel.DataAnnotations;

namespace ClearRiskApi.Models
{
    public class AuditReport
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(42)]
        public string ContractAddress { get; set; } = string.Empty;

        public double FinalScore { get; set; }

        [Required]
        [MaxLength(20)]
        public string RiskTier { get; set; } = string.Empty;

        public double OwnershipRisk { get; set; }
        public double LiquidityRisk { get; set; }
        public double DistributionRisk { get; set; }
        public double CodeTransparencyRisk { get; set; }
        public double ActivityRisk { get; set; }

        [Required]
        [MaxLength(64)]
        public string ReportHash { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool BlockchainLogged { get; set; } = false;

        [MaxLength(200)]
        public string? TransactionHash { get; set; }
    }
}