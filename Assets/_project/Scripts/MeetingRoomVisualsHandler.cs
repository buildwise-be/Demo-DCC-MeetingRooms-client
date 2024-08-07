using UnityEngine;

[RequireComponent(typeof(MeetingRoom))]
[DisallowMultipleComponent]
public class MeetingRoomVisualsHandler : MonoBehaviour, IMeetingRoomVisualsHandler
{
    private MeetingRoom _meetingRoom;
    private Material _focusedMaterial;
    private Material _unFocusedMaterial;
    private Material _occupancyMaterial;

    private MeetingRoomsManager _meetingRoomsManager;

    private void Awake()
    {
        _focusedMaterial = Resources.Load<Material>("Materials/Rooms/RoomFocusedMaterial");
        _unFocusedMaterial = Resources.Load<Material>("Materials/Rooms/RoomUnfocusedMaterial");
        _occupancyMaterial = Resources.Load<Material>("Materials/Rooms/RoomOccupancyMaterial");
        _meetingRoomsManager = FindObjectOfType<MeetingRoomsManager>();
    }
    public void SetFocusedVisuals()
    {
        if (_focusedMaterial == null)
        {
            Debug.LogWarning("No focused material found!");
            return;
        }
        _meetingRoom = GetComponent<MeetingRoom>();
        _meetingRoom.GetComponent<MeshRenderer>().material = _focusedMaterial;
    }

    public void SetUnFocusedVisuals()
    {
        if (_unFocusedMaterial == null)
        {
            Debug.LogWarning("No unfocused material found!");
            return;
        }
        _meetingRoom = GetComponent<MeetingRoom>();
        _meetingRoom.GetComponent<MeshRenderer>().material = _unFocusedMaterial;
    }

    /*
    public void SetOccupancyVisuals()
    {
        if (_occupancyMaterial == null)
        {
            Debug.LogWarning("No occupancy material found!");
            return;
        }
        _meetingRoom = GetComponent<MeetingRoom>();
        var _meshRenderer = _meetingRoom.GetComponent<MeshRenderer>();
        _meshRenderer.material = _occupancyMaterial;

        Color c;
        if (_meetingRoom.occupancyRate == -1)
        {
            Debug.LogWarning("No data fetched for room occupancy");
            c = Color.grey;
        }
        else
        {
            c = Color.Lerp(Color.green, Color.red, _meetingRoom.occupancyRate);
        }
        Color transparentC = new Color(c.r, c.g, c.b, 0.3f);
        _meshRenderer.material.color = transparentC;
    }
    */
    

    public void SetNumberOfMeetingsVisuals()
    {
        if (_occupancyMaterial == null)
        {
            Debug.LogWarning("No occupancy material found!");
            return;
        }
        _meetingRoom = GetComponent<MeetingRoom>();
        var _meshRenderer = _meetingRoom.GetComponent<MeshRenderer>();
        _meshRenderer.material = _occupancyMaterial;

        Color c;
        if (_meetingRoom.MeetingsPerDay == 0)
        {
            Debug.LogWarning("No data fetched for room occupancy");
            c = Color.grey;
        }
        else
        {
            c = Color.Lerp(Color.green, Color.red, _meetingRoom.MeetingsPerDay/ _meetingRoomsManager.MaxMeetingsPerDay);
        }
        Color transparentC = new Color(c.r, c.g, c.b, 0.3f);
        _meshRenderer.material.color = transparentC;
    }
}