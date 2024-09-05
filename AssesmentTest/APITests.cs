using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;

namespace ApiTests
{
    public class ApiClientTests
    {
        private HttpClient httpClient;
        private string baseUrl;
        private string endpoint;
        private string contentType;
        private string testName;
        private string testEmail;
        private string testBalance;

        [SetUp]
        public void Setup()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("AppConfig.json");

            var configuration = builder.Build();

            baseUrl = configuration["APITestSettings:baseUrl"];
            endpoint = configuration["APITestSettings:endpoint"];
            contentType = configuration["APITestSettings:headers:content-type"];
            testName = configuration["APITestSettings:testRequestBody:name"];
            testEmail = configuration["APITestSettings:testRequestBody:email"];
            testBalance = configuration["APITestSettings:testRequestBody:balance"];

            Assert.IsNotNull(baseUrl);
            Assert.IsNotNull(endpoint);
            Assert.IsNotNull(contentType);
            Assert.IsNotNull(testName);
            Assert.IsNotNull(testEmail);
            Assert.IsNotNull(testBalance);

            httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
        }

        [Test]
        public async Task APIResponseTest()
        {
            var client = new RestClient(baseUrl);
            var request = new RestRequest(endpoint, Method.Post);
            request.AddHeader("Content-Type", contentType);

            var requestBody = new
            {
                name = testName,
                email = testEmail,
                balance = int.TryParse(testBalance, out int x) ? x : 0
            };

            request.AddJsonBody(requestBody);

            var response = await client.ExecuteAsync(request);

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Content);

            Assert.That((int)response.StatusCode, Is.EqualTo(200), "Status code is not 200");

            var responseBody = JsonConvert.DeserializeObject<dynamic>(response.Content);

            Assert.IsNotNull(responseBody);

            Assert.That((string)responseBody.name, Is.EqualTo(requestBody.name));
            Assert.That((string)responseBody.email, Is.EqualTo(requestBody.email));
            Assert.That((int)responseBody.balance, Is.EqualTo(requestBody.balance));
        }

        [TearDown]
        public void TearDown()
        {
            httpClient.Dispose();
        }
    }
}
