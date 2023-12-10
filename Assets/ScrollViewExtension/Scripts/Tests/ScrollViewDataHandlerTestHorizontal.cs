using NUnit.Framework;
using ScrollViewExtension.Scripts.Core.Entity;
using ScrollViewExtension.Scripts.Core.Entity.Interface;
using ScrollViewExtension.Scripts.Core.Service;
using ScrollViewExtension.Scripts.Core.Service.Interface;
using ScrollViewExtension.Scripts.Core.UseCase;
using UnityEngine;

namespace ScrollViewExtension.Scripts.Tests
{
    [TestFixture]
    public class ScrollViewDataHandlerTestHorizontal
    {
        private ScrollViewDataHandler<TestScrollItem> handler;
        private IScrollViewEntity<TestScrollItem> viewEntity;
        private IFindIndex<TestScrollItem> findIndex;
        
        [SetUp]
        public void SetUp()
        {
            
            viewEntity = ScrollViewEntity<TestScrollItem>.CreateInstance(
                new RectOffset(5, 0, 5, 0),
                10,
                new Vector2(200, 200),
                new Vector2(180, 0),
                true);
            findIndex = FindIndex<TestScrollItem>.CreateInstance();
            handler = ScrollViewDataHandler<TestScrollItem>.CreateInstance(viewEntity, findIndex);
            
            handler.CreateItem(new Vector2(200, 50));
            handler.CreateItem(new Vector2(200, 50));
            handler.CreateItem(new Vector2(200, 100));
            handler.CreateItem(new Vector2(200, 50));
            handler.CreateItem(new Vector2(200, 50));
            handler.CreateItem(new Vector2(200, 50));
            handler.CreateItem(new Vector2(200, 50));
            handler.CreateItem(new Vector2(200, 50));
            handler.CreateItem(new Vector2(200, 50));
            handler.CreateItem(new Vector2(200, 50));
        }

        [TearDown]
        public void TearDown()
        {
            viewEntity.Dispose();
        }
        
        [Test]
        public void CreateInstance_Called_ReturnsInstance()
        {
            Assert.NotNull(handler);
        }

        [Test]
        public void CreateItem_Called_ExpectCreateItemCalled()
        {
            // Assert
            Assert.AreEqual(10, viewEntity.Data.Count);
        }

        [Test]
        public void GetRange_ByIndexAndCount_CallsEntityGetRange()
        {
            // Arrange
            int index = 1;
            int count = 3;

            // Act
            var list = handler.GetRange(index, count);

            // Assert
            Assert.AreEqual(1, list[0].Index);
            Assert.AreEqual(3, list.Count);

            index = 9;
            count = 1;
            
            list = handler.GetRange(index, count);
            Assert.AreEqual(9, list[0].Index);
        }
        
        [Test]
        public void Test_UpdatePositionsFromIndex_WithIndexTwo_ShouldUpdateLastTwoElements()
        {
            var item = handler.GetRange(7, 1);
            item[0].Size = new Vector2(200, 100);
            
            // Act
            handler.UpdatePositionsFromIndex(7);
            item = handler.GetRange(6, 4);

            // Assert
            Assert.That(item[0].Position, Is.EqualTo(new Vector2(1255, -405))); 
            Assert.That(item[1].Position, Is.EqualTo(new Vector2(1465, -465))); //Positions should be updated
            Assert.That(item[2].Position, Is.EqualTo(new Vector2(1675, -575))); //Positions should be updated
            Assert.That(item[3].Position, Is.EqualTo(new Vector2(1885, -635))); //Positions should be updated
        }
    }
}