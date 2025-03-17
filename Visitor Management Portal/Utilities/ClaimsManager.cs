using System;
using System.Security.Claims;

namespace Visitor_Management_Portal.Utilities
{
    public class ClaimsManager
    {
        public const string OrganizationId = "OrganizationId";
        public const string OrganizationName = "OrganizationName";
        public const string UserId = "UserId";
        public const string UserName = "UserName";

        public static string GetClaimValue(string name)
        {
            Claim claim = ClaimsPrincipal.Current.FindFirst(name);
            if (claim != null)
                return claim.Value;
            else
                return null;
        }

        public static Guid GetUserId()
        {
            return new Guid(GetClaimValue(ClaimTypes.NameIdentifier));
        }

        public static string GetUserName()
        {
            return GetClaimValue(ClaimTypes.Name);
        }

        public static string GetUserRole()
        {
            return GetClaimValue(ClaimTypes.Role);
        }

        public static string GetAuthenticationMethod()
        {
            return GetClaimValue(ClaimTypes.AuthenticationMethod);
        }

        public static Guid GetOrganizationId()
        {
            //return new Guid("744bb051-9d96-ef11-8a6a-000d3ab4aed4");
            return new Guid(GetClaimValue(OrganizationId));
        }

        public static string GetOrganizationName()
        {
            return GetClaimValue(OrganizationName);
        }

        public static string GetUserEmailFromSession()
        {
            var userEmail = System.Web.HttpContext.Current.Session["AzureUserEmail"];
            return userEmail != null ? userEmail.ToString() : null;
        }

        public static string GetUserNameFromSession()
        {
            var userName = System.Web.HttpContext.Current.Session["AzureUserName"];
            return userName != null ? userName.ToString() : null;
        }

        public static string GetUserEmail()
        {
            return GetClaimValue(ClaimTypes.Email);
        }

    }
}