using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private RectTransform _hoverUI;
    private TextMeshProUGUI _title;
    private TextMeshProUGUI _text;
    private TextMeshProUGUI _number;

    private bool _isHovering = false;

    private void Start()
    {
        _title = _hoverUI.Find("Title").GetComponent<TextMeshProUGUI>();
        _text = _hoverUI.Find("Text").GetComponent<TextMeshProUGUI>();
        _number = _hoverUI.Find("Number").GetComponent<TextMeshProUGUI>();
        HideHoverUI();
    }

    private void Update()
    {
        if (_isHovering)
        {
            _hoverUI.transform.LookAt(Camera.main.transform);
            _hoverUI.transform.Rotate(0f, 180f, 0f);
        }
    }
    public void ShowHoverUI(MeetingRoom room)
    {
        PlaceHoverUI(room);
        _title.text = "Meeting Room " + room.RoomNumber;
        _number.text = room.MeetingsPerDay.ToString("F2");
        _text.text = "Average meetings per day";
        _hoverUI.gameObject.SetActive(true);
        _isHovering = true;
    }

    public void HideHoverUI()
    {
        _hoverUI.gameObject.SetActive(false);
        _isHovering = false;
    }

    private void PlaceHoverUI(MeetingRoom room)
    {
        Vector3 max = room.gameObject.GetComponent<BoxCollider>().bounds.max;
        float verticalExtents = room.gameObject.GetComponent<BoxCollider>().bounds.extents.y;
        Vector3 center = room.gameObject.GetComponent<BoxCollider>().bounds.center;
        transform.position = new Vector3(center.x, center.y + 2*verticalExtents, center.z);
        _hoverUI.transform.LookAt(Camera.main.transform);
        _hoverUI.transform.Rotate(0f, 180f, 0f);
    }
}