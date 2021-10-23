using System;
using Moq;
using Xunit;

using Magnett.Automation.Core.Contexts;

namespace Magnett.Automation.Core.UnitTest.Contexts
{
    public class ContextManagerTest
    {
        private const string FieldValue = "Value";
        
        private static readonly ContextField<string> CtxField
            = ContextField<string>.Create("OkFieldName");
        
        private static readonly ContextField<string> NofCtxField
            = ContextField<string>.Create("NofFieldName");

        [Fact]
        public void Create_When_Invoke_Return_Valid_Instance()
        {
            var ctxVault = new Mock<IContextVault>();
            var instance = Context.Create(ctxVault.Object);
            
            Assert.NotNull(instance);
            Assert.IsType<Context>(instance);
        }
        
        [Fact]
        public void Create_When_Invoke_With_Null_Throw_Exception()
        {
            Assert.Throws<ArgumentNullException>(()=> 
                Context.Create(null));
        }    
        
        [Fact]
        public void Store_When_Invoke_Call_Vault_Set()
        {
            var ctxVault = new Mock<IContextVault>();
            var ctxManager = Context.Create(ctxVault.Object);

            ctxManager.Store(CtxField, FieldValue);
            
            ctxVault.Verify(
                dic =>  dic.Set(CtxField, FieldValue), 
                Times.Once);
        }
        
        [Fact]
        public void Value_When_Field_Is_Found_Call_Vault_Get()
        {
            var ctxVault = new Mock<IContextVault>();
            var ctxManager = Context.Create(ctxVault.Object);

            ctxVault
                .Setup(vault => vault.HasItem(CtxField))
                .Returns(true);
            
            ctxVault
                .Setup(vault => vault.Get(CtxField))
                .Returns(FieldValue);

            var result = ctxManager.Value(CtxField);
            
            ctxVault.Verify(
                dic =>  dic.Get(CtxField), 
                Times.Once);
            
            Assert.Equal(FieldValue, result);
        }

        [Fact]
        public void Value_When_Field_Is_Not_Found_Return_Default()
        {
            var ctxVault = new Mock<IContextVault>();
            var ctxManager = Context.Create(ctxVault.Object);

            ctxVault
                .Setup(vault => vault.HasItem(NofCtxField))
                .Returns(false);

            var result = ctxManager.Value(NofCtxField);
            
            Assert.Equal(default(string), result);
        }
    }
}