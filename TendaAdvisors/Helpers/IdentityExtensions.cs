using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TendaAdvisors
{
    public static class IdentityExtensions
    {
        public static void ThrowOnFailure(this IdentityResult result)
        {
            if (!result.Succeeded)
            {
                string errors = string.Join(". ", result.Errors);
                throw new InvalidOperationException("The operation failed. " + errors);
            }
        }
    }
}