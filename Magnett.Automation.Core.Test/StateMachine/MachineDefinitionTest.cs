﻿using System;
using Xunit;

using Magnett.Automation.Core.StateMachine;
using Magnett.Automation.Core.StateMachine.Collections;
using Moq;

namespace Magnett.Automation.Core.Test.StateMachine
{
    public class MachineDefinitionTest
    {
        private const string InitialStateName = "InitialState";

        [Fact]
        public void Create_When_Invoke_Without_InitialState_Throws_Exception()
        {
            var stateList    = new Mock<StateList>();
            
            Assert.Throws<ArgumentNullException>(()=>
                MachineDefinition.Create(null, stateList.Object));
            
        }
        
        [Fact]
        public void Create_When_Invoke_Without_StateList_Throws_Exception()
        {
            var initialState = new Mock<IState>();

            Assert.Throws<ArgumentNullException>(()=>
                MachineDefinition.Create(initialState.Object, null));
        }
        
        [Fact]
        public void Create_When_Invoke_Return_Instance()
        {
            var initialState = new Mock<IState>();
            var stateList    = new Mock<StateList>();
            
            var definition = MachineDefinition.Create(
                initialState.Object, 
                stateList.Object);
            
            Assert.NotNull(definition);
        }
        
        [Fact]
        public void Create_When_InitialState_Is_Good_Is_Proper_Stored()
        {
            var initialState = new Mock<IState>();
            var stateList    = new Mock<StateList>();

            var definition = MachineDefinition.Create(
                initialState.Object, 
                stateList.Object);

            Assert.NotNull(definition.InitialState);
            Assert.Equal(initialState.Object, definition.InitialState);
        }
        
        [Fact]
        public void HasState_When_Invoke_Call_StateList_HasItem()
        {
            var initialState = new Mock<IState>();
            var stateList    = new Mock<StateList>();
            
            var definition = MachineDefinition.Create(
                initialState.Object, 
                stateList.Object);

            _ = definition.HasState(InitialStateName);
            
            stateList.Verify(
                dic =>  dic.HasItem(InitialStateName), 
                Times.Once);
        }
        
        [Fact]
        public void GetState_When_Invoke_Call_StateList_Get()
        {
            var initialState = new Mock<IState>();
            var stateList    = new Mock<StateList>();
            
            var definition = MachineDefinition.Create(
                initialState.Object, 
                stateList.Object);

            stateList
                .Setup(list => list.GetItem(InitialStateName))
                .Returns(initialState.Object);

            _ = definition.GetState(InitialStateName);
            
            stateList.Verify(
                dic =>  dic.GetItem(InitialStateName), 
                Times.Once);
        }
    }
}