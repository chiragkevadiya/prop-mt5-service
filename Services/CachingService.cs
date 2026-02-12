using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace PropMT5ConnectionService.Services
{
    /// <summary>
    /// Interface for caching service
    /// </summary>
    public interface ICachingService
    {
        T Get<T>(string key);
        void Set<T>(string key, T value, TimeSpan? expiration = null);
        void Remove(string key);
        bool Exists(string key);
        void Clear();
    }

    /// <summary>
    /// Simple in-memory caching service using ConcurrentDictionary
    /// </summary>
    public class MemoryCachingService : ICachingService
    {
        private readonly ConcurrentDictionary<string, CacheEntry> _cache;
        private readonly TimeSpan _defaultExpiration;
        private readonly System.Threading.Timer _cleanupTimer;

        public MemoryCachingService(TimeSpan? defaultExpiration = null)
        {
            _cache = new ConcurrentDictionary<string, CacheEntry>();
            _defaultExpiration = defaultExpiration ?? TimeSpan.FromMinutes(15);

            // Cleanup expired entries every minute
            _cleanupTimer = new System.Threading.Timer(CleanupExpiredEntries, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
        }

        /// <summary>
        /// Get cached value by key
        /// </summary>
        public T Get<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or empty", nameof(key));

            if (_cache.TryGetValue(key, out CacheEntry entry))
            {
                if (entry.ExpiresAt > DateTime.UtcNow && entry.Value is T typedValue)
                {
                    return typedValue;
                }
                else
                {
                    // Remove expired entry
                    _cache.TryRemove(key, out _);
                }
            }

            return default(T);
        }

        /// <summary>
        /// Set value in cache with optional expiration
        /// </summary>
        public void Set<T>(string key, T value, TimeSpan? expiration = null)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or empty", nameof(key));

            var entry = new CacheEntry
            {
                Value = value,
                ExpiresAt = DateTime.UtcNow.Add(expiration ?? _defaultExpiration)
            };

            _cache.AddOrUpdate(key, entry, (k, oldValue) => entry);
        }

        /// <summary>
        /// Remove item from cache
        /// </summary>
        public void Remove(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or empty", nameof(key));

            _cache.TryRemove(key, out _);
        }

        /// <summary>
        /// Check if key exists in cache
        /// </summary>
        public bool Exists(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return false;

            if (_cache.TryGetValue(key, out CacheEntry entry))
            {
                if (entry.ExpiresAt > DateTime.UtcNow)
                {
                    return true;
                }
                else
                {
                    _cache.TryRemove(key, out _);
                }
            }

            return false;
        }

        /// <summary>
        /// Clear all cache entries
        /// </summary>
        public void Clear()
        {
            _cache.Clear();
        }

        private void CleanupExpiredEntries(object state)
        {
            var now = DateTime.UtcNow;
            var expiredKeys = new List<string>();

            foreach (var kvp in _cache)
            {
                if (kvp.Value.ExpiresAt <= now)
                {
                    expiredKeys.Add(kvp.Key);
                }
            }

            foreach (var key in expiredKeys)
            {
                _cache.TryRemove(key, out _);
            }
        }

        private class CacheEntry
        {
            public object Value { get; set; }
            public DateTime ExpiresAt { get; set; }
        }
    }

    /// <summary>
    /// Cache helper extensions
    /// </summary>
    public static class CacheExtensions
    {
        /// <summary>
        /// Get or set cached value
        /// </summary>
        public static T GetOrSet<T>(this ICachingService cache, string key, Func<T> valueFactory, TimeSpan? expiration = null)
        {
            if (cache.Exists(key))
            {
                return cache.Get<T>(key);
            }

            var value = valueFactory();
            cache.Set(key, value, expiration);
            return value;
        }

        /// <summary>
        /// Generate cache key from parts
        /// </summary>
        public static string GenerateKey(params object[] parts)
        {
            return string.Join(":", parts);
        }

        /// <summary>
        /// Cache key constants
        /// </summary>
        public static class CacheKeys
        {
            public const string AccountPrefix = "account";
            public const string TradingHistoryPrefix = "trading:history";
            public const string TradingDataPrefix = "trading:data";
            public const string GroupPrefix = "group";
            public const string SymbolPrefix = "symbol";
            public const string LeaderboardPrefix = "leaderboard";
            public const string DashboardPrefix = "dashboard";

            public static string Account(ulong loginId) => GenerateKey(AccountPrefix, loginId);
            public static string TradingHistory(ulong loginId, string date) => GenerateKey(TradingHistoryPrefix, loginId, date);
            public static string TradingData(ulong loginId, uint entryType, string date) => GenerateKey(TradingDataPrefix, loginId, entryType, date);
            public static string Group(string groupName) => GenerateKey(GroupPrefix, groupName);
            public static string Symbol(string symbolName) => GenerateKey(SymbolPrefix, symbolName);
            public static string Leaderboard(string groupName) => GenerateKey(LeaderboardPrefix, groupName);
        }
    }
}

