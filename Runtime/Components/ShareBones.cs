#if EDITOR_ATTRIBUTES
using EditorAttributes;
#endif
using BioluminescentGames.Utils.MonoBehaviourExtensions;
using UnityEngine;

namespace BioluminescentGames.Utils.Runtime
{
    [DisallowMultipleComponent]
    public class ShareBones : BioluminescentBehaviour
    {
        #if EDITOR_ATTRIBUTES
        [Required]
        #endif
        [SerializeField] private SkinnedMeshRenderer master;
        
        #if EDITOR_ATTRIBUTES
        [HelpBox("If Slave is null, it will automatically try to grab one from this GameObject")]
        #endif
        [Tooltip("If this is null, it will automatically try to grab one from this GameObject")]
        [SerializeField] private SkinnedMeshRenderer slave;

        private int _recheckCounter;
        
        private void Awake()
        {
            if (!slave)
                slave = GetComponent<SkinnedMeshRenderer>();
        }

        public override void OnUpdate()
        {
            if (!slave || !master)
                return;

            _recheckCounter--;
            if (_recheckCounter > 0)
                return;
            _recheckCounter = 60;
            
            slave.bones = master.bones;
            slave.rootBone = master.rootBone;
        }
    }
}
