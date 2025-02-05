using System.Collections.Generic;
using CreditRiskAnalysisApp.Models;
using X.PagedList;

namespace CreditRiskAnalysisApp.Models
{
    public class ReportViewModel
    {
        public List<CompanyPrediction> TopBestCompanies { get; set; }
        public List<CompanyPrediction> TopWorstCompanies { get; set; }
        public string SelectedRisk { get; set; } // To track selected risk category

        public IPagedList<CompanyPrediction> FilteredCompanies { get; set; }  // Enable paging for this property
    }
}
