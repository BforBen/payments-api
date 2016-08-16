using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Threading.Tasks;
using CustomerPoint.Payments.Api.Models;
using GuildfordBoroughCouncil.Linq;

namespace CustomerPoint.Payments.Api.Controllers.v1
{
    /// <summary>
    /// 
    /// </summary>
    [RoutePrefix("v1")]
    public class PaymentsController : ApiController
    {
        private const string GW_LEDGER_CODE = "B7744K3936";

        /// <summary>
        /// Returns a filtered list of transactions.
        /// </summary>
        /// <param name="id">Transcation ID</param>
        /// <param name="reference">List of references</param>
        /// <param name="fund">Fund code</param>
        /// <param name="ledgerCode">Ledger code</param>
        /// <param name="from">From date</param>
        /// <param name="to">To date</param>
        /// <param name="methods">Methods of payment to include</param>
        /// <param name="xmethods">Methods of payment to exclude</param>
        /// <returns>Returns a list of transactions.</returns>
        [HttpGet]
        [Route("transactions")]
        [ResponseType(typeof(IEnumerable<Transaction>))]
        public async Task<IHttpActionResult> Get(string id = null, [FromUri] IEnumerable<string> reference = null, string fund = null, string ledgerCode = null, DateTime? from = null, DateTime? to = null, [FromUri] IEnumerable<string> methods = null, [FromUri] IEnumerable<string> xmethods = null)
        {
            if (reference == null)
            {
                reference = new List<string>();
            }

            if (methods == null)
            {
                methods = new List<string>();
            }

            if (xmethods == null)
            {
                xmethods = new List<string>();
            }

            reference = reference.Where(r => !String.IsNullOrWhiteSpace(r));
            methods = methods.Where(m => !String.IsNullOrWhiteSpace(m)).Select(m => m.ToUpper());
            xmethods = xmethods.Where(m => !String.IsNullOrWhiteSpace(m)).Select(m => m.ToUpper());

            if (methods.Count() > 0 && xmethods.Count() > 0)
            {
                var Both = methods.Intersect(xmethods);

                if (Both.Count() > 0)
                {
                    return BadRequest("The same type of payment exists in both the include and exclude lists.");
                }
            }

            if (methods.Where(m => !Utils.Methods.Select(k => k.Item2).Contains(m)).Count() > 0)
            {
                return BadRequest("Unknown method of payment specified in methods.");
            }

            if (xmethods.Where(m => !Utils.Methods.Select(k => k.Item2).Contains(m)).Count() > 0)
            {
                return BadRequest("Unknown method of payment specified in xmethods.");
            }

            methods = Utils.Methods.Where(m => methods.Contains(m.Item2)).Select(m => m.Item1).ToList();
            xmethods = Utils.Methods.Where(m => xmethods.Contains(m.Item2)).Select(m => m.Item1).ToList();

            using (var pdb = new PaymentsData())
            {
                IEnumerable<Transaction> Payments = new List<Transaction>();

                // Load Direct Debits for garden waste only
                if ((ledgerCode == null || ledgerCode == GW_LEDGER_CODE) && ((xmethods.Count() > 0 && !xmethods.Contains("DDBT")) || (methods.Count() > 0 && methods.Contains("DDBT")) || (methods.Count() == 0 && xmethods.Count() == 0)))
                {
                    decimal? DirectDebitId = null;

                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        // If an ID is specified try and cast it to a decimal so we can look for payments. If it doesn't convert, set to -0.1 which I'm hoping will never be a valid ID.
                        try
                        {
                            DirectDebitId = Decimal.Parse(id);
                        }
                        catch
                        {
                            DirectDebitId = -0.1m;
                        }
                    }

                    Payments = await pdb.GardenWasteDirectDebits
                        .WhereIf(DirectDebitId.HasValue, t => t.PaymentReference == DirectDebitId.Value)
                        .WhereIf(reference.Count() > 0, t => reference.Contains(t.RadiusNumber))
                        .WhereIf(from.HasValue, t => t.Generated >= from)
                        .WhereIf(to.HasValue, t => t.Generated < to)
                        .Select(t => new Transaction
                        {
                            Account = new List<string>() { t.RadiusNumber },
                            Amount = t.Amount.Value,
                            LedgerCode = GW_LEDGER_CODE,
                            Channel = "DDBT",
                            Date = t.Generated.Value,
                            Description = t.Name,
                            Method = "DDBT",
                            Id = t.PaymentReference.ToString(),
                            User = String.Empty,
                            Vat = 0
                        }).ToListAsync();
                }

                if ((ledgerCode == null || ledgerCode == GW_LEDGER_CODE) && ((xmethods.Count() > 0 && !xmethods.Contains("31")) || (methods.Count() > 0 && methods.Contains("31")) || (methods.Count() == 0 && xmethods.Count() == 0)))
                {
                    List<string> RadiusNumbersToCheck = null;

                    if (reference.Count() > 0)
                    {
                        RadiusNumbersToCheck = reference.ToList();
                    }
                    else if (!string.IsNullOrWhiteSpace(id))
                    {
                        RadiusNumbersToCheck = await pdb.GardenWasteDirectDebits.Select(d => d.RadiusNumber).Distinct().ToListAsync();
                    }

                    var UnpaidDirectDebits = await pdb.CivicaIconTransactions
                        .WhereIf(from.HasValue, t => t.Transaction_Date >= from)
                        .WhereIf(to.HasValue, t => t.Transaction_Date < to)
                        .WhereIf(RadiusNumbersToCheck != null, t => RadiusNumbersToCheck.Contains(t.Account_Reference))
                        .WhereIf(!string.IsNullOrWhiteSpace(id), t => t.Transaction_Reference == id)
                        .Where(t => t.Fund_Code == "88" && t.Mop_Code == "31").Select(t => new Transaction
                        {
                            Account = new List<string>() { t.Account_Reference },
                            Amount = t.Amount,
                            Channel = t.Mop_Code,
                            Date = t.Transaction_Date.Value,
                            Description = t.Narrative,
                            LedgerCode = GW_LEDGER_CODE,
                            Method = t.Mop_Code,
                            Id = t.Transaction_Reference,
                            User = t.Network_User_ID,
                            Vat = t.VAT
                        }).ToListAsync();

                    Payments = Payments.Concat(UnpaidDirectDebits);
                }

                var Civica = await pdb.CivicaIconTransactions
                    .WhereIf(from.HasValue, t => t.Transaction_Date >= from)
                    .WhereIf(to.HasValue, t => t.Transaction_Date < to)
                    .WhereIf(!String.IsNullOrWhiteSpace(ledgerCode), t => t.ledgerCode == ledgerCode)
                    .WhereIf(methods.Count() > 0, t => methods.Contains(t.Mop_Code))
                    .WhereIf(xmethods.Count() > 0, t => !xmethods.Contains(t.Mop_Code))
                    .WhereIf(reference.Count() > 0, t => reference.Contains(t.Account_Reference))
                    .WhereIf(!string.IsNullOrWhiteSpace(id), t => t.Transaction_Reference == id)
                    .Select(t => new Transaction
                    {
                        Account = new List<string>() { t.Account_Reference },
                        Amount = t.Amount,
                        Channel = t.Mop_Code,
                        Date = t.Transaction_Date.Value,
                        Description = t.Narrative,
                        LedgerCode = t.ledgerCode,
                        Method = t.Mop_Code,
                        Id = t.Transaction_Reference,
                        User = t.Network_User_ID,
                        Vat = t.VAT
                    }).ToListAsync();

                // Update channel and method to standardised values
                Payments = Payments.Concat(Civica).Select(t =>
                {
                    t.Channel = Utils.GetChannel(t.Description, t.Channel);
                    t.Method = Utils.GetMethod(t.Method);
                    return t;
                });
                
                // A little hack to stop sending DDBT to Adelante

                if (!methods.All(m => m == "DDBT") || methods.Count() == 0)
                {
                    using (var wc = new HttpClient())
                    {
                        string IasUrl = "https://iaspayments.guildford.gov.uk/services/payments/adelante/v3/transactions" + Request.RequestUri.Query.Replace("methods=DDBT", "").Replace("methods[]=DDBT", "").Replace("&&", "&");

                        Serilog.Log.Debug("Sending request to {0}", IasUrl);

                        var IasRequest = await wc.GetAsync(IasUrl);

                        IasRequest.EnsureSuccessStatusCode();

                        var Adelante = await IasRequest.Content.ReadAsAsync<IEnumerable<TempAdelanteTransaction>>();

                        // Cast to transaction as a temporary measure because service is returning ledger in fund.
                        Payments = Payments.Concat(Adelante.Select(a => a as Transaction));
                    }
                }

                return Ok(Payments.OrderBy(t => t.Date));
            }
        }

        /// <summary>
        /// Returns details of a specific transaction.
        /// </summary>
        /// <param name="id">The transaction reference number</param>
        /// <returns>Returns the details of the transaction with the specified reference number.</returns>
        [HttpGet]
        [Route("transactions/{id}")]
        [ResponseType(typeof(IEnumerable<Transaction>))]
        public async Task<IHttpActionResult> GetPayment(string id = null)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                return BadRequest("No transaction reference specified.");
            }

            return await Get(id);
        }
    }
}
