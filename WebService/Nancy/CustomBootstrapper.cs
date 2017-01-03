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

using Nancy;
using Nancy.TinyIoc;
using Nancy.Conventions;
using Newtonsoft.Json;
using System;
using System.IO;
using Nancy.Bootstrapper;

namespace AlarmWorkflow.BackendService.WebService.Nancy
{
    public class CustomBootstrapper : DefaultNancyBootstrapper
    {
        #region Fields

        private IServiceProvider _serviceProvider;
        private WebServiceConfiguration _configuration;

        #endregion

        #region Properties

        protected override IRootPathProvider RootPathProvider => new CustomRootPathProvider(_configuration);

        #endregion

        #region Constructors

        public CustomBootstrapper(IServiceProvider serviceProvider, WebServiceConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;

            StaticConfiguration.DisableErrorTraces = false;
        }

        #endregion

        #region Methods

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            pipelines.AfterRequest += (ctx) => 
            {
                if (ctx.Response.ContentType == "text/html")
                    ctx.Response.ContentType = "text/html; charset=utf-8";
            };
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            container.Register(_serviceProvider);
            container.Register(_configuration);

            container.Register<JsonSerializer, CustomJsonSerializer>();
        }

        protected override void ConfigureConventions(NancyConventions conventions)
        {
            base.ConfigureConventions(conventions);

            string rootPath = RootPathProvider.GetRootPath();

            conventions.StaticContentsConventions.AddDirectory("assets", Path.Combine(rootPath, "assets"));
            conventions.StaticContentsConventions.AddDirectory("images", Path.Combine(rootPath, "images"));
            conventions.StaticContentsConventions.AddDirectory("modules", Path.Combine(rootPath, "bower_components"));
        }

        #endregion
    }
}
