using System;
using System.Threading.Tasks;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security;
using Microsoft.Owin;
using TendaAdvisors.Models;
using Microsoft.AspNet.Identity.Owin;

namespace TendaAdvisors.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId = string.Empty;
            string clientSecret = string.Empty;

            bool success = context.TryGetBasicCredentials(out clientId, out clientSecret) || context.TryGetFormCredentials(out clientId, out clientSecret);
            if (!success || string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                context.Rejected();
                context.SetError("invalid_client", "Client credentials could not be retrieved through the Authorization header.");
                return;
            }

            try
            {
                var user = await AuthenticateAsync(clientId, clientSecret, context);

                if (user != null)
                {
                    context.OwinContext.Set("oauth:client", user);
                    context.Validated(clientId);
                }
                else
                {
                    context.Rejected();
                    context.SetError("invalid_client", "Client credentials are invalid.");
                }
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                context.Rejected();
                context.SetError(ex.Message);
            }
        }

        public override async Task GrantClientCredentials(OAuthGrantClientCredentialsContext context)
        {
            AuthenticationTicket ticket = await GetTicketAsync(context.OwinContext);
            context.Validated(ticket);
        }

        private static async Task<ApplicationUser> AuthenticateAsync(string clientId, string clientSecret, OAuthValidateClientAuthenticationContext context)
        {
            var userManager = context.OwinContext.Get<ApplicationUserManager>();
            return await userManager.FindAsync(clientId, clientSecret);
        }

        private static async Task<AuthenticationTicket> GetTicketAsync(IOwinContext owinContext)
        {
            var user = owinContext.Get<ApplicationUser>("oauth:client");
            if (user == null) throw new InvalidOperationException("oauth:client was not set");

            var userManager = owinContext.Get<ApplicationUserManager>();
            var identity = await userManager.CreateIdentityAsync(user, OAuthDefaults.AuthenticationType);

            AuthenticationProperties properties = new AuthenticationProperties();
            AuthenticationTicket ticket = new AuthenticationTicket(identity, properties);
            return ticket;
        }
    }
}