using System;
using System.Collections.Generic;
using System.Text;

namespace SharpTox.Core.Interfaces
{
    public interface IToxFriend
    {
        uint[] Friends { get; }

        event EventHandler<ToxEventArgs.FriendConnectionStatusEventArgs> OnFriendConnectionStatusChanged;
        event EventHandler<ToxEventArgs.FriendPacketEventArgs> OnFriendLosslessPacketReceived;
        event EventHandler<ToxEventArgs.FriendPacketEventArgs> OnFriendLossyPacketReceived;
        event EventHandler<ToxEventArgs.FriendMessageEventArgs> OnFriendMessageReceived;
        event EventHandler<ToxEventArgs.NameChangeEventArgs> OnFriendNameChanged;
        event EventHandler<ToxEventArgs.FriendRequestEventArgs> OnFriendRequestReceived;
        event EventHandler<ToxEventArgs.StatusEventArgs> OnFriendStatusChanged;
        event EventHandler<ToxEventArgs.StatusMessageEventArgs> OnFriendStatusMessageChanged;
        event EventHandler<ToxEventArgs.TypingStatusEventArgs> OnFriendTypingChanged;

        uint SendMessage(uint friendNumber, string message, ToxMessageType type, out ToxErrorSendMessage error);
        bool SetTypingStatus(uint friendNumber, bool isTyping, out ToxErrorSetTyping error);
        uint AddFriend(ToxId id, string message, out ToxErrorFriendAdd error);
        uint AddFriendNoRequest(ToxKey publicKey, out ToxErrorFriendAdd error);
        bool DeleteFriend(uint friendNumber, out ToxErrorFriendDelete error);
        bool FileControl(uint friendNumber, uint fileNumber, ToxFileControl control, out ToxErrorFileControl error);
        byte[] FileGetId(uint friendNumber, uint fileNumber, out ToxErrorFileGet error);
        bool FileSeek(uint friendNumber, uint fileNumber, long position, out ToxErrorFileSeek error);
        IToxFileInfo FileSend(uint friendNumber, ToxFileKind kind, long fileSize, string fileName, byte[] fileId, out ToxErrorFileSend error);
        IToxFileInfo FileSend(uint friendNumber, ToxFileKind kind, long fileSize, string fileName, out ToxErrorFileSend error);
        bool FileSendChunk(uint friendNumber, uint fileNumber, ulong position, byte[] data, out ToxErrorFileSendChunk error);
        bool FriendExists(uint friendNumber);
        bool FriendSendLosslessPacket(uint friendNumber, byte[] data, out ToxErrorFriendCustomPacket error);
        bool FriendSendLossyPacket(uint friendNumber, byte[] data, out ToxErrorFriendCustomPacket error);
        uint GetFriendByPublicKey(ToxKey publicKey, out ToxErrorFriendByPublicKey error);
        DateTime GetFriendLastOnline(uint friendNumber);
        DateTime GetFriendLastOnline(uint friendNumber, out ToxErrorFriendGetLastOnline error);
        string GetFriendName(uint friendNumber, out ToxErrorFriendQuery error);
        ToxKey GetFriendPublicKey(uint friendNumber, out ToxErrorFriendGetPublicKey error);
        string GetFriendStatusMessage(uint friendNumber, out ToxErrorFriendQuery error);
    }
}
