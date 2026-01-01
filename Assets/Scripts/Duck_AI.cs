using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Duck_AI : MonoBehaviour
{
    private static int _numOfDucks; // Timer Code

    private static HeadStartTimer _headStartTimer; // Timer Code

    private static bool _isCommunicatingWithTimer; // Timer Code

    private static PointSystem _pointSystem; // Point Code

    private enum State
    {
        Running,
        Hiding,
        Waiting, // Occupy Code
        Dead,
        Escaped,
    }

    [SerializeField]
    private State _currentState = State.Running;
    private NavMeshAgent _agent;
    [SerializeField]
    private Waypoint _finalWaypoint;
    [SerializeField]
    private List<Waypoint> _selectedWaypoints;
    [SerializeField]
    private int _currentWaypoint = 0;
    [SerializeField]
    private int _designatedPriority;
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
        _headStartTimer = FindObjectOfType<HeadStartTimer>(); // Timer Code
        _pointSystem = FindObjectOfType<PointSystem>(); // Point Code
      /*  _agent = GetComponent<NavMeshAgent>();

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
        _selectedWaypoints[_currentWaypoint].SetToOccupied(); // Occupy Code */
    }

    public void SetDuckPriority(int priority)
    {
        _designatedPriority = priority;
        _agent.avoidancePriority = _designatedPriority;
    }

    public void SetNumberOfDucks(int number) // Timer Code
    {
        _numOfDucks = number;
        _isCommunicatingWithTimer = true;
    }

    public void DefineWaypoints(List<Waypoint> columnWaypoints, Waypoint finalWaypoint) // SpawnManager Code
    {
        _currentState = State.Running; // Win/Lose Code
        _agent = GetComponent<NavMeshAgent>();
        RandomizeWaypoints(columnWaypoints, finalWaypoint);
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
       // _selectedWaypoints[_currentWaypoint].SetToOccupied(); // Occupy Code ////////////// Maybe add back in later
    }

    private void RandomizeWaypoints(List<Waypoint> columnWaypoints, Waypoint finalWaypoint)
    {
        foreach (Waypoint point in columnWaypoints)
        {
            bool randomBool = Random.value < 0.5f;
            if (randomBool)
            {
                _selectedWaypoints.Add(point);
            }
        }
        _finalWaypoint = finalWaypoint;
        _selectedWaypoints.Add(_finalWaypoint);
    }

    private void SelectFirstWaypoint() // Occupy Code
    {
        foreach (Waypoint waypoint in _selectedWaypoints) // Maybe update this to a for loop later, just for extra accuracy 
        {
            if (waypoint.IsOccupied() == false)
            { 
                waypoint.SetToOccupied();
               // Debug.Log(waypoint + " set to Occupied"); /////////////////
                _agent.SetDestination(waypoint.transform.position);
                if(waypoint == _finalWaypoint) // MAKE SURE THIS ACTUALLY WORKS LATER
                {
                    _isMakingFinalDash = true;
                }
               // _targetedPosition = waypoint.transform.position; ///////////////////////
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

        if (_isCommunicatingWithTimer)
        {
            CommunicateWithTimer(); // Timer Code
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
        _agent.avoidancePriority = 0;
        float _hidingTime = ((Random.value * (_maxHidingTime - _minHidingTime)) + _minHidingTime);
        yield return new WaitForSeconds(_hidingTime);
        _isHiding = false;
        if (_selectedWaypoints[_currentWaypoint].IsOccupied() == false) // Occupy Code
        { // Occupy Code
            _currentState = State.Running;
            _agent.isStopped = false;
            _agent.avoidancePriority = _designatedPriority;
            _selectedWaypoints[_currentWaypoint - 1].SetToUnoccupied(); // Occupy code
           // Debug.Log(_selectedWaypoints[_currentWaypoint - 1] + " set to Unoccupied"); ////////
            _selectedWaypoints[_currentWaypoint].SetToOccupied(); // Occupy code
           // Debug.Log(_selectedWaypoints[_currentWaypoint] + " set to Occupied"); ////////
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
            _agent.avoidancePriority = _designatedPriority;
            _selectedWaypoints[_currentWaypoint - 1].SetToUnoccupied();
           // Debug.Log(_selectedWaypoints[_currentWaypoint - 1] + " set to Unoccupied"); /////
            _selectedWaypoints[_currentWaypoint].SetToOccupied();
           // Debug.Log(_selectedWaypoints[_currentWaypoint] + " set to Occupied"); /////////////
        }
        _isHesitating = false; // Maybe put this in an "else" statement
    }

    private void CommunicateWithTimer() // Timer Code
    {
        if (transform.position.y <= 1.03882f || _numOfDucks >= 6 && _numOfDucks <= 11 && transform.position.y <= 4.63882f)
        {
           // Debug.Log("Duck Triggered Timer");
            _headStartTimer.StartTimer();
            _isCommunicatingWithTimer = false;
        }
    }

    public void OnShot()
    {
        if (_currentState != State.Dead)
        {
            StopAllCoroutines();
            _agent.isStopped = true;
            _isHiding = false;
            _isHesitating = false;
            if (_currentState == State.Running)
            {
                _selectedWaypoints[_currentWaypoint].SetToUnoccupied();
               // Debug.Log(_selectedWaypoints[_currentWaypoint] + " set to Unoccupied"); /////
            }
            else
            {
                _selectedWaypoints[_currentWaypoint - 1].SetToUnoccupied();
               // Debug.Log(_selectedWaypoints[_currentWaypoint - 1] + " set to Unoccupied"); ////
            }
            _currentState = State.Dead;
            _selectedWaypoints = new List<Waypoint>(); // Round Manager Code
            _currentWaypoint = 0; // Round Manager Code
            _pointSystem.CheckDucks(); // Point System
            gameObject.SetActive(false);
            // Destroy(this.gameObject);
            // Trigger Death Animation
        }
    }

    public bool IsDead()
    {
        return _currentState == State.Dead;
    }

    public bool IsEscaped()
    {
        return _currentState == State.Escaped;
    }

    private void Escape()
    {
        // Trigger code to subtract points for when a Duck escapes
        _currentState = State.Escaped;
        _pointSystem.PlayerLost();
        _pointSystem.CheckDucks();
        gameObject.SetActive(false);
        // Destroy(this.gameObject);
    }

        
}