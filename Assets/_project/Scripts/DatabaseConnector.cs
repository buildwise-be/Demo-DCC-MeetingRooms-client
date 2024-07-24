using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DatabaseConnector : MonoBehaviour
{
    private List<float> _roomOccupancies;
    private List<float> _meetingsPerDay;
    [SerializeField] private MeetingRoomsManager _meetingRoomsManager;
    [HideInInspector] public UnityEvent<List<float>> OnRoomOccupanciesUpdated;
    [HideInInspector] public UnityEvent<List<float>> OnMeetingsPerDayUpdated;

    private void Awake()
    {
        OnRoomOccupanciesUpdated = new UnityEvent<List<float>>();
        OnMeetingsPerDayUpdated = new UnityEvent<List<float>>();
        _roomOccupancies = new List<float>();
        _meetingsPerDay = new List<float>();
    }

    private void Start()
    {
        _meetingRoomsManager.OnMeetingRoomsObtained.AddListener(FetchMeetingRoomData);
    }

    private void FetchMeetingRoomData()
    {
        _roomOccupancies.Clear();
        // Fake data
        for (int i = 0; i < 20; i++)
        {
            _roomOccupancies.Add(Random.Range(0.0f, 1.0f));
        }
        OnRoomOccupanciesUpdated.Invoke(_roomOccupancies);

        for (int i = 0; i < 20; i++)
        {
            _meetingsPerDay.Add(Random.Range(0.0f, 10.0f));
        }
        OnMeetingsPerDayUpdated.Invoke(_meetingsPerDay);
    }
}
