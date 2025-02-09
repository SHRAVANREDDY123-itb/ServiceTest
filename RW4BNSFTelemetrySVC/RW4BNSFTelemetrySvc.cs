using Microsoft.Extensions.DependencyInjection;
using ServiceManagerRW4;
using System.Threading;
using static System.Formats.Asn1.AsnWriter;

namespace RW4BNSFTelemetry
{
    public class RW4BNSFTelemetrySvc : BackgroundService
    {
        private readonly ILogger<RW4BNSFTelemetrySvc> _logger;
        private readonly ServiceManager oServiceManager;

        public RW4BNSFTelemetrySvc(ServiceManager serviceManager, ILogger<RW4BNSFTelemetrySvc> logger)
        {
            _logger = logger;
            oServiceManager = serviceManager;
        }
        public override async Task StartAsync(CancellationToken cancellationToken)
        {

            try
            {

                _logger.LogInformation("RW4BNSFTelemetry has started");
                if (oServiceManager.LoadThreads())
                {
                    await base.StartAsync(cancellationToken);
                }
                else
                {
                    _logger.LogError("RW4BNSFTelemetry couldnt load threads");
                }

            }
            catch (Exception ex)
            {

                _logger.LogError(ex.ToString());
            }


        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogCritical("RW4BNSFTelemetry has stopped");
            return base.StopAsync(cancellationToken);
        }
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await oServiceManager.InvokeServiceAsync(cancellationToken);
                
            }
        }
    }
}