using QRScanner.BottomSheets;
using QRScanner.Services;
using QRShared;
using The49.Maui.BottomSheet;
using ZXing.Net.Maui;
using ContentPage = Microsoft.Maui.Controls.ContentPage;

namespace QRScanner;
public partial class MainPage : ContentPage
{
    private readonly BottomSheetCollection _bottomSheetCollection;
    private readonly RestService _rest;
    public  readonly DebugService _debug;
    private readonly AuthService _auth;
    
    private bool _isProcessing ;
    private bool _devEnabled;

    public readonly List<ResultCabinet> History = new();
    public bool DevEnabled
    {
        get => _devEnabled;
        set
        {
            DevButton.IsVisible = value;
            _devEnabled = value;
        }
    }

    public MainPage(RestService service,DebugService debug,AuthService auth)
    {
        InitializeComponent();
        CameraBarcodeReaderView.Options = new BarcodeReaderOptions
        {
            Formats = BarcodeFormats.TwoDimensional,
            AutoRotate = true,
            Multiple = true
        };

        _bottomSheetCollection = new BottomSheetCollection(this);
        _rest = service;
        _debug = debug;
        _auth = auth;
        
        if (Dumper.TryReadDump(out var dumpReader))
        {
            _debug.Debug("____START READ DUMP____");
            while (dumpReader.ReadLine() is { } line)
            {
                _debug.Debug(line);
            }
            
            dumpReader.Dispose();
            _debug.Debug("____END READ DUMP____");
        }
        
        _debug.Debug("Application was started!");
    }

    private async void BarcodesDetected(object? sender, BarcodeDetectionEventArgs e)
    {
        var scanned = e.Results[0].Value;
        _debug.Debug("SCANNED: " + scanned);
        
        if(_isProcessing || _bottomSheetCollection.IsSheetsShow ||
           !Uri.TryCreate(scanned, UriKind.Absolute,out var uri) || 
           !Uri.TryCreate(scanned + "/Images",UriKind.Absolute, out var imageUri))
            return;
        
        _debug.Debug("RESOLVING: " + scanned);
        
        _isProcessing = true;
        MainThread.BeginInvokeOnMainThread(() =>
        {
            ProcessingImage.IsVisible = true;
            ProcessingImage.IsAnimationPlaying = true;
        });

        var information = await _rest.GetAsyncDefault(uri,new RoomInformation()
        {
            Id = -1,
            Name = "Ошибка",
            Description = "Кабинет не найден!"
        });
        
        var imageInformation = await _rest.GetAsyncDefault<List<RoomImageInformation>>(imageUri,[]);
        var result = new ResultCabinet(information, imageInformation);
        
        if(information.Id != -1)
            History.Add(result);
        
        await _bottomSheetCollection.ShowBottomSheet(new ResultBottomSheet(result));
        
        _isProcessing = false;
        MainThread.BeginInvokeOnMainThread(() =>
        {
            ProcessingImage.IsVisible = false;
            ProcessingImage.IsAnimationPlaying = false;
        });
    }

    private async void DebugButtonClicked(object? sender, EventArgs e)
    {
        await _bottomSheetCollection.ShowBottomSheet(new DebugBottomSheet(_debug));
    }

    private async void MenuButtonClicked(object? sender, EventArgs e)
    {
        await _bottomSheetCollection.ShowBottomSheet(new MenuBottomSheet(this));
    }
}

public class BottomSheetCollection
{
    private readonly Page _page;
    private readonly List<BottomSheet> _sheetQueue = [];
    private Window Window => _page.Window!;
    private BottomSheet? _currentSheet = null;
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
        
        await MainThread.InvokeOnMainThreadAsync(async () =>
            await _currentSheet.ShowAsync(Window)
        );
    }

    private async void CurrentSheetDismissed(object? sender, DismissOrigin e)
    {
        _currentSheet!.Dismissed -= CurrentSheetDismissed;
        _currentSheet = null;
        await EnsureSheet();
    }
}