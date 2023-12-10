using System;
using Magnett.Automation.Core.Contexts;
using Xunit;

namespace Magnett.Automation.Core.UnitTest.Contexts;

public class ContextTest
{
    [Fact]
    public void Create_WithNullName_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => 
            Context.Create(null!));
    }
    
    [Fact]
    public void Create_WhenCalled_ReturnsContext()
    {
        var context = Context.Create();
        
        Assert.NotNull(context);
    }
}