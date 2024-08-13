using UnityEngine;

[DisallowMultipleComponent]
public class VisualMeetingRoom : MeetingRoom, IMeetingRoomVisualsHandler
{
    private Material _focusedMaterial;
    private Material _unFocusedMaterial;
    private Material _occupancyMaterial;

    private MeetingRoomsManager _meetingRoomsManager;

    private bool _isFocused = false;
    public bool IsFocused
    {
        get { return _isFocused; }
        set
        {
            _isFocused = value;
            if (_isFocused)
            {
                SetFocusedVisuals();
            }
            else
            {
                SetUnFocusedVisuals();
            }
        }
    }

    private RoomDataTypes _currentDataTypeShown = RoomDataTypes.NoData;
    public RoomDataTypes CurrentDataTypeShown
    {
        get { return _currentDataTypeShown; }
        set
        {
            _currentDataTypeShown = value;
            if (_currentDataTypeShown == RoomDataTypes.MeetingsPerDay)
            {
                SetNumberOfMeetingsVisuals();
            }
        }
    }

    private void Awake()
    {
        _focusedMaterial = Resources.Load<Material>("Materials/Rooms/RoomFocusedMaterial");
        _unFocusedMaterial = Resources.Load<Material>("Materials/Rooms/RoomUnfocusedMaterial");
        _occupancyMaterial = Resources.Load<Material>("Materials/Rooms/RoomOccupancyMaterial");
        _meetingRoomsManager = FindObjectOfType<MeetingRoomsManager>();
    }

    private void SetFocusedVisuals()
    {
        if (_currentDataTypeShown != RoomDataTypes.NoData)
        {
            return;
        }
        if (_focusedMaterial == null)
        {
            Debug.LogError("No focused material found!");
            return;
        }
        MeshRenderer mr;
        if (TryGetComponent<MeshRenderer>(out mr))
        {
            mr.material = _focusedMaterial;
        }
    }

    public void SetUnFocusedVisuals()
    {
        if (_currentDataTypeShown != RoomDataTypes.NoData)
        {
            return;
        }
        if (_unFocusedMaterial == null)
        {
            Debug.LogWarning("No unfocused material found!");
            return;
        }
        MeshRenderer mr;
        if (TryGetComponent<MeshRenderer>(out mr))
        {
            mr.material = _unFocusedMaterial;
        }
    }

    public void SetNumberOfMeetingsVisuals()
    {
        if (_occupancyMaterial == null)
        {
            Debug.LogWarning("No occupancy material found!");
            return;
        }
        //_meetingRoom = GetComponent<MeetingRoom>();
        var _meshRenderer = GetComponent<MeshRenderer>();
        _meshRenderer.material = _occupancyMaterial;

        Color c;
        if (MeetingsPerDay == 0)
        {
            Debug.LogWarning("No data fetched for room occupancy");
            c = Color.grey;
        }
        else
        {
            c = Color.Lerp(
                Color.green,
                Color.red,
                MeetingsPerDay / _meetingRoomsManager.MaxMeetingsPerDay
            );
        }
        Color transparentC = new Color(c.r, c.g, c.b, 0.3f);
        _meshRenderer.material.color = transparentC;
    }
}
