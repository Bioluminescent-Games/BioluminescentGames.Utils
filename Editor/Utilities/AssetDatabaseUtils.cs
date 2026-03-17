using UnityEditor;

namespace BioluminescentGames.Utils.Editor.Utilities
{
    public static class AssetDatabaseUtils
    {
        public static void CreateFolderTree(string tree)
        {
            string[] splitTree = tree.Trim('/').Split('/');

            for (int i = 0; i < splitTree.Length; i++)
            {
                string parentPath = "";
                for (int j = 0; j < i; j++)
                    parentPath += splitTree[j] + "/";
                if (string.IsNullOrWhiteSpace(parentPath))
                    continue;
                string path = parentPath + splitTree[i];
                if (AssetDatabase.IsValidFolder(path))
                    continue;
                AssetDatabase.CreateFolder(parentPath.TrimEnd('/'), splitTree[i]);
            }

            AssetDatabase.Refresh();
        }
    }
}
