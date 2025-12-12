#region

using UnityEngine;

#endregion

namespace BioluminescentGames.Systems.Settings.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Settings Category", menuName = "Scriptable Objects/Settings/Category")]
    public class CategoryDefinition : ScriptableObject
    {
        [field: SerializeField] public int OrderIndex { get; private set; }

        #if UNITY_EDITOR
        public void EDITOR_SetOrderIndex(int orderIndex) => OrderIndex = orderIndex;
        #endif
    }
}
