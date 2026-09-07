using ZXing;
using ZXing.Common;
using ZXing.QrCode;

namespace BDBlio.Services;

public interface IBarcodeService
{
    string? DecodeFromImage(string imagePath);
    string? DecodeFromBytes(byte[] imageData);
    byte[] GenerateBarcode(string content, int width = 200, int height = 100);
}

public class BarcodeService : IBarcodeService
{
    public string? DecodeFromImage(string imagePath)
    {
        try
        {
            var bitmap = new System.Drawing.Bitmap(imagePath);
            var source = new BitmapLuminanceSource(bitmap);
            var result = new MultiFormatReader().Decode(source);
            return result?.Text;
        }
        catch
        {
            return null;
        }
    }

    public string? DecodeFromBytes(byte[] imageData)
    {
        try
        {
            using var ms = new MemoryStream(imageData);
            var bitmap = new System.Drawing.Bitmap(ms);
            var source = new BitmapLuminanceSource(bitmap);
            var result = new MultiFormatReader().Decode(source);
            return result?.Text;
        }
        catch
        {
            return null;
        }
    }

    public byte[] GenerateBarcode(string content, int width = 200, int height = 100)
    {
        try
        {
            var writer = new ZXing.QrCode.QRCodeWriter();
            var hints = new Dictionary<EncodeHintType, object>
            {
                { EncodeHintType.WIDTH, width },
                { EncodeHintType.HEIGHT, height }
            };

            var bitMatrix = writer.encode(content, BarcodeFormat.QR_CODE, width, height, hints);
            var bitmap = new ZXing.Rendering.BitmapRenderer().Render(bitMatrix, BarcodeFormat.QR_CODE, content);

            using var ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return ms.ToArray();
        }
        catch
        {
            return Array.Empty<byte>();
        }
    }
}
