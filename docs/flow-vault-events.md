# FlowVault Events

Este documento identifica los tipos de eventos que el `FlowVault` podría emitir para notificar sobre las operaciones realizadas sobre definiciones de flujos e instancias de flujos.

## Eventos de Definiciones (Flow Definitions)

### 1. FlowDefinitionPublishedEvent
**Cuándo se emite:** Cuando una definición de flujo es publicada exitosamente (`PublishDefinitionAsync`).

**Datos:**
```csharp
public record FlowDefinitionPublishedEvent(
    string DefinitionKey,
    int Version,
    DateTime PublishedAt
) : Event("FlowDefinitionPublished", "FlowVault");
```

**Uso:** Notificar a sistemas externos sobre nuevas versiones de definiciones, actualizar cachés, etc.

---

### 2. FlowDefinitionPublishFailedEvent
**Cuándo se emite:** Cuando falla la publicación de una definición.

**Datos:**
```csharp
public record FlowDefinitionPublishFailedEvent(
    string DefinitionKey,
    int Version,
    string ErrorMessage,
    Exception? Exception = null
) : Event("FlowDefinitionPublishFailed", "FlowVault");
```

**Uso:** Logging, alertas, sistemas de notificación.

---

## Eventos de Instancias (Flow Instances)

### 3. FlowInstanceCreatedEvent
**Cuándo se emite:** Cuando una instancia de flujo es creada exitosamente (`CreateInstanceAsync`).

**Datos:**
```csharp
public record FlowInstanceCreatedEvent(
    Guid FlowId,
    string DefinitionKey,
    int DefinitionVersion,
    DateTime CreatedAt
) : Event("FlowInstanceCreated", "FlowVault");
```

**Uso:** 
- Notificar inicio de ejecuciones
- Inicializar monitoreo
- Actualizar dashboards
- Integraciones con sistemas externos

---

### 4. FlowInstanceCreationFailedEvent
**Cuándo se emite:** Cuando falla la creación de una instancia.

**Datos:**
```csharp
public record FlowInstanceCreationFailedEvent(
    string DefinitionKey,
    string ErrorMessage,
    Exception? Exception = null
) : Event("FlowInstanceCreationFailed", "FlowVault");
```

**Uso:** Alertas, análisis de errores.

---

### 5. FlowInstanceStateChangedEvent
**Cuándo se emite:** Cuando el estado de una instancia cambia (`PauseInstanceAsync`, `ResumeInstanceAsync`, o cambios de estado internos).

**Datos:**
```csharp
public record FlowInstanceStateChangedEvent(
    Guid FlowId,
    string PreviousState,
    string NewState,
    DateTime ChangedAt,
    string? Reason = null
) : Event("FlowInstanceStateChanged", "FlowVault");
```

**Uso:**
- Tracking de estados
- Auditoría
- Dashboards en tiempo real
- Notificaciones a usuarios

---

### 6. FlowInstancePausedEvent
**Cuándo se emite:** Cuando una instancia es pausada exitosamente (`PauseInstanceAsync`).

**Datos:**
```csharp
public record FlowInstancePausedEvent(
    Guid FlowId,
    DateTime PausedAt,
    string? Reason = null
) : Event("FlowInstancePaused", "FlowVault");
```

**Uso:** 
- Notificar pausas manuales/automáticas
- Sistema de alertas
- Análisis de rendimiento

---

### 7. FlowInstanceResumedEvent
**Cuándo se emite:** Cuando una instancia es reanudada exitosamente (`ResumeInstanceAsync`).

**Datos:**
```csharp
public record FlowInstanceResumedEvent(
    Guid FlowId,
    DateTime ResumedAt
) : Event("FlowInstanceResumed", "FlowVault");
```

**Uso:** Monitoreo, métricas de tiempo de pausa.

---

### 8. FlowInstanceSavedEvent
**Cuándo se emite:** Cuando una instancia es guardada exitosamente (`SaveInstanceAsync`).

**Datos:**
```csharp
public record FlowInstanceSavedEvent(
    Guid FlowId,
    string State,
    DateTime SavedAt
) : Event("FlowInstanceSaved", "FlowVault");
```

**Uso:**
- Confirmar persistencia
- Sincronización entre sistemas
- Backup/Replicación

---

### 9. FlowInstanceSaveFailedEvent
**Cuándo se emite:** Cuando falla el guardado de una instancia.

**Datos:**
```csharp
public record FlowInstanceSaveFailedEvent(
    Guid FlowId,
    string ErrorMessage,
    Exception? Exception = null
) : Event("FlowInstanceSaveFailed", "FlowVault");
```

**Uso:** Alertas críticas, logging de errores.

---

## Eventos Adicionales (Opcionales)

### 10. FlowInstanceCompletedEvent
**Cuándo se emite:** Cuando una instancia completa su ejecución (si se implementa detección automática).

**Datos:**
```csharp
public record FlowInstanceCompletedEvent(
    Guid FlowId,
    DateTime CompletedAt,
    bool Success,
    string? FailureReason = null
) : Event("FlowInstanceCompleted", "FlowVault");
```

**Uso:** 
- Notificar finalización
- Limpieza de recursos
- Reportes de ejecución

---

### 11. FlowInstanceFailedEvent
**Cuándo se emite:** Cuando una instancia falla durante la ejecución.

**Datos:**
```csharp
public record FlowInstanceFailedEvent(
    Guid FlowId,
    string ErrorMessage,
    Exception? Exception = null,
    DateTime FailedAt = default
) : Event("FlowInstanceFailed", "FlowVault");
```

**Uso:** Alertas, análisis de errores, notificaciones.

---

### 12. FlowDefinitionRetrievedEvent
**Cuándo se emite:** Cuando se recupera una definición (`GetDefinitionAsync`) - útil para auditoría.

**Datos:**
```csharp
public record FlowDefinitionRetrievedEvent(
    string DefinitionKey,
    int? Version,
    DateTime RetrievedAt
) : Event("FlowDefinitionRetrieved", "FlowVault");
```

**Uso:** Auditoría, métricas de acceso.

---

### 13. FlowInstanceRetrievedEvent
**Cuándo se emite:** Cuando se recupera una instancia (`GetInstanceAsync`) - útil para auditoría.

**Datos:**
```csharp
public record FlowInstanceRetrievedEvent(
    Guid FlowId,
    DateTime RetrievedAt
) : Event("FlowInstanceRetrieved", "FlowVault");
```

**Uso:** Auditoría, métricas de acceso.

---

## Priorización de Implementación

### Fase 1 (Críticos - MVP):
1. ✅ `FlowInstanceCreatedEvent` - Esencial para monitoreo
2. ✅ `FlowInstanceStateChangedEvent` - Tracking de estados
3. ✅ `FlowDefinitionPublishedEvent` - Gestión de versiones

### Fase 2 (Importantes):
4. ✅ `FlowInstancePausedEvent`
5. ✅ `FlowInstanceResumedEvent`
6. ✅ `FlowInstanceSaveFailedEvent` - Alertas críticas

### Fase 3 (Mejoras):
7. ✅ `FlowInstanceCreationFailedEvent`
8. ✅ `FlowDefinitionPublishFailedEvent`
9. ✅ `FlowInstanceSavedEvent` - Si se necesita confirmación

### Fase 4 (Opcionales - Auditoría):
10. ✅ `FlowInstanceCompletedEvent`
11. ✅ `FlowInstanceFailedEvent`
12. ✅ `FlowDefinitionRetrievedEvent`
13. ✅ `FlowInstanceRetrievedEvent`

---

## Ejemplo de Uso

```csharp
// En FlowVault.CreateInstanceAsync después de guardar exitosamente:
if (saveResult.IsSuccess)
{
    await _eventBus.PublishAsync(new FlowInstanceCreatedEvent(
        FlowId: flow.Id,
        DefinitionKey: definitionKey,
        DefinitionVersion: definition.Version,
        CreatedAt: DateTime.UtcNow
    ), ct);
    
    _logger.LogInformation("Created workflow instance {FlowId} from definition {DefinitionKey}", flow.Id, definitionKey);
}
```

---

## Notas de Diseño

1. **Caller**: Todos los eventos usan `"FlowVault"` como caller para identificar el origen.
2. **Timestamps**: Usar `DateTime.UtcNow` para consistencia.
3. **Error Handling**: Los eventos de error incluyen `Exception?` opcional para debugging.
4. **Idempotencia**: Los eventos deben ser idempotentes (poder procesarse múltiples veces sin efectos secundarios).
5. **Versionado**: Los eventos son records inmutables, facilitando el versionado futuro.

