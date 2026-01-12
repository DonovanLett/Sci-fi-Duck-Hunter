using System;
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

    private WorldSpaceUIManager _worldSpaceUI; // UI Code

    private ScreenSpaceUIManager _screenSpaceUI;

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

    private void OnValidate() // UI Code
    {
        _worldSpaceUI = FindObjectOfType<WorldSpaceUIManager>(); // UI Code
        if (_rounds == null || _worldSpaceUI._roundTextGroups == null)
            return;

        if (_worldSpaceUI._roundTextGroups.Length != _rounds.Length)
        {
            System.Array.Resize(ref _worldSpaceUI._roundTextGroups, _rounds.Length);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
       // _spawnManager = FindObjectOfType<SpawnManager>();
        _pointSystem = FindObjectOfType<PointSystem>();
        _screenSpaceUI = FindObjectOfType<ScreenSpaceUIManager>(); // UI Code
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartFirstRound()
    {
        _spawnManager = FindObjectOfType<SpawnManager>();
        _worldSpaceUI = FindObjectOfType<WorldSpaceUIManager>(); // UI Code
       // _worldSpaceUI.TriggerRoundText(_currentRound, 0); //  UI Code
       // _spawnManager.StartRound(_rounds[_currentRound]); // Crossed out for UI Code
    }

    public void CurrentRoundCompleted(int points)
    {
        _screenSpaceUI.SwitchOffAll(); // UI Code
        if (_currentRound == (_rounds.Length - 1))
        {
            _pointSystem.FinalizeGameResults();
        }
        else
        {
            _currentRound++;
            _worldSpaceUI.TriggerRoundText(_currentRound, points); //  UI Code
           // _spawnManager.StartRound(_rounds[_currentRound]); // Crossed out for UI Code
        }
    }

    public void TriggerNextRound() // UI Code
    {
        _screenSpaceUI.TriggerSteadyText(); // UI Code
        _spawnManager.StartRound(_rounds[_currentRound]);
    }
}