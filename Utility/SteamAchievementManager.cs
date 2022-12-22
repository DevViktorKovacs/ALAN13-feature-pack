using ALAN13featurepack.Utility;
using Steamworks;
using System.Collections.Generic;

namespace UntitledRobotGamePrototype.Helpers
{
    public static class SteamAchievementManager
    {
        public static string ACHIEVEMENT_FIRST = "ACHIEVEMENT_0_1";

        public static string ACHIEVEMENT_SECOND = "ACHIEVEMENT_0_2";

        private static Dictionary<string, string> achiNames = new Dictionary<string, string>()
        {
            { ACHIEVEMENT_FIRST, nameof(ACHIEVEMENT_FIRST) },
            { ACHIEVEMENT_SECOND, nameof(ACHIEVEMENT_SECOND) },
        };


        public static bool TryToSetAchievement(string achivementName)
        {
            bool alreadyAwarded = false;

            string achiname = achivementName;

            var success = achiNames.TryGetValue(achivementName, out achiname);

            DebugHelper.PrettyPrintVerbose($"Trying to set steam achievement: {achiname} ({achivementName})", System.ConsoleColor.Green);

            if (!InputProcessor.IsSteamInitialized)
            {
                DebugHelper.PrintError($"Steam is not initialized!");

                return false;
            }

            SteamAPI.RunCallbacks();

            if (!SteamUserStats.GetAchievement(achivementName, out alreadyAwarded))
            {
                DebugHelper.PrintError($"Failed To Get Achievement: {achivementName}!");

                return false;
            }

            if (alreadyAwarded)
            {
                DebugHelper.PrettyPrintVerbose($"{achivementName} is already awarded!");

                return false;
            }

            if (!SteamUserStats.SetAchievement(achivementName))
            {
                DebugHelper.PrintError($"Failed To Set Achievement: {achivementName}!");

                return false;
            }

            SteamUserStats.StoreStats();

            return true;
        }

        public static bool TryToClearAchievement(string achivementName)
        {
            bool alreadyAwarded = false;

            if (!InputProcessor.IsSteamInitialized)
            {
                DebugHelper.PrintError($"Steam is not initialized!");

                return false;
            }

            SteamAPI.RunCallbacks();

            if (!SteamUserStats.GetAchievement(achivementName, out alreadyAwarded))
            {
                DebugHelper.PrintError($"Failed To Get Achievement: {achivementName}!");

                return false;
            }

            if (!alreadyAwarded)
            {
                DebugHelper.PrettyPrintVerbose($"{achivementName} is not yet awarded!");

                return false;
            }

            if (!SteamUserStats.ClearAchievement(achivementName))
            {
                DebugHelper.PrintError($"Failed To clear Achievement: {achivementName}!");

                return false;
            }

            return true;
        }
    }
}
