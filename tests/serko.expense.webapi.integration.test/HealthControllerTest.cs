using System;
using System.IO;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace serko.expense.webapi.integration.test
{
    [TestClass]
    public class HealthControllerTest
    {
        string _baseUrl = "http://localhost.com";
        private HttpClient _client;
        TestServer _server;

        [TestInitialize()]
        public void TestInit()
        {
            _server = new TestServer(
                new WebHostBuilder()
                    .UseEnvironment("Development")
                    .UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        [TestCleanup()]
        public void TestCleanUp()
        {
            _server.Dispose();
        }

        [TestMethod]
        public void HealthController_Should_Return_Up()
        {
            var url = _baseUrl + "/api/health";
            var response = _client.GetAsync(url).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;

            Assert.IsTrue(responseString.ToLower().Contains("\"up\""));
        }
    }
}
