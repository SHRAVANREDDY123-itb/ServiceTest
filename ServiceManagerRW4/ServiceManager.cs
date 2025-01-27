using DBConstants;
using Entities.RefData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServiceManagerRW4.Models;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading;

namespace ServiceManagerRW4
{


    public class ServiceManager : IServiceManager
    {

        private readonly ILogger _logger;      
        private readonly IServiceProvider _serviceProvider;

        private ConcurrentBag<long> ThreadIds = new ConcurrentBag<long>();
        private readonly string _assemblyPath;
        ServiceManagerDBHelper _dbHelper;
        private readonly string _serviceCode;

        public ServiceManager(ILogger<ServiceManager> logger, IConfiguration configuration, IServiceProvider serviceProvider, ServiceManagerDBHelper serviceManagerDBHelper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));          
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _dbHelper = serviceManagerDBHelper ?? throw new ArgumentNullException(nameof(serviceManagerDBHelper));
            _assemblyPath = configuration["appSettings:AssemblyPath"]
                ?? throw new ArgumentException("AssemblyPath is not configured.", nameof(configuration));
            _serviceCode = configuration["appSettings:ServiceCode"]
                ?? throw new ArgumentException("ServiceCode is not configured.", nameof(configuration));
        }

        public async Task InvokeServiceAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Service Invoked for {ServiceCode}", _serviceCode);
            try
            {
                foreach (var thread in ThreadIds)
                {
                    await InvokeThreadAsync(thread, cancellationToken);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error invoking service {ServiceCode}", _serviceCode);

            }
        }

        public bool LoadThreads()
        {
            try
            {
                if (_dbHelper.CheckServiceAndThreadsExists(_serviceCode))
                {
                    List<SysServiceThread> sysServiceThreads = new List<SysServiceThread>();
                    var threadIds = _dbHelper.GetActiveThreads(_serviceCode);
                    _logger.LogInformation($"{string.Join(",", threadIds)} threads");
                  
                    foreach (var threadId in threadIds) { ThreadIds.Add(threadId); }
                    return true;
                }
                else
                { return false; }
               
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Loading the threads");
                return false;
            }

        }

        private async Task InvokeThreadAsync(long threadId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Invoking thread {ThreadId}", threadId);
            int? retryCount = 1; 

            try
            {
               
                SysServiceThread syServiceThreadConfiguration = _dbHelper.GetActiveThreadDetails(threadId);
                if (syServiceThreadConfiguration == null)
                {
                    _logger.LogWarning($"No thread found for thread ID = {threadId}");
                    return;
                }
                else
                {
                    syServiceThreadConfiguration.CurrentStatusCode = ServiceStatus.Running;
                    syServiceThreadConfiguration.CurrentProcessingStart = DateTime.UtcNow;
                    await _dbHelper.UpdateServiceThreadAsync(syServiceThreadConfiguration, cancellationToken);
                    retryCount = syServiceThreadConfiguration.RetryCount??1;
                    bool isSuccess = false;
                   
                    for (int i = 0; i < retryCount && !isSuccess; i++)
                    {
                        if (i > 0)
                        {
                            _logger.LogInformation("Retrying {RetryNumber} for Thread ID {ThreadId} after delay", i, threadId);
                            await Task.Delay(syServiceThreadConfiguration.SleepTime * 1000, cancellationToken);
                        }

                        try
                        {
                            isSuccess = await InvokeAssembly(syServiceThreadConfiguration.AssemblyFullName, threadId, cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Error during retry {RetryNumber} for Thread ID {ThreadId}", i, threadId);
                            await _dbHelper.InsertThreadException(new ThreadExceptionLog() { CreateDtTm = DateTime.UtcNow, SysServiceThreadId = threadId, ThreadException = ex.ToString() }, cancellationToken);
                        }
                    }
                   

                    syServiceThreadConfiguration.IsSuccessful = isSuccess==true?"Y":"N";
                    syServiceThreadConfiguration.LastStarted = syServiceThreadConfiguration.CurrentProcessingStart;
                    syServiceThreadConfiguration.LastStopped = DateTime.UtcNow;
                    syServiceThreadConfiguration.CurrentProcessingStart = null;
                    await _dbHelper.UpdateServiceThreadAsync(syServiceThreadConfiguration, cancellationToken);

                    if (!isSuccess)
                    {
                        await _dbHelper.InsertThreadException(new ThreadExceptionLog() { CreateDtTm = DateTime.UtcNow, SysServiceThreadId = threadId, ThreadException ="Invoking thread assembly was not succesfull" }, cancellationToken);
                    }

                    await Task.Delay(syServiceThreadConfiguration.SleepTime * 1000, cancellationToken);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in thread {ThreadId}", threadId);
                await _dbHelper.InsertThreadException(new ThreadExceptionLog() { CreateDtTm = DateTime.UtcNow, SysServiceThreadId = threadId, ThreadException = ex.ToString() },cancellationToken);
            }
        }       

        private async Task<bool> InvokeAssembly(string AssemblyFullName, long ThreadId, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(AssemblyFullName))
                {
                    throw new ArgumentException("Method name cannot be null or empty.", nameof(AssemblyFullName));
                }

                var parts = AssemblyFullName.Split('.');

                if (parts.Length != 3)
                {
                    throw new ArgumentException("Invalid method name format. Expected format: AssemblyName.ClassName.MethodName", nameof(AssemblyFullName));
                }

                var assemblyName = parts[0];
                var className = parts[1];
                var methodName = parts[2];

                var assemblyPath = Path.Combine(_assemblyPath, $"{assemblyName}.dll");
                if (!File.Exists(assemblyPath))
                {
                    throw new FileNotFoundException($"Assembly not found: {assemblyPath}");
                }

                var assembly = await Task.Run(() => Assembly.LoadFrom(assemblyPath));


                var type = assembly.GetType($"{assemblyName}.{className}");
                if (type == null)
                {
                    throw new TypeLoadException($"Type not found: {assemblyName}.{className}");
                }



                using (var scope = _serviceProvider.CreateScope())
                {
                    var scopedProvider = scope.ServiceProvider;
                    var service = scopedProvider.GetService(type);
                    var methodInfo = type.GetMethod(methodName);
                    if (methodInfo == null)
                    {
                        throw new MissingMethodException($"Method not found: {methodName} in type {assemblyName}.{className}");
                    }
                    await Task.Run(() =>
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                        }

                        methodInfo.Invoke(service, new object[] { ThreadId });
                    }, cancellationToken);

                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing method {AssemblyFullName}");
                return false;
            }
        }


    }




}
