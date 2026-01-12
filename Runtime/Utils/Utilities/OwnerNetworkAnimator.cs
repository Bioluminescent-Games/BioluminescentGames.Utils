#if UNITY_NGO

using System;
using Unity.Netcode.Components;

namespace BioluminescentGames.Utils.Utilities
{
    [Obsolete]
    public class OwnerNetworkAnimator : NetworkAnimator
    {
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}

#endif
