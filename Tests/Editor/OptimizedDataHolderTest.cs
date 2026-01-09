using System;
using BioluminescentGames.Utils.Utilities;
using NUnit.Framework;

namespace BioluminescentGames.Utils.Tests.Editor
{
    [TestFixture]
    [TestOf(typeof(OptimizedDataHolder<>))]
    public class OptimizedDataHolderTest
    {
        public class TestHeldValue : IEquatable<TestHeldValue>
        {
            public readonly string SomeData;

            private TestHeldValue(string someData)
            {
                SomeData = someData;
            }

            public TestHeldValue()
            {
                SomeData = "Default Value";
            }

            public static implicit operator TestHeldValue(string value) => new(value);
            public static implicit operator string(TestHeldValue value) => value.SomeData;

            public bool Equals(TestHeldValue other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;
                return SomeData == other.SomeData;
            }

            public override bool Equals(object obj)
            {
                if (obj is null) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != GetType()) return false;
                return Equals((TestHeldValue)obj);
            }

            public override int GetHashCode()
            {
                return SomeData != null ? SomeData.GetHashCode() : 0;
            }

            public static bool operator ==(TestHeldValue left, TestHeldValue right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(TestHeldValue left, TestHeldValue right)
            {
                return !Equals(left, right);
            }
        }

        [Test]
        public void Constructor_SetsInitialState_ValueIsNull()
        {
            OptimizedDataHolder<TestHeldValue> holder = new();
            Assert.IsTrue(holder.ValueIsNull);
            Assert.IsFalse(holder.ValueIsNotNull);
            Assert.IsNull(holder.Value);
        }

        [Test]
        public void Constructor_WithInitialValue_SetsValueCorrectly()
        {
            OptimizedDataHolder<TestHeldValue> holder = new("Initial Value");
            Assert.IsFalse(holder.ValueIsNull);
            Assert.IsTrue(holder.ValueIsNotNull);
            Assert.AreEqual("Initial Value", holder.Value.SomeData);
        }

        [Test]
        public void DeleteValue_ResetsStateToNull()
        {
            OptimizedDataHolder<TestHeldValue> holder = new("Initial Value");
            holder.DeleteValue();
            Assert.IsTrue(holder.ValueIsNull);
            Assert.IsFalse(holder.ValueIsNotNull);
            Assert.IsNull(holder.Value);
        }

        [Test]
        public void Set_ValueUpdatesCorrectly()
        {
            OptimizedDataHolder<TestHeldValue> holder = new();
            holder.Set("Updated Value");
            Assert.IsFalse(holder.ValueIsNull);
            Assert.IsTrue(holder.ValueIsNotNull);
            Assert.AreEqual("Updated Value", holder.Value.SomeData);
        }

        [Test]
        public void Set_WithNullValue_SetsStateToNull()
        {
            OptimizedDataHolder<TestHeldValue> holder = new("Initial Value");
            holder.Set(null);
            Assert.IsTrue(holder.ValueIsNull);
            Assert.IsFalse(holder.ValueIsNotNull);
            Assert.IsNull(holder.Value);
        }

        [Test]
        public void Set_WithExplicitIsNull_SetsStateCorrectly()
        {
            OptimizedDataHolder<TestHeldValue> holder = new();
            holder.Set("Non-Null Value", false);
            Assert.IsFalse(holder.ValueIsNull);
            Assert.IsTrue(holder.ValueIsNotNull);
            Assert.AreEqual("Non-Null Value", holder.Value.SomeData);

            holder.Set(null, true);
            Assert.IsTrue(holder.ValueIsNull);
            Assert.IsFalse(holder.ValueIsNotNull);
            Assert.IsNull(holder.Value);
        }

        [Test]
        public void Get_ReturnsCurrentValue()
        {
            OptimizedDataHolder<TestHeldValue> holder = new("Initial Value");
            Assert.AreEqual("Initial Value", holder.Get().SomeData);
        }

        [Test]
        public void GetOrCreate_WithValue_ReturnsExistingValue()
        {
            OptimizedDataHolder<TestHeldValue> holder = new("Existing Value");
            TestHeldValue value = holder.GetOrCreate();
            Assert.AreEqual("Existing Value", value.SomeData);
            Assert.IsFalse(holder.ValueIsNull);
            Assert.IsTrue(holder.ValueIsNotNull);
        }

        [Test]
        public void GetOrCreate_WithNullValue_CreatesDefault()
        {
            OptimizedDataHolder<TestHeldValue> holder = new();
            TestHeldValue value = holder.GetOrCreate();
            Assert.IsNotNull(value);
            Assert.IsFalse(holder.ValueIsNull);
            Assert.IsTrue(holder.ValueIsNotNull);
        }

        [Test]
        public void CreateDefault_ReplacesValueWithNewDefault()
        {
            OptimizedDataHolder<TestHeldValue> holder = new("Previous Value");
            TestHeldValue newValue = holder.CreateDefault();
            Assert.IsNotNull(newValue);
            Assert.AreEqual(newValue.SomeData, holder.Value.SomeData);
            Assert.IsFalse(holder.ValueIsNull);
            Assert.IsTrue(holder.ValueIsNotNull);
        }

        [Test]
        public void ValueProperty_GetAndSet_WorksCorrectly()
        {
            OptimizedDataHolder<TestHeldValue> holder = new()
            {
                Value = "Test Value"
            };

            Assert.IsFalse(holder.ValueIsNull);
            Assert.IsTrue(holder.ValueIsNotNull);
            Assert.AreEqual("Test Value", holder.Value.SomeData);

            holder.Value = null;
            Assert.IsTrue(holder.ValueIsNull);
            Assert.IsFalse(holder.ValueIsNotNull);
            Assert.IsNull(holder.Value);
        }
    }
}
