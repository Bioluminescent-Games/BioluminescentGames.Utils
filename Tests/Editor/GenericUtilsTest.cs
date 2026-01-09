using BioluminescentGames.Utils.Utilities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BioluminescentGames.Utils.Tests.Editor
{
    [TestFixture]
    [TestOf(typeof(GenericUtils))]
    public class GenericUtilsTest
    {
        [Test]
        public void PlayerPrefsGetBool_ReturnsCorrectValue_WhenKeyExists()
        {
            // Arrange
            const string key = "testKey";
            const bool expectedValue = true;
            PlayerPrefs.SetInt(key, 1);

            // Act
            bool result = GenericUtils.PlayerPrefsGetBool(key);

            // Assert
            Assert.AreEqual(expectedValue, result);

            // Cleanup
            PlayerPrefs.DeleteKey(key);
        }

        [Test]
        public void PlayerPrefsGetBool_ReturnsFallback_WhenKeyDoesNotExist()
        {
            // Arrange
            const string key = "nonExistentKey";
            const bool fallback = true;

            // Act
            bool result = GenericUtils.PlayerPrefsGetBool(key, fallback);

            // Assert
            Assert.AreEqual(fallback, result);
        }

        [Test]
        public void PlayerPrefsSetBool_SetsCorrectValue()
        {
            // Arrange
            const string key = "testKey";
            const bool value = true;

            // Act
            GenericUtils.PlayerPrefsSetBool(key, value);
            bool storedValue = PlayerPrefs.GetInt(key) != 0;

            // Assert
            Assert.AreEqual(value, storedValue);

            // Cleanup
            PlayerPrefs.DeleteKey(key);
        }

        [Test]
        public void Dispose_DisposesAllItems_InEnumerable()
        {
            // Arrange
            TestDisposable disposable1 = new();
            TestDisposable disposable2 = new();
            List<IDisposable> disposables = new() { disposable1, disposable2 };

            // Act
            disposables.Dispose();

            // Assert
            Assert.IsTrue(disposable1.IsDisposed);
            Assert.IsTrue(disposable2.IsDisposed);
        }

        [Test]
        public void Get16BitHash_ReturnsDeterministicHash()
        {
            // Arrange
            const string input = "testString";
            ushort expectedHash = GenericUtils.Get16BitHash(input);

            // Act
            ushort result = GenericUtils.Get16BitHash(input);

            // Assert
            Assert.AreEqual(expectedHash, result);
        }

        [Test]
        public void Get16BitHash_ReturnsDifferentHash_ForDifferentInputs()
        {
            // Arrange
            const string input1 = "stringOne";
            const string input2 = "stringTwo";

            // Act
            ushort hash1 = GenericUtils.Get16BitHash(input1);
            ushort hash2 = GenericUtils.Get16BitHash(input2);

            // Assert
            Assert.AreNotEqual(hash1, hash2);
        }
    }

    // Helper class for IDisposable testing
    public class TestDisposable : IDisposable
    {
        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}
