using ServiceManagerRW4;
using RW4Entities;
using Microsoft.EntityFrameworkCore;
using RW4BNSFTelemetry;
using System.Diagnostics;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostcontext, config) =>
    {
        #if DEBUG   
        config.AddJsonFile($"appsettings.Development.json", optional: true, reloadOnChange:true);
        #else
            if (args.Length==0)
	        {
                throw new ArgumentException("Environment name argument is required");
	        }
           var environmentName = args[0];  // While creating the windows service pass the environment name as parameter                 
           config.AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true);                      
        #endif
        config.Build();
    })
    .ConfigureLogging((hostcontext, logger) =>
    {
        #if DEBUG
                logger.ClearProviders().AddConsole().AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning); 
        #else
        #pragma warning disable CA1416 // Validate platform compatibility 
                logger.ClearProviders()
                       .AddEventLog(c =>
                       {
                           c.SourceName = "RW4BNSFTelemetrySvc";
                           c.LogName = "RW4";
                           if (!EventLog.SourceExists("RW4BNSFTelemetrySvc"))
                           {
                               EventLog.CreateEventSource("RW4BNSFTelemetrySvc", "RW4");
                           }


                       }).AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning); 
#pragma warning restore CA1416
#endif

    })
    .ConfigureServices((context,services )=>
    {
        #if DEBUG
                services.AddHostedService<RW4BNSFTelemetrySvc>();
#else
                services.AddWindowsService(options =>
                {
                    options.ServiceName = "RW4BNSFTelemetry";
                });
                services.AddHostedService<RW4BNSFTelemetrySvc>();
#endif


        services.AddSingleton<ServiceManager>();
        services.AddSingleton<ServiceManagerDBHelper>();
        services.AddSingleton<DBHelper>();       

        services.AddScoped<RWWamTelemetryData>();

        IConfiguration configuration = context.Configuration;
       
        string? sConnectString = configuration["appSettings:DBConnectionName"];
      
        services.AddDbContext<RWBNSFTelemetryEntities>(options =>
                       options.UseSqlServer(sConnectString).EnableSensitiveDataLogging(false));

        services.AddDbContext<RWServiceManagerEntities>(options =>
                      options.UseSqlServer(sConnectString));



    })
    .Build();

#if DEBUG
host.RunAsync().GetAwaiter().GetResult();
#else
await host.RunAsync();
#endif
