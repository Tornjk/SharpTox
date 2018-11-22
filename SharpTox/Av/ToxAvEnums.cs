using System;
namespace SharpTox.Av
{
    public enum ToxAvErrorNew
    {
        Ok,
        Null,
        Malloc,
        Multiple
    }

    public enum ToxAvErrorCall
    {
        Ok,
        Malloc,
        Sync,
        FriendNotFound,
        FriendNotConnected,
        FriendAlreadyInCall,
        InvalidBitRate
    }

    public enum ToxAvErrorAnswer
    {
        Ok,
        Sync,
        CodecInitialization,
        FriendNotFound,
        FriendNotCalling,
        InvalidBitRate
    }

    [Flags]
    public enum ToxAvFriendCallState
    {
        Paused = 0,
        Error = 1,
        Finished = 2,
        SendingAudio = 4,
        SendingVideo = 8,
        ReceivingAudio = 16,
        ReceivingVideo = 32,
    }

    public enum ToxAvCallControl
    {
        Resume,
        Pause,
        Cancel,
        MuteAudio,
        UnmuteAudio,
        HideVideo,
        ShowVideo
    }

    public enum ToxAvErrorCallControl
    {
        Ok,
        Sync,
        FriendNotFound,
        FriendNotInCall,
        InvalidTransition
    }

    public enum ToxAvErrorBitRateSet
    {
        Ok,
        Sync,
        InvalidBitrate,
        FriendNotFound,
        FriendNotInCall
    }

    public enum ToxAvErrorSendFrame
    {
        Ok,
        Null,
        FrienNotFound,
        FriendNotInCall,
        Sync,
        Invalid,
        PayloadTypeDisabled,
        RtpFailed
    }
}
