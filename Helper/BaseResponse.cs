using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;

namespace PropMT5ConnectionService.Helper
{
    // Base response without data
    public class BaseResponse
    {
        private bool _success;
        public bool Success
        {
            get => _success;
            set
            {
                _success = value;
                StatusCode = value ? 200 : 400; // Set status code based on success
            }
        }
        public string Message { get; set; }
        public int StatusCode { get; set; } = 200; // Default to OK
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

    // Generic base response with data
    public class BaseResponse<T>
    {
        private bool _success;
        public bool Success
        {
            get => _success;
            set
            {
                _success = value;
                StatusCode = value ? 200 : 400; // Set status code based on success
            }
        }
        public string Message { get; set; }
        public int StatusCode { get; set; } = 200; // Default to OK
        public T Data { get; set; }
        public BaseResponse<T> WithError(string message, int statusCode = 400)
        {
            Success = false;
            Message = message;
            StatusCode = statusCode;
            return this;
        }
      
    }

    public class BaseResponseModel<T>
    {
        private bool _success;
        public bool Success
        {
            get => _success;
            set
            {
                _success = value;
                StatusCode = value ? 200 : 400; // Set status code based on success
            }
        }
        public string Message { get; set; }
        public int StatusCode { get; set; } = 200; // Default to OK
        public int TotalRecords { get; set; }
        public T Data { get; set; }
    }

    public class BaseResponseObject<T>
    {
        private bool _success;
        public bool Success
        {
            get => _success;
            set
            {
                _success = value;
                StatusCode = value ? 200 : 400; // Set status code based on success
            }
        }
        public string Message { get; set; }
        public int StatusCode { get; set; } = 200; // Default to OK
        public T Data { get; set; }
    }

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
