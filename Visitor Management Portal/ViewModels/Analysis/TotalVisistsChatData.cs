using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Visitor_Management_Portal.ViewModels.Analysis;

namespace Visitor_Management_Portal.ViewModels.Analisis
{
    public class TotalVisistsChatData
    {
        public List<TotalVisitsChart> TotalVisitsCharts { get; set; } = new List<TotalVisitsChart>();
        public string DateFromatedRange { get; set; }
        public int TotoaVisitsPerPeriod {  get; set; }
        public bool IsItMonth {  get; set; } = false;
        public bool SetSpecialOption { get; set; } = false;
        public string optionName { get; set; }
    }
}