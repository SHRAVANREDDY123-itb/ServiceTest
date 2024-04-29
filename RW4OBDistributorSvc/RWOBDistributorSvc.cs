using ServiceManagerRW4;

namespace RW4OBDistributorSvc
{
    public class RWOBDistributorSvc : BackgroundService
    {
        private readonly ILogger<RWOBDistributorSvc> _logger;       
        private readonly ServiceManager oServiceManager;
        private readonly IConfiguration _configuration;

        public RWOBDistributorSvc(ILogger<RWOBDistributorSvc> logger, 
                                  IConfiguration configuration,  
                                  ServiceManager serviceManager)
        {
            _logger = logger;           
             oServiceManager = serviceManager;
            _configuration = configuration;
        }
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("OnStart");
            try
            {
                string sServiceCode = _configuration["appSettings:ServiceCode"];
              
                if (!string.IsNullOrWhiteSpace(sServiceCode))
                {
                    await oServiceManager.InvokeService(sServiceCode);

                }

            }
            catch (Exception ex)
            {
                // RWUtilities.Common.Log.write(ex);
                _logger.LogError(ex.ToString());
            }

           
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogCritical("RWOBDistributorSvc is stopping");
            return base.StopAsync(cancellationToken);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
             await Task.Delay(1000, stoppingToken);
            }
        }
    }
}