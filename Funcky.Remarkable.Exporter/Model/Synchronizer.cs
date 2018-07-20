// -----------------------------------------------------------------------
//  <copyright file="Synchronizer.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Remarkable.Exporter.Model
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    using NLog;

    public class Synchronizer
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        
        public async Task Synchronize(DeviceRegistration deviceRegistration, List<BlobItem> items)
        {
            foreach (var item in items)
            {
                await this.Synchronize(deviceRegistration, item);
            }
        }

        public async Task Synchronize(DeviceRegistration deviceRegistration, BlobItem item)
        {
            this.logger.Debug($"Sync {item.ID} ({item.VissibleName})");
            
            var baseDirectory = deviceRegistration.LocalPath;

            for (var i = 0; i < 5; i++)
            {
                baseDirectory = Path.Combine(baseDirectory, item.ID[i].ToString());
            }

            baseDirectory = Path.Combine(baseDirectory, item.Version.ToString("0000"));
            var destination = Path.Combine(baseDirectory, "content.zip");
            var destinationBlob = Path.Combine(baseDirectory, "content.json");

            if (!File.Exists(destination))
            {
                this.logger.Info($"Downloading {item.ID} #{item.Version} ({item.VissibleName})");
                
                var descriptorContent = JsonConvert.SerializeObject(item, Formatting.Indented);
                
                item = await item.GetWithBlobItem(deviceRegistration);
                var data = await new WebClient().DownloadDataTaskAsync(item.BlobURLGet);

                Directory.CreateDirectory(baseDirectory);
                File.WriteAllBytes(destination, data);

                File.WriteAllText(destinationBlob, descriptorContent);
            }
        }
    }
}