using SharpTox.Core;
using System;
using System.Runtime.InteropServices;
using SizeT = System.UInt32;

namespace SharpTox.Av
{
    static class ToxAvCallbacks
    {
        const string Base = "toxav_callback_";

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Base + "call")]
        public static extern void Call(ToxAvHandle toxAv, ToxAvDelegates.CallDelegate callback, IntPtr userData);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Base + "call_state")]
        public static extern void CallState(ToxAvHandle toxAv, ToxAvDelegates.CallStateDelegate callback, IntPtr userData);

        public static class Audio
        {
            const string Prefix = Base + "audio_";
            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Prefix + "bit_rate")]
            public static extern void BitRate(ToxAvHandle toxAv, ToxAvDelegates.AudioBitRateDelegate callback, IntPtr userData);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Prefix + "receive_frame")]
            public static extern void ReceiveFrame(ToxAvHandle toxAv, ToxAvDelegates.AudioReceiveFrameDelegate callback, IntPtr userData);
        }

        public static class Video
        {
            const string Prefix = Base + "video_";

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Prefix + "bit_rate")]
            public static extern void BitRate(ToxAvHandle toxAv, ToxAvDelegates.VideoBitRateDelegate callback, IntPtr userData);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Prefix + "receive_frame")]
            public static extern void ReceiveFrame(ToxAvHandle toxAv, ToxAvDelegates.VideoReceiveFrameDelegate callback, IntPtr userData);
        }
    }

    static class ToxAvFunctions
    {
        const string Base = "toxav_";

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Base + "new")]
        public static extern ToxAvHandle New(ToxHandle tox, ref ToxAvErrorNew error);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Base + "kill")]
        public static extern void Kill(IntPtr toxAv);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Base + "get_tox")]
        public static extern IntPtr GetTox(ToxAvHandle toxAv);

        public static class Version
        {
            const string Prefix = Base + "version_";

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Prefix + "major")]
            public static extern UInt32 Major();

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Prefix + "minor")]
            public static extern UInt32 Minor();

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Prefix + "patch")]
            public static extern UInt32 Patch();

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Prefix + "is_compatible")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern Boolean IsCompatible(UInt32 major, UInt32 minor, UInt32 patch);
        }

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Base + "iteration_interval")]
        public static extern UInt32 IterationInterval(ToxAvHandle toxAv);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Base + "iterate")]
        public static extern void Iterate(ToxAvHandle toxAv);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Base + "call")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern Boolean Call(ToxAvHandle toxAv, UInt32 friendNumber, UInt32 audioBitrate, UInt32 videoBitrate, ref ToxAvErrorCall error);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Base + "answer")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern Boolean Answer(ToxAvHandle toxAv, UInt32 friendNumber, UInt32 audioBitrate, UInt32 videoBitrate, ref ToxAvErrorAnswer error);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Base + "call_control")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern Boolean CallControl(ToxAvHandle toxAv, UInt32 friendNumber, ToxAvCallControl control, ref ToxAvErrorCallControl error);

        public static class Audio
        {
            const string Prefix = Base + "audio_";

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Prefix + "send_frame")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern Boolean SendFrame(ToxAvHandle toxAv, UInt32 friendNumber, Int16[] pcm, SizeT sampleCount, Byte channels, UInt32 samplingRate, ref ToxAvErrorSendFrame error);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Prefix + "set_bit_rate")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool SetBitrate(ToxAvHandle toxAv, UInt32 friendNumber, UInt32 bitrate, ref ToxAvErrorBitRateSet error);
        }

        public static class Video
        {
            const string Prefix = Base + "video_";

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Prefix + "send_frame")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern Boolean SendFrame(ToxAvHandle toxAv, UInt32 friendNumber, UInt16 width, UInt16 height, Byte[] y, Byte[] u, Byte[] v, ref ToxAvErrorSendFrame error);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Prefix + "set_bit_rate")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool SetBitrate(ToxAvHandle toxAv, UInt32 friendNumber, UInt32 bitrate, ref ToxAvErrorBitRateSet error);
        }

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "toxav_add_av_groupchat")]
        public static extern Int32 AddAvGroupchat(ToxHandle tox, ToxAvDelegates.GroupAudioReceiveDelegate callback, IntPtr userData);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "toxav_join_av_groupchat")]
        public static extern Int32 JoinAvGroupchat(ToxHandle tox, UInt32 friendNumber, Byte[] data, UInt16 length, ToxAvDelegates.GroupAudioReceiveDelegate callback, IntPtr userData);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "toxav_group_send_audio")]
        public static extern Int32 GroupSendAudio(ToxHandle tox, UInt32 groupNumber, Int16[] pcm, UInt32 sampleCount, Byte channels, UInt32 sampleRate);
    }

    sealed class ToxAvCallbackHandler<TEventArgs, TDelegate> : BaseToxCallbackHandler<ToxAvHandle, TEventArgs, TDelegate> where TEventArgs : EventArgs where TDelegate : class
    {
        public ToxAvCallbackHandler(Action<ToxAvHandle, TDelegate, IntPtr> register, Func<Action<TEventArgs>, TDelegate> create) : base((handle, del) => register(handle, del, IntPtr.Zero), create)
        {
        }

        private event EventHandler<TEventArgs> @event;

        public void Add(ToxAv tox, EventHandler<TEventArgs> handler)
        {
            if (this.tDelegate == null)
            {
                this.tDelegate = this.create(args => this.OnCallback(tox, args));
                this.register(tox.Handle, this.tDelegate);
            }

            this.@event += handler;
        }

        public void Remove(ToxAv tox, EventHandler<TEventArgs> handler)
        {
            if (this.@event.GetInvocationList().Length == 1)
            {
                this.register(tox.Handle, null);
                this.tDelegate = null;
            }

            this.@event -= handler;
        }

        private void OnCallback(ToxAv tox, TEventArgs args) => this.@event?.Invoke(tox, args);
    }
}
