using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    // Waypoint Script
    [SerializeField]
    private bool _isFinalWaypoint = false;

    private bool _isOccupied = false;

    public bool IsOccupied()
    {
        return _isOccupied;
    }

    public void SetToOccupied()
    {
        if (!(_isFinalWaypoint))
        {
            _isOccupied = true;
        }
    }

    public void SetToUnoccupied()
    {
        if (!(_isFinalWaypoint))
        {
            _isOccupied = false;
        }

    }

}
