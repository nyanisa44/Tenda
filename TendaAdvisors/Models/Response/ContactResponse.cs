using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TendaAdvisors.Models.Response
{
    public class ContactResponse : BasicContactResponse
    {
        public string Tel1 { get; set; }
        public string Cell1 { get; set; }
    }
}