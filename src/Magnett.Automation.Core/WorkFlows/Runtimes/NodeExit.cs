namespace Magnett.Automation.Core.WorkFlows.Runtimes;

public record NodeExit
{
    public string Code { get; }
    public string Data { get; }
    public ExitState State { get; }

    private NodeExit(
        string code,
        ExitState state,
        string data)
    {
        if (string.IsNullOrEmpty(code))
            throw new ArgumentNullException(nameof(code));

        Code  = code;
        State = state;
        Data  = data;
    }
    
    public static NodeExit Paused(
        string code, 
        string data = null)
    {
        return new NodeExit(code, ExitState.Paused, data);
    }
    
    public static NodeExit Paused(
        Enumeration code, 
        string data = null)
    {
        return new NodeExit(code?.Name, ExitState.Paused, data);
    }
    
    public static NodeExit Failed(
        string code, 
        string data = null)
    {
        return new NodeExit(code, ExitState.Failed, data);
    }   
    
    public static NodeExit Failed(
        Enumeration code, 
        string data = null)
    {
        return new NodeExit(code?.Name, ExitState.Failed, data);
    }
    
    public static NodeExit Cancelled(
        string code, 
        string data = null)
    {
        return new NodeExit(code, ExitState.Cancelled, data);
    }
    
    public static NodeExit Cancelled(
        Enumeration code, 
        string data = null)
    {
        return new NodeExit(code?.Name, ExitState.Cancelled, data);
    }
    
    public static NodeExit Completed(
        string code, 
        string data = null)
    {
        return new NodeExit(code, ExitState.Completed, data);
    }
    
    public static NodeExit Completed(
        Enumeration code, 
        string data = null)
    {
        return new NodeExit(code?.Name, ExitState.Completed, data);
    }
}