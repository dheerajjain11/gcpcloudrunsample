﻿using Google.Cloud.SecretManager.V1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Google.Api.Gax.ResourceNames;
using Google.Protobuf;
using System.Text;

namespace GCPCloudRunSample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public void StoreSecret([FromForm]string projectId, [FromForm]string secretId)
        {
           
            //call SDK
            SecretManagerServiceClient client = SecretManagerServiceClient.Create();

            // Build the parent project name.
            ProjectName projectName = new ProjectName(projectId);

            // Build the secret to create.
            Secret secret = new Secret
            { 
                Replication = new Replication
                {
                    Automatic = new Replication.Types.Automatic(),
                },
            };

            Secret createdSecret = client.CreateSecret(projectName, secretId, secret);

            // Build a payload.
            SecretPayload payload = new SecretPayload
            {
                Data = ByteString.CopyFrom("my super secret data", Encoding.UTF8),
            };

            // Add a secret version.
            SecretVersion createdVersion = client.AddSecretVersion(createdSecret.SecretName, payload);

            // Access the secret version.
            AccessSecretVersionResponse result = client.AccessSecretVersion(createdVersion.SecretVersionName);

            // Print the resultszxcvbnj
            //
            // WARNING: Do not print secrets in production environments. This
            // snippet is for demonstration purposes only.
            string data = result.Payload.Data.ToStringUtf8();
            Console.WriteLine($"Plaintext: {data}");

        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            //insert some delay for simulating
            Thread.Sleep(2000);
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
