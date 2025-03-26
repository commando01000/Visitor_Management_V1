using System;
using System.Web.Mvc;
using Unity;
using Unity.AspNet.Mvc;
using Visitor_Management_Portal.BLL.Interfaces;
using Visitor_Management_Portal.BLL.Ser;
using Visitor_Management_Portal.BLL.Services;
using Visitor_Management_Portal.DAL.Repository.AccountRepository;
using Visitor_Management_Portal.DAL.Repository.BuildingRepository;
using Visitor_Management_Portal.DAL.Repository.LocationRepository;
using Visitor_Management_Portal.DAL.Repository.OrganizationDate;
using Visitor_Management_Portal.DAL.Repository.OrganizationSetupRepository;
using Visitor_Management_Portal.DAL.Repository.OrganizationUsersRepository;
using Visitor_Management_Portal.DAL.Repository.ProfileRepository;
using Visitor_Management_Portal.DAL.Repository.VisitingMemberRepository;
using Visitor_Management_Portal.DAL.Repository.VisitorsHubRepository;
using Visitor_Management_Portal.DAL.Repository.VisitRequestRepository;
using Visitor_Management_Portal.DAL.Repository.VisitorMemberHubRepository;

namespace Visitor_Management_Portal
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public static class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container =
          new Lazy<IUnityContainer>(() =>
          {
              var container = new UnityContainer();
              RegisterTypes(container);
              return container;
          });

        /// <summary>
        /// Configured Unity Container.
        /// </summary>
        public static IUnityContainer Container => container.Value;
        #endregion

        /// <summary>
        /// Registers the type mappings with the Unity container.
        /// </summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>
        /// There is no need to register concrete types such as controllers or
        /// API controllers (unless you want to change the defaults), as Unity
        /// allows resolving a concrete type even if it was not previously
        /// registered.
        /// </remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below.
            // Make sure to add a Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your type's mappings here.
            // container.RegisterType<IProductRepository, ProductRepository>();


            container.RegisterType<IProfileRepository, ProfileRepository>();
            container.RegisterType<IAccountRepository, AccountRepository>();
            container.RegisterType<IAccountService, AccountService>();
            container.RegisterType<IVisitingMemberService, VisitingMemberService>();

            container.RegisterType<ILocationRepository, LocationRepository>();
            container.RegisterType<IVisitRequestRepository, VisitRequestRepository>();
            container.RegisterType<IOrganizationDataRepository, OrganizationDataRepository>();
            container.RegisterType<IOrganizationUsersRepository, OrganizationUsersRepository>();
            container.RegisterType<IOrganizationUserService, OrganizationUserService>();
            container.RegisterType<IBuildingRepository, BuildingRepository>();
            container.RegisterType<IVisitorsHubRepository, VisitorsHubRepository>();
            container.RegisterType<IVisitingMemberRepository , VisitingMemberRepository>();
            container.RegisterType<IOrganizationService, OrganizationService>();
            container.RegisterType<IVisitorMemberHubRepository, VisitorMemberHubRepository>();

            container.RegisterType<IZoneRepository, ZoneRepository>();
            container.RegisterType<IZoneService, ZoneService>();

            container.RegisterType<IMeetingAreaRepository, MeetingAreaRepository>();
            container.RegisterType<IMeetingAreaService, MeetingAreaService>();

            container.RegisterType<IDashboardService, DashboardService>();

            container.RegisterType<IVisitorsService, VisitorsService>();
            container.RegisterType<IBuildingService, BuildingService>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}