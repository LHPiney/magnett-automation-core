using Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern.Definitions.Entities;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern;

public record SagaResponse(Order Order, Payment Payment, bool IsFailed = false);