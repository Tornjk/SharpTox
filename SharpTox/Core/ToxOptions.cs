using System;
using System.Runtime.InteropServices;

namespace SharpTox.Core
{
    public sealed class ToxOptions : IDisposable
    {
        private readonly ToxOptionsHandle options;

        public ToxOptions()
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

        /// <summary>
        /// Apply the default options to the ToxOptions and overrides the current values.
        /// </summary>
        public void ApplyDefault()
            => ToxFunctions.Options.Default(this.options);

        public static ToxOptions Default()
        {
            var options = new ToxOptions();
            options.ApplyDefault();
            return options;
        }

        internal ToxHandle Create()
        {
            var err = ToxErrorNew.Ok;
            var tox = ToxFunctions.New(this.options, ref err);
            if (tox == null || tox.IsInvalid || err != ToxErrorNew.Ok)
            {
                throw new Exception("Could not create a new instance of tox, error: " + err.ToString());
            }

            return tox;
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

        internal void SetData(byte[] data, ToxSavedataType type)
        {
            if (data == null)
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

        public void Dispose()
            => this.options.Dispose();
    }
}