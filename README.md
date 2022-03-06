# magnett automation
[![Build](https://github.com/LHPiney/magnett-automation-core/actions/workflows/build.yml/badge.svg)](https://github.com/LHPiney/magnett-automation-core/actions/workflows/build.yml)
[![Build Status](https://dev.azure.com/Magnett/Magnett.Automation/_apis/build/status/magnett-automation-core?branchName=azure-pipelines)](https://dev.azure.com/Magnett/Magnett.Automation/_build/latest?definitionId=1&branchName=azure-pipelines)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=magnett_automation&metric=alert_status)](https://sonarcloud.io/dashboard?id=magnett_automation) [![Coverage](https://sonarcloud.io/api/project_badges/measure?project=magnett_automation&metric=coverage)](https://sonarcloud.io/dashboard?id=magnett_automation)
![GitHub issues](https://img.shields.io/github/issues/lhpiney/magnett-automation-core)
![Nuget](https://img.shields.io/nuget/v/Magnett.Automation.Core)
![Nuget](https://img.shields.io/nuget/dt/Magnett.Automation.Core)

![Logo](./assets/logo.svg)

Library designed for create custom workflow, orquestation between components, microservices and other automation utilities, created on dotnet version 6

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
    public static readonly State Init     = new State(1, nameof(Init));
    public static readonly State Working  = new State(2, nameof(Working));
    public static readonly State Paused   = new State(3, nameof(Paused));
    public static readonly State Finished = new State(4, nameof(Finished));

    private State(int id, string name) : base(id, name)
    {

    }
}

//Helper class with action enumerations
public class Action : Enumeration
{
    public static readonly Action Start    = new Action(1, nameof(Start));
    public static readonly Action Pause    = new Action(2, nameof(Pause));
    public static readonly Action Continue = new Action(3, nameof(Continue));
    public static readonly Action Finish   = new Action(4, nameof(Finish));

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

```csharp
var machine = Machine
    .Create(SimpleMachineDefinition.GetDefinition());

machine.Dispatch(Action.Start);

var currentState = machine.State;
```

## Workflows    

Under this namespace, we will have the necessary classes to define a workflow and execute it. As in the previous section, we will keep the workflow definition separate from the execution. 

### Structure

This separation will be done using the *IWorkflowDefinition* and *IWorkflowRunner* interfaces.

To encapsulate the definition and execution we have the *IFlow* interface, this interface also will allow us in the future to build subflows, create flows that are encapsulated as a service within more complex applications... etc.

If we think in a basic flow,just and initial node to reset field values, next node just to caculate to random numbers, and a final node to sum both values the definition should be somthin like that.

Example workflow definition code.

```csharp
  var contextDefinition = ContextDefinition.Create();

  var definition = FlowDefinitionBuilder.Create()
      .WithInitialNode(ResetValue.Create(Node.Reset, contextDefinition))
          .OnExitCode(ResetValue.ExitCode.Ok).GoTo(Node.SetValue)
      .Build()

      .WithNode(SetValue.Create(Node.SetValue, contextDefinition))
          .OnExitCode(SetValue.ExitCode.Assigned).GoTo(Node.SumValue)
      .Build()

      .WithNode(SumValue.Create(Node.SumValue, contextDefinition)).Build()

      .BuildDefinition();
 ```

Previously you have defined some helper classes like *ContextDefinition* it's just a class to contains Context field and to avoid duplication with name definitions.

```csharp
internal class ContextDefinition
{
    public ContextField<int> FirstDigit  { get; }
    public ContextField<int> SecondDigit { get; }      
    public ContextField<int> Result      { get; }

    private ContextDefinition()
    {
        FirstDigit  =  ContextField<int>.Create("FieldOne");           
        SecondDigit =  ContextField<int>.Create("FieldTwo");           
        Result      =  ContextField<int>.Create("FieldResult");
    }

    public static ContextDefinition Create()
    {
        return new ContextDefinition();
    }
}
 ```
 We have created also the abstract class *Common* that we will use as base class for all of our node classes, we will use as a way to ensure that all nodes have avaliable ConxtextDefinition
 
```csharp
internal abstract class Common : Core.WorkFlows.Implementations.Node
{
    protected ContextDefinition ContextDefinition { get; }

    protected Common(CommonNamedKey key, ContextDefinition contextDefinition) : base(key)
    {
        ContextDefinition = contextDefinition
                            ?? throw new ArgumentNullException(nameof(contextDefinition));
    }
}
 ```
 We have two node types sync and async, under the *INode* and *INodeAsync* interfaces, so we can use nodes as a wrapper of both type of process. In this example we have only the sync implementation.

 In our example we will use only sync nodes.

Example Node

```csharp
internal class ResetValue : Common
{
    #region ExitCodes

    public class ExitCode : Enumeration
    {
        public static readonly ExitCode Ok  = new ExitCode(1, "Ok"); 

        private ExitCode(int id, string name) : base(id, name)
        {
        }
    }
    
    #endregion

    private ResetValue(CommonNamedKey key, ContextDefinition contextDefinition) : 
        base(key, contextDefinition)
    {
        
    }

    public override NodeExit Execute(Context context)
    {
        context.Store(ContextDefinition.FirstDigit, 0);
        context.Store(ContextDefinition.SecondDigit, 0);
        context.Store(ContextDefinition.Result, 0);
        
        return NodeExit.Create(ExitCode.Ok.Name);
    }

    public static ResetValue Create(CommonNamedKey name, ContextDefinition contextDefinition)
    {
        return new ResetValue(name, contextDefinition);
    }
}
 ```

The inner class *ExitCodes* is just another helper class, build over Enumeration class with the definition of avaliable exit codes for this node, we use also somthin similira

A runner, to instantiate itself, will need to receive the workflow definition and a context instance that will be used to share information between nodes. Once the runner has been executed we can retrieve return values from context if there are any.

We have the abstract class *FlowRunnerBase* so we can implement our custom runners, step to step, distributed, etc..


Example Flow runner

```csharp
var flowRunner = FlowRunner.Create(definition, Context.Create());

var exit = await flowRunner.Start();
 ```

 The class flow as we said before, it's a wrapper for all this process, now have basic functionalities but in future versions will be used as main class for workflow management. 

```csharp
var definition = SimpleFlowDefinition.GetDefinition();
var context    = Context.Create();

var flow = Flow.Create(FlowRunner.Create(definition, context));

var exit = await flow.Run();
 ```
