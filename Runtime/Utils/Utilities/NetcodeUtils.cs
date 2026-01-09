#if UNITY_NGO

using Unity.Netcode;
using UnityEngine;

namespace BioluminescentGames.Utils.Utilities
{
    public static class NetcodeUtils
    {
        private static ulong[] _singleTargetRpcParamArray = new ulong[1];

        public static ClientRpcParams GenerateMultipleTargetRpcParams(ulong[] targetClientIds)
        {
            return new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = targetClientIds
                }
            };
        }

        public static ClientRpcParams GenerateSingleTargetRpcParams(ulong targetClientId)
        {
            _singleTargetRpcParamArray[0] = targetClientId;
            return GenerateMultipleTargetRpcParams(_singleTargetRpcParamArray);
        }

        [RuntimeInitializeOnLoadMethod]
        private static void ResetOnLoad()
        {
            _singleTargetRpcParamArray = new ulong[1];
        }
    }
}

#endif
