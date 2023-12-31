using NUnit.Framework;
using ScrollViewExtension.Scripts.Core.Entity;
using ScrollViewExtension.Scripts.Core.Entity.Interface;
using ScrollViewExtension.Scripts.Core.Service;
using UnityEngine;

namespace ScrollViewExtension.Scripts.Tests
{
    [TestFixture]
    public class FindIndexTestHorizontal
    {
        private FindIndex<TestScrollItem> serviceUnderTest;
        private IScrollViewEntity<TestScrollItem> fakeScrollViewEntity;

        [SetUp]
        public void SetUp()
        {
            serviceUnderTest = FindIndex<TestScrollItem>.CreateInstance();

            using (fakeScrollViewEntity = ScrollViewEntity<TestScrollItem>.CreateInstance(
                       new RectOffset(5, 0, 5, 0),
                       10,
                       new Vector2(200, 200),
                       new Vector2(180, 0),
                       false
                   ))
            {
            }
            
            fakeScrollViewEntity.CreateItem(new Vector2(50, 50), new Vector2(0, 0));
            fakeScrollViewEntity.CreateItem(new Vector2(50, 50), new Vector2(55, -55));
            fakeScrollViewEntity.CreateItem(new Vector2(100, 100), new Vector2(115, -115));
        }

        [Test]
        public void ByPosition_ReturnsCorrectIndex_GivenPosition()
        {
            // Arrange
            Vector3 position1 = new Vector3(0, 0);
            Vector3 position2 = new Vector3(-35, 35);
            Vector3 position3 = new Vector3(-54.99f, 54.99f);
            Vector3 position4 = new Vector3(-115, 115);
            Vector3 position5 = new Vector3(-165, 165);
            
            // Act
            int result = serviceUnderTest.ByPosition(position1, fakeScrollViewEntity);
            Assert.AreEqual(0,result);
            
            result = serviceUnderTest.ByPosition(position2, fakeScrollViewEntity);
            Assert.AreEqual(0,result);
            
            result = serviceUnderTest.ByPosition(position3, fakeScrollViewEntity);
            Assert.AreEqual(0,result);
            
            result = serviceUnderTest.ByPosition(position4, fakeScrollViewEntity);
            Assert.AreEqual(2,result);
            
            result = serviceUnderTest.ByPosition(position5, fakeScrollViewEntity);
            Assert.AreEqual(2,result);
        }
    }
}