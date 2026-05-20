using MetaQuotes.MT5ManagerAPI;
using PropMT5Service.Services;
using PropMT5Service.ViewModels;
using System.Threading.Tasks;
using System.Web.Http;

namespace PropMT5Service.Controllers
{
    /// <summary>
    /// Controller for managing trade closing operations
    /// </summary>
    [RoutePrefix("api/trading/close")]
    public class CloseTradeController : BaseApiController
    {
        private readonly IMT5TradingService _tradingService;

        public CloseTradeController(CIMTManagerAPI manager) : base(manager)
        {
            _tradingService = new MT5TradingService(manager);
        }

        /// <summary>
        /// Close multiple trading positions
        /// </summary>
        /// <param name="request">Request containing login ID and position IDs to close</param>
        [HttpPost]
        [Route("positions")]
        public async Task<IHttpActionResult> ClosePositions([FromBody] ClosePositionRequest request)
        {
            if (request == null)
                return BadRequest("Request cannot be null");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _tradingService.ClosePositionsAsync(request);
            return Content((System.Net.HttpStatusCode)result.StatusCode, result);
        }

    }
}

