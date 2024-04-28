namespace Magnett.Automation.Core.WorkFlows.Runtimes;

public record NodeExit
{
    public string Code    { get; }
    public bool   IsError { get; }
    public string Data    { get; }

    private NodeExit(
        string code,
        bool isError,
        string data)
    {
        if (string.IsNullOrEmpty(code))
            throw new ArgumentNullException(nameof(code));

        Code    = code;
        IsError = isError;
        Data    = data;
    }

    public static NodeExit Create(
        string code, 
        bool isError = false,
        string data = null)
    {
        return new NodeExit(code, isError, data);
    }
        
    public static NodeExit Create(
        Enumeration code, 
        bool isError = false,
        string data = null)
    {
        return new NodeExit(code?.Name, isError, data);
    }
}