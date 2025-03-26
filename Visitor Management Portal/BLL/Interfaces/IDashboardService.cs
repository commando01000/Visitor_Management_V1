using System;
using System.Collections.Generic;
using Visitor_Management_Portal.ViewModels.Analisis;
using Visitor_Management_Portal.ViewModels.Analysis;

namespace Visitor_Management_Portal.BLL.Interfaces
{
    public interface IDashboardService
    {
        bool IsOrganizationSetupComplete();
        DashboardNumbers DashboardAnalysis();
        TotalVisistsChatData TotalVisistsChart(int Period, int weekNumber = 0);
        List<PurposeChart> PurposeChartAnalysis(int Period);
        peakTimeChartData PeakTimeChartAnalysis(int Period);
        Dictionary<string, int> TotalVisitsAnalysis(int Period);
        List<VisitDepartmentData> VisitByDepartmentAnalysis(int Period);
        VisitsByLocationsData VisitsByZoneAnalysis(int Period);
        Dictionary<string, string> PendingApprovalVisitRequestsList();
        List<TopVisitorsDataView> TopVisitorsList(int Period);
    }
}