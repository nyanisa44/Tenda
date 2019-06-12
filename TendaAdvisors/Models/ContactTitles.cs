using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TendaAdvisors.Models
{
    public class ContactTitles
    {
        public int Id { get; set; }
        //Invalid, Not Regustered
        public string Name { get; set; }

        public ICollection<Contact> Contacts { get; set; }
    }
}