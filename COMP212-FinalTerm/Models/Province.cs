using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COMP229_FinalTerm.Models
{
    public class Province
    {
        public int ProvinceId { get; set; }
        public string? ProvinceName { get; set; }

        public string CountryName { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public virtual List<CasesReport> CasesReports { get; set; }
    }
}
