using DBConstants;
using Entities.RefData;
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





    public interface IServiceManager
    {
        Task InvokeServiceAsync(CancellationToken cancellationToken);
    }








    public class ServiceManager : IServiceManager
    {

        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        private ConcurrentBag<long> ThreadIds = new ConcurrentBag<long>();
        private readonly string _assemblyPath;
        ServiceManagerDBHelper _dbHelper;
        private readonly string _serviceCode;

        public ServiceManager(ILogger<ServiceManager> logger, IConfiguration configuration, IServiceProvider serviceProvider, ServiceManagerDBHelper serviceManagerDBHelper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
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
                    // TODO remove hardcoded value
                    ThreadIds.Add(419);
                    //foreach (var threadId in threadIds) { ThreadIds.Add(threadId); }
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return false;
            }

        }

        public async Task InvokeThreadAsync(long ThreadId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Invoking thread {ThreadId}", ThreadId);

            try
            {

                SyServiceThreadConfiguration syServiceThreadConfiguration = _dbHelper.GetActiveThreadDetails(ThreadId);

                await ExecuteBusinessLogicAsync(syServiceThreadConfiguration.AssemblyFullName, ThreadId);

                await Task.Delay(syServiceThreadConfiguration.ThreadSleepTm * 1000, cancellationToken);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in thread {ThreadId}", ThreadId);
            }
        }

        private async Task ExecuteBusinessLogicAsync(string methodName, long ThreadId)
        {
            try
            {
                if (DoProcess(methodName, ThreadId))
                {
                    await Task.CompletedTask;
                }
                else
                {
                    throw new InvalidOperationException($"Execution failed for method {methodName}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing business logic for method {MethodName}", methodName);
                throw;
            }
        }

        private bool DoProcess(string AssemblyFullName, long ThreadId)
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

                var assembly = Assembly.LoadFrom(assemblyPath);
                var type = assembly.GetType($"{assemblyName}.{className}");
                if (type == null)
                {
                    throw new TypeLoadException($"Type not found: {assemblyName}.{className}");
                }
                Type Type = assembly.GetType(assemblyName + "." + className);
                ConstructorInfo constructor = Type.GetConstructor(new[] { typeof(IConfiguration), typeof(IServiceProvider), typeof(ILogger) });

                object cls = constructor.Invoke(new object[] { _configuration, _serviceProvider, _logger });


                var methodInfo = cls.GetType().GetMethod(methodName);
                if (methodInfo == null)
                {
                    throw new MissingMethodException($"Method not found: {methodName} in type {assemblyName}.{className}");
                }


                var result = methodInfo.Invoke(cls, new object[] { ThreadId });

                return result is bool success && success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing method {AssemblyFullName}");
                return false;
            }
        }
    }




}
