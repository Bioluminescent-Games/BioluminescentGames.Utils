#if UNITY_NGO

using System;
using Unity.Netcode.Components;
using UnityEngine;

namespace BioluminescentGames.Utils.Utilities
{
    [DisallowMultipleComponent]
    [Obsolete]
    public class ClientNetworkTransform : NetworkTransform
    {
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}

#endif
