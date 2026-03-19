using BioluminescentGames.Utils.Core;
using BioluminescentGames.Utils.Utilities;
using EditorAttributes;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace BioluminescentGames.Utils.Systems.Settings.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Keybind Setting", menuName = "Scriptable Objects/Settings/Keybind")]
    public class KeybindSetting : SavableSetting
    {
        [HelpBox("Do not edit the \"Default Value\" field.")]
        private Void _;

        [field: FormerlySerializedAs("<InputAction>k__BackingField")]
        [field: SerializeField] public InputActionReference InputActionReference { get; private set; }

        public InputAction InputAction => GameInterface.Instance.GetInputHandler().InputActionAsset.FindAction(InputActionReference.action.id);


        [field: BindingIndex(nameof(InputActionReference))]
        [field: SerializeField] public int BindingIndex { get; private set; }

        protected override void LoadFromPlayerPrefs()
        {
            InputAction.LoadBindingOverridesFromJson(EnhancedPlayerPrefs.GetString(ID), false);
        }

        protected override void SaveToPlayerPrefs()
        {
            EnhancedPlayerPrefs.SetString(ID, InputAction.SaveBindingOverridesAsJson());
        }
    }
}
