using Magnett.Automation.Core.Contexts;
using Xunit;

namespace Magnett.Automation.Core.Test.Contexts
{
    public class ContextVaultTest
    {
        [Fact]
        public void Create_When_Invoke_Return_Valid_Instance()
        {
            var instance = ContextVault.Create();
            
            Assert.NotNull(instance);
            Assert.IsType<ContextVault>(instance);
        }
    }
}