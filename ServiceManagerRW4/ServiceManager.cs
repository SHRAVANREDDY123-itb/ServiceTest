using DBConstants;
using Entities.RefData;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RW4Entities;
using ServiceManagerRW4.Models;
using System;
using System.Collections.Concurrent;
using System.Data;
using System.Reflection;
using System.Threading;
using Utility.DataSetManagement;

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
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return false;
            }

        }

        private async Task InvokeThreadAsync(long ThreadId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Invoking thread {ThreadId}", ThreadId);

            try
            {

                SyServiceThreadConfiguration syServiceThreadConfiguration = _dbHelper.GetActiveThreadDetails(ThreadId);

                await InvokeAssembly(syServiceThreadConfiguration.AssemblyFullName, ThreadId, cancellationToken);

                await Task.Delay(syServiceThreadConfiguration.ThreadSleepTm * 1000, cancellationToken);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in thread {ThreadId}", ThreadId);
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
