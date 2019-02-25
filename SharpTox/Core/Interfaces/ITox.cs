using System;
using SharpTox.Av.Interfaces;
using SharpTox.Encryption;

namespace SharpTox.Core.Interfaces
{
    public interface ITox : IDisposable
    {
        ToxKey DhtId { get; }

        uint[] Friends { get; }

        ToxId Id { get; }

        string Name { get; set; }

        ToxUserStatus Status { get; set; }

        string StatusMessage { get; set; }

        uint Nospam { get; set; }

        event EventHandler<ToxEventArgs.ConferencePeerListEventArgs> ConferencePeerListChanged;
        event EventHandler<ToxEventArgs.ConferencePeerNameEventArgs> ConferencePeerNameChanged;
        event EventHandler<ToxEventArgs.ConferenceConnectedEventArgs> OnConferenceConnected;
        event EventHandler<ToxEventArgs.ConferenceInviteEventArgs> OnConferenceInvite;
        event EventHandler<ToxEventArgs.ConferenceMessageEventArgs> OnConferenceMessage;
        event EventHandler<ToxEventArgs.ConferenceTitleEventArgs> OnConferenceTitleChanged;
        event EventHandler<ToxEventArgs.ConnectionStatusEventArgs> OnConnectionStatusChanged;
        event EventHandler<ToxEventArgs.FileChunkEventArgs> OnFileChunkReceived;
        event EventHandler<ToxEventArgs.FileRequestChunkEventArgs> OnFileChunkRequested;
        event EventHandler<ToxEventArgs.FileControlEventArgs> OnFileControlReceived;
        event EventHandler<ToxEventArgs.FileSendRequestEventArgs> OnFileSendRequestReceived;
        event EventHandler<ToxEventArgs.FriendConnectionStatusEventArgs> OnFriendConnectionStatusChanged;
        event EventHandler<ToxEventArgs.FriendPacketEventArgs> OnFriendLosslessPacketReceived;
        event EventHandler<ToxEventArgs.FriendPacketEventArgs> OnFriendLossyPacketReceived;
        event EventHandler<ToxEventArgs.FriendMessageEventArgs> OnFriendMessageReceived;
        event EventHandler<ToxEventArgs.NameChangeEventArgs> OnFriendNameChanged;
        event EventHandler<ToxEventArgs.FriendRequestEventArgs> OnFriendRequestReceived;
        event EventHandler<ToxEventArgs.StatusEventArgs> OnFriendStatusChanged;
        event EventHandler<ToxEventArgs.StatusMessageEventArgs> OnFriendStatusMessageChanged;
        event EventHandler<ToxEventArgs.TypingStatusEventArgs> OnFriendTypingChanged;
        event EventHandler<ToxEventArgs.ReadReceiptEventArgs> OnReadReceiptReceived;

        IToxAv CreateAv();

        uint AddFriend(ToxId id, string message, out ToxErrorFriendAdd error);
        uint AddFriendNoRequest(ToxKey publicKey, out ToxErrorFriendAdd error);
        bool AddTcpRelay(ToxNode node, out ToxErrorBootstrap error);
        bool Bootstrap(ToxNode node, out ToxErrorBootstrap error);
        bool DeleteConference(uint conferenceNumber, out ToxErrorConferenceDelete error);
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
        uint GetConferencePeerCount(uint conferenceNumber, out ToxErrorConferencePeerQuery error);
        string GetConferencePeerName(uint conferenceNumber, uint peerNumber, out ToxErrorConferencePeerQuery error);
        ToxKey GetConferencePeerPublicKey(uint conferenceNumber, uint peerNumber, out ToxErrorConferencePeerQuery error);
        string GetConferenceTitle(uint conferenceNumber, out ToxErrorConferenceTitle error);
        ToxConferenceType GetConferenceType(uint conferenceNumber, out ToxErrorConferenceGetType err);
        IToxData GetData();
        IToxData GetData(string password, out ToxErrorEncryption error);
        IToxData GetData(ToxEncryptionKey key, out ToxErrorEncryption error);
        uint GetFriendByPublicKey(ToxKey publicKey, out ToxErrorFriendByPublicKey error);
        DateTime GetFriendLastOnline(uint friendNumber);
        DateTime GetFriendLastOnline(uint friendNumber, out ToxErrorFriendGetLastOnline error);
        string GetFriendName(uint friendNumber, out ToxErrorFriendQuery error);
        ToxKey GetFriendPublicKey(uint friendNumber, out ToxErrorFriendGetPublicKey error);
        string GetFriendStatusMessage(uint friendNumber, out ToxErrorFriendQuery error);
        ToxKey GetSecretKey();
        ushort GetTcpPort(out ToxErrorGetPort error);
        ushort GetUdpPort(out ToxErrorGetPort error);
        bool InviteFriend(uint friendNumber, uint conferenceNumber, out ToxErrorConferenceInvite error);
        TimeSpan Iterate();
        uint JoinConference(uint friendNumber, byte[] cookie, out ToxErrorConferenceJoin error);
        uint NewConference(out ToxErrorConferenceNew error);
        bool PeerNumberIsOurs(uint conferenceNumber, uint peerNumber, out ToxErrorConferencePeerQuery error);
        bool SendConferenceMessage(uint conferenceNumber, ToxMessageType type, string message, out ToxErrorConferenceSendMessage error);
        uint SendMessage(uint friendNumber, string message, ToxMessageType type, out ToxErrorSendMessage error);
        bool SetConferenceTitle(uint conferenceNumber, string title, out ToxErrorConferenceTitle error);
        bool SetTypingStatus(uint friendNumber, bool isTyping, out ToxErrorSetTyping error);
        bool ValidMessage([NotNull] string message);
    }
}