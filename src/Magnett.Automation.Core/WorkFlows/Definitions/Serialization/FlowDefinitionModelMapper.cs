using System;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.WorkFlows.Definitions.Collections;
using Magnett.Automation.Core.WorkFlows.Definitions.Implementations;
using Magnett.Automation.Core.WorkFlows.Definitions.Serialization.Models;

namespace Magnett.Automation.Core.WorkFlows.Definitions.Serialization;

internal static class FlowDefinitionModelMapper
{
    public static IFlowDefinition ToFlowDefinition(FlowDefinitionModel model)
    {
        var key = CommonNamedKey.Create(model.Key);
        var initialNode = CommonNamedKey.Create(model.InitialNode);

        var nodes = NodeDefinitionList.Create();
        foreach (var nodeModel in model.Nodes)
        {
            var nodeKey = CommonNamedKey.Create(nodeModel.Key);
            var nodeType = Type.GetType(nodeModel.NodeType);
            if (nodeType == null)
            {
                throw new InvalidOperationException($"Node type '{nodeModel.NodeType}' not found for node '{nodeModel.Key}'");
            }

            var nodeDefinition = NodeDefinition.Create(nodeType, nodeKey);
            nodes.Add(nodeKey, nodeDefinition);
        }

        var links = NodeLinkList.Create();
        foreach (var linkModel in model.Links)
        {
            var fromNodeKey = CommonNamedKey.Create(linkModel.FromNodeKey);
            var toNodeKey = CommonNamedKey.Create(linkModel.ToNodeKey);
            var link = NodeLink.Create(fromNodeKey, toNodeKey, linkModel.Code);
            links.Add(link.Key, link);
        }

        return FlowDefinition.Create(model.Key, model.Version, initialNode, nodes, links);
    }
}

