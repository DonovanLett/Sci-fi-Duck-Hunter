using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    [SerializeField]
    private int[] _rounds;

    [SerializeField]
    private int _currentRound;

    private SpawnManager _spawnManager;

    private PointSystem _pointSystem;

    // Singleton
    public static RoundManager Instance;

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
        _spawnManager = FindObjectOfType<SpawnManager>();
        _pointSystem = FindObjectOfType<PointSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartFirstRound()
    {
        _spawnManager.StartRound(_rounds[_currentRound]);
    }

    public void CurrentRoundCompleted()
    {
        if (_currentRound == (_rounds.Length - 1))
        {
            _pointSystem.FinalizeGameResults();
        }
        else
        {
            _currentRound++;
            _spawnManager.StartRound(_rounds[_currentRound]);
        }
    }
}