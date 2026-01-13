using UnityEngine;
using UnityEngine.InputSystem;

namespace BioluminescentGames.Utils.Utilities
{
    public static class InputUtils
    {
        /// <summary>
        /// Is the input action currently down (not this frame)?
        /// </summary>
        /// <param name="inputAction">The input action</param>
        /// <returns>True if it is down</returns>
        public static bool IsInputDown(this InputAction inputAction)
        {
            return inputAction.IsPressed();
        }

        /// <summary>
        /// Was the input action down this frame?
        /// </summary>
        /// <param name="inputAction">The input action</param>
        /// <returns>True if the input action was down this frame</returns>
        public static bool IsInputDownThisFrame(this InputAction inputAction)
        {
            return inputAction.triggered;
        }
    }
}
