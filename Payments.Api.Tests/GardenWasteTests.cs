using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Http.Results;
using CustomerPoint.Payments.Api.Controllers.v1;
using CustomerPoint.Payments.Api.Models;

using System.Web.Http.Controllers;
using System.Net.Http;

namespace Payments.Api.Tests
{
    [TestClass]
    public class GardenWasteTests
    {
        public PaymentsController Controller = new PaymentsController();

        [TestMethod]
        public async Task TestFullRefund()
        {
            var context = new HttpControllerContext { Request = new HttpRequestMessage { RequestUri = new Uri("http://localhost/?reference[]=20215158&ledgerCode=B7744K3936&from=2015-01-01&to=2015-04-01") } };

            Controller.ControllerContext = context;

            var TransactionsRequest = await Controller.Get(reference: new List<string>() { "20215158" }, ledgerCode: "B7744K3936", from: new DateTime(2015, 01, 01), to: new DateTime(2015, 04, 01));

            Assert.IsInstanceOfType(TransactionsRequest, typeof(OkNegotiatedContentResult<IOrderedEnumerable<Transaction>>), "Request failed.");

            var TransactionsContent = TransactionsRequest as OkNegotiatedContentResult<IOrderedEnumerable<Transaction>>;

            var Transactions = TransactionsContent.Content;

            Assert.IsNotNull(Transactions, "No transactions returned.");

            Assert.AreEqual<int>(3, Transactions.Count(), Transactions.Count().ToString() + " transaction(s) were found and 3 were expected.");

            Assert.AreEqual<int>(1, Transactions.Where(t => t.Amount < 0).Count(), "No refunds found.");
        }

        [TestMethod]
        public async Task TestSessionWithFullRefund()
        {
            var context = new HttpControllerContext { Request = new HttpRequestMessage { RequestUri = new Uri("http://localhost/?id=29RV-6E0L-3NR4") } };

            Controller.ControllerContext = context;

            var TransactionsRequest = await Controller.GetPayment(id: "29RV-6E0L-3NR4");

            Assert.IsInstanceOfType(TransactionsRequest, typeof(OkNegotiatedContentResult<IOrderedEnumerable<Transaction>>), "Request failed.");

            var TransactionsContent = TransactionsRequest as OkNegotiatedContentResult<IOrderedEnumerable<Transaction>>;

            var Transactions = TransactionsContent.Content;

            Assert.IsNotNull(Transactions, "No transactions returned.");

            Assert.AreEqual<int>(2, Transactions.Count(), Transactions.Count().ToString() + " transaction(s) were found and 2 were expected.");

            Assert.AreEqual<int>(1, Transactions.Where(t => t.Amount < 0).Count(), "No refunds found.");
        }
    }
}
