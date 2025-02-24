using CrmEarlyBound;
using D365_Add_ons.Extensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using Visitor_Management_Portal.BLL.Interfaces;
using Visitor_Management_Portal.DAL.Repository.BuildingRepository;
using Visitor_Management_Portal.Utilities;
using Visitor_Management_Portal.ViewModels.OrganizationSetup;
using Visitor_Management_Portal.Models;
using Visitor_Management_Portal.Helpers;

namespace Visitor_Management_Portal.BLL.Services
{
    public class BuildingService : IBuildingService
    {
        private readonly IBuildingRepository _buildingRepository;
        public BuildingService(IBuildingRepository buildingRepository)
        {
            _buildingRepository = buildingRepository;
        }

        public List<BuildingVM> GetAllByOrganization(Guid organizationId)
        {
            var query = new QueryExpression(vm_Building.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true)
            };

            query.Criteria.AddCondition("vm_organization", ConditionOperator.Equal, organizationId);

            var entities = _buildingRepository.GetAll(query);

            var buildings = entities.Select(e => new BuildingVM
            {
                Id = e.Id,
                Name = e.GetAttributeValue<string>("vm_buildingname"),
                Code = e.GetAttributeValue<string>("vm_buildingcode"),
                ContactPerson = e.Check("vm_contactperson") ? e.GetEntityReference("vm_contactperson").Name : string.Empty,
                Location = e.GetAttributeValue<string>("vm_locationlink"),
                NoOfZones = e.Check("vm_noofzones") ? VMHelpers.GetZonesString(e.GetAttributeValue<int>("vm_noofzones")) : string.Empty,
                NoOfMeetingAreas = e.Check("vm_noofmeetingareas") ? VMHelpers.GetMeetingAreaString(e.GetAttributeValue<int>("vm_noofmeetingareas")) : string.Empty,
                IsExcludeFromOfficeSelection = e.Check("vm_excludefromofficeselection") ? e.GetAttributeValue<bool>("vm_excludefromofficeselection") : false,

            }).ToList();

            return buildings;
        }

        public BuildingWithZonesAndMeetingAreasDetailsVM Get(Guid id)
        {
            string query = $@"<fetch>
                              <entity name='vm_building'>
                                <attribute name='statecode' />
                                <attribute name='vm_buildingcode' />
                                <attribute name='vm_buildingid' />
                                <attribute name='vm_buildingname' />
                                <attribute name='vm_contactperson' />
                                <attribute name='vm_locationlink' />
                                <attribute name='vm_excludefromofficeselection' />
                                <attribute name='vm_address' />
                                <attribute name='vm_noofmeetingareas' />
                                <attribute name='vm_noofzones' />
                                <filter>
                                  <condition attribute='vm_buildingid' operator='eq' value='{id}' uitype='vm_building' />
                                </filter>
                                <link-entity name='vm_zone' from='vm_building' to='vm_buildingid' link-type='outer' alias='zone'>
                                  <attribute name='statecode' />
                                  <attribute name='vm_excludefromofficeselection' />
                                  <attribute name='vm_zonecode' />
                                  <attribute name='vm_zonename' />
                                  <attribute name='vm_zoneid' />
                                </link-entity>
                                <link-entity name='vm_meetingarea' from='vm_building' to='vm_buildingid' link-type='outer' alias='area'>
                                  <attribute name='statecode' />
                                  <attribute name='vm_appearinvisitrequests' />
                                  <attribute name='vm_building' />
                                  <attribute name='vm_meetingareacode' />
                                  <attribute name='vm_meetingareaid' />
                                  <attribute name='vm_meetingareaname' />
                                  <attribute name='vm_zone' />
                                </link-entity>
                              </entity>
                            </fetch>";

            var data = _buildingRepository.GetAll(query);

            if (data == null || !data.Any())
                return null;


            var zonsData = new List<Zone>();
            var meetingAreaData = new List<MeetingArea>();
            var buildingData = data.First();

            var details = new BuildingWithZonesAndMeetingAreasDetailsVM
            {
                Id = buildingData.Id,
                Name = buildingData.GetAttributeValue<string>("vm_buildingname"),
                Code = buildingData.GetAttributeValue<string>("vm_buildingcode"),
                Address = buildingData.GetAttributeValue<string>("vm_address"),
                LocationLink = buildingData.GetAttributeValue<string>("vm_locationlink"),
                IsExcludeFromOfficeSelection = buildingData.Check("vm_excludefromofficeselection")
                ? buildingData.GetAttributeValue<bool>("vm_excludefromofficeselection") : false,
                ContactPersonId = buildingData.Check("vm_contactperson") ? buildingData.GetEntityReferenceId("vm_contactperson").Value : Guid.Empty,
                ContactPersonName = buildingData.Check("vm_contactperson") ? buildingData.GetEntityReference("vm_contactperson").Name : string.Empty,
                NoOfZones = buildingData.Check("vm_noofzones") ? VMHelpers.GetZonesString(buildingData.GetAttributeValue<int>("vm_noofzones")) : string.Empty,
                NoOfMeetingAreas = buildingData.Check("vm_noofmeetingareas") ? VMHelpers.GetMeetingAreaString(buildingData.GetAttributeValue<int>("vm_noofmeetingareas")) : string.Empty,
                Zones = data.Where(zone => zone.Contains("zone.vm_zoneid"))
                        .GroupBy(zone => zone.GetAliasedValue<Guid>("zone.vm_zoneid"))
                        .Select(group => group.First())
                        .Select(zone => new Zone
                        {
                            Id = zone.GetAliasedValue<Guid>("zone.vm_zoneid"),
                            Name = zone.GetAliasedValue<string>("zone.vm_zonename"),
                            Code = zone.GetAliasedValue<string>("zone.vm_zonecode"),
                            IsExcludeFromOfficeSelection = zone.Check("zone.vm_excludefromofficeselection") ? zone.GetAliasedValue<bool>("zone.vm_excludefromofficeselection") : false,

                        }).ToList(),

                MeetingAreas = data.Where(area => area.Contains("area.vm_meetingareaid"))
                        .GroupBy(area => area.GetAliasedValue<Guid>("area.vm_meetingareaid"))
                        .Select(group => group.First())
                        .Select(area => new MeetingArea
                        {
                            Id = area.GetAliasedValue<Guid>("area.vm_meetingareaid"),
                            Name = area.GetAliasedValue<string>("area.vm_meetingareaname"),
                            Code = area.GetAliasedValue<string>("area.vm_meetingareacode"),
                            IsAvailableForVisitRequests = area.Check("area.vm_appearinvisitrequests") ? area.GetAliasedValue<bool>("area.vm_appearinvisitrequests") : false,
                            ZoneId = area.Check("area.vm_zone") ? area.GetAliasedValue<EntityReference>("area.vm_zone").Id : Guid.Empty,
                            ZoneName = area.Check("area.vm_zone") ? area.GetAliasedValue<EntityReference>("area.vm_zone").Name : string.Empty,

                        }).ToList()
            };

            return details;
        }

        public OperationResult<Guid> AddBuildingWithZonesAndMeetingAreas(AddBuildingWithZonesAndMeetingAreasVM model)
        {
            var message = string.Empty;
            var building = new vm_Building
            {
                Id = Guid.NewGuid(),
                vm_BuildingName = model.Name,
                vm_BuildingCode = model.Code,
                vm_Organization = new EntityReference(vm_Organization.EntityLogicalName, ClaimsManager.GetOrganizationId()),
                vm_ContactPerson = new EntityReference(vm_organizationuser.EntityLogicalName, model.ContactPersonId),
                vm_ExcludeFromOfficeSelection = model.IsExcludeFromOfficeSelection,
                vm_Address = model.Address,
                vm_LocationLink = model.LocationLink,
            };

            var zones = new List<vm_Zone>();
            if (model.Zones != null && model.Zones.Count > 0)
            {
                foreach (var zone in model.Zones)
                {
                    var obj = new vm_Zone
                    {
                        Id = Guid.NewGuid(),
                        vm_ZoneName = zone.Name,
                        vm_ZoneCode = zone.Code,
                        vm_Building = new EntityReference(vm_Building.EntityLogicalName, building.Id),
                        vm_ExcludeFromOfficeSelection = zone.IsExcludeFromOfficeSelection,
                    };

                    zones.Add(obj);
                }
            }

            var meetingAreas = new List<vm_MeetingArea>();
            if (model.MeetingAreas != null && model.MeetingAreas.Count > 0 && zones.Count > 0)
            {
                foreach (var meeting in model.MeetingAreas)
                {
                    var zoneId = zones.Where(x => x.vm_ZoneName == meeting.ZoneName).Select(z => z.vm_ZoneId).FirstOrDefault();

                    var obj = new vm_MeetingArea
                    {
                        Id = Guid.NewGuid(),
                        vm_MeetingAreaName = meeting.Name,
                        vm_MeetingAreaCode = meeting.Code,
                        vm_Building = new EntityReference(vm_Building.EntityLogicalName, building.Id),
                        vm_Zone = zoneId != null ? new EntityReference(vm_Zone.EntityLogicalName, zoneId.Value) : null,
                        vm_AppearInVisitRequests = meeting.IsAvailableForVisitRequests,
                    };

                    meetingAreas.Add(obj);
                }
            }

            var request = new OrganizationRequestCollection();
            request.Add(new CreateRequest { Target = building });

            request.AddRange(zones.Select(zone => new CreateRequest { Target = zone }));

            request.AddRange(meetingAreas.Select(meetingArea => new CreateRequest { Target = meetingArea }));


            var response = _buildingRepository.ExecuteTransaction(request);
            if (response != null)
            {
                if (zones.Count == 0 && meetingAreas.Count == 0)
                    message = "Building is created successfully";
                else if (zones.Count > 0 && meetingAreas.Count == 0)
                    message = "Building and Zones are created successfully";
                else
                    message = "Building, Zones, and Meeting Areas are created successfully";

                return new OperationResult<Guid> { Status = true, Message = message, Data = building.Id };
            }


            return new OperationResult<Guid> { Status = false, Message = "Failed to create building. Please try again" };
        }

        public OperationResult<Guid> UpdateBuildingExcludeFromOfficeStatus(BuildingStatusUpdateVM model)
        {
            var building = new vm_Building
            {
                Id = model.Id,
                vm_ExcludeFromOfficeSelection = !model.IsExcludeFromOfficeSelection,
            };

            var isUpdated = _buildingRepository.Update(building);

            if (isUpdated)
                return new OperationResult<Guid> { Status = true, Message = "Status updated successfully", Data = building.Id };

            return new OperationResult<Guid> { Status = false, Message = "Failed to update status", };
        }

        public OperationResult<Guid> UpdateBuildingDetails(UpdateBuildingDetails model)
        {
            var building = new vm_Building
            {
                Id = model.Id,
                vm_BuildingCode = model.Code,
                vm_BuildingName = model.Name,
                vm_Address = model.Address,
                vm_LocationLink = model.LocationLink,
                vm_ContactPerson = model.ContactPersonId != null ? new EntityReference(vm_organizationuser.EntityLogicalName, model.ContactPersonId.Value) : null,
            };

            var isUpdated = _buildingRepository.Update(building);
            if (isUpdated)
                return new OperationResult<Guid> { Status = true, Message = "Building details updated successfully", Data = building.Id };

            return new OperationResult<Guid> { Status = false, Message = "Failed to update building details", };

        }
    }
}
