using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using PropMT5Service.Constants;
using PropMT5Service.Helpers;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace PropMT5Service.Controllers
{
    /// <summary>
    /// Base controller providing common error handling, response factories,
    /// and MT5 resource management for all API controllers.
    /// </summary>
    public abstract class BaseApiController : ApiController
    {
        protected readonly CIMTManagerAPI _manager;

        protected BaseApiController(CIMTManagerAPI manager)
        {
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
        }

        // ──────────────────────────────────────────────────────────────
        // Synchronous execution wrappers
        // ──────────────────────────────────────────────────────────────

        /// <summary>Executes a synchronous action returning BaseResponse&lt;T&gt; with unified exception handling.</summary>
        protected IHttpActionResult ExecuteSafe<T>(Func<BaseResponse<T>> action)
        {
            try
            {
                var result = action();
                // Add this null check!
                if (result == null)
                {
                    return InternalErrorResponse<T>("The requested action returned a null response.");
                }
                return Content((HttpStatusCode)result.StatusCode, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequestResponse<T>(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequestResponse<T>(ex.Message);
            }
            catch (Exception ex)
            {
                return InternalErrorResponse<T>(ex.Message);
            }
        }

        /// <summary>Executes a synchronous action returning BaseResponse with unified exception handling.</summary>
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
                    new BaseResponse().WithError(string.Format(MT5Constants.ResponseMessages.UnexpectedError, ex.Message), 500));
            }
        }

        // ──────────────────────────────────────────────────────────────
        // Asynchronous execution wrappers
        // ──────────────────────────────────────────────────────────────

        /// <summary>Executes an async action returning BaseResponse&lt;T&gt; with unified exception handling.</summary>
        protected async Task<IHttpActionResult> ExecuteSafeAsync<T>(Func<Task<BaseResponse<T>>> action)
        {
            try
            {
                var result = await action();
                return Content((HttpStatusCode)result.StatusCode, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequestResponse<T>(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequestResponse<T>(ex.Message);
            }
            catch (Exception ex)
            {
                return InternalErrorResponse<T>(ex.Message);
            }
        }

        // ──────────────────────────────────────────────────────────────
        // Response factory helpers
        // ──────────────────────────────────────────────────────────────

        protected IHttpActionResult OkResponse<T>(T data, string message = "Success")
            => Content(HttpStatusCode.OK, new BaseResponse<T>().WithSuccess(data, message));

        protected IHttpActionResult BadRequestResponse<T>(string message)
            => Content(HttpStatusCode.BadRequest, new BaseResponse<T>().WithError(message, 400));

        protected IHttpActionResult NotFoundResponse<T>(string message)
            => Content(HttpStatusCode.NotFound, new BaseResponse<T>().WithError(message, 404));

        protected IHttpActionResult InternalErrorResponse<T>(string message)
            => Content(HttpStatusCode.InternalServerError,
                new BaseResponse<T>().WithError(
                    string.Format(MT5Constants.ResponseMessages.UnexpectedError, message), 500));

        // ──────────────────────────────────────────────────────────────
        // MT5 helpers
        // ──────────────────────────────────────────────────────────────

        /// <summary>Returns true when an MT5 operation succeeded.</summary>
        protected bool IsSuccessful(MTRetCode retCode) => retCode == MTRetCode.MT_RET_OK;

        /// <summary>Translates an MT5 return code to a human-readable error message.</summary>
        protected string GetMT5ErrorMessage(MTRetCode retCode)
        {
            switch (retCode)
            {
                case MTRetCode.MT_RET_USR_LOGIN_EXIST: return "Login already exists";
                case MTRetCode.MT_RET_USR_INVALID_PASSWORD: return "Invalid password";
                case MTRetCode.MT_RET_ERR_NOTFOUND: return "User not found";
                case MTRetCode.MT_RET_ERR_PARAMS: return "Invalid parameters";
                case MTRetCode.MT_RET_ERR_CONNECTION: return "Connection error";
                default: return $"MT5 operation failed with code: {retCode}";
            }
        }

        /// <summary>
        /// Executes <paramref name="action"/> with a freshly created CIMTUser that is
        /// guaranteed to be released in the finally block.
        /// </summary>
        protected TResult WithUser<TResult>(ulong loginId, Func<CIMTUser, MTRetCode, TResult> action)
        {
            CIMTUser user = _manager.UserCreate();
            try
            {
                MTRetCode ret = _manager.UserGet(loginId, user);
                return action(user, ret);
            }
            finally
            {
                user.Release();
            }
        }

        /// <summary>
        /// Executes <paramref name="action"/> with a freshly created CIMTPositionArray that is
        /// guaranteed to be released in the finally block.
        /// </summary>
        protected TResult WithPositions<TResult>(ulong loginId, Func<CIMTPositionArray, MTRetCode, TResult> action)
        {
            CIMTPositionArray positions = _manager.PositionCreateArray();
            try
            {
                MTRetCode ret = _manager.PositionGet(loginId, positions);
                return action(positions, ret);
            }
            finally
            {
                positions.Release();
            }
        }
    }
}
