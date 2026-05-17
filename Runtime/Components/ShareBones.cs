using UnityEngine;

namespace BioluminescentGames.Utils.Runtime
{
    [RequireComponent(typeof(SkinnedMeshRenderer))]
    [DisallowMultipleComponent]
    public class ShareBones : MonoBehaviour
    {
        [SerializeField] private SkinnedMeshRenderer master;

        private void Start()
        {
            SkinnedMeshRenderer slave = GetComponent<SkinnedMeshRenderer>();

            slave.bones = master.bones;
            slave.rootBone = master.rootBone;
        }
    }
}
