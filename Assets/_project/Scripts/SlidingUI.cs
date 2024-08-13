using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlidingUI : MonoBehaviour
{

    public MeetingRoom _associatedRoom;
    private RectTransform _rectTransform;
    private TextMeshProUGUI _value;
    private TextMeshProUGUI _description;
    private TextMeshProUGUI _title;

    private bool _isUIVisible = false;
    private float _initialWidth;
    private float _xPosition;


    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        float parentScale = _rectTransform.parent.GetComponent<RectTransform>().localScale.x;
        _initialWidth = _associatedRoom.GetComponent<BoxCollider>().bounds.extents.x * 2 / parentScale;
        //_xPosition = (_associatedRoom.GetComponent<BoxCollider>().bounds.center.x - _associatedRoom.GetComponent<BoxCollider>().bounds.extents.x)/ parentScale;
        //_xPosition = (_associatedRoom.GetComponent<BoxCollider>().bounds.center.x - _associatedRoom.GetComponent<BoxCollider>().bounds.extents.x);
        //Vector3 parentPosition = _rectTransform.parent.GetComponent<RectTransform>().position;
        //Debug.Log($"Moving parent position from {parentPosition} to {new Vector3(_xPosition, parentPosition.y, parentPosition.z)}");
        //parentPosition = new Vector3(_xPosition, parentPosition.y, parentPosition.z);
        //_rectTransform.parent.GetComponent<RectTransform>().rect.x = _xPosition;
        Debug.Log("Sliding UI " + gameObject.name + " initial width: " + _initialWidth);
        Debug.Log("Sliding UI " + gameObject.name + " _xPosition: " + _xPosition);
        foreach (var item in _rectTransform.GetComponentsInChildren<TextMeshProUGUI>(true))
        {
            if (item.name == "Value")
            {
                _value = item;
            }
            else if (item.name == "Description")
            {
                _description = item;
            }
            else if (item.name == "Title")
            {
                _title = item;
            }
        }
    }

    private void UpdateUI()
    {
        _value.text = _associatedRoom.MeetingsPerDay.ToString("F2");
        _description.text = "Average meetings per day";
    }

    public void ShowUI()
    {
        UpdateUI();
        StartCoroutine(ShowUICoroutine());
    }

    private IEnumerator ShowUICoroutine()
    {
        if (_isUIVisible)
        {
            yield break;
        }
        GetComponent<Image>().enabled = true;
        Tween myTween = DOTween.To(() => _rectTransform.offsetMax, x => _rectTransform.offsetMax = x, new Vector2(0, 0), 1);
        yield return myTween.WaitForCompletion();
        _value.gameObject.SetActive(true);
        _description.gameObject.SetActive(true);
        _title.gameObject.SetActive(true);
        _isUIVisible = true;
    }

    private IEnumerator HideUICoroutine()
    {
        if (!_isUIVisible)
        {
            yield break;
        }
        _value.gameObject.SetActive(false);
        _description.gameObject.SetActive(false);
        _title.gameObject.SetActive(false);
        Tween myTween = DOTween.To(() => _rectTransform.offsetMax, x => _rectTransform.offsetMax = x, new Vector2(-_initialWidth, 0), 1);
        yield return myTween.WaitForCompletion();
        GetComponent<Image>().enabled = false;
        _isUIVisible = false;
    }

    public void HideUI()
    {
        StartCoroutine(HideUICoroutine());
    }
}
