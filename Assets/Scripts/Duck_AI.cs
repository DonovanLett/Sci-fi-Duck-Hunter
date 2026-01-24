using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

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
    private Animator _animator; // Robot Code
    private CapsuleCollider _collider;
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

    [Header("Y-Positions To Trigger Head Start Timer")]
    [SerializeField]
    private float _secondLevel;
    [SerializeField]
    private float _firstLevel;

    [Header("Agent Stall Detector")]
    [SerializeField]
    private float stallSpeed = 0.05f;
    [SerializeField]
    private float stallTime = 0.75f;
    [SerializeField]
    private float stalledTimer;
    [SerializeField]
    private Vector3 _stallPosition;
    [SerializeField]
    private bool _isRecorrectingStall;
    [SerializeField]
    private float _sideWaysRecorrectDistance = 2.4f;
    [SerializeField]
    private float _backwardsRecorrectDistance = 0.5f;

    [Header("Sound Effects")]
    [SerializeField]
    private AudioSource _audioSource;
    [SerializeField]
    private AudioClip[] _deathSoundEffects;
    [SerializeField]
    private AudioClip _escapeSoundEffect;

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>(); // Escape Code
        _headStartTimer = FindObjectOfType<HeadStartTimer>(); // Timer Code
        _pointSystem = FindObjectOfType<PointSystem>(); // Point Code
        _collider = GetComponent<CapsuleCollider>();
        if(GetComponent<Animator>() != null) // Robot Code
        { // Robot Code
            _animator = GetComponent<Animator>(); // Robot Code
        } // Robot Code
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
        if (GetComponent<Animator>() != null) // Robot Code
        { // Robot Code
            _animator = GetComponent<Animator>(); // Robot Code
        } // Robot Code
        _currentState = State.Running; // Win/Lose Code
        if(_animator != null)
        {
            _animator.SetFloat("Speed", 3.1f); // Robot Code
        } 
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
                // Side Step Code
                AgentStallDetector();
                if(transform.position == _stallPosition && _isRecorrectingStall)
                {
                    _isRecorrectingStall = false;
                    _agent.SetDestination(_targetedPosition);
                    Debug.Log("Robot " + _designatedPriority + " is back on Track");
                }
                // Side Step Code
                //_animator.SetFloat("Speed", _agent.velocity.magnitude); // Robot Code
                if (transform.position == _targetedPosition)
                {
                    if (_animator != null) // Robot Code
                    { // Robot Code
                        _animator.SetFloat("Speed", 0); // Robot Code
                    } // Robot Code
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
                    if (_animator != null)
                    {
                        _animator.SetBool("Hiding", true); // Robot Code
                    }
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

    // Side Step Code
    private void AgentStallDetector()
    {
        if (!_agent.hasPath || _agent.pathPending)
        {
            stalledTimer = 0f;
            return;
        }

        bool shouldMove = _agent.remainingDistance > _agent.stoppingDistance;

        bool notMoving = _agent.velocity.sqrMagnitude < stallSpeed * stallSpeed;

        if (shouldMove && notMoving)
        {
            stalledTimer += Time.deltaTime;

            if (stalledTimer >= stallTime)
            {
                stalledTimer = 0;
                _isRecorrectingStall = true; // Maybe
                TryLocalSidestep();
            }
        }
        else
        {
            stalledTimer = 0f;
        }
    }

    private void TryLocalSidestep()
    {
        stalledTimer = 0;
        Vector3 right = transform.right * _sideWaysRecorrectDistance; // Originally .75, then 1.5, 1.75 (3 works really well)
        Vector3 left = -right;

        Debug.Log("Robot " + _designatedPriority + " is attempting Left");
        if (TryMove(left)) { return; }

        Debug.Log("Robot " + _designatedPriority + " is attempting Right");
        if (TryMove(right)) { return; }

        TryBackstep();
    }

    private bool TryMove(Vector3 offset)
    {
        Vector3 target = transform.position + offset;

        if (NavMesh.SamplePosition(target, out var hit, 1f, _agent.areaMask))
        {
            _stallPosition = hit.position;
            _agent.SetDestination(_stallPosition);
            return true;
        }

        return false;
    }

    private void TryBackstep()
    {
        Debug.Log("Robot " + _designatedPriority + " is attempting Backstep");
        Vector3 back = -_agent.transform.forward * _backwardsRecorrectDistance;

        if (NavMesh.SamplePosition(_agent.transform.position + back, out var hit, 1f, _agent.areaMask))
        {
            _stallPosition = hit.position;
            _agent.SetDestination(_stallPosition);
        }
    }
    // Side Step Code


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
            if (_animator != null)
            {
                _animator.SetBool("Hiding", false); // Robot Code
                _animator.SetFloat("Speed", 3.1f); // Robot Code
            }
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
            if(_animator != null) { 
                _animator.SetBool("Hiding", false); // Robot Code
                _animator.SetFloat("Speed", 3.1f); // Robot Code
            }
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
        if (transform.position.y <= _firstLevel || _numOfDucks >= 6 && _numOfDucks <= 11 && transform.position.y <= _secondLevel) // Robot Code (originally 1.03882 and 4.63882)
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
            // Extra Code
            int deathSoundEffectNumber = Random.Range(0, _deathSoundEffects.Length);
            PlayClipWithSourceSettings(_audioSource, _deathSoundEffects[deathSoundEffectNumber], transform.position);
            //Extra Code
            StopAllCoroutines();
            _isMakingFinalDash = false; //
            _collider.enabled = false;
            _agent.isStopped = true;
            _agent.avoidancePriority = 99; // AI
            _agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance; // AI
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
            if (_animator != null) // Robot Code
            { // Robot Code
                _animator.SetFloat("Speed", 0); // Robot Code
                _animator.SetTrigger("Death"); // Robot Code
            } // Robot Code
            else // Robot Code
            { // Robot Code
                gameObject.SetActive(false);
            } // Robot Code

           // gameObject.SetActive(false); // Crossed out for Robot Code
        }
    }

    public void OnDeathAnimationComplete() // Robot Code
    {
        _animator.ResetTrigger("Death");
        gameObject.SetActive(false);
        _collider.enabled = true;
        _agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance; // AI
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
        PlayClipWithSourceSettings(_audioSource, _escapeSoundEffect, transform.position); // Escape Code
        _pointSystem.PlayerLost();
        _pointSystem.CheckDucks();
        gameObject.SetActive(false);
        // Destroy(this.gameObject);
    }

    // Extra Code
    private void PlayClipWithSourceSettings(AudioSource sourceTemplate, AudioClip clip, Vector3 position)
    {
        GameObject tempGO = new GameObject("OneShotAudio");
        tempGO.transform.position = position;

        AudioSource tempSource = tempGO.AddComponent<AudioSource>();

        // Copy settings
        tempSource.outputAudioMixerGroup = sourceTemplate.outputAudioMixerGroup;
        tempSource.spatialBlend = sourceTemplate.spatialBlend;
        tempSource.volume = sourceTemplate.volume;
        tempSource.pitch = sourceTemplate.pitch;
        tempSource.rolloffMode = sourceTemplate.rolloffMode;
        tempSource.minDistance = sourceTemplate.minDistance;
        tempSource.maxDistance = sourceTemplate.maxDistance;
        tempSource.dopplerLevel = sourceTemplate.dopplerLevel;
        tempSource.spread = sourceTemplate.spread;

        tempSource.clip = clip;
        tempSource.Play();

        Object.Destroy(tempGO, clip.length / tempSource.pitch);
    }
    // Extra

}