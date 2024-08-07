using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class MeetingRoom : MonoBehaviour
{
    public int roomNumber;

    /// <summary>
    /// The average number of meetings held in the room per day, between DateStartFilter and DateEndFilter included.
    /// </summary>
    public float MeetingsPerDay { get { return GetAverageMeetingsPerDay(); } }

    public List<DateTimeOffset?> StartTimes { get; set; } = null;
    public List<string> Organizers { get; set; } = null;
    public List<string> Titles { get; set; } = null;

    public DateTime DateStartFilter { get; set; } = DateTime.MinValue;
    public DateTime DateEndFilter { get; set; } = DateTime.MaxValue;

    /// <summary>
    /// Gets the average number of meetings held in the room per day, between DateStartFilter and DateEndFilter included.
    /// </summary>
    /// <returns>The average number of meetings per day</returns>
    private float GetAverageMeetingsPerDay()
    {
        if (StartTimes == null || StartTimes.Count == 0)
        {
            return 0;
        }
        var filteredStartTimes = StartTimes
            .Where(startTime =>
                startTime.HasValue
                && startTime.Value.Date >= DateStartFilter.Date
                && startTime.Value.Date <= DateEndFilter.Date
            )
            .ToList();

        var occurrencesPerDay = filteredStartTimes
            .GroupBy(startTime => startTime.Value.Date)
            .Select(group => new { Date = group.Key, Occurrences = group.Count() })
            .Average(group => group.Occurrences);

        float parsedOccurrencesPerDay = float.Parse(occurrencesPerDay.ToString());
        return parsedOccurrencesPerDay;
    }
}
