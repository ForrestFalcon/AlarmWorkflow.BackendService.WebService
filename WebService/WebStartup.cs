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

using AlarmWorkflow.BackendService.WebService.Nancy;
using AlarmWorkflow.Shared.Core;
using Owin;
using System;

namespace AlarmWorkflow.BackendService.WebService
{
    internal class WebStartup
    {
        #region Constants

        private IServiceProvider _serviceProvider;
        private WebServiceConfiguration _configuration;

        #endregion

        #region Methods

        public WebStartup(IServiceProvider serviceProvider, WebServiceConfiguration configuration)
        {
            Assertions.AssertNotNull(serviceProvider, "serviceProvider");
            Assertions.AssertNotNull(configuration, "serviceProvider");

            this._serviceProvider = serviceProvider;
            this._configuration = configuration;
        }

        /// <summary>
        /// This code configures Web API. The Startup class is specified as a type
        /// parameter in the WebApp.Start method.
        /// </summary>
        /// <param name="app"></param>
        public void Configuration(IAppBuilder app)
        {
            app.UseNancy(options =>
            {
                options.Bootstrapper = new CustomBootstrapper(_serviceProvider, _configuration);
            });
        }

        #endregion
    }
}
