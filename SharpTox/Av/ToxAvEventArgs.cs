using System;
using SharpTox.Core;

namespace SharpTox.Av
{
    public class ToxAvEventArgs
    {
        public abstract class CallBaseEventArgs : EventArgs
        {
            public uint FriendNumber { get; }

            protected CallBaseEventArgs(uint friendNumber)
            {
                this.FriendNumber = friendNumber;
            }
        }

        public class CallRequestEventArgs : CallBaseEventArgs
        {
            public ToxAvFriendCallState State { get; }

            public bool AudioEnabled { get; }

            public bool VideoEnabled { get; }

            public CallRequestEventArgs(uint friendNumber, bool audioEnabled, bool videoEnabled)
                : base(friendNumber)
            {
                this.AudioEnabled = audioEnabled;
                this.VideoEnabled = videoEnabled;
            }
        }

        public class CallStateEventArgs : CallBaseEventArgs
        {
            public ToxAvFriendCallState State { get; }

            public CallStateEventArgs(uint friendNumber, ToxAvFriendCallState state) : base(friendNumber)
            {
                this.State = state;
            }
        }

        public sealed class AudioBitrateEventArgs : CallBaseEventArgs
        {
            public uint Bitrate { get; }

            public AudioBitrateEventArgs(uint friendNumber, uint bitrate)
                : base(friendNumber)
            {
                this.Bitrate = bitrate;
            }
        }

        public sealed class VideoBitrateEventArgs : CallBaseEventArgs
        {
            public uint Bitrate { get; }

            public VideoBitrateEventArgs(uint friendNumber, uint bitrate)
                : base(friendNumber)
            {
                this.Bitrate = bitrate;
            }
        }

        public class AudioFrameEventArgs : CallBaseEventArgs
        {
            public ToxAvAudioFrame Frame { get; }

            public AudioFrameEventArgs(uint friendNumber, ToxAvAudioFrame frame)
                : base(friendNumber)
            {
                this.Frame = frame;
            }
        }

        public class VideoFrameEventArgs : CallBaseEventArgs
        {
            public ToxAvVideoFrame Frame { get; }

            public VideoFrameEventArgs(uint friendNumber, ToxAvVideoFrame frame)
                : base(friendNumber)
            {
                this.Frame = frame;
            }
        }

        public class GroupAudioDataEventArgs : EventArgs
        {
            public uint GroupNumber { get; }

            public uint PeerNumber { get; }

            public short[] Data { get; }

            public byte Channels { get; }

            public uint SampleRate { get; }

            public GroupAudioDataEventArgs(uint groupNumber, uint peerNumber, short[] data, byte channels, uint sampleRate)
            {
                this.GroupNumber = groupNumber;
                this.PeerNumber = peerNumber;
                this.Data = data;
                this.Channels = channels;
                this.SampleRate = sampleRate;
            }
        }
    }
}
