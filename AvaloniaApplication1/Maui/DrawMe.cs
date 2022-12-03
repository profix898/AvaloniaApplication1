using Microsoft.Maui.Graphics;

namespace AvaloniaApplication1.Maui;

public class DrawMe : IDrawable
{
    public void Draw(ICanvas canvas, RectF rect)
    {
        canvas.StrokeColor = Colors.Red;
        canvas.StrokeSize = 4;
        canvas.DrawEllipse(rect.Width / 2 - 125, rect.Height / 2 - 125, 250, 250);
    }
}