using MetaQuotes.MT5ManagerAPI;
using PropMT5ConnectionService.Services;
using System.Threading.Tasks;
using System.Web.Http;

namespace PropMT5ConnectionService.Controllers
{
    /// <summary>
    /// Controller for managing live trading data operations
    /// </summary>
    [RoutePrefix("api/trading/data")]
    public class LiveTradingDataController : BaseApiController
    {
        private readonly IMT5TradingService _tradingService;

        public LiveTradingDataController(CIMTManagerAPI manager) : base(manager)
        {
            _tradingService = new MT5TradingService(manager);
        }

        /// <summary>
        /// Get trading data filtered by entry type
        /// </summary>
        /// <param name="loginId">MT5 login ID</param>
        /// <param name="entryType">Entry type (0 = Open, 1 = Close)</param>
        /// <param name="fromDate">Start date (format: yyyy-MM-dd)</param>
        /// <param name="toDate">End date (format: yyyy-MM-dd)</param>
        [HttpGet]
        [Route("history")]
        public async Task<IHttpActionResult> GetTradingData(ulong loginId, uint? entryType, string fromDate, string toDate)
        {
            var result = await _tradingService.GetTradingDataAsync(loginId, entryType, fromDate, toDate);
            return Content((System.Net.HttpStatusCode)result.StatusCode, result);
        }

        /// <summary>
        /// Get open trades for a login
        /// </summary>
        [HttpGet]
        [Route("open/{loginId:long}")]
        public async Task<IHttpActionResult> GetOpenTrades(long loginId)
        {
            var result = await _tradingService.GetOpenTradesAsync((ulong)loginId);
            return Content((System.Net.HttpStatusCode)result.StatusCode, result);
        }

        /// <summary>
        /// Get closed trades for a login
        /// </summary>
        [HttpGet]
        [Route("closed/{loginId:long}")]
        public async Task<IHttpActionResult> GetClosedTrades(long loginId, string fromDate, string toDate)
        {
            var result = await _tradingService.GetClosedTradesAsync((ulong)loginId, fromDate, toDate);
            return Content((System.Net.HttpStatusCode)result.StatusCode, result);
        }

        /// <summary>
        /// Legacy endpoint for backward compatibility
        /// </summary>
        [HttpGet]
        [System.Obsolete("Use GET /api/trading/data instead")]
        public async Task<IHttpActionResult> TradingHistory(ulong loginId, uint entryType, string fromDate, string toDate)
        {
            var result = await _tradingService.GetTradingDataAsync(loginId, entryType, fromDate, toDate);

            if (result.Success)
            {
                return Ok(result.Data);
            }
            else
            {
                return Content((System.Net.HttpStatusCode)result.StatusCode, result);
            }
        }
    }
}

