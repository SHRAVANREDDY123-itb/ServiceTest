using RW4OBDistributorSvc;
using ServiceManagerRW4;
using RW4Entities;
using Microsoft.EntityFrameworkCore;
using RW4OBDistributorProcess;
using System.Diagnostics;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostcontext, config) =>
    {
        var environmentName = args[0]; // While creating the windows service pass the environment name as parameter
        //  config.SetBasePath(Directory.GetCurrentDirectory());
        config.AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true);
        config.Build();
    })
    .ConfigureLogging((hostcontext, logger) =>
    {
        #pragma warning disable CA1416 // Validate platform compatibility 
        logger.ClearProviders()
               .AddEventLog(c =>
               {
                   c.SourceName = "RW4OBDistributorSvc";
                   c.LogName = "RW4";
                   if (!EventLog.SourceExists("RW4OBDistributorSvc"))
                   {
                       EventLog.CreateEventSource("RW4OBDistributorSvc", "RW4");
                   }


               });
        #pragma warning restore CA1416

    })
    .ConfigureServices((context,services )=>
    {
        services.AddWindowsService(options =>
        {
            options.ServiceName = "RW4OBDistributor";
        });
        services.AddHostedService<RWOBDistributorSvc>();
        services.AddSingleton<ServiceManager>();
        services.AddSingleton<ServiceThread>();
        services.AddSingleton<ServiceDB>();

        IConfiguration configuration = context.Configuration;
        string? DBConnectionName = configuration["appSettings:DBConnectionName"];
        string? sConnectString = configuration["appSettings:" + DBConnectionName];
        DbContextOptionsBuilder<RWOBDistributorsEntities> dbContextOptionsBuilderobdb = new DbContextOptionsBuilder<RWOBDistributorsEntities>();
        dbContextOptionsBuilderobdb.UseSqlServer(sConnectString);

        DbContextOptionsBuilder<RWServiceManagerEntities> dbContextOptionsBuilderdb = new DbContextOptionsBuilder<RWServiceManagerEntities>();
        dbContextOptionsBuilderdb.UseSqlServer(sConnectString);


        services.AddSingleton(new SQLDBHelper( 
                              
                              new RWOBDistributorsEntities(dbContextOptionsBuilderobdb.Options),
                              new RWServiceManagerEntities(dbContextOptionsBuilderdb.Options)));



    })
    .Build();

await host.RunAsync();
