using SharpTox.Core.Interfaces;

namespace SharpTox.Core
{
    /// <summary>
    /// Represent information about a file transfer.
    /// </summary>
    sealed class ToxFileInfo : IToxFileInfo
    {
        private readonly byte[] id;

        /// <inheritdoc/>
        public uint Number { get; }

        /// <inheritdoc/>
        public byte[] Id => (byte[])id.Clone();

        public ToxFileInfo(uint number, byte[] id)
        {
            this.Number = number;
            this.id = id;
        }
    }
}
