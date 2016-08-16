using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomerPoint.Payments.Api
{
    public static class Utils
    {
        public static List<Tuple<string, string, string>> Methods = new List<Tuple<string, string, string>>()
            {
                new Tuple<string, string, string>("01", "CASH", "Staff"),
                new Tuple<string, string, string>("02", "CHEQ", "Staff"),
                new Tuple<string, string, string>("03", "PODR", "Staff"),
                new Tuple<string, string, string>("04", "CASH", "Staff"),
                new Tuple<string, string, string>("05", "CHEQ", "Staff"),
                new Tuple<string, string, string>("06", "PODR", "Staff"),
                new Tuple<string, string, string>("07", "GIRO", "Auto"),
                new Tuple<string, string, string>("08", "DRAW", "Staff"),
                new Tuple<string, string, string>("09", "INDM", "Staff"),
                new Tuple<string, string, string>("10", "DCRD", "Staff"),
                new Tuple<string, string, string>("11", "CCRD", "Staff"),
                new Tuple<string, string, string>("13", "CARD", "Staff"),
                new Tuple<string, string, string>("14", "BALF", "Staff"),
                new Tuple<string, string, string>("15", "CARD", "Staff"),
                new Tuple<string, string, string>("16", "CCRD", "Web"),
                new Tuple<string, string, string>("17", "DCRD", "Web"),
                new Tuple<string, string, string>("19", "BANK", "Auto"),
                new Tuple<string, string, string>("20", "BANK", "Auto"),
                new Tuple<string, string, string>("21", "UNDD", "Auto"),
                new Tuple<string, string, string>("22", "GIRO", "Auto"),
                new Tuple<string, string, string>("23", "BANK", "Auto"),
                new Tuple<string, string, string>("24", "BANK", "Auto"),
                new Tuple<string, string, string>("26", "BANK", "Auto"),
                new Tuple<string, string, string>("27", "BANK", "Auto"),
                new Tuple<string, string, string>("28", "PRLL", "Auto"),
                new Tuple<string, string, string>("30", "BANK", "Auto"),
                new Tuple<string, string, string>("31", "UNDD", "Auto"),
                new Tuple<string, string, string>("32", "BANK", "Auto"),
                new Tuple<string, string, string>("33", "BANK", "Auto"),
                new Tuple<string, string, string>("34", "BANK", "Auto"),
                new Tuple<string, string, string>("35", "BANK", "Auto"),
                new Tuple<string, string, string>("36", "CCRD", "Staff"),
                new Tuple<string, string, string>("37", "DCRD", "Staff"),
                new Tuple<string, string, string>("38", "CCRD", "ATP"),
                new Tuple<string, string, string>("39", "DCRD", "ATP"),
                new Tuple<string, string, string>("40", "CHEQ", "Staff"),
                new Tuple<string, string, string>("CQ", "CHEQ", "Staff"),
                new Tuple<string, string, string>("CU", "CASH", "Staff"),
                new Tuple<string, string, string>("JN", "JRNL", "Staff"),
                new Tuple<string, string, string>("MP", "MIXD", "Staff"),
                new Tuple<string, string, string>("PC", "CASH", "Staff"),
                new Tuple<string, string, string>("TR", "TRFR", "Staff"),
                new Tuple<string, string, string>("TS", "LODF", "Staff"),
                new Tuple<string, string, string>("TT", "BALF", "Staff"),
                new Tuple<string, string, string>("DDBT", "DDBT", "Auto"),

                /*new Tuple<string, string>("AM", "CCRD"),
                new Tuple<string, string>("BANKTAP1", "BANK"),
                new Tuple<string, string>("BANKTAP2", "BANK"),
                new Tuple<string, string>("BANKTAP3", "BANK"),
                new Tuple<string, string>("BANKTAP4", "BANK"),
                new Tuple<string, string>("BANKTAP5", "BANK"),
                new Tuple<string, string>("CARD", "CARD"),
                new Tuple<string, string>("CASH", "CASH"),
                new Tuple<string, string>("CASHPOST", "CASH"),
                new Tuple<string, string>("CHQ", "CHEQ"),
                new Tuple<string, string>("CHEQUE", "CHEQ"),
                new Tuple<string, string>("CHQPOST", "CHEQ"),
                new Tuple<string, string>("DD", "DD"),
                new Tuple<string, string>("GIRO", "GIRO"),
                new Tuple<string, string>("GIROIMP", "GIRO"),
                new Tuple<string, string>("INDEMNITY", "INDM"),
                new Tuple<string, string>("INTRACR", "CARD"),
                new Tuple<string, string>("INTRADB", "CARD"),
                new Tuple<string, string>("JRNL", "JRNL"),
                new Tuple<string, string>("MC", "CCRD"),
                new Tuple<string, string>("MD", "DCRD"),
                new Tuple<string, string>("MI", "DCRD"),
                new Tuple<string, string>("MIXED", "MIXD"),
                new Tuple<string, string>("PAYROLL", "PRLL"),
                new Tuple<string, string>("PETTYCASH", "CASH"),
                new Tuple<string, string>("REFDRAW", "DRAW"),
                new Tuple<string, string>("SW", "DCRD"),
                new Tuple<string, string>("TR", "TRFR"),
                new Tuple<string, string>("TRANBAI", "BALF"),
                new Tuple<string, string>("TS", "LODF"),
                new Tuple<string, string>("TU", "DEBT"),
                new Tuple<string, string>("UDD", "UNDD"),
                new Tuple<string, string>("VC", "CCRD"),
                new Tuple<string, string>("VD", "DCRD"),
                new Tuple<string, string>("VE", "DCRD"),
                new Tuple<string, string>("VP", "CCRD")*/
            };

        public static IEnumerable<string> TrimList(IEnumerable<string> list)
        {
            return list.Where(s => !String.IsNullOrWhiteSpace(s));
        }

        public static string GetChannel(string description, string payMethod)
        {
            var Channel = "Unknown";

            var Method = Methods.Where(m => m.Item1 == payMethod).SingleOrDefault();

            if (Method != null)
                Channel = Method.Item3;

            if (!String.IsNullOrWhiteSpace(description))
            {
                description = description.Trim();

                if (description.EndsWith("CPx"))
                {
                    Channel = "Web";
                }
                else if (description.EndsWith("CPi"))
                {
                    Channel = "CSC";
                }
            }

            return Channel;
        }

        public static string GetMethod(string type)
        {
            var Method = Methods.Where(m => m.Item1 == type).SingleOrDefault();

            return (Method != null) ? Method.Item2 : "OTHR";
        }
    }
}
