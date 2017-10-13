using System;
using System.Text;
using System.Runtime.InteropServices;

namespace SharpTox.Dns
{
    internal static class ToxDnsFunctions
    {
        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_dns3_new")]
        internal static extern ToxDnsHandle New(byte[] publicKey);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_dns3_kill")]
        internal static extern void Kill(IntPtr dns3Object);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_generate_dns3_string")]
        internal static extern int GenerateDns3String(ToxDnsHandle dns3Object, byte[] str, ushort strMaxLength, ref uint requestId, byte[] name, byte nameLength);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_decrypt_dns3_TXT")]
        internal static extern int DecryptDns3TXT(ToxDnsHandle dns3Object, byte[] toxId, byte[] idRecord, uint idRecordLenght, uint requestId);
    }
}
