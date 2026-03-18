using UnityEngine;

namespace BioluminescentGames.Utils.Utilities
{
    public class BindingIndexAttribute : PropertyAttribute
    {
        // Name of the sibling InputActionReference field to read bindings from
        public readonly string ActionReferenceField;

        public BindingIndexAttribute(string actionReferenceField)
        {
            ActionReferenceField = actionReferenceField;
        }
    }
}
