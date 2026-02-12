using System;
using System.Diagnostics;
using System.Threading;

namespace PropMT5ConnectionService.Services
{
    /// <summary>
    /// Interface for performance monitoring
    /// </summary>
    public interface IPerformanceMonitoringService
    {
        void TrackRequest(string endpoint, long durationMs, bool success);
        void TrackException(string source, Exception exception);
        PerformanceMetrics GetMetrics();
        void Reset();
    }

    /// <summary>
    /// Service for monitoring application performance
    /// </summary>
    public class PerformanceMonitoringService : IPerformanceMonitoringService
    {
        private long _totalRequests;
        private long _successfulRequests;
        private long _failedRequests;
        private long _totalDurationMs;
        private long _totalExceptions;
        private DateTime _startTime;
        private readonly object _lockObject = new object();

        public PerformanceMonitoringService()
        {
            _startTime = DateTime.UtcNow;
        }

        /// <summary>
        /// Track API request performance
        /// </summary>
        public void TrackRequest(string endpoint, long durationMs, bool success)
        {
            lock (_lockObject)
            {
                Interlocked.Increment(ref _totalRequests);
                Interlocked.Add(ref _totalDurationMs, durationMs);

                if (success)
                {
                    Interlocked.Increment(ref _successfulRequests);
                }
                else
                {
                    Interlocked.Increment(ref _failedRequests);
                }
            }
        }

        /// <summary>
        /// Track exceptions
        /// </summary>
        public void TrackException(string source, Exception exception)
        {
            lock (_lockObject)
            {
                Interlocked.Increment(ref _totalExceptions);
            }
        }

        /// <summary>
        /// Get current performance metrics
        /// </summary>
        public PerformanceMetrics GetMetrics()
        {
            lock (_lockObject)
            {
                var uptime = DateTime.UtcNow - _startTime;
                var avgResponseTime = _totalRequests > 0 ? _totalDurationMs / _totalRequests : 0;
                var successRate = _totalRequests > 0 ? (_successfulRequests * 100.0) / _totalRequests : 0;

                return new PerformanceMetrics
                {
                    TotalRequests = _totalRequests,
                    SuccessfulRequests = _successfulRequests,
                    FailedRequests = _failedRequests,
                    TotalExceptions = _totalExceptions,
                    AverageResponseTimeMs = avgResponseTime,
                    SuccessRate = successRate,
                    UptimeSeconds = (long)uptime.TotalSeconds,
                    StartTime = _startTime
                };
            }
        }

        /// <summary>
        /// Reset all metrics
        /// </summary>
        public void Reset()
        {
            lock (_lockObject)
            {
                _totalRequests = 0;
                _successfulRequests = 0;
                _failedRequests = 0;
                _totalDurationMs = 0;
                _totalExceptions = 0;
                _startTime = DateTime.UtcNow;
            }
        }
    }

    /// <summary>
    /// Performance metrics data
    /// </summary>
    public class PerformanceMetrics
    {
        public long TotalRequests { get; set; }
        public long SuccessfulRequests { get; set; }
        public long FailedRequests { get; set; }
        public long TotalExceptions { get; set; }
        public long AverageResponseTimeMs { get; set; }
        public double SuccessRate { get; set; }
        public long UptimeSeconds { get; set; }
        public DateTime StartTime { get; set; }
    }

    /// <summary>
    /// Performance monitoring middleware helper
    /// </summary>
    public static class PerformanceMonitoringExtensions
    {
        /// <summary>
        /// Measure execution time of an action
        /// </summary>
        public static T MeasurePerformance<T>(
            this IPerformanceMonitoringService monitor,
            string endpoint,
            Func<T> action)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var result = action();
                stopwatch.Stop();
                monitor.TrackRequest(endpoint, stopwatch.ElapsedMilliseconds, true);
                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                monitor.TrackRequest(endpoint, stopwatch.ElapsedMilliseconds, false);
                monitor.TrackException(endpoint, ex);
                throw;
            }
        }

        /// <summary>
        /// Measure execution time of an action (void)
        /// </summary>
        public static void MeasurePerformance(
            this IPerformanceMonitoringService monitor,
            string endpoint,
            Action action)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                action();
                stopwatch.Stop();
                monitor.TrackRequest(endpoint, stopwatch.ElapsedMilliseconds, true);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                monitor.TrackRequest(endpoint, stopwatch.ElapsedMilliseconds, false);
                monitor.TrackException(endpoint, ex);
                throw;
            }
        }
    }
}
