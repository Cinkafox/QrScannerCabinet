using Android.Widget;

namespace QRScanner.Services;

public class DebugService
{
    public delegate void MessageAppend(DebugMessage debugMessage, DebugService service);

    public List<DebugMessage> Messages = new();

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
        AppendMessage(new DebugMessage(MessageStat.DEBG, message));
    }

    public void Error(string message)
    {
        AppendMessage(new DebugMessage(MessageStat.ERRO, message));
    }

    public void Toast(string message)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            var toast = Android.Widget.Toast.MakeText(Platform.AppContext,message,ToastLength.Long);
            toast?.Show();
        });
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
    INFO,
    ERRO,
    DEBG
}