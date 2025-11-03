using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.WorkFlows.Definitions.Serialization.Models;

namespace Magnett.Automation.Core.WorkFlows.Definitions.Validation;

public interface IFlowDefinitionValidator
{
    Result ValidateSchema(FlowDefinitionModel model);
    Result ValidateReferences(FlowDefinitionModel model);
    Result ValidateTypes(FlowDefinitionModel model);
    Result Validate(FlowDefinitionModel model);
}

