// -----------------------------------------------------------------------
//  <copyright file="RemarkableApi.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Remarkable.Exporter.Model
{
    using System.Net.Http;
    using System.Threading.Tasks;

    using Newtonsoft.Json.Linq;

    public class RemarkableApi
    {
        private static string storageService;

        public static string Authentication => "https://my.remarkable.com/token/device/new";

        public static string Root => "https://my.remarkable.com";

        public static string ServiceDiscovery =>
            "https://service-manager-production-dot-remarkable-production.appspot.com/service/json/1/document-storage?environment=production&group=auth0%7C5a68dc51cb30df3877a1d7c4&apiVer=2";

        public static async Task<string> GetStorageService()
        {
            if (string.IsNullOrWhiteSpace(storageService))
            {
                var client = new HttpClient();
                var response = await client.GetAsync(ServiceDiscovery);
                var content = await response.Content.ReadAsStringAsync();
                var parsedContent = JObject.Parse(content);

                storageService = "https://" + parsedContent["Host"];
            }

            return storageService;
        }
    }
}