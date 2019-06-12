using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TendaAdvisors.Models.Response
{
    public class ProfileDetailsResponse
    {

        public int Id { get; set; }
        public int ContactId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Email2 { get; set; }
        public string Tel1 { get; set; }
        public string Tel2 { get; set; }
        public string Cell1 { get; set; }
        public string Cell2 { get; set; }
        public string IdNumber { get; set; }
        public string ContactTitle_Name { get; set; }
        public int ContactTitle_Id { get; set; }
    }
}