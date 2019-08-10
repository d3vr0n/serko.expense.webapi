using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using serko.expense.models;

namespace serko.expense.webapi.integration.test
{
    [TestClass]
    public class ExpenseControllerTest
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
        public void ExpenseController_Should_Fail_With_Empty_Body()
        {
            var url = _baseUrl + "/api/expense/ParseExpenseFromEmail";
            var response = _client.PostAsync(url,new StringContent("")).Result;
            Assert.IsTrue(response.StatusCode == HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void ExpenseController_Should_Fail_With_Invalid_Total()
        {
            string cost_centre = "DEV002";
            string payment_method = "personal card";
            string total = "total is 118";
            var emailContent = $"<expense><cost_centre>{cost_centre}</cost_centre><total>{total}</total><payment_method>{payment_method}</payment_method></expense>";

            var url = _baseUrl + "/api/expense/ParseExpenseFromEmail";
            var response = _client.PostAsync(url, new StringContent(emailContent)).Result;

            Assert.IsTrue(response.StatusCode == HttpStatusCode.InternalServerError);
        }

        [TestMethod]
        public void ExpenseController_Should_Fail_With_Missing_Total()
        {
            string cost_centre = "DEV002";
            string payment_method = "personal card";
            var emailContent = $"<expense><cost_centre>{cost_centre}</cost_centre><payment_method>{payment_method}</payment_method></expense>";

            var url = _baseUrl + "/api/expense/ParseExpenseFromEmail";
            var response = _client.PostAsync(url, new StringContent(emailContent)).Result;

            Assert.IsTrue(response.StatusCode == HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void ExpenseController_Should_Succeed()
        {
            var emailContent = @"Hi Yvaine,
                                Please create an expense claim for the below. Relevant details are marked up as
                                requested...
                                <expense><cost_centre>DEV002</cost_centre>
                                <total>1024.01</total><payment_method>personal card</payment_method>
                                </expense>
                                From: Ivan Castle
                                Sent: Friday, 16 February 2018 10:32 AM
                                To: Antoine Lloyd <Antoine.Lloyd@example.com>
                                Subject: test
                                Hi Antoine,
                                Please create a reservation at the <vendor>Viaduct Steakhouse</vendor> our
                                <description>development team’s project end celebration dinner</description> on
                                <date>Tuesday 27 April 2017</date>. We expect to arrive around
                                7.15pm. Approximately 12 people but I’ll confirm exact numbers closer to the day.
                                Regards,
                                Ivan";
            var url = _baseUrl + "/api/expense/ParseExpenseFromEmail";
            var response = _client.PostAsync(url, new StringContent(emailContent)).Result;

            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);

            var responseString = response.Content.ReadAsStringAsync().Result;

            var expense = JsonConvert.DeserializeObject<ExpenseModel>(responseString);

            Assert.IsNotNull(expense);
            Assert.IsTrue(expense.Total == 1024.01m);
            Assert.IsNotNull(expense.TotalExcludingGST);
        }
    }
}
