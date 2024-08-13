using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class MeetingRoomsManager : MonoBehaviour
{
    [HideInInspector]
    public List<VisualMeetingRoom> meetingRoomsInScene;

    [SerializeField] private TMP_Dropdown _roomsDropdown;
    [SerializeField] private GameObject _meetingRoomsCenter;
    [SerializeField] private DatabaseConnector _databaseConnector;

    [SerializeField] private GameObject _meetingRoomCenterGO;

    [HideInInspector]
    public UnityEvent<VisualMeetingRoom> OnRoomSelectionInDropdown;
    [HideInInspector]
    public UnityEvent OnMeetingRoomsObtained;

    public float MaxMeetingsPerDay
    {
        get
        {
            return meetingRoomsInScene.Max(room => room.MeetingsPerDay);
        }
    }


    private int _currentlySelectedRoomDropDownIndex = -1;
    public int CurrentlySelectedRoomDropDownIndex 
    {
        get
        {
            return _currentlySelectedRoomDropDownIndex;
        }
        set 
        {
            _currentlySelectedRoomDropDownIndex = value;
            VisualMeetingRoom mr = meetingRoomsInScene.Where((room) => room.RoomNumber == (_currentlySelectedRoomDropDownIndex)).FirstOrDefault();
            OnRoomSelectionInDropdown.Invoke(mr);
        }
    }
    
    
    private Transform _centerOfAllMeetingRooms;
    /// <summary>
    /// The Transform of the center of all meeting rooms, in world space.
    /// </summary>
    public Transform CenterOfAllMeetingRooms
    {
        get { return _centerOfAllMeetingRooms; }
        private set { }
    }

    private void Awake()
    {
        OnRoomSelectionInDropdown = new UnityEvent<VisualMeetingRoom>();
        OnMeetingRoomsObtained = new UnityEvent();
    }

    private void Start()
    {
        OnRoomSelectionInDropdown.AddListener(SetFocusedRoom);
        _databaseConnector.OnMeetingRoomsDataUpdated.AddListener(UpdateMeetingRoomsGameObjects);

        foreach (Transform t in transform.GetComponentsInChildren<Transform>())
        {
            if (t == transform)
            {
                continue;
            }
            if (!t.gameObject.TryGetComponent<VisualMeetingRoom>(out VisualMeetingRoom vmr))
            {
                t.gameObject.AddComponent<VisualMeetingRoom>();
            }
        }

        meetingRoomsInScene = transform.GetComponentsInChildren<VisualMeetingRoom>().ToList();

        OnMeetingRoomsObtained.Invoke();
        PopulateRoomsDropDown();
        PositionMeetingRoomsCenter();
        //UpdateAllRoomVisualsAsUnfocused();
    }

    public void SetFocusedRoom(VisualMeetingRoom room)
    {
        foreach (VisualMeetingRoom r in meetingRoomsInScene)
        {
            if (r != room)
            {
                r.IsFocused = false;
            }
        }
        room.IsFocused = true;
    }

    private void UpdateMeetingRoomsGameObjects(List<MeetingRoomData> meetingRoomsData)
    {
        foreach (VisualMeetingRoom mr_go in meetingRoomsInScene)
        {
            var matchingRoom = meetingRoomsData.Find((data) => data.RoomNumber == mr_go.RoomNumber);

            if (matchingRoom != null)
            {
                matchingRoom.UpdateMeetingRoomGameObject(mr_go);
            }
        }
    }

    private void PositionMeetingRoomsCenter()
    {
        Vector3 center = Vector3.zero;
        foreach (MeetingRoom room in meetingRoomsInScene)
        {
            center += room.transform.position;
        }
        center /= transform.childCount;
        _meetingRoomsCenter.transform.position = center;
        _centerOfAllMeetingRooms = _meetingRoomsCenter.transform;
        if (_meetingRoomCenterGO != null)
        {
            _meetingRoomCenterGO.transform.position = center;
        }
    }

    private void PopulateRoomsDropDown()
    {
        _roomsDropdown.ClearOptions();
        List<string> roomNames = new List<string>();
        foreach (MeetingRoom room in meetingRoomsInScene)
        {
            if (room.name == "Global MR")
            {
                roomNames.Add("Global view");
            }
            else
            {
                roomNames.Add(room.name);
            }
        }
        _roomsDropdown.AddOptions(roomNames);
    }

    public void SetAverageMeetingsVisuals()
    {
        foreach (VisualMeetingRoom visualMeetingRoom in meetingRoomsInScene)
        {
            visualMeetingRoom.CurrentDataTypeShown = RoomDataTypes.MeetingsPerDay;
        }
    }
}
