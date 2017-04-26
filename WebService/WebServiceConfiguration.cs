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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using AlarmWorkflow.BackendService.SettingsContracts;
using AlarmWorkflow.Shared;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Settings;
using AlarmWorkflow.Shared.Specialized;
using Microsoft.AspNet.SignalR;
using AlarmWorkflow.BackendService.WebService.Hubs;

namespace AlarmWorkflow.BackendService.WebService
{
    public sealed class WebServiceConfiguration : DisposableObject
    {
        #region Constants

        private const string Identifier = WebServiceSettingKeys.Identifier;

        #endregion

        #region Fields

        private ISettingsServiceInternal _settings;

        #endregion

        #region Properties

        internal bool Active => _settings.GetSetting(WebServiceSettingKeys.Active).GetValue<bool>();

        internal string WebDirectory => _settings.GetSetting(WebServiceSettingKeys.WebDirectory).GetValue<string>();

        internal int Port => _settings.GetSetting(WebServiceSettingKeys.Port).GetValue<int>();

        internal int MaxAge => _settings.GetSetting(WebServiceSettingKeys.MaxAge).GetValue<int>();

        internal int DwdId => _settings.GetSetting(WebServiceSettingKeys.DwdId).GetValue<int>(); 

        internal string[] CalendarUrls
        {
            get
            {
                var item = _settings.GetSetting(WebServiceSettingKeys.CalendarUrls);
                return item.Value == null ? null : item.GetStringArray();
            }
        }
        internal int CalendarEntries
        {
            get
            {
                int count = _settings.GetSetting(WebServiceSettingKeys.CalendarEntries).GetValue<int>();
                return count < 1 ? 1 : count;
            }
        }

        #endregion


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WebServiceConfiguration"/> class.
        /// </summary>
        /// <param name="serviceProvider"></param>
        public WebServiceConfiguration(IServiceProvider serviceProvider)
        {
            _settings = serviceProvider.GetService<ISettingsServiceInternal>();
            _settings.SettingChanged += _settings_SettingChanged;
        }

        #endregion

        #region Event-Handler

        private void _settings_SettingChanged(object sender, SettingChangedEventArgs e)
        {
            //reload changes per push (maybe with SignalR)
            IHubContext operationHub = GlobalHost.ConnectionManager.GetHubContext<OperationHub>();
            operationHub.Clients.All.settingsChanged();
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        protected override void DisposeCore()
        {
            _settings.SettingChanged -= _settings_SettingChanged;
            _settings = null;
        }

        #endregion
    }
}
