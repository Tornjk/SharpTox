using SharpTox.Core.Interfaces;
using SharpTox.Encryption;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharpTox.Core
{
    /// <summary>
    /// Represents an instance of Tox.
    /// </summary>
    public sealed class Tox : ITox
    {
        private CancellationTokenSource cancelTokenSource;

        private bool running = false;
        private bool disposed = false;

        /// <summary>
        /// The nickname of this Tox instance.
        /// </summary>
        public string Name {
            get => this.DisposedCheck(handle => ToxHelper.GetString(handle, ToxFunctions.Self.GetNameSize, ToxFunctions.Self.GetName));
            set => this.DisposedCheck(handle =>
            {
                byte[] bytes = ToxConstants.Encoding.GetBytes(value);
                var error = ToxErrorSetInfo.Ok;
                var success = ToxFunctions.Self.SetName(handle, bytes, (uint)bytes.Length, ref error);

                if (!success)
                {
                    throw new InvalidOperationException();
                }

                if (error == ToxErrorSetInfo.TooLong)
                {
                    throw new ArgumentException(nameof(value));
                }
            });
        }

        /// <summary>
        /// The nospam for this Tox instance
        /// </summary>
        public uint Nospam
        {
            get => this.DisposedCheck(handle => ToxFunctions.Self.GetNospam(handle));
            set => this.DisposedCheck(handle => ToxFunctions.Self.SetNospam(handle, value));
        }

        /// <summary>
        /// The status message of this Tox instance.
        /// </summary>
        public string StatusMessage {
            get => this.DisposedCheck(handle => ToxHelper.GetString(handle, ToxFunctions.Self.GetStatusMessageSize, ToxFunctions.Self.GetStatusMessage));
            set => this.DisposedCheck(handle =>
            {
                byte[] bytes = ToxConstants.Encoding.GetBytes(value);
                var error = ToxErrorSetInfo.Ok;
                ToxFunctions.Self.SetStatusMessage(handle, bytes, (uint)bytes.Length, ref error);

                if (error == ToxErrorSetInfo.TooLong)
                {
                    throw new ArgumentException(nameof(value));
                }
            });
        }

        /// <summary>
        /// The Tox ID of this Tox instance.
        /// </summary>
        public ToxId Id => this.DisposedCheck(handle =>
        {
            byte[] address = new byte[ToxConstants.AddressSize];
            ToxFunctions.Self.GetAddress(handle, address);

            return new ToxId(address);
        });

        /// <summary>
        /// Retrieves the temporary DHT public key of this Tox instance.
        /// </summary>
        public ToxKey DhtId => this.DisposedCheck(tox => new ToxKey(ToxKeyType.Public, ToxHelper.GetArray<byte>(tox, ToxFunctions.Self.GetDhtId, ToxConstants.PublicKeySize)));

        /// <summary>
        /// Current user status of this Tox instance.
        /// </summary>
        public ToxUserStatus Status {
            get => this.DisposedCheck(tox => ToxFunctions.Self.GetStatus(tox));
            set => this.DisposedCheck(tox => ToxFunctions.Self.SetStatus(tox, value));
        }

        /// <summary>
        /// The handle of this instance of Tox. 
        /// Do not dispose this handle manually, use the Dispose method in this class instead.
        /// </summary>
        internal ToxHandle Handle { get; }

        public IToxFriend Friends { get; }

        // THIS IS THE ONE AND ONLY CTOR
        internal Tox([NotNull] ToxHandle handle)
        {
            if (handle == null)
            {
                throw new ArgumentNullException(nameof(handle));
            }

            if (handle.IsInvalid || handle.IsClosed)
            {
                throw new ArgumentException(nameof(handle));
            }

            this.Handle = handle;
            this.Friends = new ToxFriend(handle);
        }

        /// <summary>
        /// Initializes a new instance of Tox. If no secret key is specified, toxcore will generate a new keypair.
        /// </summary>
        /// <param name="options">The options to initialize this instance of Tox with.</param>
        /// <param name="secretKey">Optionally, specify the secret key to initialize this instance of Tox with. Must be ToxConstants.SecretKeySize bytes in size.</param>
        public Tox(ToxOptions options) : this(options.Create())
        {
        }

        /// <summary>
        /// Releases all resources used by this instance of Tox.
        /// </summary>
        public void Dispose() => Dispose(true);

        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                // security critical
                if (this.cancelTokenSource != null)
                {
                    try
                    {
                        this.cancelTokenSource.Cancel();
                        this.cancelTokenSource.Dispose();
                    }
                    catch (ObjectDisposedException) { }

                    this.cancelTokenSource = null;
                }
            }

            if (!Handle.IsInvalid && !Handle.IsClosed)
            {
                Handle.Dispose();
            }

            this.disposed = true;
        }

        /// <summary>
        /// Starts the main 'tox_iterate' loop at an interval retrieved with 'tox_iteration_interval'.
        /// If you want to manage your own loop, use the Iterate method instead.
        /// </summary>
        public void Start()
        {
            ThrowIfDisposed();

            if (this.running)
            {
                return;
            }

            Loop();
        }

        /// <summary>
        /// Stops the main tox_do loop if it's running.
        /// </summary>
        public void Stop()
        {
            ThrowIfDisposed();

            if (!this.running)
            {
                return;
            }

            if (this.cancelTokenSource != null)
            {
                this.cancelTokenSource.Cancel();
                this.cancelTokenSource.Dispose();

                this.running = false;
            }
        }

        /// <summary>
        /// Runs the tox_iterate once in the current thread.
        /// </summary>
        /// <returns>The next timeout.</returns>
        public TimeSpan Iterate()
        {
            ThrowIfDisposed();

            if (running)
            {
                throw new Exception("Loop already running");
            }

            return DoIterate();
        }

        private TimeSpan DoIterate()
        {
            ToxFunctions.Iterate(Handle);
            return TimeSpan.FromMilliseconds(ToxFunctions.IterationInterval(Handle));
        }

        private void Loop()
        {
            this.cancelTokenSource = new CancellationTokenSource();
            this.running = true;

            Task.Factory.StartNew(async () =>
            {
                while (running && !disposed)
                {
                    if (cancelTokenSource.IsCancellationRequested)
                        break;

                    await Task.Delay(DoIterate());
                }
            }, cancelTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        /// <summary>
        /// Adds a node as a TCP relay. 
        /// This method can be used to initiate TCP connections to different ports on the same bootstrap node, or to add TCP relays without using them as bootstrap nodes.
        /// </summary>
        /// <param name="node">The node to add.</param>
        /// <param name="error"></param>
        /// <returns>True on success.</returns>
        public bool AddTcpRelay(ToxNode node, out ToxErrorBootstrap error)
        {
            ThrowIfDisposed();

            if (node == null)
                throw new ArgumentNullException("node");

            error = ToxErrorBootstrap.Ok;
            return ToxFunctions.AddTcpRelay(Handle, node.Address, node.Port, node.PublicKey.GetBytes(), ref error);
        }

        /// <summary>
        /// Attempts to bootstrap this Tox instance with a ToxNode. A 'getnodes' request is sent to the given node.
        /// </summary>
        /// <param name="node">The node to bootstrap off of.</param>
        /// <param name="error"></param>
        /// <returns>True if the 'getnodes' request was sent successfully.</returns>
        public bool Bootstrap(ToxNode node, out ToxErrorBootstrap error)
        {
            ThrowIfDisposed();

            if (node == null)
                throw new ArgumentNullException("node");

            error = ToxErrorBootstrap.Ok;
            return ToxFunctions.Bootstrap(Handle, node.Address, node.Port, node.PublicKey.GetBytes(), ref error);
        }

        /// <summary>
        /// Retrieves a ToxData object that contains the profile data of this Tox instance.
        /// </summary>
        /// <returns></returns>
        public IToxData GetData()
        {
            ThrowIfDisposed();

            byte[] bytes = new byte[ToxFunctions.GetSaveDataSize(Handle)];
            ToxFunctions.GetSaveData(Handle, bytes);

            return ToxData.FromBytes(bytes);
        }

        /// <summary>
        /// Retrieves a ToxData object that contains the profile data of this Tox instance, encrypted with the provided key.
        /// </summary>
        /// <param name="key">The key to encrypt the Tox data with.</param>
        /// <returns></returns>
        public IToxData GetData(ToxEncryptionKey key, out ToxErrorEncryption error)
        {
            ThrowIfDisposed();
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var data = this.GetData();

            byte[] encrypted = key.Encrypt(data.Bytes, out error);
            if (error != ToxErrorEncryption.Ok)
            {
                return null;
            }

            return ToxData.FromBytes(encrypted);
        }

        public IToxData GetData(string password, out ToxErrorEncryption error)
        {
            ThrowIfDisposed();

            var data = this.GetData();
            byte[] encrypted = ToxEncryption.Encrypt(data.Bytes, password, out error);
            if (error != ToxErrorEncryption.Ok)
            {
                return null;
            }

            return ToxData.FromBytes(encrypted);
        }

        /// <summary>
        /// Retrieves the private key of this Tox instance.
        /// </summary>
        /// <returns>The private key of this Tox instance.</returns>
        public ToxKey GetSecretKey()
        {
            ThrowIfDisposed();

            byte[] key = new byte[ToxConstants.SecretKeySize];
            ToxFunctions.Self.GetSecretKey(Handle, key);

            return new ToxKey(ToxKeyType.Secret, key);
        }

        /// <summary>
        /// Retrieves the UDP port this instance of Tox is bound to.
        /// </summary>
        /// <param name="error"></param>
        /// <returns>The UDP port on success.</returns>
        public ushort GetUdpPort(out ToxErrorGetPort error)
        {
            ThrowIfDisposed();

            error = ToxErrorGetPort.Ok;
            return ToxFunctions.Self.GetUdpPort(Handle, ref error);
        }

        /// <summary>
        /// Retrieves the TCP port this instance of Tox is bound to.
        /// </summary>
        /// <param name="error"></param>
        /// <returns>The TCP port on success.</returns>
        public ushort GetTcpPort(out ToxErrorGetPort error)
        {
            ThrowIfDisposed();

            error = ToxErrorGetPort.Ok;
            return ToxFunctions.Self.GetTcpPort(Handle, ref error);
        }

        /// <summary>
        /// Joins a Conference with the given public key of the Conference.
        /// </summary>
        /// <param name="friendNumber">The friend number we received an invite from.</param>
        /// <param name="cookie">Data obtained from the OnConferenceInvite event.</param>
        /// <returns>The Conference number on success.</returns>
        public uint JoinConference(uint friendNumber, byte[] cookie, out ToxErrorConferenceJoin error)
        {
            if (cookie == null)
            {
                throw new ArgumentNullException(nameof(cookie));
            }

            ThrowIfDisposed();

            error = ToxErrorConferenceJoin.Ok;
            return ToxFunctions.Conference.Join(this.Handle, friendNumber, cookie, (uint)cookie.Length, ref error);
        }

        /// <summary>
        /// Retrieves the name of a Conference member.
        /// </summary>
        /// <param name="conferenceNumber">The Conference that the peer is in.</param>
        /// <param name="peerNumber">The peer to retrieve the name of.</param>
        /// <returns>The peer's name on success.</returns>
        public string GetConferencePeerName(uint conferenceNumber, uint peerNumber, out ToxErrorConferencePeerQuery error)
        {
            ThrowIfDisposed();

            error = ToxErrorConferencePeerQuery.Ok;
            var size = ToxFunctions.Conference.Peer.GetNameSize(this.Handle, conferenceNumber, peerNumber, ref error);
            if (error != ToxErrorConferencePeerQuery.Ok)
            {
                return null;
            }

            byte[] name = new byte[size];
            if (ToxFunctions.Conference.Peer.GetName(this.Handle, conferenceNumber, peerNumber, name, ref error))
            {
                return ToxConstants.Encoding.GetString(name);
            }

            return null;
        }

        /// <summary>
        /// Retrieves the number of Conference members in a Conference chat.
        /// </summary>
        /// <param name="conferenceNumber">The Conference to get the member count of.</param>
        /// <returns>The member count on success.</returns>
        public uint GetConferencePeerCount(uint conferenceNumber, out ToxErrorConferencePeerQuery error)
        {
            ThrowIfDisposed();
            error = ToxErrorConferencePeerQuery.Ok;
            return ToxFunctions.Conference.Peer.Count(this.Handle, conferenceNumber, ref error);
        }

        /// <summary>
        /// Deletes a Conference chat.
        /// </summary>
        /// <param name="conferenceNumber">The Conference to delete.</param>
        /// <returns>True on success.</returns>
        public bool DeleteConference(uint conferenceNumber, out ToxErrorConferenceDelete error)
        {
            ThrowIfDisposed();
            error = ToxErrorConferenceDelete.Ok;
            return ToxFunctions.Conference.Delete(Handle, conferenceNumber, ref error);
        }

        /// <summary>
        /// Invites a friend to a Conference chat.
        /// </summary>
        /// <param name="friendNumber">The friend to invite to a Conference.</param>
        /// <param name="conferenceNumber">The Conference to invite the friend to.</param>
        /// <returns>True on success.</returns>
        public bool InviteFriend(uint friendNumber, uint conferenceNumber, out ToxErrorConferenceInvite error)
        {
            ThrowIfDisposed();
            error = ToxErrorConferenceInvite.Ok;
            return ToxFunctions.Conference.Invite(this.Handle, friendNumber, conferenceNumber, ref error);
        }

        public bool ValidMessage([NotNull] string message)
            => ToxConstants.Encoding.GetByteCount(message) < ToxFunctions.Max.MessageLength();

        /// <summary>
        /// Sends a message to a Conference.
        /// </summary>
        /// <param name="conferenceNumber">The Conference to send the message to.</param>
        /// <param name="message">The message to send.</param>
        /// <returns>True on success.</returns>
        public bool SendConferenceMessage(uint conferenceNumber, ToxMessageType type, string message, out ToxErrorConferenceSendMessage error)
        {
            ThrowIfDisposed();
            if (!this.ValidMessage(message))
            {
                throw new ArgumentException(nameof(message));
            }

            error = ToxErrorConferenceSendMessage.Ok;
            byte[] msg = ToxConstants.Encoding.GetBytes(message);
            return ToxFunctions.Conference.SendMessage(this.Handle, conferenceNumber, type, msg, (uint)msg.Length, ref error);
        }

        /// <summary>
        /// Creates a new Conference and retrieves the Conference number.
        /// </summary>
        /// <returns>The number of the created Conference on success.</returns>
        public uint NewConference(out ToxErrorConferenceNew error)
        {
            ThrowIfDisposed();
            error = ToxErrorConferenceNew.Ok;
            return ToxFunctions.Conference.New(this.Handle, ref error);
        }

        /// <summary>
        /// Check if the given peernumber corresponds to ours.
        /// </summary>
        /// <param name="conferenceNumber">The Conference to check in.</param>
        /// <param name="peerNumber">The peer number to check.</param>
        /// <returns>True if the given peer number is ours.</returns>
        public bool PeerNumberIsOurs(uint conferenceNumber, uint peerNumber, out ToxErrorConferencePeerQuery error)
        {
            ThrowIfDisposed();

            error = ToxErrorConferencePeerQuery.Ok;
            return ToxFunctions.Conference.Peer.NumberIsOurs(Handle, conferenceNumber, peerNumber, ref error);
        }

        public static bool ValidConferenceTitle(string title)
            => ToxConstants.Encoding.GetByteCount(title) < ToxFunctions.Max.NameLength();

        /// <summary>
        /// Changes the title of a Conference.
        /// </summary>
        /// <param name="conferenceNumber">The Conference to change the title of.</param>
        /// <param name="title">The title to set.</param>
        /// <returns>True on success.</returns>
        public bool SetConferenceTitle(uint conferenceNumber, string title, out ToxErrorConferenceTitle error)
        {
            ThrowIfDisposed();

            if (!ValidConferenceTitle(title))
            {
                throw new ArgumentException($"The specified Conference title is too long. (Check before with {nameof(ValidConferenceTitle)})");
            }

            error = ToxErrorConferenceTitle.Ok;
            byte[] bytes = ToxConstants.Encoding.GetBytes(title);
            return ToxFunctions.Conference.SetTitle(Handle, conferenceNumber, bytes, (byte)bytes.Length, ref error);
        }

        /// <summary>
        /// Retrieves the type of a Conference.
        /// </summary>
        /// <param name="conferenceNumber">The Conference to retrieve the type of.</param>
        /// <returns>The Conference type on success.</returns>
        public ToxConferenceType GetConferenceType(uint conferenceNumber, out ToxErrorConferenceGetType err)
        {
            ThrowIfDisposed();
            err = ToxErrorConferenceGetType.Ok;
            return ToxFunctions.Conference.GetType(Handle, conferenceNumber, ref err);
        }

        /// <summary>
        /// Retrieves the title of a Conference.
        /// </summary>
        /// <param name="conferenceNumber">The Conference to retrieve the title of.</param>
        /// <returns>The Conference's title on success.</returns>
        public string GetConferenceTitle(uint conferenceNumber, out ToxErrorConferenceTitle error)
        {
            ThrowIfDisposed();

            error = ToxErrorConferenceTitle.Ok;
            var size = ToxFunctions.Conference.GetTitleSize(this.Handle, conferenceNumber, ref error);
            if (error != ToxErrorConferenceTitle.Ok)
            {
                return null;
            }

            byte[] title = new byte[size];
            if (ToxFunctions.Conference.GetTitle(Handle, conferenceNumber, title, ref error))
            {
                return ToxConstants.Encoding.GetString(title);
            }

            return string.Empty;
        }

        /// <summary>
        /// Retrieves the public key of a peer.
        /// </summary>
        /// <param name="conferenceNumber">The Conference that the peer is in.</param>
        /// <param name="peerNumber">The peer to retrieve the public key of.</param>
        /// <returns>The peer's public key on success.</returns>
        public ToxKey GetConferencePeerPublicKey(uint conferenceNumber, uint peerNumber, out ToxErrorConferencePeerQuery error)
        {
            ThrowIfDisposed();

            byte[] key = new byte[ToxConstants.PublicKeySize];

            error = ToxErrorConferencePeerQuery.Ok;
            if (ToxFunctions.Conference.Peer.GetPublicKey(Handle, conferenceNumber, peerNumber, key, ref error) && error == ToxErrorConferencePeerQuery.Ok)
            {
                return new ToxKey(ToxKeyType.Public, key);
            }

            return null;
        }

        #region Events
        private readonly ToxCallbackHandler<ToxEventArgs.ConnectionStatusEventArgs, ToxDelegates.CallbackConnectionStatusDelegate> connectionStatusChange
          = new ToxCallbackHandler<ToxEventArgs.ConnectionStatusEventArgs, ToxDelegates.CallbackConnectionStatusDelegate>(ToxCallbacks.Self.ConnectionStatus, cb =>
                    (tox, status, userData) => cb(new ToxEventArgs.ConnectionStatusEventArgs(status)));

        /// <summary>
        /// Occurs when the connection status of this Tox instance has changed.
        /// </summary>
        public event EventHandler<ToxEventArgs.ConnectionStatusEventArgs> OnConnectionStatusChanged {
            add => this.connectionStatusChange.Add(this, this.Handle, value);
            remove => this.connectionStatusChange.Remove(this.Handle, value);
        }

        private readonly ToxCallbackHandler<ToxEventArgs.ReadReceiptEventArgs, ToxDelegates.CallbackReadReceiptDelegate> readReceipt
          = new ToxCallbackHandler<ToxEventArgs.ReadReceiptEventArgs, ToxDelegates.CallbackReadReceiptDelegate>(ToxCallbacks.Friend.ReadReceipt, cb =>
                    (tox, friendNumber, receipt, userData) => cb(new ToxEventArgs.ReadReceiptEventArgs(friendNumber, receipt)));

        /// <summary>
        /// Occurs when a read receipt is received.
        /// </summary>
        public event EventHandler<ToxEventArgs.ReadReceiptEventArgs> OnReadReceiptReceived {
            add => this.readReceipt.Add(this, this.Handle, value);
            remove => this.readReceipt.Remove(this.Handle, value);
        }

        private readonly ToxCallbackHandler<ToxEventArgs.FileControlEventArgs, ToxDelegates.CallbackFileControlDelegate> fileControlReceived
          = new ToxCallbackHandler<ToxEventArgs.FileControlEventArgs, ToxDelegates.CallbackFileControlDelegate>(ToxCallbacks.File.ReceiveControl, cb =>
                    (tox, friendNumber, fileNumber, control, userData) => cb(new ToxEventArgs.FileControlEventArgs(friendNumber, fileNumber, control)));

        /// <summary>
        /// Occurs when a file control is received.
        /// </summary>
        public event EventHandler<ToxEventArgs.FileControlEventArgs> OnFileControlReceived {
            add => this.fileControlReceived.Add(this, this.Handle, value);
            remove => this.fileControlReceived.Remove(this.Handle, value);
        }

        private readonly ToxCallbackHandler<ToxEventArgs.FileChunkEventArgs, ToxDelegates.CallbackFileReceiveChunkDelegate> fileChunkReceived
          = new ToxCallbackHandler<ToxEventArgs.FileChunkEventArgs, ToxDelegates.CallbackFileReceiveChunkDelegate>(ToxCallbacks.File.ReceiveChunk, cb =>
                    (tox, friendNumber, fileNumber, position, data, length, userData) =>
                            cb(new ToxEventArgs.FileChunkEventArgs(friendNumber, fileNumber, data, position)));

        /// <summary>
        /// Occurs when a chunk of data from a file transfer is received
        /// </summary>
        public event EventHandler<ToxEventArgs.FileChunkEventArgs> OnFileChunkReceived {
            add => this.fileChunkReceived.Add(this, this.Handle, value);
            remove => this.fileChunkReceived.Remove(this.Handle, value);
        }

        private readonly ToxCallbackHandler<ToxEventArgs.FileSendRequestEventArgs, ToxDelegates.CallbackFileReceiveDelegate> fileSendRequestReceived
          = new ToxCallbackHandler<ToxEventArgs.FileSendRequestEventArgs, ToxDelegates.CallbackFileReceiveDelegate>(ToxCallbacks.File.Receive, cb =>
                (tox, friendNumber, fileNumber, kind, fileSize, filename, filenameLength, userData) =>
                        cb(new ToxEventArgs.FileSendRequestEventArgs(friendNumber,
                                                                     fileNumber,
                                                                     kind,
                                                                     fileSize,
                                                                     filename == null ? string.Empty : ToxConstants.Encoding.GetString(filename, 0, filename.Length))));

        /// <summary>
        /// Occurs when a new file transfer request has been received.
        /// </summary>
        public event EventHandler<ToxEventArgs.FileSendRequestEventArgs> OnFileSendRequestReceived {
            add => this.fileSendRequestReceived.Add(this, this.Handle, value);
            remove => this.fileSendRequestReceived.Remove(this.Handle, value);
        }

        private readonly ToxCallbackHandler<ToxEventArgs.FileRequestChunkEventArgs, ToxDelegates.CallbackFileRequestChunkDelegate> fileChunkRequested
          = new ToxCallbackHandler<ToxEventArgs.FileRequestChunkEventArgs, ToxDelegates.CallbackFileRequestChunkDelegate>(ToxCallbacks.File.ChunkRequest, cb =>
                (tox, friendNumber, fileNumber, position, length, userData) =>
                            cb(new ToxEventArgs.FileRequestChunkEventArgs(friendNumber, fileNumber, position, length)));

        /// <summary>
        /// Occurs when the core requests the next chunk of the file.
        /// </summary>
        public event EventHandler<ToxEventArgs.FileRequestChunkEventArgs> OnFileChunkRequested {
            add => this.fileChunkRequested.Add(this, this.Handle, value);
            remove => this.fileChunkRequested.Remove(this.Handle, value);
        }

        private readonly ToxCallbackHandler<ToxEventArgs.ConferenceMessageEventArgs, ToxDelegates.ConferenceMessageDelegate> conferenceMessage
          = new ToxCallbackHandler<ToxEventArgs.ConferenceMessageEventArgs, ToxDelegates.ConferenceMessageDelegate>(ToxCallbacks.Conference.Message, cb =>
                     (tox, conferenceNumber, peerNumber, type, message, length, userData) =>
                            cb(new ToxEventArgs.ConferenceMessageEventArgs(conferenceNumber, peerNumber, ToxConstants.Encoding.GetString(message, 0, (int)length), type)));

        /// <summary>
        /// Occurs when a message is received from a Conference.
        /// </summary>
        public event EventHandler<ToxEventArgs.ConferenceMessageEventArgs> OnConferenceMessage {
            add => this.conferenceMessage.Add(this, this.Handle, value);
            remove => this.conferenceMessage.Remove(this.Handle, value);
        }

        private readonly ToxCallbackHandler<ToxEventArgs.ConferenceInviteEventArgs, ToxDelegates.ConferenceInviteDelegate> conferenceInvite
          = new ToxCallbackHandler<ToxEventArgs.ConferenceInviteEventArgs, ToxDelegates.ConferenceInviteDelegate>(ToxCallbacks.Conference.Invite, cb =>
               (tox, friendNumber, type, data, length, userData) => cb(new ToxEventArgs.ConferenceInviteEventArgs(friendNumber, type, data)));

        /// <summary>
        /// Occurs when a friend has sent an invite to a Conference.
        /// </summary>
        public event EventHandler<ToxEventArgs.ConferenceInviteEventArgs> OnConferenceInvite {
            add => this.conferenceInvite.Add(this, this.Handle, value);
            remove => this.conferenceInvite.Remove(this.Handle, value);
        }

        private readonly ToxCallbackHandler<ToxEventArgs.ConferenceTitleEventArgs, ToxDelegates.ConferenceTitleDelegate> conferenceTitleChange
          = new ToxCallbackHandler<ToxEventArgs.ConferenceTitleEventArgs, ToxDelegates.ConferenceTitleDelegate>(ToxCallbacks.Conference.Title, cb =>
              (tox, conferenceNumber, peerNumber, title, length, userData) =>
                            cb(new ToxEventArgs.ConferenceTitleEventArgs(conferenceNumber, peerNumber, ToxConstants.Encoding.GetString(title))));

        /// <summary>
        /// Occurs when the title of a Conference is changed.
        /// </summary>
        public event EventHandler<ToxEventArgs.ConferenceTitleEventArgs> OnConferenceTitleChanged {
            add => this.conferenceTitleChange.Add(this, this.Handle, value);
            remove => this.conferenceTitleChange.Remove(this.Handle, value);
        }

        private readonly ToxCallbackHandler<ToxEventArgs.ConferenceConnectedEventArgs, ToxDelegates.ConferenceConnectedDelegate> conferenceConnected
          = new ToxCallbackHandler<ToxEventArgs.ConferenceConnectedEventArgs, ToxDelegates.ConferenceConnectedDelegate>(ToxCallbacks.Conference.Connected, cb =>
                (tox, conferenceNumber, userData) => cb(new ToxEventArgs.ConferenceConnectedEventArgs(conferenceNumber)));

        /// <summary>
        /// Occurs after a conference was joined and successfully connected.
        /// </summary>
        public event EventHandler<ToxEventArgs.ConferenceConnectedEventArgs> OnConferenceConnected {
            add => this.conferenceConnected.Add(this, this.Handle, value);
            remove => this.conferenceConnected.Remove(this.Handle, value);
        }

        private readonly ToxCallbackHandler<ToxEventArgs.ConferencePeerNameEventArgs, ToxDelegates.ConferencePeerNameDelegate> conferencePeerName
          = new ToxCallbackHandler<ToxEventArgs.ConferencePeerNameEventArgs, ToxDelegates.ConferencePeerNameDelegate>(ToxCallbacks.Conference.PeerName, cb =>
           (tox, conferenceNumber, peerNumber, name, length, userData) => cb(new ToxEventArgs.ConferencePeerNameEventArgs(conferenceNumber, peerNumber, ToxConstants.Encoding.GetString(name))));

        /// <summary>
        /// This event is triggered when a peer changes their name.
        /// </summary>
        public event EventHandler<ToxEventArgs.ConferencePeerNameEventArgs> ConferencePeerNameChanged {
            add => this.conferencePeerName.Add(this, this.Handle, value);
            remove => this.conferencePeerName.Remove(this.Handle, value);
        }

        private readonly ToxCallbackHandler<ToxEventArgs.ConferencePeerListEventArgs, ToxDelegates.ConferencePeerListChangedDelegate> conferencePeerList
          = new ToxCallbackHandler<ToxEventArgs.ConferencePeerListEventArgs, ToxDelegates.ConferencePeerListChangedDelegate>(ToxCallbacks.Conference.PeerListChanged, cb =>
              (tox, conferenceNumber, userdata) => cb(new ToxEventArgs.ConferencePeerListEventArgs(conferenceNumber)));

        public event EventHandler<ToxEventArgs.ConferencePeerListEventArgs> ConferencePeerListChanged {
            add => this.conferencePeerList.Add(this, this.Handle, value);
            remove => this.conferencePeerList.Remove(this.Handle, value);
        }
        #endregion

        private T DisposedCheck<T>(Func<ToxHandle, T> cb)
        {
            this.ThrowIfDisposed();
            return cb(this.Handle);
        }

        private void DisposedCheck(Action<ToxHandle> cb)
        {
            this.ThrowIfDisposed();
            cb(this.Handle);
        }

        private void ThrowIfDisposed()
        {
            if (this.disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }
    }
}
