using System;
using System.Collections.Generic;
using Magnett.Automation.Core.Test.StateMachine.Helpers;
using Moq;
using Xunit;

namespace Magnett.Automation.Core.Test.StateMachine.Collections
{
    public class DictionaryWrapperTest
    {
        private const string Value = "OK";
        private const string Key = "Key";

        [Fact]
        public void Create_When_Values_IsNull_Throw_Exception()
        {
            Assert.Throws<ArgumentNullException>(() => 
                DictionaryWrapperMockup.Create(null));
        }

        [Fact]
        public void Add_When_Key_IsNull_Throw_Exception()
        {
            var dictionaryWrapper = DictionaryWrapperMockup
                .Create(new Dictionary<string, string>());

            Assert.Throws<ArgumentNullException>(() =>
                dictionaryWrapper.Add(null, Value));
        }

        [Fact]
        public void Add_When_Key_IsEmpty_Throw_Exception()
        {
            var dictionaryWrapper = DictionaryWrapperMockup
                .Create(new Dictionary<string, string>());

            Assert.Throws<ArgumentNullException>(() =>
                dictionaryWrapper.Add(string.Empty, Value));
        }
        
        [Fact]
        public void Add_When_Item_IsNull_Throw_Exception()
        {
            var dictionaryWrapper = DictionaryWrapperMockup
                .Create(new Dictionary<string, string>());

            Assert.Throws<ArgumentNullException>(() =>
                dictionaryWrapper.Add(Key, null));
        }
        
        [Fact]
        public void Add_When_Invoke_Call_Dictionary_Add()
        {
            var dictionary = new Mock<IDictionary<string, string>>();
            var dictionaryWrapper = DictionaryWrapperMockup
                .Create(dictionary.Object);
            
            dictionaryWrapper.Add(Key, Value);

            dictionary.Verify(
                dic => dic.Add(Key, Value), 
                Times.Once);
        }
        
        [Fact]
        public void HasItem_When_Invoke_Call_Dictionary_ContainsKey()
        {
            var dictionary = new Mock<IDictionary<string, string>>();
            var dictionaryWrapper = DictionaryWrapperMockup
                .Create(dictionary.Object);
            
            dictionaryWrapper.HasItem(Key);

            dictionary.Verify(
                dic => dic.ContainsKey(Key), 
                Times.Once);
        }
        
        [Fact]
        public void GetItem_When_Invoke_Call_Dictionary_IndexProperty()
        {
            var dictionary = new Mock<IDictionary<string, string>>();
            var dictionaryWrapper = DictionaryWrapperMockup
                .Create(dictionary.Object);
            
            dictionaryWrapper.GetItem(Key);

            dictionary.Verify(
                dic => dic[Key], 
                Times.Once);
        }
        
        [Fact]
        public void Count_When_Invoke_Call_Dictionary_Count()
        {
            var dictionary = new Mock<IDictionary<string, string>>();
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
            var dictionary = new Mock<IDictionary<string, string>>();

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
            var dictionary = new Mock<IDictionary<string, string>>();

            dictionary
                .SetupGet(dic => dic.Count)
                .Returns(default(int) + 1);
            
            var dictionaryWrapper = DictionaryWrapperMockup
                .Create(dictionary.Object);

            var result = dictionaryWrapper.IsEmpty();

            Assert.False(result);
        }
    }
}