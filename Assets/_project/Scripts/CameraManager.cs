using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Volume _globalVolume;
    [SerializeField]
    private MeetingRoomsManager _meetingRoomsManager;
    [SerializeField]
    private CinemachineVirtualCamera _virtualCamera;

    private List<MeetingRoom> _meetingRooms;

    void Start()
    {
        CinemachineCore.GetInputAxis = GetAxisOnlyIfRightMouseButtonDown;
        _meetingRoomsManager.OnFocusRoomChanged.AddListener(FocusCameraOnRoom);
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

    private IEnumerator ChangeCameraFocusCoroutine(int roomNumber)
    {
        _meetingRooms = _meetingRoomsManager.meetingRoomsInScene;
        if (_meetingRooms == null) { yield break; }
        if (roomNumber == 0)
        {
            Debug.Log($"Virtual camera position before change is {_virtualCamera.transform.position}");
            _virtualCamera.LookAt = _meetingRoomsManager.CenterOfAllMeetingRooms;
            _virtualCamera.Follow = _meetingRoomsManager.CenterOfAllMeetingRooms;
            yield return new WaitForSeconds(2f); // TODO Change this to a more elegant solution, with events from virtual camera
            Debug.Log($"Virtual camera position after change is {_virtualCamera.transform.position}");
            var transposer = _virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            transposer.m_FollowOffset = new Vector3(0, 40, -40);
            Debug.Log($"Virtual camera position after offset change is {_virtualCamera.transform.position}");
            AdjustDepthOfField(_meetingRoomsManager.CenterOfAllMeetingRooms);
        }
        else
        {
            Debug.Log($"Virtual camera position before change is {_virtualCamera.transform.position}");
            _virtualCamera.LookAt = _meetingRooms[roomNumber - 1].transform;
            _virtualCamera.Follow = _meetingRooms[roomNumber - 1].transform;
            yield return new WaitForSeconds(2f);
            var transposer = _virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            Debug.Log($"Virtual camera position after change is {_virtualCamera.transform.position}");
            transposer.m_FollowOffset = new Vector3(0, 15, -10);
            Debug.Log($"Virtual camera position after offset change is {_virtualCamera.transform.position}");
            AdjustVolumeDepthOfField(_meetingRooms[roomNumber - 1].gameObject);
        }
    }

    public void FocusCameraOnRoom(int roomNumber)
    {
        _meetingRooms = _meetingRoomsManager.meetingRoomsInScene;
        if (_meetingRooms == null) { return; }
        if (roomNumber == 0)
        {
            _virtualCamera.LookAt = _meetingRoomsManager.CenterOfAllMeetingRooms;
            _virtualCamera.Follow = _meetingRoomsManager.CenterOfAllMeetingRooms;
            var transposer = _virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            transposer.m_FollowOffset = new Vector3(0, 40, -40);
        }
        else
        {
            _virtualCamera.LookAt = _meetingRooms[roomNumber - 1].transform;
            _virtualCamera.Follow = _meetingRooms[roomNumber - 1].transform;
            var transposer = _virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            transposer.m_FollowOffset = new Vector3(0, 15, -10);
        }
    }

    /// <summary>
    /// Moves the virtual camera to focus on the given room
    /// </summary>
    /// <param name="room">The room on which to focus</param>
    public void FocusCameraOnRoom(MeetingRoom room)
    {
        if (room.roomNumber == 0)
        {
            _virtualCamera.LookAt = _meetingRoomsManager.CenterOfAllMeetingRooms;
            _virtualCamera.Follow = _meetingRoomsManager.CenterOfAllMeetingRooms;
            var transposer = _virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            transposer.m_FollowOffset = new Vector3(0, 40, -40);
        }
        else
        {
            _virtualCamera.LookAt = room.transform;
            _virtualCamera.Follow = room.transform;
            var transposer = _virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        }
    }

    private void AdjustVolumeDepthOfField(GameObject go)
    {
        var boxCollider = go.GetComponent<BoxCollider>().center;
        Vector3 centerInWorldCoordinates = go.transform.TransformPoint(boxCollider);

        var transposer = _virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        Vector3 delta = transposer.m_FollowOffset;
        Debug.Log($"Virtual camera offset is {delta}");
        Vector3 deltaCorrectedCameraPosition = _virtualCamera.transform.position - delta;
        Vector3 actualCameraPosition = _virtualCamera.transform.position;
        float distanceFromCamera = Vector3.Distance(centerInWorldCoordinates, actualCameraPosition);
        Debug.Log($"Virtual actual camera position is {actualCameraPosition}");
        Debug.Log($"Virtual camera position is {deltaCorrectedCameraPosition}");
        Debug.Log($"Target position is {centerInWorldCoordinates}");
        Debug.Log($"Adjusting depth of field with distance {distanceFromCamera}");
        if (_globalVolume.profile.TryGet<DepthOfField>(out var depthOfField))
        {
            depthOfField.focusDistance.value = distanceFromCamera;
            depthOfField.aperture.value = 1.0f;
            depthOfField.focalLength.value = 130.0f;
        }
    }

    private void AdjustDepthOfField(Transform target)
    {
        if (_globalVolume.profile.TryGet<DepthOfField>(out var depthOfField))
        {
            var transposer = _virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            Vector3 delta = transposer.m_FollowOffset;
            Debug.Log($"Virtual camera offset is {delta}");
            Vector3 trueCameraPosition = _virtualCamera.transform.position - delta;
            depthOfField.focusDistance.value = Vector3.Distance(target.position, trueCameraPosition);
            Debug.Log($"Virtual camera position is {trueCameraPosition}");
            Debug.Log($"Target position is {target.position}");
            Debug.Log($"Adjusting depth of field with distance {Vector3.Distance(target.position, trueCameraPosition)}");
            depthOfField.aperture.value = 1.0f;
            depthOfField.focalLength.value = 130.0f;
        }
    }
}
