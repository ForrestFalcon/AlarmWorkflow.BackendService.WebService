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

namespace AlarmWorkflow.BackendService.WebService.Modules
{
    public class Operations : NancyModule
    {
        #region Fields

        private IEmkServiceInternal _emkService;
        private IOperationServiceInternal _operationService;
        private WebServiceConfiguration _configuration;

        #endregion

        #region Constructors

        public Operations(IServiceProvider serviceProvider, WebServiceConfiguration configuration)
        {
            _emkService = serviceProvider.GetService<IEmkServiceInternal>();
            _operationService = serviceProvider.GetService<IOperationServiceInternal>();
            _configuration = configuration;

            Get["/api/operation"] = parameter =>
            {
                var ops = _operationService.GetOperationIds(0, false, 0);
                return Response.AsJson(ops);
            };
            Get["/api/operation/latest"] = _ => Latest();
            Get["/api/operation/get/{id:int}"] = parameter => GetOp(parameter.id);
            Get["/api/operation/getFilteredResources/{id:int}"] = parameter => GetFilteredResources(parameter.id);
            Get["/api/operation/getAllResources"] = _ => GetAllResources();
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

        private Response Latest()
        {
            IList<int> ids = _operationService.GetOperationIds(_configuration.MaxAge, _configuration.NonAcknowledgedOnly, 1);
            if (ids.Count == 1)
            {
                Operation item = _operationService.GetOperationById(ids[0]);
                return Response.AsJson(item);
            }
            else
            {
                string ret = null;
                return Response.AsJson(ret);
            }
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
            IList<EmkResource> emkResources = _emkService.GetAllResources().ToList();
            return Response.AsJson(emkResources);
        }

        private Response GetFilteredResources(int id)
        {
            Operation operation = _operationService.GetOperationById(id);

            if (operation == null)
                return HttpStatusCode.NotFound;

            IList<EmkResource> emkResources = _emkService.GetAllResources().ToList();
            List<ResourceObject> filteredResources = new List<ResourceObject>();

            IList<OperationResource> filtered = _emkService.GetFilteredResources(operation.Resources).ToList();
            foreach (OperationResource resource in filtered)
            {
                EmkResource emk = emkResources.FirstOrDefault(item => item.IsActive && item.IsMatch(resource));
                filteredResources.Add(new ResourceObject(emk, resource));
            }

            return Response.AsJson(filteredResources);
        }
        
        #endregion
    }
}
