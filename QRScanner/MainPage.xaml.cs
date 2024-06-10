using QRScanner.BottomSheets;
using QRScanner.Services;
using QRScanner.Utils;
using QRShared.Datum;
using The49.Maui.BottomSheet;
using ZXing.Net.Maui;
using ContentPage = Microsoft.Maui.Controls.ContentPage;

namespace QRScanner;
public partial class MainPage : ContentPage
{
    public readonly BottomSheetCollection BottomSheetCollection;
    public readonly RestService Rest;
    public readonly DebugService Debug;
    public readonly AuthService Auth;
    
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

        BottomSheetCollection = new BottomSheetCollection(this);
        Rest = service;
        Debug = debug;
        Auth = auth;
        
        if (Dumper.TryReadDump(out var dumpReader))
        {
            Debug.Debug("____START READ DUMP____");
            while (dumpReader.ReadLine() is { } line)
            {
                Debug.Debug(line);
            }
            
            dumpReader.Dispose();
            Debug.Debug("____END READ DUMP____");
        }
        
        Debug.Debug("Application was started!");
    }

    private async void BarcodesDetected(object? sender, BarcodeDetectionEventArgs e)
    {
        var scanned = e.Results[0].Value;
        Debug.Debug("SCANNED: " + scanned);
        
        if(_isProcessing || BottomSheetCollection.IsSheetsShow ||
           !Uri.TryCreate(scanned, UriKind.Absolute,out var uri) || 
           !Uri.TryCreate(scanned + "/Images",UriKind.Absolute, out var imageUri))
            return;
        
        Debug.Debug("RESOLVING: " + scanned);
        
        _isProcessing = true;
        MainThread.BeginInvokeOnMainThread(() =>
        {
            ProcessingImage.IsVisible = true;
            ProcessingImage.IsAnimationPlaying = true;
        });

        var information = await Rest.GetAsyncDefault(uri,new RoomInformation()
        {
            Id = -1,
            Name = "Ошибка",
            Description = "Кабинет не найден!"
        },CancellationToken.None);
        
        var imageInformation = await Rest.GetAsyncDefault<List<RoomImageInformation>>(imageUri,[],CancellationToken.None);
        var result = new ResultCabinet(information, imageInformation);
        
        if(information.Id != -1)
            History.Add(result);
        
        await BottomSheetCollection.ShowBottomSheet(new ResultBottomSheet(result));
        
        _isProcessing = false;
        MainThread.BeginInvokeOnMainThread(() =>
        {
            ProcessingImage.IsVisible = false;
            ProcessingImage.IsAnimationPlaying = false;
        });
    }

    private async void DebugButtonClicked(object? sender, EventArgs e)
    {
        await BottomSheetCollection.ShowBottomSheet(new DebugBottomSheet(Debug));
    }

    private async void MenuButtonClicked(object? sender, EventArgs e)
    {
        await BottomSheetCollection.ShowBottomSheet(new MenuBottomSheet(this));
    }
}
