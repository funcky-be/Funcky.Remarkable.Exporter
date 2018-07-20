// -----------------------------------------------------------------------
//  <copyright file="SmtpConfiguration.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Remarkable.Exporter.Model
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Mail;
    using System.Runtime.Serialization;

    /// <summary>
    /// This represent the smtp configuration used for this project
    /// </summary>
    [DataContract]
    public class SmtpConfiguration
    {
        [DataMember(Name = "enableSsl")]
        public bool EnableSsl { get; set; }

        [DataMember(Name = "host")]
        public string Host { get; set; }

        [DataMember(Name = "password")]
        public string Password { get; set; }

        [DataMember(Name = "port")]
        public int Port { get; set; }

        [DataMember(Name = "userName")]
        public string UserName { get; set; }
        
        [DataMember(Name = "delay")]
        public int Delay { get; set; }

        /// <summary>
        /// Gets the smtp client with the configuration from this instance
        /// </summary>
        /// <returns>A configured SmtpClient</returns>
        public SmtpClient GetSmtpClient()
        {
            var smtpClient = new SmtpClient(this.Host);

            if (this.Port != 0)
            {
                smtpClient.Port = this.Port;
            }

            smtpClient.EnableSsl = this.EnableSsl;

            if (!string.IsNullOrWhiteSpace(this.UserName) && !string.IsNullOrWhiteSpace(this.Password))
            {
                smtpClient.Credentials = new NetworkCredential(this.UserName, this.Password);
            }

            return smtpClient;
        }
    }
}