#pragma warning disable CA1716

namespace RaysGitOpsDemo.Chassis.Caching;

/// <summary>
/// A provider for caching data.
/// </summary>
public interface ICache
{
    /// <summary>
    /// Returns the value for the specified key if it exists.
    /// </summary>
    /// <param name="key">The key of the value to be returned.</param>
    /// <typeparam name="T">The type of the value to be returned.</typeparam>
    /// <returns>The value for the specified key if it exists, otherwise default(T).</returns>
    T? Get<T>(string key);

    /// <summary>
    /// Returns the value for the specified key if it exists.
    /// </summary>
    /// <param name="key">The key of the value to be returned.</param>
    /// <typeparam name="T">The type of the value to be returned.</typeparam>
    /// <returns>The value for the specified key if it exists, otherwise default(T).</returns>
    Task<T?> GetAsync<T>(string key);

    /// <summary>
    /// Caches a value with a specified key.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Values inserted into the cache by this method will use the default expiration strategy.
    /// </para>
    /// </remarks>
    /// <param name="key">The key of the value to be stored in the cache.</param>
    /// <param name="value">The value to be stored in the cache.</param>
    /// <typeparam name="T">The type of the value being set.</typeparam>
    void Set<T>(string key, T value);

    /// <summary>
    /// Caches a value with a specified key.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Values inserted into the cache by this method will use the default expiration strategy.
    /// </para>
    /// </remarks>
    /// <param name="key">The key of the value to be stored in the cache.</param>
    /// <param name="value">The value to be stored in the cache.</param>
    /// <typeparam name="T">The type of the value being set.</typeparam>
    Task SetAsync<T>(string key, T value);

    /// <summary>
    /// Caches a value with a specified key.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Values inserted into the cache by this method will expire at the DateTime,
    /// <paramref name="absoluteExpiration" />.
    /// </para>
    /// </remarks>
    /// <param name="key">The key of the value to be stored in the cache.</param>
    /// <param name="value">The value to be stored in the cache.</param>
    /// <param name="absoluteExpiration">The absolute expiration date for the cache entry.</param>
    /// <typeparam name="T">The type of the value being set.</typeparam>
    void Set<T>(string key, T value, DateTime absoluteExpiration);

    /// <summary>
    /// Caches a value with a specified key.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Values inserted into the cache by this method will expire at the DateTime,
    /// <paramref name="absoluteExpiration" />.
    /// </para>
    /// </remarks>
    /// <param name="key">The key of the value to be stored in the cache.</param>
    /// <param name="value">The value to be stored in the cache.</param>
    /// <param name="absoluteExpiration">The absolute expiration date for the cache entry.</param>
    /// <typeparam name="T">The type of the value being set.</typeparam>
    Task SetAsync<T>(string key, T value, DateTime absoluteExpiration);

    /// <summary>
    /// Caches a value with a specified key.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Values inserted into the cache by this method will expire if idle for the TimeSpan,
    /// <paramref name="slidingExpiration" />.
    /// </para>
    /// </remarks>
    /// <param name="key">The key of the value to be stored in the cache.</param>
    /// <param name="value">The value to be stored in the cache.</param>
    /// <param name="slidingExpiration">The amount of time the value should remain cached if unused.</param>
    /// <typeparam name="T">The type of the value being set.</typeparam>
    void Set<T>(string key, T value, TimeSpan slidingExpiration);

    /// <summary>
    /// Caches a value with a specified key.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Values inserted into the cache by this method will expire if idle for the TimeSpan,
    /// <paramref name="slidingExpiration" />.
    /// </para>
    /// </remarks>
    /// <param name="key">The key of the value to be stored in the cache.</param>
    /// <param name="value">The value to be stored in the cache.</param>
    /// <param name="slidingExpiration">The amount of time the value should remain cached if unused.</param>
    /// <typeparam name="T">The type of the value being set.</typeparam>
    Task SetAsync<T>(string key, T value, TimeSpan slidingExpiration);

    /// <summary>
    /// Removes the value with the specified key from the cache.
    /// </summary>
    /// <param name="key">The specific key to remove from the cache.</param>
    void Remove(string key);

    /// <summary>
    /// Removes the value with the specified key from the cache.
    /// </summary>
    /// <param name="key">The specific key to remove from the cache.</param>
    Task RemoveAsync(string key);
}

#pragma warning restore CA1716