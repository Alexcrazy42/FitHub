namespace FitHub.Utilities.Collections;

public static class DictionaryExtension
{
    /// <summary>
    /// Получить или добавить
    /// </summary>
    public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue addedValue)
        where TKey : notnull
    {
        if (!dictionary.TryGetValue(key, out var value))
        {
            value = addedValue;
            dictionary.Add(key, value);
        }

        return value;
    }

    /// <summary>
    /// Получить или добавить
    /// </summary>
    public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> addValueFactory)
        where TKey : notnull
    {
        if (!dictionary.TryGetValue(key, out var value))
        {
            value = addValueFactory(key);
            dictionary.Add(key, value);
        }

        return value;
    }
}
