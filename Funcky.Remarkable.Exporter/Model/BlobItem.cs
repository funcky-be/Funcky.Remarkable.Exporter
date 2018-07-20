// -----------------------------------------------------------------------
//  <copyright file="BlobItem.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Remarkable.Exporter.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    public class BlobItem
    {
        public string BlobURLGet { get; set; }

        public string BlobURLGetExpires { get; set; }

        public bool Bookmarked { get; set; }

        public int CurrentPage { get; set; }

        public string ID { get; set; }

        public string Message { get; set; }

        public string ModifiedClient { get; set; }

        public string Parent { get; set; }

        public bool Success { get; set; }

        public string Type { get; set; }

        public int Version { get; set; }

        public string VissibleName { get; set; }

        public async Task<BlobItem> GetWithBlobItem(DeviceRegistration device)
        {
            var client = new HttpClient();
            var serviceEndpoint = await RemarkableApi.GetStorageService() + "/document-storage/json/2/docs?withBlob=true&doc=" + this.ID;
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, serviceEndpoint);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", device.SessionBearer);
            var response = await client.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<BlobItem>>(content).Single();
        }
    }
}