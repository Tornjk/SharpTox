using System;
using System.Runtime.InteropServices;
using SizeT = System.UInt32;

namespace SharpTox.Core
{
    /// <summary>
    /// Native Functions to C tox.h
    /// </summary>
    static class ToxFunctions
    {
        public static class Version
        {
            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_version_major")]
            public static extern UInt32 Major();

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_version_minor")]
            public static extern UInt32 Minor();

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_version_patch")]
            public static extern UInt32 Patch();

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_version_is_compatible")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern Boolean IsCompatible(UInt32 major, UInt32 minor, UInt32 patch);
        }

        public static class Max
        {
            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_max_name_length")]
            public static extern UInt32 NameLength();

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_max_status_message_length")]
            public static extern UInt32 StatusMessageLength();

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_max_friend_request_length")]
            public static extern UInt32 FriendRequestLength();

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_max_message_length")]
            public static extern UInt32 MessageLength();

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_max_custom_packet_size")]
            public static extern UInt32 CustomPacketSize();

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_max_filename_length")]
            public static extern UInt32 FilenameLength();

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_max_hostname_length")]
            public static extern UInt32 HostnameLength();
        }

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
            public static extern void SetProxyHost(ToxOptionsHandle options, [In, MarshalAs(UnmanagedType.LPStr)] String proxy_host);

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
        public static extern Boolean Bootstrap(ToxHandle tox, String host, UInt16 port, Byte[] publicKey, ref ToxErrorBootstrap error);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_add_tcp_relay")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern Boolean AddTcpRelay(ToxHandle tox, String host, UInt16 port, Byte[] publicKey, ref ToxErrorBootstrap error);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_self_get_connection_status")]
        [Obsolete("Use the event and store the status.", false)]
        public static extern ToxConnectionStatus SelfGetConnectionStatus(ToxHandle tox);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_iteration_interval")]
        public static extern UInt32 IterationInterval(ToxHandle tox);

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_iterate")]
        public static extern void Iterate(ToxHandle tox);

        public static class Self
        {
            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_self_get_address")]
            public static extern void GetAddress(ToxHandle tox, Byte[] address);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_self_set_nospam")]
            public static extern void SetNospam(ToxHandle tox, UInt32 nospam);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_self_get_nospam")]
            public static extern UInt32 GetNospam(ToxHandle tox);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_self_get_public_key")]
            public static extern void GetPublicKey(ToxHandle tox, Byte[] publicKey);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_self_get_secret_key")]
            public static extern void GetSecretKey(ToxHandle tox, Byte[] secretKey);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_self_set_name")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern Boolean SetName(ToxHandle tox, Byte[] name, SizeT length, ref ToxErrorSetInfo error);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_self_get_name_size")]
            public static extern SizeT GetNameSize(ToxHandle tox);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_self_get_name")]
            public static extern void GetName(ToxHandle tox, Byte[] name);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_self_set_status_message")]
            public static extern void SetStatusMessage(ToxHandle tox, Byte[] status, SizeT length, ref ToxErrorSetInfo error);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_self_get_status_message_size")]
            public static extern SizeT GetStatusMessageSize(ToxHandle tox);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_self_get_status_message")]
            public static extern void GetStatusMessage(ToxHandle tox, Byte[] status);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_self_set_status")]
            public static extern void SetStatus(ToxHandle tox, ToxUserStatus status);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_self_get_status")]
            public static extern ToxUserStatus GetStatus(ToxHandle tox);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_self_get_friend_list_size")]
            public static extern SizeT GetFriendListSize(ToxHandle tox);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_self_get_friend_list")]
            public static extern void GetFriendList(ToxHandle tox, UInt32[] list);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_self_set_typing")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern Boolean SetTyping(ToxHandle tox, UInt32 friendNumber, [MarshalAs(UnmanagedType.Bool)]Boolean typing, ref ToxErrorSetTyping error);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_self_get_dht_id")]
            public static extern void GetDhtId(ToxHandle tox, Byte[] dhtId);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_self_get_udp_port")]
            public static extern UInt16 GetUdpPort(ToxHandle tox, ref ToxErrorGetPort error);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_self_get_tcp_port")]
            public static extern UInt16 GetTcpPort(ToxHandle tox, ref ToxErrorGetPort error);
        }

        public static class Friend
        {
            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_friend_add")]
            public static extern UInt32 Add(ToxHandle tox, Byte[] address, Byte[] message, SizeT length, ref ToxErrorFriendAdd error);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_friend_add_norequest")]
            public static extern UInt32 AddNoRequest(ToxHandle tox, Byte[] publicKey, ref ToxErrorFriendAdd error);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_friend_delete")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern Boolean Delete(ToxHandle tox, UInt32 friendNumber, ref ToxErrorFriendDelete error);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_friend_by_public_key")]
            public static extern UInt32 ByPublicKey(ToxHandle tox, Byte[] publicKey, ref ToxErrorFriendByPublicKey error);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_friend_exists")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern Boolean Exists(ToxHandle tox, UInt32 friendNumber);

            public static SizeT GetFriendListSize(ToxHandle tox) => Self.GetFriendListSize(tox);

            public static void GetFriendList(ToxHandle tox, UInt32[] list) => Self.GetFriendList(tox, list);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_friend_get_public_key")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern Boolean GetPublicKey(ToxHandle tox, UInt32 friendNumber, Byte[] publicKey, ref ToxErrorFriendGetPublicKey error);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_friend_get_last_online")]
            public static extern UInt64 GetLastOnline(ToxHandle tox, UInt32 friendNumber, ref ToxErrorFriendGetLastOnline error);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_friend_get_name_size")]
            public static extern SizeT GetNameSize(ToxHandle tox, UInt32 friendNumber, ref ToxErrorFriendQuery error);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_friend_get_name")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern Boolean GetName(ToxHandle tox, UInt32 friendNumber, Byte[] name, ref ToxErrorFriendQuery error);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_friend_get_status_message_size")]
            public static extern SizeT GetStatusMessageSize(ToxHandle tox, UInt32 friendNumber, ref ToxErrorFriendQuery error);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_friend_get_status_message")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern Boolean GetStatusMessage(ToxHandle tox, UInt32 friendNumber, Byte[] message, ref ToxErrorFriendQuery error);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_friend_send_message")]
            public static extern UInt32 SendMessage(ToxHandle tox, UInt32 friendNumber, ToxMessageType messageType, Byte[] message, SizeT length, ref ToxErrorSendMessage error);

            [Obsolete("Use event", false)]
            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_friend_get_connection_status")]
            public static extern ToxConnectionStatus GetConnectionStatus(ToxHandle tox, UInt32 friendNumber, ref ToxErrorFriendQuery error);

            [Obsolete("Use event", false)]
            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_friend_get_status")]
            public static extern ToxUserStatus GetStatus(ToxHandle tox, UInt32 friendNumber, ref ToxErrorFriendQuery error);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_friend_send_lossy_packet")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool SendLossyPacket(ToxHandle tox, UInt32 friendNumber, Byte[] data, SizeT length, ref ToxErrorFriendCustomPacket error);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_friend_send_lossless_packet")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool SendLosslessPacket(ToxHandle tox, UInt32 friendNumber, Byte[] data, SizeT length, ref ToxErrorFriendCustomPacket error);
        }

        [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_hash")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern Boolean Hash(Byte[] hash, Byte[] data, SizeT length);

        public static class File
        {
            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_file_control")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern Boolean Control(ToxHandle tox, UInt32 friendNumber, UInt32 fileNumber, ToxFileControl control, ref ToxErrorFileControl error);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_file_seek")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern Boolean Seek(ToxHandle tox, UInt32 friendNumber, UInt32 fileNumber, UInt64 position, ref ToxErrorFileSeek error);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_file_get_file_id")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern Boolean GetFileId(ToxHandle tox, UInt32 friendNumber, UInt32 fileNumber, Byte[] fileId, ref ToxErrorFileGet error);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_file_send")]
            public static extern UInt32 Send(ToxHandle tox, UInt32 friendNumber, ToxFileKind kind, UInt64 fileSize, Byte[] fileId, Byte[] fileName, SizeT fileNameLength, ref ToxErrorFileSend error);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_file_send_chunk")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern Boolean SendChunk(ToxHandle tox, UInt32 friendNumber, UInt32 fileNumber, UInt64 position, Byte[] data, SizeT length, ref ToxErrorFileSendChunk error);

        }

        public static class Conference
        {
            const string Base = "tox_conference_";

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Base + "new")]
            public static extern UInt32 New(ToxHandle tox, ref ToxErrorConferenceNew error);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Base + "delete")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern Boolean Delete(ToxHandle tox, UInt32 conferenceNumber, ref ToxErrorConferenceDelete error);

            public static class Peer
            {
                const string Prefix = Base + "peer_";

                [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Prefix + "count")]
                public static extern UInt32 Count(ToxHandle tox, UInt32 conferenceNumber, ref ToxErrorConferencePeerQuery error);

                [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Prefix + "get_name_size")]
                public static extern SizeT GetNameSize(ToxHandle tox, UInt32 conferenceNumber, UInt32 peerNumber, ref ToxErrorConferencePeerQuery error);

                [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Prefix + "get_public_key")]
                [return: MarshalAs(UnmanagedType.I1)]
                public static extern Boolean GetPublicKey(ToxHandle tox, UInt32 conferenceNumber, UInt32 peerNumber, Byte[] publicKey, ref ToxErrorConferencePeerQuery error);

                [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Prefix + "number_is_ours")]
                [return: MarshalAs(UnmanagedType.I1)]
                public static extern Boolean NumberIsOurs(ToxHandle tox, UInt32 conferenceNumber, UInt32 peerNumber, ref ToxErrorConferencePeerQuery error);
            }

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Base + "invite")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern Boolean Invite(ToxHandle tox, UInt32 friendNumber, UInt32 conferenceNumber, ref ToxErrorConferenceInvite error);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Base + "join")]
            public static extern UInt32 Join(ToxHandle tox, UInt32 friendNumber, Byte[] cookie, SizeT length, ref ToxErrorConferenceJoin error);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Base + "send_message")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern Boolean SendMessage(ToxHandle tox, UInt32 conferenceNumber, ToxMessageType type, Byte[] message, SizeT length, ref ToxErrorConferenceSendMessage error);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Base + "get_title_size")]
            public static extern SizeT GetTitleSize(ToxHandle tox, UInt32 conferenceNumber, ref ToxErrorConferenceTitle error);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Base + "get_title")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern Boolean GetTitle(ToxHandle tox, UInt32 conferenceNumber, Byte[] title, ref ToxErrorConferenceTitle error);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Base + "set_title")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern Boolean SetTitle(ToxHandle tox, UInt32 conferenceNumber, Byte[] title, SizeT length, ref ToxErrorConferenceTitle error);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Base + "get_chatlist_size")]
            public static extern SizeT GetChatlistSize(ToxHandle tox);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Base + "get_chatlist")]
            public static extern void GetChatlist(ToxHandle tox, UInt32[] chatlist);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Base + "get_type")]
            public static extern ToxConferenceType GetType(ToxHandle tox, UInt32 conferenceNumber, ref ToxErrorConferenceGetType error);

            // TOX_CONFERENCE_ID_SIZE
            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Base + "get_id")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern Boolean GetId(ToxHandle tox, UInt32 conferenceNumber, Byte[] id);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Base + "by_id")]
            public static extern UInt32 ById(ToxHandle tox, Byte[] id, ref ToxErrorConferenceById error);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Base + "get_uid")]
            [return: MarshalAs(UnmanagedType.I1)]
            [Obsolete("Use GetId instead (exactly the same function, just renamed)")]
            public static extern Boolean GetUid(ToxHandle tox, UInt32 conferenceNumber, Byte[] uid);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = Base + "by_uid")]
            [Obsolete("Use ById instead (exactly the same function, just renamed)")]
            public static extern UInt32 ByUid(ToxHandle tox, Byte[] uid, ref ToxErrorConferenceByUid error);

        }
    }

    sealed class ToxCallbackHandler<TEventArgs, TDelegate> where TEventArgs : EventArgs where TDelegate : class
    {
        private readonly Action<ToxHandle, TDelegate> register;
        private readonly Func<Action<TEventArgs>, TDelegate> create;

        private TDelegate tDelegate;

        private event EventHandler<TEventArgs> @event;

        public ToxCallbackHandler(Action<ToxHandle, TDelegate> register, Func<Action<TEventArgs>, TDelegate> create)
        {
            this.register = register ?? throw new ArgumentNullException(nameof(register));
            this.create = create ?? throw new ArgumentNullException(nameof(create));
        }

        public void Add(Tox tox, EventHandler<TEventArgs> handler)
        {
            if (this.tDelegate == null)
            {
                this.tDelegate = this.create(args => this.OnCallback(tox, args));
                this.register(tox.Handle, this.tDelegate);
            }

            this.@event += handler;
        }

        public void Remove(Tox tox, EventHandler<TEventArgs> handler)
        {
            if (this.@event.GetInvocationList().Length == 1)
            {
                this.register(tox.Handle, null);
                this.tDelegate = null;
            }

            this.@event -= handler;
        }

        private void OnCallback(Tox tox, TEventArgs args) => this.@event?.Invoke(tox, args);
    }

    /// <summary>
    /// Callbacks of Native tox.h
    /// </summary>
    static class ToxCallbacks
    {
        public static class Self
        {
            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_callback_self_connection_status")]
            public static extern void ConnectionStatus(ToxHandle tox, ToxDelegates.CallbackConnectionStatusDelegate callback);
        }

        public static class Friend
        {
            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_callback_friend_name")]
            public static extern void NameChange(ToxHandle tox, ToxDelegates.CallbackNameChangeDelegate callback);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_callback_friend_status_message")]
            public static extern void StatusMessageChange(ToxHandle tox, ToxDelegates.CallbackStatusMessageDelegate callback);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_callback_friend_status")]
            public static extern void StatusChange(ToxHandle tox, ToxDelegates.CallbackUserStatusDelegate callback);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_callback_friend_connection_status")]
            public static extern void ConnectionStatusChange(ToxHandle tox, ToxDelegates.CallbackFriendConnectionStatusDelegate callback);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_callback_friend_typing")]
            public static extern void TypingChange(ToxHandle tox, ToxDelegates.CallbackTypingChangeDelegate callback);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_callback_friend_read_receipt")]
            public static extern void ReadReceipt(ToxHandle tox, ToxDelegates.CallbackReadReceiptDelegate callback);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_callback_friend_request")]
            public static extern void FriendRequest(ToxHandle tox, ToxDelegates.CallbackFriendRequestDelegate callback);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_callback_friend_message")]
            public static extern void Message(ToxHandle tox, ToxDelegates.CallbackFriendMessageDelegate callback);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_callback_friend_lossy_packet")]
            public static extern void LossyPacket(ToxHandle tox, ToxDelegates.CallbackFriendPacketDelegate callback);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_callback_friend_lossless_packet")]
            public static extern void LosslessPacket(ToxHandle tox, ToxDelegates.CallbackFriendPacketDelegate callback);
        }

        public static class File
        {
            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_callback_file_recv_control")]
            public static extern void ReceiveControl(ToxHandle tox, ToxDelegates.CallbackFileControlDelegate callback);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_callback_file_chunk_request")]
            public static extern void ChunkRequest(ToxHandle tox, ToxDelegates.CallbackFileRequestChunkDelegate callback);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_callback_file_recv")]
            public static extern void Receive(ToxHandle tox, ToxDelegates.CallbackFileReceiveDelegate callback);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_callback_file_recv_chunk")]
            public static extern void ReceiveChunk(ToxHandle tox, ToxDelegates.CallbackFileReceiveChunkDelegate callback);
        }

        public static class Conference
        {
            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_callback_conference_invite")]
            public static extern void Invite(ToxHandle tox, ToxDelegates.ConferenceInviteDelegate callback);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_callback_conference_connected")]
            public static extern void Connected(ToxHandle tox, ToxDelegates.ConferenceConnectedDelegate callback);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_callback_conference_message")]
            public static extern void Message(ToxHandle tox, ToxDelegates.ConferenceMessageDelegate callback);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_callback_conference_title")]
            public static extern void Title(ToxHandle tox, ToxDelegates.ConferenceTitleDelegate callback);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_callback_conference_peer_name")]
            public static extern void PeerName(ToxHandle tox, ToxDelegates.ConferencePeerNameDelegate callback);

            [DllImport(Extern.DLL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "tox_callback_conference_peer_list_changed")]
            public static extern void PeerListChanged(ToxHandle tox, ToxDelegates.ConferencePeerListChangedDelegate callback);
        }
    }


}
