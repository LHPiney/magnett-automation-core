using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Text.Json.Serialization;
using Magnett.Automation.Core.WorkFlows.Serialization;
using Magnett.Automation.Core.Events;
using Magnett.Automation.Core.Contexts.Serializations.Attributes;
using Magnett.Automation.Core.Contexts.Serializations.Implementations;
using Microsoft.Extensions.Logging;

namespace Magnett.Automation.Core.Contexts.Serializations;

internal static class ContextSerializationHelper
{
    private static readonly ConcurrentDictionary<Type, bool> CompatibilityCache = new();
    
    internal static ContextSnapshot CreateSnapshot(Context context, IFlowCodec codec, ILogger? logger = null)
    {
        var snapshot = new ContextSnapshot
        {
            Id = context.Id,
            CreatedAt = context.CreatedAt,
            Fields = []
        };

        var vault = GetContextVault(context);
        var eventBus = GetEventBusFromContext(context);
        var serializationContext = new SerializationContext(
            eventBus,
            logger,
            null);

        foreach (var key in vault.GetKeys())
        {
            var value = vault.Get(key);
            var fieldSnapshot = CreateFieldSnapshot(key.Name, value, codec, serializationContext);
            snapshot.Fields.Add(fieldSnapshot);
        }

        return snapshot;
    }

    internal static Context CreateFromSnapshot(ContextSnapshot snapshot, IFlowCodec codec, IEventBus? eventBus = null, ILogger? logger = null)
    {
        var vault = ContextVault.Create();
        var serializationContext = new SerializationContext(
            eventBus,
            logger,
            null);
        
        foreach (var fieldSnapshot in snapshot.Fields)
        {
            var value = DeserializeFieldValue(fieldSnapshot, codec, serializationContext);
            if (value == null) continue;

            var fieldType = Type.GetType(fieldSnapshot.FieldType);
            if (fieldType == null) continue;

            var field = CreateContextField(fieldSnapshot.FieldName, fieldType);
            vault.Set(field, value);
        }
        
        return CreateContextFromSnapshot(vault, snapshot.Id, snapshot.CreatedAt, eventBus);
    }

    private static CommonNamedKey CreateContextField(string fieldName, Type fieldType)
    {
        var contextFieldType = typeof(ContextField<>).MakeGenericType(fieldType);
        var withNameMethod = contextFieldType.GetMethod("WithName", new[] { typeof(string) });
        return (CommonNamedKey)withNameMethod!.Invoke(null, new object[] { fieldName })!;
    }

    private static Context CreateContextFromSnapshot(IContextVault vault, Guid id, DateTime createdAt, IEventBus? eventBus)
    {
        var contextType = typeof(Context);
        var constructor = contextType.GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            [typeof(IContextVault), typeof(Guid), typeof(DateTime), typeof(IEventBus)],
            null);
        
        return (Context)constructor!.Invoke(new object?[] { vault, id, createdAt, eventBus });
    }

    private static ContextFieldSnapshot CreateFieldSnapshot(string fieldName, object? value, IFlowCodec codec, ISerializationContext context)
    {
        if (value == null)
        {
            return new ContextFieldSnapshot
            {
                FieldName = fieldName,
                FieldType = "System.Object",
                Value = null,
                SerializedValue = null
            };
        }

        var fieldType = value.GetType();
        var isSimpleType = IsSimpleType(fieldType);

        if (isSimpleType)
        {
            return new ContextFieldSnapshot
            {
                FieldName = fieldName,
                FieldType = fieldType.AssemblyQualifiedName ?? "System.Object",
                Value = value,
                SerializedValue = null
            };
        }

        if (value is IContextSerializable customSerializable)
        {
            try
            {
                return customSerializable.Serialize(fieldName, context);
            }
            catch (Exception ex)
            {
                context.Logger?.LogWarning(ex, "Failed to serialize field '{FieldName}' of type '{FieldType}' via IContextSerializable", 
                    fieldName, fieldType.Name);
                return CreateEmptySnapshot(fieldName, fieldType);
            }
        }

        var isCompatibleComplexType = IsContextCompatibleType(fieldType, context.Logger);
        if (isCompatibleComplexType)
        {
            return new ContextFieldSnapshot
            {
                FieldName = fieldName,
                FieldType = fieldType.AssemblyQualifiedName ?? "System.Object",
                Value = null,
                SerializedValue = SerializeToJsonString(value, codec, context.Logger)
            };
        }

        context.Logger?.LogWarning("Field '{FieldName}' of type '{FieldType}' is not compatible with context serialization and will be skipped", 
            fieldName, fieldType.Name);
            
        return CreateEmptySnapshot(fieldName, fieldType);
    }

    private static ContextFieldSnapshot CreateEmptySnapshot(string fieldName, Type fieldType)
    {
        return new ContextFieldSnapshot
        {
            FieldName = fieldName,
            FieldType = fieldType.AssemblyQualifiedName ?? "System.Object",
            Value = null,
            SerializedValue = null
        };
    }

    private static object? DeserializeFieldValue(ContextFieldSnapshot fieldSnapshot, IFlowCodec codec, ISerializationContext context)
    {
        if (fieldSnapshot.Value != null)
        {
            return fieldSnapshot.Value;
        }

        if (string.IsNullOrEmpty(fieldSnapshot.SerializedValue)) return null;
        
        var type = Type.GetType(fieldSnapshot.FieldType);
        if (type == null) return null;

        if (typeof(IContextSerializable).IsAssignableFrom(type))
        {
            try
            {
                var deserializeMethod = type.GetMethod("Deserialize", 
                    BindingFlags.Static | BindingFlags.Public,
                    null,
                    [typeof(ContextFieldSnapshot), typeof(ISerializationContext)],
                    null);

                if (deserializeMethod != null)
                {
                    var result = deserializeMethod.Invoke(null, [fieldSnapshot, context]);
                    return result;
                }

                context.Logger?.LogWarning("Type '{TypeName}' implements IContextSerializable but does not have a static Deserialize method", 
                    type.Name);
            }
            catch (Exception ex)
            {
                context.Logger?.LogWarning(ex, "Failed to deserialize field '{FieldName}' of type '{TypeName}' via IContextSerializable", 
                    fieldSnapshot.FieldName, type.Name);
                return null;
            }
        }

        if (IsContextCompatibleType(type, context.Logger))
        {
            return DeserializeFromJsonString(fieldSnapshot.SerializedValue, type, codec, context.Logger);
        }

        return null;
    }

    private static bool IsSimpleType(Type? type)
    {
        if (type == null) return true;
        
        return type.IsPrimitive || 
               type == typeof(string) || 
               type == typeof(DateTime) || 
               type == typeof(DateTimeOffset) ||
               type == typeof(TimeSpan) ||
               type == typeof(Guid) ||
               type == typeof(decimal) ||
               type == typeof(Uri) ||
               type.IsEnum ||
               Nullable.GetUnderlyingType(type) != null;
    }

    private static bool IsContextCompatibleType(Type type, ILogger? logger = null)
    {
        if (CompatibilityCache.TryGetValue(type, out var cachedResult))
        {
            return cachedResult;
        }

        bool isValid;

        if (typeof(IContextSerializable).IsAssignableFrom(type))
        {
            isValid = true;
        }
        else
        {
            var hasAttribute = type.GetCustomAttribute<ContextSerializableAttribute>() != null;
            if (hasAttribute)
            {
                isValid = ValidateSerializableType(type, logger);
                if (!isValid)
                {
                    logger?.LogError(
                        "Type '{TypeName}' is marked with [ContextSerializable] but fails validation. " +
                        "Ensure it has a parameterless constructor and all properties are serializable or marked with [ContextIgnore].",
                        type.Name);
                }
            }
            else
            {
                isValid = ValidateSerializableTypeLegacy(type);
                if (!isValid)
                {
                    logger?.LogWarning(
                        "Type '{TypeName}' may not serialize correctly. " +
                        "Consider marking it with [ContextSerializable] attribute for better performance and validation.",
                        type.Name);
                }
            }
        }

        CompatibilityCache[type] = isValid;
        return isValid;
    }

    private static bool ShouldIgnoreProperty(PropertyInfo property)
    {
        if (property.GetCustomAttribute<ContextIgnoreAttribute>() != null)
            return true;

        if (property.GetCustomAttribute<JsonIgnoreAttribute>() != null)
            return true;

        var ignoreDataMemberType = Type.GetType("System.Runtime.Serialization.IgnoreDataMemberAttribute, System.Runtime.Serialization.Primitives");
        if (ignoreDataMemberType != null && property.GetCustomAttribute(ignoreDataMemberType) != null)
            return true;

        if (!property.CanWrite)
            return true;

        return false;
    }

    private static bool ValidateSerializableType(Type type, ILogger? logger = null)
    {
        if (type.IsAbstract) return false;
        if (type.GetConstructor(Type.EmptyTypes) == null) return false;

        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in properties)
        {
            if (ShouldIgnoreProperty(prop))
            {
                var ignoreAttr = prop.GetCustomAttribute<ContextIgnoreAttribute>();
                if (ignoreAttr != null)
                {
                    logger?.LogDebug(
                        "Skipping property '{PropertyName}' in type '{TypeName}'. Reason: {Reason}",
                        prop.Name, type.Name, ignoreAttr.Reason ?? "Marked with [ContextIgnore]");
                }
                continue;
            }

            if (!prop.CanRead || !prop.CanWrite)
            {
                logger?.LogWarning(
                    "Property '{PropertyName}' in type '{TypeName}' is not readable/writable",
                    prop.Name, type.Name);
                return false;
            }

            var propType = prop.PropertyType;
            if (!IsSimpleType(propType) && !IsContextCompatibleType(propType, logger))
            {
                logger?.LogWarning(
                    "Property '{PropertyName}' of type '{PropertyType}' in '{TypeName}' is not serializable",
                    prop.Name, propType.Name, type.Name);
                return false;
            }
        }

        return true;
    }

    private static bool ValidateSerializableTypeLegacy(Type type)
    {
        var hasParameterlessConstructor = type.GetConstructors()
            .Any(c => c.GetParameters().Length == 0);

        if (!hasParameterlessConstructor) return false;

        var properties = type.GetProperties();
        foreach (var prop in properties)
        {
            var propType = prop.PropertyType;

            if (IsNonSerializableType(propType)) return false;
            if (IsExternalDependency(propType)) return false;
        }

        return true;
    }

    private static bool IsNonSerializableType(Type type)
    {
        return type.IsInterface ||
               type.IsAbstract ||
               type == typeof(IEventBus) ||
               type == typeof(ILogger) ||
               type == typeof(ILogger<>) ||
               type.IsSubclassOf(typeof(Delegate)) ||
               type.IsSubclassOf(typeof(MulticastDelegate));
    }

    private static bool IsExternalDependency(Type type)
    {
        var namespaceName = type.Namespace ?? "";
        return namespaceName.StartsWith("Microsoft.Extensions") ||
               namespaceName.StartsWith("System.Threading") ||
               namespaceName.StartsWith("System.IO");
    }

    private static string? SerializeToJsonString(object? value, IFlowCodec codec, ILogger? logger = null)
    {
        if (value == null) return null;
        
        try
        {
            var bytes = codec.Serialize(value);
            return Encoding.UTF8.GetString(bytes.ToArray());
        }
        catch (Exception ex)
        {
            logger?.LogWarning(ex, "Failed to serialize value of type '{ValueType}' using codec '{CodecId}'", 
                value.GetType().Name, codec.Id);
            return null;
        }
    }

    private static object? DeserializeFromJsonString(string jsonString, Type type, IFlowCodec codec, ILogger? logger = null)
    {
        if (string.IsNullOrEmpty(jsonString)) return null;

        try
        {
            var bytes = Encoding.UTF8.GetBytes(jsonString);
            var helperMethod = typeof(ContextSerializationHelper)
                .GetMethod(nameof(DeserializeGeneric), BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(type);

            return helperMethod.Invoke(null, [codec, bytes]);
        }
        catch (Exception ex)
        {
            logger?.LogWarning(ex, "Failed to deserialize JSON to type '{TypeName}' using codec '{CodecId}'", 
                type.Name, codec.Id);
            return null;
        }
    }

    private static T? DeserializeGeneric<T>(IFlowCodec codec, byte[] bytes)
    {
        return codec.Deserialize<T>(new ReadOnlySpan<byte>(bytes));
    }

    private static IContextVault GetContextVault(Context context)
    {
        var field = typeof(Context).GetField("_contextVault", 
            BindingFlags.NonPublic | BindingFlags.Instance);
        return (IContextVault)field!.GetValue(context)!;
    }

    private static IEventBus? GetEventBusFromContext(Context context)
    {
        var property = typeof(Context).BaseType!
            .GetProperty("EventBus", BindingFlags.NonPublic | BindingFlags.Instance);
        return (IEventBus?)property?.GetValue(context);
    }
}
