using CrmEarlyBound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Visitor_Management_Portal.ViewModels.Analysis
{
    public static class DictionaryExtensions
    {
        public static TValue Pop<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default)
        {
            if (dict.TryGetValue(key, out TValue value))
            {
                dict.Remove(key);
                return value;
            }
            return defaultValue;
        }
    }
}