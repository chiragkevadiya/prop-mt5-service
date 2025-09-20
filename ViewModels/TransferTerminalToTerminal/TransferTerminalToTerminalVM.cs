using MetaQuotes.MT5CommonAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT5ConnectionService.ViewModels
{
    public class TransferTerminalToTerminalVM
    {
        public float Amount { get; set; }
        public ulong From { get; set; }
        public ulong To { get; set; }
    }
    
    public class TransferTerminalToTerminalResponse
    {
        public ulong SenderLoginId { get; set; }
        public ulong ReceiveLoginId { get; set; }
        //public double withdrawalTransactionId { get; set; }   // sender
        //public double depositTransactionId { get; set; }     // Receiver
        public double TransferAmount { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string mTRetCode { get; set; }
    }
}
