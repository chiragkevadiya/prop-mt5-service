namespace PropMT5ConnectionService.APIResponse
{
    public class ResponseHttpMessage
    {
        public string Data { get; set; } // Use appropriate type based on expected data structure
        public bool Success { get; set; }
        public string Message { get; set; }
        public int MTRetErrorCode { get; set; }
    }
    public class MT5AccountResponse<T>
    {
        public T Data { get; set; } // Use appropriate type based on expected data structure
        public bool Success { get; set; }
        public string Message { get; set; }
        public int MTRetErrorCode { get; set; }
    }
}
