using System;
using Magnett.Automation.Core.WorkFlows;
using Xunit;

namespace Magnett.Automation.Core.UnitTest.WorkFlows
{
    public class NodeExitTest
    {
        private const string GoodCode = "Ok";
        private const string Data = "Data Info";
            
        [Fact]
        public void Create_WhenCodeIsNull_ThrowException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = NodeExit.Create(null));
        }

        [Fact]
        public void Create_WhenCodeNotIsNull_ReturnValidInstance()
        {
            var instance = NodeExit.Create(GoodCode);
            
            Assert.NotNull(instance);
            Assert.Equal(GoodCode, instance.Code);
        }
        
        [Fact]
        public void Create_WhenIsErrorIsInformed_ValueIsCorrectlyStored()
        {
            var instance = NodeExit.Create(GoodCode, true);
            
            Assert.NotNull(instance);
            Assert.True(instance.IsError);
        }
        
        [Fact]
        public void Create_WhenDataIsInformed_ValueIsCorrectlyStored()
        {
            var instance = NodeExit.Create(GoodCode, false,   Data);
            
            Assert.NotNull(instance);
            Assert.Equal(Data, instance.Data);
        }
    }
}