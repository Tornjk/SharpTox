namespace SharpTox.Core
{
    /// <summary>
    /// Represent information about a file transfer.
    /// </summary>
    public sealed class ToxFileInfo
    {
        private readonly byte[] id;

        /// <summary>
        /// The number of this file transfer.
        /// </summary>
        public uint Number { get; }

        /// <summary>
        /// The unique ID if this file transfer. This can be used to resume file transfer across restarts.
        /// </summary>
        public byte[] Id => (byte[])id.Clone();

        internal ToxFileInfo(uint number, byte[] id)
        {
            this.Number = number;
            this.id = id;
        }
    }
}
