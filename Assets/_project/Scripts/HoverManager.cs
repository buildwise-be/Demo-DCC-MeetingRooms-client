using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverManager : MonoBehaviour
{
    
    public LayerMask meetingRoomLayer; // Layer for MeetingRoom objects
    public float raycastDistance = 100f; // Max distance for raycast

    [SerializeField]
    private UIManager _uiManager;
    [SerializeField]
    private CameraManager _cameraManager;
    [SerializeField]
    private MeetingRoomsManager _meetingRoomsManager;

    private Camera mainCamera;
    private VisualMeetingRoom currentHoveredRoom;

    public bool IsHoveringEnabled { get; private set; } = true;

    void Start()
    {
        mainCamera = Camera.main;
        PerformInitialChecks();
        _meetingRoomsManager.OnRoomSelectionInDropdown.AddListener((room) => StartCoroutine(DisableHoveringForSeconds(5)));
    }

    void Update()
    {
        CheckForHovering();
    }

    private void CheckForHovering()
    {
        if (!IsHoveringEnabled)
        {
            return;
        }
        // Check if the mouse is over UI elements
        if (EventSystem.current.IsPointerOverGameObject())
        {
            HideHoverUI();
            return;
        }

        // Perform raycast from mouse position
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDistance, meetingRoomLayer))
        {
            VisualMeetingRoom hitRoom = hit.collider.GetComponent<VisualMeetingRoom>();
            if (hitRoom != null)
            {
                if (hitRoom != currentHoveredRoom)
                {
                    currentHoveredRoom = hitRoom;
                    ShowHoverUI(hitRoom);
                    FocusCameraOnRoom(hitRoom);
                    hitRoom.IsFocused = true;
                }
            }
            else
            {
                HideHoverUI();
            }
        }
        else
        {
            HideHoverUI();
        }
    }

    private IEnumerator DisableHoveringForSeconds(float duration)
    {
        IsHoveringEnabled = false;
        yield return new WaitForSeconds(duration);
        IsHoveringEnabled = true;
    }

    private void FocusCameraOnRoom(MeetingRoom room)
    {
        _cameraManager.FocusCameraOnRoom(room);
    }

    void ShowHoverUI(MeetingRoom room)
    {
        if (_uiManager != null)
        {
            _uiManager.ShowHoverUI(room);
        }
    }

    void HideHoverUI()
    {
        if (currentHoveredRoom != null)
        {
            currentHoveredRoom = null;
            if (_uiManager != null)
            {
                _uiManager.HideHoverUI();
            }
        }
    }

    private void PerformInitialChecks()
    {
        if (_uiManager == null)
        {
            Debug.LogError("UIManager reference is missing in HoverManager!");
        }
        if (_cameraManager == null)
        {
            Debug.LogError("CameraManager reference is missing in HoverManager!");
        }
        if (_meetingRoomsManager == null)
        {
            Debug.LogError("MeetingRoomsManager reference is missing in HoverManager");
        }
    }
}
