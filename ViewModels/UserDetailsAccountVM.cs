namespace MT5ConnectionService.ViewModels
{
    public class UserDetailsAccountVM
    {
        public double Balance { get; set; }
        public double Credit { get; set; }
        public double Equity { get; set; }
        public double Margin { get; set; }
        public double MarginFree { get; set; }
        public double Profit { get; set; }
        public double TotalCloseProfit { get; set; }
    }

    public class AccountDetailsVM
    {
        public ulong Login { get; set; }
        public double Balance { get; set; }
        public double Credit { get; set; }
        public double Equity { get; set; }
        public double MarginFree { get; set; }
        public double Profit { get; set; }
    }
}
