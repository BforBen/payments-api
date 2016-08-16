using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Runtime.Caching;
using CustomerPoint.Payments.Api.Models;
using GuildfordBoroughCouncil.Linq;

namespace CustomerPoint.Payments.Api.Controllers.v1
{
    /// <summary>
    /// 
    /// </summary>
    [RoutePrefix("v1")]
    public class LedgerCodesController : ApiController
    {
        private LedgerCodeData db = new LedgerCodeData();
        private MemoryCache cache = MemoryCache.Default;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("account-codes")]
        [ResponseType(typeof(IEnumerable<LedgerAccount>))]
        public async Task<IHttpActionResult> GetAccounts(string Account = null)
        {
            var Accounts = (IEnumerable<LedgerAccount>)cache.Get("LedgerAccounts");

            if (Accounts == null)
            {
                Accounts = await db.Accounts.ToListAsync();

                cache.Add("LedgerAccounts", Accounts, new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(3) });
            }

            Accounts = Accounts
                .WhereIf(!String.IsNullOrWhiteSpace(Account), c => c.Account.Equals(Account, StringComparison.CurrentCultureIgnoreCase));

            return Ok(Accounts);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("cost-centres")]
        [ResponseType(typeof(IEnumerable<LedgerCostCentre>))]
        public async Task<IHttpActionResult> GetCostCentres(string CostCentre = null)
        {
            var CostCentres = (IEnumerable<LedgerCostCentre>)cache.Get("CostCentres");

            if (CostCentres == null)
            {
                CostCentres = await db.CostCentres.ToListAsync();

                cache.Add("CostCentres", CostCentres, new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(3) });
            }

            CostCentres = CostCentres
                .WhereIf(!String.IsNullOrWhiteSpace(CostCentre), c => c.CostCentre.Equals(CostCentre, StringComparison.CurrentCultureIgnoreCase));

            return Ok(CostCentres);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ledger-codes")]
        [ResponseType(typeof(IEnumerable<LedgerAccountCostCentre>))]
        public async Task<IHttpActionResult> GetCombinations(string Code = null, string CostCentre = null, string Account = null)
        {
            var Combinations = (IEnumerable<LedgerAccountCostCentre>)cache.Get("LedgerCodes");

            if (Combinations == null)
            {
                Combinations = await db.Combinations.ToListAsync();

                cache.Add("LedgerCodes", Combinations, new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(3) });
            }

            Combinations = Combinations
                .WhereIf(!String.IsNullOrWhiteSpace(Code), c => c.Code.Equals(Code, StringComparison.CurrentCultureIgnoreCase))
                .WhereIf(!String.IsNullOrWhiteSpace(CostCentre), c => c.CostCentre.Equals(CostCentre, StringComparison.CurrentCultureIgnoreCase))
                .WhereIf(!String.IsNullOrWhiteSpace(Account), c => c.Account.Equals(Account, StringComparison.CurrentCultureIgnoreCase));

            return Ok(Combinations);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}