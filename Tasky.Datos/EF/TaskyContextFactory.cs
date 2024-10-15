using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasky.Datos.EF
{
    public class TaskyContextFactory : IDesignTimeDbContextFactory<TaskyContext>
    {
        public TaskyContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TaskyContext>();
            optionsBuilder.UseSqlServer(
                "Server=INF-037\\SQLEXPRESS;Database=SmartTask;Trusted_Connection=True;");

            return new TaskyContext(optionsBuilder.Options);
        }
    }
}
