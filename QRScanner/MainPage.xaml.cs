using QRScanner.BottomSheets;
using QRScanner.Services;
using QRScanner.Utils;
using ZXing.Net.Maui;
using ContentPage = Microsoft.Maui.Controls.ContentPage;

namespace QRScanner;

public partial class MainPage : ContentPage
{
    private readonly CabinetInfoService _cabinetInfoService;
    private readonly IServiceProvider _serviceProvider;

    private readonly DebugService Debug;
    public readonly List<ResultCabinet> History = [];
    private bool _devEnabled;

    private bool _isProcessing;

    public MainPage(DebugService debug, CabinetInfoService cabinetInfoService, IServiceProvider serviceProvider)
    {
        InitializeComponent();
        CameraBarcodeReaderView.Options = new BarcodeReaderOptions
        {
            Formats = BarcodeFormats.TwoDimensional,
            AutoRotate = true,
            Multiple = true
        };

        BottomSheetCollection = new BottomSheetCollection(this);
        Debug = debug;
        _cabinetInfoService = cabinetInfoService;
        _serviceProvider = serviceProvider;

        if (Dumper.TryReadDump(out var dumpReader))
        {
            Debug.Debug("____START READ DUMP____");
            while (dumpReader.ReadLine() is { } line) Debug.Debug(line);

            dumpReader.Dispose();
            Debug.Debug("____END READ DUMP____");
        }

        Debug.Debug("Application was started!");
    }

    public BottomSheetCollection BottomSheetCollection { get; }

    public bool DevEnabled
    {
        get => _devEnabled;
        set
        {
            DevButton.IsVisible = value;
            _devEnabled = value;
        }
    }

    private async void BarcodesDetected(object? sender, BarcodeDetectionEventArgs e)
    {
        var scanned = e.Results[0].Value;
        Debug.Debug("SCANNED: " + scanned);

        if (_isProcessing || BottomSheetCollection.IsSheetsShow ||
            !Uri.TryCreate(scanned, UriKind.Absolute, out var uri))
            return;

        Debug.Debug("RESOLVING: " + scanned);
        SetProcessing(true);

        var result = await _cabinetInfoService.Get(uri, CancellationToken.None);
        if (result is not null)
        {
            History.Add(result.Value);
            await BottomSheetCollection.ShowBottomSheet(new ResultBottomSheet(result.Value));
        }

        SetProcessing(false);
    }

    private void SetProcessing(bool process)
    {
        _isProcessing = process;
        MainThread.BeginInvokeOnMainThread(() =>
        {
            ProcessingImage.IsVisible = process;
            ProcessingImage.IsAnimationPlaying = process;
        });
    }

    private async void DebugButtonClicked(object? sender, EventArgs e)
    {
        await BottomSheetCollection.ShowBottomSheet(_serviceProvider.GetService<DebugBottomSheet>()!);
    }

    private async void MenuButtonClicked(object? sender, EventArgs e)
    {
        var menu = _serviceProvider.GetService<MenuBottomSheet>()!;

        await BottomSheetCollection.ShowBottomSheet(menu);
    }
}