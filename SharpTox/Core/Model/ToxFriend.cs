using System;
using System.Collections.Generic;
using System.Text;
using SharpTox.Core.Interfaces;

namespace SharpTox.Core
{
    class ToxFriend : IToxFriend, IDisposable
    {
        private ToxHandle handle;
        private bool isDisposed;

        /// <summary>
        /// An array of friendnumbers of this Tox instance.
        /// </summary>
        public uint[] Friends => ToxHelper.GetArray<uint>(this.handle, ToxFunctions.Friend.GetFriendListSize, ToxFunctions.Friend.GetFriendList);

        public ToxFriend(ToxHandle handle)
        {
            this.handle = handle;
        }

        private readonly ToxCallbackHandler<ToxEventArgs.FriendRequestEventArgs, ToxDelegates.CallbackFriendRequestDelegate> friendRequest
            = new ToxCallbackHandler<ToxEventArgs.FriendRequestEventArgs, ToxDelegates.CallbackFriendRequestDelegate>(ToxCallbacks.Friend.FriendRequest, cb =>
                 (tox, publicKey, message, length, userData) =>
                        cb(new ToxEventArgs.FriendRequestEventArgs(new ToxKey(ToxKeyType.Public, ToxTools.HexBinToString(publicKey)), ToxConstants.Encoding.GetString(message, 0, (int)length))));

        /// <summary>
        /// Occurs when a friend request is received.
        /// Friend requests should be accepted with AddFriendNoRequest.
        /// </summary>
        public event EventHandler<ToxEventArgs.FriendRequestEventArgs> OnFriendRequestReceived {
            add => this.friendRequest.Add(this, this.handle, value);
            remove => this.friendRequest.Remove(this.handle, value);
        }

        private readonly ToxCallbackHandler<ToxEventArgs.FriendMessageEventArgs, ToxDelegates.CallbackFriendMessageDelegate> friendMessage
          = new ToxCallbackHandler<ToxEventArgs.FriendMessageEventArgs, ToxDelegates.CallbackFriendMessageDelegate>(ToxCallbacks.Friend.Message, cb =>
                (tox, friendNumber, type, message, length, userdata) =>
                    cb(new ToxEventArgs.FriendMessageEventArgs(friendNumber, ToxConstants.Encoding.GetString(message, 0, (int)length), type)));

        /// <summary>
        /// Occurs when a message is received from a friend.
        /// </summary>
        public event EventHandler<ToxEventArgs.FriendMessageEventArgs> OnFriendMessageReceived {
            add => this.friendMessage.Add(this, this.handle, value);
            remove => this.friendMessage.Remove(this.handle, value);
        }

        private readonly ToxCallbackHandler<ToxEventArgs.NameChangeEventArgs, ToxDelegates.CallbackNameChangeDelegate> friendNameChange
          = new ToxCallbackHandler<ToxEventArgs.NameChangeEventArgs, ToxDelegates.CallbackNameChangeDelegate>(ToxCallbacks.Friend.NameChange, cb =>
                    (tox, friendNumber, newName, length, userData) =>
                            cb(new ToxEventArgs.NameChangeEventArgs(friendNumber, ToxConstants.Encoding.GetString(newName, 0, (int)length))));

        /// <summary>
        /// Occurs when a friend has changed his/her name.
        /// </summary>
        public event EventHandler<ToxEventArgs.NameChangeEventArgs> OnFriendNameChanged {
            add => this.friendNameChange.Add(this, this.handle, value);
            remove => this.friendNameChange.Remove(this.handle, value);
        }

        private readonly ToxCallbackHandler<ToxEventArgs.StatusMessageEventArgs, ToxDelegates.CallbackStatusMessageDelegate> friendStatusMessageChange
          = new ToxCallbackHandler<ToxEventArgs.StatusMessageEventArgs, ToxDelegates.CallbackStatusMessageDelegate>(ToxCallbacks.Friend.StatusMessageChange, cb =>
                    (tox, friendNumber, newStatus, length, userData) =>
                            cb(new ToxEventArgs.StatusMessageEventArgs(friendNumber, ToxConstants.Encoding.GetString(newStatus, 0, (int)length))));

        /// <summary>
        /// Occurs when a friend has changed their status message.
        /// </summary>
        public event EventHandler<ToxEventArgs.StatusMessageEventArgs> OnFriendStatusMessageChanged {
            add => this.friendStatusMessageChange.Add(this, this.handle, value);
            remove => this.friendStatusMessageChange.Remove(this.handle, value);
        }

        private readonly ToxCallbackHandler<ToxEventArgs.StatusEventArgs, ToxDelegates.CallbackUserStatusDelegate> friendStatusChange
          = new ToxCallbackHandler<ToxEventArgs.StatusEventArgs, ToxDelegates.CallbackUserStatusDelegate>(ToxCallbacks.Friend.StatusChange, cb =>
                    (tox, friendNumber, status, userData) =>
                            cb(new ToxEventArgs.StatusEventArgs(friendNumber, status)));

        /// <summary>
        /// Occurs when a friend has changed their user status.
        /// </summary>
        public event EventHandler<ToxEventArgs.StatusEventArgs> OnFriendStatusChanged {
            add => this.friendStatusChange.Add(this, this.handle, value);
            remove => this.friendStatusChange.Remove(this.handle, value);
        }

        private readonly ToxCallbackHandler<ToxEventArgs.TypingStatusEventArgs, ToxDelegates.CallbackTypingChangeDelegate> friendTypingChange
          = new ToxCallbackHandler<ToxEventArgs.TypingStatusEventArgs, ToxDelegates.CallbackTypingChangeDelegate>(ToxCallbacks.Friend.TypingChange, cb =>
                    (tox, friendNumber, typing, userData) => cb(new ToxEventArgs.TypingStatusEventArgs(friendNumber, typing)));

        /// <summary>
        /// Occurs when a friend's typing status has changed.
        /// </summary>
        public event EventHandler<ToxEventArgs.TypingStatusEventArgs> OnFriendTypingChanged {
            add => this.friendTypingChange.Add(this, this.handle, value);
            remove => this.friendTypingChange.Remove(this.handle, value);
        }

        private readonly ToxCallbackHandler<ToxEventArgs.FriendConnectionStatusEventArgs, ToxDelegates.CallbackFriendConnectionStatusDelegate> friendConnectionStatusChange
          = new ToxCallbackHandler<ToxEventArgs.FriendConnectionStatusEventArgs, ToxDelegates.CallbackFriendConnectionStatusDelegate>(ToxCallbacks.Friend.ConnectionStatusChange, cb =>
                    (tox, friendNumber, status, userData) => cb(new ToxEventArgs.FriendConnectionStatusEventArgs(friendNumber, status)));

        /// <summary>
        /// Occurs when the connection status of a friend has changed.
        /// </summary>
        public event EventHandler<ToxEventArgs.FriendConnectionStatusEventArgs> OnFriendConnectionStatusChanged {
            add => this.friendConnectionStatusChange.Add(this, this.handle, value);
            remove => this.friendConnectionStatusChange.Remove(this.handle, value);
        }

        private readonly ToxCallbackHandler<ToxEventArgs.FriendPacketEventArgs, ToxDelegates.CallbackFriendPacketDelegate> onFriendLossyPacket
          = new ToxCallbackHandler<ToxEventArgs.FriendPacketEventArgs, ToxDelegates.CallbackFriendPacketDelegate>(ToxCallbacks.Friend.LossyPacket, cb =>
                (tox, friendNumber, data, length, userData) => cb(new ToxEventArgs.FriendPacketEventArgs(friendNumber, data)));

        /// <summary>
        /// Occurs when a lossy packet from a friend is received.
        /// </summary>
        public event EventHandler<ToxEventArgs.FriendPacketEventArgs> OnFriendLossyPacketReceived {
            add => this.onFriendLossyPacket.Add(this, this.handle, value);
            remove => this.onFriendLossyPacket.Remove(this.handle, value);
        }

        private readonly ToxCallbackHandler<ToxEventArgs.FriendPacketEventArgs, ToxDelegates.CallbackFriendPacketDelegate> onFriendLosslessPacket
          = new ToxCallbackHandler<ToxEventArgs.FriendPacketEventArgs, ToxDelegates.CallbackFriendPacketDelegate>(ToxCallbacks.Friend.LosslessPacket, cb =>
              (tox, friendNumber, data, length, userData) => cb(new ToxEventArgs.FriendPacketEventArgs(friendNumber, data)));

        /// <summary>
        /// Occurs when a lossless packet from a friend is received.
        /// </summary>
        public event EventHandler<ToxEventArgs.FriendPacketEventArgs> OnFriendLosslessPacketReceived {
            add => this.onFriendLosslessPacket.Add(this, this.handle, value);
            remove => this.onFriendLosslessPacket.Remove(this.handle, value);
        }

        /// <summary>
        /// Adds a friend to the friend list and sends a friend request.
        /// </summary>
        /// <param name="id">The Tox id of the friend to add.</param>
        /// <param name="message">The message that will be sent along with the friend request.</param>
        /// <param name="error"></param>
        /// <returns>The friend number.</returns>
        public uint AddFriend([NotNull]ToxId id, [NotNull] string message, out ToxErrorFriendAdd error)
        {
            byte[] msg = ToxConstants.Encoding.GetBytes(message);
            error = ToxErrorFriendAdd.Ok;
            return ToxFunctions.Friend.Add(this.handle, id.GetBytes(), msg, (uint)msg.Length, ref error);
        }

        /// <summary>
        /// Adds a friend to the friend list without sending a friend request.
        /// This method should be used to accept friend requests.
        /// </summary>
        /// <param name="publicKey">The public key of the friend to add.</param>
        /// <param name="error"></param>
        /// <returns>The friend number.</returns>
        public uint AddFriendNoRequest([NotNull] ToxKey publicKey, out ToxErrorFriendAdd error)
        {
            ThrowIfDisposed();
            error = ToxErrorFriendAdd.Ok;
            return ToxFunctions.Friend.AddNoRequest(this.handle, publicKey.GetBytes(), ref error);
        }

        /// <summary>
        /// Checks if there exists a friend with given friendNumber.
        /// </summary>
        /// <param name="friendNumber">The friend number to check.</param>
        /// <returns>True if the friend exists.</returns>
        public bool FriendExists(uint friendNumber)
        {
            ThrowIfDisposed();
            return ToxFunctions.Friend.Exists(this.handle, friendNumber);
        }

        /// <summary>
        /// Retrieves the friendNumber associated with the specified public key.
        /// </summary>
        /// <param name="publicKey">The public key to look for.</param>
        /// <param name="error"></param>
        /// <returns>The friend number on success.</returns>
        public uint GetFriendByPublicKey([NotNull] ToxKey publicKey, out ToxErrorFriendByPublicKey error)
        {
            ThrowIfDisposed();
            error = ToxErrorFriendByPublicKey.Ok;
            return ToxFunctions.Friend.ByPublicKey(this.handle, publicKey.GetBytes(), ref error);
        }

        /// <summary>
        /// Retrieves a friend's public key.
        /// </summary>
        /// <param name="friendNumber">The friend number to retrieve the public key of.</param>
        /// <param name="error"></param>
        /// <returns>The friend's public key on success.</returns>
        public ToxKey GetFriendPublicKey(uint friendNumber, out ToxErrorFriendGetPublicKey error)
        {
            ThrowIfDisposed();
            byte[] address = new byte[ToxConstants.PublicKeySize];
            error = ToxErrorFriendGetPublicKey.Ok;

            if (!ToxFunctions.Friend.GetPublicKey(this.handle, friendNumber, address, ref error))
            {
                return null;
            }

            return new ToxKey(ToxKeyType.Public, address);
        }

        /// <summary>
        /// Sets the typing status of this Tox instance for a friend.
        /// </summary>
        /// <param name="friendNumber">The friend number to set the typing status for.</param>
        /// <param name="isTyping">Whether or not we're typing.</param>
        /// <param name="error"></param>
        /// <returns>True on success.</returns>
        public bool SetTypingStatus(uint friendNumber, bool isTyping, out ToxErrorSetTyping error)
        {
            ThrowIfDisposed();
            error = ToxErrorSetTyping.Ok;
            return ToxFunctions.Self.SetTyping(this.handle, friendNumber, isTyping, ref error);
        }

        /// <summary>
        /// Sends a message to a friend.
        /// </summary>
        /// <param name="friendNumber">The friend number to send the message to.</param>
        /// <param name="message">The message to be sent. Maximum length: <see cref="ToxConstants.MaxMessageLength"/></param>
        /// <param name="type">The type of this message.</param>
        /// <param name="error"></param>
        /// <returns>Message ID on success.</returns>
        public uint SendMessage(uint friendNumber, [NotNull] string message, ToxMessageType type, out ToxErrorSendMessage error)
        {
            ThrowIfDisposed();
            byte[] bytes = ToxConstants.Encoding.GetBytes(message);
            error = ToxErrorSendMessage.Ok;

            return ToxFunctions.Friend.SendMessage(this.handle, friendNumber, type, bytes, (uint)bytes.Length, ref error);
        }

        /// <summary>
        /// Deletes a friend from the friend list.
        /// </summary>
        /// <param name="friendNumber">The friend number to be deleted.</param>
        /// <param name="error"></param>
        /// <returns>True on success.</returns>
        public bool DeleteFriend(uint friendNumber, out ToxErrorFriendDelete error)
        {
            ThrowIfDisposed();
            error = ToxErrorFriendDelete.Ok;
            return ToxFunctions.Friend.Delete(this.handle, friendNumber, ref error);
        }
        /// <summary>
        /// Retrieves the name of a friend.
        /// </summary>
        /// <param name="friendNumber">The friend number to retrieve the name of.</param>
        /// <param name="error"></param>
        /// <returns>The friend's name on success.</returns>
        public string GetFriendName(uint friendNumber, out ToxErrorFriendQuery error)
        {
            ThrowIfDisposed();
            error = ToxErrorFriendQuery.Ok;
            uint size = ToxFunctions.Friend.GetNameSize(this.handle, friendNumber, ref error);

            if (error != ToxErrorFriendQuery.Ok)
                return string.Empty;

            byte[] name = new byte[size];
            if (!ToxFunctions.Friend.GetName(this.handle, friendNumber, name, ref error))
                return string.Empty;

            return ToxConstants.Encoding.GetString(name, 0, name.Length);
        }

        /// <summary>
        /// Retrieves the status message of a friend.
        /// </summary>
        /// <param name="friendNumber">The friend number to retrieve the status message of.</param>
        /// <param name="error"></param>
        /// <returns>The friend's status message on success.</returns>
        public string GetFriendStatusMessage(uint friendNumber, out ToxErrorFriendQuery error)
        {
            ThrowIfDisposed();

            error = ToxErrorFriendQuery.Ok;
            uint size = ToxFunctions.Friend.GetStatusMessageSize(this.handle, friendNumber, ref error);

            if (error != ToxErrorFriendQuery.Ok)
                return string.Empty;

            byte[] message = new byte[size];
            if (!ToxFunctions.Friend.GetStatusMessage(this.handle, friendNumber, message, ref error))
                return string.Empty;

            return ToxConstants.Encoding.GetString(message, 0, message.Length);
        }

        /// <summary>
        /// Sends a file control command to a friend for a given file transfer.
        /// </summary>
        /// <param name="friendNumber">The friend to send the file control to.</param>
        /// <param name="fileNumber">The file transfer that this control is meant for.</param>
        /// <param name="control">The control to send.</param>
        /// <param name="error"></param>
        /// <returns>True on success.</returns>
        public bool FileControl(uint friendNumber, uint fileNumber, ToxFileControl control, out ToxErrorFileControl error)
        {
            ThrowIfDisposed();

            error = ToxErrorFileControl.Ok;
            return ToxFunctions.File.Control(this.handle, friendNumber, fileNumber, control, ref error);
        }

        /// <summary>
        /// Send a file transmission request.
        /// </summary>
        /// <param name="friendNumber">The friend number to send the request to.</param>
        /// <param name="kind">The kind of file that will be transferred.</param>
        /// <param name="fileSize">The size of the file that will be transferred.</param>
        /// <param name="fileName">The filename of the file that will be transferred.</param>
        /// <param name="error"></param>
        /// <returns>Info about the file transfer on success.</returns>
        public IToxFileInfo FileSend(uint friendNumber, ToxFileKind kind, long fileSize, string fileName, out ToxErrorFileSend error)
        {
            ThrowIfDisposed();

            error = ToxErrorFileSend.Ok;
            byte[] fileNameBytes = ToxConstants.Encoding.GetBytes(fileName);
            var fileNumber = ToxFunctions.File.Send(this.handle, friendNumber, kind, (ulong)fileSize, null, fileNameBytes, (uint)fileNameBytes.Length, ref error);

            if (error == ToxErrorFileSend.Ok)
            {
                return new ToxFileInfo(fileNumber, FileGetId(friendNumber, fileNumber, out _));
            }

            return null;
        }

        /// <summary>
        /// Send a file transmission request.
        /// </summary>
        /// <param name="friendNumber">The friend number to send the request to.</param>
        /// <param name="kind">The kind of file that will be transferred.</param>
        /// <param name="fileSize">The size of the file that will be transferred.</param>
        /// <param name="fileName">The filename of the file that will be transferred.</param>
        /// <param name="fileId">The id to identify this transfer with. Should be ToxConstants.FileIdLength bytes long.</param>
        /// <param name="error"></param>
        /// <returns>Info about the file transfer on success.</returns>
        public IToxFileInfo FileSend(uint friendNumber, ToxFileKind kind, long fileSize, string fileName, byte[] fileId, out ToxErrorFileSend error)
        {
            ThrowIfDisposed();

            if (fileId.Length != ToxConstants.FileIdLength)
            {
                throw new ArgumentException($"fileId should be exactly {ToxConstants.FileIdLength} bytes long", "fileId");
            }

            error = ToxErrorFileSend.Ok;
            byte[] fileNameBytes = ToxConstants.Encoding.GetBytes(fileName);
            var fileNumber = ToxFunctions.File.Send(this.handle, friendNumber, kind, (ulong)fileSize, fileId, fileNameBytes, (uint)fileNameBytes.Length, ref error);

            if (error == ToxErrorFileSend.Ok)
            {
                return new ToxFileInfo(fileNumber, fileId);
            }

            return null;
        }

        /// <summary>
        /// Sends a file seek control command to a friend for a given file transfer.
        /// </summary>
        /// <param name="friendNumber">The friend to send the seek command to.</param>
        /// <param name="fileNumber">The file transfer that this command is meant for.</param>
        /// <param name="position">The position that the friend should change his stream to.</param>
        /// <param name="error"></param>
        /// <returns>True on success.</returns>
        public bool FileSeek(uint friendNumber, uint fileNumber, long position, out ToxErrorFileSeek error)
        {
            ThrowIfDisposed();

            error = ToxErrorFileSeek.Ok;
            return ToxFunctions.File.Seek(this.handle, friendNumber, fileNumber, (ulong)position, ref error);
        }

        /// <summary>
        /// Sends a chunk of file data to a friend. This should be called in response to OnFileChunkRequested.
        /// </summary>
        /// <param name="friendNumber">The friend to send the chunk to.</param>
        /// <param name="fileNumber">The file transfer that this chunk belongs to.</param>
        /// <param name="position">The position from which to continue reading.</param>
        /// <param name="data">The data to send. (should be equal to 'Length' received through OnFileChunkRequested).</param>
        /// <param name="error"></param>
        /// <returns>True on success.</returns>
        public bool FileSendChunk(uint friendNumber, uint fileNumber, ulong position, byte[] data, out ToxErrorFileSendChunk error)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            ThrowIfDisposed();
            error = ToxErrorFileSendChunk.Ok;
            return ToxFunctions.File.SendChunk(this.handle, friendNumber, fileNumber, position, data, (uint)data.Length, ref error);
        }

        /// <summary>
        /// Retrieves the unique id of a file transfer. This can be used to uniquely identify file transfers across core restarts.
        /// </summary>
        /// <param name="friendNumber">The friend number that's associated with this transfer.</param>
        /// <param name="fileNumber">The target file transfer.</param>
        /// <param name="error"></param>
        /// <returns>File transfer id on success.</returns>
        public byte[] FileGetId(uint friendNumber, uint fileNumber, out ToxErrorFileGet error)
        {
            ThrowIfDisposed();

            error = ToxErrorFileGet.Ok;
            byte[] id = new byte[ToxConstants.FileIdLength];

            if (ToxFunctions.File.GetFileId(this.handle, friendNumber, fileNumber, id, ref error))
            {
                return id;
            }

            return null;
        }

        /// <summary>
        /// Sends a custom lossy packet to a friend. 
        /// Lossy packets are like UDP packets, they may never reach the other side, arrive more than once or arrive in the wrong order.
        /// </summary>
        /// <param name="friendNumber">The friend to send the packet to.</param>
        /// <param name="data">The data to send. The first byte must be in the range 200-254. The maximum length of the data is ToxConstants.MaxCustomPacketSize</param>
        /// <param name="error"></param>
        /// <returns>True on success.</returns>
        public bool FriendSendLossyPacket(uint friendNumber, byte[] data, out ToxErrorFriendCustomPacket error)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            ThrowIfDisposed();
            error = ToxErrorFriendCustomPacket.Ok;

            return ToxFunctions.Friend.SendLossyPacket(this.handle, friendNumber, data, (uint)data.Length, ref error);
        }

        /// <summary>
        /// Sends a custom lossless packet to a friend.
        /// Lossless packets behave like TCP, they're reliable and arrive in order. The difference is that it's not a stream.
        /// </summary>
        /// <param name="friendNumber">The friend to send the packet to.</param>
        /// <param name="data">The data to send. The first byte must be in the range 160-191. The maximum length of the data is ToxConstants.MaxCustomPacketSize</param>
        /// <param name="error"></param>
        /// <returns>True on success.</returns>
        public bool FriendSendLosslessPacket(uint friendNumber, byte[] data, out ToxErrorFriendCustomPacket error)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            ThrowIfDisposed();
            error = ToxErrorFriendCustomPacket.Ok;
            return ToxFunctions.Friend.SendLosslessPacket(this.handle, friendNumber, data, (uint)data.Length, ref error);
        }

        /// <summary>
        /// Retrieves the time a friend was last seen online.
        /// </summary>
        /// <param name="friendNumber">The friend to retrieve the 'last online' of.</param>
        /// <param name="error"></param>
        /// <returns>The time this friend was last seen online, on success.</returns>
        public DateTime GetFriendLastOnline(uint friendNumber, out ToxErrorFriendGetLastOnline error)
        {
            error = ToxErrorFriendGetLastOnline.Ok;
            ulong time = ToxFunctions.Friend.GetLastOnline(this.handle, friendNumber, ref error);

            return ToxTools.EpochToDateTime(time);
        }

        /// <summary>
        /// Retrieves the time a friend was last seen online.
        /// </summary>
        /// <param name="friendNumber">The friend to retrieve the 'last online' of.</param>
        /// <returns>The time this friend was last seen online, on success.</returns>
        public DateTime GetFriendLastOnline(uint friendNumber)
        {
            var error = ToxErrorFriendGetLastOnline.Ok;
            return GetFriendLastOnline(friendNumber, out error);
        }


        private void ThrowIfDisposed()
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException(nameof(ToxFriend));
            }
        }

        void IDisposable.Dispose()
        {
            this.isDisposed = true;
            this.handle = null;
        }
    }
}
