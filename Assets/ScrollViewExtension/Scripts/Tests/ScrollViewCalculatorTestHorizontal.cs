using System;
using NUnit.Framework;
using ScrollViewExtension.Scripts.Core.Entity;
using ScrollViewExtension.Scripts.Core.Entity.Interface;
using ScrollViewExtension.Scripts.Core.Service;
using ScrollViewExtension.Scripts.Core.Service.Interface;
using ScrollViewExtension.Scripts.Core.UseCase;
using ScrollViewExtension.Scripts.Core.UseCase.Interface;
using UnityEngine;

namespace ScrollViewExtension.Scripts.Tests
{
    [TestFixture]
    public class ScrollViewCalculatorTestHorizontal
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
                new Vector2(0, 0),
                false);
            findIndex = FindIndex<TestScrollItem>.CreateInstance();
            calculator = ScrollViewCalculator<TestScrollItem>.CreateInstance(viewEntity, findIndex);

            viewEntity.CreateItem(new Vector2(50, 50), new Vector2(0, 0));
            viewEntity.CreateItem(new Vector2(50, 50), new Vector2(65, -55));
            viewEntity.CreateItem(new Vector2(100, 100), new Vector2(125, -115));
            viewEntity.CreateItem(new Vector2(50, 50), new Vector2(235, -225));
            viewEntity.CreateItem(new Vector2(50, 50), new Vector2(295, -285));
            viewEntity.CreateItem(new Vector2(50, 50), new Vector2(355, -345));
            viewEntity.CreateItem(new Vector2(50, 50), new Vector2(415, -405));
            viewEntity.CreateItem(new Vector2(50, 50), new Vector2(475, -465));
            viewEntity.CreateItem(new Vector2(50, 50), new Vector2(535, -525));
            viewEntity.CreateItem(new Vector2(50.55f, 50.55f), new Vector2(595, -585));
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

            viewEntity.CreateItem(new Vector2(50, 50), new Vector2(0, 0));
            viewEntity.CreateItem(new Vector2(50, 50), new Vector2(205, -55));
            viewEntity.CreateItem(new Vector2(100, 100), new Vector2(415, -115));
            
            actualInstanceNumber = calculator.CalculateInstanceNumber();
            Assert.AreEqual(3, actualInstanceNumber);
        }

        [Test]
        public void CalculateContentSize_ReturnsExpectedSize()
        {
            // Act: call the method
            var actualSize = calculator.CalculateContentSize();

            // Assert: check that the result is as expected
            Assert.AreEqual(645.55f, actualSize.x, 0.01f);
        }

        [Test]
        public void CalculateBarPosition_ReturnsExpectedPosition()
        {
            // Arrange
            var index = 0;
            var actualPosition = calculator.CalculateBarPosition(index);
            Assert.AreEqual(0.01123f, actualPosition, 0.01f);

            index = 2;
            actualPosition = calculator.CalculateBarPosition(index);

            Assert.AreEqual(0.28056f, actualPosition, 0.01f);
        }

        [Test]
        public void CalculateRolling_ReturnsExpectedRolling()
        {
            // Arrange
            var currentPadding = new Vector4(0,0,120,0);
            var newPadding = new Vector4(0, 0, 290, 0);
            var startIndex = 2;

            // Act
            var actualRolling = calculator.CalculateRolling(currentPadding, newPadding, startIndex);
            Assert.AreEqual(2, actualRolling);
            
            currentPadding = new Vector4(0, 0, 290, 0);
            actualRolling = calculator.CalculateRolling(currentPadding, newPadding, startIndex);
            Assert.AreEqual(0, actualRolling);
            
            currentPadding = new Vector4(0, 0, 350, 0);
            newPadding  = new Vector4(0, 0, 230, 0);
            startIndex = 5;
            actualRolling = calculator.CalculateRolling(currentPadding, newPadding, startIndex);
            Assert.AreEqual(-2, actualRolling);
        }
        
        [Test]
        public void CalculateRolling_WithDecimalPadding_ShouldReturnExpectedValue()
        {
            viewEntity.Data.Clear();

            viewEntity.CreateItem(new Vector2(57.75f, 57.75f), new Vector2(0, 0));
            viewEntity.CreateItem(new Vector2(67.45f, 67.45f), new Vector2(-62.75f, -62.75f));
            viewEntity.CreateItem(new Vector2(77.15f, 77.15f), new Vector2(-140.2f, -140.2f));
            viewEntity.CreateItem(new Vector2(86.85f, 86.85f), new Vector2(-227.35f, -227.35f));
            viewEntity.CreateItem(new Vector2(96.55f, 96.55f), new Vector2(-324.2f, -324.2f));
            viewEntity.CreateItem(new Vector2(106.25f, 106.25f), new Vector2(-430.75f, -430.75f));
            viewEntity.CreateItem(new Vector2(115.95f, 115.95f), new Vector2(-547, -547));
            
            // Arrange
            var currentPadding = new Vector4(0, 0, 5f, 0);
            var newPadding = new Vector4(0, 0, 67.75f, 0);
            int startIndex = 0;

            // Act
            var result = calculator.CalculateRolling(currentPadding, newPadding, startIndex);
    
            // Assert
            Assert.AreEqual(1, result);
            
            currentPadding = new Vector4(0, 0, 67.75f, 0);
            newPadding = new Vector4(0, 0, 145.2f, 0);
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
            var actualOffset = calculator.CalculateOffset(count, contentPos);
            Assert.AreEqual(new Vector4(5, 0, 5, 0), actualOffset);

            index = 1;
            contentPos = new Vector3(-119.05f, 0, 0);
            actualOffset = calculator.CalculateOffset(count, contentPos);
            Assert.AreEqual(new Vector4(5, 0, 65, 0), actualOffset);

            index = 5;
            contentPos = new Vector3(-445, 0, 0);
            actualOffset = calculator.CalculateOffset(count, contentPos);
            Assert.AreEqual(new Vector4(5, 0, 355, 0), actualOffset);
        }

        [Test]
        public void CalculateOffset_ThrowsExceptionForCountZero()
        {
            // Arrange
            var index =  0;
            var count = 0;
            var contentPos = new Vector3(0, 0, 0);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => calculator.CalculateOffset(count, contentPos));
        }
    }
}