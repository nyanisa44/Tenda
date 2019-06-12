using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace TendaAdvisors.Models
{
    public class ApplicationAdvisorEditHistory
    {
        public int Id { get; set; }
        public DateTime DateEdited { get; set; }

        // foreign keys
        public int ApplicationId { get; set; }
        public int AdvisorId { get; set; }
    }
}