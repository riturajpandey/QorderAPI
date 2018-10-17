using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrinkingBuddy.Models
{
    public class DrankBankModel
    {





    }

    public class BalanceResponse
    {
        public int PatronWalletID { get; set; }
        public decimal? Balance { get; set; }


    }

    public class TransectionsResponse
    {
        public string TransectionType { get; set; }
        public DateTime? TransectionDate { get; set; }
        public decimal? Amount { get; set; }
        
    }

    public class TransferModel
    {
        public int SenderPatronID { get; set; }
        public string Email { get; set; }
        public decimal Amount { get; set; }
    }

    public class TransferResponsemodel
    {
       public decimal? UpdatedBalace { get; set; }
    }

    public class Startwalletresponse
    {
        public int PatronwalletID { get; set; }
        public decimal? CurrentBalance { get; set; }

    }
}