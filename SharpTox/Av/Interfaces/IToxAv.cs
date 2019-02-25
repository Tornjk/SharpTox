using System;

namespace SharpTox.Av.Interfaces
{
    public interface IToxAv : IDisposable
    {
        event EventHandler<ToxAvEventArgs.AudioBitrateEventArgs> OnAudioBitrateChanged;
        event EventHandler<ToxAvEventArgs.AudioFrameEventArgs> OnAudioFrameReceived;
        event EventHandler<ToxAvEventArgs.CallRequestEventArgs> OnCallRequestReceived;
        event EventHandler<ToxAvEventArgs.CallStateEventArgs> OnCallStateChanged;
        event EventHandler<ToxAvEventArgs.GroupAudioDataEventArgs> OnReceivedGroupAudio;
        event EventHandler<ToxAvEventArgs.VideoBitrateEventArgs> OnVideoBitrateChanged;
        event EventHandler<ToxAvEventArgs.VideoFrameEventArgs> OnVideoFrameReceived;

        int AddAvGroupchat();
        bool Answer(uint friendNumber, uint audioBitrate, uint videoBitrate, out ToxAvErrorAnswer error);
        bool Call(uint friendNumber, uint audioBitrate, uint videoBitrate, out ToxAvErrorCall error);
        bool GroupSendAudio(uint groupNumber, short[] pcm, uint perframe, byte channels, uint sampleRate);
        TimeSpan Iterate();
        int JoinAvGroupchat(uint friendNumber, byte[] data);
        bool SendAudioFrame(uint friendNumber, ToxAvAudioFrame frame, out ToxAvErrorSendFrame error);
        bool SendControl(uint friendNumber, ToxAvCallControl control, out ToxAvErrorCallControl error);
        bool SendVideoFrame(uint friendNumber, ToxAvVideoFrame frame, out ToxAvErrorSendFrame error);
        bool SetAudioBitrate(uint friendNumber, uint bitrate, out ToxAvErrorBitRateSet error);
        bool SetVideoBitrate(uint friendNumber, uint bitrate, out ToxAvErrorBitRateSet error);
    }
}