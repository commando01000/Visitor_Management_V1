using CrmEarlyBound;
using Microsoft.Ajax.Utilities;
using Microsoft.Graph;
using Microsoft.Kiota.Abstractions;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web.Management;
using System.Windows.Forms;
using System.Windows.Markup;
using Visitor_Management_Portal.BLL.Interfaces;
using Visitor_Management_Portal.DAL.Repository.OrganizationDate;
using Visitor_Management_Portal.DAL.Repository.OrganizationUsersRepository;
using Visitor_Management_Portal.DAL.Repository.VisitorMemberHubRepository;
using Visitor_Management_Portal.DAL.Repository.VisitorsHubRepository;
using Visitor_Management_Portal.DAL.Repository.VisitRequestRepository;
using Visitor_Management_Portal.Utilities;
using Visitor_Management_Portal.ViewModels.Analisis;
using Visitor_Management_Portal.ViewModels.Analysis;
using Visitor_Management_Portal.ViewModels.VisitRequest;
using XDesk.Helpers;

namespace Visitor_Management_Portal.BLL.Services
{
    public class DashboardService : IDashboardService
    {
        #region start up
        private readonly IOrganizationDataRepository _organizationDataRepository;
        private readonly IVisitRequestRepository _visitRequestRepository;
        private readonly IVisitorsHubRepository _VisitorsHubRepository;
        private readonly IOrganizationUsersRepository _OrganizationUsersRepository;
        private readonly IVisitorMemberHubRepository _visitorMemberHubRepository;

        public DashboardService(IOrganizationDataRepository organizationDataRepository,
            IVisitRequestRepository visitRequestRepository,
            IVisitorsHubRepository visitorsHubRepository,
            IOrganizationUsersRepository organizationUsersRepository,
            IVisitorMemberHubRepository visitorMemberHubRepository)
        {
            _organizationDataRepository = organizationDataRepository;
            _visitRequestRepository = visitRequestRepository;
            _VisitorsHubRepository = visitorsHubRepository;
            _OrganizationUsersRepository = organizationUsersRepository;
            _visitorMemberHubRepository = visitorMemberHubRepository;
        }

        static string GetDayWithSuffix(DateTime date)
        {
            int day = date.Day;
            string suffix;

            switch (day)
            {
                case 1:
                case 21:
                case 31:
                    suffix = "st";
                    break;
                case 2:
                case 22:
                    suffix = "nd";
                    break;
                case 3:
                case 23:
                    suffix = "rd";
                    break;
                default:
                    suffix = "th";
                    break;
            }

            return $"{day}{suffix}";
        }

        static readonly Dictionary<int, string> selectedPeriodFetchXmlValue = new Dictionary<int, string>
        {
            {0, "today"},
            {1, "this-week"},
            {2, "this-month"},
            {3, "this-year"}
        };
        static readonly string[] Colors = { "#9FB9FF", "#3A5A92", "#628ACC", "#5B85FF", "#1B3565", "#C1D3FF", "#7D9FFF", "#E3ECFF" };
        static readonly string MaxColor = "#12204B";
        string FormatDateRange(DateTime startDate, DateTime endDate)
        {
            string startDay = GetDayWithSuffix(startDate);
            string endDay = GetDayWithSuffix(endDate);

            string startMonth = startDate.ToString("MMMM");
            string endMonth = endDate.ToString("MMMM");

            string formattedRange = startMonth == endMonth
                ? $"{startDay} - {endDay} {endMonth}"
                : $"{startDay} {startMonth} - {endDay} {endMonth}";

            return $"Date from {formattedRange}";
        }
        static readonly string NoZoneName = "No Zone";
        static readonly string NoMeetingAreaName = "No MeetingArea";
        #endregion

        private bool IsEmptyStateOn = false;

        public bool IsOrganizationSetupComplete()
        {
            // Steps :
            // 1/  Check if the org optional data is filled (Email , website , phone)
            // 2/  Check if the org users count is greater than 1
            // 3/ check if there at least one building in the organization

            // IF Any of the checks fails return False else return True

            var organizationId = ClaimsManager.GetOrganizationId();
            var organization = _organizationDataRepository.Get(organizationId);

            if (string.IsNullOrWhiteSpace(organization.vm_EmailAddress) ||
                     string.IsNullOrWhiteSpace(organization.vm_WebsiteURL) ||
                         string.IsNullOrWhiteSpace(organization.vm_PhoneNumber)) return false;


            string fetchUsersCountQuery = $@"
                        <fetch>
                          <entity name='vm_organizationuser'>
                            <attribute name='vm_organizationuserid' />
                            <filter>
                              <condition attribute='vm_organization' operator='eq' value='{organizationId}' uitype='vm_organization' />
                            </filter>
                          </entity>
                        </fetch>
                        ";

            var users = _organizationDataRepository.GetAll(fetchUsersCountQuery);
            if (users == null || users.Count() <= 1) return false;


            string fetchBuildingsCountQuery = $@"
                       <fetch>
                          <entity name='vm_building'>
                            <attribute name='vm_buildingid' />
                            <filter>
                              <condition attribute='vm_organization' operator='eq' value='{organizationId}' uitype='vm_organization' />
                            </filter>
                          </entity>
                        </fetch> ";

            var buildings = _organizationDataRepository.GetAll(fetchBuildingsCountQuery);
            if (buildings == null || buildings.Count() < 1) return false;

            return true;
        }

        public DashboardNumbers DashboardAnalysis()
        {
            var organizationId = ClaimsManager.GetOrganizationId();

            #region visit requests
            var visitsNumbers = TotalVisitsAnalysis(0);
            #endregion

            #region visitors
            string fetchvisitors = $@"
                                  <fetch>
                                      <entity name=""vm_visitor"">
                                        <attribute name=""vm_organizationref"" />
                                        <attribute name=""statecode"" />
                                        <filter>
                                          <condition attribute=""vm_organizationref"" operator=""eq"" value=""{organizationId}"" uiname=""BlueLink 1"" uitype=""vm_organization"" />
                                        </filter>
                                      </entity>
                                    </fetch>
                             ";

            var visitors = _VisitorsHubRepository.GetAll(fetchvisitors);

            var visitorsAnalysis = visitors != null && visitors.Count > 0 ? visitors.Select(e => e.ToEntity<vm_Visitor>()).Select(e => new VisitorAnalysis
            {
                VisitorID = e.Id,
                StateCode = e.StateCode.Value

            }).ToList() : new List<VisitorAnalysis>();
            #endregion

            #region Users
            string fetchUsers = $@"
                                 <fetch>
                                  <entity name=""vm_organizationuser"">
                                    <attribute name=""statecode"" />
                                    <attribute name=""vm_organizationuserid"" />
                                    <filter>
                                      <condition attribute=""vm_organization"" operator=""eq"" value=""{organizationId}"" uitype=""vm_organization"" />
                                    </filter>
                                  </entity>
                                </fetch>
                             ";

            var Users = _OrganizationUsersRepository.GetAll(fetchUsers);

            var usersAnalysis = Users != null && Users.Count > 0 ? Users.Select(e => e.ToEntity<vm_organizationuser>()).Select(e => new UserAnalysis
            {
                UserID = e.Id,
                StateCode = e.StateCode.Value

            }).ToList() : new List<UserAnalysis>();
            #endregion

            DashboardNumbers dashboardNumbers = new DashboardNumbers
            {
                Total_Visits = visitsNumbers["Totalvisits"],
                Active_Visits = visitsNumbers["ActiveVisits"],
                Active_Visitors = visitorsAnalysis.Where(e => e.StateCode == vm_VisitorState.Active).ToList().Count,
                Total_Users = usersAnalysis.Count,
                Active_Users = usersAnalysis.Where(e => e.StateCode == vm_organizationuserState.Active).ToList().Count
            };

            return dashboardNumbers;
        }
        public Dictionary<string, int> TotalVisitsAnalysis(int Period)
        {
            var organizationId = ClaimsManager.GetOrganizationId();

            #region visit requests
            string fetchVisitRequests = $@"
                                  <fetch>
                                      <entity name=""vm_visitrequest"">
                                        <attribute name=""statecode"" />
                                        <filter type=""and"">
                                          <condition entityname=""organizationUser"" attribute=""vm_organization"" operator=""eq"" value=""{organizationId}"" uitype=""vm_organization"" />
                                          <condition attribute=""createdon"" operator=""{selectedPeriodFetchXmlValue[Period > 3 || Period < 0 ? 0 : Period]}"" />
                                        </filter>
                                        <link-entity name=""vm_organizationuser"" from=""vm_organizationuserid"" to=""vm_requestedby"" link-type=""outer"" alias=""organizationUser"" />
                                      </entity>
                                    </fetch>
                             ";

            var visitRequests = _visitRequestRepository.GetAll(fetchVisitRequests);

            var visitRequestAnalysis = visitRequests != null && visitRequests.Count > 0 ?
                visitRequests.Select(e => e.ToEntity<vm_VisitRequest>())
                .Select(e => e.StateCode.Value).ToList() : new List<vm_VisitRequestState>();
            #endregion

            return new Dictionary<string, int>
            {
              {"Totalvisits" , visitRequestAnalysis.Count},
              {"ActiveVisits" , visitRequestAnalysis.Where(e => e == vm_VisitRequestState.Active).Count()}
            };
        }
        public TotalVisistsChatData TotalVisistsChart(int Period, int weekNumber = 0)
        {
            #region get data from crm
            var organizationId = ClaimsManager.GetOrganizationId();

            var periodCalc = Period > 2 || Period < 1 ? 1 : Period;

            string fetchVisitRequestsForTheLast_Period = $@"
                                    <fetch>
                                      <entity name=""vm_visitrequest"">
                                        <attribute name=""vm_visittime"" />
                                        <filter type=""and"">
                                          <condition attribute=""vm_visittime"" operator=""{selectedPeriodFetchXmlValue[periodCalc]}"" />
                                          <condition entityname=""organizationUser"" attribute=""vm_organization"" operator=""eq"" value=""{organizationId}"" uitype=""vm_organization"" />
                                        </filter>
                                        <link-entity name=""vm_organizationuser"" from=""vm_organizationuserid"" to=""vm_requestedby"" link-type=""inner"" alias=""organizationUser"" />
                                      </entity>
                                    </fetch>
                                    ";

            var visitRequestsData = _visitRequestRepository.GetAll(fetchVisitRequestsForTheLast_Period);
            #endregion
            #region filter data
            var visitRequestsGrouped = visitRequestsData == null || !visitRequestsData.Any() ?
                new List<TotalVisitChartAnalysis>() :
                visitRequestsData
                 .Select(e => (DateTime)e.ToEntity<vm_VisitRequest>().vm_VisitTime)
                 .GroupBy(e => e.Date)
                 .Select(g => new TotalVisitChartAnalysis
                 {
                     VisitDate = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day, 0, 0, 0),
                     VisitsCount = g.Count(),
                 })
                 .ToList();
            #endregion
            #region prepare data for the view
            var data = new TotalVisistsChatData();

            if (periodCalc == 1)
            {
                #region this week
                DateTime today = DateTime.Today;
                //int daysSinceMonday = (int)today.DayOfWeek - 1; // as Monday is always = 1

                //// If today is Monday, return today; otherwise, get the last Monday
                //DateTime lastMonday = today.AddDays(daysSinceMonday >= 0 ? -daysSinceMonday : -7 - daysSinceMonday);

                int daysSinceSunday = (int)today.DayOfWeek;

                DateTime lastSunday = today.AddDays(-daysSinceSunday);

                var startOfTheWeek = lastSunday;

                data.DateFromatedRange = FormatDateRange(startOfTheWeek, startOfTheWeek.AddDays(6));

                for (int i = 0; i < 7; i++)
                {
                    data.TotalVisitsCharts.Add(new TotalVisitsChart
                    {
                        TotalVisits = visitRequestsGrouped.FirstOrDefault(e => e.VisitDate.Date == startOfTheWeek.Date)?.VisitsCount ?? 0,
                        Name = ((DaysNameShort)startOfTheWeek.DayOfWeek).ToString()
                    });

                    startOfTheWeek = startOfTheWeek.AddDays(1);
                }

                #endregion
            }
            else if (periodCalc == 2 && weekNumber == 0)
            {
                #region this month
                DateTime today = DateTime.Today;
                DateTime firstDay = new DateTime(today.Year, today.Month, 1);
                int daysInMonth = DateTime.DaysInMonth(today.Year, today.Month);
                var lastDateInMonth = new DateTime(today.Year, today.Month, daysInMonth);
                DateTime endDay = firstDay.AddDays(6);
                int week = 1;

                data.DateFromatedRange = FormatDateRange(firstDay, lastDateInMonth);
                data.IsItMonth = true;

                while (firstDay <= lastDateInMonth)
                {
                    // Ensure endDay does not exceed the last day of the month
                    if (endDay > lastDateInMonth) endDay = lastDateInMonth;

                    data.TotalVisitsCharts.Add(new TotalVisitsChart
                    {
                        TotalVisits = visitRequestsGrouped.Where(e => e.VisitDate.Date >= firstDay.Date && e.VisitDate.Date <= endDay.Date).Sum(e => e.VisitsCount),
                        Name = $"W-{week++}"
                    });

                    // Move to the next week
                    firstDay = endDay.AddDays(1);
                    endDay = firstDay.AddDays(6);
                }
                #endregion
            }
            else if (periodCalc == 2 && weekNumber > 0)
            {
                #region week in this month
                DateTime today = DateTime.Today;
                DateTime firstDay = new DateTime(today.Year, today.Month, 1);
                int daysInMonth = DateTime.DaysInMonth(today.Year, today.Month);
                var startDayNumber = 1 + (7 * (weekNumber - 1));

                if (startDayNumber <= daysInMonth)
                {
                    firstDay = firstDay.AddDays(startDayNumber - 1);
                    var endDay = firstDay.AddDays(6);

                    data.DateFromatedRange = FormatDateRange(firstDay, endDay);
                    data.SetSpecialOption = true;
                    data.optionName = $"Week {weekNumber}";

                    while (firstDay <= endDay)
                    {
                        data.TotalVisitsCharts.Add(new TotalVisitsChart
                        {
                            TotalVisits = visitRequestsGrouped.FirstOrDefault(e => e.VisitDate.Date == firstDay.Date)?.VisitsCount ?? 0,
                            Name = ((DaysNameShort)firstDay.DayOfWeek).ToString()
                        });

                        firstDay = firstDay.AddDays(1);
                    }
                }
                #endregion
            }
            #region max period
            int maxVisits = data.TotalVisitsCharts.Select(v => v.TotalVisits).DefaultIfEmpty(0).Max();

            foreach (var chart in data.TotalVisitsCharts)
                chart.IsItLargestVisits = maxVisits == chart.TotalVisits;
            #endregion
            #region total visits
            data.TotoaVisitsPerPeriod = data.TotalVisitsCharts.Sum(e => e.TotalVisits);
            #endregion
            #endregion

            if (IsEmptyStateOn)
                return new TotalVisistsChatData();
            else
                return data;
        }
        public List<PurposeChart> PurposeChartAnalysis(int Period)
        {
            #region get data from crm
            var organizationId = ClaimsManager.GetOrganizationId();

            string fetchVisitRequestsFor_purpose = $@"
                                           <fetch>
                                              <entity name=""vm_visitrequest"">
                                                <attribute name=""vm_visitpurpose"" />
                                                <filter type=""and"">
                                                  <condition entityname=""organizationuser"" attribute=""vm_organization"" operator=""eq"" value=""{organizationId}"" uitype=""vm_organization"" />
                                                  <condition attribute=""createdon"" operator=""{selectedPeriodFetchXmlValue[Period > 3 || Period < 0 ? 0 : Period]}"" />
                                                </filter>
                                                <link-entity name=""vm_organizationuser"" from=""vm_organizationuserid"" to=""vm_requestedby"" link-type=""outer"" alias=""organizationuser"" />
                                              </entity>
                                            </fetch>
                                            ";

            var VisitRequestsFor_purposeData = _visitRequestRepository.GetAll(fetchVisitRequestsFor_purpose);
            #endregion
            #region filter data
            List<vm_VisitPurposes?> purposeAnalysis = VisitRequestsFor_purposeData != null && VisitRequestsFor_purposeData.Count > 0 ?
                VisitRequestsFor_purposeData.Select(e => e.ToEntity<vm_VisitRequest>()).
                Select(e => e.vm_VisitPurpose).ToList() : new List<vm_VisitPurposes?>();
            #endregion
            #region prepare data for the view
            var data = new List<PurposeChart>();

            int totalVisits = purposeAnalysis.Count;

            foreach (var visitPurpose in Enum.GetNames(typeof(vm_VisitPurposes)))
            {
                int visitCount = purposeAnalysis.Count(e => e.ToString() == visitPurpose);

                data.Add(new PurposeChart
                {
                    PurposeName = visitPurpose,
                    VisitsNumber = visitCount,
                    VisitsPercentage = totalVisits == 0 ? 0 : (int)(((float)visitCount / totalVisits) * 100)
                });
            }

            if (totalVisits != 0 && data.Sum(e => e.VisitsPercentage) != 100)
                data.First().VisitsPercentage +=
                    100 - data.Sum(e => e.VisitsPercentage);
            #endregion

            if (IsEmptyStateOn)
                return new List<PurposeChart>();
            else
                return data;
        }
        public peakTimeChartData PeakTimeChartAnalysis(int Period)
        {
            #region get data from crm
            var organizationId = ClaimsManager.GetOrganizationId();

            string fetchVisitRequests_peakTime = $@"
                                                   <fetch>
                                                      <entity name=""vm_visitrequest"">
                                                        <attribute name=""vm_visitorscount"" />
                                                        <attribute name=""vm_visittime"" />
                                                        <filter>
                                                          <condition attribute=""createdon"" operator=""{selectedPeriodFetchXmlValue[Period > 3 || Period < 0 ? 0 : Period]}"" />
                                                        </filter>
                                                        <link-entity name=""vm_organizationuser"" from=""vm_organizationuserid"" to=""vm_requestedby"" link-type=""outer"" alias=""orgusr"">
                                                          <attribute name=""vm_organization"" />
                                                          <filter>
                                                            <condition attribute=""vm_organization"" operator=""eq"" value=""{organizationId}"" uitype=""vm_organization"" />
                                                          </filter>
                                                        </link-entity>
                                                      </entity>
                                                    </fetch>
                                                    ";

            var VisitRequestsFor_peakTimeData = _visitRequestRepository.GetAll(fetchVisitRequests_peakTime);
            #endregion
            #region prepare data for the view
            var data = new peakTimeChartData();
            int WhichColorIndex = 0, LengthColorArray = Colors.Length;

            data.analyses = VisitRequestsFor_peakTimeData != null && VisitRequestsFor_peakTimeData.Count > 0 ?
                    VisitRequestsFor_peakTimeData
                    .Select(e => e.ToEntity<vm_VisitRequest>())
                    .Select(e => new
                    {
                        VisitorsCount = e.vm_VisitorsCount ?? 0,
                        VisitingTime = new DateTime(1, 1, 1, e.vm_VisitTime?.Hour ?? 0, 0, 0)
                    })
                     .OrderBy(e => e.VisitingTime.Hour < 12 ? 0 : 1)
                     .ThenBy(e => e.VisitingTime.Hour)
                    .GroupBy(e => e.VisitingTime)
                    .Select(g => new peakTimeChartAnalysis
                    {
                        VisitingTime = g.Key.ToString("htt").ToLower().Replace("m", ""),
                        VisitorsCount = g.Sum(e => e.VisitorsCount),
                        WhichColor = Colors[WhichColorIndex++ % LengthColorArray]
                    })
                    .ToList()
                : new List<peakTimeChartAnalysis>();

            #region peak hour

            if (data.analyses != null && data.analyses.Any())
            {
                var peakHour = data.analyses
              .OrderByDescending(e => e.VisitorsCount)
              .FirstOrDefault() ?? new peakTimeChartAnalysis();

                peakHour.WhichColor = MaxColor;
                data.MaxVisitors = peakHour.VisitorsCount;
                data.PeakTime = peakHour.VisitingTime.Replace("a", " AM").Replace("p", " PM").ToUpper();
            }
            #endregion
            #endregion

            if (IsEmptyStateOn)
                return new peakTimeChartData();
            else
                return data;
        }
        public List<VisitDepartmentData> VisitByDepartmentAnalysis(int Period)
        {
            #region get data from crm
            var organizationId = ClaimsManager.GetOrganizationId();

            string fetchVisitRequests = $@"
                                                        <fetch>
                                                          <entity name=""vm_visitrequest"">
                                                            <filter type=""and"">
                                                              <condition entityname=""organizationUser"" attribute=""vm_organization"" operator=""eq"" value=""{organizationId}"" uitype=""vm_organization"" />
                                                              <condition attribute=""createdon"" operator=""{selectedPeriodFetchXmlValue[Period > 3 || Period < 0 ? 0 : Period]}"" />
                                                            </filter>
                                                            <link-entity name=""vm_organizationuser"" from=""vm_organizationuserid"" to=""vm_requestedby"" link-type=""outer"" alias=""organizationUser"">
                                                              <attribute name=""vm_department"" />
                                                            </link-entity>
                                                          </entity>
                                                        </fetch>
                                                        ";

            var visitRequests = _visitRequestRepository.GetAll(fetchVisitRequests);
            #endregion
            #region filter data
            var departmentNames = visitRequests != null && visitRequests.Count > 0 ?
                visitRequests
                .Select(e =>
                e.Contains("organizationUser.vm_department") && e["organizationUser.vm_department"] is AliasedValue
                    ? (string)((AliasedValue)e["organizationUser.vm_department"]).Value
                    : null)
                .ToList()
                : new List<string>();
            #endregion
            #region prepare data for the view
            int WhichColorIndex = 0, LengthColorArray = Colors.Length;

            var data = departmentNames
                    .GroupBy(dept => dept)
                    .Select(g => new VisitDepartmentData
                    {
                        DepartmentName = g.Key ?? "Unknown",
                        Count = g.Count(),
                        color = Colors[WhichColorIndex++ % LengthColorArray]
                    })
                    .ToList();

            (data.OrderByDescending(e => e.Count).FirstOrDefault()
                ?? new VisitDepartmentData()).color = MaxColor;
            #endregion

            if (IsEmptyStateOn)
                return new List<VisitDepartmentData>();
            else
                return data;
        }
        public VisitsByLocationsData VisitsByZoneAnalysis(int Period)
        {
            #region get data from CRM
            var organizationId = ClaimsManager.GetOrganizationId();

            string fetchVisitRequests = $@"
                                        <fetch>
                                          <entity name=""vm_visitrequest"">
                                            <attribute name=""vm_location"" />
                                            <filter type=""and"">
                                              <condition attribute=""createdon"" operator=""{selectedPeriodFetchXmlValue[Period > 3 || Period < 0 ? 0 : Period]}"" />
                                              <condition entityname=""organizationUser"" attribute=""vm_organization"" operator=""eq"" value=""{organizationId}"" uitype=""vm_organization"" />
                                            </filter>
                                            <link-entity name=""vm_organizationuser"" from=""vm_organizationuserid"" to=""vm_requestedby"" link-type=""outer"" alias=""organizationUser"">
                                              <link-entity name=""vm_zone"" from=""vm_zoneid"" to=""vm_zone"" link-type=""outer"" alias=""OrgZone"">
                                                <attribute name=""vm_zonename"" />
                                              </link-entity>
                                            </link-entity>
                                            <link-entity name=""vm_meetingarea"" from=""vm_meetingareaid"" to=""vm_meetingarea"" link-type=""outer"" alias=""meetingArea"">
                                              <attribute name=""vm_meetingareaname"" />
                                              <link-entity name=""vm_zone"" from=""vm_zoneid"" to=""vm_zone"" link-type=""outer"" alias=""meetingZone"">
                                                <attribute name=""vm_zonename"" />
                                              </link-entity>
                                            </link-entity>
                                          </entity>
                                        </fetch>
                                        ";

            var visitRequests = _visitRequestRepository.GetAll(fetchVisitRequests);
            #endregion
            #region filter data according to visit location
            var visitsRequestsByOffice = visitRequests == null || !visitRequests.Any() ?
                 new Dictionary<string, int>() :
                 visitRequests
                 .Select(e => e.ToEntity<vm_VisitRequest>())
                 .Where(e => e.vm_Location.Value == vm_VisitRequest_vm_Location.Office)
                 .Select(e => e.GetAttributeValue<AliasedValue>("OrgZone.vm_zonename")?.Value as string ?? NoZoneName)
                 .GroupBy(zone => zone)
                 .ToDictionary(g => g.Key, g => g.Count());

            var visitsRequestsByMeetingArea = visitRequests == null || !visitRequests.Any() ?
                 new List<BusiestAreasData>() :
                 visitRequests
                 .Select(e => e.ToEntity<vm_VisitRequest>())
                 .Where(e => e.vm_Location.Value == vm_VisitRequest_vm_Location.MeetingArea)
                 .Select(e => new
                 {
                     zone_name = e.GetAttributeValue<AliasedValue>("meetingZone.vm_zonename")?.Value as string ?? NoZoneName,
                     meetingArea_name = e.GetAttributeValue<AliasedValue>("meetingArea.vm_meetingareaname")?.Value as string ?? NoMeetingAreaName
                 })
                 .ToList()
                 .GroupBy(zone => zone.zone_name)
                 .Select(key => new BusiestAreasData
                 {
                     MeetingAreaName = key.FirstOrDefault()?.meetingArea_name ?? NoMeetingAreaName,
                     Zone_Name = key.Key,
                     NumberVisitsPerZone = key.Count()
                 })
                 .OrderByDescending(e => e.NumberVisitsPerZone)
                 .ToList();
            #endregion
            #region prepare data for the view
            int WhichColorIndex = 0, LengthColorArray = Colors.Length;

            var data = new VisitsByLocationsData
            {
                BusiestAreas = visitsRequestsByMeetingArea.Take(3).ToList(),
                visitsByZones = visitsRequestsByMeetingArea.Select(
                    e => new VisitsByZoneData
                    {
                        Zone_Name = e.Zone_Name,
                        NumberVisitsPerZone = e.NumberVisitsPerZone + visitsRequestsByOffice.Pop(e.Zone_Name),
                        ZoneColumnColor = Colors[WhichColorIndex++ % LengthColorArray]
                    }).ToList()
            };

            foreach (var item in visitsRequestsByOffice)
                data.visitsByZones.Add(new VisitsByZoneData
                {
                    Zone_Name = item.Key,
                    NumberVisitsPerZone = item.Value,
                    ZoneColumnColor = Colors[WhichColorIndex++ % LengthColorArray]
                });

            (data.visitsByZones.OrderByDescending(e => e.NumberVisitsPerZone).FirstOrDefault()
                    ?? new VisitsByZoneData()).ZoneColumnColor = MaxColor;
            #endregion

            if (IsEmptyStateOn)
                return new VisitsByLocationsData();
            else
                return data;
        }
        public Dictionary<string, string> PendingApprovalVisitRequestsList()
        {
            #region get data from CRM
            var organizationId = ClaimsManager.GetOrganizationId();

            string fetchVisitRequests = $@"
                                        <fetch top=""5"">
                                          <entity name=""vm_visitrequest"">
                                            <attribute name=""vm_subject"" />
                                            <attribute name=""vm_visitrequestid"" />
                                            <filter type=""and"">
                                              <condition attribute=""statecode"" operator=""eq"" value=""0"" />
                                              <condition attribute=""statuscode"" operator=""eq"" value=""1"" />
                                              <condition entityname=""organizationUser"" attribute=""vm_organization"" operator=""eq"" value=""{organizationId}"" uitype=""vm_organization"" />
                                            </filter>
                                            <link-entity name=""vm_organizationuser"" from=""vm_organizationuserid"" to=""vm_requestedby"" link-type=""outer"" alias=""organizationUser"" />
                                          </entity>
                                        </fetch>
                                        ";

            var visitRequests = _visitRequestRepository.GetAll(fetchVisitRequests);
            #endregion
            #region prepare data for the view
            var data = visitRequests == null || !visitRequests.Any() ?
                new Dictionary<string, string>() :
                visitRequests
                .Select(e => e.ToEntity<vm_VisitRequest>())
                .ToDictionary(id => ((Guid)id.vm_VisitRequestId).ToString(), name => name.vm_Subject);
            #endregion

            if (IsEmptyStateOn)
                return new Dictionary<string, string>();
            else
                return data;
        }
        public List<TopVisitorsDataView> TopVisitorsList(int Period)
        {
            #region get data from CRM
            var organizationId = ClaimsManager.GetOrganizationId();

            string fetchVisitors = $@"
                                       <fetch>
                                          <entity name=""vm_visitingmember"">
                                            <filter type=""and"">
                                              <condition entityname=""visitor"" attribute=""vm_organizationref"" operator=""eq"" value=""{organizationId}"" uitype=""vm_organization"" />
                                              <condition entityname=""visitRequest"" attribute=""vm_visittime"" operator=""{selectedPeriodFetchXmlValue[Period > 3 || Period < 0 ? 0 : Period]}"" />
                                            </filter>
                                            <link-entity name=""vm_visitrequest"" from=""vm_visitrequestid"" to=""vm_visitrequest"" link-type=""inner"" alias=""visitRequest"" />
                                            <link-entity name=""vm_visitor"" from=""vm_visitorid"" to=""vm_visitor"" link-type=""inner"" alias=""visitor"">
                                              <attribute name=""vm_fullname"" />
                                              <attribute name=""vm_visitorid"" />
                                              <attribute name=""vm_organization"" />
                                              <link-entity name=""vm_organization"" from=""vm_organizationid"" to=""vm_organizationref"" link-type=""inner"" alias=""organization"" />
                                            </link-entity>
                                          </entity>
                                        </fetch>
                                        ";

            var visitors = _visitorMemberHubRepository.GetAll(fetchVisitors);
            #endregion
            #region prepare data for the view
            var data = visitors == null || !visitors.Any() ?
                new List<TopVisitorsDataView>()
                : visitors
                .Select(e =>
                {
                    var entity = e.ToEntity<vm_visitingmember>();
                    return new
                    {
                        visitorName = entity.GetAttributeValue<AliasedValue>("visitor.vm_fullname")?.Value as string ?? "No Name",
                        visitorID = entity.GetAttributeValue<AliasedValue>("visitor.vm_visitorid")?.Value as Guid?,
                        visitorComp = entity.GetAttributeValue<AliasedValue>("visitor.vm_organization")?.Value as string ?? "No Company"
                    };
                })
                .Where(e => e.visitorID != null)
                .GroupBy(k => k.visitorID)
                .Select(group => new TopVisitorsDataView
                {
                    Visitor_Name = group.FirstOrDefault()?.visitorName ?? "No Name",
                    NumberVisits = group.Count(),
                    Visitor_Comp = group.FirstOrDefault()?.visitorComp ?? "No Company"
                })
                .OrderByDescending(e => e.NumberVisits)
                .Take(5)
                .ToList();
            #endregion

            if (IsEmptyStateOn)
                return new List<TopVisitorsDataView>();
            else
                return data;
        }
    }
}
