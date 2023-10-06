using QRScanner.Services;
using QRShared;
using ZXing.Net.Maui;
using ContentPage = Microsoft.Maui.Controls.ContentPage;

namespace QRScanner;
public partial class MainPage : ContentPage
{
    private ResultBottomSheet? _currentBottomSheet = null;
    private RestService _rest;

    public MainPage(RestService service)
    {
        InitializeComponent();
        CameraBarcodeReaderView.Options = new BarcodeReaderOptions
        {
            Formats = BarcodeFormats.TwoDimensional,
            AutoRotate = true,
            Multiple = true,
            //TryHarder = true
        };
        _rest = service;
    }
    private void ShowBottomSheet(RoomInformation information)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (_currentBottomSheet == null)
                {
                    _currentBottomSheet = new ResultBottomSheet(information)
                    {
                        HasHandle = true
                    };
                    _currentBottomSheet.Dismissed += (sender, args) => _currentBottomSheet = null;
                    
                    await _currentBottomSheet.ShowAsync(Window);
                }
            });
    }
    

    private async void BarcodesDetected(object? sender, BarcodeDetectionEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Result.Text = e.Results[0].Value;
        });

        var url = new Uri(e.Results[0].Value);
        var information = await _rest.GetRoomInformationAsync(url);
        
        ShowBottomSheet(information);

    }
}