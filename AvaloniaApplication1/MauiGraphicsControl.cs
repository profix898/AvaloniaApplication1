using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using Avalonia.Threading;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Skia;
using SkiaSharp;
using Point = Avalonia.Point;
using Rect = Avalonia.Rect;

namespace AvaloniaApplication1;

public class MauiGraphicsControl : Control
{
    private IDrawable _drawable;

    public static readonly DirectProperty<MauiGraphicsControl, IDrawable> DrawableProperty =
        AvaloniaProperty.RegisterDirect<MauiGraphicsControl, IDrawable>(nameof(Drawable), o => o.Drawable, (o, v) => o.Drawable = v);

    public MauiGraphicsControl()
    {
        ClipToBounds = true;
    }

    public IDrawable Drawable
    {
        get => _drawable;
        set => SetAndRaise(DrawableProperty, ref _drawable, value);
    }

    public void Invalidate()
    {
        Dispatcher.UIThread.InvokeAsync(InvalidateVisual, DispatcherPriority.Background);
    }


    public override void Render(DrawingContext context)
    {
        context.Custom(new MauiGraphicsDrawOperation(_drawable, new Rect(0, 0, Bounds.Width, Bounds.Height)));

        Invalidate();
    }

    #region NestedType: MauiGraphicsDrawOperation

    private class MauiGraphicsDrawOperation : ICustomDrawOperation
    {
        private readonly IDrawable _drawable;
        private readonly SkiaCanvas _canvas;

        public MauiGraphicsDrawOperation(IDrawable drawable, Rect bounds)
        {
            _drawable = drawable;
            _canvas = new SkiaCanvas();

            Bounds = bounds;
        }

        public Rect Bounds { get; }

        public bool HitTest(Point p) => false;

        public bool Equals(ICustomDrawOperation other) => false;

        public void Render(IDrawingContextImpl context)
        {
            var skiaApiLeaseFeature = context.GetFeature<ISkiaSharpApiLeaseFeature>();
            if (skiaApiLeaseFeature == null)
            {
                using var fallbackContext = new DrawingContext(context, false);
                var noSkiaText = new FormattedText("Drawing context is not Skia.", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, Typeface.Default, 12, Brushes.DeepPink);
                fallbackContext.DrawText(noSkiaText, new Point(0, (float) Bounds.Height / 2));
            }
            else
            {
                using var skiaApiLease = skiaApiLeaseFeature.Lease();
                var skiaCanvas = skiaApiLease.SkCanvas;

                if (_drawable == null)
                    skiaCanvas.DrawText("No IDrawable given.", new SKPoint(0, (float) Bounds.Height / 2), new SKPaint(new SKFont(SKTypeface.Default)) { Color = SKColors.DeepPink });
                else
                {
                    _canvas.Canvas = skiaCanvas;
                    _drawable.Draw(_canvas, new RectF(0, 0, (float) Bounds.Width, (float) Bounds.Height));
                }
            }
        }

        public void Dispose()
        {
            _canvas.Dispose();
        }
    }

    #endregion
}