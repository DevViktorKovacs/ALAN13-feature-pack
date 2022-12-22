﻿// This file is provided under The MIT License as part of Steamworks.NET.
// Copyright (c) 2013-2022 Riley Labrecque
// Please see the included LICENSE.txt for additional information.

// This file is automatically generated.
// Changes to this file will be reverted when you update Steamworks.NET


using System.Runtime.InteropServices;
using IntPtr = System.IntPtr;

namespace Steamworks
{
    /// A message that has been received.
    [System.Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct SteamNetworkingMessage_t
    {
        /// Message payload
        public IntPtr m_pData;

        /// Size of the payload.
        public int m_cbSize;

        /// For messages received on connections: what connection did this come from?
        /// For outgoing messages: what connection to send it to?
        /// Not used when using the ISteamNetworkingMessages interface
        public HSteamNetConnection m_conn;

        /// For inbound messages: Who sent this to us?
        /// For outbound messages on connections: not used.
        /// For outbound messages on the ad-hoc ISteamNetworkingMessages interface: who should we send this to?
        public SteamNetworkingIdentity m_identityPeer;

        /// For messages received on connections, this is the user data
        /// associated with the connection.
        ///
        /// This is *usually* the same as calling GetConnection() and then
        /// fetching the user data associated with that connection, but for
        /// the following subtle differences:
        ///
        /// - This user data will match the connection's user data at the time
        ///   is captured at the time the message is returned by the API.
        ///   If you subsequently change the userdata on the connection,
        ///   this won't be updated.
        /// - This is an inline call, so it's *much* faster.
        /// - You might have closed the connection, so fetching the user data
        ///   would not be possible.
        ///
        /// Not used when sending messages.
        public long m_nConnUserData;

        /// Local timestamp when the message was received
        /// Not used for outbound messages.
        public SteamNetworkingMicroseconds m_usecTimeReceived;

        /// Message number assigned by the sender.  This is not used for outbound
        /// messages.  Note that if multiple lanes are used, each lane has its own
        /// message numbers, which are assigned sequentially, so messages from
        /// different lanes will share the same numbers.
        public long m_nMessageNumber;

        /// Function used to free up m_pData.  This mechanism exists so that
        /// apps can create messages with buffers allocated from their own
        /// heap, and pass them into the library.  This function will
        /// usually be something like:
        ///
        /// free( pMsg->m_pData );
        public IntPtr m_pfnFreeData;

        /// Function to used to decrement the internal reference count and, if
        /// it's zero, release the message.  You should not set this function pointer,
        /// or need to access this directly!  Use the Release() function instead!
        internal IntPtr m_pfnRelease;

        /// When using ISteamNetworkingMessages, the channel number the message was received on
        /// (Not used for messages sent or received on "connections")
        public int m_nChannel;

        /// Bitmask of k_nSteamNetworkingSend_xxx flags.
        /// For received messages, only the k_nSteamNetworkingSend_Reliable bit is valid.
        /// For outbound messages, all bits are relevant
        public int m_nFlags;

        /// Arbitrary user data that you can use when sending messages using
        /// ISteamNetworkingUtils::AllocateMessage and ISteamNetworkingSockets::SendMessage.
        /// (The callback you set in m_pfnFreeData might use this field.)
        ///
        /// Not used for received messages.
        public long m_nUserData;

        /// For outbound messages, which lane to use?  See ISteamNetworkingSockets::ConfigureConnectionLanes.
        /// For inbound messages, what lane was the message received on?
        public ushort m_idxLane;

        public ushort _pad1__;

        /// You MUST call this when you're done with the object,
        /// to free up memory, etc.
        public void Release()
        {
            throw new System.NotImplementedException("Please use the static Release function instead which takes an IntPtr.");
        }

        /// You MUST call this when you're done with the object,
        /// to free up memory, etc.
        /// This is a Steamworks.NET extension.
        public static void Release(IntPtr pointer)
        {
            NativeMethods.SteamAPI_SteamNetworkingMessage_t_Release(pointer);
        }

        /// Convert an IntPtr received from ISteamNetworkingSockets.ReceiveMessagesOnPollGroup into our structure.
        /// This is a Steamworks.NET extension.
        public static SteamNetworkingMessage_t FromIntPtr(IntPtr pointer)
        {
            return (SteamNetworkingMessage_t)Marshal.PtrToStructure(pointer, typeof(SteamNetworkingMessage_t));
        }
    }
}
