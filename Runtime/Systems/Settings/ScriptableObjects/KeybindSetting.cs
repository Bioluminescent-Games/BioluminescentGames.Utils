using BioluminescentGames.Utils.Utilities;
using EditorAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BioluminescentGames.Utils.Systems.Settings.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Keybind Setting", menuName = "Scriptable Objects/Settings/Keybind")]
    public class KeybindSetting : SavableSetting
    {
        [HelpBox("Do not edit the \"Default Value\" field.")]
        private Void _;

        [field: SerializeField] public InputActionReference InputAction { get; private set; }

        protected override void LoadFromPlayerPrefs()
        {
            InputAction.action.LoadBindingOverridesFromJson(EnhancedPlayerPrefs.GetString(ID));
        }

        protected override void SaveToPlayerPrefs()
        {
            EnhancedPlayerPrefs.SetString(ID, InputAction.action.SaveBindingOverridesAsJson());
        }
    }
}
