using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tfl.Dynamic.Observation.Plugins.Common
{
    public abstract class PluginBase : IPlugin
    {
        public IPluginExecutionContext PluginExecutionContext { get; private set; }
        public IOrganizationServiceFactory ServiceFactory { get; private set; }
        public IOrganizationService OrganizationService { get; private set; }
        public IOrganizationService OrganizationServiceAsSystem { get; private set; }
        public ITracingService TracingService { get; private set; }
        public Entity Target { get; private set; }
        public string TracePrefix { get; private set; }
        protected abstract int PluginDepth { get; }

        private const int DO_NOT_CHECK_DEPTH = -1;

        public void Execute(IServiceProvider serviceProvider)
        {
            TracePrefix = this.GetType().Name;
            TracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            PluginExecutionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            //TraceHelper($"Plugin Assembly Version: {Assembly.GetEntryAssembly().GetName().Version}.");
            TraceHelper("Context Initiated");
            TraceHelper($"IPluginExecutionContext Depth is {PluginExecutionContext.Depth}. Required plugin depth for processing is {PluginDepth}.");
            ServiceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            OrganizationService = ServiceFactory.CreateOrganizationService(PluginExecutionContext.UserId);
            OrganizationServiceAsSystem = ServiceFactory.CreateOrganizationService(null);
            if (PluginExecutionContext.InputParameters.Contains("Target") && PluginExecutionContext.InputParameters["Target"] is Entity entity)
            {
                Target = entity;
            }
            TracingService.Trace($"Primary Entity Name:{Target.LogicalName}");
            TracingService.Trace($"Primary Entity Id:{Target.Id}");
            if (PluginExecutionContext.Depth <= PluginDepth || PluginExecutionContext.Depth == DO_NOT_CHECK_DEPTH)
            {
                ExecutePlgin();
            }
            TraceHelper("Context Completed");
        }

        public void TraceHelper(string message)
        {
            TracingService?.Trace($"{TracePrefix}:{message}");
        }

        public abstract void ExecutePlgin();
    }
}
