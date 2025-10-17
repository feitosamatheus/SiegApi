using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sieg.Infrastructure.Contexts;

public class FactoryContext : IDesignTimeDbContextFactory<DatabaseContext>
{
    public DatabaseContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
        optionsBuilder.UseSqlServer("Server=sieg-database-desafio.cjywwqakqxcw.us-east-2.rds.amazonaws.com,1433;Database=SiegDB;User Id=admin;Password=74107410;TrustServerCertificate=True;");

        return new DatabaseContext(optionsBuilder.Options);
    }
}
