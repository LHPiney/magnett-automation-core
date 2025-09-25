using Magnett.Automation.Core.Commons;

namespace Magnett.Automation.Core.UnitTest.Commons.Helpers;

public record EnumerationMockup : Enumeration
{
    public int GetId()
    {
        return Id;
    }
    public EnumerationMockup(int id, string name) :
        base(id, name)
    {
    }
}