using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COMP229_FinalTerm.Models
{
    public class CasesReport
    {
        public int CasesReportId { get; set; }

        public DateTime date { get; set; }

        public int amount { get; set; }

        public virtual Province Province { get; set; }
    }
}
