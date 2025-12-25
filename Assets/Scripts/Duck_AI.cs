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
        Dead,
    }
    [SerializeField]
    private State _currentState = State.Running;
    private NavMeshAgent _agent;
    [SerializeField]
    private List<Transform> _columnWaypoints;
    [SerializeField]
    private Transform _finalWaypoint;
    [SerializeField]
    private List<Transform> _selectedWaypoints;
    [SerializeField]
    private int _currentWaypoint = 0;
    [SerializeField]
    private bool _isHiding = false;
    [SerializeField]
    private float _minHidingTime, _maxHidingTime;
    [SerializeField]
    private bool _isMakingFinalDash = false;
    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        RandomizeWaypoints();
        if (_selectedWaypoints.Count > 1)
        {
            _agent.SetDestination(_selectedWaypoints[_currentWaypoint].position);
        }
        else
        {
            _agent.SetDestination(_finalWaypoint.position);
            _isMakingFinalDash = true;
        }
    }

    private void RandomizeWaypoints()
    {
        foreach (Transform point in _columnWaypoints)
        {
            bool randomBool = Random.value < 0.5f;
            if (randomBool)
            {
                _selectedWaypoints.Add(point);
            }
        }
        _selectedWaypoints.Add(_finalWaypoint);
    }

    // Update is called once per frame
    void Update()
    {
        switch (_currentState)
        {
            case State.Running:
                if (HasReachedDestination(_agent))
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
        }
    }

    private void SelectNewWaypoint()
    {
        _currentWaypoint++;
        if (_currentWaypoint < _selectedWaypoints.Count - 1) 
        {
            _agent.SetDestination(_selectedWaypoints[_currentWaypoint].position);
        }
        else
        {
            _agent.SetDestination(_finalWaypoint.position);
            _isMakingFinalDash = true;
        }
        _currentState = State.Hiding;
    }

    bool HasReachedDestination(NavMeshAgent agent)
    {
        return !agent.pathPending && // Ensures the path calculation is finished
                agent.hasPath && // Confirms a destination was actually set
               agent.remainingDistance <= agent.stoppingDistance && // Agent is close enough along the NavMesh path
               agent.velocity.sqrMagnitude < 0.01f; // Confirms the agent has fully stopped
    } 

  /*  bool HasReachedDestination(NavMeshAgent agent)
    {
        return agent.remainingDistance <= agent.stoppingDistance;
    } */

    IEnumerator HidingRoutine()
    {
        _agent.isStopped = true;
        float _hidingTime = ((Random.value * (_maxHidingTime - _minHidingTime)) + _minHidingTime);
        yield return new WaitForSeconds(_hidingTime);
        _currentState = State.Running;
        _isHiding = false;
        _agent.isStopped = false;
    }


    public void OnKilled()
    {
        StopAllCoroutines();
        _agent.isStopped = true;
        _isHiding = false;
        _currentState = State.Dead;
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