using System;
using System.Collections.Generic;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.UnitTest.Commons.Helpers;
using Moq;
using Xunit;

namespace Magnett.Automation.Core.UnitTest.Commons;

public class DictionaryWrapperTest
{
    private const string Value = "OK";
    private const string Key   = "Key";

    [Fact]
    public void Create_When_Invoke_Return_Instance()
    {
        var instance = DictionaryWrapperMockup.Create();
            
        Assert.NotNull(instance);
        Assert.IsType<DictionaryWrapperMockup>(instance);
    }
        
    [Fact]
    public void Create_When_Invoke_With_Valid_Dictionary_Return_Instance()
    {
        var dictionary = new Mock<IDictionary<CommonNamedKey, string>>();
        var instance = DictionaryWrapperMockup.Create(dictionary.Object);
            
        Assert.NotNull(instance);
        Assert.IsType<DictionaryWrapperMockup>(instance);
    }
        
    [Fact]
    public void Create_When_Dictionary_IsNull_Throw_Exception()
    {
        Assert.Throws<ArgumentNullException>(() => 
            DictionaryWrapperMockup.Create(null));
    }
        
    [Fact]
    public void Add_When_Invoke_Call_Dictionary_Add()
    {
        var dictionary = new Mock<IDictionary<CommonNamedKey, string>>();
        var dictionaryWrapper = DictionaryWrapperMockup
            .Create(dictionary.Object);
        var key = CommonNamedKey.Create(Key);
            
        dictionaryWrapper.Add(key, Value);

        dictionary.Verify(
            dic => dic.Add(key, Value), 
            Times.Once);
    }
        
    [Fact]
    public void HasItem_When_Invoke_Call_Dictionary_ContainsKey()
    {
        var dictionary = new Mock<IDictionary<CommonNamedKey, string>>();
        var dictionaryWrapper = DictionaryWrapperMockup
            .Create(dictionary.Object);
        var key = CommonNamedKey.Create(Key);
            
        dictionaryWrapper.HasItem(key);

        dictionary.Verify(
            dic => dic.ContainsKey(key), 
            Times.Once);
    }
        
    [Fact]
    public void Get_When_Invoke_Call_Dictionary_IndexProperty()
    {
        var dictionary = new Mock<IDictionary<CommonNamedKey, string>>();
        var dictionaryWrapper = DictionaryWrapperMockup
            .Create(dictionary.Object);

        var key = CommonNamedKey.Create(Key);
        dictionaryWrapper.Get(key);

        dictionary.Verify(
            dic => dic[key], 
            Times.Once);
    }
        
    [Fact]
    public void Set_When_Invoke_Call_Dictionary_IndexProperty()
    {
        var dictionary = new Mock<IDictionary<CommonNamedKey, string>>();
        var dictionaryWrapper = DictionaryWrapperMockup
            .Create(dictionary.Object);
        var key = CommonNamedKey.Create(Key);
            
        dictionary
            .SetupSet(dic => dic[key]=Value)
            .Verifiable();
            
        dictionaryWrapper.Set(key, Value);

        dictionary.Verify();
    }

    [Fact]
    public void Count_When_Invoke_Call_Dictionary_Count()
    {
        var dictionary = new Mock<IDictionary<CommonNamedKey, string>>();
        var dictionaryWrapper = DictionaryWrapperMockup
            .Create(dictionary.Object);
            
        _ = dictionaryWrapper.Count;

        dictionary.Verify(
            dic => dic.Count, 
            Times.Once);
    }
        
    [Fact]
    public void IsEmpty_When_Count_Is_Zero_Return_True()
    {
        var dictionary = new Mock<IDictionary<CommonNamedKey, string>>();

        dictionary
            .SetupGet(dic => dic.Count)
            .Returns(default(int));
            
        var dictionaryWrapper = DictionaryWrapperMockup
            .Create(dictionary.Object);

        var result = dictionaryWrapper.IsEmpty();

        Assert.True(result);
    }
        
    [Fact]
    public void IsEmpty_When_Count_Greater_Than_Zero_Return_False()
    {
        var dictionary = new Mock<IDictionary<CommonNamedKey, string>>();

        dictionary
            .SetupGet(dic => dic.Count)
            .Returns(default(int) + 1);
            
        var dictionaryWrapper = DictionaryWrapperMockup
            .Create(dictionary.Object);

        var result = dictionaryWrapper.IsEmpty();

        Assert.False(result);
    }
}