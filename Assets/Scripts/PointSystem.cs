using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointSystem : MonoBehaviour
{
    [SerializeField]
    private int _maxPoints;

    [SerializeField]
    private int _minPoints;

    private int _currentPoints;

    private int _finalTally; // Round Manager Code

    private bool _isPlayerDead;

    [SerializeField]
    private float _pointDecayDuration;

    private List<Duck_AI> _currentDucks;

    private RoundManager _roundManager; // Round Manager Code

    private HeadStartTimer _headStartTimer;

    [SerializeField]
    private SniperRifle _sniper; // Round Manager Code

    // Singleton
    public static PointSystem Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        _roundManager = FindObjectOfType<RoundManager>(); // Round Manager Code
        _headStartTimer = FindObjectOfType<HeadStartTimer>(); // Round Manager Code
    }

    public void SetDucks(List<Duck_AI> ducks)
    {
        _currentDucks = ducks;
    }

    public void StartTimer()
    {
        _currentPoints = _maxPoints;
        StartCoroutine(PointDecay());
    }

    IEnumerator PointDecay() // AI GENERATED: REVIEW AND LEARN FROM LATER!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    {
        float elapsed = 0f;

        while (elapsed < _pointDecayDuration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / _pointDecayDuration;
            _currentPoints = (int)(Mathf.Abs((Mathf.Lerp(_maxPoints, _minPoints, t))));

            yield return null;
        }

        _currentPoints = _minPoints;
    }

  /*  IEnumerator PointDecay()
    {
        float countDown = _pointDecayDuration;
        while (countDown > 0)
        {
            yield return new WaitForSeconds(00.01f);
            countDown -= 00.01f;
            if (countDown <= 0f)
            {
                countDown = 0f;
            }
            _currentPoints = (int)((((countDown / _pointDecayDuration) * ((float)(_maxPoints) - (float)(_minPoints))) + (float)(_minPoints)));
        }
    } */

    public void CheckDucks()
    {
        foreach(var duck in _currentDucks){
            if (duck.IsDead() == false && duck.IsEscaped() == false)
            {
                return;
            }
        }
        _headStartTimer.RestartTimer(); // Round Manager Code
        _sniper.SetCanFireToFalse(); // Round Manager Code

        Debug.Log("Round Ended");
        FinalizeRoundResults(); // Round Manager Code: Originally named FinalizeResults()
    }

    public void PlayerLost()
    {
        _isPlayerDead = true;
    }

    private void FinalizeRoundResults() // Round Manager Code: Originally named FinalizeResults()
    {
        StopAllCoroutines();
        if(_isPlayerDead == false)
        {
            Debug.Log(_currentPoints);
            _finalTally += _currentPoints; // Round Manager Code
            _currentPoints = 0; // Round Manager Code
            _roundManager.CurrentRoundCompleted(); // Round Manager Code
        }
        else
        {
            Debug.Log("Eaten By Ducks");
        }
    }

    public void FinalizeGameResults() // Round Manager Code
    {
        Debug.Log("Final Tally: " + _finalTally);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
