#region

using System.Text;

#endregion

namespace BioluminescentGames.Utils
{
    public static class BinaryUtils
    {
        public static string DecodeToString(this byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
