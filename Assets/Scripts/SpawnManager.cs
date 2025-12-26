using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private Duck_AI _duckPrefab;

    [SerializeField]
    private int _numberOfDucks;

    [SerializeField]
    private List<Waypoint> _columnWaypoints;

    [SerializeField]
    private Waypoint _finalWaypoint;

    [SerializeField]
    private bool _isSpawning;

    [SerializeField]
    private float _minSpawnPause, _maxSpawnPause;

    private static int _currentDuckPriority = 1;
    // Start is called before the first frame update
    void Start()
    {
       StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        _isSpawning = true;
        for (int i = 0; i < _numberOfDucks; i++)
        {
            float _spawnPause = ((Random.value * (_maxSpawnPause - _minSpawnPause)) + _minSpawnPause);
            yield return new WaitForSeconds(_spawnPause);
            Duck_AI _currentDuckBeingSpawned = Instantiate(_duckPrefab, transform.position, Quaternion.identity);
            _currentDuckBeingSpawned.DefineWaypoints(_columnWaypoints, _finalWaypoint);
            _currentDuckBeingSpawned.SetDuckPriority(_currentDuckPriority);
            _currentDuckPriority++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}