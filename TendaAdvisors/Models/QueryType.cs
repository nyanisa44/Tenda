using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TendaAdvisors.Models
{
    public class QueryType
    {
        public int Id { get; set; }
        [MaxLength(30)]
        public string QueryName { get; set; }

        //nav
        
        public ICollection<Query> Querys { get; set; }
    }
}