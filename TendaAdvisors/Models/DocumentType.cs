using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace TendaAdvisors.Models
{
    public class DocumentType
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; } //ID Document, FAIS Certificate, Qualification, 
        public string BaseFileStoragePath { get; set; }

        public int ApplicationType_Id { get; set; }

        public ICollection<AdvisorType> AdvisorTypes { get; set; }
        public ICollection<ApplicationType> ApplicationTypes { get; set; }
    }
}