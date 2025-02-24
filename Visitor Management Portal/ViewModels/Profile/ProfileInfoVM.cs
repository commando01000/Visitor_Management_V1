using Visitor_Management_Portal.ViewModels.OrganizationSetup;

namespace Visitor_Management_Portal.ViewModels.Profile
{
    public class ProfileInfoVM
    {
        public string JobTitle { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public int? FloorNumber { get; set; }
        public OrganizationSetup.BuildingVM Building { get; set; }
        public ZoneVM Zone { get; set; }
    }
}