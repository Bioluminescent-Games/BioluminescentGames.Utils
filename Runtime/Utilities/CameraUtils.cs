using BioluminescentGames.Utils.Core;
using UnityEngine;

namespace BioluminescentGames.Utils.Runtime
{
    public static class CameraUtils
    {
        private static int _lastFrustumFrameRecalcTime = -1;
        private static Plane[] _cachedFrustumPlanes;
        
        public static Plane[] GetFrustumPlanes()
        {
            if (_lastFrustumFrameRecalcTime == Time.frameCount && _cachedFrustumPlanes.Length != 0)
                return _cachedFrustumPlanes;

            _lastFrustumFrameRecalcTime = Time.frameCount;

            return _cachedFrustumPlanes = GeometryUtility.CalculateFrustumPlanes(GameInterface.Instance.GetCurrentCamera());
        }

        public static bool IsObjectWithinViewFrustum(Bounds objectBounds)
        {
            return GeometryUtility.TestPlanesAABB(GetFrustumPlanes(), objectBounds);
        }
    }
}
