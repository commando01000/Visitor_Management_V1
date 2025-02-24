using CrmEarlyBound;
using D365_Add_ons.Extensions;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Visitor_Management_Portal.BLL.Interfaces;
using Visitor_Management_Portal.Utilities;
using Visitor_Management_Portal.DAL.Repository.OrganizationSetupRepository;
using Visitor_Management_Portal.Models;
using Visitor_Management_Portal.ViewModels.OrganizationSetup;

namespace Visitor_Management_Portal.BLL.Ser
{
    public class MeetingAreaService : IMeetingAreaService
    {
        private readonly IMeetingAreaRepository _meetingAreaRepository;
        private readonly IBuildingService _buildingService;
        private readonly IZoneService _zoneService;

        public MeetingAreaService(IMeetingAreaRepository meetingAreaRepository, IBuildingService buildingService, IZoneService zoneService)
        {
            _meetingAreaRepository = meetingAreaRepository;
            _buildingService = buildingService;
            _zoneService = zoneService;
        }

        public List<MeetingAreaVM> GetMeetingAreasForUser()
        {
            var organizationId = ClaimsManager.GetOrganizationId();
            var buildingIds = _buildingService
                              .GetAllByOrganization(organizationId)
                              .Select(b => b.Id)
                              .ToList();

            if (!buildingIds.Any()) return new List<MeetingAreaVM>();

            StringBuilder fetchXml = new StringBuilder(string.Format(
                                         $@"<fetch>
                                          <entity name='vm_meetingarea'>
                                            <order attribute='vm_meetingareaname'/>
                                             <attribute name='vm_meetingareaid' />
                                                <attribute name='vm_appearinvisitrequests' />
                                                <attribute name='vm_building' />
                                                <attribute name='vm_meetingareacode' />
                                                <attribute name='vm_meetingareaname' />
                                                <attribute name='vm_zone' />
                                            <filter>
                                              <condition attribute='vm_building' operator='in'>"
                                        ));

            foreach (var buildingId in buildingIds)
            {
                fetchXml.AppendFormat($@"<value>{buildingId}</value>");
            }

            fetchXml.Append(string.Format($@"</condition>
                                            </filter>
                                          </entity>
                                        </fetch>"));

            var fetchExpression = fetchXml.ToString();
            var entities = _meetingAreaRepository.GetAll(fetchExpression);

            if (entities == null || !entities.Any()) return new List<MeetingAreaVM>();

            var meetingAreas = entities.Select(e => e.ToEntity<vm_MeetingArea>()).ToList();

            var meetingAreasVMs = new List<MeetingAreaVM>();

            foreach (var meetingArea in meetingAreas)
            {
                var meetingAreaVM = MeetingAreaVM.MapFromEntity(meetingArea);
                meetingAreasVMs.Add(meetingAreaVM);
            }

            return meetingAreasVMs;
        }

        public List<MeetingAreaVM> GetAllByBuilding(Guid buildingId)
        {
            var zones = _zoneService.GetZonesByBuildingId(buildingId);
            if (zones == null || !zones.Any())
                return new List<MeetingAreaVM>();

            List<Guid> zoneIds = zones.Select(zone => zone.Id).ToList();

            string queryConditions = $@"<condition attribute='vm_zone' operator='in' uiname='Selma Marks' uitype='vm_zone'>
                                        {string.Join("", zoneIds.Select(id => $"<value>{id}</value>"))}
                                      </condition>";

            string query = $@"<fetch>
                              <entity name='vm_meetingarea'>
                                <attribute name='vm_appearinvisitrequests' />
                                <attribute name='vm_meetingareacode' />
                                <attribute name='vm_meetingareaid' />
                                <attribute name='vm_meetingareaname' />
                                <attribute name='vm_zone' />
                                <filter>
                                {queryConditions}
                                </filter>
                              </entity>
                            </fetch>";

            var data = _meetingAreaRepository.GetAll(query);

            if(data == null || !data.Any())
                return new List<MeetingAreaVM>();

            List<MeetingAreaVM> meetingAreas = new List<MeetingAreaVM>();

            foreach (var area in data)
            {
                var obj = new MeetingAreaVM
                {
                    Id = area.Id,
                    Name = area.GetAttributeValue<string>("vm_meetingareaname"),
                    Code = area.GetAttributeValue<string>("vm_meetingareacode"),
                    AppearInVisitRequests = area.Check("vm_appearinvisitrequests") ? area.GetAttributeValue<bool>("vm_appearinvisitrequests") : false,
                    ZoneId = area.Check("vm_zone") ? area.GetEntityReferenceId("vm_zone").Value : Guid.Empty,
                    ZoneName = area.Check("vm_zone") ? area.GetAttributeValue<EntityReference>("vm_zone").Name : string.Empty,
                };

                meetingAreas.Add(obj);
            }

            return meetingAreas;
        }

        public OperationResult<Guid> AddMeetingArea(AddMeetingAreaVM addMeetingAreaVM)
        {
            try
            {
                vm_MeetingArea newMeetinArea = AddMeetingAreaVM.MapToEntity(addMeetingAreaVM);

                var createdMeetingAreaId = _meetingAreaRepository.Create(newMeetinArea);

                if (createdMeetingAreaId == null || createdMeetingAreaId == Guid.Empty)
                {
                    return new OperationResult<Guid>
                    {
                        Status = false,
                        Message = "Error adding meeting area, try again later"
                    };
                }

                return new OperationResult<Guid>
                {
                    Status = true,
                    Message = "Meeting area added successfully",
                    Data = createdMeetingAreaId
                };
            }
            catch (Exception ex)
            {
                return new OperationResult<Guid>
                {
                    Status = false,
                    Message = "Error adding meeting area, try again later"
                };
            }
        }

        public OperationResult ToggleMeetingAreaAvailability(Guid meetingAreaId)
        {
            try
            {
                var meetingArea = _meetingAreaRepository.Get(meetingAreaId);
                if (meetingArea == null)
                {
                    return new OperationResult
                    {
                        Status = false,
                        Message = "Meeting area not found"
                    };
                }

                meetingArea.vm_AppearInVisitRequests = !meetingArea.vm_AppearInVisitRequests;

                bool isUpdated = _meetingAreaRepository.Update(meetingArea);

                if (isUpdated)
                {
                    return new OperationResult
                    {
                        Status = true,
                        Message = "Meeting area availability toggled successfully"
                    };
                }
                else
                {
                    return new OperationResult
                    {
                        Status = false,
                        Message = "Error toggling meeting area availability, try again later"
                    };
                }
            }
            catch (Exception ex)
            {
                return new OperationResult
                {
                    Status = false,
                    Message = "Error toggling meeting area availability, try again later"
                };
            }
        }

        public OperationResult EditMeetingArea(EditMeetingAreaVM editMeetingAreaVM)
        {
            try
            {
                var meetingArea = _meetingAreaRepository.Get(editMeetingAreaVM.Id);
                if (meetingArea == null)
                {
                    return new OperationResult
                    {
                        Status = false,
                        Message = "Meeting area not found"
                    };
                }

                EditMeetingAreaVM.MapToEntity(meetingArea, editMeetingAreaVM);
                meetingArea.Id = editMeetingAreaVM.Id;

                var isUpdated = _meetingAreaRepository.Update(meetingArea);

                if (isUpdated)
                {
                    return new OperationResult
                    {
                        Status = true,
                        Message = "Meeting area updated successfully"
                    };
                }
                else
                {
                    return new OperationResult
                    {
                        Status = false,
                        Message = "Error updating meeting area, try again later"
                    };
                }
            }
            catch (Exception ex)
            {
                return new OperationResult
                {
                    Status = false,
                    Message = "Error updating meeting area, try again later"
                };
            }
        }

        public OperationResult DeleteMeetingArea(Guid areaId)
        {
            try
            {
                var meetingArea = _meetingAreaRepository.Get(areaId);
                if (meetingArea == null)
                {
                    return new OperationResult
                    {
                        Status = false,
                        Message = "Meeting area not found"
                    };
                }

                var isDeleted = _meetingAreaRepository.Delete(areaId);

                if (isDeleted)
                {
                    return new OperationResult
                    {
                        Status = true,
                        Message = "Meeting area deleted successfully"
                    };
                }
                else
                {
                    return new OperationResult
                    {
                        Status = false,
                        Message = "Error deleting meeting area, try again later"
                    };
                }
            }
            catch (Exception ex)
            {
                return new OperationResult
                {
                    Status = false,
                    Message = "Error deleting meeting area, try again later"
                };
            }
        }

        public OperationResult<List<MeetingAreaVM>> GetMeetingAreasByZone(Guid zoneId)
        {
            try
            {
                var fetchAreasXmlQuery =
                  $@"
                        <fetch>
                          <entity name='vm_meetingarea'>
                            <attribute name='vm_meetingareaid' />
                            <attribute name='vm_appearinvisitrequests' />
                            <attribute name='vm_building' />
                            <attribute name='vm_meetingareacode' />
                            <attribute name='vm_meetingareaname' />
                            <attribute name='vm_zone' />
                            <filter>
                              <condition attribute='vm_zone' operator='eq' value='{zoneId}' uitype='vm_zone' />
                            </filter>
                          </entity>
                        </fetch>
                  ";

                var relatedAreas = _meetingAreaRepository.GetAll(fetchAreasXmlQuery);

                if (relatedAreas == null || !relatedAreas.Any())
                {
                    return new OperationResult<List<MeetingAreaVM>>
                    {
                        Status = true,
                        Message = "No meeting areas found for this zone",
                        Data = new List<MeetingAreaVM>()
                    };
                }

                List<MeetingAreaVM> meetingAreasVMs = new List<MeetingAreaVM>();

                foreach (var area in relatedAreas)
                {
                    meetingAreasVMs.Add(MeetingAreaVM.MapFromEntity((vm_MeetingArea)area));
                }

                return new OperationResult<List<MeetingAreaVM>>
                {
                    Status = true,
                    Data = meetingAreasVMs,
                    Message = "Meeting areas found successfully"
                };

            }
            catch (Exception ex)
            {
                return new OperationResult<List<MeetingAreaVM>>
                {
                    Status = false,
                    Message = "Error getting meeting areas, try again later"
                };
            }
        }
    }
}