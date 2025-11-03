using System.IO;
using System.Threading.Tasks;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.WorkFlows.Definitions.Implementations;
using Xunit;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.Definitions;

public class FlowDefinitionRegistryIntegrationTest
{
    private static string GetTestDefinitionPath()
    {
        var assemblyLocation = typeof(FlowDefinitionRegistryIntegrationTest).Assembly.Location;
        var assemblyDirectory = Path.GetDirectoryName(assemblyLocation) ?? Directory.GetCurrentDirectory();
        
        var testDefinitionPath = Path.Combine(assemblyDirectory, "WorkFlows", "Definitions", "saga-pattern.json");
        
        if (!File.Exists(testDefinitionPath))
        {
            var projectRoot = Path.GetFullPath(Path.Combine(assemblyDirectory, "..", "..", "..", "..", ".."));
            testDefinitionPath = Path.Combine(projectRoot, "test", "Magnett.Automation.Core.IntegrationTest", "WorkFlows", "Definitions", "saga-pattern.json");
        }
        
        if (!File.Exists(testDefinitionPath))
        {
            var sourcePath = Path.Combine(Directory.GetCurrentDirectory(), "WorkFlows", "Definitions", "saga-pattern.json");
            if (File.Exists(sourcePath))
            {
                return sourcePath;
            }
        }
        
        return testDefinitionPath;
    }

    [Fact]
    public async Task LoadAsync_WithValidJsonFile_ShouldLoadDefinition()
    {
        var registry = FlowDefinitionRegistry.Create();
        var filePath = GetTestDefinitionPath();

        var result = await registry.LoadAsync(filePath);

        Assert.True(result.IsSuccess, $"Error loading definition: {result.ErrorMessage}");
        Assert.NotNull(result.Value);
        Assert.Equal("saga-pattern", result.Value!.Key.Name);
        Assert.Equal(1, result.Value.Version);
    }

    [Fact]
    public async Task LoadAsync_WithValidJsonFile_ShouldHaveAllNodes()
    {
        var registry = FlowDefinitionRegistry.Create();
        var filePath = GetTestDefinitionPath();

        var result = await registry.LoadAsync(filePath);

        Assert.True(result.IsSuccess);
        var definition = result.Value!;

        Assert.True(definition.HasNode(CommonNamedKey.Create("CreateOrder")));
        Assert.True(definition.HasNode(CommonNamedKey.Create("PreAuthorizePayment")));
        Assert.True(definition.HasNode(CommonNamedKey.Create("ConfirmPayment")));
        Assert.True(definition.HasNode(CommonNamedKey.Create("ConfirmOrder")));
        Assert.True(definition.HasNode(CommonNamedKey.Create("CancelPayment")));
        Assert.True(definition.HasNode(CommonNamedKey.Create("CancelOrder")));
    }

    [Fact]
    public async Task LoadAsync_WithValidJsonFile_ShouldHaveCorrectInitialNode()
    {
        var registry = FlowDefinitionRegistry.Create();
        var filePath = GetTestDefinitionPath();

        var result = await registry.LoadAsync(filePath);

        Assert.True(result.IsSuccess);
        var definition = result.Value!;

        Assert.Equal("CreateOrder", definition.InitialNode.Name);
    }

    [Fact]
    public async Task LoadAsync_WithValidJsonFile_ShouldHaveAllLinks()
    {
        var registry = FlowDefinitionRegistry.Create();
        var filePath = GetTestDefinitionPath();

        var result = await registry.LoadAsync(filePath);

        Assert.True(result.IsSuccess);
        var definition = result.Value!;

        var createOrderKey = CommonNamedKey.Create("CreateOrder");
        Assert.True(definition.HasLink(createOrderKey, "Created"));
        Assert.True(definition.HasLink(createOrderKey, "Cancelled"));

        var preAuthKey = CommonNamedKey.Create("PreAuthorizePayment");
        Assert.True(definition.HasLink(preAuthKey, "PreAuthorized"));
        Assert.True(definition.HasLink(preAuthKey, "Denied"));
        Assert.True(definition.HasLink(preAuthKey, "Cancelled"));

        var confirmPaymentKey = CommonNamedKey.Create("ConfirmPayment");
        Assert.True(definition.HasLink(confirmPaymentKey, "Done"));
        Assert.True(definition.HasLink(confirmPaymentKey, "Failed"));
        Assert.True(definition.HasLink(confirmPaymentKey, "Cancelled"));

        var cancelPaymentKey = CommonNamedKey.Create("CancelPayment");
        Assert.True(definition.HasLink(cancelPaymentKey, "Done"));
    }

    [Fact]
    public async Task LoadAsync_WithValidJsonFile_ShouldHaveCorrectStructure()
    {
        var registry = FlowDefinitionRegistry.Create();
        var filePath = GetTestDefinitionPath();

        var loadResult = await registry.LoadAsync(filePath);
        Assert.True(loadResult.IsSuccess);

        var loadedDefinition = loadResult.Value!;

        Assert.Equal("saga-pattern", loadedDefinition.Key.Name);
        Assert.Equal(1, loadedDefinition.Version);
        Assert.Equal("CreateOrder", loadedDefinition.InitialNode.Name);

        var expectedNodes = new[] { "CreateOrder", "PreAuthorizePayment", "ConfirmPayment", "ConfirmOrder", "CancelPayment", "CancelOrder" };
        
        foreach (var nodeKey in expectedNodes)
        {
            var key = CommonNamedKey.Create(nodeKey);
            Assert.True(loadedDefinition.HasNode(key), $"Loaded definition should have node '{nodeKey}'");
        }

        var createOrderKey = CommonNamedKey.Create("CreateOrder");
        var preAuthKey = CommonNamedKey.Create("PreAuthorizePayment");
        var cancelOrderKey = CommonNamedKey.Create("CancelOrder");
        var cancelPaymentKey = CommonNamedKey.Create("CancelPayment");
        var confirmPaymentKey = CommonNamedKey.Create("ConfirmPayment");

        Assert.True(loadedDefinition.HasLink(createOrderKey, "Created"));
        Assert.True(loadedDefinition.HasLink(createOrderKey, "Cancelled"));
        Assert.True(loadedDefinition.HasLink(preAuthKey, "PreAuthorized"));
        Assert.True(loadedDefinition.HasLink(preAuthKey, "Denied"));
        Assert.True(loadedDefinition.HasLink(preAuthKey, "Cancelled"));
        Assert.True(loadedDefinition.HasLink(confirmPaymentKey, "Done"));
        Assert.True(loadedDefinition.HasLink(confirmPaymentKey, "Failed"));
        Assert.True(loadedDefinition.HasLink(confirmPaymentKey, "Cancelled"));
        Assert.True(loadedDefinition.HasLink(cancelPaymentKey, "Done"));
    }

    [Fact]
    public async Task GetAsync_AfterLoading_ShouldRetrieveFromCache()
    {
        var registry = FlowDefinitionRegistry.Create();
        var filePath = GetTestDefinitionPath();

        var loadResult = await registry.LoadAsync(filePath);
        Assert.True(loadResult.IsSuccess);

        var getResult = await registry.GetAsync("saga-pattern");
        Assert.True(getResult.IsSuccess);
        Assert.NotNull(getResult.Value);
        Assert.Equal("saga-pattern", getResult.Value!.Key.Name);
    }

    [Fact]
    public async Task GetAsync_WithVersion_ShouldRetrieveSpecificVersion()
    {
        var registry = FlowDefinitionRegistry.Create();
        var filePath = GetTestDefinitionPath();

        var loadResult = await registry.LoadAsync(filePath);
        Assert.True(loadResult.IsSuccess);

        var getResult = await registry.GetAsync("saga-pattern", 1);
        Assert.True(getResult.IsSuccess);
        Assert.NotNull(getResult.Value);
        Assert.Equal(1, getResult.Value!.Version);
    }

    [Fact]
    public async Task LoadAsync_WithInvalidFilePath_ShouldReturnFailure()
    {
        var registry = FlowDefinitionRegistry.Create();

        var result = await registry.LoadAsync("non-existent-file.json");

        Assert.True(result.IsFailure);
        Assert.Null(result.Value);
        Assert.NotNull(result.ErrorMessage);
    }

    [Fact]
    public async Task LoadAsync_WithInvalidJson_ShouldReturnFailure()
    {
        var registry = FlowDefinitionRegistry.Create();
        var tempFile = Path.GetTempFileName();
        
        try
        {
            await File.WriteAllTextAsync(tempFile, "{ invalid json }");

            var result = await registry.LoadAsync(tempFile);

            Assert.True(result.IsFailure);
            Assert.Null(result.Value);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }
}