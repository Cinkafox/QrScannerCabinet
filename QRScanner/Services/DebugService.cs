namespace QRScanner.Services;

public class DebugService
{
    public List<DebugMessage> Messages = new();

    public delegate void MessageAppend(DebugMessage debugMessage, DebugService service);

    public event MessageAppend? OnMessageAppend;

    public void AppendMessage(DebugMessage message)
    {
        MainThread.InvokeOnMainThreadAsync(() =>
        {
            Messages.Add(message);
            OnMessageAppend?.Invoke(message, this);
        });
    }

    public void Debug(string message)
    {
        AppendMessage(new DebugMessage(MessageStat.DEBG,message));
    }

    public void Error(string message)
    {
        AppendMessage(new DebugMessage(MessageStat.ERRO,message));
    }
}

public struct DebugMessage
{
    public MessageStat Stat;
    public string Message;

    public DebugMessage(MessageStat stat, string message)
    {
        Stat = stat;
        Message = message;
    }
}

public enum MessageStat
{
    INFO,ERRO,DEBG
}