using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpTox.Core
{
    public class ToxEventArgs
    {
        #region Base Classes
        public abstract class FriendBaseEventArgs : EventArgs
        {
            public uint FriendNumber { get; private set; }

            protected FriendBaseEventArgs(uint friendNumber)
            {
                FriendNumber = friendNumber;
            }
        }

        public abstract class ConferenceBaseEventArgs : EventArgs
        {
            public uint ConferenceNumber { get; }

            public uint PeerNumber { get; }

            protected ConferenceBaseEventArgs(uint conferenceNumber, uint peerNumber)
            {
                this.ConferenceNumber = conferenceNumber;
                this.PeerNumber = peerNumber;
            }
        }

        public abstract class FileBaseEventArgs : FriendBaseEventArgs
        {
            public uint FileNumber { get; private set; }

            protected FileBaseEventArgs(uint friendNumber, uint fileNumber)
                : base(friendNumber)
            {
                FileNumber = fileNumber;
            }
        }
        #endregion

        public class FriendRequestEventArgs : EventArgs
        {
            public ToxKey PublicKey { get; private set; }
            public string Message { get; private set; }

            public FriendRequestEventArgs(ToxKey publicKey, string message)
            {
                PublicKey = publicKey;
                Message = message;
            }
        }

        public class ConnectionStatusEventArgs : EventArgs
        {
            public ToxConnectionStatus Status { get; private set; }

            public ConnectionStatusEventArgs(ToxConnectionStatus status)
            {
                Status = status;
            }
        }

        public class FriendConnectionStatusEventArgs : FriendBaseEventArgs
        {
            public ToxConnectionStatus Status { get; private set; }

            public FriendConnectionStatusEventArgs(uint friendNumber, ToxConnectionStatus status)
                : base(friendNumber)
            {
                Status = status;
            }
        }

        public class FriendMessageEventArgs : FriendBaseEventArgs
        {
            public string Message { get; private set; }

            public ToxMessageType MessageType { get; private set; }

            public FriendMessageEventArgs(uint friendNumber, string message, ToxMessageType type)
                : base(friendNumber)
            {
                Message = message;
                MessageType = type;
            }
        }

        public class NameChangeEventArgs : FriendBaseEventArgs
        {
            public string Name { get; private set; }

            public NameChangeEventArgs(uint friendNumber, string name)
                : base(friendNumber)
            {
                Name = name;
            }
        }

        public class StatusMessageEventArgs : FriendBaseEventArgs
        {
            public string StatusMessage { get; private set; }

            public StatusMessageEventArgs(uint friendNumber, string statusMessage)
                : base(friendNumber)
            {
                StatusMessage = statusMessage;
            }
        }

        public class StatusEventArgs : FriendBaseEventArgs
        {
            public ToxUserStatus Status { get; private set; }

            public StatusEventArgs(uint friendNumber, ToxUserStatus status)
                : base(friendNumber)
            {
                Status = status;
            }
        }

        public class TypingStatusEventArgs : FriendBaseEventArgs
        {
            public bool IsTyping { get; private set; }

            public TypingStatusEventArgs(uint friendNumber, bool isTyping)
                : base(friendNumber)
            {
                IsTyping = isTyping;
            }
        }

        public class FileControlEventArgs : FileBaseEventArgs
        {
            public ToxFileControl Control { get; private set; }

            public FileControlEventArgs(uint friendNumber, uint fileNumber, ToxFileControl control)
                : base(friendNumber, fileNumber)
            {
                Control = control;
            }
        }

        public class FileRequestChunkEventArgs : FileBaseEventArgs
        {
            public ulong Position { get; set; }

            public uint Length { get; set; }

            public FileRequestChunkEventArgs(uint friendNumber, uint fileNumber, ulong position, uint length)
                : base(friendNumber, fileNumber)
            {
                this.Position = position;
                this.Length = length;
            }
        }

        public class FriendPacketEventArgs : FriendBaseEventArgs
        {
            public byte[] Data { get; set; }

            public FriendPacketEventArgs(uint friendNumber, byte[] data)
                : base(friendNumber)
            {
                Data = data;
            }
        }

        public class FileChunkEventArgs : FileBaseEventArgs
        {
            public byte[] Data { get; private set; }

            public ulong Position { get; private set; }

            public FileChunkEventArgs(uint friendNumber, uint fileNumber, byte[] data, ulong position)
                : base(friendNumber, fileNumber)
            {
                this.Data = data;
                this.Position = position;
            }
        }

        public class FileSendRequestEventArgs : FileBaseEventArgs
        {
            public ulong FileSize { get; }

            public string FileName { get; }

            public ToxFileKind FileKind { get; }

            public FileSendRequestEventArgs(uint friendNumber, uint fileNumber, ToxFileKind kind, ulong fileSize, string fileName)
                : base(friendNumber, fileNumber)
            {
                this.FileSize = fileSize;
                this.FileName = fileName;
                this.FileKind = kind;
            }
        }

        public class ReadReceiptEventArgs : FriendBaseEventArgs
        {
            public uint Receipt { get; }

            public ReadReceiptEventArgs(uint friendNumber, uint receipt)
                : base(friendNumber)
            {
                Receipt = receipt;
            }
        }

        public class CustomPacketEventArgs : FriendBaseEventArgs
        {
            public byte[] Packet { get; }

            public CustomPacketEventArgs(uint friendNumber, byte[] packet)
                : base(friendNumber)
            {
                if (packet == null)
                {
                    throw new ArgumentNullException(nameof(packet));
                }

                Packet = (byte[])packet.Clone();
            }
        }

        public class ConferenceInviteEventArgs : FriendBaseEventArgs
        {
            public byte[] Data { get; }

            public ToxConferenceType ConferenceType { get; }

            public ConferenceInviteEventArgs(uint friendNumber, ToxConferenceType type, byte[] data)
                : base(friendNumber)
            {
                if (data == null)
                {
                    throw new ArgumentNullException(nameof(data));
                }

                this.Data = (byte[])data.Clone();
                this.ConferenceType = type;
            }
        }

        public class ConferenceMessageEventArgs : ConferenceBaseEventArgs
        {
            public string Message { get; }

            public ToxMessageType Type { get; }

            public ConferenceMessageEventArgs(uint conferenceNumber, uint peerNumber, string message, ToxMessageType type)
                : base(conferenceNumber, peerNumber)
            {
                this.Message = message;
                this.Type = type;
            }
        }

        public class ConferenceTitleEventArgs : ConferenceBaseEventArgs
        {
            public string Title { get; }

            public ConferenceTitleEventArgs(uint conferenceNumber, uint peerNumber, string title) : base(conferenceNumber, peerNumber)
                => this.Title = title;
        }

        public class ConferencePeerNameEventArgs : ConferenceBaseEventArgs
        {
            public string Name { get; }

            public ConferencePeerNameEventArgs(uint conferenceNumber, uint peerNumber, string name) : base(conferenceNumber, peerNumber)
            {
                this.Name = name;
            }
        }

        public class ConferenceConnectedEventArgs : EventArgs
        {
            public uint ConferenceNumber { get; }

            public ConferenceConnectedEventArgs(uint conferenceNumber)
                => this.ConferenceNumber = conferenceNumber;
        }

        public class ConferencePeerListEventArgs : EventArgs
        {
            public uint ConferenceNumber { get; }

            public ConferencePeerListEventArgs(uint conferenceNumber)
                => this.ConferenceNumber = conferenceNumber;
        }
    }
}
