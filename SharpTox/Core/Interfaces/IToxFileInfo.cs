namespace SharpTox.Core.Interfaces
{
    public interface IToxFileInfo
    {
        /// <summary>
        /// The number of this file transfer.
        /// </summary>
        byte[] Id { get; }

        /// <summary>
        /// The unique ID if this file transfer. This can be used to resume file transfer across restarts.
        /// </summary>
        uint Number { get; }
    }
}