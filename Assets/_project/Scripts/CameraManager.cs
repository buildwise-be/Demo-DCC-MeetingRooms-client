using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Volume _globalVolume;
    [SerializeField]
    private MeetingRoomsManager _meetingRoomsManager;

    [SerializeField]
    private CinemachineVirtualCamera[] _virtualCameras;

    void Start()
    {
        CinemachineCore.GetInputAxis = GetAxisOnlyIfRightMouseButtonDown;
        _meetingRoomsManager.OnRoomSelectionInDropdown.AddListener(FocusCameraOnRoom);
    }

    public float GetAxisOnlyIfRightMouseButtonDown(string axisName)
    {
        if (axisName == "Mouse X")
        {
            if (Input.GetMouseButton(1))
            {
                return UnityEngine.Input.GetAxis("Mouse X");
            }
            else
            {
                return 0;
            }
        }
        else if (axisName == "Mouse Y")
        {
            if (Input.GetMouseButton(1))
            {
                return UnityEngine.Input.GetAxis("Mouse Y");
            }
            else
            {
                return 0;
            }
        }
        return UnityEngine.Input.GetAxis(axisName);
    }

    public void FocusCameraOnRoom(MeetingRoom room)
    {
        if (room.RoomNumber < 0 || room.RoomNumber > _virtualCameras.Length)
        {
            Debug.LogError("Camera index out of range");
            return;
        }

        // Disable all cameras
        foreach (var cam in _virtualCameras)
        {
            cam.Priority = 0;
        }

        // Enable the selected camera
        _virtualCameras[room.RoomNumber].Priority = 10;
    }    
}
