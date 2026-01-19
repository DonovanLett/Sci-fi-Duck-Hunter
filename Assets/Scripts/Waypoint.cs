using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{ 
    [SerializeField]
    private bool _isFinalWaypoint = false;

    [SerializeField] // Unserialize after experimentation
    private bool _isOccupied = false;

    [SerializeField]
    private bool _requiresGhosting; // Ghoster Code

    private bool _isGhosting; // Ghoster 2 Code

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
            // Ghoster 2 Code
            if(_isGhosting == true)
            {
                _isGhosting = false;
            }
            // Ghoster 2 Code
        }

    }

    // Ghoster Code
    public bool IsGhosting()
    {
        //return _requiresGhosting;
        return _isGhosting; // Ghoster 2 Code
    }
    // Ghoster Code

    // Ghoster Code 2
    public void SetGhosting()
    {
        _isGhosting = _requiresGhosting;
    }
    // Ghoster Code 2
}
