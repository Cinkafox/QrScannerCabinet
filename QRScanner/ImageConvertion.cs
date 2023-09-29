using SkiaSharp;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;

namespace QRScanner;
public static class ImageConvertion
{
    public static async Task<byte[]> ConvertImageSourceToBytesAsync(ImageSource imageSource)
    {
        Stream stream = await ((StreamImageSource)imageSource).Stream(CancellationToken.None);
        byte[] bytesAvailable = new byte[stream.Length];
        stream.Read(bytesAvailable, 0, bytesAvailable.Length);

        return bytesAvailable;
    }
    
    public static string Decode(Stream stream)
    {
        using var sKManagedStream = new SKManagedStream(stream, true);
        using var sKBitmap = SKBitmap.Decode(sKManagedStream) ?? throw new("ВААЙ БЛЯЯ!");
        var w = sKBitmap.Width;
        var h = sKBitmap.Height;
        var ps = w * h;
        var bytes = new byte[ps * 3];
        var byteIndex = 0;
        for (var x = 0; x < w; x++)
        {
            for (var y = 0; y < h; y++)
            {
                var color = sKBitmap.GetPixel(x, y);
                bytes[byteIndex + 0] = color.Red;
                bytes[byteIndex + 1] = color.Green;
                bytes[byteIndex + 2] = color.Blue;
                byteIndex += 3;
            }
        }
        var rGbLuminanceSource = new RGBLuminanceSource(bytes, w, h);
        var hybrid = new HybridBinarizer(rGbLuminanceSource);
        var binaryBitmap = new BinaryBitmap(hybrid);
        var hints = new Dictionary<DecodeHintType, object>
        {
            { DecodeHintType.CHARACTER_SET, "utf-8" }
        };
        var qRCodeReader = new QRCodeReader();
        var result = qRCodeReader.decode(binaryBitmap, hints);
        return result is not null ? result.Text : "";
    }
}