using System;
using SharpTox.Core;

namespace SharpTox.Encryption
{
    public static class ToxEncryption
    {
        public static byte[] Encrypt(byte[] data, string password, out ToxErrorEncryption error)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            byte[] cipher = new byte[data.Length + ToxEncryptionConstants.EncryptionExtraLength];
            byte[] passBytes = ToxConstants.Encoding.GetBytes(password);
            error = ToxErrorEncryption.Ok;

            var success = ToxEncryptionFunctions.Pass.Encrypt(data, (uint)data.Length, passBytes, (uint)passBytes.Length, cipher, ref error);
            if (success && error == ToxErrorEncryption.Ok)
            {
                return cipher;
            }

            return null;
        }

        public static byte[] Decrypt(byte[] data, string password, out ToxErrorDecryption error)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            byte[] plain = new byte[data.Length - ToxEncryptionConstants.EncryptionExtraLength];
            byte[] passBytes = ToxConstants.Encoding.GetBytes(password);
            error = ToxErrorDecryption.Ok;

            var success = ToxEncryptionFunctions.Pass.Decrypt(data, (uint)data.Length, passBytes, (uint)passBytes.Length, plain, ref error);
            if (success && error == ToxErrorDecryption.Ok)
            {
                return plain;
            }

            return null;
        }

        public static bool IsDataEncrypted(byte[] data) => ToxEncryptionFunctions.IsDataEncrypted(data);

        public static byte[] GetSalt(byte[] data, out ToxErrorGetSalt error)
        {
            byte[] salt = new byte[ToxEncryptionConstants.SaltLength];

            error = ToxErrorGetSalt.Ok;
            var success = ToxEncryptionFunctions.GetSalt(data, salt, ref error);
            if (success && error == ToxErrorGetSalt.Ok)
            {
                return salt;
            }

            return null;
        }
    }
}
