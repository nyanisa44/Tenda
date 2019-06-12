using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TendaAdvisors.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        [DataType(DataType.Currency)]
        public decimal DebitAmount { get; set; }
        [DataType(DataType.Currency)]
        public decimal CreditAmount { get; set; }
        public string Note { get; set; }

        //For dropdown in view
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime TransactionDate { get; set; }

        //Add "timestamp" for modification and MD5 of all columns on create
       
        //FK
        public int AccountId { get; set; }
        public int ContraAccountId { get; set; }
    }
}