namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SagaPattern;

public record SagaRequest(double Amount, string Description, double CreditLimit, bool CanMakePayment);