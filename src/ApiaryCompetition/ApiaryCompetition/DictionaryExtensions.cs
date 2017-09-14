using System.Collections.Generic;

namespace ApiaryCompetition
{
    public static class DictionaryExtensions
    {
        public static void Add<TKey, TValue>(IDictionary<TKey, TValue> keyToValue, IDictionary<TValue, TKey> valueToKey, TKey key, TValue value)
        {
            keyToValue[key] = value;
            valueToKey[value] = key;
        }
    }
}
