using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace serko.expense.webapi.integration.test
{
    [TestClass]
    public class ExpenseControllerTest_WithoutRawFormatter
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
                    .UseStartup<StartupWithOutRawInputFormatter>());
            _client = _server.CreateClient();
        }

        [TestCleanup()]
        public void TestCleanUp()
        {
            _server.Dispose();
        }

        [TestMethod]
        public void ExpenseController_Should_Fail()
        {
            var url = _baseUrl + "/api/expense/ParseExpenseFromEmail";
            var response = _client.PostAsync(url,new StringContent("Hello")).Result;

            Assert.IsTrue(response.StatusCode == HttpStatusCode.UnsupportedMediaType);
        }
    }
}
