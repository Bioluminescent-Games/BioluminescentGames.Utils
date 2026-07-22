#if FMOD

using BioluminescentGames.Utils.StaticUtilities;
using FMOD;

namespace BioluminescentGames.Utils.Utilities
{
    public static class FMODUtils
    {
        // ReSharper disable Unity.PerformanceAnalysis
        public static bool CheckFMODResult(RESULT result)
        {
            if (result == RESULT.OK) return false;

            Log.Warning($"FMOD Error: {result.ToString()}");
            return true;
        }
    }
}

#endif
