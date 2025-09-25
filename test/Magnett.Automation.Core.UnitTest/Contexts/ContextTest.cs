#nullable enable
using Magnett.Automation.Core.Contexts;
using Magnett.Automation.Core.Events;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Magnett.Automation.Core.UnitTest.Contexts;

public class ContextTest
{
    private const string FieldValue = "Value";
        
    private static readonly ContextField<string> CtxField
        = ContextField<string>.WithName("OkFieldName");
        
    private static readonly ContextField<string> NofCtxField
        = ContextField<string>.WithName("NofFieldName");

    [Fact]
    public void Create_When_Invoke_Return_Valid_Instance()
    {
        var ctxVault = new Mock<IContextVault>();
        var eventBus = new Mock<IEventBus>();
        var instance = Context.Create(ctxVault.Object, eventBus.Object);
        
        Assert.NotNull(instance);
        Assert.IsType<Context>(instance);
    }
    
    [Fact]
    public void Create_When_Invoke__WithOut_ContextVault_Return_Valid_Instance()
    {
        var eventBus = new Mock<IEventBus>();
        var instance = Context.Create(eventBus.Object);
        
        Assert.NotNull(instance);
        Assert.IsType<Context>(instance);
    }
    
    [Fact]
    public void Create_When_Invoke__WithOut_Parameters_Return_Valid_Instance()
    {
        var instance = Context.Create();
        
        Assert.NotNull(instance);
        Assert.IsType<Context>(instance);
    }
    
    
    [Fact]
    public async Task StoreAsync_When_Invoke_Call_Vault_Set()
    {
        var ctxVault = new Mock<IContextVault>();
        var eventBus = new Mock<IEventBus>();
        var context = Context.Create(ctxVault.Object, eventBus.Object);

        await context.StoreAsync(CtxField, FieldValue);
            
        ctxVault.Verify(
            dic =>  dic.Set(CtxField, FieldValue), 
            Times.Once);
    }
        
    [Fact]
    public void Value_When_Field_Is_Found_Call_Vault_Get()
    {
        var ctxVault = new Mock<IContextVault>();
        var eventBus = new Mock<IEventBus>();
        var context = Context.Create(ctxVault.Object, eventBus.Object);

        ctxVault
            .Setup(vault => vault.HasItem(CtxField))
            .Returns(true);
        ctxVault
            .Setup(vault => vault.Get(CtxField))
            .Returns(FieldValue);

        var result = context.Value(CtxField);
            
        ctxVault.Verify(
            dic =>  dic.Get(CtxField), 
            Times.Once);
            
        Assert.Equal(FieldValue, result);
    }

    [Fact]
    public void Value_When_Field_Is_Not_Found_Return_Default()
    {
        var ctxVault = new Mock<IContextVault>();
        var eventBus = new Mock<IEventBus>();
        var context = Context.Create(ctxVault.Object, eventBus.Object);

        ctxVault
            .Setup(vault => vault.HasItem(NofCtxField))
            .Returns(false);

        var result = context.Value(NofCtxField);
            
        Assert.Null(result);
    }
    
    [Fact]
    public async Task StoreAsync_WhenValue_IsEqual_PreviousValue_DoNotPublishEvent()
    {
        var ctxVault = new Mock<IContextVault>();
        var eventBus = new Mock<IEventBus>();
        var context = Context.Create(ctxVault.Object, eventBus.Object);

        ctxVault
            .Setup(vault => vault.HasItem(CtxField))
            .Returns(true);
        ctxVault
            .Setup(vault => vault.Get(CtxField))
            .Returns(FieldValue);        

        await context.StoreAsync(CtxField, FieldValue);
            
        eventBus.Verify(
            bus => bus.PublishAsync(It.IsAny<IEvent>(), It.IsAny<CancellationToken>()), 
            Times.Never);
    }
    
    [Fact]
    public async Task StoreAsync_WhenValue_IsNotEqual_PreviousValue_PublishEvent()
    {
        var ctxVault = new Mock<IContextVault>();
        var eventBus = new Mock<IEventBus>();
        var context = Context.Create(ctxVault.Object, eventBus.Object);

        ctxVault
            .Setup(vault => vault.HasItem(CtxField))
            .Returns(true);
        ctxVault
            .Setup(vault => vault.Get(CtxField))
            .Returns("DifferentValue");

        await context.StoreAsync(CtxField, FieldValue);
            
        eventBus.Verify(
            bus => bus.PublishAsync(It.IsAny<IEvent>(), It.IsAny<CancellationToken>()), 
            Times.Once);
    }
    
    [Fact]
    public async Task StoreAsync_WhenValue_IsNull_And_PreviousValue_IsNull_NotPublishEvent()
    {
        var ctxVault = new Mock<IContextVault>();
        var eventBus = new Mock<IEventBus>();
        var context = Context.Create(ctxVault.Object, eventBus.Object);

        ctxVault
            .Setup(vault => vault.HasItem(CtxField))
            .Returns(true);
        ctxVault
            .Setup(vault => vault.Get(CtxField))
            .Returns(null);

        await context.StoreAsync(CtxField!, null);
             
        eventBus.Verify(
            bus => bus.PublishAsync(It.IsAny<IEvent>(), It.IsAny<CancellationToken>()), 
            Times.Never);   
    }
    
    [Fact]
    public async Task StoreAsync_WhenValue_IsNull_And_PreviousValue_IsNotNull_PublishEvent()
    {
        var ctxVault = new Mock<IContextVault>();
        var eventBus = new Mock<IEventBus>();
        var context = Context.Create(ctxVault.Object, eventBus.Object);

        ctxVault
            .Setup(vault => vault.HasItem(CtxField))
            .Returns(true);
        ctxVault
            .Setup(vault => vault.Get(CtxField))
            .Returns(FieldValue);

        await context.StoreAsync(CtxField!, null);
            
        eventBus.Verify(
            bus => bus.PublishAsync(It.IsAny<IEvent>(), It.IsAny<CancellationToken>()), 
            Times.Once);
    }
    
    [Fact]
    public async Task StoreAsync_WhenValue_IsNotNull_And_PreviousValue_IsNull_PublishEvent()
    {
        var ctxVault = new Mock<IContextVault>();
        var eventBus = new Mock<IEventBus>();
        var context = Context.Create(ctxVault.Object, eventBus.Object);

        ctxVault
            .Setup(vault => vault.HasItem(CtxField))
            .Returns(true);
        ctxVault
            .Setup(vault => vault.Get(CtxField))
            .Returns(null);

        await context.StoreAsync(CtxField, FieldValue);
            
        eventBus.Verify(
            bus => bus.PublishAsync(It.IsAny<IEvent>(), It.IsAny<CancellationToken>()), 
            Times.Once);
    }
    
    [Fact]
    public void Create_WithContextVaultAndEventBus_ShouldReturnContextInstance()
    {
        // Arrange
        var contextVault = new Mock<IContextVault>();
        var eventBus = new Mock<IEventBus?>();

        // Act
        var context = Context.Create(contextVault.Object, eventBus.Object);

        // Assert
        Assert.NotNull(context);
    }

    [Fact]
    public void Create_NullContextVault_ShouldThrowArgumentNullException()
    {
        // Arrange
        IContextVault? contextVault = null;
        var eventBus = new Mock<IEventBus?>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => Context.Create(contextVault, eventBus.Object));
    }

    [Fact]
    public async Task StoreAsync_WhenCalled_ShouldStoreValueInContextVault()
    {
        // Arrange
        var contextVault = new Mock<IContextVault>();
        var eventBus = new Mock<IEventBus?>();
        var context = Context.Create(contextVault.Object, eventBus.Object);
        var field = ContextField<string>.WithName("TestField")!;
        const string value = "TestValue";

        // Act
        await context.StoreAsync(field, value);

        // Assert
        contextVault.Verify(cv => cv.Set(field, value), Times.Once);
    }

    [Fact]
    public async Task StoreAsync_WhenValueDoesNotChange_ShouldNotPublishEvent()
    {
        // Arrange
        var contextVault = new Mock<IContextVault>();
        var eventBus = new Mock<IEventBus?>();
        var field = ContextField<string>.WithName("TestField")!;
        const string value = "TestValue";

        contextVault.Setup(cv => cv.HasItem(field)).Returns(true);
        contextVault.Setup(cv => cv.Get(field)).Returns(value);

        var context = Context.Create(contextVault.Object, eventBus.Object);

        // Act
        await context.StoreAsync(field, value);

        // Assert
        eventBus.Verify(eb => eb.PublishAsync(It.IsAny<IEvent>(), It.IsAny<CancellationToken>())
            ,Times.Never);
    }
    
    [Fact]
    public void TryGetValue_When_FieldIsFound_ShouldReturnTrueAndValue()
    {
        // Arrange
        var contextVault = new Mock<IContextVault>();
        var field = ContextField<string>.WithName("TestField");
        const string value = "TestValue";

        contextVault.Setup(cv => cv.HasItem(field)).Returns(true);
        contextVault.Setup(cv => cv.Get(field)).Returns(value);

        var context = Context.Create(contextVault.Object);

        // Act
        var result = context.TryGetValue(field, out var retrievedValue);

        // Assert
        Assert.True(result);
        Assert.Equal(value, retrievedValue);
    }
    
    [Fact]
    public void TryGetValue_When_FieldIsNotFound_ShouldReturnFalse()
    {
        // Arrange
        var contextVault = new Mock<IContextVault>();
        var field = ContextField<string>.WithName("TestField");

        contextVault.Setup(cv => cv.HasItem(field)).Returns(false);

        var context = Context.Create(contextVault.Object);

        // Act
        var result = context.TryGetValue(field, out var retrievedValue);

        // Assert
        Assert.False(result);
        Assert.Null(retrievedValue);
    }

    [Fact]
    public async Task StoreAsync_WhenValueChanges_ShouldPublishEvent()
    {
        // Arrange
        var contextVault = new Mock<IContextVault>();
        var eventBus = new Mock<IEventBus?>();
        var field = ContextField<string>.WithName("TestField")!;
        const string oldValue = "OldValue";
        const string newValue = "NewValue";

        contextVault.Setup(cv => cv.HasItem(field)).Returns(true);
        contextVault.Setup(cv => cv.Get(field)).Returns(oldValue);

        var context = Context.Create(contextVault.Object, eventBus.Object);

        // Act
        await context.StoreAsync(field, newValue);

        // Assert
        eventBus.Verify(eb => eb.PublishAsync(It.IsAny<IEvent>() ,It.IsAny<CancellationToken>())
            , Times.Once);
    }

    [Fact]
    public void Value_WhenCalled_ShouldReturnStoredValue()
    {
        // Arrange
        var contextVault = new Mock<IContextVault>();
        var field = ContextField<string>.WithName("TestField");
        const string value = "TestValue";

        contextVault.Setup(cv => cv.HasItem(field)).Returns(true);
        contextVault.Setup(cv => cv.Get(field)).Returns(value);

        var context = Context.Create(contextVault.Object);

        // Act
        var result = context.Value(field);

        // Assert
        Assert.Equal(value, result);
    }

    [Fact]
    public void Value_WhenFieldNotPresent_ShouldReturnDefaultValue()
    {
        // Arrange
        var field = ContextField<string>.WithName("TestField");
        var contextVault = new Mock<IContextVault>();
        var context = Context.Create(contextVault.Object);
        
        contextVault.Setup(cv => cv.HasItem(field)).Returns(false);
        
        // Act
        var result = context.Value(field);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Events_ShouldContainEventAfterPublish()
    {
        // Arrange
        var contextVault = new Mock<IContextVault>();
        var eventBus = new Mock<IEventBus?>();
        var context = Context.Create(contextVault.Object, eventBus.Object);
        ContextField<string?>? field = ContextField<string>.WithName("TestField")!;

        // Act
        await context.StoreAsync(field, "NewValue");

        // Assert
        Assert.Single(context.LocalEvents);
    }

    [Fact]
    public async Task StoreAsync_ShouldBeThreadSafe()
    {
        var context = Context.Create();
        var field = ContextField<string>.WithName("TestField");
        int threadCount = 20;
        var values = new List<string>();
        for (int i = 0; i < threadCount; i++)
            values.Add($"Value_{i}");

        var tasks = new List<Task>();
        foreach (var value in values)
        {
            tasks.Add(context.StoreAsync(field, value));
        }

        await Task.WhenAll(tasks);

        // El valor final debe ser uno de los valores escritos, sin corrupción
        var finalValue = context.Value(field);
        Assert.Contains(finalValue, values);
    }

    [Fact]
    public void Events_ShouldBeEmptyInitially()
    {
        // Arrange
        var contextVault = new Mock<IContextVault>();
        var eventBus = new Mock<IEventBus?>();
        var context = Context.Create(contextVault.Object, eventBus.Object);
     
        // Act & Assert
        Assert.Empty(context.LocalEvents);
    }

    [Fact]
    public async Task StoreAsync_ShouldBeThreadSafe_WhenCalledConcurrently()
    {
        var context = Context.Create();
        var field = ContextField<string>.WithName("ConcurrentField");
        int threadCount = 50;
        var values = new List<string>();
        for (int i = 0; i < threadCount; i++)
            values.Add($"Value_{i}");

        var tasks = new List<Task>();
        foreach (var value in values)
        {
            tasks.Add(context.StoreAsync(field, value));
        }

        await Task.WhenAll(tasks);

        var finalValue = context.Value(field);
        Assert.Contains(finalValue, values);
    }

    [Fact]
    public async Task TryGetValue_ShouldReturnConsistentValue_WhenCalledConcurrently()
    {
        var context = Context.Create();
        var field = ContextField<string>.WithName("ConcurrentTryGet");
        int threadCount = 20;
        var values = new List<string>();
        for (int i = 0; i < threadCount; i++)
            values.Add($"TryValue_{i}");

        // Write values concurrently
        var writeTasks = new List<Task>();
        foreach (var value in values)
            writeTasks.Add(context.StoreAsync(field, value));
        await Task.WhenAll(writeTasks);

        // Read values concurrently
        var readTasks = new List<Task>();
        for (int i = 0; i < threadCount; i++)
        {
            readTasks.Add(Task.Run(() =>
            {
                bool found = context.TryGetValue(field, out var readValue);
                Assert.True(found);
                Assert.Contains(readValue, values);
            }));
        }
        await Task.WhenAll(readTasks);
    }

    [Fact]
    public async Task Value_ShouldReturnNull_WhenFieldNotSet_Concurrent()
    {
        var context = Context.Create();
        var field = ContextField<string>.WithName("NeverSetField");

        var tasks = new List<Task>();
        for (int i = 0; i < 10; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                var value = context.Value(field);
                Assert.Null(value);
            }));
        }
        await Task.WhenAll(tasks);
    }
}