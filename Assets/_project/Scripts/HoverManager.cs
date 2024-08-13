using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverManager : MonoBehaviour
{
    
    public LayerMask meetingRoomLayer; // Layer for MeetingRoom objects
    public float raycastDistance = 100f; // Max distance for raycast

    [SerializeField]
    private GameObject _uiManagerGameObject;
    private IUIManager _uiManager;
    [SerializeField]
    private CameraManager _cameraManager;
    [SerializeField]
    private MeetingRoomsManager _meetingRoomsManager;

    private Camera mainCamera;
    private VisualMeetingRoom currentHoveredRoom;

    public bool IsHoveringEnabled { get; private set; } = true;

    void Start()
    {
        _uiManager = _uiManagerGameObject.GetComponent<IUIManager>();
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
            HideAllHoverUI();
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
                    Debug.Log("Hovering over room " + hitRoom.RoomNumber);
                    HideHoverUI(currentHoveredRoom);
                    currentHoveredRoom = hitRoom;
                    ShowHoverUI(hitRoom);
                    FocusCameraOnRoom(hitRoom);
                    //hitRoom.IsFocused = true;
                    _meetingRoomsManager.SetFocusedRoom(hitRoom);
                }
            }
            else
            {
                HideAllHoverUI();
            }
        }
        else
        {
            HideAllHoverUI();
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
        if (_uiManagerGameObject != null)
        {
            _uiManager.ShowHoverUI(room);
        }
    }

    void HideAllHoverUI()
    {
        foreach (var room in _meetingRoomsManager.meetingRoomsInScene)
        {
            HideHoverUI(room);
        }
        currentHoveredRoom = null;
    }

    private void HideHoverUI(MeetingRoom room)
    {
        if (_uiManagerGameObject != null)
        {
            _uiManager.HideHoverUI(room);
        }
    }

    private void PerformInitialChecks()
    {
        if (_uiManagerGameObject == null)
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
