using System.Collections.Generic;
using CreditRiskAnalysisApp.Models;

namespace CreditRiskAnalysisApp.Models
{
    public class ReportViewModel
    {
        public List<CompanyPrediction> TopBestCompanies { get; set; }
        public List<CompanyPrediction> TopWorstCompanies { get; set; }
        public List<CompanyPrediction> FilteredCompanies { get; set; }
        public string SelectedRisk { get; set; } // To track selected risk category
    }
}
