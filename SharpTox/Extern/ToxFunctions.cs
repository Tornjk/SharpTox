using System;
using System.Runtime.InteropServices;
using SizeT = System.UInt32;

namespace SharpTox.Core
{
    /// <summary>
    /// Native Functions to C tox.h
    /// </summary>
    internal static class ToxFunctions
    {
        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_version_major")]
        public static extern UInt32 VersionMajor();

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_version_minor")]
        public static extern UInt32 VersionMinor();

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_version_patch")]
        public static extern UInt32 VersionPatch();

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_version_is_compatible")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern Boolean VersionIsCompatible(UInt32 major, UInt32 minor, UInt32 patch);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_max_name_length")]
        public static extern UInt32 GetMaxNameLength();

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_max_status_message_length")]
        public static extern UInt32 GetMaxStatusMessageLength();

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_max_friend_request_length")]
        public static extern UInt32 GetMaxFriendRequestLength();

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_max_message_length")]
        public static extern UInt32 GetMaxMessageLength();

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_max_custom_packet_size")]
        public static extern UInt32 GetMaxCustomPacketSize();

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_max_filename_length")]
        public static extern UInt32 GetMaxFilenameLength();

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_max_hostname_length")]
        public static extern UInt32 GetMaxHostnameLength();

        public static class Options
        {
            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_options_new")]
            public static extern ToxOptionsHandle New(ref ToxErrorOptionsNew error);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_options_free")]
            public static extern void Free(IntPtr options);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_options_default")]
            public static extern void Default(ToxOptionsHandle options);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_options_get_ipv6_enabled")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern Boolean GetIpv6Enabled(ToxOptionsHandle options);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_options_set_ipv6_enabled")]
            public static extern void SetIpv6Enabled(ToxOptionsHandle options, Boolean ipv6_enabled);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_options_get_udp_enabled")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern Boolean GetUdpEnabled(ToxOptionsHandle options);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_options_set_udp_enabled")]
            public static extern void SetUdpEnabled(ToxOptionsHandle options, Boolean udp_enabled);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_options_get_local_discovery_enabled")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern Boolean GetLocalDiscoveryEnabled(ToxOptionsHandle options);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_options_set_local_discovery_enabled")]
            public static extern void SetLocalDiscoveryEnabled(ToxOptionsHandle options, Boolean local_discovery_enabled);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_options_get_proxy_type")]
            public static extern ToxProxyType GetProxyType(ToxOptionsHandle options);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_options_set_proxy_type")]
            public static extern void SetProxyType(ToxOptionsHandle options, ToxProxyType proxy_type);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_options_get_proxy_host")]
            [return: MarshalAs(UnmanagedType.LPStr)]
            public static extern String GetProxyHost(ToxOptionsHandle options);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_options_set_proxy_host")]
            public static extern void SetProxyHost(ToxOptionsHandle options, [MarshalAs(UnmanagedType.LPStr)] String proxy_host);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_options_get_proxy_port")]
            public static extern UInt16 GetProxyPort(ToxOptionsHandle options);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_options_set_proxy_port")]
            public static extern void SetProxyPort(ToxOptionsHandle options, UInt16 proxy_port);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_options_get_start_port")]
            public static extern UInt16 GetStartPort(ToxOptionsHandle options);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_options_set_start_port")]
            public static extern void SetStartPort(ToxOptionsHandle options, UInt16 start_port);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_options_get_end_port")]
            public static extern UInt16 GetEndPort(ToxOptionsHandle options);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_options_set_end_port")]
            public static extern void SetEndPort(ToxOptionsHandle options, UInt16 end_port);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_options_get_tcp_port")]
            public static extern UInt16 GetTcpPort(ToxOptionsHandle options);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_options_set_tcp_port")]
            public static extern void SetTcpPort(ToxOptionsHandle options, UInt16 tcp_port);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_options_get_hole_punching_enabled")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern Boolean GetHolePunchingEnabled(ToxOptionsHandle options);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_options_set_hole_punching_enabled")]
            public static extern void SetHolePunchingEnabled(ToxOptionsHandle options, Boolean hole_punching_enabled);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_options_get_savedata_type")]
            public static extern ToxSavedataType GetSavedataType(ToxOptionsHandle options);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_options_set_savedata_type")]
            public static extern void SetSavedataType(ToxOptionsHandle options, ToxSavedataType type);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_options_get_savedata_data")]
            public static extern IntPtr GetSavedataData(ToxOptionsHandle options);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_options_set_savedata_data")]
            public static extern void SetSavedataData(ToxOptionsHandle options, IntPtr data, SizeT length);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_options_get_savedata_length")]
            public static extern SizeT GetSavedataLength(ToxOptionsHandle options);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_options_set_savedata_length")]
            public static extern void SetSavedataLength(ToxOptionsHandle options, SizeT length);
        }

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_new")]
        public static extern ToxHandle New(ToxOptionsHandle options, ref ToxErrorNew error);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_kill")]
        public static extern void Kill(IntPtr tox);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_get_savedata_size")]
        public static extern SizeT GetSaveDataSize(ToxHandle tox);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_get_savedata")]
        public static extern void GetSaveData(ToxHandle tox, Byte[] bytes);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_bootstrap")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool Bootstrap(ToxHandle tox, String host, UInt16 port, Byte[] publicKey, ref ToxErrorBootstrap error);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_self_get_connection_status")]
        public static extern ToxConnectionStatus SelfGetConnectionStatus(ToxHandle tox);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_self_get_address")]
        public static extern void SelfGetAddress(ToxHandle tox, byte[] address);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_friend_by_public_key")]
        public static extern uint FriendByPublicKey(ToxHandle tox, byte[] publicKey, ref ToxErrorFriendByPublicKey error);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_friend_get_public_key")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool FriendGetPublicKey(ToxHandle tox, uint friendNumber, byte[] publicKey, ref ToxErrorFriendGetPublicKey error);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_iterate")]
        public static extern void Iterate(ToxHandle tox);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_iteration_interval")]
        public static extern uint IterationInterval(ToxHandle tox);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_friend_delete")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool FriendDelete(ToxHandle tox, uint friendNumber, ref ToxErrorFriendDelete error);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_friend_get_connection_status")]
        public static extern ToxConnectionStatus FriendGetConnectionStatus(ToxHandle tox, uint friendNumber, ref ToxErrorFriendQuery error);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_friend_get_status")]
        public static extern ToxUserStatus FriendGetStatus(ToxHandle tox, uint friendNumber, ref ToxErrorFriendQuery error);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_self_get_status")]
        public static extern ToxUserStatus SelfGetStatus(ToxHandle tox);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_friend_exists")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool FriendExists(ToxHandle tox, uint friendNumber);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_self_get_friend_list_size")]
        public static extern uint SelfGetFriendListSize(ToxHandle tox);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_self_get_friend_list")]
        public static extern void SelfGetFriendList(ToxHandle tox, uint[] list);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_friend_send_message")]
        public static extern uint FriendSendMessage(ToxHandle tox, uint friendNumber, ToxMessageType messageType, byte[] message, uint length, ref ToxErrorSendMessage error);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_friend_add")]
        public static extern uint FriendAdd(ToxHandle tox, byte[] address, byte[] message, uint length, ref ToxErrorFriendAdd error);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_friend_add_norequest")]
        public static extern uint FriendAddNoRequest(ToxHandle tox, byte[] publicKey, ref ToxErrorFriendAdd error);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_self_set_name")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool SelfSetName(ToxHandle tox, byte[] name, uint length, ref ToxErrorSetInfo error);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_self_get_name")]
        public static extern void SelfGetName(ToxHandle tox, byte[] name);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_self_get_name_size")]
        public static extern uint SelfGetNameSize(ToxHandle tox);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_self_set_typing")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool SelfSetTyping(ToxHandle tox, uint friendNumber, [MarshalAs(UnmanagedType.Bool)]bool is_typing, ref ToxErrorSetTyping error);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_friend_get_typing")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool FriendGetTyping(ToxHandle tox, uint friendNumber, ref ToxErrorFriendQuery error);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_add_tcp_relay")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool AddTcpRelay(ToxHandle tox, string host, ushort port, byte[] publicKey, ref ToxErrorBootstrap error);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_self_set_nospam")]
        public static extern void SelfSetNospam(ToxHandle tox, uint nospam);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_self_get_nospam")]
        public static extern uint SelfGetNospam(ToxHandle tox);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_self_get_public_key")]
        public static extern void SelfGetPublicKey(ToxHandle tox, byte[] publicKey);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_self_get_secret_key")]
        public static extern void SelfGetSecretKey(ToxHandle tox, byte[] secretKey);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_self_get_status_message")]
        public static extern void SelfGetStatusMessage(ToxHandle tox, byte[] status);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_self_get_status_message_size")]
        public static extern uint SelfGetStatusMessageSize(ToxHandle tox);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_self_set_status_message")]
        public static extern void SelfSetStatusMessage(ToxHandle tox, byte[] status, uint length, ref ToxErrorSetInfo error);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_self_set_status")]
        public static extern void SelfSetStatus(ToxHandle tox, ToxUserStatus status);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_friend_get_name_size")]
        public static extern uint FriendGetNameSize(ToxHandle tox, uint friendNumber, ref ToxErrorFriendQuery error);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_friend_get_name")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool FriendGetName(ToxHandle tox, uint friendNumber, byte[] name, ref ToxErrorFriendQuery error);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_friend_get_status_message_size")]
        public static extern uint FriendGetStatusMessageSize(ToxHandle tox, uint friendNumber, ref ToxErrorFriendQuery error);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_friend_get_status_message")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool FriendGetStatusMessage(ToxHandle tox, uint friendNumber, byte[] message, ref ToxErrorFriendQuery error);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_self_get_udp_port")]
        public static extern ushort SelfGetUdpPort(ToxHandle tox, ref ToxErrorGetPort error);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_self_get_tcp_port")]
        public static extern ushort SelfGetTcpPort(ToxHandle tox, ref ToxErrorGetPort error);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_self_get_dht_id")]
        public static extern void SelfGetDhtId(ToxHandle tox, byte[] dhtId);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_hash")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool Hash(byte[] hash, byte[] data, uint length);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_file_control")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool FileControl(ToxHandle tox, uint friendNumber, uint fileNumber, ToxFileControl control, ref ToxErrorFileControl error);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_file_send")]
        public static extern uint FileSend(ToxHandle tox, uint friendNumber, ToxFileKind kind, ulong fileSize, byte[] fileId, byte[] fileName, uint fileNameLength, ref ToxErrorFileSend error);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_file_send_chunk")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool FileSendChunk(ToxHandle tox, uint friendNumber, uint fileNumber, ulong position, byte[] data, uint length, ref ToxErrorFileSendChunk error);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_file_get_file_id")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool FileGetFileId(ToxHandle tox, uint friendNumber, uint fileNumber, byte[] fileId, ref ToxErrorFileGet error);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_file_seek")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool FileSeek(ToxHandle tox, uint friendNumber, uint fileNumber, ulong position, ref ToxErrorFileSeek error);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_friend_send_lossy_packet")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool FriendSendLossyPacket(ToxHandle tox, uint friendNumber, byte[] data, uint length, ref ToxErrorFriendCustomPacket error);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_friend_send_lossless_packet")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool FriendSendLosslessPacket(ToxHandle tox, uint friendNumber, byte[] data, uint length, ref ToxErrorFriendCustomPacket error);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_friend_get_last_online")]
        public static extern ulong FriendGetLastOnline(ToxHandle tox, uint friendNumber, ref ToxErrorFriendGetLastOnline error);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_add_groupchat")]
        public static extern int AddGroupchat(ToxHandle tox);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_del_groupchat")]
        public static extern int DelGroupchat(ToxHandle tox, int groupnumber);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_group_peername")]
        public static extern int GroupPeername(ToxHandle tox, int groupnumber, int peernumber, byte[] name);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_invite_friend")]
        public static extern int InviteFriend(ToxHandle tox, int friendnumber, int groupnumber);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_join_groupchat")]
        public static extern int JoinGroupchat(ToxHandle tox, int friendnumber, byte[] data, ushort length);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_group_message_send")]
        public static extern int GroupMessageSend(ToxHandle tox, int groupnumber, byte[] message, ushort length);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_group_action_send")]
        public static extern int GroupActionSend(ToxHandle tox, int groupnumber, byte[] action, ushort length);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_group_number_peers")]
        public static extern int GroupNumberPeers(ToxHandle tox, int groupnumber);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "tox_group_get_names")]
        public static extern int GroupGetNames(ToxHandle tox, int groupnumber, byte[,] names, ushort[] lengths, ushort length);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_group_peernumber_is_ours")]
        public static extern uint GroupPeerNumberIsOurs(ToxHandle tox, int groupnumber, int peernumber);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_group_set_title")]
        public static extern int GroupSetTitle(ToxHandle tox, int groupnumber, byte[] title, byte length);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_group_get_type")]
        public static extern int GroupGetType(ToxHandle tox, int groupnumber);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_group_get_title")]
        public static extern int GroupGetTitle(ToxHandle tox, int groupnumber, byte[] title, uint max_length);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_group_peer_pubkey")]
        public static extern int GroupPeerPubkey(ToxHandle tox, int groupnumber, int peernumber, byte[] pk);

        #region Register callback functions
        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_callback_friend_request")]
        public static extern void RegisterFriendRequestCallback(ToxHandle tox, ToxDelegates.CallbackFriendRequestDelegate callback, IntPtr userdata);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_callback_friend_message")]
        public static extern void RegisterFriendMessageCallback(ToxHandle tox, ToxDelegates.CallbackFriendMessageDelegate callback, IntPtr userdata);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_callback_friend_name")]
        public static extern void RegisterNameChangeCallback(ToxHandle tox, ToxDelegates.CallbackNameChangeDelegate callback, IntPtr userdata);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_callback_friend_status_message")]
        public static extern void RegisterStatusMessageCallback(ToxHandle tox, ToxDelegates.CallbackStatusMessageDelegate callback, IntPtr userdata);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_callback_friend_status")]
        public static extern void RegisterUserStatusCallback(ToxHandle tox, ToxDelegates.CallbackUserStatusDelegate callback, IntPtr userdata);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_callback_friend_typing")]
        public static extern void RegisterTypingChangeCallback(ToxHandle tox, ToxDelegates.CallbackTypingChangeDelegate callback, IntPtr userdata);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_callback_self_connection_status")]
        public static extern void RegisterConnectionStatusCallback(ToxHandle tox, ToxDelegates.CallbackConnectionStatusDelegate callback, IntPtr userdata);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_callback_friend_connection_status")]
        public static extern void RegisterFriendConnectionStatusCallback(ToxHandle tox, ToxDelegates.CallbackFriendConnectionStatusDelegate callback, IntPtr userdata);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_callback_friend_read_receipt")]
        public static extern void RegisterFriendReadReceiptCallback(ToxHandle tox, ToxDelegates.CallbackReadReceiptDelegate callback, IntPtr userdata);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_callback_file_recv")]
        public static extern void RegisterFileReceiveCallback(ToxHandle tox, ToxDelegates.CallbackFileReceiveDelegate callback, IntPtr userdata);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_callback_file_recv_control")]
        public static extern void RegisterFileControlRecvCallback(ToxHandle tox, ToxDelegates.CallbackFileControlDelegate callback, IntPtr userdata);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_callback_file_recv_chunk")]
        public static extern void RegisterFileReceiveChunkCallback(ToxHandle tox, ToxDelegates.CallbackFileReceiveChunkDelegate callback, IntPtr userdata);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_callback_file_chunk_request")]
        public static extern void RegisterFileChunkRequestCallback(ToxHandle tox, ToxDelegates.CallbackFileRequestChunkDelegate callback, IntPtr userdata);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_callback_friend_lossy_packet")]
        public static extern void RegisterFriendLossyPacketCallback(ToxHandle tox, ToxDelegates.CallbackFriendPacketDelegate callback, IntPtr userdata);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_callback_friend_lossless_packet")]
        public static extern void RegisterFriendLosslessPacketCallback(ToxHandle tox, ToxDelegates.CallbackFriendPacketDelegate callback, IntPtr userdata);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_callback_group_invite")]
        public static extern void RegisterGroupInviteCallback(ToxHandle tox, ToxDelegates.CallbackGroupInviteDelegate callback, IntPtr userdata);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_callback_group_message")]
        public static extern void RegisterGroupMessageCallback(ToxHandle tox, ToxDelegates.CallbackGroupMessageDelegate callback, IntPtr userdata);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_callback_group_action")]
        public static extern void RegisterGroupActionCallback(ToxHandle tox, ToxDelegates.CallbackGroupActionDelegate callback, IntPtr userdata);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_callback_group_namelist_change")]
        public static extern void RegisterGroupNamelistChangeCallback(ToxHandle tox, ToxDelegates.CallbackGroupNamelistChangeDelegate callback, IntPtr userdata);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_callback_group_title")]
        public static extern void RegisterGroupTitleCallback(ToxHandle tox, ToxDelegates.CallbackGroupTitleDelegate callback, IntPtr userdata);

        #endregion
    }
}
