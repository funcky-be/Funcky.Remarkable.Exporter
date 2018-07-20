// -----------------------------------------------------------------------
//  <copyright file="SaveToEvernote.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Remarkable.Exporter.Workers
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net.Mail;
    using System.Threading;

    using Funcky.Remarkable.Exporter.Model;

    using Newtonsoft.Json.Linq;

    using NLog;

    public static class SaveToEvernote
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public static void Execute()
        {
            Logger.Info("Start Exporting to Evernote");

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

                // Check if Evernote is enabled
                if (!IsEnabled(device))
                {
                    Logger.Info($"Skipping step for device {device.Name} because it's not enabled");
                    continue;
                }

                // Check if smtp is present
                if (config.Smtp == null)
                {
                    Logger.Error("Cannot continue with this processor because no smtp info are configured");
                    continue;
                }

                // Build mail and process send, with a delay    
                var baseDirectory = new DirectoryInfo(device.LocalPath);

                foreach (var file in baseDirectory.GetFiles("content.json", SearchOption.AllDirectories))
                {
                    var evernoteflag = file.FullName.Replace(".json", ".evernote");

                    if (File.Exists(evernoteflag))
                    {
                        continue;
                    }

                    var content = JObject.Parse(File.ReadAllText(file.FullName));

                    var mail = new MailMessage(device.EvernoteSourceEmail, device.EvernoteDestinationEmail);
                    mail.Subject = content["VissibleName"] + " @" + device.EvernoteNotebook;
                    mail.Body = $"Note synchronisée depuis Remarkable le {DateTime.Now:dd/MM/yyyy HH:mm:ss}";

                    var renderedDirectory = Path.Combine(file.DirectoryName ?? throw new ArgumentNullException(nameof(file.DirectoryName)), "content", "png");

                    if (Directory.Exists(renderedDirectory))
                    {
                        foreach (var page in Directory.GetFiles(renderedDirectory))
                        {
                            var attachement = new Attachment(page);
                            mail.Attachments.Add(attachement);
                        }
                    }

                    Logger.Info($"Sending mail : {mail.Subject}");

                    config.Smtp.GetSmtpClient().Send(mail);

                    File.WriteAllText(evernoteflag, DateTime.Now.ToLongDateString());

                    Thread.Sleep(config.Smtp.Delay * 1000);
                }
            }

            Logger.Info("End Exporting to Evernote");
        }

        private static bool IsEnabled(DeviceRegistration device)
        {
            if (device == null)
            {
                return false;
            }

            return !string.IsNullOrWhiteSpace(device.EvernoteDestinationEmail)
                   && !string.IsNullOrWhiteSpace(device.EvernoteNotebook)
                   && !string.IsNullOrWhiteSpace(device.EvernoteSourceEmail);
        }
    }
}