using System;
using UnityEngine;

namespace BioluminescentGames.Utils.Systems.Settings.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Button Setting",
        menuName = "Scriptable Objects/Settings/Button Setting")]
    public class ButtonSetting : Setting
    {
        /// <summary>
        /// When the button is pressed.
        /// </summary>
        public event Action OnButtonPressed;

        internal void ButtonPressed() => OnButtonPressed?.Invoke();
    }
}
