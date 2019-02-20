namespace SharpTox.Core.Interfaces
{
    /// <summary>
    /// Represents Tox data (unencrypted or encrypted).
    /// </summary>
    public interface IToxData
    {
        /// <summary>
        /// The Tox data in a byte array.
        /// </summary>
        byte[] Bytes { get; }

        /// <summary>
        /// Flag if <see cref="Bytes"/> are encrypted.
        /// </summary>
        bool IsEncrypted { get; }
    }
}