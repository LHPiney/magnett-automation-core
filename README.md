# magnett automation

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=magnett_automation&metric=alert_status)](https://sonarcloud.io/dashboard?id=magnett_automation) [![Coverage](https://sonarcloud.io/api/project_badges/measure?project=magnett_automation&metric=coverage)](https://sonarcloud.io/dashboard?id=magnett_automation)
![GitHub issues](https://img.shields.io/github/issues/lhpiney/magnett-automation-core)

Library for create custom workflow, orquestation between components, microservices and other automation utilities

## Introduction

This library has been designed to have an ecosystem of entities oriented to the orchestration of any type of process. From the simplest processes such as calls between several of our classes to more complex processes such as calls to microservices, etc. Always, however, in a declarative way and looking to save code in terms of tedious and complex nesting of “If”, “else”... etc.
Structure 

In this repository, will be the core classes of this ecosystem, grouped under the namespace magnett.automation.core. Some of these classes can be used outside the scope of creating a workflow, as they are generic enough to be useful independently.

- Common
- Context.
- StateMachine.
- Workflows.

## Common

In this namespace we found utilities class used inside the libray classes

  - Enumeration
  - CommonNamedKey
  - DictionaryWrapper

## Context

The concept context is something very generic and is used in several fields. For us, a context will be a common space where values are stored to be shared between several components, being that they are values of the heterogeneous type. 

We are, therefore, in front of a key/value system, where the values will be of any type.

### Structure

The structure of the context is simple, it is formed only by the *Context* class, which will be our input and retrieval of values class, and the *IContextVault* interface which will be the definition of the vault where the values are stored.

By default, we will have an implementation of the *IContextVault*, where it will store the values in memory, but will be open for any other implementation that stores these values in any other way.

We will use the class *ContextField* to Get and Set values in a context, with this class we can define the type and the name of the class.

Example of how to Get / Set value in a context

```csharp
var context = Context.Create();
var field   = ContextField<int>.Create("FieldName");

//Set Value
context.Store(field, random.Next(1000));

//Get Value
var value  = context.Value(field);
 ```

## StateMachine

We have at our disposal several interfaces for the definition of a state machine as well as its execution. The definition of the machine will be separated from the execution of the machine itself, to avoid couplings and clear separation of responsibilities.

### Structure
           
The main interface is *IMachine*. with this interface we will have access to the current state, *IState* interface, and the possibility of transitioning to another state using action codes that generate a transition, *ITransaction* entity, to another state.

It is not possible to go from one state to another directly, only through a transition, so that we have a model to which states we can go from one in particular.

A state without defined transitions can be given and this means that the state is terminal. In this way, we can define finite or non-finite machines.

Regarding the runtime part, the definition of a machine will be done from the *IMachineDefinition* interface, which will be generated from the *MachineDefinitionBuilder* class.

Example of machine definition code


```csharp

//Helper class with states enumeration
public class State : Enumeration
{
    public static readonly State Init     = new State(1, "Init");
    public static readonly State Working  = new State(2, "Working");
    public static readonly State Paused   = new State(3, "Paused");
    public static readonly State Finished = new State(4, "Finished");

    private State(int id, string name) : base(id, name)
    {

    }
}

//Helper class with action enumerations
public class Action : Enumeration
{
    public static readonly Action Start    = new Action(1, "Start");
    public static readonly Action Pause    = new Action(2, "Pause");
    public static readonly Action Continue = new Action(3, "Continue");
    public static readonly Action Finish   = new Action(4, "Finish");

    private Action(int id, string name) : base(id, name)
    {

    }
}

//Now we can create a definition
_definition = MachineDefinitionBuilder.Create()

    .InitialState(State.Init)
        .OnAction(Action.Start).ToState(State.Working)
        .Build()

    .AddState(State.Working)
        .OnAction(Action.Pause).ToState(State.Paused)
        .OnAction(Action.Finish).ToState(State.Finished)
        .Build()

    .AddState(State.Paused)
        .OnAction(Action.Continue).ToState(State.Working)
        .Build()

    .AddState(State.Finished)
        .Build()

    .BuildDefinition();
 ```

Example of machine creation and usage code.

## Workflows    

Under this namespace, we will have the necessary classes to define a workflow and execute it. As in the previous section, we will keep the workflow definition separate from the execution. 

### Structure

This separation will be done using the *IWorkflowDefinition* and IWorkflowRunner interfaces.

Example workflow definition code.

A runner, to instantiate itself, will need to receive the workflow definition and a context instance that will be used to share information between nodes. Once the workflow has been executed we can retrieve return values if there are any.

To encapsulate the definition and execution we have the *IFlow* interface, this interface will allow us in the future to build subflows, create flows that are encapsulated as a service within more complex applications... etc.

Example Flow runner


