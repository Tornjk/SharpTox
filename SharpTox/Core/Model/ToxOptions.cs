using System;
using System.Runtime.InteropServices;
using SharpTox.Core.Interfaces;

namespace SharpTox.Core
{
    public sealed class ToxOptions : IDisposable, IToxOptions
    {
        private readonly ToxOptionsHandle options;

        public ToxOptions(IToxOptionsSavedata savedata = null)
        {
            var err = ToxErrorOptionsNew.Ok;
            this.options = ToxFunctions.Options.New(ref err);

            if (err != ToxErrorOptionsNew.Ok)
            {
                throw new InvalidOperationException();
            }

            if (savedata != null)
            {
                this.SetSavedata(savedata.Data, savedata.Type);
            }
        }

        public ToxOptions(IToxOptions other) : this(other?.GetSaveData())
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            this.Ipv6Enabled = other.Ipv6Enabled;
            this.UdpEnabled = other.UdpEnabled;
            this.LocalDiscoveryEnabled = other.LocalDiscoveryEnabled;
            this.ProxyType = other.ProxyType;
            this.ProxyHost = other.ProxyHost;
            this.ProxyPort = other.ProxyPort;
            this.StartPort = other.StartPort;
            this.EndPort = other.EndPort;
            this.TcpPort = other.TcpPort;
            this.HolePunchingEnabled = other.HolePunchingEnabled;
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

        public static ToxOptions Default(byte[] data, ToxSavedataType type)
        {
            var options = new ToxOptions();
            options.ApplyDefault();
            options.SetSavedata(data, type);
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

        private void SetSavedata(byte[] data, ToxSavedataType type)
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

        void IDisposable.Dispose()
            => this.options.Dispose();

        public IToxOptionsSavedata GetSaveData()
        {
            var type = ToxFunctions.Options.GetSavedataType(this.options);
            var data = new byte[ToxFunctions.Options.GetSavedataLength(this.options)];
            {
                var ptr = ToxFunctions.Options.GetSavedataData(this.options);
                Marshal.Copy(ptr, data, 0, data.Length);
            }

            return new OptionsSavedata(type, data);
        }

        private class OptionsSavedata : IToxOptionsSavedata
        {
            public ToxSavedataType Type { get; }

            public byte[] Data { get; }

            public OptionsSavedata(ToxSavedataType type, byte[] savedata)
            {
                this.Type = type;
                this.Data = savedata;
            }
        }
    }
}