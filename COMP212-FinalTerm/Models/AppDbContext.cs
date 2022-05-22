using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COMP229_FinalTerm.Models
{
    public class AppDbContext : DbContext
    {
        // ==================
        // ADD YOUR CODE HERE
        // ==================
        // Q1.2
        public DbSet<CasesReport> CasesReports  { get; set; }

        public DbSet<Province> Province { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB; Trusted_Connection=True;");
        }



        // ==================  
    }
}
