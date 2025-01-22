using DBConstants;
using Entities.RefData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RW4Entities;
using System;
using System.Collections.Concurrent;
using System.Data;
using System.Reflection;
using System.Threading;
using Utility.DataSetManagement;

namespace ServiceManagerRW4
{
    

   
    public interface IServiceThread
    {
        long ThreadId { get; set; }
        Task InvokeThreadAsync(CancellationToken cancellationToken);
    }

   
    public interface IServiceManager
    {
        Task<bool> InvokeServiceAsync(string serviceCode, CancellationToken cancellationToken);
    }

   
   


 


public class ServiceManager : IServiceManager
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ServiceManager> _logger;
        private readonly List<IServiceThread> _serviceThreads;
        private readonly IConfiguration _configuration;

        public List<long> ThreadIds = new List<long>();
        private readonly string _assemblyPath;

        ServiceManagerDBHelper _dbHelper;

        public ServiceManager(IServiceProvider serviceProvider, ILogger<ServiceManager> logger, IConfiguration configuration, ServiceManagerDBHelper serviceManagerDBHelper)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _serviceThreads = new List<IServiceThread>();
            _configuration = configuration;
            _dbHelper = serviceManagerDBHelper;
            _assemblyPath = _configuration["appSettings:AssemblyPath"];
        }

        public async Task<bool> InvokeServiceAsync(string serviceCode, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Service Invoked for {ServiceCode}", serviceCode);

            try
            {
                if (await LoadThreadsAsync(serviceCode))
                {
                    _logger.LogInformation("Threads Loaded for {ServiceCode}", serviceCode);

                    foreach (var thread in ThreadIds)
                    {
                        _ = Task.Run(() => InvokeThreadAsync(thread,cancellationToken), cancellationToken);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error invoking service {ServiceCode}", serviceCode);
                return false;
            }
        }

        private async Task<bool> LoadThreadsAsync(string serviceCode)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var serviceDB = scope.ServiceProvider.GetRequiredService<RWServiceManagerEntities>();

                var serviceDefinition =  serviceDB.GetServiceDefinition(serviceCode);

                if (serviceDefinition.Tables.Contains("RG_SysService") && serviceDefinition.Tables["RG_SysService"].Rows.Count > 0)
                {
                    if (serviceDefinition.Tables.Contains("RG_SysServiceThreads"))
                    {
                        var threadsTable = serviceDefinition.Tables["RG_SysServiceThreads"];

                        _serviceThreads.Clear();
                        foreach (DataRow threadRow in threadsTable.Rows)
                        {
                            var threadId = Convert.ToInt64(threadRow["SysServiceThreadId"]);


                            ThreadIds.Add(threadId);

                           
                        }

                        return true;
                    }
                }

                return false;
            }
        }

        public async Task InvokeThreadAsync(long ThreadId,CancellationToken cancellationToken)
        {
            _logger.LogInformation("Invoking thread {ThreadId}", ThreadId);

            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var serviceDB = scope.ServiceProvider.GetRequiredService<RWServiceManagerEntities>();

                    var threadConfig = await serviceDB.GetThreadConfigurationAsync(ThreadId);

                    if (threadConfig.Rows.Count == 0)
                    {
                        _logger.LogWarning("No configuration found for thread {ThreadId}", ThreadId);
                        return;
                    }

                    var threadRow = threadConfig.Rows[0];
                    var sleepTime = Convert.ToInt32(threadRow["ThreadSleepTime"]);
                    var methodName = threadRow["MethodName"].ToString();

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        await ExecuteBusinessLogicAsync(methodName, ThreadId);
                        await Task.Delay(sleepTime * 1000, cancellationToken);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in thread {ThreadId}", ThreadId);
            }
        }

        private async Task ExecuteBusinessLogicAsync(string methodName,long ThreadId)
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

        private bool DoProcess(string methodName,long ThreadId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(methodName))
                {
                    throw new ArgumentException("Method name cannot be null or empty.", nameof(methodName));
                }

                var parts = methodName.Split('.');

                if (parts.Length != 3)
                {
                    throw new ArgumentException("Invalid method name format. Expected format: AssemblyName.ClassName.MethodName", nameof(methodName));
                }

                var assemblyName = parts[0];
                var className = parts[1];
                var method = parts[2];

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

                var methodInfo = type.GetMethod(method);
                if (methodInfo == null)
                {
                    throw new MissingMethodException($"Method not found: {method} in type {assemblyName}.{className}");
                }

                var instance = Activator.CreateInstance(type);
                var result = methodInfo.Invoke(instance, new object[] { ThreadId });

                return result is bool success && success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing method {MethodName}", methodName);
                return false;
            }
        }
    }




}
