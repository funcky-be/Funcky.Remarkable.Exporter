// -----------------------------------------------------------------------
//  <copyright file="LinesDrawer.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Remarkable.Exporter.Drawer
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;

    using NLog;

    using SkiaSharp;

    public class LinesDrawer
    {
        private const int CanvanWidth = 1404;

        private const int CanvasHeight = 1872;
        
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly List<Page> pages;

        private readonly List<string> templates;

        private string TemplateRoot => ConfigurationManager.AppSettings["TemplateRoot"];

        public LinesDrawer(List<Page> pages, List<string> templates)
        {
            this.pages = pages;
            this.templates = templates;
        }

        public List<byte[]> Draw()
        {
            Logger.Info("Start drawing");

            var images = new List<byte[]>();

            var currentPage = 0;
            foreach (var page in this.pages)
            {
                var template = string.Empty;

                if (currentPage < this.templates.Count)
                {
                    template = this.templates[currentPage];
                }
                
                currentPage++;
                Logger.Debug($"Drawing page {currentPage} on  {this.pages.Count}");

                using (var bitmap = new SKBitmap(CanvanWidth, CanvasHeight))
                using (var canvas = new SKCanvas(bitmap))
                {
                    canvas.DrawRect(0, 0, CanvanWidth, CanvasHeight, new SKPaint { Color = new SKColor(255, 255, 255) });
                    
                    if (!string.IsNullOrWhiteSpace(template))
                    {
                        var templateBitmap = SKBitmap.Decode($"{this.TemplateRoot}{template}.png");
                        canvas.DrawBitmap(templateBitmap, 0, 0);
                    }

                    var currentLayer = 0;
                    foreach (var layer in page.Layers)
                    {
                        currentLayer++;
                        Logger.Debug($"Drawing layer {currentLayer} on  {page.Layers.Count}");

                        var currentStroke = 0;
                        foreach (var stroke in layer.Strokes)
                        {
                            currentStroke++;
                            Logger.Debug($"Drawing stroke {currentStroke} on  {layer.Strokes.Count}");

                            for (var currentSegment = 0; currentSegment < stroke.Segments.Count; currentSegment++)
                            {
                                Logger.Debug($"Drawing segment {currentSegment} on  {stroke.Segments.Count}");

                                if (currentSegment < stroke.Segments.Count - 1)
                                {
                                    this.DrawSegment(stroke.Segments[currentSegment], stroke.Segments[currentSegment + 1], canvas);
                                }
                            }
                        }
                    }

                    canvas.Flush();

                    var image = SKImage.FromBitmap(bitmap);
                    var data = image.Encode(SKEncodedImageFormat.Png, 100);

                    using (var stream = new MemoryStream())
                    {
                        data.SaveTo(stream);
                        images.Add(data.ToArray());
                    }
                }
            }

            return images;
        }

        private void DrawSegment(Segment start, Segment end, SKCanvas canvas)
        {
            var paint = this.GetPaint(start);
            if (paint == null)
            {
                canvas.DrawRect(0, 0, CanvanWidth, CanvasHeight, new SKPaint { Color = new SKColor(255, 255, 255) });
                return;
            }
            
            canvas.DrawLine(start.HorizontalPosition, start.VerticalPosition, end.HorizontalPosition, end.VerticalPosition, paint);
        }

        private SKPaint GetPaint(Segment segment)
        {
            if (segment == null)
            {
                throw new ArgumentNullException(nameof(segment));
            }
            
            // Get the base color
            var color = new SKColor(0, 0, 0);

            switch (segment.Stroke?.PenColor)
            {
                case 0:
                    color = new SKColor(0, 0, 0);
                    break;
                case 1:
                    color = new SKColor(69, 69, 69);
                    break;
                case 2:
                    color = new SKColor(255, 255, 255);
                    break;
            }

            var width = segment.Stroke?.PenWidth ?? 1;
            var opacity = 1f;

            // Manage the "simple" pen type
            switch (segment.Stroke?.PenType)
            {
                case 2:
                case 4:
                    width = 32 * width * width - 116 * width + 107;
                    break;
                case 3:
                    width = 64 * width - 112;
                    opacity = 0.9f;
                    break;
                case 5:
                    width = 30;
                    opacity = 0.2f;
                    break;
                case 6:
                    width = 1280 * width * width - 4800 * width + 4510;
                    color = new SKColor(255, 255, 255);
                    break;
                case 7:
                    width = 16 * width - 27;
                    opacity = 0.9f;
                    break;
                case 8:
                    // Empty the canvas
                    return null;
            }
            
            // Manage the pressure / tilt sensitive pens
            switch (segment.Stroke?.PenType)
            {
                    case 0:
                        width = (5 * segment.Tilt) * (6 * width - 10) * (1 + 2 * segment.Pressure * segment.Pressure * segment.Pressure);
                        break;
                    case 1:
                        width = (10 * segment.Tilt - 2) * (8 * width - 14);
                        opacity = (segment.Pressure - 0.2f) * (segment.Pressure - 0.2f);
                        break;
            }

            opacity = Math.Min(opacity, 1);
            opacity = Math.Max(opacity, 0);

            // Create the paint
            return new SKPaint
                       {
                           Color = color.WithAlpha(Convert.ToByte(255 * opacity)),
                           StrokeWidth = width
                       };
        }
    }
}