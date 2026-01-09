// D:\UnityProjects\BioluminescentGames.Utils\Assets\BioluminescentGames.Utils\Tests\Editor\GameObjectUtilsTest.cs

using BioluminescentGames.Utils.Utilities;
using NUnit.Framework;
using UnityEngine;

namespace BioluminescentGames.Utils.Tests.Editor
{
    [TestFixture]
    [TestOf(typeof(GameObjectUtils))]
    public class GameObjectUtilsTest
    {
        [Test]
        public void ToggleActive_TogglesActiveState()
        {
            GameObject obj = new();
            obj.SetActive(true);

            obj.ToggleActive();
            Assert.IsFalse(obj.activeSelf, "ToggleActive should deactivate the object.");

            obj.ToggleActive();
            Assert.IsTrue(obj.activeSelf, "ToggleActive should activate the object.");

            Object.DestroyImmediate(obj);
        }

        [Test]
        public void Show_ActivatesGameObject()
        {
            GameObject obj = new();
            obj.SetActive(false);

            obj.Show();
            Assert.IsTrue(obj.activeSelf, "Show should activate the object.");

            Object.DestroyImmediate(obj);
        }

        [Test]
        public void Hide_DeactivatesGameObject()
        {
            GameObject obj = new();
            obj.SetActive(true);

            obj.Hide();
            Assert.IsFalse(obj.activeSelf, "Hide should deactivate the object.");

            Object.DestroyImmediate(obj);
        }

        [Test]
        public void HasComponent_ReturnsTrueIfComponentExists()
        {
            GameObject obj = new();
            obj.AddComponent<Rigidbody>();

            Assert.IsTrue(obj.HasComponent<Rigidbody>(), "HasComponent should return true if the component exists.");

            Object.DestroyImmediate(obj);
        }

        [Test]
        public void HasComponent_ReturnsFalseIfComponentDoesNotExist()
        {
            GameObject obj = new();

            Assert.IsFalse(obj.HasComponent<Rigidbody>(),
                "HasComponent should return false if the component does not exist.");

            Object.DestroyImmediate(obj);
        }

        [Test]
        public void GetOrAddComponent_GetsExistingComponent()
        {
            GameObject obj = new();
            Rigidbody rigidbody = obj.AddComponent<Rigidbody>();

            Rigidbody result = obj.GetOrAddComponent<Rigidbody>();

            Assert.AreSame(rigidbody, result, "GetOrAddComponent should return the existing component.");

            Object.DestroyImmediate(obj);
        }

        [Test]
        public void GetOrAddComponent_AddsNewComponentIfNotExists()
        {
            GameObject obj = new();

            Rigidbody result = obj.GetOrAddComponent<Rigidbody>();

            Assert.IsNotNull(result, "GetOrAddComponent should add a new component if it does not exist.");
            Assert.IsTrue(obj.HasComponent<Rigidbody>(), "GameObject should have the newly added component.");

            Object.DestroyImmediate(obj);
        }

        [Test]
        public void AddComponentCustom_AddsComponentCorrectly()
        {
            GameObject obj = new();

            Rigidbody result = obj.AddComponentCustom<Rigidbody>();

            Assert.IsNotNull(result, "AddComponentCustom should add a new component.");
            Assert.IsTrue(obj.HasComponent<Rigidbody>(), "GameObject should have the newly added component.");

            Object.DestroyImmediate(obj);
        }

        [Test]
        public void OrNull_ReturnsObjectIfNotDestroyedOrNull()
        {
            GameObject obj = new();

            GameObject result = obj.OrNull();

            Assert.AreSame(obj, result, "OrNull should return the object if it is not destroyed.");
            Assert.IsNotNull(result);

            Object.DestroyImmediate(obj);
        }

        [Test]
        public void OrNull_ReturnsNullIfDestroyed()
        {
            GameObject obj = new();
            Object.DestroyImmediate(obj);

            GameObject result = obj.OrNull();

            Assert.IsNull(result, "OrNull should return null if the object is destroyed.");
        }

        [Test]
        public void Destroy_DestroysGameObject()
        {
            GameObject obj = new();

            obj.Destroy();
            Assert.IsTrue(obj == null, "Destroy should destroy the GameObject.");
        }
    }
}
