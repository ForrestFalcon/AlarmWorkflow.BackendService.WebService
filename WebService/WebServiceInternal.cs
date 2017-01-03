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


using AlarmWorkflow.Backend.ServiceContracts.Core;
using AlarmWorkflow.BackendService.WebService.Properties;
using AlarmWorkflow.BackendService.WebServiceContracts;
using AlarmWorkflow.Shared.Diagnostics;
using Microsoft.Owin.Hosting;
using System;
using System.IO;

namespace AlarmWorkflow.BackendService.WebService
{
    public class WebServiceInternal : InternalServiceBase, IWebServiceInternal
    {
        #region Fields

        private IDisposable _webService;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WebServiceInternal"/> class.
        /// </summary>
        public WebServiceInternal()
            : base()
        {

        }

        #endregion

        #region Methods

        /// <summary>
        /// Called when the parent service is iterating through all services to signal them they can start.
        /// Overridden to start the engine immediately.
        /// </summary>
        public override void OnStart()
        {
            base.OnStart();
            
            WebServiceConfiguration configuration = new WebServiceConfiguration(this.ServiceProvider);

            try
            {
                if (configuration.Active)
                {
                    if(!Directory.Exists(configuration.WebDirectory))
                    {
                        throw new DirectoryNotFoundException(Resources.WebDirectoryNotFound);
                    }

                    _webService = StartWebService(configuration);
                }
            }
            catch (Exception e)
            {
                Logger.Instance.LogFormat(LogType.Warning, this, Resources.WebServiceStartingError);
                Logger.Instance.LogException(this, e);
            }
        }

        /// <summary>
        /// Start the WebService
        /// </summary>
        private IDisposable StartWebService(WebServiceConfiguration configuration)
        {
            Logger.Instance.LogFormat(LogType.Info, this, Resources.WebServiceStarting, configuration.Port);
            WebStartup startup = new WebStartup(this.ServiceProvider, configuration);

            var options = new StartOptions()
            {
                ServerFactory = "Nowin",
                Port = configuration.Port
            };

            return WebApp.Start(options, startup.Configuration);
        }

        /// <summary>
        /// Overridden to stop the web service
        /// </summary>
        protected override void DisposeCore()
        {
            base.DisposeCore();

            if (_webService != null)
            {
                _webService.Dispose();
                _webService = null;
            }
        }

        #endregion
    }
}
