using SharpTox.Core;
using System;

namespace SharpTox.Encryption
{
    public sealed class ToxEncryptionKey : IDisposable
    {
        private ToxEncryptionKeyHandle handle;

        public ToxEncryptionKey(string passphrase, byte[] salt = null)
        {
            if (string.IsNullOrEmpty(passphrase))
            {
                throw new ArgumentException(nameof(passphrase));
            }

            ToxErrorKeyDerivation error;
            var handle = salt == null ? Derive(passphrase, out error) : Derive(passphrase, salt, out error);
            if (handle == null || error != ToxErrorKeyDerivation.Ok)
            {
                throw new Exception("Could not derive key from passphrase");
            }

            this.handle = handle;
        }

        public byte[] Encrypt(byte[] data, out ToxErrorEncryption error)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            byte[] output = new byte[data.Length + ToxEncryptionConstants.EncryptionExtraLength];

            error = ToxErrorEncryption.Ok;

            var success = ToxEncryptionFunctions.Key.Encrypt(this.handle, data, (uint)data.Length, output, ref error);
            if (success && error == ToxErrorEncryption.Ok)
            {
                return output;
            }

            return null;
        }

        public byte[] Decrypt(byte[] data, out ToxErrorDecryption error)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            byte[] plain = new byte[data.Length - ToxEncryptionConstants.EncryptionExtraLength];
            error = ToxErrorDecryption.Ok;
            var success = ToxEncryptionFunctions.Key.Decrypt(this.handle, data, (uint)data.Length, plain, ref error);

            if (success && error == ToxErrorDecryption.Ok)
            {
                return plain;
            }

            return null;
        }


        private static ToxEncryptionKeyHandle Derive(string passphrase, out ToxErrorKeyDerivation error)
        {
            // warning (no salt means one gets pseudo-random generated) ?
            byte[] binaryPassphrase = ToxConstants.Encoding.GetBytes(passphrase);
            error = ToxErrorKeyDerivation.Ok;
            return ToxEncryptionFunctions.Key.Derive(binaryPassphrase, (uint)binaryPassphrase.Length, ref error);
        }

        private static ToxEncryptionKeyHandle Derive(string passphrase, byte[] salt, out ToxErrorKeyDerivation error)
        {
            if (salt.Length != ToxEncryptionConstants.SaltLength)
            {
                throw new ArgumentException(nameof(salt));
            }

            byte[] binaryPassphrase = ToxConstants.Encoding.GetBytes(passphrase);
            error = ToxErrorKeyDerivation.Ok;
            return ToxEncryptionFunctions.Key.DeriveWithSalt(binaryPassphrase, (uint)binaryPassphrase.Length, salt, ref error);
        }

        #region IDisposable Support
        private bool disposed = false;

        public void Dispose() => Dispose(true);

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects).
                    this.handle.Dispose();
                    this.handle = null;
                }

                // free unmanaged resources (unmanaged objects) and override a finalizer below.

                disposed = true;
            }
        }
        #endregion
    }
}
