using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Visitor_Management_Portal.ViewModels.Analysis;

namespace Visitor_Management_Portal.ViewModels.Analisis
{
    public class TotalVisitChartAnalysis
    {
        public DateTime VisitDate { get; set; }
        public int VisitsCount { get; set; }
    }
}