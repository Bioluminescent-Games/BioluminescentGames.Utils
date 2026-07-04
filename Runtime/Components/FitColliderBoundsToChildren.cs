using BioluminescentGames.Utils.Utilities;
using UnityEngine;
#if ZLINQ
using ZLinq;
#endif

namespace BioluminescentGames.Utils.Runtime
{
    [RequireComponent(typeof(BoxCollider))]
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class FitColliderBoundsToChildren : MonoBehaviour
    {
        [SerializeField] private float updateInterval = 0.5f;

        private BoxCollider _collider;
        
        private void Awake()
        {
            _collider = GetComponent<BoxCollider>();
        }

        private void Start()
        {
            InvokeRepeating(nameof(UpdateColliders), 0.0f, updateInterval);
        }

        private Bounds[] GetBounds()
        {
            var colliders = GetComponentsInChildren<Collider>();
            var bounds = colliders
#if ZLINQ
                .AsValueEnumerable()
#endif
                .Where(collider => collider != _collider)
                .Select(collider => collider.bounds);

            return bounds.ToArray();
        }

        private void UpdateColliders()
        {
            _collider.SetBounds(BoundsUtils.GetBoundsEncapsulating(GetBounds()));
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            foreach (Bounds bound in GetBounds())
                Gizmos.DrawWireCube(bound.center, bound.size);
        }
    }
}
