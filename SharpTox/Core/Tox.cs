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
    public sealed class Tox : IDisposable
    {
        private static Encoding ToxEncoding = Encoding.UTF8;

        private ToxHandle tox;
        private CancellationTokenSource _cancelTokenSource;

        private bool _running = false;
        private bool _disposed = false;

        /// <summary>
        /// Options that are used for this instance of Tox.
        /// </summary>
        public ToxOptions Options { get; }

        /// <summary>
        /// An array of friendnumbers of this Tox instance.
        /// </summary>
        public uint[] Friends => ToxHelper.Get<uint>(this.tox, ToxFunctions.Friend.GetFriendListSize, ToxFunctions.Friend.GetFriendList);

        /// <summary>
        /// The nickname of this Tox instance.
        /// </summary>
        public string Name {
            get {
                ThrowIfDisposed();
                return ToxHelper.GetString(this.tox, ToxFunctions.Self.GetNameSize, ToxFunctions.Self.GetName);
            }

            set {
                ThrowIfDisposed();
                byte[] bytes = ToxEncoding.GetBytes(value);
                var error = ToxErrorSetInfo.Ok;
                ToxFunctions.Self.SetName(this.tox, bytes, (uint)bytes.Length, ref error);
            }
        }

        /// <summary>
        /// The status message of this Tox instance.
        /// </summary>
        public string StatusMessage {
            get {
                ThrowIfDisposed();
                return ToxHelper.GetString(this.tox, ToxFunctions.Self.GetStatusMessageSize, ToxFunctions.Self.GetStatusMessage);
            }
            set {
                ThrowIfDisposed();

                byte[] msg = ToxEncoding.GetBytes(value);
                var error = ToxErrorSetInfo.Ok;
                ToxFunctions.Self.SetStatusMessage(tox, msg, (uint)msg.Length, ref error);
            }
        }

        /// <summary>
        /// The Tox ID of this Tox instance.
        /// </summary>
        public ToxId Id {
            get {
                ThrowIfDisposed();

                byte[] address = new byte[ToxConstants.AddressSize];
                ToxFunctions.Self.GetAddress(tox, address);

                return new ToxId(address);
            }
        }

        /// <summary>
        /// Retrieves the temporary DHT public key of this Tox instance.
        /// </summary>
        public ToxKey DhtId => new ToxKey(ToxKeyType.Public, ToxHelper.Get<byte>(this.tox, ToxFunctions.Self.GetDhtId, ToxConstants.PublicKeySize));

        /// <summary>
        /// Current user status of this Tox instance.
        /// </summary>
        public ToxUserStatus Status {
            get {
                ThrowIfDisposed();
                return ToxFunctions.Self.GetStatus(tox);
            }

            set {
                ThrowIfDisposed();
                ToxFunctions.Self.SetStatus(tox, value);
            }
        }

        /// <summary>
        /// The handle of this instance of Tox. 
        /// Do not dispose this handle manually, use the Dispose method in this class instead.
        /// </summary>
        internal ToxHandle Handle => this.tox;

        /// <summary>
        /// Initializes a new instance of Tox. If no secret key is specified, toxcore will generate a new keypair.
        /// </summary>
        /// <param name="options">The options to initialize this instance of Tox with.</param>
        /// <param name="secretKey">Optionally, specify the secret key to initialize this instance of Tox with. Must be ToxConstants.SecretKeySize bytes in size.</param>
        public Tox(ToxOptions options, ToxKey secretKey = null)
        {
            if (secretKey != null)
                options.SetData(secretKey.GetBytes(), ToxSavedataType.SecretKey);

            tox = options.Create();

            if (tox == null || tox.IsInvalid)
                throw new Exception("Could not create a new instance of tox, error: ");

            Options = options;
        }

        /// <summary>
        /// Initializes a new instance of Tox.
        /// </summary>
        /// <param name="options">The options to initialize this instance of Tox with.</param>
        /// <param name="data">A byte array containing Tox save data.</param>
        public Tox(ToxOptions options, ToxData data)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));

            if (data == null)
                throw new ArgumentNullException(nameof(data));

            options.SetData(data.Bytes, ToxSavedataType.ToxSave);
            tox = options.Create();
        }

        /// <summary>
        /// Initializes a new instance of Tox.
        /// </summary>
        /// <param name="options">The options to initialize this instance of Tox with.</param>
        /// <param name="data">A byte array containing Tox save data.</param>
        /// <param name="key">The key to decrypt the given encrypted Tox profile data.</param>
        public Tox(ToxOptions options, ToxData data, ToxEncryptionKey key)
        {
            this.Options = options ?? throw new ArgumentNullException(nameof(options));
            if (data == null)
                throw new ArgumentNullException("data");

            if (key == null)
                throw new ArgumentNullException("key");

            if (!data.IsEncrypted)
                throw new Exception("This data is not encrypted");

            var decryptError = ToxErrorDecryption.Ok;
            byte[] decryptedData = ToxEncryption.DecryptData(data.Bytes, key, out decryptError);

            if (decryptError != ToxErrorDecryption.Ok)
            {
                throw new ArgumentException($"Failed to decrypt with the given key, error: {decryptError}");
            }

            options.SetData(decryptedData, ToxSavedataType.ToxSave);
            tox = options.Create();
        }

        /// <summary>
        /// Initializes a new instance of Tox.
        /// </summary>
        /// <param name="options">The options to initialize this instance of Tox with.</param>
        /// <param name="data">A byte array containing Tox save data.</param>
        /// <param name="password">The password to decrypt the given encrypted Tox profile data.</param>
        public Tox(ToxOptions options, ToxData data, string password)
        {
            this.Options = options ?? throw new ArgumentNullException(nameof(options));

            if (data == null)
                throw new ArgumentNullException("data");

            if (password == null)
                throw new ArgumentNullException("password");

            if (!data.IsEncrypted)
                throw new Exception("This data is not encrypted");

            var decryptError = ToxErrorDecryption.Ok;

            byte[] decryptedData = ToxEncryption.DecryptData(data.Bytes, password, out decryptError);

            if (decryptError != ToxErrorDecryption.Ok)
            {
                throw new ArgumentException($"Failed to decrypt with the given key, error: {decryptError}");
            }

            options.SetData(decryptedData, ToxSavedataType.ToxSave);
            tox = options.Create();
        }

        /// <summary>
        /// Releases all resources used by this instance of Tox.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        //dispose pattern as described on msdn for a class that uses a safe handle
        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // security critical
                if (_cancelTokenSource != null)
                {
                    try
                    {
                        _cancelTokenSource.Cancel();
                        _cancelTokenSource.Dispose();
                    }
                    catch (ObjectDisposedException) { }

                    _cancelTokenSource = null;
                }
            }

            if (tox != null && !tox.IsInvalid && !tox.IsClosed)
                tox.Dispose();

            _disposed = true;
        }

        /// <summary>
        /// Starts the main 'tox_iterate' loop at an interval retrieved with 'tox_iteration_interval'.
        /// If you want to manage your own loop, use the Iterate method instead.
        /// </summary>
        public void Start()
        {
            ThrowIfDisposed();

            if (_running)
                return;

            Loop();
        }

        /// <summary>
        /// Stops the main tox_do loop if it's running.
        /// </summary>
        public void Stop()
        {
            ThrowIfDisposed();

            if (!_running)
                return;

            if (_cancelTokenSource != null)
            {
                _cancelTokenSource.Cancel();
                _cancelTokenSource.Dispose();

                _running = false;
            }
        }

        /// <summary>
        /// Runs the tox_iterate once in the current thread.
        /// </summary>
        /// <returns>The next timeout.</returns>
        public TimeSpan Iterate()
        {
            ThrowIfDisposed();

            if (_running)
                throw new Exception("Loop already running");

            return DoIterate();
        }

        private TimeSpan DoIterate()
        {
            ToxFunctions.Iterate(tox);
            return TimeSpan.FromMilliseconds(ToxFunctions.IterationInterval(tox));
        }

        private void Loop()
        {
            _cancelTokenSource = new CancellationTokenSource();
            _running = true;

            Task.Factory.StartNew(async () =>
            {
                while (_running && !_disposed)
                {
                    if (_cancelTokenSource.IsCancellationRequested)
                        break;

                    await Task.Delay(DoIterate());
                }
            }, _cancelTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        /// <summary>
        /// Adds a friend to the friend list and sends a friend request.
        /// </summary>
        /// <param name="id">The Tox id of the friend to add.</param>
        /// <param name="message">The message that will be sent along with the friend request.</param>
        /// <param name="error"></param>
        /// <returns>The friend number.</returns>
        public uint AddFriend(ToxId id, string message, out ToxErrorFriendAdd error)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            ThrowIfDisposed();
            byte[] msg = ToxEncoding.GetBytes(message);
            error = ToxErrorFriendAdd.Ok;
            return ToxFunctions.Friend.Add(tox, id.Bytes, msg, (uint)msg.Length, ref error);
        }

        /// <summary>
        /// Adds a friend to the friend list without sending a friend request.
        /// This method should be used to accept friend requests.
        /// </summary>
        /// <param name="publicKey">The public key of the friend to add.</param>
        /// <param name="error"></param>
        /// <returns>The friend number.</returns>
        public uint AddFriendNoRequest(ToxKey publicKey, out ToxErrorFriendAdd error)
        {
            if (publicKey == null)
            {
                throw new ArgumentNullException(nameof(publicKey));
            }

            ThrowIfDisposed();
            error = ToxErrorFriendAdd.Ok;
            return ToxFunctions.Friend.AddNoRequest(tox, publicKey.GetBytes(), ref error);
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
            return ToxFunctions.AddTcpRelay(tox, node.Address, (ushort)node.Port, node.PublicKey.GetBytes(), ref error);
        }

        /// <summary>
        /// Adds a node as a TCP relay. 
        /// This method can be used to initiate TCP connections to different ports on the same bootstrap node, or to add TCP relays without using them as bootstrap nodes.
        /// </summary>
        /// <param name="node">The node to add.</param>
        /// <returns>True on success.</returns>
        public bool AddTcpRelay(ToxNode node)
        {
            var error = ToxErrorBootstrap.Ok;
            return AddTcpRelay(node, out error);
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
            return ToxFunctions.Bootstrap(tox, node.Address, (ushort)node.Port, node.PublicKey.GetBytes(), ref error);
        }

        /// <summary>
        /// Attempts to bootstrap this Tox instance with a ToxNode. A 'getnodes' request is sent to the given node.
        /// </summary>
        /// <param name="node">The node to bootstrap off of.</param>
        /// <returns>True if the 'getnodes' request was sent successfully.</returns>
        public bool Bootstrap(ToxNode node)
        {
            var error = ToxErrorBootstrap.Ok;
            return Bootstrap(node, out error);
        }

        /// <summary>
        /// Checks if there exists a friend with given friendNumber.
        /// </summary>
        /// <param name="friendNumber">The friend number to check.</param>
        /// <returns>True if the friend exists.</returns>
        public bool FriendExists(uint friendNumber)
        {
            ThrowIfDisposed();
            return ToxFunctions.Friend.Exists(tox, friendNumber);
        }

        /// <summary>
        /// Retrieves the friendNumber associated with the specified public key.
        /// </summary>
        /// <param name="publicKey">The public key to look for.</param>
        /// <param name="error"></param>
        /// <returns>The friend number on success.</returns>
        public uint GetFriendByPublicKey(ToxKey publicKey, out ToxErrorFriendByPublicKey error)
        {
            ThrowIfDisposed();

            if (publicKey == null)
                throw new ArgumentNullException("publicKey");

            error = ToxErrorFriendByPublicKey.Ok;
            return ToxFunctions.Friend.ByPublicKey(tox, publicKey.GetBytes(), ref error);
        }

        /// <summary>
        /// Retrieves the friendNumber associated with the specified public key.
        /// </summary>
        /// <param name="publicKey">The public key to look for.</param>
        /// <returns>The friend number on success.</returns>
        public uint GetFriendByPublicKey(ToxKey publicKey)
        {
            var error = ToxErrorFriendByPublicKey.Ok;
            return GetFriendByPublicKey(publicKey, out error);
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

            if (!ToxFunctions.Friend.GetPublicKey(tox, friendNumber, address, ref error))
                return null;

            return new ToxKey(ToxKeyType.Public, address);
        }

        /// <summary>
        /// Retrieves a friend's public key.
        /// </summary>
        /// <param name="friendNumber">The friend number to retrieve the public key of.</param>
        /// <returns>The friend's public key on success.</returns>
        public ToxKey GetFriendPublicKey(uint friendNumber)
        {
            var error = ToxErrorFriendGetPublicKey.Ok;
            return GetFriendPublicKey(friendNumber, out error);
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
            return ToxFunctions.Self.SetTyping(tox, friendNumber, isTyping, ref error);
        }

        /// <summary>
        /// Sets the typing status of this Tox instance for a friend.
        /// </summary>
        /// <param name="friendNumber">The friend number to set the typing status for.</param>
        /// <param name="isTyping">Whether or not we're typing.</param>
        /// <returns>True on success.</returns>
        public bool SetTypingStatus(uint friendNumber, bool isTyping)
        {
            var error = ToxErrorSetTyping.Ok;
            return SetTypingStatus(friendNumber, isTyping, out error);
        }

        /// <summary>
        /// Sends a message to a friend.
        /// </summary>
        /// <param name="friendNumber">The friend number to send the message to.</param>
        /// <param name="message">The message to be sent. Maximum length: <see cref="ToxConstants.MaxMessageLength"/></param>
        /// <param name="type">The type of this message.</param>
        /// <param name="error"></param>
        /// <returns>Message ID on success.</returns>
        public uint SendMessage(uint friendNumber, string message, ToxMessageType type, out ToxErrorSendMessage error)
        {
            ThrowIfDisposed();

            byte[] bytes = ToxEncoding.GetBytes(message);
            error = ToxErrorSendMessage.Ok;

            return ToxFunctions.Friend.SendMessage(tox, friendNumber, type, bytes, (uint)bytes.Length, ref error);
        }

        /// <summary>
        /// Sends a message to a friend.
        /// </summary>
        /// <param name="friendNumber">The friend number to send the message to.</param>
        /// <param name="message">The message to be sent. Maximum length: <see cref="ToxConstants.MaxMessageLength"/></param>
        /// <param name="type">The type of this message.</param>
        /// <returns>Message ID on success.</returns>
        public uint SendMessage(uint friendNumber, string message, ToxMessageType type)
        {
            var error = ToxErrorSendMessage.Ok;
            return SendMessage(friendNumber, message, type, out error);
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
            return ToxFunctions.Friend.Delete(tox, friendNumber, ref error);
        }

        /// <summary>
        /// Deletes a friend from the friend list.
        /// </summary>
        /// <param name="friendNumber">The friend number to be deleted.</param>
        /// <returns>True on success.</returns>
        public bool DeleteFriend(uint friendNumber)
        {
            var error = ToxErrorFriendDelete.Ok;
            return DeleteFriend(friendNumber, out error);
        }

        /// <summary>
        /// Retrieves a ToxData object that contains the profile data of this Tox instance.
        /// </summary>
        /// <returns></returns>
        public ToxData GetData()
        {
            ThrowIfDisposed();

            byte[] bytes = new byte[ToxFunctions.GetSaveDataSize(tox)];
            ToxFunctions.GetSaveData(tox, bytes);

            return ToxData.FromBytes(bytes);
        }

        /// <summary>
        /// Retrieves a ToxData object that contains the profile data of this Tox instance, encrypted with the provided key.
        /// </summary>
        /// <param name="key">The key to encrypt the Tox data with.</param>
        /// <returns></returns>
        public ToxData GetData(ToxEncryptionKey key)
        {
            ThrowIfDisposed();

            var data = GetData();
            byte[] encrypted = ToxEncryption.EncryptData(data.Bytes, key);

            return ToxData.FromBytes(encrypted);
        }

        public ToxData GetData(string password)
        {
            ThrowIfDisposed();

            var data = GetData();
            byte[] encrypted = ToxEncryption.EncryptData(data.Bytes, password);

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
            ToxFunctions.Self.GetSecretKey(tox, key);

            return new ToxKey(ToxKeyType.Secret, key);
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
            uint size = ToxFunctions.Friend.GetNameSize(tox, friendNumber, ref error);

            if (error != ToxErrorFriendQuery.Ok)
                return string.Empty;

            byte[] name = new byte[size];
            if (!ToxFunctions.Friend.GetName(tox, friendNumber, name, ref error))
                return string.Empty;

            return ToxEncoding.GetString(name, 0, name.Length);
        }

        /// <summary>
        /// Retrieves the name of a friend.
        /// </summary>
        /// <param name="friendNumber">The friend number to retrieve the name of.</param>
        /// <returns>The friend's name on success.</returns>
        public string GetFriendName(uint friendNumber)
        {
            var error = ToxErrorFriendQuery.Ok;
            return GetFriendName(friendNumber, out error);
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
            uint size = ToxFunctions.Friend.GetStatusMessageSize(tox, friendNumber, ref error);

            if (error != ToxErrorFriendQuery.Ok)
                return string.Empty;

            byte[] message = new byte[size];
            if (!ToxFunctions.Friend.GetStatusMessage(tox, friendNumber, message, ref error))
                return string.Empty;

            return ToxEncoding.GetString(message, 0, message.Length);
        }

        /// <summary>
        /// Retrieves the status message of a friend.
        /// </summary>
        /// <param name="friendNumber">The friend number to retrieve the status message of.</param>
        /// <returns>The friend's status message on success.</returns>
        public string GetFriendStatusMessage(uint friendNumber)
        {
            var error = ToxErrorFriendQuery.Ok;
            return GetFriendStatusMessage(friendNumber, out error);
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
            return ToxFunctions.Self.GetUdpPort(tox, ref error);
        }

        /// <summary>
        /// Retrieves the UDP port this instance of Tox is bound to.
        /// </summary>
        /// <returns>The UDP port on success.</returns>
        public int GetUdpPort()
        {
            var error = ToxErrorGetPort.Ok;
            return GetUdpPort(out error);
        }

        /// <summary>
        /// Retrieves the TCP port this instance of Tox is bound to.
        /// </summary>
        /// <param name="error"></param>
        /// <returns>The TCP port on success.</returns>
        public int GetTcpPort(out ToxErrorGetPort error)
        {
            ThrowIfDisposed();

            error = ToxErrorGetPort.Ok;
            return ToxFunctions.Self.GetTcpPort(tox, ref error);
        }

        /// <summary>
        /// Retrieves the TCP port this instance of Tox is bound to.
        /// </summary>
        /// <returns>The TCP port on success.</returns>
        public int GetTcpPort()
        {
            var error = ToxErrorGetPort.Ok;
            return GetTcpPort(out error);
        }

        /// <summary>
        /// Sets the nospam value for this Tox instance.
        /// </summary>
        /// <param name="nospam">The nospam value to set.</param>
        public void SetNospam(int nospam)
        {
            ThrowIfDisposed();

            ToxFunctions.Self.SetNospam(tox, ToxTools.Map(nospam));
        }

        /// <summary>
        /// Retrieves the nospam value of this Tox instance.
        /// </summary>
        /// <returns>The nospam value.</returns>
        public int GetNospam()
        {
            ThrowIfDisposed();

            return ToxTools.Map(ToxFunctions.Self.GetNospam(tox));
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
            return ToxFunctions.File.Control(tox, friendNumber, fileNumber, control, ref error);
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
        public ToxFileInfo FileSend(uint friendNumber, ToxFileKind kind, long fileSize, string fileName, out ToxErrorFileSend error)
        {
            ThrowIfDisposed();

            error = ToxErrorFileSend.Ok;
            byte[] fileNameBytes = ToxEncoding.GetBytes(fileName);
            int fileNumber = ToxTools.Map(ToxFunctions.File.Send(tox, friendNumber, kind, (ulong)fileSize, null, fileNameBytes, (uint)fileNameBytes.Length, ref error));

            if (error == ToxErrorFileSend.Ok)
                return new ToxFileInfo(fileNumber, FileGetId(friendNumber, fileNumber));

            return null;
        }

        /// <summary>
        /// Send a file transmission request.
        /// </summary>
        /// <param name="friendNumber">The friend number to send the request to.</param>
        /// <param name="kind">The kind of file that will be transferred.</param>
        /// <param name="fileSize">The size of the file that will be transferred.</param>
        /// <param name="fileName">The filename of the file that will be transferred.</param>
        /// <returns>Info about the file transfer on success.</returns>
        public ToxFileInfo FileSend(uint friendNumber, ToxFileKind kind, long fileSize, string fileName)
        {
            var error = ToxErrorFileSend.Ok;
            return FileSend(friendNumber, kind, fileSize, fileName, out error);
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
        public ToxFileInfo FileSend(uint friendNumber, ToxFileKind kind, long fileSize, string fileName, byte[] fileId, out ToxErrorFileSend error)
        {
            ThrowIfDisposed();

            if (fileId.Length != ToxConstants.FileIdLength)
                throw new ArgumentException("fileId should be exactly ToxConstants.FileIdLength bytes long", "fileId");

            error = ToxErrorFileSend.Ok;
            byte[] fileNameBytes = ToxEncoding.GetBytes(fileName);
            int fileNumber = ToxTools.Map(ToxFunctions.File.Send(tox, friendNumber, kind, (ulong)fileSize, fileId, fileNameBytes, (uint)fileNameBytes.Length, ref error));

            if (error == ToxErrorFileSend.Ok)
                return new ToxFileInfo(fileNumber, fileId);

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
        /// <returns>Info about the file transfer on success.</returns>
        public ToxFileInfo FileSend(uint friendNumber, ToxFileKind kind, long fileSize, string fileName, byte[] fileId)
        {
            var error = ToxErrorFileSend.Ok;
            return FileSend(friendNumber, kind, fileSize, fileName, fileId, out error);
        }

        /// <summary>
        /// Sends a file seek control command to a friend for a given file transfer.
        /// </summary>
        /// <param name="friendNumber">The friend to send the seek command to.</param>
        /// <param name="fileNumber">The file transfer that this command is meant for.</param>
        /// <param name="position">The position that the friend should change his stream to.</param>
        /// <param name="error"></param>
        /// <returns>True on success.</returns>
        public bool FileSeek(uint friendNumber, int fileNumber, long position, out ToxErrorFileSeek error)
        {
            ThrowIfDisposed();

            error = ToxErrorFileSeek.Ok;
            return ToxFunctions.File.Seek(tox, friendNumber, ToxTools.Map(fileNumber), (ulong)position, ref error);
        }

        /// <summary>
        /// Sends a file seek control command to a friend for a given file transfer.
        /// </summary>
        /// <param name="friendNumber">The friend to send the seek command to.</param>
        /// <param name="fileNumber">The file transfer that this command is meant for.</param>
        /// <param name="position">The position that the friend should change his stream to.</param>
        /// <returns>True on success.</returns>
        public bool FileSeek(uint friendNumber, int fileNumber, long position)
        {
            var error = ToxErrorFileSeek.Ok;
            return FileSeek(friendNumber, fileNumber, position, out error);
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
            return ToxFunctions.File.SendChunk(tox, friendNumber, fileNumber, position, data, (uint)data.Length, ref error);
        }

        /// <summary>
        /// Retrieves the unique id of a file transfer. This can be used to uniquely identify file transfers across core restarts.
        /// </summary>
        /// <param name="friendNumber">The friend number that's associated with this transfer.</param>
        /// <param name="fileNumber">The target file transfer.</param>
        /// <param name="error"></param>
        /// <returns>File transfer id on success.</returns>
        public byte[] FileGetId(uint friendNumber, int fileNumber, out ToxErrorFileGet error)
        {
            ThrowIfDisposed();

            error = ToxErrorFileGet.Ok;
            byte[] id = new byte[ToxConstants.FileIdLength];

            if (ToxFunctions.File.GetFileId(tox, friendNumber, ToxTools.Map(fileNumber), id, ref error))
            {
                return id;
            }

            return null;
        }

        /// <summary>
        /// Retrieves the unique id of a file transfer. This can be used to uniquely identify file transfers across core restarts.
        /// </summary>
        /// <param name="friendNumber">The friend number that's associated with this transfer.</param>
        /// <param name="fileNumber">The target file transfer.</param>
        /// <returns>File transfer id on success.</returns>
        public byte[] FileGetId(uint friendNumber, int fileNumber)
        {
            var error = ToxErrorFileGet.Ok;
            return FileGetId(friendNumber, fileNumber, out error);
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

            return ToxFunctions.Friend.SendLossyPacket(tox, friendNumber, data, (uint)data.Length, ref error);
        }

        /// <summary>
        /// Sends a custom lossy packet to a friend. 
        /// Lossy packets are like UDP packets, they may never reach the other side, arrive more than once or arrive in the wrong order.
        /// </summary>
        /// <param name="friendNumber">The friend to send the packet to.</param>
        /// <param name="data">The data to send. The first byte must be in the range 200-254. The maximum length of the data is ToxConstants.MaxCustomPacketSize</param>
        /// <returns>True on success.</returns>
        public bool FriendSendLossyPacket(uint friendNumber, byte[] data)
        {
            var error = ToxErrorFriendCustomPacket.Ok;
            return FriendSendLossyPacket(friendNumber, data, out error);
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
            return ToxFunctions.Friend.SendLosslessPacket(tox, friendNumber, data, (uint)data.Length, ref error);
        }

        /// <summary>
        /// Sends a custom lossless packet to a friend.
        /// Lossless packets behave like TCP, they're reliable and arrive in order. The difference is that it's not a stream.
        /// </summary>
        /// <param name="friendNumber">The friend to send the packet to.</param>
        /// <param name="data">The data to send. The first byte must be in the range 160-191. The maximum length of the data is ToxConstants.MaxCustomPacketSize</param>
        /// <returns>True on success.</returns>
        public bool FriendSendLosslessPacket(uint friendNumber, byte[] data)
        {
            var error = ToxErrorFriendCustomPacket.Ok;
            return FriendSendLosslessPacket(friendNumber, data, out error);
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
            return ToxFunctions.Conference.Join(this.tox, friendNumber, cookie, (uint)cookie.Length, ref error);
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
            var size = ToxFunctions.Conference.Peer.GetNameSize(this.tox, conferenceNumber, peerNumber, ref error);
            if (error != ToxErrorConferencePeerQuery.Ok)
            {
                return null;
            }

            byte[] name = new byte[size];
            if (ToxFunctions.Conference.Peer.GetName(this.tox, conferenceNumber, peerNumber, name, ref error))
            {
                return ToxEncoding.GetString(name);
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
            return ToxFunctions.Conference.Peer.Count(this.tox, conferenceNumber, ref error);
        }

        /// <summary>
        /// Deletes a Conference chat.
        /// </summary>
        /// <param name="conferenceNumber">The Conference to delete.</param>
        /// <returns>True on success.</returns>
        public bool DeleteConferenceChat(uint conferenceNumber, out ToxErrorConferenceDelete error)
        {
            ThrowIfDisposed();
            error = ToxErrorConferenceDelete.Ok;
            return ToxFunctions.Conference.Delete(tox, conferenceNumber, ref error);
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
            return ToxFunctions.Conference.Invite(this.tox, friendNumber, conferenceNumber, ref error);
        }

        public static bool ValidMessage(string message)
            => ToxEncoding.GetByteCount(message ?? throw new ArgumentNullException(nameof(message))) < ToxFunctions.Max.MessageLength();

        /// <summary>
        /// Sends a message to a Conference.
        /// </summary>
        /// <param name="conferenceNumber">The Conference to send the message to.</param>
        /// <param name="message">The message to send.</param>
        /// <returns>True on success.</returns>
        public bool SendConferenceMessage(uint conferenceNumber, ToxMessageType type, string message, out ToxErrorConferenceSendMessage error)
        {
            ThrowIfDisposed();
            if (!ValidMessage(message))
            {
                throw new ArgumentException(nameof(message));
            }

            error = ToxErrorConferenceSendMessage.Ok;
            byte[] msg = ToxEncoding.GetBytes(message);
            return ToxFunctions.Conference.SendMessage(this.tox, conferenceNumber, type, msg, (uint)msg.Length, ref error);
        }

        /// <summary>
        /// Creates a new Conference and retrieves the Conference number.
        /// </summary>
        /// <returns>The number of the created Conference on success.</returns>
        public uint NewConference(out ToxErrorConferenceNew error)
        {
            ThrowIfDisposed();
            error = ToxErrorConferenceNew.Ok;
            return ToxFunctions.Conference.New(this.tox, ref error);
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
            return ToxFunctions.Conference.Peer.NumberIsOurs(tox, conferenceNumber, peerNumber, ref error);
        }

        public static bool ValidConferenceTitle(string title)
            => ToxEncoding.GetByteCount(title) < ToxFunctions.Max.NameLength();

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
            byte[] bytes = ToxEncoding.GetBytes(title);
            return ToxFunctions.Conference.SetTitle(tox, conferenceNumber, bytes, (byte)bytes.Length, ref error);
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
            return ToxFunctions.Conference.GetType(tox, conferenceNumber, ref err);
        }

        /// <summary>
        /// Retrieves the title of a Conference.
        /// </summary>
        /// <param name="conferenceNumber">The Conference to retrieve the title of.</param>
        /// <returns>The Conference's title on success.</returns>
        public string GetConferenceTitle(uint conferenceNumber)
        {
            ThrowIfDisposed();

            var err = ToxErrorConferenceTitle.Ok;
            var size = ToxFunctions.Conference.GetTitleSize(this.tox, conferenceNumber, ref err);
            if (err != ToxErrorConferenceTitle.Ok)
            {
                throw new InvalidOperationException();
            }

            byte[] title = new byte[size];
            if (ToxFunctions.Conference.GetTitle(tox, conferenceNumber, title, ref err))
            {
                return ToxEncoding.GetString(title);
            }

            return string.Empty;
        }

        /// <summary>
        /// Retrieves the public key of a peer.
        /// </summary>
        /// <param name="conferenceNumber">The Conference that the peer is in.</param>
        /// <param name="peerNumber">The peer to retrieve the public key of.</param>
        /// <returns>The peer's public key on success.</returns>
        public ToxKey GetConferencePeerPublicKey(uint conferenceNumber, uint peerNumber)
        {
            ThrowIfDisposed();

            byte[] key = new byte[ToxConstants.PublicKeySize];

            var err = ToxErrorConferencePeerQuery.Ok;
            if (ToxFunctions.Conference.Peer.GetPublicKey(tox, conferenceNumber, peerNumber, key, ref err) && err == ToxErrorConferencePeerQuery.Ok)
            {
                return new ToxKey(ToxKeyType.Public, key);
            }

            return null;
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
            ulong time = ToxFunctions.Friend.GetLastOnline(tox, friendNumber, ref error);

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

        #region Events
        private readonly ToxCallbackHandler<ToxEventArgs.FriendRequestEventArgs, ToxDelegates.CallbackFriendRequestDelegate> friendRequest
            = new ToxCallbackHandler<ToxEventArgs.FriendRequestEventArgs, ToxDelegates.CallbackFriendRequestDelegate>(ToxCallbacks.Friend.FriendRequest, cb =>
                 (tox, publicKey, message, length, userData) =>
                        cb(new ToxEventArgs.FriendRequestEventArgs(new ToxKey(ToxKeyType.Public, ToxTools.HexBinToString(publicKey)), ToxEncoding.GetString(message, 0, (int)length))));

        /// <summary>
        /// Occurs when a friend request is received.
        /// Friend requests should be accepted with AddFriendNoRequest.
        /// </summary>
        public event EventHandler<ToxEventArgs.FriendRequestEventArgs> OnFriendRequestReceived {
            add => this.friendRequest.Add(this, value);
            remove => this.friendRequest.Remove(this, value);
        }

        private readonly ToxCallbackHandler<ToxEventArgs.FriendMessageEventArgs, ToxDelegates.CallbackFriendMessageDelegate> friendMessage
          = new ToxCallbackHandler<ToxEventArgs.FriendMessageEventArgs, ToxDelegates.CallbackFriendMessageDelegate>(ToxCallbacks.Friend.Message, cb =>
                (tox, friendNumber, type, message, length, userdata) =>
                    cb(new ToxEventArgs.FriendMessageEventArgs(friendNumber, ToxEncoding.GetString(message, 0, (int)length), type)));

        /// <summary>
        /// Occurs when a message is received from a friend.
        /// </summary>
        public event EventHandler<ToxEventArgs.FriendMessageEventArgs> OnFriendMessageReceived {
            add => this.friendMessage.Add(this, value);
            remove => this.friendMessage.Remove(this, value);
        }

        private readonly ToxCallbackHandler<ToxEventArgs.NameChangeEventArgs, ToxDelegates.CallbackNameChangeDelegate> friendNameChange
          = new ToxCallbackHandler<ToxEventArgs.NameChangeEventArgs, ToxDelegates.CallbackNameChangeDelegate>(ToxCallbacks.Friend.NameChange, cb =>
                    (tox, friendNumber, newName, length, userData) =>
                            cb(new ToxEventArgs.NameChangeEventArgs(friendNumber, ToxEncoding.GetString(newName, 0, (int)length))));

        /// <summary>
        /// Occurs when a friend has changed his/her name.
        /// </summary>
        public event EventHandler<ToxEventArgs.NameChangeEventArgs> OnFriendNameChanged {
            add => this.friendNameChange.Add(this, value);
            remove => this.friendNameChange.Remove(this, value);
        }

        private readonly ToxCallbackHandler<ToxEventArgs.StatusMessageEventArgs, ToxDelegates.CallbackStatusMessageDelegate> friendStatusMessageChange
          = new ToxCallbackHandler<ToxEventArgs.StatusMessageEventArgs, ToxDelegates.CallbackStatusMessageDelegate>(ToxCallbacks.Friend.StatusMessageChange, cb =>
                    (tox, friendNumber, newStatus, length, userData) =>
                            cb(new ToxEventArgs.StatusMessageEventArgs(friendNumber, ToxEncoding.GetString(newStatus, 0, (int)length))));

        /// <summary>
        /// Occurs when a friend has changed their status message.
        /// </summary>
        public event EventHandler<ToxEventArgs.StatusMessageEventArgs> OnFriendStatusMessageChanged {
            add => this.friendStatusMessageChange.Add(this, value);
            remove => this.friendStatusMessageChange.Remove(this, value);
        }

        private readonly ToxCallbackHandler<ToxEventArgs.StatusEventArgs, ToxDelegates.CallbackUserStatusDelegate> friendStatusChange
          = new ToxCallbackHandler<ToxEventArgs.StatusEventArgs, ToxDelegates.CallbackUserStatusDelegate>(ToxCallbacks.Friend.StatusChange, cb =>
                    (tox, friendNumber, status, userData) =>
                            cb(new ToxEventArgs.StatusEventArgs(friendNumber, status)));

        /// <summary>
        /// Occurs when a friend has changed their user status.
        /// </summary>
        public event EventHandler<ToxEventArgs.StatusEventArgs> OnFriendStatusChanged {
            add => this.friendStatusChange.Add(this, value);
            remove => this.friendStatusChange.Remove(this, value);
        }

        private readonly ToxCallbackHandler<ToxEventArgs.TypingStatusEventArgs, ToxDelegates.CallbackTypingChangeDelegate> friendTypingChange
          = new ToxCallbackHandler<ToxEventArgs.TypingStatusEventArgs, ToxDelegates.CallbackTypingChangeDelegate>(ToxCallbacks.Friend.TypingChange, cb =>
                    (tox, friendNumber, typing, userData) => cb(new ToxEventArgs.TypingStatusEventArgs(friendNumber, typing)));

        /// <summary>
        /// Occurs when a friend's typing status has changed.
        /// </summary>
        public event EventHandler<ToxEventArgs.TypingStatusEventArgs> OnFriendTypingChanged {
            add => this.friendTypingChange.Add(this, value);
            remove => this.friendTypingChange.Remove(this, value);
        }

        private readonly ToxCallbackHandler<ToxEventArgs.ConnectionStatusEventArgs, ToxDelegates.CallbackConnectionStatusDelegate> connectionStatusChange
          = new ToxCallbackHandler<ToxEventArgs.ConnectionStatusEventArgs, ToxDelegates.CallbackConnectionStatusDelegate>(ToxCallbacks.Self.ConnectionStatus, cb =>
                    (tox, status, userData) => cb(new ToxEventArgs.ConnectionStatusEventArgs(status)));

        /// <summary>
        /// Occurs when the connection status of this Tox instance has changed.
        /// </summary>
        public event EventHandler<ToxEventArgs.ConnectionStatusEventArgs> OnConnectionStatusChanged {
            add => this.connectionStatusChange.Add(this, value);
            remove => this.connectionStatusChange.Remove(this, value);
        }

        private readonly ToxCallbackHandler<ToxEventArgs.FriendConnectionStatusEventArgs, ToxDelegates.CallbackFriendConnectionStatusDelegate> friendConnectionStatusChange
          = new ToxCallbackHandler<ToxEventArgs.FriendConnectionStatusEventArgs, ToxDelegates.CallbackFriendConnectionStatusDelegate>(ToxCallbacks.Friend.ConnectionStatusChange, cb =>
                    (tox, friendNumber, status, userData) => cb(new ToxEventArgs.FriendConnectionStatusEventArgs(friendNumber, status)));

        /// <summary>
        /// Occurs when the connection status of a friend has changed.
        /// </summary>
        public event EventHandler<ToxEventArgs.FriendConnectionStatusEventArgs> OnFriendConnectionStatusChanged {
            add => this.friendConnectionStatusChange.Add(this, value);
            remove => this.friendConnectionStatusChange.Remove(this, value);
        }

        private readonly ToxCallbackHandler<ToxEventArgs.ReadReceiptEventArgs, ToxDelegates.CallbackReadReceiptDelegate> readReceipt
          = new ToxCallbackHandler<ToxEventArgs.ReadReceiptEventArgs, ToxDelegates.CallbackReadReceiptDelegate>(ToxCallbacks.Friend.ReadReceipt, cb =>
                    (tox, friendNumber, receipt, userData) => cb(new ToxEventArgs.ReadReceiptEventArgs(friendNumber, receipt)));

        /// <summary>
        /// Occurs when a read receipt is received.
        /// </summary>
        public event EventHandler<ToxEventArgs.ReadReceiptEventArgs> OnReadReceiptReceived {
            add => this.readReceipt.Add(this, value);
            remove => this.readReceipt.Remove(this, value);
        }

        private readonly ToxCallbackHandler<ToxEventArgs.FileControlEventArgs, ToxDelegates.CallbackFileControlDelegate> fileControlReceived
          = new ToxCallbackHandler<ToxEventArgs.FileControlEventArgs, ToxDelegates.CallbackFileControlDelegate>(ToxCallbacks.File.ReceiveControl, cb =>
                    (tox, friendNumber, fileNumber, control, userData) => cb(new ToxEventArgs.FileControlEventArgs(friendNumber, fileNumber, control)));

        /// <summary>
        /// Occurs when a file control is received.
        /// </summary>
        public event EventHandler<ToxEventArgs.FileControlEventArgs> OnFileControlReceived {
            add => this.fileControlReceived.Add(this, value);
            remove => this.fileControlReceived.Remove(this, value);
        }

        private readonly ToxCallbackHandler<ToxEventArgs.FileChunkEventArgs, ToxDelegates.CallbackFileReceiveChunkDelegate> fileChunkReceived
          = new ToxCallbackHandler<ToxEventArgs.FileChunkEventArgs, ToxDelegates.CallbackFileReceiveChunkDelegate>(ToxCallbacks.File.ReceiveChunk, cb =>
                    (tox, friendNumber, fileNumber, position, data, length, userData) =>
                            cb(new ToxEventArgs.FileChunkEventArgs(friendNumber, fileNumber, data, position)));

        /// <summary>
        /// Occurs when a chunk of data from a file transfer is received
        /// </summary>
        public event EventHandler<ToxEventArgs.FileChunkEventArgs> OnFileChunkReceived {
            add => this.fileChunkReceived.Add(this, value);
            remove => this.fileChunkReceived.Remove(this, value);
        }

        private readonly ToxCallbackHandler<ToxEventArgs.FileSendRequestEventArgs, ToxDelegates.CallbackFileReceiveDelegate> fileSendRequestReceived
          = new ToxCallbackHandler<ToxEventArgs.FileSendRequestEventArgs, ToxDelegates.CallbackFileReceiveDelegate>(ToxCallbacks.File.Receive, cb =>
                (tox, friendNumber, fileNumber, kind, fileSize, filename, filenameLength, userData) =>
                        cb(new ToxEventArgs.FileSendRequestEventArgs(friendNumber,
                                                                     fileNumber,
                                                                     kind,
                                                                     fileSize,
                                                                     filename == null ? string.Empty : ToxEncoding.GetString(filename, 0, filename.Length))));

        /// <summary>
        /// Occurs when a new file transfer request has been received.
        /// </summary>
        public event EventHandler<ToxEventArgs.FileSendRequestEventArgs> OnFileSendRequestReceived {
            add => this.fileSendRequestReceived.Add(this, value);
            remove => this.fileSendRequestReceived.Remove(this, value);
        }

        private readonly ToxCallbackHandler<ToxEventArgs.FileRequestChunkEventArgs, ToxDelegates.CallbackFileRequestChunkDelegate> fileChunkRequested
          = new ToxCallbackHandler<ToxEventArgs.FileRequestChunkEventArgs, ToxDelegates.CallbackFileRequestChunkDelegate>(ToxCallbacks.File.ChunkRequest, cb =>
                (tox, friendNumber, fileNumber, position, length, userData) =>
                            cb(new ToxEventArgs.FileRequestChunkEventArgs(friendNumber, fileNumber, position, length)));

        /// <summary>
        /// Occurs when the core requests the next chunk of the file.
        /// </summary>
        public event EventHandler<ToxEventArgs.FileRequestChunkEventArgs> OnFileChunkRequested {
            add => this.fileChunkRequested.Add(this, value);
            remove => this.fileChunkRequested.Remove(this, value);
        }

        private readonly ToxCallbackHandler<ToxEventArgs.FriendPacketEventArgs, ToxDelegates.CallbackFriendPacketDelegate> onFriendLossyPacket
          = new ToxCallbackHandler<ToxEventArgs.FriendPacketEventArgs, ToxDelegates.CallbackFriendPacketDelegate>(ToxCallbacks.Friend.LossyPacket, cb =>
                (tox, friendNumber, data, length, userData) => cb(new ToxEventArgs.FriendPacketEventArgs(friendNumber, data)));

        /// <summary>
        /// Occurs when a lossy packet from a friend is received.
        /// </summary>
        public event EventHandler<ToxEventArgs.FriendPacketEventArgs> OnFriendLossyPacketReceived {
            add => this.onFriendLossyPacket.Add(this, value);
            remove => this.onFriendLossyPacket.Remove(this, value);
        }

        private readonly ToxCallbackHandler<ToxEventArgs.FriendPacketEventArgs, ToxDelegates.CallbackFriendPacketDelegate> onFriendLosslessPacket
          = new ToxCallbackHandler<ToxEventArgs.FriendPacketEventArgs, ToxDelegates.CallbackFriendPacketDelegate>(ToxCallbacks.Friend.LosslessPacket, cb =>
              (tox, friendNumber, data, length, userData) => cb(new ToxEventArgs.FriendPacketEventArgs(friendNumber, data)));

        /// <summary>
        /// Occurs when a lossless packet from a friend is received.
        /// </summary>
        public event EventHandler<ToxEventArgs.FriendPacketEventArgs> OnFriendLosslessPacketReceived {
            add => this.onFriendLosslessPacket.Add(this, value);
            remove => this.onFriendLosslessPacket.Remove(this, value);
        }

        private readonly ToxCallbackHandler<ToxEventArgs.ConferenceMessageEventArgs, ToxDelegates.ConferenceMessageDelegate> conferenceMessage
          = new ToxCallbackHandler<ToxEventArgs.ConferenceMessageEventArgs, ToxDelegates.ConferenceMessageDelegate>(ToxCallbacks.Conference.Message, cb =>
                     (tox, conferenceNumber, peerNumber, type, message, length, userData) =>
                            cb(new ToxEventArgs.ConferenceMessageEventArgs(conferenceNumber, peerNumber, ToxEncoding.GetString(message, 0, (int)length), type)));

        /// <summary>
        /// Occurs when a message is received from a Conference.
        /// </summary>
        public event EventHandler<ToxEventArgs.ConferenceMessageEventArgs> OnConferenceMessage {
            add => this.conferenceMessage.Add(this, value);
            remove => this.conferenceMessage.Remove(this, value);
        }

        private readonly ToxCallbackHandler<ToxEventArgs.ConferenceInviteEventArgs, ToxDelegates.ConferenceInviteDelegate> conferenceInvite
          = new ToxCallbackHandler<ToxEventArgs.ConferenceInviteEventArgs, ToxDelegates.ConferenceInviteDelegate>(ToxCallbacks.Conference.Invite, cb =>
               (tox, friendNumber, type, data, length, userData) => cb(new ToxEventArgs.ConferenceInviteEventArgs(friendNumber, type, data)));

        /// <summary>
        /// Occurs when a friend has sent an invite to a Conference.
        /// </summary>
        public event EventHandler<ToxEventArgs.ConferenceInviteEventArgs> OnConferenceInvite {
            add => this.conferenceInvite.Add(this, value);
            remove => this.conferenceInvite.Remove(this, value);
        }

        private readonly ToxCallbackHandler<ToxEventArgs.ConferenceTitleEventArgs, ToxDelegates.ConferenceTitleDelegate> conferenceTitleChange
          = new ToxCallbackHandler<ToxEventArgs.ConferenceTitleEventArgs, ToxDelegates.ConferenceTitleDelegate>(ToxCallbacks.Conference.Title, cb =>
              (tox, conferenceNumber, peerNumber, title, length, userData) =>
                            cb(new ToxEventArgs.ConferenceTitleEventArgs(conferenceNumber, peerNumber, ToxEncoding.GetString(title))));

        /// <summary>
        /// Occurs when the title of a Conference is changed.
        /// </summary>
        public event EventHandler<ToxEventArgs.ConferenceTitleEventArgs> OnConferenceTitleChanged {
            add => this.conferenceTitleChange.Add(this, value);
            remove => this.conferenceTitleChange.Remove(this, value);
        }

        private readonly ToxCallbackHandler<ToxEventArgs.ConferenceConnectedEventArgs, ToxDelegates.ConferenceConnectedDelegate> conferenceConnected
          = new ToxCallbackHandler<ToxEventArgs.ConferenceConnectedEventArgs, ToxDelegates.ConferenceConnectedDelegate>(ToxCallbacks.Conference.Connected, cb =>
                (tox, conferenceNumber, userData) => cb(new ToxEventArgs.ConferenceConnectedEventArgs(conferenceNumber)));

        /// <summary>
        /// Occurs after a conference was joined and successfully connected.
        /// </summary>
        public event EventHandler<ToxEventArgs.ConferenceConnectedEventArgs> OnConferenceConnected {
            add => this.conferenceConnected.Add(this, value);
            remove => this.conferenceConnected.Remove(this, value);
        }

        private readonly ToxCallbackHandler<ToxEventArgs.ConferencePeerNameEventArgs, ToxDelegates.ConferencePeerNameDelegate> conferencePeerName
          = new ToxCallbackHandler<ToxEventArgs.ConferencePeerNameEventArgs, ToxDelegates.ConferencePeerNameDelegate>(ToxCallbacks.Conference.PeerName, cb =>
           (tox, conferenceNumber, peerNumber, name, length, userData) => cb(new ToxEventArgs.ConferencePeerNameEventArgs(conferenceNumber, peerNumber, ToxEncoding.GetString(name))));

        /// <summary>
        /// This event is triggered when a peer changes their name.
        /// </summary>
        public event EventHandler<ToxEventArgs.ConferencePeerNameEventArgs> ConferencePeerNameChanged {
            add => this.conferencePeerName.Add(this, value);
            remove => this.conferencePeerName.Remove(this, value);
        }

        private readonly ToxCallbackHandler<ToxEventArgs.ConferencePeerListEventArgs, ToxDelegates.ConferencePeerListChangedDelegate> conferencePeerList
          = new ToxCallbackHandler<ToxEventArgs.ConferencePeerListEventArgs, ToxDelegates.ConferencePeerListChangedDelegate>(ToxCallbacks.Conference.PeerListChanged, cb =>
              (tox, conferenceNumber, userdata) => cb(new ToxEventArgs.ConferencePeerListEventArgs(conferenceNumber)));

        public event EventHandler<ToxEventArgs.ConferencePeerListEventArgs> ConferencePeerListChanged {
            add => this.conferencePeerList.Add(this, value);
            remove => this.conferencePeerList.Remove(this, value);
        }
        #endregion

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }
    }
}
