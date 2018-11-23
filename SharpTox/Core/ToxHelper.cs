using SharpTox.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharpTox
{
    static class ToxHelper
    {
        public static TValue[] Get<TValue>(ToxHandle tox, Action<ToxHandle, TValue[]> fill, uint size)
        {
            var values = new TValue[size];
            fill(tox, values);
            return values;
        }

        public static TValue[] Get<TValue>(ToxHandle tox, Func<ToxHandle, uint> size, Action<ToxHandle, TValue[]> fill)
        {
            var values = new TValue[size(tox)];
            fill(tox, values);
            return values;
        }

        public static string GetString(ToxHandle tox, Func<ToxHandle, uint> size, Action<ToxHandle, byte[]> fill)
        {
            var bytes = new byte[size(tox)];
            fill(tox, bytes);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
