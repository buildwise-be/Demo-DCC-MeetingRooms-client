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
    public float AverageMeetingsPerDay { get { return GetAverageMeetingsPerDay(); } }

    public List<DateTimeOffset?> StartTimes { get; set; } = null;
    public List<string> Organizers { get; set; } = null;
    public List<string> Titles { get; set; } = null;

    private DateTime _dateStartFilter;
    private DateTime _dateEndFilter;

    protected MeetingRoomsManager _meetingRoomsManager;

    private void Awake()
    {
        _meetingRoomsManager = FindObjectOfType<MeetingRoomsManager>();
    }

    /// <summary>
    /// Gets the average number of meetings held in the room per day, between DateStartFilter and DateEndFilter included.
    /// </summary>
    /// <returns>The average number of meetings per day</returns>
    private float GetAverageMeetingsPerDay()
    {
        _dateStartFilter = _meetingRoomsManager.FilteredStartDate;
        _dateEndFilter = _meetingRoomsManager.FilteredEndDate;
        if (StartTimes == null || StartTimes.Count == 0)
        {
            Debug.LogWarning($"Room {RoomNumber} has no start times.");
            return 0;
        }
        var filteredStartTimes = StartTimes
            .Where(startTime =>
                startTime.HasValue
                && startTime.Value.Date >= _dateStartFilter.Date
                && startTime.Value.Date <= _dateEndFilter.Date
            )
            .ToList();

        int numberOfMeetings = filteredStartTimes.Count;
        int numberOfDays = (_dateEndFilter - _dateStartFilter).Days + 1;
        float averageMeetingsPerDay = (float)numberOfMeetings / (float)numberOfDays;

        if (RoomNumber == 1)
        {
            Debug.Log($"Room 1:");
            Debug.Log($"\tAverage meetings per day: {averageMeetingsPerDay}");
            Debug.Log($"\tFiltered start date: {_dateStartFilter.ToShortDateString()}");
            Debug.Log($"\tFiltered end date: {_dateEndFilter.ToShortDateString()}");
        }
        else if (RoomNumber == 2)
        {
            Debug.Log($"Room 2:");
            Debug.Log($"\tAverage meetings per day: {averageMeetingsPerDay}");
            Debug.Log($"\tFiltered start date: {_dateStartFilter.ToShortDateString()}");
            Debug.Log($"\tFiltered end date: {_dateEndFilter.ToShortDateString()}");
        }

        float parsedOccurrencesPerDay = float.Parse(averageMeetingsPerDay.ToString());
        return parsedOccurrencesPerDay;
    }
}
