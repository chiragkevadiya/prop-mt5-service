using MetaQuotes.MT5ManagerAPI;
using PropMT5Service.Constants;
using PropMT5Service.Services;
using System;
using System.Web.Http;

namespace PropMT5Service.Controllers
{
    /// <summary>
    /// Health check and diagnostics controller
    /// </summary>
    [RoutePrefix("api/health")]
    public class HealthCheckController : ApiController
    {
        private readonly CIMTManagerAPI _manager;
        private readonly IPerformanceMonitoringService _performanceMonitor;
        private readonly ICachingService _cache;

        public HealthCheckController(
            CIMTManagerAPI manager,
            IPerformanceMonitoringService performanceMonitor,
            ICachingService cache)
        {
            _manager = manager;
            _performanceMonitor = performanceMonitor;
            _cache = cache;
        }

        /// <summary>
        /// Basic health check endpoint
        /// </summary>
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetHealth()
        {
            var health = new
            {
                Status = MT5Constants.HealthCheck.StatusHealthy,
                Service = MT5Constants.ServiceInfo.Name,
                Version = MT5Constants.ServiceInfo.Version,
                Timestamp = DateTime.UtcNow,
                Uptime = GetUptime()
            };

            return Ok(health);
        }

        /// <summary>
        /// Detailed health check with all dependencies
        /// </summary>
        [HttpGet]
        [Route("detailed")]
        public IHttpActionResult GetDetailedHealth()
        {
            var mt5Status = CheckMT5Connection();
            var cacheStatus = CheckCacheService();
            var metrics = _performanceMonitor.GetMetrics();

            var health = new
            {
                Status = mt5Status && cacheStatus ? MT5Constants.HealthCheck.StatusHealthy : MT5Constants.HealthCheck.StatusDegraded,
                Service = MT5Constants.ServiceInfo.Name,
                Version = MT5Constants.ServiceInfo.Version,
                Timestamp = DateTime.UtcNow,
                Dependencies = new
                {
                    MT5Connection = new
                    {
                        Status = mt5Status ? MT5Constants.HealthCheck.MT5Connected : MT5Constants.HealthCheck.MT5Disconnected,
                        Healthy = mt5Status
                    },
                    CacheService = new
                    {
                        Status = cacheStatus ? MT5Constants.HealthCheck.CacheRunning : MT5Constants.HealthCheck.CacheDown,
                        Healthy = cacheStatus
                    }
                },
                Performance = new
                {
                    TotalRequests = metrics.TotalRequests,
                    SuccessRate = $"{metrics.SuccessRate:F2}%",
                    AverageResponseTimeMs = metrics.AverageResponseTimeMs,
                    TotalExceptions = metrics.TotalExceptions,
                    UptimeSeconds = metrics.UptimeSeconds
                },
                Memory = new
                {
                    WorkingSetMB = GC.GetTotalMemory(false) / (1024.0 * 1024.0),
                    GCGeneration = GC.MaxGeneration
                }
            };

            return Ok(health);
        }

        /// <summary>
        /// Get performance metrics
        /// </summary>
        [HttpGet]
        [Route("metrics")]
        public IHttpActionResult GetMetrics()
        {
            var metrics = _performanceMonitor.GetMetrics();
            return Ok(metrics);
        }

        /// <summary>
        /// Reset performance metrics
        /// </summary>
        [HttpPost]
        [Route("metrics/reset")]
        public IHttpActionResult ResetMetrics()
        {
            _performanceMonitor.Reset();
            return Ok(new { Message = MT5Constants.HealthCheck.MetricsResetSuccess, Timestamp = DateTime.UtcNow });
        }

        /// <summary>
        /// Liveness probe
        /// </summary>
        [HttpGet]
        [Route("live")]
        public IHttpActionResult GetLiveness()
        {
            return Ok(new { Status = MT5Constants.HealthCheck.StatusAlive, Timestamp = DateTime.UtcNow });
        }

        /// <summary>
        /// Readiness probe
        /// </summary>
        [HttpGet]
        [Route("ready")]
        public IHttpActionResult GetReadiness()
        {
            var mt5Ready = CheckMT5Connection();

            if (mt5Ready)
                return Ok(new { Status = MT5Constants.HealthCheck.StatusReady, Timestamp = DateTime.UtcNow });

            return Content(System.Net.HttpStatusCode.ServiceUnavailable,
                new { Status = MT5Constants.HealthCheck.StatusNotReady, Reason = MT5Constants.HealthCheck.MT5NotAvailable, Timestamp = DateTime.UtcNow });
        }

        #region Private Helper Methods

        private bool CheckMT5Connection()
        {
            try
            {
                if (_manager == null)
                    return false;

                // Try to create a user array as a simple connectivity check
                var testArray = _manager.UserCreateArray();
                if (testArray != null)
                {
                    testArray.Release();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        private bool CheckCacheService()
        {
            try
            {
                _cache.Set(MT5Constants.HealthCheck.CacheTestKey, MT5Constants.HealthCheck.CacheTestValue,
                    TimeSpan.FromSeconds(MT5Constants.HealthCheck.CacheTestTimeoutSeconds));
                var value = _cache.Get<string>(MT5Constants.HealthCheck.CacheTestKey);
                _cache.Remove(MT5Constants.HealthCheck.CacheTestKey);
                return value == MT5Constants.HealthCheck.CacheTestValue;
            }
            catch
            {
                return false;
            }
        }

        private string GetUptime()
        {
            var startTime = System.Diagnostics.Process.GetCurrentProcess().StartTime;
            var uptime = DateTime.Now - startTime;
            return $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s";
        }

        #endregion
    }
}
