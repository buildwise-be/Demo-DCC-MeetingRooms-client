using UnityEngine;

[DisallowMultipleComponent]
public class MeetingRoom : MonoBehaviour
{
    public int roomNumber;
    /// <summary>
    /// The occupancy rate of the room. A value between 0 and 1.
    /// </summary>
    public float occupancyRate = -1;

    /// <summary>
    /// The average number of meetings held in the room per day.
    /// </summary>
    public float meetingsPerDay = -1;
}
