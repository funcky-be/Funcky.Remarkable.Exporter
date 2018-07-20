// -----------------------------------------------------------------------
//  <copyright file="ExtractNotes.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Remarkable.Exporter.Workers
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;

    using Funcky.Remarkable.Exporter.Model;

    using NLog;

    public static class ExtractNotes
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public static void Execute()
        {
            Logger.Info("Start Extracting the zip files");

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

                foreach (var file in baseDirectory.GetFiles("*.zip", SearchOption.AllDirectories))
                {
                    var extractedPath = Path.Combine(file.DirectoryName ?? throw new ArgumentNullException(nameof(file.DirectoryName)), "content");
                    var unzipped = new DirectoryInfo(extractedPath);

                    if (unzipped.Exists)
                    {
                        continue;
                    }

                    Logger.Info($"Extracting {file.FullName}");
                    ZipFile.ExtractToDirectory(file.FullName, extractedPath);
                }
            }

            Logger.Info("End Extracting the zip files");
        }
    }
}