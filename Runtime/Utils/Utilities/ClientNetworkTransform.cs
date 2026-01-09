#if UNITY_NGO

using Unity.Netcode.Components;
using UnityEngine;

namespace BioluminescentGames.Utils.Utilities
{
    [DisallowMultipleComponent]
    public class ClientNetworkTransform : NetworkTransform
    {
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}

#endif
