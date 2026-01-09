using BioluminescentGames.Utils.Utilities;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace BioluminescentGames.Utils.Tests.Editor
{
    public class TransformUtilsTest
    {
        private GameObject _testGameObject;
        private Transform _testTransform;

        [SetUp]
        public void SetUp()
        {
            _testGameObject = new GameObject("TestObject");
            _testTransform = _testGameObject.transform;
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_testGameObject);
        }

        [Test]
        public void SetWorldX_SetsXCorrectly()
        {
            _testTransform.position = new Vector3(0, 10, 20);
            _testTransform.SetWorldX(50);
            Assert.AreEqual(new Vector3(50, 10, 20), _testTransform.position);
        }

        [Test]
        public void SetWorldY_SetsYCorrectly()
        {
            _testTransform.position = new Vector3(10, 0, 20);
            _testTransform.SetWorldY(50);
            Assert.AreEqual(new Vector3(10, 50, 20), _testTransform.position);
        }

        [Test]
        public void SetWorldZ_SetsZCorrectly()
        {
            _testTransform.position = new Vector3(10, 20, 0);
            _testTransform.SetWorldZ(50);
            Assert.AreEqual(new Vector3(10, 20, 50), _testTransform.position);
        }

        [Test]
        public void SetLocalX_SetsXCorrectly()
        {
            _testTransform.localPosition = new Vector3(0, 10, 20);
            _testTransform.SetLocalX(50);
            Assert.AreEqual(new Vector3(50, 10, 20), _testTransform.localPosition);
        }

        [Test]
        public void SetLocalY_SetsYCorrectly()
        {
            _testTransform.localPosition = new Vector3(10, 0, 20);
            _testTransform.SetLocalY(50);
            Assert.AreEqual(new Vector3(10, 50, 20), _testTransform.localPosition);
        }

        [Test]
        public void SetLocalZ_SetsZCorrectly()
        {
            _testTransform.localPosition = new Vector3(10, 20, 0);
            _testTransform.SetLocalZ(50);
            Assert.AreEqual(new Vector3(10, 20, 50), _testTransform.localPosition);
        }

        [Test]
        public void ChangeWorldX_ChangesXCorrectly()
        {
            _testTransform.position = new Vector3(10, 20, 30);
            _testTransform.ChangeWorldX(15);
            Assert.AreEqual(new Vector3(25, 20, 30), _testTransform.position);
        }

        [Test]
        public void ChangeWorldY_ChangesYCorrectly()
        {
            _testTransform.position = new Vector3(10, 20, 30);
            _testTransform.ChangeWorldY(15);
            Assert.AreEqual(new Vector3(10, 35, 30), _testTransform.position);
        }

        [Test]
        public void ChangeWorldZ_ChangesZCorrectly()
        {
            _testTransform.position = new Vector3(10, 20, 30);
            _testTransform.ChangeWorldZ(15);
            Assert.AreEqual(new Vector3(10, 20, 45), _testTransform.position);
        }

        [Test]
        public void ChangeLocalX_ChangesXCorrectly()
        {
            _testTransform.localPosition = new Vector3(10, 20, 30);
            _testTransform.ChangeLocalX(15);
            Assert.AreEqual(new Vector3(25, 20, 30), _testTransform.localPosition);
        }

        [Test]
        public void ChangeLocalY_ChangesYCorrectly()
        {
            _testTransform.localPosition = new Vector3(10, 20, 30);
            _testTransform.ChangeLocalY(15);
            Assert.AreEqual(new Vector3(10, 35, 30), _testTransform.localPosition);
        }

        [Test]
        public void ChangeLocalZ_ChangesZCorrectly()
        {
            _testTransform.localPosition = new Vector3(10, 20, 30);
            _testTransform.ChangeLocalZ(15);
            Assert.AreEqual(new Vector3(10, 20, 45), _testTransform.localPosition);
        }

        [Test]
        public void TransformAllPoints_TransformsPointsCorrectly()
        {
            Vector3[] points = new Vector3[]
            {
                new Vector3(1, 1, 1),
                new Vector3(2, 2, 2)
            };
            GameObject child = new("Child")
            {
                transform =
                {
                    parent = _testTransform
                }
            };

            Vector3[] transformedPoints = child.transform.TransformAllPoints(points);
            for (int i = 0; i < points.Length; i++)
                Assert.AreEqual(child.transform.TransformPoint(points[i]), transformedPoints[i]);
        }

        [Test]
        public void ClearAllChildren_RemovesAllChildren()
        {
            GameObject child1 = new GameObject("Child1");
            GameObject child2 = new GameObject("Child2");
            child1.transform.parent = _testTransform;
            child2.transform.parent = _testTransform;

            _testTransform.ClearAllChildren();

            Assert.AreEqual(0, _testTransform.childCount);
        }

        [Test]
        public void ClearChildren_RemovesOnlyMatchingChildren()
        {
            GameObject match = new GameObject("Match");
            GameObject noMatch = new GameObject("NoMatch");
            match.transform.parent = _testTransform;
            noMatch.transform.parent = _testTransform;

            _testTransform.ClearChildren(t => t.name == "Match");

            Assert.AreEqual(1, _testTransform.childCount);
            Assert.AreEqual("NoMatch", _testTransform.GetChild(0).name);
        }

        [Test]
        public void ClearChildren_RemovesNothingWhenPredicateMatchesNone()
        {
            GameObject child1 = new GameObject("Child1");
            GameObject child2 = new GameObject("Child2");
            child1.transform.parent = _testTransform;
            child2.transform.parent = _testTransform;

            _testTransform.ClearChildren(_ => false);

            Assert.AreEqual(2, _testTransform.childCount);
        }

        [Test]
        public void SetWorldScale_SetsScaleCorrectly()
        {
            _testTransform.localScale = new Vector3(1, 2, 3);
            _testTransform.SetWorldScale(new Vector3(4, 5, 6));
            Assert.AreEqual(new Vector3(4, 5, 6), _testTransform.lossyScale);
        }

        [Test]
        public void AsRectTransform_ReturnsCorrectly()
        {
            RectTransform rectTransform = _testGameObject.AddComponent<RectTransform>();
            Assert.AreEqual(rectTransform, rectTransform.AsRectTransform());
        }

        [UnityTest]
        public IEnumerator InterpolateLocal_InterpolatesCorrectly()
        {
            _testTransform.localPosition = Vector3.zero;
            _testTransform.InterpolateLocal(new Vector3(1, 2, 3), 10);

            yield return new WaitForSeconds(0.1f);

            Assert.AreNotEqual(Vector3.zero, _testTransform.localPosition);
        }

        [UnityTest]
        public IEnumerator InterpolateWorld_InterpolatesCorrectly()
        {
            _testTransform.position = Vector3.zero;
            _testTransform.InterpolateWorld(new Vector3(5, 5, 5), 10);

            yield return new WaitForSeconds(0.1f);

            Assert.AreNotEqual(Vector3.zero, _testTransform.position);
        }
    }
}
