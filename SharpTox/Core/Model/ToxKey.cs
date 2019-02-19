using System;
using System.Linq;

namespace SharpTox.Core
{
    /// <summary>
    /// Represents a 32 byte long tox key (either public or secret).
    /// </summary>
    public sealed class ToxKey
    {
        private readonly byte[] key;

        /// <summary>
        /// The key type (either public or secret).
        /// </summary>
        public ToxKeyType KeyType { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToxKey"/> class
        /// </summary>
        /// <param name="type"></param>
        /// <param name="key"></param>
        public ToxKey(ToxKeyType type, byte[] key)
        {
            if(key.Length != KeySize(type))
            {
                throw new ArgumentException(nameof(key));
            }

            this.KeyType = type;
            this.key = (byte[])key.Clone();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToxKey"/> class
        /// </summary>
        /// <param name="type"></param>
        /// <param name="key"></param>
        public ToxKey(ToxKeyType type, string key) : this(type, ToxTools.StringToHexBin(key))
        {
        }

        /// <summary>
        /// Retrieves a byte array of the tox key.
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes() => (byte[])key.Clone();

        /// <summary>
        /// Retrieves a string of the tox key.
        /// </summary>
        /// <returns></returns>
        public string GetString() => ToxTools.HexBinToString(key);

        public static bool operator ==(ToxKey key1, ToxKey key2)
        {
            if (object.ReferenceEquals(key1, key2))
            {
                return true;
            }

            if ((object)key1 == null ^ (object)key2 == null)
            {
                return false;
            }

            return (key1.key.SequenceEqual(key2.key) && key1.KeyType == key2.KeyType);
        }

        public static bool operator !=(ToxKey key1, ToxKey key2)
        {
            return !(key1 == key2);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            ToxKey key = obj as ToxKey;
            if ((object)key == null)
                return false;

            return this == key;
        }

        public override int GetHashCode() => base.GetHashCode();

        public override string ToString() => ToxTools.HexBinToString(key);

        private static int KeySize(ToxKeyType type)
        {
            switch (type)
            {
                case ToxKeyType.Public:
                    return ToxConstants.PublicKeySize;
                case ToxKeyType.Secret:
                    return ToxConstants.SecretKeySize;
            }

            throw new NotImplementedException();
        }
    }
}
