using System;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.UnitTest.Commons.Helpers;
using Xunit;

namespace Magnett.Automation.Core.UnitTest.Commons
{
    public class CommonNamedKeyTest
    {
        private const string KeyName            = "Key";
        private const string AlternativeKeyName = "AlternativeKey";
        
        #region Create
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
        #endregion

        [Fact]
        public void Equals_WhenTwoDifferentsInstancesWithSameKeyAreCompared_ShouldReturnTrue()
        {
            var instanceOne = CommonNamedKey.Create(KeyName);
            var instanceTwo = CommonNamedKey.Create(KeyName);

            var areEquals = instanceOne.Equals(instanceTwo);
            
            Assert.True(areEquals);
        }
        
        [Fact]
        public void Equals_WhenTwoDifferentsInstancesWithDifferentKeyAreCompared_ShouldReturnFalse()
        {
            var instanceOne = CommonNamedKey.Create(KeyName);
            var instanceTwo = CommonNamedKey.Create(AlternativeKeyName);

            var areEquals = instanceOne.Equals(instanceTwo);
            
            Assert.False(areEquals);
        }
        
        [Fact]
        public void Equals_WhenComparedToEqualsString_ShouldReturnTrue()
        {
            var instance = CommonNamedKey.Create(KeyName);
            
            var areEquals = instance.Equals(KeyName);
            
            Assert.True(areEquals);
        }
        
        [Fact]
        public void Equals_WhenComparedToNotEqualsString_ShouldReturnFalse()
        {
            var instance = CommonNamedKey.Create(KeyName);
            
            var areEquals = instance.Equals("Dummy");
            
            Assert.False(areEquals);
        }
        
        [Fact]
        public void Equals_WhenComparedToEnumerationWithEqualsKey_ShouldReturnTrue()
        {
            var instance    = CommonNamedKey.Create(KeyName);
            var enumeration = new EnumerationMockup(0, KeyName);
            
            var areEquals = instance.Equals(enumeration);
            
            Assert.True(areEquals);
        }
        
        [Fact]
        public void Equals_WhenComparedToEnumerationWithNotEqualsKey__ShouldReturnFalse()
        {
            var instance    = CommonNamedKey.Create(KeyName);
            var enumeration = new EnumerationMockup(0, "Dummy");
            
            var areEquals = instance.Equals(enumeration);
            
            Assert.False(areEquals);
        }
        
        
    }
}