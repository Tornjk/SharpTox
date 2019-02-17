namespace SharpTox.Core.Interfaces
{
    public interface IToxOptions
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
    }

    public interface IToxOptionsSavedata
    {
        ToxSavedataType Type { get; }
        byte[] Data { get; }
    }
}