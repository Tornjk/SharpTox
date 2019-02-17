using System.Diagnostics;

namespace SharpTox.Core
{
    /// <summary>
    /// Represents a version of Tox.
    /// </summary>
    [DebuggerDisplay("Version: {Major}.{Minor}.{Patch}")]
    public sealed class ToxVersion
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
        /// The current version of Tox. Assuming there's a libtox.dll/libtoxcore.so in our PATH.
        /// </summary>
        public static ToxVersion Current
               => new ToxVersion(ToxFunctions.Version.Major(), ToxFunctions.Version.Minor(), ToxFunctions.Version.Patch());

        /// <summary>
        /// Checks whether or not this version is compatible with the version of Tox that we're using.
        /// </summary>
        /// <returns>True if this version is compatible, false if it's not.</returns>
        public bool IsCompatible()
            => ToxFunctions.Version.IsCompatible(this.Major, this.Minor, this.Patch);

        /// <summary>
        /// Initializes a new instance of the ToxVersion class.
        /// </summary>
        /// <param name="major">The major version number.</param>
        /// <param name="minor">The minor version number.</param>
        /// <param name="patch">The patch or revision number.</param>
        public ToxVersion(uint major, uint minor, uint patch)
        {
            this.Major = major;
            this.Minor = minor;
            this.Patch = patch;
        }
    }
}
