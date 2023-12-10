using System;
using NUnit.Framework;
using ScrollViewExtension.Scripts.Core.Entity;
using ScrollViewExtension.Scripts.DTO;
using UnityEngine;

namespace ScrollViewExtension.Scripts.Tests
{
    public class TestScrollItem : ScrollItemBase
    {
        public TestScrollItem() : base(new Vector2(200,50))
        {
        }
    }

    /// <summary>
    /// 手間を省くために何個のテストを一つの関数に入れました
    /// </summary>
    [TestFixture]
    public class ScrollViewEntityTestVertical
    {
        private ScrollViewEntity<TestScrollItem> instance;

        [SetUp]
        public void SetUp()
        {
            using (instance = ScrollViewEntity<TestScrollItem>.CreateInstance(
                       new RectOffset(5, 0, 5, 0),
                       10,
                       new Vector2(200, 200),
                       new Vector2(180, 0),
                       true
                   ))
            {
            }
        }

        [Test]
        public void CreateInstance_WithValidParameters_ReturnsInstance()
        {
            Assert.NotNull(instance);
        }

        [Test]
        public void IsVertical_WhenHorizontalFalseAndVerticalTrue_ReturnsTrue()
        {
            Assert.IsTrue(instance.IsVertical);
        }

        [Test]
        public void SetGetViewLength_WithValidParameters_ReturnsSetValue()
        {
            instance.SetViewLength = new Vector2(60, 60);

            Assert.AreEqual(60, instance.GetViewLength);
        }

        [Test]
        public void CreateItem_WithValidParameters_ReturnsCorrectItem()
        {
            var item = instance.CreateItem(new Vector2(20, 20), new Vector2(5, 5));

            Assert.AreEqual(0, item.Index);
            Assert.AreEqual(new Vector2(20, 20), item.Size);
            Assert.AreEqual(new Vector2(5, 5), item.Position);
        }

        [Test]
        public void GetRange_WithValidParameters_ReturnsCorrectRange()
        {
            for (int i = 0; i < 10; i++)
            {
                instance.CreateItem(new Vector2(i, i), new Vector2(i, i));
            }

            var range = instance.GetRange(5, 2);
            Assert.AreEqual(2, range.Count);
            Assert.AreEqual(5, range[0].Index);
            Assert.AreEqual(6, range[1].Index);
        }

        [Test]
        public void GetItemMinLength_WithValidParameters_ReturnsCorrectValue()
        {
            for (int i = 10; i <= 20; i++)
            {
                instance.CreateItem(new Vector2(i, i), new Vector2(i, i));
            }

            Assert.AreEqual(10, instance.GetItemMinLength());
        }

        [Test]
        public void TestDefaultPaddingException()
        {
            Assert.Throws<Exception>(() => instance.DefaultPadding = new RectOffset(-1, 0, -1, 0));
        }

        [Test]
        public void TestSpacingException()
        {
            Assert.Throws<Exception>(() => instance.Spacing = -1);
        }

        [Test]
        public void TestGetRangeException()
        {
            Assert.Throws<ArgumentException>(() => instance.GetRange(-1, 0));
        }
        
        [Test]
        public void GetContentLength_SpecifiedItems_LengthIsCorrect()
        {
            instance.CreateItem(new Vector2(1, 1), new Vector2(1, 1));
            instance.CreateItem(new Vector2(2, 2), new Vector2(2, 2));
            instance.CreateItem(new Vector2(3, 3), new Vector2(3, 3));
            instance.CreateItem(new Vector2(4, 4.5f), new Vector2(4, 4));
            
            var length = instance.GetContentLength(instance.Data.Count);
            Assert.AreEqual(45.5f, length, 0.01f);

            length = instance.GetContentLength(1);
            Assert.AreEqual(6, length, 0.01f); 
            
            length = instance.GetContentLength(2, 1);
            Assert.AreEqual(25, length, 0.01f);
            
            length = instance.GetContentLength(2, 2);
            Assert.AreEqual(27.5f, length, 0.01f);
        }

        [Test]
        public void TestCalculateItemPosition()
        {
            var containerView = ScrollViewEntity<TestScrollItem>.CreateInstance(new RectOffset(5, 5, 5, 5),
                10f,
                new Vector2(100, 100),
                new Vector2(200, 200),
                false);
            // Let's add some items
            containerView.CreateItem(new Vector2(20, 20), new Vector2(0, 0)); // Item 0
            containerView.CreateItem(new Vector2(30, 30), new Vector2(25, -25)); // Item 1
            containerView.CreateItem(new Vector2(40, 40), new Vector2(65, -65)); // Item 2
            containerView.CreateItem(new Vector2(50, 54.12f), new Vector2(115, -115)); // Item 3
            containerView.CreateItem(new Vector2(60, 62.33f), new Vector2(175, -179.12f)); // Item 4

            // Now let's test the CalculateItemPosition method
            // The first item should have position (0,0)
            Assert.AreEqual(new Vector2(0, 0), containerView.CalculateItemPosition(0));

            Assert.AreEqual(new Vector2(25, -25), containerView.CalculateItemPosition(1));

            Assert.AreEqual(new Vector2(65, -65), containerView.CalculateItemPosition(2));
            
            Assert.AreEqual(new Vector2(115, -115), containerView.CalculateItemPosition(3));
            
            Assert.AreEqual(new Vector2(175, -179.12f), containerView.CalculateItemPosition(4));
        }
        
        [Test]
        public void UpdateItemPosition_Successful_When_Valid_Index_And_Position_Are_Provided()
        {
            instance.CreateItem(new Vector2(200, 50), new Vector2(0, 0));
            
            instance.UpdateItemPosition(0, new Vector2(10, 20));
            Assert.AreEqual(new Vector2(10, 20), instance.Data[0].Position);
        }

        [Test]
        public void UpdateItemPosition_Throws_Exception_When_Invalid_Index_Is_Provided()
        {
            Assert.Throws<ArgumentException>(() => instance.UpdateItemPosition(-1, new Vector2(10, 20)));
            Assert.Throws<ArgumentException>(() => instance.UpdateItemPosition(2, new Vector2(10, 20)));
        }

        [Test]
        public void TestGetItemSize()
        {
            var containerView = ScrollViewEntity<TestScrollItem>.CreateInstance(new RectOffset(5, 5, 5, 5),
                10f,
                new Vector2(100, 100),
                new Vector2(200, 200),
                false);
            containerView.CreateItem(new Vector2(20, 20), new Vector2(0, 0)); // Item 0
            containerView.CreateItem(new Vector2(30, 30), new Vector2(35, 35)); // Item 1
            containerView.CreateItem(new Vector2(40, 40), new Vector2(75, 75)); // Item 2

            // The first item's width should be equal to its size plus the left padding
            Assert.AreEqual(20 + 5, containerView.GetItemSize(0));

            // The size of any other item should be equal to its width plus the spacing
            Assert.AreEqual(30 + 10, containerView.GetItemSize(1));
            Assert.AreEqual(40 + 10, containerView.GetItemSize(2));
        }
    }
}