using System;
using System.Collections.Concurrent;
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
        var dictionary = new Mock<ConcurrentDictionary<CommonNamedKey, string>>();
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
    public void Add_When_Dictionary_IsConcurrentDictionary_Keeps_OriginalData()
    {
        var dictionary = new ConcurrentDictionary<CommonNamedKey, string>();
        dictionary.TryAdd(Key, Value);
        dictionary.TryAdd(CommonNamedKey.Create("AnotherKey"), "AnotherValue");

        var dictionaryWrapper = DictionaryWrapperMockup
            .Create(dictionary);

        Assert.Equal(dictionary.Count, dictionaryWrapper.Count);
    }

    [Fact]
    public void Add_When_Dictionary_IsDictionary_Keeps_OriginalData()
    {
        var dictionary = new Dictionary<CommonNamedKey, string>
        {
            { Key, Value },
            { CommonNamedKey.Create("AnotherKey"), "AnotherValue" }
        };

        var dictionaryWrapper = DictionaryWrapperMockup
            .Create(dictionary);
            
        Assert.Equal(dictionary.Count, dictionaryWrapper.Count);
    }
              
    [Fact]
    public void IsEmpty_When_Count_Greater_Than_Zero_Return_False()
    {
        var dictionary = new ConcurrentDictionary<CommonNamedKey, string>();
        dictionary.TryAdd(Key, Value);
            
        var dictionaryWrapper = DictionaryWrapperMockup
            .Create(dictionary);

        var result = dictionaryWrapper.IsEmpty();

        Assert.False(result);
    }

    [Fact]
    public void Add_When_DuplicateKey_ThrowsException()
    {
        var wrapper = DictionaryWrapperMockup.Create();
        var key = CommonNamedKey.Create(Key);
        wrapper.Add(key, Value);
        Assert.Throws<ArgumentException>(() => wrapper.Add(key, Value));
    }

    [Fact]
    public void GetKeys_When_DictionaryIsEmpty_ReturnsEmptyEnumeration()
    {
        var wrapper = DictionaryWrapperMockup.Create();
        Assert.Empty(wrapper.GetKeys());
    }

    [Fact]
    public void GetValues_When_DictionaryIsEmpty_ReturnsEmptyEnumeration()
    {
        var wrapper = DictionaryWrapperMockup.Create();
        Assert.Empty(wrapper.GetValues());
    }

    [Fact]
    public void GetKeysAndValues_WithMultipleItems_ReturnsAll()
    {
        var wrapper = DictionaryWrapperMockup.Create();
        var key1 = CommonNamedKey.Create("A");
        var key2 = CommonNamedKey.Create("B");
        wrapper.Add(key1, "1");
        wrapper.Add(key2, "2");
        Assert.Contains(key1, wrapper.GetKeys());
        Assert.Contains(key2, wrapper.GetKeys());
        Assert.Contains("1", wrapper.GetValues());
        Assert.Contains("2", wrapper.GetValues());
    }

    [Fact]
    public void Get_When_KeyDoesNotExist_ThrowsException()
    {
        var wrapper = DictionaryWrapperMockup.Create();
        var key = CommonNamedKey.Create("notfound");
        Assert.Throws<KeyNotFoundException>(() => wrapper.Get(key));
    }

    [Fact]
    public void Set_When_KeyExists_UpdatesValue_And_ReturnsNewValue()
    {
        var wrapper = DictionaryWrapperMockup.Create();
        var key = CommonNamedKey.Create(Key);
        wrapper.Add(key, "old");
        var result = wrapper.Set(key, Value);
        Assert.Equal(Value, result);
        Assert.Equal(Value, wrapper.Get(key));
    }

    [Fact]
    public void Set_When_KeyDoesNotExist_AddsValue_And_ReturnsValue()
    {
        var wrapper = DictionaryWrapperMockup.Create();
        var key = CommonNamedKey.Create("newkey");
        var result = wrapper.Set(key, Value);
        Assert.Equal(Value, result);
        Assert.Equal(Value, wrapper.Get(key));
    }

    [Fact]
    public void Remove_When_KeyExists_RemovesAndReturnsTrue()
    {
        var wrapper = DictionaryWrapperMockup.Create();
        var key = CommonNamedKey.Create(Key);
        wrapper.Add(key, Value);
        var removed = wrapper.Remove(key);
        Assert.True(removed);
        Assert.False(wrapper.HasItem(key));
    }

    [Fact]
    public void Remove_When_KeyDoesNotExist_ReturnsFalse()
    {
        var wrapper = DictionaryWrapperMockup.Create();
        var key = CommonNamedKey.Create("notfound");
        var removed = wrapper.Remove(key);
        Assert.False(removed);
    }

    [Fact]
    public void Add_When_Called_ReturnsAddedValue()
    {
        var wrapper = DictionaryWrapperMockup.Create();
        var key = CommonNamedKey.Create("addkey");
        var result = wrapper.Add(key, Value);
        Assert.Equal(Value, result);
        Assert.Equal(Value, wrapper.Get(key));
    }
}