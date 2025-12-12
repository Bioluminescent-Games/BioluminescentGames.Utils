#if UNITY_NGO

using Unity.Netcode.Components;

namespace BioluminescentGames.Utils
{
    public class OwnerNetworkAnimator : NetworkAnimator
    {
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}

#endif
