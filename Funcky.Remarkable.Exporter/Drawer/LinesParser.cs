// -----------------------------------------------------------------------
//  <copyright file="LinesDrawer.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Remarkable.Exporter.Drawer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using NLog;

    public class LinesParser
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly string fileName;

        private readonly byte[] content;

        public LinesParser(byte[] content, string fileName)
        {
            this.content = content;
            this.fileName = fileName;
        }

        public List<Page> Parse()
        {
            Logger.Info($"Start Parsing {this.fileName}");

            var workingData = new Queue<byte>(this.content);

            this.Skip(workingData, 43);

            var pages = new List<Page>();

            var pageCount = this.GetInteger(workingData);
            Logger.Debug($"Pages : {pageCount}");

            for (var currentPage = 1; currentPage <= pageCount; currentPage++)
            {
                Logger.Debug($"Current Page : {currentPage}");

                var page = new Page();

                var layersCount = this.GetInteger(workingData);
                Logger.Debug($"Layers : {layersCount}");

                for (var currentLayer = 1; currentLayer <= layersCount; currentLayer++)
                {
                    Logger.Debug($"Current Layer : {currentLayer}");

                    var layer = new Layer();

                    var strokesCount = this.GetInteger(workingData);
                    Logger.Debug($"Strokes : {strokesCount}");

                    for (var currentStroke = 1; currentStroke <= strokesCount; currentStroke++)
                    {
                        Logger.Debug($"Current Segment : {currentStroke}");

                        var stroke = new Stroke();

                        stroke.PenType = this.GetInteger(workingData);
                        stroke.PenColor = this.GetInteger(workingData);
                        stroke.Padding = this.GetInteger(workingData);
                        stroke.PenWidth = this.GetFloat(workingData);

                        var segmentCounts = this.GetInteger(workingData);
                        Logger.Debug($"Segments : {segmentCounts}");

                        for (var currentSegment = 1; currentSegment <= segmentCounts; currentSegment++)
                        {
                            Logger.Debug($"Current Segment : {currentSegment}");

                            var segment = new Segment();
                            segment.Stroke = stroke;

                            segment.HorizontalPosition = this.GetFloat(workingData);
                            segment.VerticalPosition = this.GetFloat(workingData);
                            segment.Tilt = this.GetFloat(workingData);
                            segment.Pressure = this.GetFloat(workingData);
                            segment.Speed = this.GetFloat(workingData);

                            stroke.Segments.Add(segment);
                        }

                        layer.Strokes.Add(stroke);
                    }

                    page.Layers.Add(layer);
                }

                pages.Add(page);
            }

            return pages;
        }

        private float GetFloat(Queue<byte> workingData)
        {
            var data = this.Read(workingData, 4);
            return BitConverter.ToSingle(data, 0);
        }

        private int GetInteger(Queue<byte> workingData)
        {
            var data = this.Read(workingData, 4);
            return BitConverter.ToInt32(data, 0);
        }

        private byte[] Read(Queue<byte> workingData, int amount)
        {
            var data = new byte[amount];

            for (var i = 0; i < amount; i++)
            {
                data[i] = workingData.Dequeue();
            }

            return data;
        }

        private void Skip(Queue<byte> workingData, int amount)
        {
            for (var i = 0; i < amount; i++)
            {
                workingData.Dequeue();
            }
        }
    }
}