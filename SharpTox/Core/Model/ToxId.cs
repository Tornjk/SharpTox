using System;
using System.Linq;

namespace SharpTox.Core
{
    /// <summary>
    /// Represents a Tox ID (38 bytes long)
    /// </summary>
    public class ToxId
    {
        private byte[] id;

        /// <summary>
        /// Retrieves the public key of this Tox ID.
        /// </summary>
        public ToxKey PublicKey
        {
            get
            {
                byte[] key = new byte[ToxConstants.PublicKeySize];
                Array.Copy(id, 0, key, 0, ToxConstants.PublicKeySize);

                return new ToxKey(ToxKeyType.Public, key);
            }
        }

        /// <summary>
        /// Retrieves the Tox ID, represented in an array of bytes.
        /// </summary>
        public byte[] GetBytes() => (byte[])id.Clone();

        /// <summary>
        /// Retrieves the nospam value of this Tox ID.
        /// </summary>
        public uint Nospam
        {
            get
            {
                byte[] nospam = new byte[ToxConstants.NospamSize];
                Array.Copy(id, ToxConstants.PublicKeySize, nospam, 0, ToxConstants.NospamSize);

                return BitConverter.ToUInt32(nospam, 0);
            }
        }

        /// <summary>
        /// Retrieves the checksum of this Tox ID.
        /// </summary>
        public ushort Checksum => GetChecksum(this.id);

        /// <summary>
        /// Initializes a new instance of the ToxId class.
        /// </summary>
        /// <param name="id">A (ToxConstant.AddressSize * 2) character long hexadecimal string, containing a Tox ID.</param>
        public ToxId([NotNull] string id)
            : this(ToxTools.StringToHexBin(id)) { }

        /// <summary>
        /// Initializes a new instance of the ToxId class.
        /// </summary>
        /// <param name="id">A byte array with a length of ToxConstant.AddressSize, containing a Tox ID.</param>
        public ToxId([NotNull] byte[] id)
        {
            if(id.Length != ToxConstants.AddressSize)
            {
                throw new ArgumentException(nameof(id));
            }

            var checksum = CalculateChecksum(id, ToxConstants.PublicKeySize + ToxConstants.NospamSize);
            if(checksum != GetChecksum(id))
            {
                throw new ArgumentException(nameof(id));
            }

            this.id = id;
        }

        /// <summary>
        /// Creates a new tox id with the specified public key and nospam.
        /// </summary>
        /// <param name="publicKey">Public key to create this Tox ID with.</param>
        /// <param name="nospam">Nospam value to create this Tox ID with.</param>
        public ToxId([NotNull] ToxKey publicKey, uint nospam)
        {
            if(publicKey.KeyType != ToxKeyType.Public)
            {
                throw new ArgumentException(nameof(publicKey));
            }

            byte[] id = new byte[ToxConstants.AddressSize];

            Array.Copy(publicKey.GetBytes(), 0, id, 0, ToxConstants.PublicKeySize);
            Array.Copy(BitConverter.GetBytes(nospam), 0, id, ToxConstants.PublicKeySize, ToxConstants.NospamSize);

            ushort checksum = CalculateChecksum(id, ToxConstants.PublicKeySize + ToxConstants.NospamSize);
            Array.Copy(BitConverter.GetBytes(checksum), 0, id, ToxConstants.PublicKeySize + ToxConstants.NospamSize, ToxConstants.ChecksumSize);

            this.id = id;
        }

        public static bool operator ==(ToxId id1, ToxId id2)
        {
            if (object.ReferenceEquals(id1, id2))
                return true;

            if ((object)id1 == null ^ (object)id2 == null)
                return false;

            return (id1.id.SequenceEqual(id2.id));
        }

        public static bool operator !=(ToxId id1, ToxId id2)
        {
            return !(id1 == id2);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            ToxId id = obj as ToxId;
            if ((object)id == null)
                return false;

            return this == id;
        }

        public override int GetHashCode()
            => base.GetHashCode();

        public override string ToString()
            => ToxTools.HexBinToString(id);

        /// <summary>
        /// Checks whether or not the given Tox ID is valid.
        /// </summary>
        /// <param name="id">A (ToxConstant.AddressSize * 2) character long hexadecimal string, containing a Tox ID.</param>
        /// <returns>True if the ID is valid, false if the ID is invalid.</returns>
        public static bool IsValid(string id)
        {
            if (!ToxTools.ValidHexString(id))
            {
                return false;
            }

            return IsValid(ToxTools.StringToHexBin(id));
        }

        /// <summary>
        /// Checks whether or not the given Tox ID is valid.
        /// </summary>
        /// <param name="id">A byte array with a length of ToxConstant.AddressSize, containing a Tox ID.</param>
        /// <returns>True if the ID is valid, false if the ID is invalid.</returns>
        public static bool IsValid(byte[] id)
        {
            if (id == null || id.Length < ToxConstants.AddressSize)
            {
                return false;
            }

            byte[] checksum = new byte[ToxConstants.ChecksumSize];
            int index = ToxConstants.PublicKeySize + ToxConstants.NospamSize;

            Array.Copy(id, index, checksum, 0, ToxConstants.ChecksumSize);
            var containedChecksum = BitConverter.ToUInt16(checksum, 0);

            return CalculateChecksum(id, index) == containedChecksum;
        }

        private static ushort CalculateChecksum(byte[] address, int length)
        {
            byte[] checksum = new byte[ToxConstants.ChecksumSize];

            for (uint i = 0; i < length; i++)
            {
                checksum[i % 2] ^= address[i];
            }

            return BitConverter.ToUInt16(checksum, 0);
        }

        private static ushort GetChecksum(byte[] id)
        {
            byte[] checksum = new byte[ToxConstants.ChecksumSize];
            Array.Copy(id, ToxConstants.PublicKeySize + ToxConstants.NospamSize, checksum, 0, ToxConstants.ChecksumSize);
            return BitConverter.ToUInt16(checksum, 0);
        }
    }
}
