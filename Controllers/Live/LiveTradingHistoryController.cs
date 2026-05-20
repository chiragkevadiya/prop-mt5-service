using MetaQuotes.MT5ManagerAPI;
using PropMT5Service.Services;
using System.Threading.Tasks;
using System.Web.Http;

namespace PropMT5Service.Controllers
{
    /// <summary>
    /// Controller for managing trading history operations
    /// </summary>
    [RoutePrefix("api/trading/history")]
    public class LiveTradingHistoryController : BaseApiController
    {
        private readonly IMT5TradingService _tradingService;

        public LiveTradingHistoryController(CIMTManagerAPI manager) : base(manager)
        {
            _tradingService = new MT5TradingService(manager);
        }

        /// <summary>
        /// Get trading history for a login between specified dates
        /// </summary>
        /// <param name="loginId">MT5 login ID</param>
        /// <param name="fromDate">Start date (format: yyyy-MM-dd)</param>
        /// <param name="toDate">End date (format: yyyy-MM-dd)</param>
        [HttpGet]
        [Route("{loginId:long}")]
        public async Task<IHttpActionResult> GetTradingHistory(long loginId, string fromDate, string toDate)
        {
            var result = await _tradingService.GetTradingHistoryAsync((ulong)loginId, fromDate, toDate);
            return Content((System.Net.HttpStatusCode)result.StatusCode, result);
        }

    }
}

