using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TendaAdvisors.Models
{
    public class ApplicationType
    {
        public int Id { get; set; }

        public string Title { get; set; } 

        public ICollection<Application> Applications { get; set; }
        public ICollection<DocumentType> DocumentTypes { get; set; }
    }
}