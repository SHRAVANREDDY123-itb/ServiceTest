using Microsoft.Extensions.DependencyInjection;
using ServiceManagerRW4;
using System.Threading;
using static System.Formats.Asn1.AsnWriter;

namespace RW4OBDistributorSvc
{
    public class RWOBDistributorSvc : BackgroundService
    {
        private readonly ILogger<RWOBDistributorSvc> _logger;

        private readonly ServiceManager oServiceManager;



        public RWOBDistributorSvc(ServiceManager serviceManager, ILogger<RWOBDistributorSvc> logger
                                  )
        {
            _logger = logger;
            oServiceManager = serviceManager;



        }
        public override async Task StartAsync(CancellationToken cancellationToken)
        {

            try
            {

                _logger.LogInformation("RWOBDistributorSvc has started");
                if (oServiceManager.LoadThreads())
                {
                    await base.StartAsync(cancellationToken);
                }
                else
                {
                    _logger.LogError("RWOBDistributorSvc couldnt load threads");
                }

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


                await oServiceManager.InvokeServiceAsync(cancellationToken);


                await Task.Delay(1000, cancellationToken);
            }
        }
    }
}