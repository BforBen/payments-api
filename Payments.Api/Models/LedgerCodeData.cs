using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CustomerPoint.Payments.Api.Models
{
    [Table("vw_Accounts")]
    public class LedgerAccount
    {
        [Key]
        [StringLength(8)]
        public string Account { get; set; }
        
        [StringLength(50)]
        [Column("Account_Name")]
        public string AccountName { get; set; }
        
        [StringLength(1)]
        public string Postable { get; set; }
        
        [StringLength(8)]
        public string Parent { get; set; }

        [StringLength(20)]
        public string Type { get; set; }
    }

    [Table("vw_Combinations")]
    public class LedgerAccountCostCentre
    {
        public string Code
        {
            get
            {
                return CostCentre + Account;
            }
        }

        [Key]
        [Column("Cost_Centre", Order = 0)]
        [StringLength(6)]
        public string CostCentre { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(8)]
        public string Account { get; set; }

        [StringLength(50)]
        [Column("Cost_Centre_Name")]
        public string CostCentreName { get; set; }

        [StringLength(50)]
        [Column("Account_Name")]
        public string AccountName { get; set; }

        [StringLength(20)]
        public string Type { get; set; }
    }

    [Table("vw_Cost_Centres")]
    public class LedgerCostCentre
    {
        [Key]
        [Column("Cost_Centre")]
        [StringLength(6)]
        public string CostCentre { get; set; }

        [StringLength(50)]
        [Column("Cost_Centre_Name")]
        public string CostCentreName { get; set; }

        [StringLength(1)]
        public string Postable { get; set; }

        [StringLength(6)]
        public string Parent { get; set; }

        [StringLength(20)]
        public string Type { get; set; }
    }

    //[Table("vw_Service_Unit_Codes")]
    //public class ServiceUnitCode
    //{
    //    [Key]
    //    [StringLength(6)]
    //    public string Cost_Centre { get; set; }

    //    [StringLength(8)]
    //    public string Account { get; set; }

    //    [StringLength(50)]
    //    public string Cost_Centre_Name { get; set; }

    //    [StringLength(50)]
    //    public string Account_Name { get; set; }

    //    [StringLength(15)]
    //    public string Cost_Code { get; set; }

    //    [StringLength(255)]
    //    public string Service_Unit { get; set; }
    //}

    //[Table("vwCostCodesServiceUnit")]
    //public class CostCodesServiceUnit
    //{
    //    [Key]
    //    [Column(Order = 0)]
    //    [StringLength(6)]
    //    public string Cost_Centre { get; set; }

    //    [Key]
    //    [Column(Order = 1)]
    //    [StringLength(50)]
    //    public string Cost_Centre_Name { get; set; }

    //    [Key]
    //    [Column(Order = 2)]
    //    [StringLength(8)]
    //    public string Account { get; set; }

    //    [Key]
    //    [Column(Order = 3)]
    //    [StringLength(50)]
    //    public string Account_Name { get; set; }

    //    [StringLength(6)]
    //    public string Service_Unit_Code { get; set; }

    //    [StringLength(50)]
    //    public string Service_Unit_Desc { get; set; }

    //    [Key]
    //    [Column(Order = 4)]
    //    [StringLength(1)]
    //    public string Postable { get; set; }
    //}

    public class LedgerCodeData : DbContext
    {
        public LedgerCodeData()
            : base("name=LedgerCodeData")
        {
        }

        public virtual DbSet<LedgerAccount> Accounts { get; set; }
        public virtual DbSet<LedgerAccountCostCentre> Combinations { get; set; }
        public virtual DbSet<LedgerCostCentre> CostCentres { get; set; }
        //public virtual DbSet<ServiceUnitCode> ServiceUnitCodes { get; set; }
        //public virtual DbSet<CostCodesServiceUnit> CostCodesServiceUnits { get; set; }
    }
}
