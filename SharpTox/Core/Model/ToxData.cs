using SharpTox.Core.Interfaces;
using SharpTox.Encryption;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace SharpTox.Core
{
    /// <inheritdoc/>
    class ToxData : IToxData
    {
        private readonly byte[] bytes;

        /// <inheritdoc/>
        public bool IsEncrypted { get; }

        /// <inheritdoc/>
        public byte[] Bytes => (byte[])this.bytes.Clone();

        public ToxData([NotNull] byte[] data)
        {
            this.bytes = data;
            this.IsEncrypted = ToxEncryptionFunctions.IsDataEncrypted(data);
        }

        /// <summary>
        /// Tries to parse this Tox profile.
        /// </summary>
        /// <param name="info"></param>
        /// <returns>Tox profile information.</returns>
        public bool TryParse(out ToxDataInfo info)
        {
            info = ToxDataInfo.FromToxData(this);
            return info != null;
        }

        /// <summary>
        /// Saves this Tox data to a stream.
        /// </summary>
        public bool SaveToStream([NotNull] Stream stream)
        {
            try
            {
                using (var ms = new MemoryStream(this.bytes))
                {
                    ms.CopyTo(stream);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Loads Tox data from a stream and creates a new instance of ToxData.
        /// </summary>
        public static ToxData LoadFromStream([NotNull] Stream stream)
        {
            using(var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return new ToxData(ms.ToArray());
            }
        }

        /// <summary>
        /// Creates a new instance of ToxData from the specified byte array.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static ToxData FromBytes([NotNull]byte[] bytes)
            => new ToxData(bytes);

        public static bool operator ==(ToxData data1, ToxData data2)
        {
            if (object.ReferenceEquals(data1, data2))
                return true;

            if ((object)data1 == null ^ (object)data2 == null)
                return false;

            return data1.bytes.SequenceEqual(data2.bytes);
        }

        public static bool operator !=(ToxData data1, ToxData data2)
        {
            return !(data1 == data2);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            ToxData data = obj as ToxData;
            if ((object)data == null)
                return false;

            return this == data;
        }

        public override int GetHashCode()
            => base.GetHashCode();
    }
}
