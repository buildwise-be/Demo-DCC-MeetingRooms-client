using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverManager : MonoBehaviour
{
    
    public LayerMask meetingRoomLayer; // Layer for MeetingRoom objects
    public float raycastDistance = 100f; // Max distance for raycast

    [SerializeField]
    [Tooltip("Delay in seconds before reanabling hover after a room dropdown selection.")]
    private float _hoverUIReenableDelay = 3f;
    [SerializeField]
    [Tooltip("Required time in seconds to detect a room as being hovered.")]
    private float _hoveringRequiredTime = 1.0f;

    [SerializeField]
    private GameObject _uiManagerGameObject;
    private IUIManager _uiManager;
    [SerializeField]
    private CameraManager _cameraManager;
    [SerializeField]
    private MeetingRoomsManager _meetingRoomsManager;

    private Camera mainCamera;
    private VisualMeetingRoom currentHoveredRoom;

    private bool _isHoveringInProgress = false;

    public bool IsHoveringEnabled { get; private set; } = true;

    void Start()
    {
        _uiManager = _uiManagerGameObject.GetComponent<IUIManager>();
        mainCamera = Camera.main;
        PerformInitialChecks();
        _meetingRoomsManager.OnRoomSelectionInDropdown.AddListener((room) => StartCoroutine(DisableHoveringForSeconds(_hoverUIReenableDelay)));
    }

    void Update()
    {
        if (!_isHoveringInProgress && IsHoveringEnabled)
        {
            StartCoroutine(CheckForHoveringCoroutine());
        }
    }

    private IEnumerator CheckForHoveringCoroutine()
    {
        _isHoveringInProgress = true;
        bool hoveringConfirmed = false;
        float _hoveringProgressDuration = 0.0f;

        // Check if the mouse is over UI elements
        if (EventSystem.current.IsPointerOverGameObject())
        {
            HideAllHoverUI();
            _isHoveringInProgress = false;
            yield break;
        }

        int count = 0;
        while (!hoveringConfirmed)
        {
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
                        _hoveringProgressDuration += Time.deltaTime;
                        if (_hoveringProgressDuration > _hoveringRequiredTime)
                        {
                            hoveringConfirmed = true;
                            HideHoverUI(currentHoveredRoom);
                            currentHoveredRoom = hitRoom;
                            ShowHoverUI(hitRoom);
                            FocusCameraOnRoom(hitRoom);
                            _meetingRoomsManager.SetFocusedRoom(hitRoom);
                            _isHoveringInProgress = false;
                        }
                        yield return null;
                    }
                    else
                    {
                        _isHoveringInProgress = false;
                        yield break;
                    }
                }
                else
                {
                    _isHoveringInProgress = false;
                    yield break;
                }
            }
            else
            {
                _isHoveringInProgress = false;
                yield break;
            }
            count++;
            if (count > 5000)
            {
                Debug.LogError("Infinite loop detected in CheckForHoveringCoroutine!");
                yield break;
            }
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
