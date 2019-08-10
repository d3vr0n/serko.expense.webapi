using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using serko.expense.models;
using Assert = NUnit.Framework.Assert;

namespace serko.expense.biz.unittest
{
    public class ExpenseManagerTest
    {
        private ExpenseManager _expenseManager;

        public ExpenseManagerTest()
        {
            // no external dependency, so no mocking required
            _expenseManager = new ExpenseManager();
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ValidateAndExtract_Should_Succeed()
        {
            var emailContent = @"Hi Yvaine,
                                    Please create an expense claim for the below. Relevant details are marked up as
                                    requested...
                                    <expense><cost_centre>DEV002</cost_centre>
                                    <total>1024.01</total><payment_method>personal card</payment_method>
                                    </expense>
                                    Another One here.
                                    <expense><cost_centre>DEV001</cost_centre>
                                    <total>118</total><payment_method>cash</payment_method>
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
            var response = _expenseManager.ValidateAndExtract(emailContent);

            Assert.IsNotNull(response);
            Assert.AreEqual(response.GetType(), typeof(ExpenseModel));
        }
        
        [Test]
        public void ValidateAndExtract_Should_Fail_If_No_Matching_Tag()
        {
            var emailContent = @"Hi Yvaine,
                                    Please create an expense claim for the below. Relevant details are marked up as
                                    requested...
                                    <expense><cost_centre>DEV002</cost_centre>
                                    <total>1024.01</total><payment_method>personal card</payment_method>
                                    </expense1>
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

            Assert.That(() => _expenseManager.ValidateAndExtract(emailContent),
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void ValidateAndExtract_Should_Extract_AllInfo()
        {
            string cost_centre = "DEV002";
            string payment_method = "personal card";
            int total = 118;
            var emailContent = $"<expense><cost_centre>{cost_centre}</cost_centre><total>{total}</total><payment_method>{payment_method}</payment_method></expense>";
            var response = _expenseManager.ValidateAndExtract(emailContent);

            Assert.IsNotNull(response);
            Assert.AreEqual(response.CostCentre, cost_centre);
            Assert.AreEqual(response.PaymentMethod, payment_method);
            Assert.AreEqual(response.GST, 18);
            Assert.AreEqual(response.TotalExcludingGST, 100);
        }

        [Test]
        public void ValidateAndExtract_CostCentre_Should_Default()
        {
            string default_cost_centre = "UNKNOWN";
            string payment_method = "personal card";
            int total = 118;
            var emailContent = $"<expense><total>{total}</total><payment_method>{payment_method}</payment_method></expense>";
            var response = _expenseManager.ValidateAndExtract(emailContent);

            Assert.IsNotNull(response);
            Assert.AreEqual(response.CostCentre, default_cost_centre);
        }

        [Test]
        public void ValidateAndExtract_Throw_Error_If_Total_Missing()
        {
            var emailContent = $"<expense></expense>";

            Assert.That(() => _expenseManager.ValidateAndExtract(emailContent),
                Throws.TypeOf<ArgumentException>());

        }
    }
}