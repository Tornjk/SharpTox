using System;
using System.Runtime.InteropServices;
using SizeT = System.UInt32;

namespace SharpTox.Core
{
    static class ToxDelegates
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CallbackFriendMessageDelegate(ToxHandle tox, UInt32 friendNumber, ToxMessageType type, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] Byte[] message, SizeT length, IntPtr userData);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CallbackFriendRequestDelegate(ToxHandle tox, [MarshalAs(UnmanagedType.LPArray, SizeConst = ToxConstants.PublicKeySize)] Byte[] publicKey, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] Byte[] message, SizeT length, IntPtr userData);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CallbackTypingChangeDelegate(ToxHandle tox, UInt32 friendNumber, Boolean isTyping, IntPtr userData);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CallbackUserStatusDelegate(ToxHandle tox, UInt32 friendNumber, ToxUserStatus status, IntPtr userData);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CallbackStatusMessageDelegate(ToxHandle tox, UInt32 friendNumber, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] Byte[] newStatus, SizeT length, IntPtr userData);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CallbackNameChangeDelegate(ToxHandle tox, UInt32 friendNumber, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] byte[] newName, uint length, IntPtr userData);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CallbackConnectionStatusDelegate(ToxHandle tox, ToxConnectionStatus status, IntPtr userData);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CallbackFriendConnectionStatusDelegate(ToxHandle tox, UInt32 friendNumber, ToxConnectionStatus status, IntPtr userData);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CallbackReadReceiptDelegate(ToxHandle tox, UInt32 friendNumber, uint messageId, IntPtr userData);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CallbackFileReceiveChunkDelegate(ToxHandle tox, UInt32 friendNumber, UInt32 fileNumber, UInt64 position, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)] Byte[] data, SizeT length, IntPtr userData);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CallbackFileControlDelegate(ToxHandle tox, UInt32 friendNumber, UInt32 fileNumber, ToxFileControl control, IntPtr userData);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CallbackFileReceiveDelegate(ToxHandle tox, UInt32 friendNumber, UInt32 fileNumber, ToxFileKind kind, UInt64 fizeSize, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 6)] Byte[] filename, SizeT filenameLength, IntPtr userData);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CallbackFileRequestChunkDelegate(ToxHandle tox, UInt32 friendNumber, UInt32 fileNumber, UInt64 position, SizeT length, IntPtr userData);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CallbackFriendPacketDelegate(ToxHandle tox, UInt32 friendNumber, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] byte[] data, uint length, IntPtr userData);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CallbackGroupInviteDelegate(ToxHandle tox, int friendNumber, byte type, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] byte[] data, ushort length, IntPtr userData);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CallbackGroupMessageDelegate(ToxHandle tox, int groupNumber, int peerNumber, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] byte[] message, ushort length, IntPtr userData);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CallbackGroupActionDelegate(ToxHandle tox, int groupNumber, int peerNumber, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] byte[] action, ushort length, IntPtr userData);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CallbackGroupNamelistChangeDelegate(ToxHandle tox, int groupNumber, int peerNumber, ToxChatChange change, IntPtr userData);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CallbackGroupTitleDelegate(ToxHandle tox, int groupNumber, int peerNumber, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] byte[] title, byte length, IntPtr userData);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ConferenceInviteDelegate(ToxHandle tox,
                                                      UInt32 friendNumber,
                                                      ToxConferenceType type,
                                                      [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)]Byte[] cookie,
                                                      SizeT length,
                                                      IntPtr userdata);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ConferenceConnectedDelegate(ToxHandle tox,
                                                         UInt32 conferenceNumber,
                                                         IntPtr userdata);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ConferenceMessageDelegate(ToxHandle tox,
                                                       UInt32 conferenceNumber,
                                                       UInt32 peerNumber,
                                                       ToxMessageType type,
                                                       [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)]Byte[] message,
                                                       SizeT length,
                                                       IntPtr userdata);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        // If peerNumber == UINT32_MAX, then author is unknown (e.g. initial joining the conference).
        public delegate void ConferenceTitleDelegate(ToxHandle tox,
                                                     UInt32 conferenceNumber,
                                                     UInt32 peerNumber,
                                                     [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)]Byte[] title,
                                                     SizeT length,
                                                     IntPtr userdata);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ConferencePeerNameDelegate(ToxHandle tox,
                                                        UInt32 conferenceNumber,
                                                        UInt32 peerNumber,
                                                        [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)]Byte[] name,
                                                        SizeT length,
                                                        IntPtr userdata);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ConferencePeerListChangedDelegate(ToxHandle tox,
                                                               UInt32 conferenceNumber,
                                                               IntPtr userdata);
    }
}
