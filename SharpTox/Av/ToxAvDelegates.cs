using System;
using System.Runtime.InteropServices;

namespace SharpTox.Av
{
    static class ToxAvDelegates
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CallDelegate(IntPtr toxAv, UInt32 friendNumber, Boolean audioEnabled, Boolean videoEnabled, IntPtr userData);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CallStateDelegate(IntPtr toxAv, UInt32 friendNumber, ToxAvFriendCallState state, IntPtr userData);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void AudioBitRateDelegate(IntPtr toxAv, UInt32 friendNumber, UInt32 bitrate, IntPtr userData);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void VideoBitRateDelegate(IntPtr toxAv, UInt32 friendNumber, UInt32 bitrate, IntPtr userData);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        // pcm: Array of audio samples (sampleCount * channels = length)
        public delegate void AudioReceiveFrameDelegate(IntPtr toxAv, UInt32 friendNumber, IntPtr pcm, UInt32 sampleCount, Byte channels, UInt32 samplingRate, IntPtr userData);

        // y: Luminosity plane. Size = MAX(width, abs(ystride)) * height.
        // u: U chroma plane. Size = MAX(width/2, abs(ustride)) * (height/2).
        // v: V chroma plane. Size = MAX(width/2, abs(vstride)) * (height/2).
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void VideoReceiveFrameDelegate(IntPtr toxAv, UInt32 friendNumber, UInt16 width, UInt16 height, IntPtr y, IntPtr u, IntPtr v, Int32 yStride, Int32 uStride, Int32 vStride, IntPtr userData);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GroupAudioReceiveDelegate(IntPtr tox, UInt32 groupNumber, UInt32 peerNumber, IntPtr frame, UInt32 sampleCount, Byte channels, UInt32 sampleRate, IntPtr userData);
    }
}
