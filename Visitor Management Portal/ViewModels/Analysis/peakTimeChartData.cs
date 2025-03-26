using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visitor_Management_Portal.ViewModels.Analysis
{
    public class peakTimeChartData
    {
       public List<peakTimeChartAnalysis> analyses = new List<peakTimeChartAnalysis>();
       public int MaxVisitors { get; set; }
       public string PeakTime {  get; set; }
    }
}
