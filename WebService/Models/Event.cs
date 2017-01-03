using Ical.Net.Interfaces.Components;
using System;

namespace AlarmWorkflow.BackendService.WebService.Models
{
    struct Event
    {
        #region Properties

        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public TimeSpan Duration { get; set; }
        public string Summary { get; set; }
        public string Location { get; set; }

        #endregion

        #region Constructors

        public Event(IEvent entry)
        {
            Start = entry.Start.Value;
            End = entry.End.Value;
            Duration = entry.Duration;
            Summary = entry.Summary;
            Location = entry.Location;
        }

        #endregion
    }
}
