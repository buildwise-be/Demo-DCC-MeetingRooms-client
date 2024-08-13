using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class MeetingRoom : MonoBehaviour
{
    /// <summary>
    /// The Meeting Room number, starting at 1.
    /// </summary>
    public int RoomNumber;

    /// <summary>
    /// The average number of meetings held in the room per day, between DateStartFilter and DateEndFilter included.
    /// </summary>
    public float MeetingsPerDay { get { return GetAverageMeetingsPerDay(); } }

    public List<DateTimeOffset?> StartTimes { get; set; } = null;
    public List<string> Organizers { get; set; } = null;
    public List<string> Titles { get; set; } = null;

    public DateTime DateStartFilter { get; set; } = new DateTime(2024,01,01);
    public DateTime DateEndFilter { get; set; } = new DateTime(2024, 08, 15);

    /// <summary>
    /// Gets the average number of meetings held in the room per day, between DateStartFilter and DateEndFilter included.
    /// </summary>
    /// <returns>The average number of meetings per day</returns>
    private float GetAverageMeetingsPerDay()
    {
        Debug.Log($"Calculating average meetings per day for room {RoomNumber} between {DateStartFilter} and {DateEndFilter}.");
        if (StartTimes == null || StartTimes.Count == 0)
        {
            Debug.LogWarning($"Room {RoomNumber} has no start times.");
            return 0;
        }
        var filteredStartTimes = StartTimes
            .Where(startTime =>
                startTime.HasValue
                && startTime.Value.Date >= DateStartFilter.Date
                && startTime.Value.Date <= DateEndFilter.Date
            )
            .ToList();

        /*
        var occurrencesPerDay = filteredStartTimes
            .GroupBy(startTime => startTime.Value.Date)
            .Select(group => new { Date = group.Key, Occurrences = group.Count() })
            .Average(group => group.Occurrences);
        */
        /*
        var occurrencesPerDay = filteredStartTimes
            .GroupBy(startTime => startTime.Value.Date)
            .Select(group => new { Date = group.Key, Occurrences = group.Count() });
        */
        int numberOfMeetings = StartTimes.Count;
        int numberOfDays = (DateEndFilter - DateStartFilter).Days + 1;
        float occurrencesPerDay = (float)numberOfMeetings / (float)numberOfDays;

        /*
        float meetings = 0.0f;
        for (DateTime date = DateStartFilter.Date; date <= DateEndFilter.Date; date.AddDays(1.0))
        {
            var result = occurrencesPerDay.FirstOrDefault(group => date == group.Date);
            if (result != null)
            {
                meetings += result.Occurrences;
            }
        }
        */

        Debug.Log($"Room {RoomNumber} has {occurrencesPerDay} occurrences per day between {DateStartFilter} and {DateEndFilter}.");

        float parsedOccurrencesPerDay = float.Parse(occurrencesPerDay.ToString());
        return parsedOccurrencesPerDay;
    }
}
