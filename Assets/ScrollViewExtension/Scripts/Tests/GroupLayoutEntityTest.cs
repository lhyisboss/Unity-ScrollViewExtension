using System;
using NUnit.Framework;
using ScrollViewExtension.Scripts.Entity;
using UnityEngine;

namespace ScrollViewExtension.Scripts.Tests
{
    [TestFixture]
    public class GroupLayoutEntityTest
    {
        private GroupLayoutEntity entityInstance;

        [SetUp]
        public void Setup()
        {
            entityInstance = GroupLayoutEntity.CreateInstance(false,
                false,
                false,
                false,
                false,
                false,
                TextAnchor.UpperCenter,
                new Vector2(0, 1),
                new Vector2(1, 1));
        }

        [TearDown]
        public void TearDown()
        {
            entityInstance = null;
        }

        [Test]
        public void Test_CreateInstance()
        {
            Assert.NotNull(entityInstance);
        }

        [Test]
        public void ChildForceExpandWidth_Set_True_Throws_Exception()
        {
            Assert.Throws<Exception>(() => entityInstance.ChildForceExpandWidth = true);
        }

        [Test]
        public void ChildForceExpandHeight_Set_True_Throws_Exception()
        {
            Assert.Throws<Exception>(() => entityInstance.ChildForceExpandHeight = true);
        }

        [Test]
        public void ReverseArrangement_Set_True_Throws_Exception()
        {
            Assert.Throws<Exception>(() => entityInstance.ReverseArrangement = true);
        }

        [Test]
        public void ChildControlWidth_Set_True_Throws_Exception()
        {
            Assert.Throws<Exception>(() => entityInstance.ChildControlWidth = true);
        }

        [Test]
        public void ChildControlHeight_Set_True_Throws_Exception()
        {
            entityInstance = GroupLayoutEntity.CreateInstance(true,
                false,
                false,
                false,
                false,
                false,
                TextAnchor.UpperCenter,
                new Vector2(0, 1),
                new Vector2(1, 1));
            Assert.Throws<Exception>(() => entityInstance.ChildControlHeight = true);
        }

        [Test]
        public void ChildAlignment_Set_NotUpper_Throws_Exception()
        {
            Assert.Throws<Exception>(() => entityInstance.ChildAlignment = TextAnchor.LowerCenter);
        }

        [Test]
        public void ChildScaleWidth_Set_Get_CorrectValue()
        {
            entityInstance.ChildScaleWidth = true;

            Assert.IsTrue(entityInstance.ChildScaleWidth);
        }

        [Test]
        public void ChildScaleHeight_Set_Get_CorrectValue()
        {
            entityInstance.ChildScaleHeight = true;

            Assert.IsTrue(entityInstance.ChildScaleHeight);
        }
        
        [Test]
        public void TestMinAnchor_SetValidValue_ShouldNotThrowException()
        {
            Assert.DoesNotThrow(() => entityInstance.MinAnchor = new Vector2(0, 1));
        }

        [Test]
        public void TestMinAnchor_SetInvalidValue_ShouldThrowException()
        {
            Assert.Throws<Exception>(() => entityInstance.MinAnchor = new Vector2(0.5f, 1));
        }

        [Test]
        public void TestMaxAnchor_SetValidValue_ShouldNotThrowException()
        {
            Assert.DoesNotThrow(() => entityInstance.MaxAnchor = new Vector2(1, 1));
        }

        [Test]
        public void TestMaxAnchor_SetInvalidValue_ShouldThrowException()
        {
            Assert.Throws<Exception>(() => entityInstance.MaxAnchor = new Vector2(1, 0.5f));
        }
    }
}