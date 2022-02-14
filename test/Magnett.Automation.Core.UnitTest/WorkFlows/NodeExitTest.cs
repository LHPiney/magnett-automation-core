using System;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.UnitTest.WorkFlows.Fakes;
using Magnett.Automation.Core.WorkFlows;
using Xunit;

namespace Magnett.Automation.Core.UnitTest.WorkFlows
{
    public class NodeExitTest
    {
        private const string GoodCode = "Ok";
        private const string Data = "Data Info";

        private static readonly Enumeration GoodEnumerationCode =  EnumerationFake.Create(1, GoodCode);

        #region Create with Enumeration
        [Fact]
        public void CreateAsEnumeration_WhenCodeIsNull_ThrowException()
        {
            Enumeration parameter = null;
            
            Assert.Throws<ArgumentNullException>(() =>
                _ = NodeExit.Create(parameter));
        }
        
        [Fact]
        public void CreateAsEnumeration_WhenCodeNotIsNull_ReturnValidInstance()
        {
            var instance = NodeExit.Create(GoodEnumerationCode);
            
            Assert.NotNull(instance);
            Assert.Equal(GoodCode, instance.Code);
        }
        
        [Fact]
        public void CreateAsEnumeration_WhenIsErrorIsInformed_ValueIsCorrectlyStored()
        {
            var instance = NodeExit.Create(GoodEnumerationCode, true);
            
            Assert.NotNull(instance);
            Assert.True(instance.IsError);
        }
        
        [Fact]
        public void CreateAsEnumeration_WhenDataIsInformed_ValueIsCorrectlyStored()
        {
            var instance = NodeExit.Create(GoodEnumerationCode, false, Data);
            
            Assert.NotNull(instance);
            Assert.Equal(Data, instance.Data);
        }
        #endregion

        #region Create with String
        [Fact]
        public void CreateAsString_WhenCodeIsNull_ThrowException()
        {
            string parameter = null;
            
            Assert.Throws<ArgumentNullException>(() =>
                _ = NodeExit.Create(parameter));
        }
        
        [Fact]
        public void CreateAsString_WhenCodeNotIsNull_ReturnValidInstance()
        {
            var instance = NodeExit.Create(GoodCode);
            
            Assert.NotNull(instance);
            Assert.Equal(GoodCode, instance.Code);
        }
        
        [Fact]
        public void CreateAsString_WhenIsErrorIsInformed_ValueIsCorrectlyStored()
        {
            var instance = NodeExit.Create(GoodCode, true);
            
            Assert.NotNull(instance);
            Assert.True(instance.IsError);
        }
        
        [Fact]
        public void CreateAsString_WhenDataIsInformed_ValueIsCorrectlyStored()
        {
            var instance = NodeExit.Create(GoodCode, false, Data);
            
            Assert.NotNull(instance);
            Assert.Equal(Data, instance.Data);
        }
        #endregion
    }
}