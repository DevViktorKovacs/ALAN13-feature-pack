using Steamworks;

namespace ALAN13featurepack.Utility
{
    public class SteamManager
    {
        protected static bool s_EverInitialized = false;

        protected static SteamManager s_instance;
        protected static SteamManager Instance
        {
            get
            {
                if (s_instance == null)
                {
                    return new SteamManager();
                }
                else
                {
                    return s_instance;
                }
            }
        }

        protected bool m_bInitialized = false;
        public static bool Initialized
        {
            get
            {
                return Instance.m_bInitialized;
            }
        }

        public virtual void ShutDown()
        {
            if (Initialized)
            {
                SteamAPI.Shutdown();
            }
        }

        public bool Init()
        {
            // Only one instance of SteamManager at a time!
            if (s_instance != null)
            {
                return true;
            }
            s_instance = this;

            if (s_EverInitialized)
            {
                // This is almost always an error.
                // The most common case where this happens is when SteamManager gets destroyed because of Application.Quit(),
                // and then some Steamworks code in some other OnDestroy gets called afterwards, creating a new SteamManager.
                // You should never call Steamworks functions in OnDestroy, always prefer OnDisable if possible.
                throw new System.Exception("Tried to Initialize the SteamAPI twice in one session!");
            }


            try
            {
                // If Steam is not running or the game wasn't started through Steam, SteamAPI_RestartAppIfNecessary starts the
                // Steam client and also launches this game again if the User owns it. This can act as a rudimentary form of DRM.

                // Once you get a Steam AppID assigned by Valve, you need to replace AppId_t.Invalid with it and
                // remove steam_appid.txt from the game depot. eg: "(AppId_t)480" or "new AppId_t(480)".
                // See the Valve documentation for more information: https://partner.steamgames.com/doc/sdk/api#initialization_and_shutdown


                if (!InputProcessor.IsInDebugMode && SteamAPI.RestartAppIfNecessary(new AppId_t(1888130)))
                {
                    DebugHelper.PrettyPrintVerbose("Restarting steam...");
                    //Application.Quit();
                    return false;
                }
            }
            catch (System.DllNotFoundException e)
            { // We catch this exception here, as it will be the first occurrence of it.
                DebugHelper.PrintError("[Steamworks.NET] Could not load [lib]steam_api.dll/so/dylib. It's likely not in the correct location. Refer to the README for more details.\n" + e);

                //Application.Quit();
                return false;
            }

            // Initializes the Steamworks API.
            // If this returns false then this indicates one of the following conditions:
            // [*] The Steam client isn't running. A running Steam client is required to provide implementations of the various Steamworks interfaces.
            // [*] The Steam client couldn't determine the App ID of game. If you're running your application from the executable or debugger directly then you must have a [code-inline]steam_appid.txt[/code-inline] in your game directory next to the executable, with your app ID in it and nothing else. Steam will look for this file in the current working directory. If you are running your executable from a different directory you may need to relocate the [code-inline]steam_appid.txt[/code-inline] file.
            // [*] Your application is not running under the same OS user context as the Steam client, such as a different user or administration access level.
            // [*] Ensure that you own a license for the App ID on the currently active Steam account. Your game must show up in your Steam library.
            // [*] Your App ID is not completely set up, i.e. in Release State: Unavailable, or it's missing default packages.
            // Valve's documentation for this is located here:
            // https://partner.steamgames.com/doc/sdk/api#initialization_and_shutdown
            m_bInitialized = SteamAPI.Init();

            if (!m_bInitialized)
            {
                DebugHelper.PrintError("[Steamworks.NET] SteamAPI_Init() failed. Refer to Valve's documentation or the comment above this line for more information.");

                return false;
            }

            s_EverInitialized = true;

            DebugHelper.PrettyPrintVerbose($"Steam connected for {SteamFriends.GetPersonaName()}");

            return true;
        }

        public virtual void Update()
        {
            if (!m_bInitialized)
            {
                return;
            }

            // Run Steam client callbacks
            SteamAPI.RunCallbacks();
        }
    }
}
