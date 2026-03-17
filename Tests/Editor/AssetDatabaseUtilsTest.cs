using BioluminescentGames.Utils.Editor.Utilities;
using NUnit.Framework;
using UnityEditor;

namespace BioluminescentGames.Utils.Tests.Editor
{
    [TestFixture]
    [TestOf(typeof(AssetDatabaseUtils))]
    public class AssetDatabaseUtilsTest
    {
        private const string k_Tree = "TestCreateFolderTree/Example1/Example2";
        private const string k_PathBase = "Assets/Test Related";

        [TearDown]
        [SetUp]
        public void SetupAndTearDown()
        {
            string[] splitTree = k_Tree.Split('/');

            if (AssetDatabase.IsValidFolder(k_PathBase + "/" + splitTree[0]))
                AssetDatabase.DeleteAsset(k_PathBase + "/" + splitTree[0]);
        }

        [Test]
        public void TestCreateFolderTree()
        {
            AssetDatabaseUtils.CreateFolderTree(k_PathBase + "/" + k_Tree);

            Assert.True(AssetDatabase.IsValidFolder(k_PathBase + "/" + k_Tree));
        }

    }
}
