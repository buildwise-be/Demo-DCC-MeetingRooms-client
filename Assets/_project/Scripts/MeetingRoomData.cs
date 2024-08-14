using System;
using System.Collections.Generic;
using UnityEngine;

public class MeetingRoomData
{
    public MeetingRoomData(int roomNumber)
    {
        RoomNumber = roomNumber;
        StartTimes = new List<DateTimeOffset?>();
        Organizers = new List<string>();
        Titles = new List<string>();
    }

    public int RoomNumber { get; set; }
    public List<DateTimeOffset?> StartTimes { get; set; }
    public List<string> Organizers { get; set; }
    public List<string> Titles { get; set; }

    internal void UpdateMeetingRoomGameObject(MeetingRoom mr_go)
    {
        Debug.Log("Updating MeetingRoom. StartTimes: " + StartTimes.Count + ", Organizers: " + Organizers.Count + ", Titles: " + Titles.Count);
        mr_go.StartTimes = StartTimes;
        mr_go.Organizers = Organizers;
        mr_go.Titles = Titles;
    }
}
