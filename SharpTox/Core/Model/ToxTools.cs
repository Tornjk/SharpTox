using System;
using System.Text;

namespace SharpTox.Core
{
    /// <summary>
    /// A set of helper functions for Tox.
    /// </summary>
    public static class ToxTools
    {
        public static string HexBinToString(byte[] b)
        {
            var sb = new StringBuilder(2 * b.Length);

            for (int i = 0; i < b.Length; i++)
                sb.AppendFormat("{0:X2}", b[i]);

            return sb.ToString();
        }

        public static byte[] StringToHexBin(string s)
        {
            byte[] bin = new byte[s.Length / 2];

            for (int i = 0; i < bin.Length; i++)
                bin[i] = Convert.ToByte(s.Substring(i * 2, 2), 16);

            return bin;
        }

        public static DateTime EpochToDateTime(ulong epoch)
            => new DateTime(1970, 1, 1).AddSeconds(Convert.ToDouble(epoch));
    }
}
