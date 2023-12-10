using System;
using Xunit;

using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.UnitTest.Commons.Helpers;

namespace Magnett.Automation.Core.UnitTest.Commons;

public class CommonNamedKeyTest 
{
    private const string KeyName = "Key";
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
        Assert.Throws<ArgumentNullException>(() =>
            CommonNamedKey.Create(null));

    }

    [Fact]
    public void Create_When_Invoke_With_Empty_KeyName_Throws_Exception()
    {
        Assert.Throws<ArgumentNullException>(() =>
            CommonNamedKey.Create(""));

    }

    #endregion

    #region Equals
    [Fact]
    public void Equals_WhenTwoDifferentInstancesWithSameKeyAreCompared_ShouldReturnTrue()
    {
        var instanceOne = CommonNamedKey.Create(KeyName);
        var instanceTwo = CommonNamedKey.Create(KeyName);

        var areEquals = instanceOne.Equals(instanceTwo);

        Assert.True(areEquals);
    }

    [Fact]
    public void Equals_WhenTwoDifferentInstancesWithDifferentKeyAreCompared_ShouldReturnFalse()
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
        var instance = CommonNamedKey.Create(KeyName);
        var enumeration = new EnumerationMockup(0, KeyName);

        var areEquals = instance.Equals(enumeration);

        Assert.True(areEquals);
    }

    [Fact]
    public void Equals_WhenComparedToEnumerationWithNotEqualsKey__ShouldReturnFalse()
    {
        var instance = CommonNamedKey.Create(KeyName);
        var enumeration = new EnumerationMockup(0, "Dummy");

        var areEquals = instance.Equals(enumeration);

        Assert.False(areEquals);
    }
    #endregion

    #region IEqualityComparer
        
    [Fact]
    public void Equals_WhenSameInstanceIsCompared_ShouldReturnTrue()
    {
        var instanceOne = CommonNamedKey.Create(KeyName);
            
        var areEquals = instanceOne.Equals(instanceOne, instanceOne);

        Assert.True(areEquals);
    }
        
    [Fact]
    public void Equals_WhenFirstInstanceIsNull_ShouldReturnFalse()
    {
        var instanceOne = CommonNamedKey.Create(KeyName);
            
        var areEquals = instanceOne.Equals(null, instanceOne);

        Assert.False(areEquals);
    }

    [Fact]
    public void Equals_WhenSecondInstanceIsNull_ShouldReturnFalse()
    {
        var instanceOne = CommonNamedKey.Create(KeyName);
            
        var areEquals = instanceOne.Equals(instanceOne, null);

        Assert.False(areEquals);
    }        
            
    [Fact]
    public void Equals_WhenInstancesHasSameName_ShouldReturnTrue()
    {
        var instanceOne = CommonNamedKey.Create(KeyName);
        var instanceTwo = CommonNamedKey.Create(KeyName);
            
        var areEquals = instanceOne.Equals(instanceOne, instanceTwo);

        Assert.True(areEquals);
    }  
        
    [Fact]
    public void Equals_WhenInstancesHasDifferentName_ShouldReturnTrue()
    {
        var instanceOne = CommonNamedKey.Create(KeyName);
        var instanceTwo = CommonNamedKey.Create(AlternativeKeyName);
            
        var areEquals = instanceOne.Equals(instanceOne, instanceTwo);

        Assert.False(areEquals);
    }          
    
    [Fact]
    public void GetHashCode_WhenInvoked_ShouldReturnNameHashCode()
    {
        var instance = CommonNamedKey.Create(KeyName);
            
        var nameHasCode = instance.Name.GetHashCode();

        Assert.Equal(nameHasCode, instance.GetHashCode());
    }
    
    [Fact]
    public void GetHashCode_WhenInvokedWithAName_ShouldReturnNameHashCode()
    {
        var instance = CommonNamedKey.Create(KeyName);
        var key = CommonNamedKey.Create(AlternativeKeyName);
            
        var keyHasCode = instance.GetHashCode(key);

        Assert.Equal(keyHasCode, key.GetHashCode());
    }

    #endregion
}