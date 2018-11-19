using System;
using System.Runtime.InteropServices;

namespace SharpTox.Core
{
    public sealed class ToxOptionsN
    {
        private readonly ToxOptionsHandle options;

        public ToxOptionsN()
        {
            var err = ToxErrorOptionsNew.Ok;
            this.options = ToxFunctions.Options.New(ref err);

            if (err != ToxErrorOptionsNew.Ok)
            {
                throw new InvalidOperationException();
            }
        }

        public bool Ipv6Enabled {
            get => ToxFunctions.Options.GetIpv6Enabled(this.options);
            set => ToxFunctions.Options.SetIpv6Enabled(this.options, value);
        }

        public bool UdpEnabled {
            get => ToxFunctions.Options.GetUdpEnabled(this.options);
            set => ToxFunctions.Options.SetUdpEnabled(this.options, value);
        }

        public bool LocalDiscoveryEnabled {
            get => ToxFunctions.Options.GetLocalDiscoveryEnabled(this.options);
            set => ToxFunctions.Options.SetLocalDiscoveryEnabled(this.options, value);
        }

        public ToxProxyType ProxyType {
            get => ToxFunctions.Options.GetProxyType(this.options);
            set => ToxFunctions.Options.SetProxyType(this.options, value);
        }

        public string ProxyHost {
            get => ToxFunctions.Options.GetProxyHost(this.options);
            set => ToxFunctions.Options.SetProxyHost(this.options, value);
        }

        public ushort ProxyPort {
            get => ToxFunctions.Options.GetProxyPort(this.options);
            set => ToxFunctions.Options.SetProxyPort(this.options, value);
        }

        public ushort StartPort {
            get => ToxFunctions.Options.GetStartPort(this.options);
            set => ToxFunctions.Options.SetStartPort(this.options, value);
        }

        public ushort EndPort {
            get => ToxFunctions.Options.GetEndPort(this.options);
            set => ToxFunctions.Options.SetEndPort(this.options, value);
        }

        public ushort TcpPort {
            get => ToxFunctions.Options.GetTcpPort(this.options);
            set => ToxFunctions.Options.SetTcpPort(this.options, value);
        }

        public bool HolePunchingEnabled {
            get => ToxFunctions.Options.GetHolePunchingEnabled(this.options);
            set => ToxFunctions.Options.SetHolePunchingEnabled(this.options, value);
        }

        internal ToxSavedataType SavedataType {
            get => ToxFunctions.Options.GetSavedataType(this.options);
        }

        internal byte[] Savedata {
            get {
                var bytes = new byte[ToxFunctions.Options.GetSavedataLength(this.options)];
                var ptr = ToxFunctions.Options.GetSavedataData(this.options);
                Marshal.Copy(ptr, bytes, 0, bytes.Length);
                return bytes;
            }
        }

        /// <summary>
        /// Apply the default options to the ToxOptions
        /// </summary>
        public void ApplyDefault()
            => ToxFunctions.Options.Default(this.options);

        internal void SetData(byte[] data, ToxSavedataType type)
        {
            if(data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (type == ToxSavedataType.SecretKey && data.Length != ToxConstants.SecretKeySize)
                throw new ArgumentException("Data must have a length of ToxConstants.SecretKeySize bytes", nameof(data));

            ToxFunctions.Options.SetSavedataType(this.options, type);

            var ptr = Marshal.AllocHGlobal(data.Length);
            Marshal.Copy(data, 0, ptr, data.Length);
            ToxFunctions.Options.SetSavedataData(this.options, ptr, (uint)data.Length);
        }
    }
    /// <summary>
    /// Represents settings to be used by an instance of tox.
    /// </summary>
    public sealed class ToxOptions
    {
        /// <summary>
        /// Default Tox Options.
        /// </summary>
        public static ToxOptions Default { get { return new ToxOptions(ToxOptionsStruct.Default); } }

        /// <summary>
        /// Whether or not IPv6 should be enabled.
        /// </summary>
        public bool Ipv6Enabled {
            get { return _options.Ipv6Enabled; }
            set { _options.Ipv6Enabled = value; }
        }

        /// <summary>
        /// Whether or not UDP should be enabled.
        /// </summary>
        public bool UdpEnabled {
            get { return _options.UdpEnabled; }
            set { _options.UdpEnabled = value; }
        }

        /// <summary>
        /// Proxy type.
        /// </summary>
        public ToxProxyType ProxyType {
            get { return _options.ProxyType; }
            set { _options.ProxyType = value; }
        }

        /// <summary>
        /// Proxy ip or domain.
        /// </summary>
        public string ProxyHost {
            get { return _options.ProxyHost; }
            set { _options.ProxyHost = value; }
        }

        /// <summary>
        /// Proxy port, in host byte order.
        /// Underlying type is ushort, don't exceed ushort.MaxValue.
        /// </summary>
        public int ProxyPort {
            get { return _options.ProxyPort; }
            set { _options.ProxyPort = (ushort)value; }
        }

        /// <summary>
        /// The start port of the inclusive port range to attempt to use.
        /// Underlying type is ushort, don't exceed ushort.MaxValue.
        /// </summary>
        public int StartPort {
            get { return _options.StartPort; }
            set { _options.StartPort = (ushort)value; }
        }

        /// <summary>
        /// The end port of the inclusive port range to attempt to use.
        /// Underlying type is ushort, don't exceed ushort.MaxValue.
        /// </summary>
        public int EndPort {
            get { return _options.EndPort; }
            set { _options.EndPort = (ushort)value; }
        }

        /// <summary>
        /// The port to use for a TCP server. This can be disabled by assigning 0.
        /// Underlying type is ushort, don't exceed ushort.MaxValue.
        /// </summary>
        public int TcpPort {
            get { return _options.TcpPort; }
            set { _options.TcpPort = (ushort)value; }
        }

        private ToxOptionsStruct _options;
        internal ToxOptionsStruct Struct { get { return _options; } }

        internal ToxOptions(ToxOptionsStruct options)
        {
            _options = options;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToxOptions"/> struct.
        /// </summary>
        /// <param name="ipv6Enabled">Whether or not IPv6 should be enabled.</param>
        /// <param name="udpEnabled">Whether or not UDP should be enabled.</param>
        public ToxOptions(bool ipv6Enabled, bool udpEnabled)
        {
            _options = new ToxOptionsStruct();
            _options.Ipv6Enabled = ipv6Enabled;
            _options.UdpEnabled = udpEnabled;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToxOptions"/> struct.
        /// </summary>
        /// <param name="ipv6Enabled">Whether or not IPv6 should be enabled.</param>
        /// <param name="type">The type of proxy we want to connect to.</param>
        /// <param name="proxyAddress">The IP address or DNS name of the proxy to be used.</param>
        /// <param name="proxyPort">The port to use to connect to the proxy.</param>
        public ToxOptions(bool ipv6Enabled, ToxProxyType type, string proxyAddress, int proxyPort)
        {
            if (string.IsNullOrEmpty(proxyAddress))
                throw new ArgumentNullException("proxyAddress");

            if (proxyAddress.Length > 255)
                throw new Exception("Parameter proxyAddress is too long.");

            _options = new ToxOptionsStruct();
            _options.Ipv6Enabled = ipv6Enabled;
            _options.UdpEnabled = false;
            _options.ProxyType = type;
            _options.ProxyHost = proxyAddress;
            _options.ProxyPort = (ushort)proxyPort;
        }

        public override bool Equals(object obj)
        {
            if (obj is ToxOptions other)
            {
                return this.Struct.Equals(other.Struct);
            }

            return false;
        }

        public override int GetHashCode() => base.GetHashCode();

        public static bool operator ==(ToxOptions options1, ToxOptions options2)
        {
            return options1.Struct.Equals(options2.Struct);
        }

        public static bool operator !=(ToxOptions options1, ToxOptions options2)
        {
            return !(options1 == options2);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    [Obsolete(DeprecatedMessage, false)]
    internal struct ToxOptionsStruct
    {
        const string DeprecatedMessage = @"The memory layout of this struct (size, alignment, and field
order) is not part of the ABI. To remain compatible, prefer to use tox_options_new to
allocate the object and accessor functions to set the members. The struct
will become opaque (i.e. the definition will become private) in v0.3.0.";

        public static ToxOptionsStruct Default {
            get {
                ToxOptionsStruct options = new ToxOptionsStruct();
                ToxFunctions.OptionsDefault(ref options);
                return options;
            }
        }

        public void SetData(byte[] data, ToxSavedataType type)
        {
            if (type == ToxSavedataType.SecretKey && data.Length != ToxConstants.SecretKeySize)
                throw new ArgumentException("Data must have a length of ToxConstants.SecretKeySize bytes", "data");

            SaveDataType = type;
            SaveDataLength = (uint)data.Length;
            SaveData = Marshal.AllocHGlobal(data.Length);

            Marshal.Copy(data, 0, SaveData, data.Length);
        }

        public void Free()
        {
            if (SaveData != IntPtr.Zero)
                Marshal.FreeHGlobal(SaveData);
        }

        [MarshalAs(UnmanagedType.I1)]
        public Boolean Ipv6Enabled;

        [MarshalAs(UnmanagedType.I1)]
        public Boolean UdpEnabled;

        [MarshalAs(UnmanagedType.I1)]
        public Boolean LocalDiscoveryEnabled;

        public ToxProxyType ProxyType;

        [MarshalAs(UnmanagedType.LPStr)]
        public String ProxyHost;

        public UInt16 ProxyPort;
        public UInt16 StartPort;
        public UInt16 EndPort;
        public UInt16 TcpPort;

        [MarshalAs(UnmanagedType.I1)]
        public Boolean HolePunchingEnabled;

        public ToxSavedataType SaveDataType;
        public IntPtr SaveData;
        public UInt32 SaveDataLength;
    }
}
