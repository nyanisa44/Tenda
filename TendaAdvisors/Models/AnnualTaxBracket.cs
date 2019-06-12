using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TendaAdvisors.Models
{
    public class AnnualTaxBracket
    {
        public int AnnualTaxBracketId { get; set; }
        public int year { get; set; }
        public string type { get; set; }
        public decimal rate { get; set; }
        public decimal MinIncome { get; set; }
        public decimal MaxIncome { get; set; }
        public decimal Threshold { get; set; }
        public decimal Basic  { get; set; }
    }
}