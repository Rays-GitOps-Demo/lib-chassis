using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RaysGitOpsDemo.Chassis.Caching;

/// <summary>
/// Extension methods for the ICache interface
/// </summary>
public static class ICacheExtensions
{
    #region Synchronous Methods

    /// <summary>
    /// Returns a collection of values corresponding to the specified collection of <paramref name="keys" /> if they
    /// exists.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The returned collection will contain only values for <paramref name="keys" /> that were cache hits.  Cache
    /// misses will be skipped silently.
    /// </para>
    /// <para>
    /// All returned values should be of the same type, <typeparamref name="T" />.
    /// </para>
    /// </remarks>
    /// <param name="cache">The ICache that this extension method executes on.</param>
    /// <param name="keys">A collection of keys for the values to be returned.</param>
    /// <typeparam name="T">The type of the values to be returned.</typeparam>
    /// <returns>A collection of values corresponding to the specified collection of <paramref name="keys" /> if they exists.</returns>
    public static IEnumerable<T?> Get<T>(this ICache cache, IEnumerable<string> keys)
    {
        cache = cache ?? throw new ArgumentNullException(nameof(cache));
        keys = keys ?? throw new ArgumentNullException(nameof(keys));
        var values = new List<T?>();
        foreach (var key in keys)
        {
            var value = cache.Get<T>(key);
            values.Add(value);
        }

        return values;
    }

    /// <summary>
    /// Returns the value for the specified <paramref name="key" />.  If the cache does not contain a value for the
    /// specified <paramref name="key" />, the <paramref name="factory" /> function will be invoked to retrieve the value
    /// from cold storage so the cache can immediately be refreshed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If the key is a cache hit then the cached value is returned.  Upon a cache miss however, the
    /// <paramref name="factory" /> function is invoked and should return the value for the <paramref name="key" />.
    /// This method will take the output of <paramref name="factory" />, immediately refresh the cache, and return the
    /// value.
    /// </para>
    /// <para>
    /// Values inserted into the cache by this method use the default expiration strategy.
    /// </para>
    /// </remarks>
    /// <param name="cache">The ICache that this extension method executes on.</param>
    /// <param name="key">The key of the value to be returned.</param>
    /// <param name="factory">The function to invoke upon a cache miss.  This function should return the value from cold storage.</param>
    /// <typeparam name="T">=The type of the value to be returned.</typeparam>
    /// <returns>The value for the specified <paramref name="key" />.</returns>
    public static T? GetOrCreate<T>(this ICache cache, string key, Func<T> factory)
    {
        cache = cache ?? throw new ArgumentNullException(nameof(cache));
        key = key ?? throw new ArgumentNullException(nameof(key));
        factory = factory ?? throw new ArgumentNullException(nameof(factory));
        var value = cache.Get<T>(key);

        if (EqualityComparer<T>.Default.Equals(value, default(T)))
        {
            value = factory();
            if (!EqualityComparer<T>.Default.Equals(value, default(T)))
            {
                cache.Set(key, value);
            }
        }

        return value;
    }

    /// <summary>
    /// Returns the value for the specified <paramref name="key" />.  If the cache does not contain a value for the
    /// specified <paramref name="key" />, the <paramref name="factory" /> function will be invoked to retrieve the value
    /// from cold storage so the cache can immediately be refreshed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If the key is a cache hit then the cached value is returned.  Upon a cache miss however, the
    /// <paramref name="factory" /> function is invoked and should return the value for the <paramref name="key" />.
    /// This method will take the output of <paramref name="factory" />, immediately refresh the cache, and return the
    ///  value.
    /// </para>
    /// <para>
    /// Values inserted into the cache by this method use the default expiration strategy.
    /// </para>
    /// </remarks>
    /// <param name="cache">The ICache that this extension method executes on.</param>
    /// <param name="key">The key of the value to be returned.</param>
    /// <param name="factory">The function to invoke upon a cache miss.  This function should return the value from cold storage.</param>
    /// <typeparam name="T">The type of the value to be returned.</typeparam>
    /// <returns>The value for the specified <paramref name="key" />.</returns>
    public static T? GetOrCreate<T>(this ICache cache, string key, Func<string, T> factory)
    {
        return cache.GetOrCreate(key, () => factory(key));
    }

    /// <summary>
    /// Returns the value for the specified <paramref name="key" />.  If the cache does not contain a value for the
    /// specified <paramref name="key" />, the <paramref name="factory" /> function will be invoked to retrieve the value
    /// from cold storage so the cache can immediately be refreshed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If the key is a cache hit then the cached value is returned.  Upon a cache miss however, the
    /// <paramref name="factory" /> function is invoked and should return the value for the <paramref name="key" />.see
    /// This method will take the output of <paramref name="factory" />, immediately refresh the cache, and return the
    /// value.
    /// </para>
    /// <para>
    /// Values inserted into the cache by this method will expire at the specified DateTime.
    /// </para>
    /// </remarks>
    /// <param name="cache">The ICache that this extension method executes on.</param>
    /// <param name="key">The key of the value to be returned.</param>
    /// <param name="factory">The function to invoke upon a cache miss.  This function should return the value from cold storage.</param>
    /// <param name="absoluteExpiration">The absolute DateTime at which the value returned by <paramref name="factory" /> should expire.</param>
    /// <typeparam name="T">The type of the value to be returned.</typeparam>
    /// <returns>The value for the specified <paramref name="key" />.</returns>
    public static T? GetOrCreate<T>(this ICache cache, string key, Func<T> factory, 
        DateTime absoluteExpiration)
    {
        cache = cache ?? throw new ArgumentNullException(nameof(cache));
        key = key ?? throw new ArgumentNullException(nameof(key));
        factory = factory ?? throw new ArgumentNullException(nameof(factory));
        var value = cache.Get<T>(key);

        if (EqualityComparer<T>.Default.Equals(value, default(T)))
        {
            value = factory();
            if (!EqualityComparer<T>.Default.Equals(value, default(T)))
            {
                cache.Set(key, value, absoluteExpiration);
            }
        }

        return value;
    }

    /// <summary>
    /// Returns the value for the specified <paramref name="key" />.  If the cache does not contain a value for the
    /// specified <paramref name="key" />, the <paramref name="factory" /> function will be invoked to retrieve the value
    /// from cold storage so the cache can immediately be refreshed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If the key is a cache hit then the cached value is returned.  Upon a cache miss however, the
    /// <paramref name="factory" /> function is invoked and should return the value for the <paramref name="key" />.see
    /// This method will take the output of <paramref name="factory" />, immediately refresh the cache, and return the
    /// value.
    /// </para>
    /// <para>
    /// Values inserted into the cache by this method will expire at the specified DateTime.
    /// </para>
    /// </remarks>
    /// <param name="cache">The ICache that this extension method executes on.
    /// </param>
    /// <param name="key">The key of the value to be returned.</param>
    /// <param name="factory">The function to invoke upon a cache miss.  This function should return the value from cold storage.</param>
    /// <param name="absoluteExpiration">The absolute DateTime at which the value returned by <paramref name="factory" /> should expire.</param>
    /// <typeparam name="T">The type of the value to be returned.</typeparam>
    /// <returns>The value for the specified <paramref name="key" />.</returns>
    public static T? GetOrCreate<T>(this ICache cache, string key, Func<string, T> factory,
        DateTime absoluteExpiration)
    {
        return cache.GetOrCreate(key, () => factory(key), absoluteExpiration);
    }

    /// <summary>
    /// Returns the value for the specified <paramref name="key" />.  If the cache does not contain a value for the
    /// specified <paramref name="key" />, the <paramref name="factory" /> function will be invoked to retrieve the value
    /// from cold storage so the cache can immediately be refreshed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If the key is a cache hit then the cached value is returned.  Upon a cache miss however, the
    /// <paramref name="factory" /> function is invoked and should return the value for the <paramref name="key" />.see
    /// This method will take the output of <paramref name="factory" />, immediately refresh the cache, and return the
    /// value.
    /// </para>
    /// <para>
    /// Values inserted into the cache by this method will expire if idle for the specified TimeSpan.
    /// </para>
    /// </remarks>
    /// <param name="cache">The ICache that this extension method executes on.</param>
    /// <param name="key">The key of the value to be returned.</param>
    /// <param name="factory">The function to invoke upon a cache miss.  This function should return the value from cold storage.</param>
    /// <param name="slidingExpiration">The the amount of time values returned by <paramref name="factory" /> can remain idle before expiring.</param>
    /// <typeparam name="T">The type of the value to be returned.</typeparam>
    /// <returns>The value for the specified <paramref name="key" />.</returns>
    public static T? GetOrCreate<T>(this ICache cache, string key, Func<T> factory,
        TimeSpan slidingExpiration)
    {
        cache = cache ?? throw new ArgumentNullException(nameof(cache));
        key = key ?? throw new ArgumentNullException(nameof(key));
        factory = factory ?? throw new ArgumentNullException(nameof(factory));
        var value = cache.Get<T>(key);

        if (EqualityComparer<T>.Default.Equals(value, default(T)))
        {
            value = factory();
            if (!EqualityComparer<T>.Default.Equals(value, default(T)))
            {
                cache.Set(key, value, slidingExpiration);
            }
        }

        return value;
    }

    /// <summary>
    /// Returns the value for the specified <paramref name="key" />.  If the cache does not contain a value for the
    /// specified <paramref name="key" />, the <paramref name="factory" /> function will be invoked to retrieve the value
    /// from cold storage so the cache can immediately be refreshed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If the key is a cache hit then the cached value is returned.  Upon a cache miss however, the
    /// <paramref name="factory" /> function is invoked and should return the value for the <paramref name="key" />.see
    /// This method will take the output of <paramref name="factory" />, immediately refresh the cache, and return the
    /// value.
    /// </para>
    /// <para>
    /// Values inserted into the cache by this method will expire if idle for the specified TimeSpan.
    /// </para>
    /// </remarks>
    /// <param name="cache">The ICache that this extension method executes on.</param>
    /// <param name="key"> The key of the value to be returned.</param>
    /// <param name="factory">The function to invoke upon a cache miss.  This function should return the value from cold storage.</param>
    /// <param name="slidingExpiration">The the amount of time values returned by <paramref name="factory" /> can remain idle before expiring.</param>
    /// <typeparam name="T">The type of the value to be returned.</typeparam>
    /// <returns>The value for the specified <paramref name="key" />.</returns>
    public static T? GetOrCreate<T>(this ICache cache, string key, Func<string, T> factory, 
        TimeSpan slidingExpiration)
    {
        return cache.GetOrCreate(key, () => factory(key), slidingExpiration);
    }

    /// <summary>
    /// Returns the values for the specified <paramref name="keys" />.  If the cache does not contain a value for one of
    /// the specified <paramref name="keys" />, the <paramref name="factory" /> function will be invoked to retrieve the
    /// value from cold storage so the cache can immediately be refreshed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If a key is a cache hit then the cached value is returned.  Upon a cache miss however, the
    /// <paramref name="factory" /> function is invoked and should return the value for the key.  This method will take
    /// the output of <paramref name="factory" />, immediately refresh the cache, and return the value.
    /// </para>
    /// <para>
    /// Values inserted into the cache by this method use the default expiration strategy.
    /// </para>
    /// <para>
    /// All returned values should be of the same type, <typeparamref name="T" />.
    /// </para>
    /// </remarks>
    /// <param name="cache">The ICache that this extension method executes on.</param>
    /// <param name="keys">A collection of keys for the values to be returned.</param>
    /// <param name="factory">The function to invoke upon a cache miss.  This function should return the value from cold storage.</param>
    /// <typeparam name="T">The type of the values to be returned.</typeparam>
    /// <returns>The values for the specified <paramref name="keys" />.</returns>
    public static IEnumerable<T?> GetOrCreate<T>(this ICache cache, IEnumerable<string> keys,
        Func<string, T> factory)
    {
        cache = cache ?? throw new ArgumentNullException(nameof(cache));
        keys = keys ?? throw new ArgumentNullException(nameof(keys));
        var values = new List<T?>();
        foreach (var key in keys)
        {
            var value = cache.GetOrCreate(key, factory);
            values.Add(value);
        }

        return values;
    }

    /// <summary>
    /// Returns the values for the specified <paramref name="keys" />.  If the cache does not contain a value for one of
    /// the specified <paramref name="keys" />, the <paramref name="factory" /> function will be invoked to retrieve the
    /// value from cold storage so the cache can immediately be refreshed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If a key is a cache hit then the cached value is returned.  Upon a cache miss however, the
    /// <paramref name="factory" /> function is invoked and should return the value for the key.  This method will take
    /// the output of <paramref name="factory" />, immediately refresh the cache, and return the value.
    /// </para>
    /// <para>
    /// Values inserted into the cache by this method will expire at the specified DateTime.
    /// </para>
    /// <para>
    /// All returned values should be of the same type, <typeparamref name="T" />.
    /// </para>
    /// </remarks>
    /// <param name="cache">The ICache that this extension method executes on.</param>
    /// <param name="keys">A collection of keys for the values to be returned.</param>
    /// <param name="factory">The function to invoke upon a cache miss.  This function should return the value from cold storage.</param>
    /// <param name="absoluteExpiration">The absolute DateTime at which the value returned by <paramref name="factory" /> should expire.</param>
    /// <typeparam name="T">The type of the values to be returned.</typeparam>
    /// <returns>The values for the specified <paramref name="keys" />.</returns>
    public static IEnumerable<T?> GetOrCreate<T>(this ICache cache, IEnumerable<string> keys,
        Func<string, T> factory, DateTime absoluteExpiration)
    {
        cache = cache ?? throw new ArgumentNullException(nameof(cache));
        keys = keys ?? throw new ArgumentNullException(nameof(keys));
        var values = new List<T?>();
        foreach (var key in keys)
        {
            var value = cache.GetOrCreate(key, factory, absoluteExpiration);
            values.Add(value);
        }

        return values;
    }

    /// <summary>
    /// Returns the values for the specified <paramref name="keys" />.  If the cache does not contain a value for one of
    /// the specified <paramref name="keys" />, the <paramref name="factory" /> function will be invoked to retrieve the
    /// value from cold storage so the cache can immediately be refreshed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If a key is a cache hit then the cached value is returned.  Upon a cache miss however, the
    /// <paramref name="factory" /> function is invoked and should return the value for the key.  This method will take
    /// the output of <paramref name="factory" />, immediately refresh the cache, and return the value.
    /// </para>
    /// <para>
    /// Values inserted into the cache by this method will expire if idle for the specified TimeSpan.
    /// </para>
    /// <para>
    /// All returned values should be of the same type, <typeparamref name="T" />.
    /// </para>
    /// </remarks>
    /// <param name="cache">The ICache that this extension method executes on.</param>
    /// <param name="keys">A collection of keys for the values to be returned.</param>
    /// <param name="factory">The function to invoke upon a cache miss.  This function should return the value from cold storage.</param>
    /// <param name="slidingExpiration">The the amount of time values returned by <paramref name="factory" /> can remain idle before expiring.</param>
    /// <typeparam name="T">The type of the values to be returned.</typeparam>
    /// <returns>The values for the specified <paramref name="keys" />.</returns>
    public static IEnumerable<T?> GetOrCreate<T>(this ICache cache, IEnumerable<string> keys,
        Func<string, T> factory, TimeSpan slidingExpiration)
    {
        cache = cache ?? throw new ArgumentNullException(nameof(cache));
        keys = keys ?? throw new ArgumentNullException(nameof(keys));
        var values = new List<T?>();
        foreach (var key in keys)
        {
            var value = cache.GetOrCreate(key, factory, slidingExpiration);
            values.Add(value);
        }

        return values;
    }

    #endregion

    #region Asynchronous Methods

    /// <summary>
    /// Asynchronously returns a collection of values corresponding to the specified collection of <paramref name="keys" />
    /// if they exists.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The returned collection will contain only values for <paramref name="keys" /> that were cache hits.  Cache
    /// misses will be skipped silently.
    /// </para>
    /// <para>
    /// All returned values should be of the same type, <typeparamref name="T" />.
    /// </para>
    /// </remarks>
    /// <param name="cache">The ICache that this extension method executes on.</param>
    /// <param name="keys">A collection of keys for the values to be returned.</param>
    /// <typeparam name="T">The type of the values to be returned.</typeparam>
    /// <returns>A Task resulting in a collection of values corresponding to the specified collection of <paramref name="keys" /> if they exists.</returns>
    public static async Task<IEnumerable<T?>> GetAsync<T>(this ICache cache, IEnumerable<string> keys)
    {
        cache = cache ?? throw new ArgumentNullException(nameof(cache));
        keys = keys ?? throw new ArgumentNullException(nameof(keys));
        var values = new List<T?>();
        foreach (var key in keys)
        {
            var value = await cache.GetAsync<T>(key).ConfigureAwait(false);
            values.Add(value);
        }

        return values;
    }

    /// <summary>
    /// Asynchronously returns the value for the specified <paramref name="key" />.  If the cache does not contain a value
    /// for the specified <paramref name="key" />, the <paramref name="factory" /> function will be invoked to retrieve the
    /// value from cold storage so the cache can immediately be refreshed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If the key is a cache hit then the cached value is returned.  Upon a cache miss however, the
    /// <paramref name="factory" /> function is invoked and should return the value for the <paramref name="key" />.see
    /// This method will take the output of <paramref name="factory" />, immediately refresh the cache, and return the
    /// value.
    /// </para>
    /// <para>
    /// Values inserted into the cache by this method will expire if idle for the specified TimeSpan.
    /// </para>
    /// </remarks>
    /// <param name="cache">The ICache that this extension method executes on.</param>
    /// <param name="key">The key of the value to be returned.</param>
    /// <param name="factory">The function to invoke upon a cache miss.  This function should return the value from cold storage.</param>
    /// <typeparam name="T">The type of the value to be returned.</typeparam>
    /// <returns>A Task resulting in the value for the specified <paramref name="key" />.</returns>
    public static async Task<T?> GetOrCreateAsync<T>(this ICache cache, string key, Func<Task<T>> factory)
    {
        cache = cache ?? throw new ArgumentNullException(nameof(cache));
        key = key ?? throw new ArgumentNullException(nameof(key));
        factory = factory ?? throw new ArgumentNullException(nameof(factory));
        var value = await cache.GetAsync<T>(key).ConfigureAwait(false);

        if (EqualityComparer<T>.Default.Equals(value, default(T)))
        {
            value = await factory().ConfigureAwait(false);
            if (!EqualityComparer<T>.Default.Equals(value, default(T)))
            {
                await cache.SetAsync(key, value).ConfigureAwait(false);
            }
        }

        return value;
    }

    /// <summary>
    /// Asynchronously returns the value for the specified <paramref name="key" />.  If the cache does not contain a value
    /// for the specified <paramref name="key" />, the <paramref name="factory" /> function will be invoked to retrieve the
    /// value from cold storage so the cache can immediately be refreshed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If the key is a cache hit then the cached value is returned.  Upon a cache miss however, the
    /// <paramref name="factory" /> function is invoked and should return the value for the <paramref name="key" />.see
    /// This method will take the output of <paramref name="factory" />, immediately refresh the cache, and return the
    /// value.
    /// </para>
    /// <para>
    /// Values inserted into the cache by this method will expire if idle for the specified TimeSpan.
    /// </para>
    /// </remarks>
    /// <param name="cache">The ICache that this extension method executes on.</param>
    /// <param name="key">The key of the value to be returned.</param>
    /// <param name="factory">The function to invoke upon a cache miss.  This function should return the value from cold storage.</param>
    /// <param name="slidingExpiration">The the amount of time values returned by <paramref name="factory" /> can remain idle before expiring.</param>
    /// <typeparam name="T">The type of the value to be returned.</typeparam>
    /// <returns>A Task resulting in the value for the specified <paramref name="key" />.</returns>
    public static async Task<T?> GetOrCreateAsync<T>(this ICache cache, string key, Func<Task<T>> factory,
        TimeSpan slidingExpiration)
    {
        cache = cache ?? throw new ArgumentNullException(nameof(cache));
        key = key ?? throw new ArgumentNullException(nameof(key));
        factory = factory ?? throw new ArgumentNullException(nameof(factory));
        var value = await cache.GetAsync<T>(key).ConfigureAwait(false);

        if (EqualityComparer<T>.Default.Equals(value, default(T)))
        {
            value = await factory().ConfigureAwait(false);
            if (!EqualityComparer<T>.Default.Equals(value, default(T)))
            {
                await cache.SetAsync(key, value, slidingExpiration).ConfigureAwait(false);
            }
        }

        return value;
    }

    /// <summary>
    /// Asynchronously returns the value for the specified <paramref name="key" />.  If the cache does not contain a value
    /// for the specified <paramref name="key" />, the <paramref name="factory" /> function will be invoked to retrieve the
    /// value from cold storage so the cache can immediately be refreshed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If the key is a cache hit then the cached value is returned.  Upon a cache miss however, the
    /// <paramref name="factory" /> function is invoked and should return the value for the <paramref name="key" />.see
    /// This method will take the output of <paramref name="factory" />, immediately refresh the cache, and return the
    /// value.
    /// </para>
    /// <para>
    /// Values inserted into the cache by this method will expire if idle for the specified TimeSpan.
    /// </para>
    /// </remarks>
    /// <param name="cache">The ICache that this extension method executes on.</param>
    /// <param name="key">The key of the value to be returned.</param>
    /// <param name="factory">The function to invoke upon a cache miss.  This function should return the value from cold storage.</param>
    /// <param name="absoluteExpiration">The absolute DateTime at which the value returned by <paramref name="factory" /> should expire.</param>
    /// <typeparam name="T">The type of the value to be returned.</typeparam>
    /// <returns>A Task resulting in the value for the specified <paramref name="key" />.</returns>
    public static async Task<T?> GetOrCreateAsync<T>(this ICache cache, string key, Func<Task<T>> factory,
        DateTime absoluteExpiration)
    {
        cache = cache ?? throw new ArgumentNullException(nameof(cache));
        key = key ?? throw new ArgumentNullException(nameof(key));
        factory = factory ?? throw new ArgumentNullException(nameof(factory));
        var value = await cache.GetAsync<T>(key).ConfigureAwait(false);

        if (EqualityComparer<T>.Default.Equals(value, default(T)))
        {
            value = await factory().ConfigureAwait(false);
            if (!EqualityComparer<T>.Default.Equals(value, default(T)))
            {
                await cache.SetAsync(key, value, absoluteExpiration).ConfigureAwait(false);
            }
        }

        return value;
    }

    /// <summary>
    /// Asynchronously returns the value for the specified <paramref name="key" />.  If the cache does not contain a value
    /// for the specified <paramref name="key" />, the <paramref name="factory" /> function will be invoked to retrieve the
    /// value from cold storage so the cache can immediately be refreshed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If the key is a cache hit then the cached value is returned.  Upon a cache miss however, the
    /// <paramref name="factory" /> function is invoked and should return the value for the <paramref name="key" />.see
    /// This method will take the output of <paramref name="factory" />, immediately refresh the cache, and return the
    /// value.
    /// </para>
    /// <para>
    /// Values inserted into the cache by this method will expire if idle for the specified TimeSpan.
    /// </para>
    /// </remarks>
    /// <param name="cache">The ICache that this extension method executes on.</param>
    /// <param name="key">The key of the value to be returned.</param>
    /// <param name="factory">The function to invoke upon a cache miss.  This function should return the value from cold storage.</param>
    /// <typeparam name="T">The type of the value to be returned.</typeparam>
    /// <returns>A Task resulting in the value for the specified <paramref name="key" />.</returns>
    public static async Task<T?> GetOrCreateAsync<T>(this ICache cache, string key, Func<string, Task<T>> factory)
    {
        cache = cache ?? throw new ArgumentNullException(nameof(cache));
        key = key ?? throw new ArgumentNullException(nameof(key));
        factory = factory ?? throw new ArgumentNullException(nameof(factory));
        return await cache.GetOrCreateAsync<T>(key, async () => await factory(key).ConfigureAwait(false)).ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously returns the value for the specified <paramref name="key" />.  If the cache does not contain a value
    /// for the specified <paramref name="key" />, the <paramref name="factory" /> function will be invoked to retrieve the
    /// value from cold storage so the cache can immediately be refreshed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If the key is a cache hit then the cached value is returned.  Upon a cache miss however, the
    /// <paramref name="factory" /> function is invoked and should return the value for the <paramref name="key" />.see
    /// This method will take the output of <paramref name="factory" />, immediately refresh the cache, and return the
    /// value.
    /// </para>
    /// <para>
    /// Values inserted into the cache by this method will expire if idle for the specified TimeSpan.
    /// </para>
    /// </remarks>
    /// <param name="cache">The ICache that this extension method executes on.</param>
    /// <param name="key">The key of the value to be returned.</param>
    /// <param name="factory">The function to invoke upon a cache miss.  This function should return the value from cold storage.</param>
    /// <param name="slidingExpiration">The the amount of time values returned by <paramref name="factory" /> can remain idle before expiring.</param>
    /// <typeparam name="T">The type of the value to be returned.</typeparam>
    /// <returns>A Task resulting in the value for the specified <paramref name="key" />.</returns>
    public static async Task<T?> GetOrCreateAsync<T>(this ICache cache, string key, Func<string, Task<T>> factory,
        TimeSpan slidingExpiration)
    {
        cache = cache ?? throw new ArgumentNullException(nameof(cache));
        key = key ?? throw new ArgumentNullException(nameof(key));
        factory = factory ?? throw new ArgumentNullException(nameof(factory));
        return await cache.GetOrCreateAsync<T>(key, async () => await factory(key).ConfigureAwait(false), slidingExpiration).ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously returns the value for the specified <paramref name="key" />.  If the cache does not contain a value
    /// for the specified <paramref name="key" />, the <paramref name="factory" /> function will be invoked to retrieve the
    /// value from cold storage so the cache can immediately be refreshed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If the key is a cache hit then the cached value is returned.  Upon a cache miss however, the
    /// <paramref name="factory" /> function is invoked and should return the value for the <paramref name="key" />.see
    /// This method will take the output of <paramref name="factory" />, immediately refresh the cache, and return the
    /// value.
    /// </para>
    /// <para>
    /// Values inserted into the cache by this method will expire if idle for the specified TimeSpan.
    /// </para>
    /// </remarks>
    /// <param name="cache">The ICache that this extension method executes on.</param>
    /// <param name="key">The key of the value to be returned.</param>
    /// <param name="factory">The function to invoke upon a cache miss.  This function should return the value from cold storage.</param>
    /// <param name="absoluteExpiration">The absolute DateTime at which the value returned by <paramref name="factory" /> should expire.</param>
    /// <typeparam name="T">The type of the value to be returned.</typeparam>
    /// <returns>A Task resulting in the value for the specified <paramref name="key" />.</returns>
    public static async Task<T?> GetOrCreateAsync<T>(this ICache cache, string key, Func<string, Task<T>> factory,
        DateTime absoluteExpiration)
    {
        cache = cache ?? throw new ArgumentNullException(nameof(cache));
        key = key ?? throw new ArgumentNullException(nameof(key));
        factory = factory ?? throw new ArgumentNullException(nameof(factory));
        return await cache.GetOrCreateAsync<T>(key, async () => await factory(key).ConfigureAwait(false), absoluteExpiration).ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously returns the values for the specified <paramref name="keys" />.  If the cache does not contain a
    /// value for one of the specified <paramref name="keys" />, the <paramref name="factory" /> function will be invoked
    /// to retrieve the value from cold storage so the cache can immediately be refreshed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If a key is a cache hit then the cached value is returned.  Upon a cache miss however, the
    /// <paramref name="factory" /> function is invoked and should return the value for the key.  This method will take
    /// the output of <paramref name="factory" />, immediately refresh the cache, and return the value.
    /// </para>
    /// <para>
    /// Values inserted into the cache by this method use the default expiration strategy.
    /// </para>
    /// <para>
    /// All returned values should be of the same type, <typeparamref name="T" />.
    /// </para>
    /// </remarks>
    /// <param name="cache">The ICache that this extension method executes on.</param>
    /// <param name="keys">A collection of keys for the values to be returned.</param>
    /// <param name="factory">The function to invoke upon a cache miss.  This function should return the value from cold storage.</param>
    /// <typeparam name="T">The type of the values to be returned.</typeparam>
    /// <returns>A Task resulting in the values for the specified <paramref name="keys" />.</returns>
    public static async Task<IEnumerable<T?>> GetOrCreateAsync<T>(this ICache cache, IEnumerable<string> keys,
        Func<string, Task<T>> factory)
    {
        cache = cache ?? throw new ArgumentNullException(nameof(cache));
        keys = keys ?? throw new ArgumentNullException(nameof(keys));
        factory = factory ?? throw new ArgumentNullException(nameof(factory));
        var values = new List<T?>();
        foreach (var key in keys)
        {
            var value = await cache.GetOrCreateAsync<T>(key, factory).ConfigureAwait(false);
            values.Add(value);
        }

        return values;
    }

    /// <summary>
    /// Asynchronously returns the values for the specified <paramref name="keys" />.  If the cache does not contain a value
    /// for one of the specified <paramref name="keys" />, the <paramref name="factory" /> function will be invoked to
    /// retrieve the value from cold storage so the cache can immediately be refreshed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If a key is a cache hit then the cached value is returned.  Upon a cache miss however, the
    /// <paramref name="factory" /> function is invoked and should return the value for the key.  This method will take
    /// the output of <paramref name="factory" />, immediately refresh the cache, and return the value.
    /// </para>
    /// <para>
    /// Values inserted into the cache by this method will expire at the specified DateTime.
    /// </para>
    /// <para>
    /// All returned values should be of the same type, <typeparamref name="T" />.
    /// </para>
    /// </remarks>
    /// <param name="cache">The ICache that this extension method executes on.</param>
    /// <param name="keys">A collection of keys for the values to be returned.</param>
    /// <param name="factory">The function to invoke upon a cache miss.  This function should return the value from cold storage.</param>
    /// <param name="absoluteExpiration">The absolute DateTime at which the value returned by <paramref name="factory" /> should expire.</param>
    /// <typeparam name="T">The type of the values to be returned.</typeparam>
    /// <returns>A Task resulting in the values for the specified <paramref name="keys" />.</returns>
    public static async Task<IEnumerable<T?>> GetOrCreateAsync<T>(this ICache cache, IEnumerable<string> keys,
        Func<string, Task<T>> factory, DateTime absoluteExpiration)
    {
        cache = cache ?? throw new ArgumentNullException(nameof(cache));
        keys = keys ?? throw new ArgumentNullException(nameof(keys));
        factory = factory ?? throw new ArgumentNullException(nameof(factory));
        var values = new List<T?>();
        foreach (var key in keys)
        {
            var value = await cache.GetOrCreateAsync<T>(key, factory, absoluteExpiration).ConfigureAwait(false);
            values.Add(value);
        }

        return values;
    }

    /// <summary>
    /// Asynchronously returns the values for the specified <paramref name="keys" />.  If the cache does not contain a
    /// value for one of the specified <paramref name="keys" />, the <paramref name="factory" /> function will be invoked
    /// to retrieve the value from cold storage so the cache can immediately be refreshed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If a key is a cache hit then the cached value is returned.  Upon a cache miss however, the
    /// <paramref name="factory" /> function is invoked and should return the value for the key.  This method will take
    /// the output of <paramref name="factory" />, immediately refresh the cache, and return the value.
    /// </para>
    /// <para>
    /// Values inserted into the cache by this method will expire if idle for the specified TimeSpan.
    /// </para>
    /// <para>
    /// All returned values should be of the same type, <typeparamref name="T" />.
    /// </para>
    /// </remarks>
    /// <param name="cache">The ICache that this extension method executes on.</param>
    /// <param name="keys">A collection of keys for the values to be returned.</param>
    /// <param name="factory">The function to invoke upon a cache miss.  This function should return the value from cold storage.</param>
    /// <param name="slidingExpiration">The the amount of time values returned by <paramref name="factory" /> can remain idle before expiring.</param>
    /// <typeparam name="T">The type of the values to be returned.</typeparam>
    /// <returns>A Task resulting in the values for the specified <paramref name="keys" />.</returns>
    public static async Task<IEnumerable<T?>> GetOrCreateAsync<T>(this ICache cache, IEnumerable<string> keys,
        Func<string, Task<T>> factory, TimeSpan slidingExpiration)
    {
        cache = cache ?? throw new ArgumentNullException(nameof(cache));
        keys = keys ?? throw new ArgumentNullException(nameof(keys));
        factory = factory ?? throw new ArgumentNullException(nameof(factory));
        var values = new List<T?>();
        foreach (var key in keys)
        {
            var value = await cache.GetOrCreateAsync<T>(key, factory, slidingExpiration).ConfigureAwait(false);
            values.Add(value);
        }

        return values;
    }

    #endregion
}
