using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
namespace MusicSDK.Netease.Library
{
    public static class Utility
    {
        public static T AssertNotNull<T>(T? obj) where T : class
        {
            if (obj == null)
            {
                throw new NullReferenceException();
            }
            return obj;
        }

        public static Dictionary<string, T> RemoveNullEntries<T>(Dictionary<string, T?> dict) where T : class
        {
            return (from kv in dict where kv.Value != null select kv).ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        public static string ToHexString(this IEnumerable<byte> bytes, bool upperCase = true)
        {
            return String.Join("", from c in bytes select ((int)c).ToString(upperCase ? "X2" : "x2"));
        }
    }
}