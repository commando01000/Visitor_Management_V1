using CrmEarlyBound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Visitor_Management_Portal.BLL.Interfaces;
using Visitor_Management_Portal.Utilities;
using Visitor_Management_Portal.DAL.Repository.OrganizationSetupRepository;
using Visitor_Management_Portal.Models;
using Visitor_Management_Portal.ViewModels.OrganizationSetup;

namespace Visitor_Management_Portal.BLL.Services
{
    public class ZoneService : IZoneService
    {
        private readonly IZoneRepository _zoneRepository;
        private readonly IBuildingService _buildingService;
        private readonly Lazy<IMeetingAreaService> _meetingAreaService;

        public ZoneService(IZoneRepository zoneRepository,
            IBuildingService buildingService, Lazy<IMeetingAreaService> meetingAreaService)
        {
            _zoneRepository = zoneRepository;
            _buildingService = buildingService;
            _meetingAreaService = meetingAreaService;
        }

        public OperationResult AddZone(AddZoneVM addZoneVM)
        {
            try
            {
                var zone = AddZoneVM.MapToEntity(addZoneVM);

                var addedZoenId = _zoneRepository.Create(zone);
                if (addedZoenId == null || addedZoenId == Guid.Empty)
                {
                    return new OperationResult
                    {
                        Status = false,
                        Message = "Error adding zone, try again later"
                    };
                }

                return new OperationResult
                {
                    Status = true,
                    Message = "Zone added successfully",
                    Id = addedZoenId
                };
            }
            catch (Exception ex)
            {
                return new OperationResult
                {
                    Status = false,
                    Message = "Error adding zone, try again later"
                };
            }
        }

        public OperationResult DeleteZone(Guid zoneId)
        {
            try
            {
                var isDeleted = _zoneRepository.Delete(zoneId);
                if (isDeleted)
                {
                    return new OperationResult
                    {
                        Status = true,
                        Message = "Zone deleted successfully"
                    };
                }
                else
                {
                    return new OperationResult
                    {
                        Status = false,
                        Message = "Error deleting zone, try again later"
                    };
                }
            }
            catch (Exception ex)
            {
                return new OperationResult
                {
                    Status = false,
                    Message = "Error deleting zone, try again later"
                };
            }
        }

        public OperationResult EditZone(EditZoneVM editZoneVM)
        {
            try
            {
                var zone = _zoneRepository.Get(editZoneVM.Id);
                if (zone == null)
                {
                    return new OperationResult
                    {
                        Status = false,
                        Message = "No Zone Found with this ID"
                    };
                }

                zone = EditZoneVM.MapToEntity(editZoneVM);
                zone.Id = editZoneVM.Id;

                var isUpdated = _zoneRepository.Update(zone);

                if (isUpdated)
                {
                    return new OperationResult
                    {
                        Status = true,
                        Message = "Zone updated successfully"
                    };
                }
                else
                {
                    return new OperationResult
                    {
                        Status = false,
                        Message = "Error updating zone, try again later"
                    };
                }
            }
            catch (Exception ex)
            {
                return new OperationResult
                {
                    Status = false,
                    Message = "Error updating zone, try again later"
                };
            }
        }

        public OperationResult<ZoneWithRelatedAreasVM> GetZoneFullDetails(Guid zoneId)
        {
            try
            {
                var zone = _zoneRepository.Get(zoneId);
                if (zone == null)
                {
                    return new OperationResult<ZoneWithRelatedAreasVM>
                    {
                        Status = false,
                        Message = "No Zone Found with this ID"
                    };
                }

                var zoneVM = ZoneVM.MapFromEntity(zone);

                var relatedMeetingAreasResult = _meetingAreaService.Value.GetMeetingAreasByZone(zoneId);
                if (!relatedMeetingAreasResult.Status)
                {
                    return new OperationResult<ZoneWithRelatedAreasVM>
                    {
                        Status = false,
                        Message = relatedMeetingAreasResult.Message
                    };
                }

                zoneVM.RelatedAreasCount = relatedMeetingAreasResult.Data.Count();

                return new OperationResult<ZoneWithRelatedAreasVM>
                {
                    Status = true,
                    Message = "Zone details fetched successfully",
                    Data = new ZoneWithRelatedAreasVM
                    {
                        Zone = zoneVM,
                        RelatedAreas = relatedMeetingAreasResult.Data
                    }
                };
            }
            catch (Exception ex)
            {
                return new OperationResult<ZoneWithRelatedAreasVM>
                {
                    Status = false,
                    Message = "Error while fetching Zone details, try again later"
                };
            }
        }

        public List<ZoneVM> GetZonesByBuildingId(Guid buildingId)
        {
            var fetchZonesByBuilingQuery = $@"
                                            <fetch>
                                                <entity name='vm_zone'>
                                                <filter>
                                                    <condition attribute='vm_building' operator='eq' value='{buildingId}' uitype='vm_building' />
                                                </filter>
                                                </entity>
                                            </fetch>";

            var zones = _zoneRepository.GetAll(fetchZonesByBuilingQuery);

            if (!zones.Any()) return new List<ZoneVM>();

            var zonesVMs = new List<ZoneVM>();

            foreach (vm_Zone zone in zones)
            {
                var zoneVM = ZoneVM.MapFromEntity(zone);
                zonesVMs.Add(zoneVM);
            }

            return zonesVMs;
        }

        public List<ZoneVM> GetZonesForUser()
        {
            var orgazinationId = ClaimsManager.GetOrganizationId();
            var buildingsIDs = _buildingService.GetAllByOrganization(orgazinationId)
                                .ToList()
                                .Select(b => b.Id)
                                .ToList();

            StringBuilder fetchXmlQuery = new StringBuilder(string.Format($@"
                 <fetch>
                   <entity name='vm_zone'>
                     <attribute name='vm_zoneid' />
                     <attribute name='vm_building' />
                     <attribute name='vm_zonename' />
                     <attribute name='vm_zonecode' />
                     <attribute name='vm_excludefromofficeselection' />
                     <filter>
                       <condition attribute='vm_building' operator='in'>
            "));

            foreach (var buildingId in buildingsIDs)
            {
                fetchXmlQuery.Append(string.Format($@"
                    <value>{buildingId}</value>
                "));
            }

            fetchXmlQuery.Append(string.Format($@"
                   </condition>
                   </filter>
                 </entity>
               </fetch>
            "));

            var zones = _zoneRepository.GetAll(fetchXmlQuery.ToString());

            if (!zones.Any()) return new List<ZoneVM>();

            var zonesVMs = new List<ZoneVM>();

            foreach (vm_Zone zone in zones)
            {
                var zoneVM = ZoneVM.MapFromEntity(zone);

                var relatedAreasCountFetchXmlQuery = $@"
                        <fetch>
                          <entity name='vm_meetingarea'>
                            <attribute name='vm_meetingareaid' />
                            <filter>
                              <condition attribute='vm_zone' operator='eq' value='{zone.Id}' uiname='Zone 1' uitype='vm_zone' />
                            </filter>
                          </entity>
                        </fetch>
                    ";

                var relatedAreasCount = _zoneRepository.GetAll(relatedAreasCountFetchXmlQuery);

                zoneVM.RelatedAreasCount = relatedAreasCount == null ||
                                           !relatedAreasCount.Any() ?
                                              0 : relatedAreasCount.Count();

                zonesVMs.Add(zoneVM);
            }

            return zonesVMs;
        }

        public OperationResult ToggleZoneExcludeFromOffice(Guid zoneId)
        {
            try
            {
                var zone = _zoneRepository.Get(zoneId);
                if (zone == null)
                {
                    return new OperationResult
                    {
                        Status = false,
                        Message = $"No Zone Found With this ID : {zoneId} "
                    };
                }

                zone.vm_ExcludeFromOfficeSelection = !zone.vm_ExcludeFromOfficeSelection;
                _zoneRepository.Update(zone);

                return new OperationResult
                {
                    Status = true,
                    Message = "Zone Updated Successfully"
                };
            }
            catch (Exception ex)
            {
                return new OperationResult
                {
                    Status = false,
                    Message = $"Error while updating Zone, try again later"
                };
            }
        }
    }
}