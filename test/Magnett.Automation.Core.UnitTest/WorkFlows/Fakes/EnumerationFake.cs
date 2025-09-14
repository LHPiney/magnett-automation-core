using Magnett.Automation.Core.Commons;

namespace Magnett.Automation.Core.UnitTest.WorkFlows.Fakes;

public record EnumerationFake : Enumeration
{
    public EnumerationFake(int id, string name) : base(id, name)
    {
    }
    
    public static Enumeration Create(int id, string name)
    {
        return new EnumerationFake(id, name);
    }
}