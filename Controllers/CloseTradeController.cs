using MetaQuotes.MT5ManagerAPI;
using PropMT5ConnectionService.Controllers;
using PropMT5ConnectionService.Services;
using PropMT5ConnectionService.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace PropMT5ConnectionService.Controllers
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

        /// <summary>
        /// Legacy endpoint for backward compatibility
        /// </summary>
        [HttpPost]
        [Route("trades-orders-close")]
        [System.Obsolete("Use POST /api/trading/close/positions instead")]
        public async Task<IHttpActionResult> TradesOrdersClose([FromBody] ClosePositionRequest entity)
        {
            var result = await _tradingService.ClosePositionsAsync(entity);
            
            if (result.Success)
            {
                return Ok(new { Success = result.Success, Message = result.Message, Data = result.Data });
            }
            else
            {
                return Content((System.Net.HttpStatusCode)result.StatusCode, result);
            }
        }
    }
}

