using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using SkiaSharp;
using System.IO;

namespace BeanScene.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
{
    [HttpGet("drawmytext")]
    public IActionResult DrawMyText([FromQuery] string text)
    {
        const int width = 400;
        const int height = 100;

        // 1. Create a bitmap canvas surface
        using var bitmap = new SKBitmap(width, height);
        using var canvas = new SKCanvas(bitmap);

        // 2. Clear the canvas with a background color
        canvas.Clear(SKColors.LightYellow);

        using var typeface = SKTypeface.FromFamilyName("Arial",
               SKFontStyle.BoldItalic);
        using var font = new SKFont(typeface, 24);
        // 3. Define styling using SKPaint 
        //(acting as our brush, color, and font style)
        using var paint = new SKPaint
        {
            Color = SKColors.DarkBlue,
            IsAntialias = true
        };

        // 4. Draw the text string at coordinates (X, Y)
        canvas.DrawText(text, 10, 50, SKTextAlign.Left, font, paint);

        // 5. Encode the canvas bitmap into a standard PNG byte stream
        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        using var ms = new MemoryStream();

        data.SaveTo(ms);
        return File(ms.ToArray(), "image/png");
    }
}
}
