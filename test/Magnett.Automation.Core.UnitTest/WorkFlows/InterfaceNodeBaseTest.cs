using Magnett.Automation.Core.UnitTest.WorkFlows.Fakes;
using Xunit;

namespace Magnett.Automation.Core.UnitTest.WorkFlows;

public class InterfaceNodeBaseTest
{
    [Fact]
    public void PropertyName_WhenCall_ReturnKeyName()
    {
        const string name = "name";

        var instance = NodeBaseFake.Create(name);
        
        Assert.Equal(name, instance.Name);
    }
}
