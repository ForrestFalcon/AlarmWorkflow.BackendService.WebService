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

using AlarmWorkflow.BackendService.WebService.Models;
using AlarmWorkflow.BackendService.WebService.Properties;
using AlarmWorkflow.Shared.Diagnostics;
using Ical.Net.Interfaces;
using Ical.Net.Interfaces.Components;
using Nancy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace AlarmWorkflow.BackendService.WebService.Modules
{
    public class WebCalendar : NancyModule
    {
        #region Fields

        private WebServiceConfiguration _configuration;

        #endregion

        #region Constructors

        public WebCalendar(WebServiceConfiguration configuration)
        {
            _configuration = configuration;

            Get["/api/calendar"] = parameter =>
            {
                List<Event> events = new List<Event>();
                if (_configuration.CalendarUrls != null)
                {
                    IEnumerable<IEvent> entries = getCalendarEntries();

                    entries = entries
                        .OrderBy(entry => entry.Start)
                        .Take(_configuration.CalendarEntries);

                    foreach (IEvent entry in entries)
                    {
                        events.Add(new Event(entry));
                    }
                }
                return Response.AsJson(events);
            };
        }

        #endregion

        #region Methods

        private IEnumerable<IEvent> getCalendarEntries()
        {
            foreach (string calendarUrl in _configuration.CalendarUrls)
            {
                Uri uriResult;
                if (Uri.TryCreate(calendarUrl, UriKind.Absolute, out uriResult))
                {
                    IICalendarCollection calendarCollection = LoadFromUri(uriResult);

                    if (calendarCollection != null)
                    {
                        foreach (IEvent webEvent in calendarCollection.First().Events)
                        {
                            if (webEvent.Start.Date >= DateTime.Now.Date)
                            {
                                yield return webEvent;
                            }
                        }
                    }
                }
            }
        }

        public IICalendarCollection LoadFromUri(Uri uri)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    using (var response = client.GetAsync(uri).Result)
                    {
                        response.EnsureSuccessStatusCode();
                        var result = response.Content.ReadAsStringAsync().Result;
                        return Ical.Net.Calendar.LoadFromStream(new StringReader(result)) as IICalendarCollection;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Instance.LogFormat(LogType.Warning, this, Resources.WebCalendarIcalError, uri.ToString());
                Logger.Instance.LogException(this, e);
                return null;
            }
        }

        #endregion
    }
}
