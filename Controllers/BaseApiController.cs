using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using PropMT5ConnectionService.Helpers;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PropMT5ConnectionService.Controllers
{
    /// <summary>
    /// Base controller providing common functionality for all API controllers
    /// </summary>
    public abstract class BaseApiController : ApiController
    {
        protected readonly CIMTManagerAPI _manager;

        protected BaseApiController(CIMTManagerAPI manager)
        {
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
        }

        /// <summary>
        /// Executes an action and handles common exceptions
        /// </summary>
        protected IHttpActionResult ExecuteSafe<T>(Func<BaseResponse<T>> action)
        {
            try
            {
                var result = action();
                return Content((HttpStatusCode)result.StatusCode, result);
            }
            catch (ArgumentException ex)
            {
                return Content(HttpStatusCode.BadRequest, new BaseResponse<T>().WithError(ex.Message, 400));
            }
            catch (InvalidOperationException ex)
            {
                return Content(HttpStatusCode.BadRequest, new BaseResponse<T>().WithError(ex.Message, 400));
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, 
                    new BaseResponse<T>().WithError($"An unexpected error occurred: {ex.Message}", 500));
            }
        }

        /// <summary>
        /// Executes an action and handles common exceptions (non-generic version)
        /// </summary>
        protected IHttpActionResult ExecuteSafe(Func<BaseResponse> action)
        {
            try
            {
                var result = action();
                return Content((HttpStatusCode)result.StatusCode, result);
            }
            catch (ArgumentException ex)
            {
                return Content(HttpStatusCode.BadRequest, new BaseResponse().WithError(ex.Message, 400));
            }
            catch (InvalidOperationException ex)
            {
                return Content(HttpStatusCode.BadRequest, new BaseResponse().WithError(ex.Message, 400));
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, 
                    new BaseResponse().WithError($"An unexpected error occurred: {ex.Message}", 500));
            }
        }

        /// <summary>
        /// Checks if MT5 operation was successful
        /// </summary>
        protected bool IsSuccessful(MTRetCode retCode)
        {
            return retCode == MTRetCode.MT_RET_OK;
        }

        /// <summary>
        /// Gets error message for MT5 return code
        /// </summary>
        protected string GetMT5ErrorMessage(MTRetCode retCode)
        {
            if (retCode == MTRetCode.MT_RET_USR_LOGIN_EXIST)
                return "Login already exists";
            if (retCode == MTRetCode.MT_RET_USR_INVALID_PASSWORD)
                return "Invalid password";
            if (retCode == MTRetCode.MT_RET_ERR_NOTFOUND)
                return "User not found";
            if (retCode == MTRetCode.MT_RET_ERR_PARAMS)
                return "Invalid parameters";
            if (retCode == MTRetCode.MT_RET_ERR_CONNECTION)
                return "Connection error";
            
            return $"MT5 operation failed with code: {retCode}";
        }
    }
}
