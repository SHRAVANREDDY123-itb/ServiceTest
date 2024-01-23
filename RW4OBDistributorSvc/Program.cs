using RW4OBDistributorSvc;
using Microsoft.Extensions.Configuration;
using ServiceManagerRW4;
using RW4Entities;
using Microsoft.EntityFrameworkCore;
using RW4OBDistributorProcess;
using Microsoft.Extensions.DependencyInjection;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostcontext, config) =>
    {
        var env = hostcontext.HostingEnvironment;

        config.SetBasePath(Directory.GetCurrentDirectory());      


        config.AddJsonFile("appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true); 
    })
    .ConfigureServices((context,services )=>
    {
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
