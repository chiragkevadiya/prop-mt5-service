using System;
using static PropMT5ConnectionService.Helper.Constant;

namespace PropMT5ConnectionService.Models
{
    public class UserChallengePhase : BaseEntity
    {
        public int UserId { get; set; }
        public Guid UserChallengeHistoryId { get; set; }
        public decimal ProfitTargetPercentage { get; set; }
        public decimal ProfitSplitPercentage { get; set; }
        public decimal DailyLossPercent { get; set; }
        public decimal OverallLossPercent { get; set; }
        public int TradingPeriodDays { get; set; }
        public long UserAccountId { get; set; }
        public string AccounTypeName { get; set; }
        public string PhaseName { get; set; }
        public decimal InitialBalance { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal ProfitEarned => CurrentBalance - InitialBalance;
        public DateTime? CompletionDate { get; set; }
        // Anchors for P&L
        public decimal StartingEquity { get; set; }
        public decimal? ClosingEquity { get; set; }

        // Guard
        public bool TradingLocked { get; set; } = false;
        public string Comment { get; set; }
        public ChallengePhaseStatus ChallengePhaseStatus { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}