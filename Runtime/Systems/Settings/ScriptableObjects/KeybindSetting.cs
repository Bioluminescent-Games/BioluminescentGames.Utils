using BioluminescentGames.Utils.Core;
using BioluminescentGames.Utils.Utilities;
#if EDITOR_ATTRIBUTES
using EditorAttributes;
#endif
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace BioluminescentGames.Utils.Systems.Settings.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Keybind Setting", menuName = "Scriptable Objects/Settings/Keybind")]
    public class KeybindSetting : SavableSetting
    {
        private const string k_PlayerPrefsPrefix = "keybind_guid_";

        #if EDITOR_ATTRIBUTES
        [HelpBox("Do not edit the \"Default Value\" field. It is useless for KeybindSettings.")]
        private Void _;
        #endif

        [field: FormerlySerializedAs("<InputAction>k__BackingField")]
        [field: SerializeField] public InputActionReference InputActionReference { get; private set; }

        public InputAction InputAction => GameInterface.Instance.GetInputHandler().InputActionAsset.FindAction(InputActionReference.action.id);


        [field: BindingIndex(nameof(InputActionReference))]
        [field: SerializeField] public int BindingIndex { get; private set; }

        protected override void LoadFromPlayerPrefs()
        {
            InputAction.LoadBindingOverridesFromJson(EnhancedPlayerPrefs.GetString($"{k_PlayerPrefsPrefix}{InputAction.id}_{Variant}"), false);
        }

        protected override void SaveToPlayerPrefs()
        {
            EnhancedPlayerPrefs.SetString($"{k_PlayerPrefsPrefix}{InputAction.id}_{Variant}", InputAction.SaveBindingOverridesAsJson());
        }
    }
}
