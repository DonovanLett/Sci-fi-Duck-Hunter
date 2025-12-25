using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Duck_AI : MonoBehaviour
{
    private enum State
    {
        Running,
        Hiding,
        Waiting, // Occupy Code
        Dead,
    }
    [SerializeField]
    private State _currentState = State.Running;
    private NavMeshAgent _agent;
    [SerializeField]
    private List<Waypoint> _columnWaypoints;
    [SerializeField]
    private Waypoint _finalWaypoint;
    [SerializeField]
    private List<Waypoint> _selectedWaypoints;
    [SerializeField]
    private int _currentWaypoint = 0;
    [SerializeField]
    private bool _isHiding = false;
    [SerializeField]
    private float _minHidingTime, _maxHidingTime;
    [SerializeField]
    private bool _isHesitating = false; // Occupy Code
    [SerializeField]
    private float _minHesitatingTime, _maxHesitatingTime; // Occupy Code
    [SerializeField]
    private bool _isMakingFinalDash = false;

    [SerializeField]
    private Vector3 _targetedPosition;
    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        RandomizeWaypoints();
        if (_selectedWaypoints.Count > 1)
        {
            SelectFirstWaypoint(); // Occupy Code
           // _agent.SetDestination(_selectedWaypoints[_currentWaypoint].transform.position); // Original before Occupy Code
        }
        else
        {
            _agent.SetDestination(_finalWaypoint.transform.position);
            _isMakingFinalDash = true;
        }
        _targetedPosition = _selectedWaypoints[_currentWaypoint].transform.position;
        _selectedWaypoints[_currentWaypoint].SetToOccupied(); // Occupy Code
    }

    private void RandomizeWaypoints()
    {
        foreach (Waypoint point in _columnWaypoints)
        {
            bool randomBool = Random.value < 0.5f;
            if (randomBool)
            {
                _selectedWaypoints.Add(point);
            }
        }
        _selectedWaypoints.Add(_finalWaypoint);
    }

    private void SelectFirstWaypoint() // Occupy Code
    {
        foreach (Waypoint waypoint in _selectedWaypoints) // Maybe update this to a for loop later, just for extra accuracy 
        {
            if (waypoint.IsOccupied() == false)
            { 
                waypoint.SetToOccupied();
                _agent.SetDestination(waypoint.transform.position);
                if(waypoint == _finalWaypoint) // MAKE SURE THIS ACTUALLY WORKS LATER
                {
                    _isMakingFinalDash = true;
                }
                return;
            }
            else
            {
                _currentWaypoint++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (_currentState)
        {
            case State.Running:
                if (transform.position == _targetedPosition)
                {
                    if (_isMakingFinalDash)
                    {
                        Escape();
                    }
                    else
                    {
                        SelectNewWaypoint();
                    }
                }
                break;
            case State.Hiding:
                if(_isHiding == false)
                {
                    StartCoroutine(HidingRoutine());
                    _isHiding = true;
                }
                break;
            case State.Waiting: // Occupy Code
                if (_isHesitating == false)
                {
                    if (_selectedWaypoints[_currentWaypoint].IsOccupied() == false)
                    {
                        StartCoroutine(HesitationTimer());
                        _isHesitating = true;
                    }
                }
                break;
        }
    }

    private void SelectNewWaypoint()
    {
        _currentWaypoint++;
        if (_currentWaypoint < _selectedWaypoints.Count - 1) 
        {
            _agent.SetDestination(_selectedWaypoints[_currentWaypoint].transform.position);
        }
        else
        {
            _agent.SetDestination(_finalWaypoint.transform.position);
            _isMakingFinalDash = true;
        }
        _targetedPosition = _selectedWaypoints[_currentWaypoint].transform.position; //
        _currentState = State.Hiding;
    }

    bool HasReachedDestination(NavMeshAgent agent)
    {
        return !agent.pathPending && // Ensures the path calculation is finished
                agent.hasPath && // Confirms a destination was actually set
               agent.remainingDistance <= agent.stoppingDistance && // Agent is close enough along the NavMesh path
               agent.velocity.sqrMagnitude < 0.01f; // Confirms the agent has fully stopped
    }

    IEnumerator HidingRoutine()
    {
        _agent.isStopped = true;
        float _hidingTime = ((Random.value * (_maxHidingTime - _minHidingTime)) + _minHidingTime);
        yield return new WaitForSeconds(_hidingTime);
        _isHiding = false;
        if (_selectedWaypoints[_currentWaypoint].IsOccupied() == false) // Occupy Code
        { // Occupy Code
            _currentState = State.Running;
            _agent.isStopped = false;
            _selectedWaypoints[_currentWaypoint - 1].SetToUnoccupied(); // Occupy code
            _selectedWaypoints[_currentWaypoint].SetToOccupied(); // Occupy code
        } // Occupy Code
        else // Occupy Code
        { // Occupy Code
            _currentState = State.Waiting; // Occupy Code
        } // Occupy Code
    }

    IEnumerator HesitationTimer() // Occupy Code
    {
        float _hesitationTime = ((Random.value * (_maxHesitatingTime - _minHesitatingTime)) + _minHesitatingTime);
        yield return new WaitForSeconds(_hesitationTime);
        if (_selectedWaypoints[_currentWaypoint].IsOccupied() == false)
        {
            _currentState = State.Running;
            _agent.isStopped = false;
            _selectedWaypoints[_currentWaypoint - 1].SetToUnoccupied();
            _selectedWaypoints[_currentWaypoint].SetToOccupied();
        }
        _isHesitating = false; // Maybe put this in an "else" statement
    }


    public void OnKilled()
    {
        StopAllCoroutines();
        _agent.isStopped = true;
        _isHiding = false;
        _currentState = State.Dead;
        // Figure Out What to Do here in terms of Occupation
        // Trigger Death Animation
    }

    public bool IsDead()
    {
        return _currentState == State.Dead;
    }

    private void Escape()
    {
        // Trigger code to subtract points for when a Duck escapes
        Destroy(this.gameObject);
    }

        
}