﻿#pragma warning disable 1591

namespace SharpTox
{
    public enum ToxAvCallbackID
    {
        /* Requests */
        OnInvite,
        OnStart,
        OnCancel,
        OnReject,
        OnEnd,

        /* Responses */
        OnRinging,
        OnStarting,
        OnEnding,

        /* Protocol */
        OnError,
        OnRequestTimeout,
        OnPeerTimeout
    }

    public enum ToxAvCallType
    {
        Audio = 70,
        Video
    }

    public enum ToxAvError
    {
        None = 0,
        Internal = -1,
        AlreadyInCall = -2,
        NoCall = -3,
        InvalidState = -4,
        NoRtpSession = -5,
        AudioPacketLost = -6,
        StartingAudioRtp = -7,
        StartingVideoRtp = -8,
        TerminatingAudioRtp = -9,
        TerminatingVideoRtp = -10,
    }

    public enum ToxAvCapabilities
    {
        None,
        AudioEncoding = 1 << 0,
        AudioDecoding = 1 << 1,
        VideoEncoding = 1 << 2,
        VideoDecoding = 1 << 3
    }
}

#pragma warning restore 1591