using QRScanner.Services;
using QRShared;
using ZXing.Net.Maui;
using ContentPage = Microsoft.Maui.Controls.ContentPage;

namespace QRScanner;
public partial class MainPage : ContentPage
{
    private ResultBottomSheet? _currentBottomSheet;
    private RestService _rest;

    public MainPage(RestService service)
    {
        InitializeComponent();
        CameraBarcodeReaderView.Options = new BarcodeReaderOptions
        {
            Formats = BarcodeFormats.TwoDimensional,
            AutoRotate = true,
            Multiple = true
        };
        _rest = service;
    }
    private void ShowBottomSheet(RoomInformation information,List<RoomImageInformation> imageInformations)
    {
        async void Action()
        {
            if (_currentBottomSheet == null)
            {
                _currentBottomSheet = new ResultBottomSheet(information, imageInformations) { HasHandle = true };
                _currentBottomSheet.Dismissed += (_, _) => _currentBottomSheet = null;

                await _currentBottomSheet.ShowAsync(Window);
            }
        }

        MainThread.BeginInvokeOnMainThread(Action);
    }
    

    private async void BarcodesDetected(object? sender, BarcodeDetectionEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Result.Text = e.Results[0].Value;
        });
        
        if(_currentBottomSheet != null) return;
        
        var infoUrl = new Uri(e.Results[0].Value);
        var imageUrl = new UriBuilder(infoUrl.Scheme, infoUrl.Host, infoUrl.Port,
            $"RoomImageInformation/{infoUrl.Segments[^1]}").Uri;
        
        var information = await _rest.GetInformationAsync<RoomInformation>(infoUrl);
        var imageInformation = await _rest.GetInformationAsync<RoomImageInformation>(imageUrl);
        
        if (information.Count == 0)
            information.Add(new RoomInformation()
            {
                Id = -1,
                Name = "Ошибка",
                Description = "Кабинет не найден!"
            });
        
        ShowBottomSheet(information[0],imageInformation);
    }
}