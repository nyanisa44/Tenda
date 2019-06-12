using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TendaAdvisors.Models
{
    public class AdvisorType
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } //Key Individual, Advisor, Compliance Officer, Support

        public ICollection<Advisor> Advisors { get; set; }
        public ICollection<DocumentType> DocumentTypes { get; set; }
    }
}