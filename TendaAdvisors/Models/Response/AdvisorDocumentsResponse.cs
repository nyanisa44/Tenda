using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TendaAdvisors.Models.Response
{
    public class AdvisorDocumentsResponse
    {
        public int DocumentId { get; set; }
        public int ? AdvisorId { get; set; }
        public string AdvisorName { get; set; }
        public string AdvisorLastname { get; set; }
        public string AdvisorIdNumber { get; set; }
        public int DocumentTypeId { get; set; }
        public string DocumentTitle { get; set; }
        public string DocumentTypeName { get; set; }
        public string DocumenTypeLocation { get; set; }
        public bool Uploaded { get; set; }
        public bool Deleted { get; set; }

        public DateTime? ValidFromDate { get; set; }
        public DateTime? ValidToDate { get; set; }
        public bool Expired { get; set; }
    }
}