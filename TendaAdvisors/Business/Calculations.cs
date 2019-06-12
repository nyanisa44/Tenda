using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TendaAdvisors.Models;

namespace TendaAdvisors.Business
{
    public class Calculations
    {
       
        public decimal GetTax(decimal monthlyIncome)
        {
          
            decimal monthlyTax = 0m;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                int currentYear = DateTime.Now.Year;
                decimal annualIncome = monthlyIncome * 12;
                AnnualTaxBracket atb = db.AnnualTaxBrackets.LastOrDefault(a => a.year == currentYear 
                && annualIncome > a.MinIncome && annualIncome <= a.MaxIncome);
                //Exerpt from SARS tax rate page 
                //33 840 + 26% of taxable income above 188 000
                //interpretation based on [AnnualTaxBracket] entity >>>  [basic] + ([rate] * (annualIncome - [threshhold]))
                decimal annualTax = atb.Basic + (atb.rate * (annualIncome - atb.Threshold));
                monthlyTax = annualTax / 12;
            }
            return monthlyTax;
        }
    }
}