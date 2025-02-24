using Microsoft.Graph;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Visitor_Management_Portal.ViewModels.OrganizationUsers;

namespace Visitor_Management_Portal.Helpers
{
    public class GraphHelper
    {
        public static async Task<OrganizationUserVM> GetUserDetailsAsync(string accessToken)
        {
            var graphClient = new GraphServiceClient(
                new DelegateAuthenticationProvider(
                    (requestMessage) =>
                    {
                        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                        return Task.CompletedTask;
                    }
                )
            );

            var user = await graphClient.Me.Request()
                .Select(u => new
                {
                    u.DisplayName,
                    u.Mail,
                    u.UserPrincipalName
                })
                .GetAsync();

            return new OrganizationUserVM
            {
                FullName = user.DisplayName,
                Email = string.IsNullOrEmpty(user.Mail) ?
                    user.UserPrincipalName : user.Mail
            };
        }
    }
}