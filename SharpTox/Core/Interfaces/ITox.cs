using System;
using SharpTox.Encryption;

namespace SharpTox.Core.Interfaces
{
    public interface ITox : IDisposable
    {
        ToxKey DhtId { get; }

        ToxId Id { get; }

        string Name { get; set; }

        ToxUserStatus Status { get; set; }

        string StatusMessage { get; set; }

        uint Nospam { get; set; }

        IToxFriend Friends { get; }

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
        event EventHandler<ToxEventArgs.ReadReceiptEventArgs> OnReadReceiptReceived;

        bool AddTcpRelay(ToxNode node, out ToxErrorBootstrap error);
        bool Bootstrap(ToxNode node, out ToxErrorBootstrap error);
        bool DeleteConference(uint conferenceNumber, out ToxErrorConferenceDelete error);
        uint GetConferencePeerCount(uint conferenceNumber, out ToxErrorConferencePeerQuery error);
        string GetConferencePeerName(uint conferenceNumber, uint peerNumber, out ToxErrorConferencePeerQuery error);
        ToxKey GetConferencePeerPublicKey(uint conferenceNumber, uint peerNumber, out ToxErrorConferencePeerQuery error);
        string GetConferenceTitle(uint conferenceNumber, out ToxErrorConferenceTitle error);
        ToxConferenceType GetConferenceType(uint conferenceNumber, out ToxErrorConferenceGetType err);
        IToxData GetData();
        IToxData GetData(string password, out ToxErrorEncryption error);
        IToxData GetData(ToxEncryptionKey key, out ToxErrorEncryption error);
        ToxKey GetSecretKey();
        ushort GetTcpPort(out ToxErrorGetPort error);
        ushort GetUdpPort(out ToxErrorGetPort error);
        bool InviteFriend(uint friendNumber, uint conferenceNumber, out ToxErrorConferenceInvite error);
        TimeSpan Iterate();
        uint JoinConference(uint friendNumber, byte[] cookie, out ToxErrorConferenceJoin error);
        uint NewConference(out ToxErrorConferenceNew error);
        bool PeerNumberIsOurs(uint conferenceNumber, uint peerNumber, out ToxErrorConferencePeerQuery error);
        bool SendConferenceMessage(uint conferenceNumber, ToxMessageType type, string message, out ToxErrorConferenceSendMessage error);
        bool SetConferenceTitle(uint conferenceNumber, string title, out ToxErrorConferenceTitle error);
        bool ValidMessage([NotNull] string message);
    }
}