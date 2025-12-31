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

    [SerializeField]
    private float _pointDecayDuration;

    private float _pointTimer; // Change Later

    private List<Duck_AI> _currentDucks;

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
        
    }

    public void SetDucks(List<Duck_AI> ducks)
    {
        _currentDucks = ducks;
    }

    public void StartTimer()
    {
        _currentPoints = _maxPoints;
        _pointTimer = _pointDecayDuration;
        StartCoroutine(PointDecay());
    }

    IEnumerator PointDecay()
    {
        while (_pointTimer >= 0)
        {
            yield return new WaitForSeconds(00.01f);
            _pointTimer -= 00.01f;
            if (_pointTimer < 0f)
            {
                _pointTimer = 0f;
                Debug.Log("PointDecayEnded");
                yield break;
            }
        }
    }


    public void CheckDucks()
    {
        foreach(var duck in _currentDucks){
            if (duck.IsDead() == false)
            {
                return;
            }
        }
        Debug.Log("Round Ended");
        FinalizeResults();
    }

    private void FinalizeResults()
    {
        StopAllCoroutines();
        int finalTally = (int)((((_pointTimer/_pointDecayDuration) * (_maxPoints - _minPoints)) + _minPoints));
        Debug.Log(_currentPoints);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
