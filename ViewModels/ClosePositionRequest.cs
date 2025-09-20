using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT5ConnectionService.ViewModels
{
    public class ClosePositionRequest
    {
        [Required]
        public ulong LoginId { get; set; }
        [Required]
        public List<ulong> PositionId { get; set; }
    }

    public class ClosedTradeResponse
    {
        public ulong DealId { get; set; }
        public ulong PositionId { get; set; }
        public string Symbol { get; set; }
        public double Volume { get; set; }
        public double Price { get; set; }
        public double PriceOpen { get; set; }
        public double Profit { get; set; }
        public double Commission { get; set; }
        public double Swap { get; set; }
        public uint Action { get; set; }
        public uint Entry { get; set; }
        public uint Reason { get; set; }
        public DateTime Time { get; set; }
        public ulong Login { get; set; }
        public ulong Order { get; set; }
    }
}
