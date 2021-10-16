﻿using System;
using Magnett.Automation.Core.Commons;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SimpleFlow.Definitions.Nodes
{
    internal class SetValue : Common
    {
        #region ExitCodes

        public class ExitCode : Enumeration
        {
            public static readonly ExitCode Assigned  = new ExitCode(1, "Assigned"); 

            private ExitCode(int id, string name) : base(id, name)
            {
            }
        }
        
        #endregion
        
        private SetValue(CommonNamedKey key, ContextDefinition contextDefinition) : 
            base(key, contextDefinition)
        {
        }
        

        public override void Execute()
        {
            var random = new Random();
            
            GlobalContext.Store(ContextDefinition.FirstDigit, random.Next(1000));
            GlobalContext.Store(ContextDefinition.SecondDigit, random.Next(1000));
        }
        
        public static SetValue Create(CommonNamedKey key, ContextDefinition contextDefinition)
        {
            return new SetValue(key, contextDefinition);
        }
    }
}