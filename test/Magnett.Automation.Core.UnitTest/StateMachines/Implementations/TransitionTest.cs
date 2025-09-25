using Magnett.Automation.Core.StateMachines.Implementations;
using Xunit;

namespace Magnett.Automation.Core.UnitTest.StateMachines.Implementations;

public class TransitionTest
{
    private const string ActionName = "Action";
    private const string StateName  = "Current";
            
    [Fact]
    public void Create_When_Invoke_Return_Instance()
    {
        var instance = Transition.Create(ActionName, StateName);
            
        Assert.NotNull(instance);
    }

    [Fact]
    public void Create_When_Instance_Is_Created_Action_Return_Correct_Value()
    {
        var instance = Transition.Create(ActionName, StateName);
            
        Assert.Equal(ActionName, instance.ActionKey.Name);
    }
        
    [Fact]
    public void Create_When_Instance_Is_Created_ToStateName_Return_Correct_Value()
    {
        var instance = Transition.Create(ActionName, StateName);
            
        Assert.Equal(StateName, instance.ToStateKey.Name);
    }
}