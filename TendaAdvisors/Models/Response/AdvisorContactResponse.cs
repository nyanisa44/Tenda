using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TendaAdvisors.Models.Response
{
    public class AdvisorContactResponse
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IdNumber { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string EmployerName { get; set; }
        public string JobTitle { get; set; }
        public string Email { get; set; }
        public string Tel1 { get; set; }
        public string Cell1 { get; set; }
    }
}