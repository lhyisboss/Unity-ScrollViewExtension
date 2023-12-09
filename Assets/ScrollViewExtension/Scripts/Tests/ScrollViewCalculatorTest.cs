using System;
using NUnit.Framework;
using ScrollViewExtension.Scripts.Entity;
using ScrollViewExtension.Scripts.Entity.Interface;
using ScrollViewExtension.Scripts.Service;
using ScrollViewExtension.Scripts.Service.Interface;
using ScrollViewExtension.Scripts.UseCase;
using ScrollViewExtension.Scripts.UseCase.Interface;
using UnityEngine;

namespace ScrollViewExtension.Scripts.Tests
{
    /// <summary>
    /// 手間を省くために何個のテストを一つの関数に入れました
    /// </summary>
    [TestFixture]
    public class ScrollViewCalculatorTest
    {
        private IScrollViewCalculator calculator;
        private IScrollViewEntity<TestScrollItem> viewEntity;
        private IFindIndex<TestScrollItem> findIndex;

        [SetUp]
        public void SetUp()
        {
            //mockフレームワーク使いたいですが、moqなどのフレームワークを手動導入しなければならないので、とりあえず手動でmockデータを作りました
            viewEntity = ScrollViewEntity<TestScrollItem>.CreateInstance(
                new RectOffset(5, 0, 5, 0),
                10,
                new Vector2(200, 200),
                new Vector2(180, 0),
                false, true);
            findIndex = FindIndex<TestScrollItem>.CreateInstance();
            calculator = ScrollViewCalculator<TestScrollItem>.CreateInstance(viewEntity, findIndex);

            viewEntity.CreateItem(new Vector2(200, 50), new Vector2(0, 0));
            viewEntity.CreateItem(new Vector2(200, 50), new Vector2(205, -55));
            viewEntity.CreateItem(new Vector2(200, 100), new Vector2(415, -115));
            viewEntity.CreateItem(new Vector2(200, 50), new Vector2(625, -225));
            viewEntity.CreateItem(new Vector2(200, 50), new Vector2(835, -285));
            viewEntity.CreateItem(new Vector2(200, 50), new Vector2(1045, -345));
            viewEntity.CreateItem(new Vector2(200, 50), new Vector2(1255, -405));
            viewEntity.CreateItem(new Vector2(200, 50), new Vector2(1465, -465));
            viewEntity.CreateItem(new Vector2(200, 50), new Vector2(1675, -525));
            viewEntity.CreateItem(new Vector2(200, 50.55f), new Vector2(1885, -585));
        }

        [TearDown]
        public void TearDown()
        {
            viewEntity.Dispose();
            findIndex.Dispose();
            calculator.Dispose();
        }

        [Test]
        public void CalculateInstanceNumber_ReturnsExpectedNumber()
        {
            var actualInstanceNumber = calculator.CalculateInstanceNumber();
            Assert.AreEqual(5, actualInstanceNumber);

            viewEntity.SetViewLength = new Vector2(100, 100);

            actualInstanceNumber = calculator.CalculateInstanceNumber();
            Assert.AreEqual(3, actualInstanceNumber);
            
            viewEntity.SetViewLength = new Vector2(200, 200);
            viewEntity.Data.Clear();

            viewEntity.CreateItem(new Vector2(200, 50), new Vector2(0, 0));
            viewEntity.CreateItem(new Vector2(200, 50), new Vector2(205, -55));
            viewEntity.CreateItem(new Vector2(200, 100), new Vector2(415, -115));
            
            actualInstanceNumber = calculator.CalculateInstanceNumber();
            Assert.AreEqual(3, actualInstanceNumber);
        }

        [Test]
        public void CalculateContentSize_ReturnsExpectedSize()
        {
            // Act: call the method
            var actualSize = calculator.CalculateContentSize();

            // Assert: check that the result is as expected
            Assert.AreEqual(645.55f, actualSize.y, 0.01f);
        }

        [Test]
        public void CalculateBarPosition_ReturnsExpectedPosition()
        {
            // Arrange
            var index = 0;
            var actualPosition = calculator.CalculateBarPosition(index);
            Assert.AreEqual(1, actualPosition);

            index = 2;
            actualPosition = calculator.CalculateBarPosition(index);

            Assert.AreEqual(0.7415f, actualPosition, 0.01f);
        }

        [Test]
        public void CalculateRolling_ReturnsExpectedRolling()
        {
            // Arrange
            var currentPadding = new Vector4(120,0,0,0);
            var newPadding = new Vector4(290, 0, 0, 0);
            var startIndex = 2;

            // Act
            var actualRolling = calculator.CalculateRolling(currentPadding, newPadding, startIndex);
            Assert.AreEqual(2, actualRolling);
            
            currentPadding = new Vector4(290, 0, 0, 0);
            actualRolling = calculator.CalculateRolling(currentPadding, newPadding, startIndex);
            Assert.AreEqual(0, actualRolling);
            
            currentPadding = new Vector4(350, 0, 0, 0);
            newPadding  = new Vector4(230, 0, 0, 0);
            startIndex = 5;
            actualRolling = calculator.CalculateRolling(currentPadding, newPadding, startIndex);
            Assert.AreEqual(-2, actualRolling);
        }
        
        [Test]
        public void CalculateRolling_WithDecimalPadding_ShouldReturnExpectedValue()
        {
            viewEntity.Data.Clear();

            viewEntity.CreateItem(new Vector2(200, 57.75f), new Vector2(0, 0));
            viewEntity.CreateItem(new Vector2(200, 67.45f), new Vector2(205, -62.75f));
            viewEntity.CreateItem(new Vector2(200, 77.15f), new Vector2(415, -140.2f));
            viewEntity.CreateItem(new Vector2(200, 86.85f), new Vector2(625, -227.35f));
            viewEntity.CreateItem(new Vector2(200, 96.55f), new Vector2(835, -324.2f));
            viewEntity.CreateItem(new Vector2(200, 106.25f), new Vector2(1045, -430.75f));
            viewEntity.CreateItem(new Vector2(200, 115.95f), new Vector2(1255, -547));
            
            // Arrange
            var currentPadding = new Vector4(5f, 0, 0, 0);
            var newPadding = new Vector4(67.75f, 0, 0, 0);
            int startIndex = 0;

            // Act
            var result = calculator.CalculateRolling(currentPadding, newPadding, startIndex);
    
            // Assert
            Assert.AreEqual(1, result);
            
            currentPadding = new Vector4(67.75f, 0, 0, 0);
            newPadding = new Vector4(145.2f, 0, 0, 0);
            startIndex = 1;
            
            result = calculator.CalculateRolling(currentPadding, newPadding, startIndex);
            Assert.AreEqual(1, result);
        }

        [Test]
        public void CalculateOffset_ReturnsExpectedOffset()
        {
            // Arrange
            var index =  0;
            var count =  5;
            var contentPos = new Vector3(0, 0, 0);

            // Act
            var actualOffset = calculator.CalculateOffset(index, count, contentPos);
            Assert.AreEqual(new Vector4(5, 0, 5, 0), actualOffset);

            index = 1;
            contentPos = new Vector3(0, 119.05f, 0);
            actualOffset = calculator.CalculateOffset(index, count, contentPos);
            Assert.AreEqual(new Vector4(120, 0, 5, 0), actualOffset);

            index = 5;
            contentPos = new Vector3(0, 445, 0);
            actualOffset = calculator.CalculateOffset(index, count, contentPos);
            Assert.AreEqual(new Vector4(350, 0, 5, 0), actualOffset);
        }

        [Test]
        public void CalculateOffset_ThrowsExceptionForCountZero()
        {
            // Arrange
            var index =  0;
            var count = 0;
            var contentPos = new Vector3(0, 0, 0);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => calculator.CalculateOffset(index, count, contentPos));
        }
    }
}