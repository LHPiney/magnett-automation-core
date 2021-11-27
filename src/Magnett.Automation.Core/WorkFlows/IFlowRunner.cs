﻿using System.Threading.Tasks;
using Magnett.Automation.Core.Contexts;

namespace Magnett.Automation.Core.WorkFlows
{
    public interface IFlowRunner
    {
        public Context FlowContext { get; }
        public Task Start();
    }
}