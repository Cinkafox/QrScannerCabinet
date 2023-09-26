using Capture.Vision.Maui;
using Dynamsoft;
using QRShared;
using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace QRScanner;
public partial class MainPage : ContentPage
{
    private BarcodeQrData[]? _data;
    private int _imageWidth;
    private int _imageHeight;

    private ResultBottomSheet? _currentBottomSheet = null;

    public MainPage()
    {
        InitializeComponent();
        InitService();
    }
    
    
    private async void InitService()
    {
        await Task.Run(() =>
        {
            BarcodeQRCodeReader.InitLicense("DLS2eyJoYW5kc2hha2VDb2RlIjoiMjAwMDAxLTE2NDk4Mjk3OTI2MzUiLCJvcmdhbml6YXRpb25JRCI6IjIwMDAwMSIsInNlc3Npb25QYXNzd29yZCI6IndTcGR6Vm05WDJrcEQ5YUoifQ==");
            return Task.CompletedTask;
        });
    }
    

    private void cameraView_ResultReady(object sender, ResultReadyEventArgs e)
    {
        if (e.Result != null)
        {
            BarcodeQRCodeReader.Result[] results = (BarcodeQRCodeReader.Result[])e.Result;
            ShowBottomSheet(new RoomInformation
            {
                Name = results[0].Text!
            });
            
            _data = BarcodeQrData.Convert((BarcodeQRCodeReader.Result[])e.Result);
        }
        _imageWidth = e.PreviewWidth;
        _imageHeight = e.PreviewHeight;

        MainThread.BeginInvokeOnMainThread(() =>
        {
            CanvasView.InvalidateSurface();
        });
    }

    private static SKPoint RotateCw90(SKPoint point, int width)
    {
        SKPoint rotatedPoint = new SKPoint();
        rotatedPoint.X = width - point.Y;
        rotatedPoint.Y = point.X;
        return rotatedPoint;
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

    void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
    {
        double width = CanvasView.Width;
        double height = CanvasView.Height;

        var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
        var orientation = mainDisplayInfo.Orientation;
        var density = mainDisplayInfo.Density;

        width *= density;
        height *= density;

        double scale, widthScale, heightScale, scaledWidth, scaledHeight;

        if (orientation == DisplayOrientation.Portrait)
        {
            widthScale = _imageHeight / width;
            heightScale = _imageWidth / height;
            scale = widthScale < heightScale ? widthScale : heightScale;
            scaledWidth = _imageHeight / scale;
            scaledHeight = _imageWidth / scale;
        }
        else
        {
            widthScale = _imageWidth / width;
            heightScale = _imageHeight / height;
            scale = widthScale < heightScale ? widthScale : heightScale;
            scaledWidth = _imageWidth / scale;
            scaledHeight = _imageHeight / scale;
        }
        
        SKSurface surface = args.Surface;
        SKCanvas canvas = surface.Canvas;

        canvas.Clear();

        SKPaint skPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Blue,
            StrokeWidth = 10,
        };

        SKPaint textPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Red,
            TextSize = (float)(18 * density),
            StrokeWidth = 4,
        };

        if (_data != null)
        {
            foreach (BarcodeQrData barcodeQrData in _data)
            {

                for (int i = 0; i < 4; i++)
                {
                    if (orientation == DisplayOrientation.Portrait)
                    {
                        barcodeQrData.points[i] = RotateCw90(barcodeQrData.points[i], _imageHeight);
                    }

                    if (widthScale < heightScale)
                    {
                        barcodeQrData.points[i].X = (float)(barcodeQrData.points[i].X / scale);
                        barcodeQrData.points[i].Y = (float)(barcodeQrData.points[i].Y / scale - (scaledHeight - height) / 2);
                    }
                    else
                    {
                        barcodeQrData.points[i].X = (float)(barcodeQrData.points[i].X / scale - (scaledWidth - width) / 2);
                        barcodeQrData.points[i].Y = (float)(barcodeQrData.points[i].Y / scale);
                    }
                }

                canvas.DrawText(barcodeQrData.text, barcodeQrData.points[0], textPaint);
                canvas.DrawLine(barcodeQrData.points[0], barcodeQrData.points[1], skPaint);
                canvas.DrawLine(barcodeQrData.points[1], barcodeQrData.points[2], skPaint);
                canvas.DrawLine(barcodeQrData.points[2], barcodeQrData.points[3], skPaint);
                canvas.DrawLine(barcodeQrData.points[3], barcodeQrData.points[0], skPaint);
            }
        }

    }
}