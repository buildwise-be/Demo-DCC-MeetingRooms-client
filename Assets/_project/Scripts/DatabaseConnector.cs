using System.Collections.Generic;
using System.Linq;
using Azure;
using Azure.Data.Tables;
using UnityEngine;
using UnityEngine.Events;

public class DatabaseConnector : MonoBehaviour
{
    private List<float> _roomOccupancies;
    private List<float> _meetingsPerDay;

    [SerializeField]
    private MeetingRoomsManager _meetingRoomsManager;

    [HideInInspector]
    public UnityEvent<List<float>> OnRoomOccupanciesUpdated;

    [HideInInspector]
    public UnityEvent<List<float>> OnMeetingsPerDayUpdated;

    [HideInInspector]
    public UnityEvent<List<MeetingRoomData>> OnMeetingRoomsDataUpdated;

    private void Awake()
    {
        OnRoomOccupanciesUpdated = new UnityEvent<List<float>>();
        OnMeetingsPerDayUpdated = new UnityEvent<List<float>>();
        OnMeetingRoomsDataUpdated = new UnityEvent<List<MeetingRoomData>>();
        _roomOccupancies = new List<float>();
        _meetingsPerDay = new List<float>();
    }

    private void Start()
    {
        _meetingRoomsManager.OnMeetingRoomsObtained.AddListener(FetchMeetingRoomData);
    }

    private async void FetchMeetingRoomData()
    {
        Debug.Log("Fetching meeting room data...");
        _roomOccupancies.Clear();

        // Construct a new TableClient using a connection string.
        Config config = ConfigManager.LoadConfig();

        var tableClient = new TableClient(
            config.DatabaseSettings.connectionString,
            config.DatabaseSettings.tableName
        );

        var response = await tableClient.CreateIfNotExistsAsync();
        Debug.Log("Connected to table " + response.Value.Name);

        var partitionKey = "BWZ";

        Pageable<TableEntity> queryResultsFilter = tableClient.Query<TableEntity>(
            filter: $"PartitionKey eq '{partitionKey}'"
        );

        List<MeetingRoomData> meetingRoomData = new List<MeetingRoomData>();
        // Iterate the <see cref="Pageable"> to access all queried entities.
        foreach (TableEntity qEntity in queryResultsFilter)
        {
            Debug.Log($"{qEntity.GetString("room")}: {qEntity.GetDateTimeOffset("start_time")}");

            if (
                meetingRoomData.Any(
                    (data) => data.RoomNumber == int.Parse(qEntity.GetString("room"))
                )
            )
            {
                meetingRoomData
                    .Find((data) => data.RoomNumber == int.Parse(qEntity.GetString("room")))
                    .StartTimes.Add(qEntity.GetDateTimeOffset("start_time"));
                meetingRoomData
                    .Find((data) => data.RoomNumber == int.Parse(qEntity.GetString("room")))
                    .Organizers.Add(qEntity.GetString("organizer"));
                meetingRoomData
                    .Find((data) => data.RoomNumber == int.Parse(qEntity.GetString("room")))
                    .Titles.Add(qEntity.GetString("title"));
            }
            else
            {
                MeetingRoomData roomData = new MeetingRoomData(
                    int.Parse(qEntity.GetString("room"))
                );
                roomData.StartTimes.Add(qEntity.GetDateTimeOffset("start_time"));
                roomData.Organizers.Add(qEntity.GetString("organizer"));
                roomData.Titles.Add(qEntity.GetString("title"));
                meetingRoomData.Add(roomData);
            }
        }

        foreach (MeetingRoomData data in meetingRoomData)
        {
            Debug.Log($"Room {data.RoomNumber} has {data.StartTimes.Count} meetings");
        }

        OnMeetingRoomsDataUpdated.Invoke(meetingRoomData);
    }
}
