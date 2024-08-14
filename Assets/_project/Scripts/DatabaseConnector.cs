using System.Collections.Generic;
using System.Linq;
using Azure;
using Azure.Data.Tables;
using UnityEngine;
using UnityEngine.Events;

public class DatabaseConnector : MonoBehaviour
{
    [SerializeField]
    private MeetingRoomsManager _meetingRoomsManager;
    [SerializeField]
    private SlidingUIManager _slidingUIManager;

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
        OnMeetingRoomsDataUpdated.AddListener(_slidingUIManager.DatesSlider.Setup);
        OnMeetingRoomsDataUpdated.AddListener(_meetingRoomsManager.UpdateMinMaxDatesFromRoomData);
        OnMeetingRoomsDataUpdated.AddListener(_meetingRoomsManager.InitializeDateFilters);
    }

    private void Start()
    {
        _meetingRoomsManager.OnMeetingRoomsObtained.AddListener(FetchMeetingRoomData);
    }

    private async void FetchMeetingRoomData()
    {
        Debug.Log("Fetching meeting room data...");

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
            int? roomToParse = qEntity.GetInt32("room");
            int room = roomToParse ?? 0;
            if (
                meetingRoomData.Any(
                    (data) => data.RoomNumber == room)
            )
            {
                meetingRoomData
                    .Find((data) => data.RoomNumber == room)
                    .StartTimes.Add(qEntity.GetDateTimeOffset("start_time"));
                meetingRoomData
                    .Find((data) => data.RoomNumber == room)
                    .Organizers.Add(qEntity.GetString("organizer"));
                meetingRoomData
                    .Find((data) => data.RoomNumber == room)
                    .Titles.Add(qEntity.GetString("title"));
            }
            else
            {
                MeetingRoomData roomData = new MeetingRoomData(room);
                roomData.StartTimes.Add(qEntity.GetDateTimeOffset("start_time"));
                roomData.Organizers.Add(qEntity.GetString("organizer"));
                roomData.Titles.Add(qEntity.GetString("title"));
                meetingRoomData.Add(roomData);
            }
        }
        /*
        foreach (MeetingRoomData data in meetingRoomData)
        {
            Debug.Log($"Room {data.RoomNumber} has {data.StartTimes.Count} meetings");
        }
        */
        OnMeetingRoomsDataUpdated.Invoke(meetingRoomData);
    }
}
