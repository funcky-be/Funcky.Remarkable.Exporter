// -----------------------------------------------------------------------
//  <copyright file="Configuration.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Remarkable.Exporter.Model
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    [DataContract(Name = "configuration")]
    public class Configuration
    {
        public static string ConfigurationPath => ConfigurationManager.AppSettings["DevicesConfiguration"];

        [DataMember(Name = "devices")]
        public List<DeviceRegistration> Devices { get; set; }
        
        [DataMember(Name = "smtp")]
        public SmtpConfiguration Smtp { get; set; } 

        public static void CreateEmptyConfiguration()
        {
            var config = new Configuration { Devices = new List<DeviceRegistration> { new DeviceRegistration() } };
            var newConfigurationContent = JsonConvert.SerializeObject(config);
            File.WriteAllText(ConfigurationPath, newConfigurationContent);
            Console.WriteLine($"A new config file has been created, please fill the informations in {ConfigurationPath}");
        }

        public static Configuration Read()
        {
            if (!File.Exists(ConfigurationPath))
            {
                return null;
            }

            var configurationContent = File.ReadAllText(ConfigurationPath);
            return JsonConvert.DeserializeObject<Configuration>(configurationContent);
        }

        public void Save()
        {
            var newConfigurationContent = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(ConfigurationPath, newConfigurationContent);
        }
    }
}