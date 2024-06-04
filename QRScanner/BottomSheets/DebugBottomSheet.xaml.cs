using QRScanner.Services;
using QRScanner.Views;
using The49.Maui.BottomSheet;

namespace QRScanner.BottomSheets;

public partial class DebugBottomSheet : BottomSheet
{
    private readonly DebugService _debugService;

    private int _messageCount;

    public DebugBottomSheet(DebugService debugService)
    {
        _debugService = debugService;
        InitializeComponent();
        
        foreach (var message in debugService.Messages)
        {
            AddMessage(message);
        }
        
        Dismissed += OnDismissed;
        debugService.OnMessageAppend += DebugServiceOnOnMessageAppend;
    }

    private void DebugServiceOnOnMessageAppend(DebugMessage message, DebugService service)
    {
        AddMessage(message);
    }

    private void AddMessage(DebugMessage message)
    {
        var messageView = new DebugMessageView
        {
            Message = message,
            BackgroundColor = _messageCount % 2 == 0 ? Colors.DarkSlateGray : Colors.DimGray
        };
        
        Messages.Add(messageView);

        _messageCount++;
    }

    private void OnDismissed(object? sender, DismissOrigin e)
    {
        _debugService.OnMessageAppend -= DebugServiceOnOnMessageAppend;
        Dismissed -= OnDismissed;
    }
}