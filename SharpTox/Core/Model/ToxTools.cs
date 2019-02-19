using System;
using System.Linq;
using System.Text;

namespace SharpTox.Core
{
    /// <summary>
    /// A set of helper functions for Tox.
    /// </summary>
    public static class ToxTools
    {
        public static string HexBinToString(byte[] bytes)
            => string.Join("", bytes.Select(x => x.ToString("X2")));

        public static byte[] StringToHexBin(string hexString)
        {
            byte[] bin = new byte[hexString.Length / 2];

            for (int i = 0; i < bin.Length; i++)
            {
                bin[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            return bin;
        }

        public static DateTime EpochToDateTime(ulong epoch)
            => new DateTime(1970, 1, 1).AddSeconds(Convert.ToDouble(epoch));
    }
}
