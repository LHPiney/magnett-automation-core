using System;
using Magnett.Automation.Core.Commons;
using Xunit;

namespace Magnett.Automation.Core.UnitTest.Commons
{
    public class CommonNamedKeyTest
    {
        private const string KeyName = "Key";
        
        [Fact]
        public void Create_When_Invoke_Return_Instance()
        {
            var instance = CommonNamedKey.Create(KeyName);
            
            Assert.NotNull(instance);
            Assert.IsType<CommonNamedKey>(instance);
        }
        
        [Fact]
        public void Create_When_Invoke_With_Valid_Name_Is_Proper_Stored()
        {
            var instance = CommonNamedKey.Create(KeyName);
            
            Assert.Equal(KeyName, instance.Name);
        }
        
        [Fact]
        public void Create_When_Invoke_With_Null_KeyName_Throws_Exception()
        {
            Assert.Throws<ArgumentNullException>(()=>
                CommonNamedKey.Create(null));
            
        }
        
        [Fact]
        public void Create_When_Invoke_With_Empty_KeyName_Throws_Exception()
        {
            Assert.Throws<ArgumentNullException>(()=>
                CommonNamedKey.Create(""));
            
        }
    }
}