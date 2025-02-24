using CrmEarlyBound;
using D365_Add_ons.Connection;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using Visitor_Management_Portal.ViewModels.OrganizationSetup;
using BuildingVM = Visitor_Management_Portal.ViewModels.OrganizationSetup.BuildingVM;

namespace Visitor_Management_Portal.DAL.Repository.LocationRepository
{
    public class LocationRepository : ILocationRepository
    {
        private readonly IOrganizationService _service;
        private readonly CrmServiceContext _context;

        public LocationRepository()
        {
            _service = ServiceManager.GetService();
            _context = new CrmServiceContext(_service);
        }

        public List<BuildingVM> GetBuildings()
        {
            var buildings = _context.vm_BuildingSet.Select(x => new BuildingVM
            {
                Id = x.Id,
                Name = x.vm_BuildingName
            }).ToList();

            return buildings;
        }

        public List<ZoneVM> GetZonesByBuildingId(Guid buildingId)
        {
            try
            {
                var zones = _context.vm_ZoneSet
                    .Where(z => z.vm_Building.Id == buildingId)
                    .Select(z => new ZoneVM
                    {
                        Id = z.Id,
                        Name = z.vm_ZoneName
                    })
                    .ToList();

                return zones;
            }
            catch (Exception ex)
            {

                return new List<ZoneVM>();
            }
        }

        public List<MeetingAreaVM> GetMeetingAreasByZoneId(Guid zoneId)
        {
            try
            {
                var meetingAreas = _context.vm_MeetingAreaSet
                    .Where(m => m.vm_Zone.Id == zoneId) 
                    .Select(m => new MeetingAreaVM
                    {
                        Id = m.Id,
                        Name = m.vm_MeetingAreaName
                    })
                    .ToList();

                return meetingAreas;
            }
            catch (Exception ex)
            {

                return new List<MeetingAreaVM>();
            }
        }

        public List<ZoneVM> GetZones()
        {
            var zones = _context.vm_ZoneSet.Select(x => new ZoneVM
            {
                Id = x.Id,
                Name = x.vm_ZoneName
            }).ToList();

            return zones;
        }
    }
}