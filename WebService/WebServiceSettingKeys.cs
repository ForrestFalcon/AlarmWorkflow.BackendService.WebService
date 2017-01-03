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

using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.BackendService.WebService
{
    static class WebServiceSettingKeys
    {
        #region Constants

        internal const string Identifier = "WebService";

        #endregion

        #region Properties

        internal static readonly SettingKey MapType = SettingKey.Create(Identifier, "MapType");
        internal static readonly SettingKey ZoomControl = SettingKey.Create(Identifier, "ZoomControl");
        internal static readonly SettingKey Route = SettingKey.Create(Identifier, "Route");
        internal static readonly SettingKey Tilt = SettingKey.Create(Identifier, "Tilt");
        internal static readonly SettingKey Traffic = SettingKey.Create(Identifier, "Traffic");
        internal static readonly SettingKey GoogleZoomLevel = SettingKey.Create(Identifier, "GoogleZoomLevel");
        internal static readonly SettingKey OSMZoomLevel = SettingKey.Create(Identifier, "OSMZoomLevel");

        internal static readonly SettingKey NonAcknowledgedOnly = SettingKey.Create(Identifier, "NonAcknowledgedOnly");
        internal static readonly SettingKey UpdateInterval = SettingKey.Create(Identifier, "UpdateInterval");
        internal static readonly SettingKey MaxAge = SettingKey.Create(Identifier, "MaxAge");
        internal static readonly SettingKey Port = SettingKey.Create(Identifier, "Port");
        internal static readonly SettingKey Active = SettingKey.Create(Identifier, "Active");
        internal static readonly SettingKey WebDirectory = SettingKey.Create(Identifier, "WebDirectory");

        internal static readonly SettingKey DwdId = SettingKey.Create(Identifier, "DwdId");

        internal static readonly SettingKey CalendarUrls = SettingKey.Create(Identifier, "CalendarUrls");
        internal static readonly SettingKey CalendarEntries = SettingKey.Create(Identifier, "CalendarEntries");

        #endregion
    }
}
