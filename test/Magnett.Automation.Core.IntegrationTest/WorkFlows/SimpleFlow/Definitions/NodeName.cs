using Magnett.Automation.Core.Commons;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SimpleFlow.Definitions;

internal record NodeName : CommonNamedKey 
{
    public static readonly NodeName Reset     = new("Reset");
    public static readonly NodeName SetValue  = new("SetValue");
    public static readonly NodeName SumValue  = new("SumValue");
        
    private NodeName(string name) : base(name)
    {
            
    }
}