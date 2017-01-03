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

namespace AlarmWorkflow.BackendService.WebService.Nancy
{
    class CustomRootPathProvider : IRootPathProvider
    {
        #region Fields

        private WebServiceConfiguration _configuration;

        #endregion

        #region

        public CustomRootPathProvider(WebServiceConfiguration configuration)
        {
            _configuration = configuration;
        }

        #endregion

        #region Methods

        public string GetRootPath()
        {
            return _configuration.WebDirectory;
        }

        #endregion
    }
}
