using Microsoft.Extensions.DependencyInjection;
using ServiceManagerRW4;
using System.Threading;
using static System.Formats.Asn1.AsnWriter;

namespace RW4OBDistributorSvc
{
    public class RWOBDistributorSvc : BackgroundService
    {
        private readonly ILogger<RWOBDistributorSvc> _logger; 
        private readonly IConfiguration _configuration;
        private readonly ServiceManager oServiceManager;
     
        string? sServiceCode;

        public RWOBDistributorSvc(ServiceManager serviceManager,ILogger<RWOBDistributorSvc> logger, 
                                  IConfiguration configuration)
        {
            _logger = logger;
            oServiceManager = serviceManager;
            _configuration = configuration;
            
             
        }
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
           
            try
            {
                sServiceCode = _configuration["appSettings:ServiceCode"]?? "RW4OBDistributor";
                _logger.LogInformation("RWOBDistributorSvc has started");
               await base.StartAsync(cancellationToken);

            }
            catch (Exception ex)
            {
               
                _logger.LogError(ex.ToString());
            }

           
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogCritical("RWOBDistributorSvc has stopped");
            return base.StopAsync(cancellationToken);
        }
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
               
                if (!string.IsNullOrWhiteSpace(sServiceCode))
                {
                    await oServiceManager.InvokeServiceAsync(sServiceCode, cancellationToken);

                }
                await Task.Delay(1000, cancellationToken);
            }
        }
    }
}