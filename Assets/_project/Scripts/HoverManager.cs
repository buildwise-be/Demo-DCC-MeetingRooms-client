using UnityEngine;
using UnityEngine.EventSystems;

public class HoverManager : MonoBehaviour
{
    public UIManager uiManager; // Reference to the UIManager
    public LayerMask meetingRoomLayer; // Layer for MeetingRoom objects
    public float raycastDistance = 100f; // Max distance for raycast
    [SerializeField]
    private CameraManager _cameraManager;

    private Camera mainCamera;
    private MeetingRoom currentHoveredRoom;

    void Start()
    {
        mainCamera = Camera.main;
        if (uiManager == null)
        {
            Debug.LogError("UIManager reference is missing in HoverManager!");
        }
    }

    void Update()
    {
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
            MeetingRoom hitRoom = hit.collider.GetComponent<MeetingRoom>();
            if (hitRoom != null)
            {
                if (hitRoom != currentHoveredRoom)
                {
                    currentHoveredRoom = hitRoom;
                    ShowHoverUI(hitRoom);
                    FocusCameraOnRoom(hitRoom);
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

    private void FocusCameraOnRoom(MeetingRoom room)
    {
        _cameraManager.FocusCameraOnRoom(room);
    }

    void ShowHoverUI(MeetingRoom room)
    {
        if (uiManager != null)
        {
            uiManager.ShowHoverUI(room);
        }
    }

    void HideHoverUI()
    {
        if (currentHoveredRoom != null)
        {
            currentHoveredRoom = null;
            if (uiManager != null)
            {
                uiManager.HideHoverUI();
            }
        }
    }
}
