using The49.Maui.BottomSheet;

namespace QRScanner.Utils;

public class BottomSheetCollection
{
    private readonly Page _page;
    private readonly List<BottomSheet> _sheetQueue = [];
    private Window Window => _page.Window!;
    private BottomSheet? _currentSheet;
    private CancellationTokenSource? CancellationTokenSource;
    public bool IsSheetsShow => _currentSheet != null;
    public BottomSheetCollection(Page page)
    {
        _page = page;
    }

    public async Task ShowBottomSheet(BottomSheet bottomSheet)
    {
        _sheetQueue.Add(bottomSheet);
        await EnsureSheet();
    }

    public async Task DismissCurrentSheet()
    {  
        if(_currentSheet is null) return;
        await _currentSheet.DismissAsync();
    }

    private async Task EnsureSheet()
    {
        if (_sheetQueue.Count <= 0 || _currentSheet is not null) return;
        
        await SetSheet(_sheetQueue[0]);
        _sheetQueue.RemoveAt(0);
    }

    private async Task SetSheet(BottomSheet bottomSheet)
    {
        _currentSheet = bottomSheet;
        _currentSheet.HasHandle = true;
        _currentSheet.Dismissed += CurrentSheetDismissed;
        if (_currentSheet is ICancellationBehaviour cancellationBehaviour)
        {
            CancellationTokenSource = new CancellationTokenSource();
            cancellationBehaviour.CancellationToken = CancellationTokenSource.Token;
        }
        
        await MainThread.InvokeOnMainThreadAsync(async () =>
            await _currentSheet.ShowAsync(Window)
        );
    }

    private async void CurrentSheetDismissed(object? sender, DismissOrigin e)
    {
        if (CancellationTokenSource != null)
        {
            await CancellationTokenSource.CancelAsync();
            CancellationTokenSource.Dispose();
            CancellationTokenSource = null;
        }
        _currentSheet!.Dismissed -= CurrentSheetDismissed;
        _currentSheet = null;
        await EnsureSheet();
    }
}