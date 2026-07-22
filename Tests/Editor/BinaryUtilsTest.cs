using System;
using BioluminescentGames.Utils.Utilities;
using NUnit.Framework;
using System.Text;
using BioluminescentGames.Utils.StaticUtilities;
using UnityEngine;

namespace BioluminescentGames.Utils.Tests.Editor
{
    public class BinaryUtilsTest
    {
        [Test]
        public void DecodeToString_WithValidUtf8Bytes_ReturnsCorrectString()
        {
            // Arrange
            byte[] utf8Bytes = Encoding.UTF8.GetBytes("Hello, World!");

            // Act
            string decodedString = utf8Bytes.DecodeToString();

            // Assert
            Assert.AreEqual("Hello, World!", decodedString);
        }

        [Test]
        public void DecodeToString_WithEmptyByteArray_ReturnsEmptyString()
        {
            // Arrange
            byte[] emptyBytes = Array.Empty<byte>();

            // Act
            string result = emptyBytes.DecodeToString();

            // Assert
            Assert.AreEqual(string.Empty, result);
        }

        [Test]
        public void DecodeToString_WithInvalidUtf8Bytes_ThrowsDecoderFallbackException()
        {
            if (Encoding.UTF8.DecoderFallback != DecoderFallback.ExceptionFallback)
                Assert.Ignore("Test only applies to UTF-8 decoder fallback");

            // Arrange
            byte[] invalidUtf8Bytes = { 0xFF, 0xFE };

            Log.Info(invalidUtf8Bytes.DecodeToString());

            // Act & Assert
            Assert.Throws<DecoderFallbackException>(() => invalidUtf8Bytes.DecodeToString());
        }

        [TestCase(new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F }, ExpectedResult = "Hello")]
        [TestCase(new byte[] { 0xE2, 0x9C, 0x94 }, ExpectedResult = "✔")] // UTF-8 checkmark
        [TestCase(new byte[] { 0x41, 0x42, 0x43 }, ExpectedResult = "ABC")]
        public string DecodeToString_TestCases(byte[] inputBytes)
        {
            // Act
            return inputBytes.DecodeToString();
        }
    }
}
