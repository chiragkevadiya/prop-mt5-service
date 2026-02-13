using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using PropMT5ConnectionService.Helpers;
using System;
using System.Web.Http;

namespace PropMT5ConnectionService.Controllers
{
    /// <summary>
    /// Controller for MT5 server management and monitoring
    /// </summary>
    [RoutePrefix("api/server")]
    public class ServerController : BaseApiController
    {
        public ServerController(CIMTManagerAPI manager) : base(manager) { }

        /// <summary>
        /// Get server time
        /// </summary>
        [HttpGet]
        [Route("time")]
        public IHttpActionResult GetServerTime()
        {
            return ExecuteSafe(() =>
            {
                long serverTime = _manager.TimeServer();
                var serverDateTime = DateTimeOffset.FromUnixTimeSeconds(serverTime).LocalDateTime;

                var data = new
                {
                    UnixTimestamp = serverTime,
                    DateTime = serverDateTime,
                    UTC = DateTimeOffset.FromUnixTimeSeconds(serverTime).UtcDateTime
                };

                return new BaseResponse<object>().WithSuccess(data, "Server time retrieved successfully");
            });
        }

        /// <summary>
        /// Get server ping (check connection)
        /// </summary>
        [HttpGet]
        [Route("ping")]
        public IHttpActionResult Ping()
        {
            return ExecuteSafe(() =>
            {
                var startTime = DateTime.UtcNow;
                // Try to get server time as a ping alternative
                long serverTime = _manager.TimeServer();
                var endTime = DateTime.UtcNow;
                var latency = (endTime - startTime).TotalMilliseconds;

                var data = new
                {
                    Success = serverTime > 0,
                    LatencyMs = latency,
                    ServerTime = DateTimeOffset.FromUnixTimeSeconds(serverTime).LocalDateTime
                };

                return new BaseResponse<object>().WithSuccess(data, "Server is reachable");
            });
        }

        /// <summary>
        /// Get MT5 API version information
        /// </summary>
        [HttpGet]
        [Route("version")]
        public IHttpActionResult GetVersion()
        {
            return ExecuteSafe(() =>
            {
                // Get version info from manager
                var data = new
                {
                    APIVersion = "MT5 Manager API",
                    ServerTime = DateTimeOffset.FromUnixTimeSeconds(_manager.TimeServer()).LocalDateTime
                };

                return new BaseResponse<object>().WithSuccess(data, "Version information retrieved successfully");
            });
        }

        /// <summary>
        /// Get total counts for various entities
        /// </summary>
        [HttpGet]
        [Route("stats")]
        public IHttpActionResult GetServerStats()
        {
            return ExecuteSafe(() =>
            {
                var stats = new
                {
                    TotalUsers = _manager.UserTotal(),
                    TotalGroups = _manager.GroupTotal(),
                    TotalSymbols = _manager.SymbolTotal(),
                    ServerTime = DateTimeOffset.FromUnixTimeSeconds(_manager.TimeServer()).LocalDateTime
                };

                return new BaseResponse<object>().WithSuccess(stats, "Server statistics retrieved successfully");
            });
        }

        /// <summary>
        /// Subscribe to trading events (requires implementation of sink)
        /// </summary>
        [HttpPost]
        [Route("subscribe")]
        public IHttpActionResult Subscribe()
        {
            return ExecuteSafe(() =>
            {
                // Note: Subscription requires implementing CIMTManagerSink interface
                // This is a placeholder - actual implementation depends on your sink class
                return new BaseResponse<object>().WithSuccess(null, "Subscribe requires CIMTManagerSink implementation");
            });
        }

        /// <summary>
        /// Check if manager is connected
        /// </summary>
        [HttpGet]
        [Route("connected")]
        public IHttpActionResult IsConnected()
        {
            return ExecuteSafe(() =>
            {
                // Try a simple operation to check connection
                long serverTime = _manager.TimeServer();
                var isConnected = serverTime > 0;

                var data = new
                {
                    Connected = isConnected,
                    ServerTime = isConnected ? DateTimeOffset.FromUnixTimeSeconds(serverTime).LocalDateTime : (DateTime?)null
                };

                return new BaseResponse<object>().WithSuccess(data, isConnected ? "Manager is connected" : "Manager is not connected");
            });
        }
    }
}
