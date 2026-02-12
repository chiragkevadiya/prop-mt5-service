namespace PropMT5ConnectionService.Configuration
{
    public class MT5ConnectionSettings
    {
        public MT5ServerConfig Live { get; set; }
        public MT5ServerConfig Demo { get; set; }
    }

    public class MT5ServerConfig
    {
        public string Server { get; set; }
        public ulong Login { get; set; }
        public string Password { get; set; }
        public uint Timeout { get; set; }
    }
}
