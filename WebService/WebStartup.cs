// This file is part of AlarmWorkflow.
// 
// AlarmWorkflow is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// AlarmWorkflow is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with AlarmWorkflow.  If not, see <http://www.gnu.org/licenses/>.

using AlarmWorkflow.BackendService.DispositioningContracts;
using AlarmWorkflow.BackendService.ManagementContracts;
using AlarmWorkflow.BackendService.WebService.Hubs;
using AlarmWorkflow.BackendService.WebService.Nancy;
using AlarmWorkflow.Shared.Core;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Owin;
using System;

namespace AlarmWorkflow.BackendService.WebService
{
    internal class WebStartup
    {
        #region Fields

        private IServiceProvider _serviceProvider;
        private WebServiceConfiguration _configuration;
        private IHubContext _operationHub;

        #endregion

        #region Methods

        public WebStartup(IServiceProvider serviceProvider, WebServiceConfiguration configuration)
        {
            Assertions.AssertNotNull(serviceProvider, "serviceProvider");
            Assertions.AssertNotNull(configuration, "serviceProvider");

            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _operationHub = GlobalHost.ConnectionManager.GetHubContext<OperationHub>();


            IDispositioningServiceInternal dispositioningService = serviceProvider.GetService<IDispositioningServiceInternal>();
            dispositioningService.Dispositioning += DispositioningService_Dispositioning;

            IOperationServiceInternal operationService = serviceProvider.GetService<IOperationServiceInternal>();
            operationService.NewOperation += OperationService_NewOperation;
            operationService.OperationAcknowledged += OperationService_OperationAcknowledged;
            
        }

        private void DispositioningService_Dispositioning(object sender, DispositionEventArgs e)
        {
            _operationHub.Clients.All.reloadResources(e.OperationId);
        }

        private void OperationService_OperationAcknowledged(int operationId)
        {
            _operationHub.Clients.All.resetOperation();
        }

        private void OperationService_NewOperation(Operation operation)
        {
            _operationHub.Clients.All.receiveOperation(operation);
        }
        

        /// <summary>
        /// This code configures Web API. The Startup class is specified as a type
        /// parameter in the WebApp.Start method.
        /// </summary>
        /// <param name="app"></param>
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();
            app.UseNancy(options =>
            {
                options.Bootstrapper = new CustomBootstrapper(_serviceProvider, _configuration);
            });
        }

        #endregion
    }
}
