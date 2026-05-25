namespace PropMT5Service.ViewModels
{
    public class AccountDisableResult
    {
        public ulong LoginId { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public int ClosedPositions { get; set; }
        public double TotalProfit { get; set; }
        public double TotalSwap { get; set; }
        public double TotalCommission { get; set; }
        public double TotalPnL { get; set; }
        public double Balance { get; set; }
        public double ProfitPercentage { get; set; }
    }
}
