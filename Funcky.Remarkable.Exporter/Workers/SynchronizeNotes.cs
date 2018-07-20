// -----------------------------------------------------------------------
//  <copyright file="Synchronizer.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Remarkable.Exporter.Workers
{
    using System;
    using System.Threading.Tasks;

    using Funcky.Remarkable.Exporter.Model;

    using NLog;

    public static class SynchronizeNotes
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        
        public static async Task Execute()
        {
            Logger.Info("Start Synchronization");

            var config = Configuration.Read();

            if (config?.Devices == null)
            {
                Logger.Warn("No configuration found, an empty one is created");
                Configuration.CreateEmptyConfiguration();
                return;
            }

            foreach (var deviceRegistration in config.Devices)
            {
                Logger.Info($"Processing device {deviceRegistration.Name}");

                if (string.IsNullOrWhiteSpace(deviceRegistration.AuthenticationBearer))
                {
                    if (string.IsNullOrWhiteSpace(deviceRegistration.Code))
                    {
                        throw new ApplicationException("You cannot request a bearer without a code");
                    }

                    await deviceRegistration.GenerateAuthenticationBearer();

                    config.Save();
                }

                await deviceRegistration.GenerateSessionBearer();
                config.Save();

                var files = await deviceRegistration.GetFiles();
                Logger.Info($"Synchronizing {files.Count} documents");

                await new Synchronizer().Synchronize(deviceRegistration, files);
            }

            Logger.Info("End Synchronization");
        }
    }
}