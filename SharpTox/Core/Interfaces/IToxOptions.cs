using System;

namespace SharpTox.Core.Interfaces
{
    public interface IToxOptions : IDisposable
    {
        ushort EndPort { get; set; }
        bool HolePunchingEnabled { get; set; }
        bool Ipv6Enabled { get; set; }
        bool LocalDiscoveryEnabled { get; set; }
        string ProxyHost { get; set; }
        ushort ProxyPort { get; set; }
        ToxProxyType ProxyType { get; set; }
        ushort StartPort { get; set; }
        ushort TcpPort { get; set; }
        bool UdpEnabled { get; set; }

        IToxOptionsSavedata GetSaveData();

        /// <summary>
        /// Initializes a new instance of Tox. If no secret key is specified, tox will generate a new keypair.
        /// </summary>
        ITox Create();

        /// <summary>
        /// Initializes a new instance of Tox with toxdata.
        /// </summary>
        ITox Restore(IToxData data);

        /// <summary>
        /// Initializes a new instance of Tox with a secretKey.
        /// </summary>
        ITox Restore(ToxKey secretKey);
    }

    public interface IToxOptionsSavedata
    {
        ToxSavedataType Type { get; }
        byte[] Data { get; }
    }
}