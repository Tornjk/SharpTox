﻿using SharpTox.Core.Interface;
using System.Diagnostics;

namespace SharpTox.Av
{
    /// <summary>
    /// Represents a version of ToxAv.
    /// </summary>
    [DebuggerDisplay("{Major}.{Minor}.{Patch}")]
    public sealed class ToxAvVersion : IToxVersion
    {
        /// <summary>
        /// The major version number. Incremented when the API or ABI changes in an incompatible way.
        /// </summary>
        public uint Major { get; }

        /// <summary>
        /// The minor version number. Incremented when functionality is added without breaking the API or ABI. 
        /// Set to 0 when the major version number is incremented.
        /// </summary>
        public uint Minor { get; }

        /// <summary>
        /// The patch or revision number. Incremented when bugfixes are applied without changing any functionality or API or ABI.
        /// </summary>
        public uint Patch { get; }

        /// <summary>
        /// The current version of Tox. Assuming there's a libtox.dll/libtoxav.so in our PATH.
        /// </summary>
        public static ToxAvVersion Current()
            => new ToxAvVersion(ToxAvFunctions.Version.Major(), ToxAvFunctions.Version.Minor(), ToxAvFunctions.Version.Patch());

        /// <summary>
        /// Checks whether or not this version is compatible with the version of ToxAv that we're using.
        /// </summary>
        /// <returns>True if this version is compatible, false if it's not.</returns>
        public bool IsCompatible() => ToxAvFunctions.Version.IsCompatible(this.Major, this.Minor, this.Patch);

        /// <summary>
        /// Initializes a new instance of the ToxAvVersion class.
        /// </summary>
        /// <param name="major">The major version number.</param>
        /// <param name="minor">The minor version number.</param>
        /// <param name="patch">The patch or revision number.</param>
        public ToxAvVersion(uint major, uint minor, uint patch)
        {
            this.Major = major;
            this.Minor = minor;
            this.Patch = patch;
        }
    }
}
