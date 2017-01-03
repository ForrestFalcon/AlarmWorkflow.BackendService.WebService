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
using Ical.Net.Interfaces.Components;
using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;

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

            Get["/calendar"] = parameter =>
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
                    var calendarCollection = Ical.Net.Calendar.LoadFromUri(uriResult);
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

        #endregion
    }
}
