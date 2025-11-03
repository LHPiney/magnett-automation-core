using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.WorkFlows.Runtimes;

namespace Magnett.Automation.Server.Repositories;

public interface IFlowRepository
{
    Task<Result> SaveAsync(IFlow flow, FlowState state, CancellationToken ct);
    
    Task<Result<IFlow>> GetAsync(Guid id, CancellationToken ct);
    
    Task<Result<FlowState>> GetStateAsync(Guid id, CancellationToken ct);
    
    Task<Result<IEnumerable<IFlow>>> GetByStateAsync(FlowState state, CancellationToken ct);
    
    Task<Result> UpdateStateAsync(Guid id, FlowState state, CancellationToken ct);
    
    Task<Result> DeleteAsync(Guid id, CancellationToken ct);
    
    Task<Result<bool>> ExistsAsync(Guid id, CancellationToken ct);
}

