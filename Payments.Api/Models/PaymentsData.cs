using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CustomerPoint.Payments.Api.Models
{
    public class CivicaIconTransaction
    {
        [Key]
        [StringLength(12)]
        public string Transaction_Reference { get; set; }

        public DateTime? Transaction_Date { get; set; }

        [StringLength(5)]
        public string Fund_Code { get; set; }

        [StringLength(30)]
        public string ledgerCode { get; set; }

        [StringLength(30)]
        public string Account_Reference { get; set; }

        [StringLength(40)]
        public string Narrative { get; set; }

        [Column(TypeName = "money")]
        public decimal? Amount { get; set; }

        [Column(TypeName = "money")]
        public decimal? VAT { get; set; }

        [StringLength(5)]
        public string Mop_Code { get; set; }

        [StringLength(15)]
        public string Card_Type { get; set; }

        [StringLength(10)]
        public string Credit_Debit { get; set; }

        [StringLength(30)]
        public string Network_User_ID { get; set; }

        [Column(TypeName = "money")]
        public decimal? Transaction_Charge { get; set; }

        [StringLength(12)]
        public string Original_TransactionReference { get; set; }

        [StringLength(3)]
        public string User_Code { get; set; }

        [StringLength(20)]
        public string User_Name { get; set; }

        [StringLength(3)]
        public string source { get; set; }
    }
    
    public class GardenWasteDirectDebit
    {
        [Key]
        [Column(TypeName = "numeric")]
        public decimal PaymentReference { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? Generated { get; set; }

        [StringLength(10)]
        public string Batch { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Amount { get; set; }

        [StringLength(18)]
        public string RadiusNumber { get; set; }

        [StringLength(8)]
        public string EfinCustomerId { get; set; }

        [StringLength(45)]
        public string Name { get; set; }

        [StringLength(45)]
        public string Address1 { get; set; }

        [StringLength(45)]
        public string Address2 { get; set; }

        [StringLength(45)]
        public string Address3 { get; set; }

        [StringLength(10)]
        public string PostCode { get; set; }

        [StringLength(15)]
        public string Telephone { get; set; }
    }

    public class PaymentsData : DbContext
    {
        public PaymentsData()
            : base("name=PaymentsData")
        {
        }

        public virtual DbSet<CivicaIconTransaction> CivicaIconTransactions { get; set; }
        public virtual DbSet<GardenWasteDirectDebit> GardenWasteDirectDebits { get; set; }
    }
}
