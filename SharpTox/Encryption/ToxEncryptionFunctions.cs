using System;
using System.Runtime.InteropServices;
using SizeT = System.UInt32;

namespace SharpTox.Encryption
{
    static class ToxEncryptionFunctions
    {
        const string Base = "tox_";

        public static class Pass
        {
            const string Prefix = Base + "pass_";

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Prefix + "encrypt")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern Boolean Encrypt(Byte[] plain, SizeT len, Byte[] passphrase, SizeT passphraseLength, Byte[] cipher, ref ToxErrorEncryption error);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Prefix + "decrypt")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern Boolean Decrypt(Byte[] data, SizeT length, Byte[] passphrase, SizeT passphraseLength, Byte[] output, ref ToxErrorDecryption error);

        }

        public static class Key
        {
            const string Prefix = Base + "pass_key_";

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Prefix + "free")]
            public static extern void Free(IntPtr key);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Prefix + "derive")]
            public static extern ToxEncryptionKeyHandle Derive(Byte[] passphrase, SizeT length, ref ToxErrorKeyDerivation error);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Prefix + "derive_with_salt")]
            public static extern ToxEncryptionKeyHandle DeriveWithSalt(Byte[] passphrase, SizeT length, Byte[] salt, ref ToxErrorKeyDerivation error);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Prefix + "encrypt")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern Boolean Encrypt(ToxEncryptionKeyHandle key, Byte[] plain, SizeT length, Byte[] cipher, ref ToxErrorEncryption error);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Prefix + "decrypt")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern Boolean Decrypt(ToxEncryptionKeyHandle key, Byte[] cipher, SizeT length, Byte[] plain, ref ToxErrorDecryption error);
        }

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Base + "get_salt")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern Boolean GetSalt(Byte[] cipher, Byte[] salt, ref ToxErrorGetSalt error);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Base + "is_data_encrypted")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern Boolean IsDataEncrypted(Byte[] data);
    }
}
