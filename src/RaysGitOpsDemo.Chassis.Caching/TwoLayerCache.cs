#pragma warning disable CA1848

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using Microsoft.Extensions.Caching.Memory;

namespace RaysGitOpsDemo.Chassis.Caching;

/// <inheritdoc />
/// <summary>
/// A 2-level distributed implementation of <see cref="ICache" />. The cache stores entries in
/// local memory for a short period of time and in a shared Redis cluster for long-term caching.
/// </summary>
internal class TwoLevelCache : ICache, IDisposable
{
    private readonly IMemoryCache _memoryCache;
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger _logger;
    private readonly CacheConfiguration _config;
    private readonly SemaphoreSlim _semaphore;
    private readonly TimeSpan _removeTimeout;

    /// <summary>
    /// Instantiate a new TwoLevelCache object.
    /// </summary>
    /// <param name="memoryCache">The MemoryCache to use.</param>
    /// <param name="distributedCache">A handle to the Microsoft distributed system this object wraps.</param>
    /// <param name="config">The configuration of the two-level cache system.</param>
    /// <param name="logger">The <see cref="ILogger" /> to use to write log events.</param>
    public TwoLevelCache(IMemoryCache memoryCache,
        IDistributedCache distributedCache,
        IOptions<CacheConfiguration> config,
        ILogger<TwoLevelCache> logger)
    {
        _memoryCache = memoryCache;
        _distributedCache = distributedCache;
        _config = config.Value ?? new CacheConfiguration();
        _logger = logger;
        _semaphore = new SemaphoreSlim(0, 1);
        _removeTimeout = _config.DefaultRemoveCommandTimeout;
    }

    /// <inheritdoc />
    public T? Get<T>(string key)
    {
        T? value = _memoryCache.Get<T>(key);
        if (!EqualityComparer<T>.Default.Equals(value, default))
        {
            return value;
        }

        _logger.LogTrace("Getting {Key} from distributed cache", key);

        string? cachedStringValue;
        try
        {
            cachedStringValue = _distributedCache.GetString(key);
        }
        catch (Exception e) when (e is RedisConnectionException || e is RedisTimeoutException)
        {
            _logger.LogWarning(e, "Redis is unavailable");
            return default;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting {Key} from distributed cache", key);
            return default;
        }


        if (string.IsNullOrWhiteSpace(cachedStringValue))
        {
            _logger.LogTrace("{Key} not found in distributed cache", key);
        }
        else
        {
            try
            {
                value = JsonConvert.DeserializeObject<T>(cachedStringValue);

                _logger.LogTrace("{Key} returned value from distributed cache", key);
                _logger.LogTrace("{@Value}", value);
                _memoryCache.Set(key, value);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error deserializing {Key} from distributed cache, clearing entry", key);
                Remove(key);
                return default;
            }
        }

        return value;
    }

    /// <inheritdoc />
    public async Task<T?> GetAsync<T>(string key)
    {
        T? value = _memoryCache.Get<T>(key);
        if (!EqualityComparer<T>.Default.Equals(value, default))
        {
            return value;
        }

        _logger.LogTrace("Getting {Key} from distributed cache", key);

        string? cachedStringValue;
        try
        {
            cachedStringValue = await _distributedCache.GetStringAsync(key).ConfigureAwait(false);
        }
        catch (Exception e) when (e is RedisConnectionException || e is RedisTimeoutException)
        {
            _logger.LogWarning(e, "Redis is unavailable");
            return default;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting {Key} from distributed cache", key);
            return default;
        }

        if (string.IsNullOrWhiteSpace(cachedStringValue))
        {
            _logger.LogTrace("{Key} not found in distributed cache", key);
        }
        else
        {
            try
            {
                value = JsonConvert.DeserializeObject<T>(cachedStringValue);
                _logger.LogTrace("{Key} returned value from distributed cache", key);
                _logger.LogTrace("{@Value}", value);
                _memoryCache.Set(key, value);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error deserializing {Key} from distributed cache, clearing entry", key);
                await RemoveAsync(key).ConfigureAwait(false);
                return default;
            }
        }

        return value;
    }

    /// <inheritdoc />
    public void Set<T>(string key, T value)
    {
        _logger.LogTrace("Setting {Key} with default expiration {DefaultExpiration} in distributed cache", key, _config.DefaultDistributedCacheTTL);
        _logger.LogTrace("{@Value}", value);

        string stringValue = JsonConvert.SerializeObject(value);

        try
        {
            _distributedCache.SetString(key, stringValue, new DistributedCacheEntryOptions { AbsoluteExpiration = DateTime.Now.Add(_config.DefaultDistributedCacheTTL) });
            _memoryCache.Set(key, value);
        }
        catch (Exception e) when (e is RedisConnectionException || e is RedisTimeoutException)
        {
            _logger.LogWarning(e, "Redis is unavailable. Falling back to in-memory cache only");
            _memoryCache.Set(key, value, DateTime.Now.Add(_config.DefaultDistributedCacheTTL));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error setting {Key} in distributed cache", key);
        }
    }

    /// <inheritdoc />
    public void Set<T>(string key, T value, DateTime absoluteExpiration)
    {
        _logger.LogTrace("Setting {Key} with absolute expiration {AbsoluteExpiration} in distributed cache", key, absoluteExpiration);
        _logger.LogTrace("{@Value}", value);

        string stringValue = JsonConvert.SerializeObject(value);

        try
        {
            _distributedCache.SetString(key, stringValue, new DistributedCacheEntryOptions { AbsoluteExpiration = absoluteExpiration });
            _memoryCache.Set(key, value);
        }
        catch (Exception e) when (e is RedisConnectionException || e is RedisTimeoutException)
        {
            _logger.LogWarning(e, "Redis is unavailable. Falling back to in-memory cache only");
            _memoryCache.Set(key, value, absoluteExpiration);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error setting {Key} in distributed cache", key);
        }
    }

    /// <inheritdoc />
    public void Set<T>(string key, T value, TimeSpan slidingExpiration)
    {
        _logger.LogTrace("Setting {Key} with sliding expiration {SlidingExpiration} in distributed cache", key, slidingExpiration);
        _logger.LogTrace("{@Value}", value);

        string stringValue = JsonConvert.SerializeObject(value);

        try
        {
            _distributedCache.SetString(key, stringValue, new DistributedCacheEntryOptions { SlidingExpiration = slidingExpiration });
            _memoryCache.Set(key, value);
        }
        catch (Exception e) when (e is RedisConnectionException || e is RedisTimeoutException)
        {
            _logger.LogWarning(e, "Redis is unavailable. Falling back to in-memory cache only");
            _memoryCache.Set(key, value, slidingExpiration);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error setting {Key} in distributed cache", key);
        }
    }

    /// <inheritdoc />
    public async Task SetAsync<T>(string key, T value)
    {
        _logger.LogTrace("Setting {Key} with default expiration {DefaultExpiration} in distributed cache", key, _config.DefaultDistributedCacheTTL);
        _logger.LogTrace("{@Value}", value);

        string stringValue = JsonConvert.SerializeObject(value);

        try
        {
            await _distributedCache.SetStringAsync(key, stringValue, new DistributedCacheEntryOptions { AbsoluteExpiration = DateTime.Now.Add(_config.DefaultDistributedCacheTTL) }).ConfigureAwait(false);
            _memoryCache.Set(key, value);
        }
        catch (Exception e) when (e is RedisConnectionException || e is RedisTimeoutException)
        {
            _logger.LogWarning(e, "Redis is unavailable. Falling back to in-memory cache only");
            _memoryCache.Set(key, value, DateTime.Now.Add(_config.DefaultDistributedCacheTTL));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error setting {Key} in distributed cache", key);
        }
    }

    /// <inheritdoc />
    public async Task SetAsync<T>(string key, T value, DateTime absoluteExpiration)
    {
        _logger.LogTrace("Setting {Key} with absolute expiration {AbsoluteExpiration} in distributed cache", key, absoluteExpiration);
        _logger.LogTrace("{@Value}", value);

        string stringValue = JsonConvert.SerializeObject(value);

        try
        {
            await _distributedCache.SetStringAsync(key, stringValue, new DistributedCacheEntryOptions { AbsoluteExpiration = absoluteExpiration }).ConfigureAwait(false);
            _memoryCache.Set(key, value);
        }
        catch (Exception e) when (e is RedisConnectionException || e is RedisTimeoutException)
        {
            _logger.LogWarning(e, "Redis is unavailable. Falling back to in-memory cache only");
            _memoryCache.Set(key, value, absoluteExpiration);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error setting {Key} in distributed cache", key);
        }
    }

    /// <inheritdoc />
    public async Task SetAsync<T>(string key, T value, TimeSpan slidingExpiration)
    {
        _logger.LogTrace("Setting {Key} with sliding expiration {SlidingExpiration} in distributed cache", key, slidingExpiration);
        _logger.LogTrace("{@Value}", value);

        string stringValue = JsonConvert.SerializeObject(value);

        try
        {
            await _distributedCache.SetStringAsync(key, stringValue, new DistributedCacheEntryOptions { SlidingExpiration = slidingExpiration }).ConfigureAwait(false);
            _memoryCache.Set(key, value);
        }
        catch (Exception e) when (e is RedisConnectionException || e is RedisTimeoutException)
        {
            _logger.LogWarning(e, "Redis is unavailable. Falling back to in-memory cache only");
            _memoryCache.Set(key, value, slidingExpiration);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error setting {Key} in distributed cache", key);
        }
    }

    /// <inheritdoc />
    public void Remove(string key)
    {
        _logger.LogTrace("Removing {Key} from distributed cache", key);

        try
        {
            _semaphore.Wait(_removeTimeout);
            _distributedCache.Remove(key);
        }
        catch (Exception e) when (e is RedisConnectionException || e is RedisTimeoutException)
        {
            _logger.LogWarning(e, "Redis is unavailable");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error removing {Key} from distributed cache", key);
        }
        finally
        {
            _semaphore.Release();
        }

        _memoryCache.Remove(key);
    }

    /// <inheritdoc />
    public async Task RemoveAsync(string key)
    {
        _logger.LogTrace("Removing {Key} from distributed cache", key);

        try
        {
            await _semaphore.WaitAsync(_removeTimeout).ConfigureAwait(false);
            await _distributedCache.RemoveAsync(key).ConfigureAwait(false);
        }
        catch (Exception e) when (e is RedisConnectionException || e is RedisTimeoutException)
        {
            _logger.LogWarning(e, "Redis is unavailable");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error removing {Key} from distributed cache", key);
        }
        finally
        {
            _semaphore.Release();
        }

        _memoryCache.Remove(key);
    }

    public void Dispose()
    {
        _semaphore.Dispose();
    }
}

#pragma warning restore CA1848