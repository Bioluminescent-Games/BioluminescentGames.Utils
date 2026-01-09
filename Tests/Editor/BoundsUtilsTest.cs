using BioluminescentGames.Utils.Utilities;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace BioluminescentGames.Utils.Tests.Editor
{
    public class BoundsUtilsTest
    {
        [Test]
        public void SetBounds_SetsCorrectCenterAndSize()
        {
            BoxCollider collider = new GameObject().AddComponent<BoxCollider>();
            Bounds bounds = new(Vector3.one, Vector3.one * 2);

            collider.SetBounds(bounds);

            Assert.AreEqual(bounds.center, collider.center);
            Assert.AreEqual(bounds.extents, collider.size);
        }

        [Test]
        public void EncapsulateAll_EncapsulatesCorrectly()
        {
            Bounds bounds = new(Vector3.zero, Vector3.one * 2);
            List<Bounds> boundsToEncapsulate = new()
            {
                new Bounds(Vector3.one * 3, Vector3.one * 2),
                new Bounds(Vector3.one * -3, Vector3.one * 2)
            };

            bounds.EncapsulateAll(boundsToEncapsulate);

            Assert.AreEqual(new Vector3(-4, -4, -4), bounds.min);
            Assert.AreEqual(new Vector3(4, 4, 4), bounds.max);
        }

        [TestCase(6, 6, 6, ExpectedResult = "5, 5, 5")]
        [TestCase(-6, -6, -6, ExpectedResult = "-5, -5, -5")]
        [TestCase(3, 3, 3, ExpectedResult = "3, 3, 3")]
        public string ConfineVector_ConfinesVectorCorrectly(float x, float y, float z)
        {
            Bounds bounds = new(Vector3.zero, Vector3.one * 10);
            Vector3 vector = new(x, y, z);

            bounds.ConfineVector(ref vector);

            return $"{vector.x}, {vector.y}, {vector.z}";
        }

        [Test]
        public void GetBoundsEncapsulating_ReturnsCorrectBounds()
        {
            List<Bounds> boundsList = new()
            {
                new Bounds(Vector3.one * 3, Vector3.one * 2),
                new Bounds(Vector3.one * -3, Vector3.one * 2),
                new Bounds(Vector3.zero, Vector3.one * 2)
            };

            Bounds result = BoundsUtils.GetBoundsEncapsulating(boundsList);

            Assert.AreEqual(new Vector3(-4, -4, -4), result.min);
            Assert.AreEqual(new Vector3(4, 4, 4), result.max);
        }

        [Test]
        public void Translate_TranslatesBoundsCorrectly()
        {
            Bounds bounds = new(Vector3.zero, Vector3.one * 2);
            GameObject obj = new()
            {
                transform =
                {
                    position = Vector3.one * 5
                }
            };

            Bounds translatedBounds = bounds.Translate(obj.transform);

            Assert.AreEqual(Vector3.one * 5, translatedBounds.center);
            Assert.AreEqual(Vector3.one, translatedBounds.extents);
        }

        [Test]
        public void InverseTranslate_InverselyTranslatesBoundsCorrectly()
        {
            Bounds bounds = new(Vector3.zero, Vector3.one * 2);
            GameObject obj = new()
            {
                transform =
                {
                    position = Vector3.one * 5
                }
            };

            Bounds translatedBounds = bounds.Translate(obj.transform);
            Bounds inverseTranslatedBounds = translatedBounds.InverseTranslate(obj.transform);

            Assert.AreEqual(bounds.center, inverseTranslatedBounds.center);
            Assert.AreEqual(bounds.size, inverseTranslatedBounds.size);
        }
    }
}
