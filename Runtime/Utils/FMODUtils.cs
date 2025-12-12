#if FMOD

using FMOD;

namespace BioluminescentGames.Utils
{
    public static class FMODUtils
    {
        // ReSharper disable Unity.PerformanceAnalysis
        public static bool CheckFMODResult(RESULT result)
        {
            if (result == RESULT.OK) return false;

            UnityEngine.Debug.LogWarning($"Failed to verify FMOD Result: {result}");
            return true;
        }
    }
}

#endif
