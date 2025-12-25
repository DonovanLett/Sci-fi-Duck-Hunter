using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{ 
    [SerializeField]
    private bool _isFinalWaypoint = false;

    [SerializeField] // Unserialize after experimentation
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
