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

using dwdWarnings;
using Nancy;
using System.Linq;
using System;

namespace AlarmWorkflow.BackendService.WebService.Modules
{
    public class DWD : NancyModule
    {
        #region Fields

        private WebServiceConfiguration _configuration;

        #endregion

        #region Constructors

        public DWD(WebServiceConfiguration configuration)
        {
            _configuration = configuration;

            Get["/api/dwd"] = _ =>
            {
                try
                {
                    var warnings = Warnings.GetWarningsById(_configuration.DwdId)
                        .Warnings
                        .OrderByDescending(warning => warning.Level)
                        .ThenBy(warning => warning.Start)
                        .FirstOrDefault();

                    return Response.AsJson(warnings);
                }
                catch (Exception)
                {
                    return HttpStatusCode.InternalServerError;
                }

            };
        }

        #endregion
    }
}
