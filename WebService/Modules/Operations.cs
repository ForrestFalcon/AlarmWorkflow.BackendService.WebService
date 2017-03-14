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

using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.BackendService.ManagementContracts;
using Nancy;
using System;
using System.Collections.Generic;
using AlarmWorkflow.BackendService.ManagementContracts.Emk;
using System.Linq;
using AlarmWorkflow.BackendService.WebService.Models;
using AlarmWorkflow.BackendService.DispositioningContracts;

namespace AlarmWorkflow.BackendService.WebService.Modules
{
    public class Operations : NancyModule
    {
        #region Fields

        private readonly IEmkServiceInternal _emkService;
        private readonly IOperationServiceInternal _operationService;
        private readonly IDispositioningServiceInternal _disposingService;
        private readonly WebServiceConfiguration _configuration;

        #endregion

        #region Constructors

        public Operations(IServiceProvider serviceProvider, WebServiceConfiguration configuration)
        {
            _emkService = serviceProvider.GetService<IEmkServiceInternal>();
            _operationService = serviceProvider.GetService<IOperationServiceInternal>();
            _disposingService = serviceProvider.GetService<IDispositioningServiceInternal>();
            _configuration = configuration;

            Get["/api/operation"] = parameter =>
            {
                var ops = _operationService.GetOperationIds(0, false, 0);
                return Response.AsJson(ops);
            };

            Get["/api/operation/{id:int}"] = parameter => GetOp(parameter.id);
            Get["/api/operation/getResources/{id:int}"] = parameter => GetResources(parameter.id);
            Get["/api/operation/getResources"] = _ => GetAllResources();
        }

        #endregion

        #region Methods

        private Response GetOp(int id)
        {
            Operation item = _operationService.GetOperationById(id);
            if (item == null)
            {
                return HttpStatusCode.NotFound;
            }
            return Response.AsJson(item);
        }

        private Response ResetOperation(int operationId)
        {
            ResetOperationData result = new ResetOperationData();
            Operation item = _operationService.GetOperationById(operationId);
            if (item == null)
            {
                result.success = false;
                result.message = "Operation not found!";
            }
            else if (item.IsAcknowledged)
            {
                result.success = false;
                result.message = "Operation is already acknowledged!";
            }
            else
            {
                _operationService.AcknowledgeOperation(operationId);
                result.success = true;
                result.message = "Operation successfully acknowledged!";
            }

            return Response.AsJson(result);
        }
        
        private Response GetAllResources()
        {
            IList<EmkResource> emkResources = _emkService.GetAllResources().Where(item => item.IsActive).ToList();
            return Response.AsJson(emkResources);
        }

        private Response GetResources(int id)
        {
            Operation operation = _operationService.GetOperationById(id);

            if (operation == null)
                return HttpStatusCode.NotFound;

            IList<EmkResource> emkResources = _emkService.GetAllResources().Where(item => item.IsActive).ToList();

            IList<OperationResource> filtered = _emkService.GetFilteredResources(operation.Resources).ToList();
            string[] dispatchedResources = _disposingService.GetDispatchedResources(id);

            List<ResourceObject> filteredResources = new List<ResourceObject>();

            foreach(EmkResource emkResource in emkResources)
            {
                OperationResource operationResource = filtered.FirstOrDefault(item => emkResource.IsMatch(item));
                if (operationResource != null)
                {
                    filteredResources.Add(new ResourceObject(emkResource, operationResource, ResourceObject.ResourceType.Alarmed));
                }
                else if(dispatchedResources.FirstOrDefault(item => item == emkResource.Id) != null)
                {
                    filteredResources.Add(new ResourceObject(emkResource, operationResource, ResourceObject.ResourceType.Disposed));
                } 
                else
                {
                    filteredResources.Add(new ResourceObject(emkResource, operationResource, ResourceObject.ResourceType.None));
                }
            }
            

            return Response.AsJson(filteredResources);
        }
        
        #endregion
    }
}
