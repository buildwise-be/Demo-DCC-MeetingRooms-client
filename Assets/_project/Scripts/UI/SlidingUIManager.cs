using System.Collections.Generic;
using UnityEngine;
using TS.DoubleSlider;

public class SlidingUIManager : MonoBehaviour, IUIManager
{
    [SerializeField]
    private List<SlidingUI> _slidingUIs;
    public DoubleSlider DatesSlider;

    [SerializeField]
    private DatabaseConnector _databaseConnector;

    public void ShowHoverUI(MeetingRoom room)
    {
        foreach (SlidingUI uI in _slidingUIs)
        {
            if (uI._associatedRoom == room)
            {
                uI.ShowUI();
                break;
            }
        }
    }

    public void HideHoverUI(MeetingRoom room)
    {
        foreach (SlidingUI uI in _slidingUIs)
        {
            if (uI._associatedRoom == room)
            {
                uI.HideUI();
                break;
            }
        }
    }
}
