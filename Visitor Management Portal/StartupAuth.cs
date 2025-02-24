using Microsoft.AspNet.Identity;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Host.SystemWeb;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System;
using System.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Visitor_Management_Portal.Helpers;

[assembly: OwinStartupAttribute(typeof(Visitor_Management_Portal.StartupAuth))]

namespace Visitor_Management_Portal

{
    public class StartupAuth
    {
        private static string clientId = ConfigurationManager.AppSettings["MS:ClientId"];

        // RedirectUri is the URL where the user will be redirected to after they sign in.
        private static string redirectUri = ConfigurationManager.AppSettings["RedirectUri"];

        // Tenant is the tenant ID (e.g. contoso.onmicrosoft.com, or 'common' for multi-tenant)
        private static string tenant = ConfigurationManager.AppSettings["MS:Tenant"];
        private static string secretId = ConfigurationManager.AppSettings["MS:ClientSecret"];

        // Authority is the URL for authority, composed by Microsoft identity platform endpoint and the tenant name (e.g. https://login.microsoftonline.com/contoso.onmicrosoft.com/v2.0)
        private static string authority = String.Format(System.Globalization.CultureInfo.InvariantCulture, ConfigurationManager.AppSettings["Authority"], tenant);

        private static string graphScopes = ConfigurationManager.AppSettings["AppScopes"];

        public void Configuration(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                CookieManager = new SystemWebCookieManager()
            });

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    // Sets the client ID, authority, and redirect URI as obtained from Web.config
                    ClientId = clientId,
                    Authority = authority,
                    RedirectUri = redirectUri,
                    ClientSecret = secretId,
                    // PostLogoutRedirectUri is the page that users will be redirected to after sign-out. In this case, it's using the home page
                    PostLogoutRedirectUri = "/Account/Index",
                    Scope = $"{graphScopes}",
                    // ResponseType is set to request the code id_token, which contains basic information about the signed-in user
                    ResponseType = OpenIdConnectResponseType.CodeIdToken,

                    TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = false // Simplification (see note below),
                    },

                    //OpenIdConnectAuthenticationNotifications configures OWIN to send notification of failed authentications to the OnAuthenticationFailed method
                    Notifications = new OpenIdConnectAuthenticationNotifications
                    {
                        AuthenticationFailed = OnAuthenticationFailedAsync,
                        AuthorizationCodeReceived = OnAuthorizationCodeReceivedAsync
                    }
                }
            );
            IdentityModelEventSource.ShowPII = true;

        }

        private static Task OnAuthenticationFailedAsync(AuthenticationFailedNotification<OpenIdConnectMessage,
       OpenIdConnectAuthenticationOptions> notification)
        {
            if (notification.Exception.Message.Contains("IDX21323"))
            {
                notification.HandleResponse();
                notification.OwinContext.Authentication.Challenge();
            }
            else
            {
                string redirect = $"/Account/Error?message={notification.Exception.Message}";
                if (notification.ProtocolMessage != null && !string.IsNullOrEmpty(notification.ProtocolMessage.ErrorDescription))
                {
                    redirect += $"&debug={notification.ProtocolMessage.ErrorDescription}";
                }
                notification.Response.Redirect(redirect);
            }
            return Task.FromResult(0);
        }

        private async Task OnAuthorizationCodeReceivedAsync(AuthorizationCodeReceivedNotification notification)
        {
            notification.HandleCodeRedemption();

            var idClient = ConfidentialClientApplicationBuilder.Create(clientId)
                .WithRedirectUri(redirectUri)
                .WithClientSecret(secretId)
                .WithAuthority(authority)
                .Build();

            var signedInUser = new ClaimsPrincipal(notification.AuthenticationTicket.Identity);

            try
            {
                string[] scopes = graphScopes.Split(' ');

                var result = await idClient.AcquireTokenByAuthorizationCode(
                    scopes, notification.Code).ExecuteAsync();

                var userDetails = await GraphHelper.GetUserDetailsAsync(result.AccessToken);

                // Save user email in session
                HttpContext.Current.Session["AzureUserEmail"] = userDetails.Email;
                notification.HandleCodeRedemption(null, result.IdToken);
            }
            catch (MsalException ex)
            {
                string message = "AcquireTokenByAuthorizationCodeAsync threw an exception";
                notification.HandleResponse();
                notification.Response.Redirect($"/Account/Error?message={message}&debug={ex.Message}");
            }
            catch (Microsoft.Graph.ServiceException ex)
            {
                string message = "GetUserDetailsAsync threw an exception";
                notification.HandleResponse();
                notification.Response.Redirect($"/Account/Error?message={message}&debug={ex.Message}");
            }
        }
    }
}