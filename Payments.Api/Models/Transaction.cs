using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CustomerPoint.Payments.Api.Models
{
    public class Transaction
    {
        /// <summary>
        /// Transaction reference number
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Transaction date
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy HH:mm}")]
        public DateTime Date { get; set; }

        public IEnumerable<string> Account { get; set; }
        public string Description { get; set; }

        public string LedgerCode { get; set; }
        public string Method { get; set; }
        public string Channel { get; set; }

        [DataType(DataType.Currency)]
        public decimal? Amount { get; set; }
        [DataType(DataType.Currency)]
        public decimal? Vat { get; set; }
        public string User { get; set; }
    }

    public class TempAdelanteTransaction : Transaction
    {
        public string Fund
        {
            get
            {
                return LedgerCode;
            }
            set
            {
                LedgerCode = value;
            }
        }
    }
}
