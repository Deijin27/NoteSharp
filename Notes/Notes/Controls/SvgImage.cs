using System;
using System.IO;
using Notes.Pages;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using SKSvg = SkiaSharp.Extended.Svg.SKSvg;

namespace Notes.Controls
{
    public class SvgImage : Frame
    {
        #region Private Members

        private readonly SKCanvasView _canvasView = new SKCanvasView();

        #endregion

        #region Bindable Properties

        #region ResourceId

        public static readonly BindableProperty ResourceIdProperty = BindableProperty.Create(
            nameof(ResourceId), typeof(string), typeof(SvgImage), default(string), propertyChanged: RedrawCanvas);

        public string ResourceId
        {
            get => (string)GetValue(ResourceIdProperty);
            set => SetValue(ResourceIdProperty, value);
        }

        #endregion

        #endregion

        #region Constructor

        public SvgImage()
        {
            Padding = new Thickness(0);
            BackgroundColor = Color.Transparent;
            HasShadow = false;
            Content = _canvasView;
            _canvasView.PaintSurface += CanvasViewOnPaintSurface;
        }

        #endregion

        //#region Private Methods

        private static void RedrawCanvas(BindableObject bindable, object oldvalue, object newvalue)
        {
            SvgImage svgImage = bindable as SvgImage;
            svgImage?._canvasView.InvalidateSurface();
        }

        private void CanvasViewOnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            if (string.IsNullOrEmpty(ResourceId))
                return;

            var svg = new SKSvg();
            using (Stream stream = GetType().Assembly.GetManifestResourceStream(ResourceId))
            {
                if (stream == null)
                {
                    throw new Exception("File not found in assembly; check in the properties of the file to ensure that the Build Action is set to Embedded Resource.");
                }
                svg.Load(stream);
            }

            var surface = e.Surface;
            var canvas = surface.Canvas;

            var width = e.Info.Width;
            var height = e.Info.Height;

            // clear the surface
            canvas.Clear();

            // the page is not visible yet
            if (svg == null)
                return;

            // calculate the scaling need to fit to screen
            float canvasMin = Math.Min(width, height);
            float svgMax = Math.Max(svg.Picture.CullRect.Width, svg.Picture.CullRect.Height);
            float scale = canvasMin / svgMax;
            var matrix = SKMatrix.MakeScale(scale, scale);

            // draw the svg
            canvas.DrawPicture(svg.Picture, ref matrix);
        }
    }
}

