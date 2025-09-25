using Xunit;

using Magnett.Automation.Core.Contexts;

namespace Magnett.Automation.Core.UnitTest.Contexts;

public class ContextFieldTest
{
    private const string FieldName = "FieldName";
        
    [Fact]
    public void Create_When_Invoke_Return_Valid_Instance()
    {
        var instance = ContextField<string>.WithName(FieldName);
            
        Assert.NotNull(instance);
        Assert.IsType<ContextField<string>>(instance);
    }
    
    [Fact]
    public void ImplicitOperator_When_Invoke_Return_Valid_Instance()
    {
        ContextField<string> instance = FieldName;
        
        Assert.NotNull(instance);
        Assert.IsType<ContextField<string>>(instance);
    }
}