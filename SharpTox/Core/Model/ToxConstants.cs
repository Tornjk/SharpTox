﻿using System;
using System.Text;

namespace SharpTox.Core
{
    /// <summary>
    /// Represents a collection of tox constants.
    /// </summary>
    public static class ToxConstants
    {
        /// <summary>
        /// The size of a public key.
        /// </summary>
        public const int PublicKeySize = 32;

        /// <summary>
        /// The size of a secret key.
        /// </summary>
        public const int SecretKeySize = 32;

        /// <summary>
        /// The size of a Tox Conference unique id in bytes.
        /// </summary>
        public const int ConferenceIdSize = 32;

        /// <summary>
        /// The size of the nospam in bytes when written in a Tox address.
        /// </summary>
        public const int NospamSize = sizeof(UInt32);

        /// <summary>
        /// The size of the checksum in a Tox Address
        /// </summary>
        public const int ChecksumSize = sizeof(UInt16);

        /// <summary>
        /// The size of an address.
        /// </summary>
        public const int AddressSize = PublicKeySize + NospamSize + ChecksumSize;

        /// <summary>
        /// The maximum message length in bytes.
        /// </summary>
        public const int MaxMessageLength = 1372;

        /// <summary>
        /// The number of bytes in a hash generated by tox_hash
        /// </summary>
        public const int HashLength = 32;

        /// <summary>
        /// The number of bytes in a file id.
        /// </summary>
        public const int FileIdLength = 32;

        internal static readonly Encoding Encoding = Encoding.UTF8;

        //Constants for the the tox data file
        internal const uint Cookie = 0x15ed1b1f;
        internal const uint CookieInner = 0x01ce;
    }
}
