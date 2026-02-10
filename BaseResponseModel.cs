using MetaQuotes.MT5CommonAPI;

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
