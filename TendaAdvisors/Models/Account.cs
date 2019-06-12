using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TendaAdvisors.Models
{
    public class Account
    {
        public int Id { get; set; }
         //maxlength()
        //[DataType(DataType.MultilineText)]
        [MaxLength(30)]
        public string Note { get; set; }
        public int? Advisor_Id { get; set; }
        public int? Supplier_Id { get; set; }
        //nav
        [ForeignKey("Advisor_Id")]
        public virtual Advisor Advisor { get; set; }
        [ForeignKey("Supplier_Id")]
        public virtual Supplier Supplier { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}