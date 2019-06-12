using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TendaAdvisors.ViewModels
{
    public class User
    {
        public string Username { get; set; }

        public string FullName { get { return $"{FirstName} {LastName}"; } private set { } }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public string Email { get; set; }

        public bool IsAdmin { get; set; }

        public int AdvisorId { get; set; }
    }
}