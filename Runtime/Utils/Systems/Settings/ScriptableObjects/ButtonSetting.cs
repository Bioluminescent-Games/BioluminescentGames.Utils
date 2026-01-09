using System;
using UnityEngine;

namespace BackroomsGame.Systems.Settings.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Button Setting",
        menuName = "Scriptable Objects/Settings/Button Setting")]
    public class ButtonSetting : Setting
    {
        /// <summary>
        /// When the button is pressed.
        /// </summary>
        public event Action OnButtonPressed;
        
        /// <summary>
        /// NOT PUBLIC API - DO NOT CALL.
        /// </summary>
        public void ButtonPressed() => OnButtonPressed?.Invoke();
    }
}
