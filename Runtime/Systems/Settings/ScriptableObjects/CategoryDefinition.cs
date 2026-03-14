#region

using System;
using UnityEngine;
using Object = System.Object;

#endregion

namespace BioluminescentGames.Utils.Systems.Settings.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Settings Category", menuName = "Scriptable Objects/Settings/Category")]
    public class CategoryDefinition : ScriptableObject, IEquatable<CategoryDefinition>, IComparable<CategoryDefinition>
    {
        [field: SerializeField] public int OrderIndex { get; private set; }

        #if UNITY_EDITOR
        public void EDITOR_SetOrderIndex(int orderIndex) => OrderIndex = orderIndex;
        #endif

        public bool Equals(CategoryDefinition other)
        {
            return UnityEngine.Object.Equals(this, other);
        }

        public int CompareTo(CategoryDefinition other)
        {
            return OrderIndex.CompareTo(other.OrderIndex);
        }
    }
}
