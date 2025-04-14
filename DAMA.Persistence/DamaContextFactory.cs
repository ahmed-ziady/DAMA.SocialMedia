using DAMA.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace DAMA.Persistence // ✅ Ensure correct namespace
{
    public class DamaContextFactory : IDesignTimeDbContextFactory<DamaContext>
    {
        public DamaContext CreateDbContext(string[] args)
        {


            // ✅ Load configuration from appsettings.json
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // Ensure correct path
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // Load config
                .AddEnvironmentVariables() // ✅ Allow connection string from environment variables
                .Build();

            // ✅ Get connection string (fallback if missing)
            string connectionString = configuration.GetConnectionString("DefaultConnection") ??
                                      throw new InvalidOperationException("Database connection string is missing!");

            // ✅ Set up DbContext options
            var optionsBuilder = new DbContextOptionsBuilder<DamaContext>();
            optionsBuilder.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(); // ✅ Enable automatic retries
            });

            // ✅ Return new instance of DamaContext
            return new DamaContext(optionsBuilder.Options);
        }
    }
}
