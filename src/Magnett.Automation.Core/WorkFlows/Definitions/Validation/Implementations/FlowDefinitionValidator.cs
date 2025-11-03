using System;
using System.Linq;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.WorkFlows.Definitions.Exceptions;
using Magnett.Automation.Core.WorkFlows.Definitions.Serialization.Models;

namespace Magnett.Automation.Core.WorkFlows.Definitions.Validation.Implementations;

public class FlowDefinitionValidator : IFlowDefinitionValidator
{
    public Result ValidateSchema(FlowDefinitionModel model)
    {
        if (model == null)
        {
            return Result.Failure("Model cannot be null");
        }

        if (string.IsNullOrWhiteSpace(model.Key))
        {
            var ex = new MissingRequiredFieldException("Key");
            return Result.Failure(ex.Message, ex);
        }

        if (model.Version <= 0)
        {
            var ex = new InvalidFlowDefinitionSchemaException("Version must be greater than zero");
            return Result.Failure(ex.Message, ex);
        }

        if (string.IsNullOrWhiteSpace(model.InitialNode))
        {
            var ex = new MissingRequiredFieldException("InitialNode");
            return Result.Failure(ex.Message, ex);
        }

        if (model.Nodes == null || model.Nodes.Count == 0)
        {
            var ex = new InvalidFlowDefinitionSchemaException("At least one node is required");
            return Result.Failure(ex.Message, ex);
        }

        if (model.Links == null)
        {
            var ex = new InvalidFlowDefinitionSchemaException("Links collection cannot be null");
            return Result.Failure(ex.Message, ex);
        }

        for (int i = 0; i < model.Nodes.Count; i++)
        {
            var node = model.Nodes[i];
            if (string.IsNullOrWhiteSpace(node.Key))
            {
                var ex = new MissingRequiredFieldException($"Nodes[{i}].Key");
                return Result.Failure(ex.Message, ex);
            }

            if (string.IsNullOrWhiteSpace(node.NodeType))
            {
                var ex = new MissingRequiredFieldException($"Nodes[{i}].NodeType");
                return Result.Failure(ex.Message, ex);
            }
        }

        for (int i = 0; i < model.Links.Count; i++)
        {
            var link = model.Links[i];
            if (string.IsNullOrWhiteSpace(link.FromNodeKey))
            {
                var ex = new MissingRequiredFieldException($"Links[{i}].FromNodeKey");
                return Result.Failure(ex.Message, ex);
            }

            if (string.IsNullOrWhiteSpace(link.ToNodeKey))
            {
                var ex = new MissingRequiredFieldException($"Links[{i}].ToNodeKey");
                return Result.Failure(ex.Message, ex);
            }

            if (string.IsNullOrWhiteSpace(link.Code))
            {
                var ex = new MissingRequiredFieldException($"Links[{i}].Code");
                return Result.Failure(ex.Message, ex);
            }
        }

        return Result.Success();
    }

    public Result ValidateReferences(FlowDefinitionModel model)
    {
        if (model == null)
        {
            return Result.Failure("Model cannot be null");
        }

        var nodeKeys = model.Nodes.Select(n => n.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (!nodeKeys.Contains(model.InitialNode, StringComparer.OrdinalIgnoreCase))
        {
            var ex = new InvalidNodeReferenceException(
                model.InitialNode, 
                "InitialNode does not exist in the nodes collection");
            return Result.Failure(ex.Message, ex);
        }

        foreach (var link in model.Links)
        {
            if (!nodeKeys.Contains(link.FromNodeKey, StringComparer.OrdinalIgnoreCase))
            {
                var ex = new InvalidLinkException(
                    link.FromNodeKey, 
                    link.ToNodeKey, 
                    link.Code, 
                    $"FromNodeKey '{link.FromNodeKey}' does not exist");
                return Result.Failure(ex.Message, ex);
            }

            if (!nodeKeys.Contains(link.ToNodeKey, StringComparer.OrdinalIgnoreCase))
            {
                var ex = new InvalidLinkException(
                    link.FromNodeKey, 
                    link.ToNodeKey, 
                    link.Code, 
                    $"ToNodeKey '{link.ToNodeKey}' does not exist");
                return Result.Failure(ex.Message, ex);
            }
        }

        return Result.Success();
    }

    public Result ValidateTypes(FlowDefinitionModel model)
    {
        if (model == null)
        {
            return Result.Failure("Model cannot be null");
        }

        foreach (var node in model.Nodes)
        {
            try
            {
                var nodeType = Type.GetType(node.NodeType);
                if (nodeType == null)
                {
                    var notFoundEx = new NodeTypeNotFoundException(node.Key, node.NodeType);
                    return Result.Failure(notFoundEx.Message, notFoundEx);
                }
            }
            catch (Exception ex)
            {
                var notFoundEx = new NodeTypeNotFoundException(node.Key, node.NodeType);
                return Result.Failure(notFoundEx.Message, ex);
            }
        }

        return Result.Success();
    }

    public Result Validate(FlowDefinitionModel model)
    {
        var schemaResult = ValidateSchema(model);
        if (schemaResult.IsFailure)
        {
            return schemaResult;
        }

        var referencesResult = ValidateReferences(model);
        if (referencesResult.IsFailure)
        {
            return referencesResult;
        }

        var typesResult = ValidateTypes(model);
        if (typesResult.IsFailure)
        {
            return typesResult;
        }

        return Result.Success();
    }
}

