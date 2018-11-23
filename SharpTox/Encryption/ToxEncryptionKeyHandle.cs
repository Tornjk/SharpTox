using Microsoft.Win32.SafeHandles;

namespace SharpTox.Encryption
{
    class ToxEncryptionKeyHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public ToxEncryptionKeyHandle(bool ownsHandle) : base(ownsHandle)
        {
        }

        protected override bool ReleaseHandle()
        {
            ToxEncryptionFunctions.Key.Free(this.handle);
            return true;
        }
    }
}
