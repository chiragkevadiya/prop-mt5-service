using MetaQuotes.MT5CommonAPI;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace PropMT5Service.Helpers
{
    /// <summary>
    /// Base response without data
    /// </summary>
    public class BaseResponse
    {
        private bool _success;
        public bool Success
        {
            get => _success;
            set
            {
                _success = value;
                StatusCode = value ? 200 : 400;
            }
        }
        public string Message { get; set; }
        public int StatusCode { get; set; } = 200;

        public BaseResponse WithError(string message, int statusCode = 400)
        {
            Success = false;
            Message = message;
            StatusCode = statusCode;
            return this;
        }

        public BaseResponse WithSuccess(string message, int statusCode = 200)
        {
            Success = true;
            Message = message;
            StatusCode = statusCode;
            return this;
        }
    }

    /// <summary>
    /// Generic base response with data
    /// </summary>
    public class BaseResponse<T>
    {
        private bool _success;
        public bool Success
        {
            get => _success;
            set
            {
                _success = value;
                StatusCode = value ? 200 : 400;
            }
        }
        public string Message { get; set; }
        public int StatusCode { get; set; } = 200;
        public T Data { get; set; }

        public BaseResponse<T> WithError(string message, int statusCode = 400)
        {
            Success = false;
            Message = message;
            StatusCode = statusCode;
            return this;
        }

        public BaseResponse<T> WithSuccess(T data, string message = "Success", int statusCode = 200)
        {
            Success = true;
            Message = message;
            StatusCode = statusCode;
            Data = data;
            return this;
        }
    }

    /// <summary>
    /// Response model with pagination support
    /// Consolidates BaseResponseModel and BaseResponseObject
    /// </summary>
    public class PagedResponse<T> : BaseResponse<T>
    {
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public PagedResponse<T> WithPagedData(T data, int totalRecords, int pageNumber = 1, int pageSize = 10, string message = "Success")
        {
            Success = true;
            Message = message;
            Data = data;
            TotalRecords = totalRecords;
            PageNumber = pageNumber;
            PageSize = pageSize;
            return this;
        }
    }

    /// <summary>
    /// MT5 specific response model
    /// </summary>
    public class MT5Response<T> : BaseResponse<T>
    {
        public MTRetCode MTRetCode { get; set; }

        public MT5Response<T> WithMT5Error(MTRetCode retCode, string message = null)
        {
            Success = false;
            MTRetCode = retCode;
            Message = message ?? GetMT5ErrorMessage(retCode);
            StatusCode = 400;
            return this;
        }

        public MT5Response<T> WithMT5Success(T data, string message = "Success")
        {
            Success = true;
            MTRetCode = MTRetCode.MT_RET_OK;
            Message = message;
            Data = data;
            StatusCode = 200;
            return this;
        }

        private string GetMT5ErrorMessage(MTRetCode retCode)
        {
            if (retCode == MTRetCode.MT_RET_USR_LOGIN_EXIST)
                return "Login already exists";
            if (retCode == MTRetCode.MT_RET_USR_INVALID_PASSWORD)
                return "Invalid password";
            if (retCode == MTRetCode.MT_RET_ERR_NOTFOUND)
                return "Not found";
            if (retCode == MTRetCode.MT_RET_ERR_PARAMS)
                return "Invalid parameters";

            return $"MT5 operation failed: {retCode}";
        }
    }

    public class BaseResponseModel<T> : PagedResponse<T> { }

    public class BaseResponseObject<T> : BaseResponse<T> { }

    public class UploadError
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
    public class ApiRequest
    {
        public HttpMethod Method { get; set; }
        public string Url { get; set; }
        public object Body { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public bool IsJsonRequest { get; set; } = true;
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string ErrorMessage { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }

    public class ResponseContent
    {
        public string ErrorMessage { get; set; }
        public string ExtraData { get; set; }
    }

    public class ResponseModel
    {
        public bool success { get; set; }
        public string message { get; set; }
        public int code { get; set; }
        public DatabaseString data { get; set; }
    }

    public class DatabaseString
    {
        public string dbString { get; set; }
    }

}
