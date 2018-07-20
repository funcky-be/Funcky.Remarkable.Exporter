// -----------------------------------------------------------------------
//  <copyright file="DrawNotes.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Remarkable.Exporter.Workers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Funcky.Remarkable.Exporter.Drawer;
    using Funcky.Remarkable.Exporter.Model;

    using NLog;

    public static class DrawNotes
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public static void Execute()
        {
            Logger.Info("Start Exporting to PNG");

            var config = Configuration.Read();

            if (config?.Devices == null)
            {
                Logger.Warn("No configuration found, an empty one is created");
                Configuration.CreateEmptyConfiguration();
                return;
            }

            foreach (var device in config.Devices)
            {
                Logger.Info($"Processing device {device.Name}");
                
                var baseDirectory = new DirectoryInfo(device.LocalPath);

                foreach (var file in baseDirectory.GetFiles("*.lines", SearchOption.AllDirectories))
                {
                    var renderedDirectory = Path.Combine(file.DirectoryName ?? throw new ArgumentNullException(nameof(file.DirectoryName)), "png");

                    if (Directory.Exists(renderedDirectory))
                    {
                        continue;
                    }

                    var templateFileName = file.FullName.Replace(".lines", ".pagedata");
                    var templates = new List<string>();

                    if (File.Exists(templateFileName))
                    {
                        templates.AddRange(File.ReadAllLines(templateFileName));
                    }

                    var parser = new LinesParser(File.ReadAllBytes(file.FullName), file.Name);
                    var pages = parser.Parse();

                    var drawer = new LinesDrawer(pages, templates);
                    var images = drawer.Draw();

                    Directory.CreateDirectory(renderedDirectory);

                    for (var image = 0; image < images.Count; image++)
                    {
                        var outputFile = Path.Combine(renderedDirectory, $"{image + 1:000}.png");
                        File.WriteAllBytes(outputFile, images[image]);
                    }
                }
            }

            Logger.Info("End Exporting to PNG");
        }
    }
}