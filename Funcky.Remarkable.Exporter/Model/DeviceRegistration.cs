// -----------------------------------------------------------------------
//  <copyright file="DeviceRegistration.cs" company="Prism">
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
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    [DataContract]
    public class DeviceRegistration
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "authenticationBearer")]
        public string AuthenticationBearer { get; set; }

        [DataMember(Name = "code")]
        public string Code { get; set; }

        [DataMember(Name = "deviceDesc")]
        public string Description { get; set; }

        [DataMember(Name = "deviceID")]
        public string DeviceIdentifier { get; set; }

        [DataMember(Name = "evernoteDestinationEmail")]
        public string EvernoteDestinationEmail { get; set; }

        [DataMember(Name = "evernoteNotebook")]
        public string EvernoteNotebook { get; set; }

        [DataMember(Name = "evernoteSourceEmail")]
        public string EvernoteSourceEmail { get; set; }

        [DataMember(Name = "localPath")]
        public string LocalPath { get; set; }

        [DataMember(Name = "sessionBearer")]
        public string SessionBearer { get; set; }

        public async Task GenerateAuthenticationBearer()
        {
            var client = new HttpClient();

            var response = await client.PostAsJsonAsync(RemarkableApi.Authentication, this);
            this.AuthenticationBearer = await response.Content.ReadAsStringAsync();
        }

        public async Task GenerateSessionBearer()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            var refreshEndpoint = "https://my.remarkable.com/token/json/2/user/new";
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, refreshEndpoint);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.AuthenticationBearer);
            requestMessage.Headers.UserAgent.ParseAdd("Mozilla/5.0");
            var response = await client.SendAsync(requestMessage);
            this.SessionBearer = await response.Content.ReadAsStringAsync();
        }

        public async Task<List<BlobItem>> GetFiles()
        {
            var client = new HttpClient();
            var serviceEndpoint = await RemarkableApi.GetStorageService() + "/document-storage/json/2/docs";
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, serviceEndpoint);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.SessionBearer);
            var response = await client.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<BlobItem>>(content);
        }
    }
}