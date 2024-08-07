﻿using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class MeetingRoomsManager : MonoBehaviour
{
    [HideInInspector]
    public List<MeetingRoom> meetingRoomsInScene;

    [SerializeField] private TMP_Dropdown _roomsDropdown;
    [SerializeField] private GameObject _meetingRoomsCenter;
    [SerializeField] private DatabaseConnector _databaseConnector;

    [SerializeField] private GameObject _meetingRoomCenterGO;

    [HideInInspector]
    public UnityEvent<int> OnFocusRoomChanged;
    [HideInInspector]
    public UnityEvent OnMeetingRoomsObtained;

    public float MaxMeetingsPerDay
    {
        get
        {
            return meetingRoomsInScene.Max(room => room.MeetingsPerDay);
        }
    }


    private int _currentFocusedRoom = -1;
    public int CurrentFocusedRoom 
    {
        get
        {
            return _currentFocusedRoom;
        }
        set 
        {
            _currentFocusedRoom = value;
            OnFocusRoomChanged.Invoke(_currentFocusedRoom);
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
        OnFocusRoomChanged = new UnityEvent<int>();
        OnMeetingRoomsObtained = new UnityEvent();
    }

    private void Start()
    {
        OnFocusRoomChanged.AddListener(UpdateAllRoomVisualsByFocus);
        _databaseConnector.OnMeetingRoomsDataUpdated.AddListener(UpdateMeetingRoomsGameObjects);

        foreach (Transform t in transform.GetComponentsInChildren<Transform>())
        {
            if (t == transform)
            {
                continue;
            }
            t.gameObject.AddComponent<MeetingRoom>();
            t.gameObject.AddComponent<MeetingRoomVisualsHandler>();
        }

        meetingRoomsInScene = transform.GetComponentsInChildren<MeetingRoom>().ToList();

        OnMeetingRoomsObtained.Invoke();
        PopulateRoomsDropDown();
        PositionMeetingRoomsCenter();
        UpdateAllRoomVisualsAsUnfocused();
    }

    private void UpdateMeetingRoomsGameObjects(List<MeetingRoomData> meetingRoomsData)
    {
        foreach (MeetingRoom mr_go in meetingRoomsInScene)
        {
            meetingRoomsData.Find((data) => data.RoomNumber == mr_go.roomNumber).UpdateMeetingRoomGameObject(mr_go);
        }
    }

    private void UpdateAllRoomVisualsByFocus(int roomNumber)
    {
        for (int i = 0; i < meetingRoomsInScene.Count; i++)
        {
            if (i == roomNumber - 1)
            {
                meetingRoomsInScene[i].GetComponent<IMeetingRoomVisualsHandler>().SetFocusedVisuals();
            }
            else
            {
                meetingRoomsInScene[i].GetComponent<IMeetingRoomVisualsHandler>().SetUnFocusedVisuals();
            }
        }
    }

    private void UpdateAllRoomVisualsAsUnfocused()
    {
        foreach (MeetingRoom room in meetingRoomsInScene)
        {
            room.GetComponent<IMeetingRoomVisualsHandler>().SetUnFocusedVisuals();
        }
    }

    /*
    public void UpdateRoomVisualsPerOccupancy()
    {
        foreach (MeetingRoom room in meetingRoomsInScene)
        {
            room.GetComponent<IMeetingRoomVisualsHandler>().SetOccupancyVisuals();
        }
    }
    */

    public void UpdateRoomVisualsPerMeetingAmount()
    {
        foreach (MeetingRoom room in meetingRoomsInScene)
        {
            room.GetComponent<IMeetingRoomVisualsHandler>().SetNumberOfMeetingsVisuals();
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
        roomNames.Add("Global");
        foreach (MeetingRoom room in meetingRoomsInScene)
        {
            roomNames.Add(room.name);
        }
        _roomsDropdown.AddOptions(roomNames);
    }

    public void SetCurrentFocusedRoom(int roomNumber)
    {
        CurrentFocusedRoom = roomNumber;
    }
}
