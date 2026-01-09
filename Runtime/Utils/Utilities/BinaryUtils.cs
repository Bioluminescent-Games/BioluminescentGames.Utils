#region

using System.Text;
using UnityEngine;

#endregion

namespace BioluminescentGames.Utils.Utilities
{
    public static class BinaryUtils
    {
        public static string DecodeToString(this byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
