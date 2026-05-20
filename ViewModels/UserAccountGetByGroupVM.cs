namespace PropMT5Service.ViewModels
{
    public class UserAccountGetByGroupVM
    {
        public ulong Login { get; set; }
        public double Balance { get; set; }
        public double Credit { get; set; }
        public double Equity { get; set; }
        public double Margin { get; set; }
        public double MarginFree { get; set; }
        public double Profit { get; set; }
    }
}
