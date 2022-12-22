using System.Runtime.InteropServices;
using IntPtr = System.IntPtr;

using System.Text;
using static Steamworks.InteropHelp;

namespace Steamworks
{
    public class InteropHelp
    {
        public static void TestIfPlatformSupported()
        {

        }

        public static void TestIfAvailableClient()
        {
            TestIfPlatformSupported();
            if (CSteamAPIContext.GetSteamClient() == System.IntPtr.Zero)
            {
                if (!CSteamAPIContext.Init())
                {
                    throw new System.InvalidOperationException("Steamworks is not initialized.");
                }
            }
        }

        public static void TestIfAvailableGameServer()
        {
            TestIfPlatformSupported();
            if (CSteamGameServerAPIContext.GetSteamClient() == System.IntPtr.Zero)
            {
                if (!CSteamGameServerAPIContext.Init())
                {
                    throw new System.InvalidOperationException("Steamworks GameServer is not initialized.");
                }
            }
        }

        // This continues to exist for both 'out string' and strings returned by Steamworks functions.
        public static string PtrToStringUTF8(IntPtr nativeUtf8)
        {
            if (nativeUtf8 == IntPtr.Zero)
            {
                return null;
            }

            int len = 0;

            while (Marshal.ReadByte(nativeUtf8, len) != 0)
            {
                ++len;
            }

            if (len == 0)
            {
                return string.Empty;
            }

            byte[] buffer = new byte[len];
            Marshal.Copy(nativeUtf8, buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer);
        }

        public static string ByteArrayToStringUTF8(byte[] buffer)
        {
            int length = 0;
            while (length < buffer.Length && buffer[length] != 0)
            {
                length++;
            }

            return Encoding.UTF8.GetString(buffer, 0, length);
        }

        public static void StringToByteArrayUTF8(string str, byte[] outArrayBuffer, int outArrayBufferSize)
        {
            outArrayBuffer = new byte[outArrayBufferSize];
            int length = Encoding.UTF8.GetBytes(str, 0, str.Length, outArrayBuffer, 0);
            outArrayBuffer[length] = 0;
        }

        // This is for 'const char *' arguments which we need to ensure do not get GC'd while Steam is using them.
        // We can't use an ICustomMarshaler because Unity crashes when a string between 96 and 127 characters long is defined/initialized at the top of class scope...

        public class UTF8StringHandle : Microsoft.Win32.SafeHandles.SafeHandleZeroOrMinusOneIsInvalid
        {
            public UTF8StringHandle(string str)
                : base(true)
            {
                if (str == null)
                {
                    SetHandle(IntPtr.Zero);
                    return;
                }

                // +1 for '\0'
                byte[] strbuf = new byte[Encoding.UTF8.GetByteCount(str) + 1];
                Encoding.UTF8.GetBytes(str, 0, str.Length, strbuf, 0);
                IntPtr buffer = Marshal.AllocHGlobal(strbuf.Length);
                Marshal.Copy(strbuf, 0, buffer, strbuf.Length);

                SetHandle(buffer);
            }

            protected override bool ReleaseHandle()
            {
                if (!IsInvalid)
                {
                    Marshal.FreeHGlobal(handle);
                }
                return true;
            }
        }

        //public class UTF8StringHandle : IDisposable {
        //	public UTF8StringHandle(string str) { }
        //	public void Dispose() {}
        //}


        // TODO - Should be IDisposable
        // We can't use an ICustomMarshaler because Unity dies when MarshalManagedToNative() gets called with a generic type.
        public class SteamParamStringArray
        {
            // The pointer to each AllocHGlobal() string
            IntPtr[] m_Strings;
            // The pointer to the condensed version of m_Strings
            IntPtr m_ptrStrings;
            // The pointer to the StructureToPtr version of SteamParamStringArray_t that will get marshaled
            IntPtr m_pSteamParamStringArray;

            public SteamParamStringArray(System.Collections.Generic.IList<string> strings)
            {
                if (strings == null)
                {
                    m_pSteamParamStringArray = IntPtr.Zero;
                    return;
                }

                m_Strings = new IntPtr[strings.Count];
                for (int i = 0; i < strings.Count; ++i)
                {
                    byte[] strbuf = new byte[Encoding.UTF8.GetByteCount(strings[i]) + 1];
                    Encoding.UTF8.GetBytes(strings[i], 0, strings[i].Length, strbuf, 0);
                    m_Strings[i] = Marshal.AllocHGlobal(strbuf.Length);
                    Marshal.Copy(strbuf, 0, m_Strings[i], strbuf.Length);
                }

                m_ptrStrings = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(IntPtr)) * m_Strings.Length);
                SteamParamStringArray_t stringArray = new SteamParamStringArray_t()
                {
                    m_ppStrings = m_ptrStrings,
                    m_nNumStrings = m_Strings.Length
                };
                Marshal.Copy(m_Strings, 0, stringArray.m_ppStrings, m_Strings.Length);

                m_pSteamParamStringArray = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SteamParamStringArray_t)));
                Marshal.StructureToPtr(stringArray, m_pSteamParamStringArray, false);
            }

            ~SteamParamStringArray()
            {
                if (m_Strings != null)
                {
                    foreach (IntPtr ptr in m_Strings)
                    {
                        Marshal.FreeHGlobal(ptr);
                    }
                }

                if (m_ptrStrings != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(m_ptrStrings);
                }

                if (m_pSteamParamStringArray != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(m_pSteamParamStringArray);
                }
            }

            public static implicit operator IntPtr(SteamParamStringArray that)
            {
                return that.m_pSteamParamStringArray;
            }
        }
    }
}
