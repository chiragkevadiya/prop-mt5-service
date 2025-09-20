using MetaQuotes.MT5CommonAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT5ConnectionService
{
    public class BaseResponseModel<T>
    {
        public T Data { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public MTRetCode MTRetErrorCode { get; set; }
    }
    public class BaseResponseError
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public MTRetCode MTRetErrorCode { get; set; }
    }
}
