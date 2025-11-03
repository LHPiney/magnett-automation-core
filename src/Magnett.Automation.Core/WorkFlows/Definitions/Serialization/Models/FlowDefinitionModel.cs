using System.Collections.Generic;

namespace Magnett.Automation.Core.WorkFlows.Definitions.Serialization.Models;

public class FlowDefinitionModel
{
    public string Key { get; set; } = string.Empty;
    public int Version { get; set; } = 1;
    public string InitialNode { get; set; } = string.Empty;
    public List<NodeDefinitionModel> Nodes { get; set; } = new();
    public List<NodeLinkModel> Links { get; set; } = new();
}

