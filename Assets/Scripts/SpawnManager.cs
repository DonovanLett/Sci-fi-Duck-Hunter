using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private Duck_AI _duckPrefab;

    private List<Duck_AI> _duckPool = new List<Duck_AI>();

    [SerializeField]
    private int _poolSize = 20;

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

    // Singleton
    public static SpawnManager Instance;

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
        for (int i = 0; i < _poolSize; i++)
        {
            Duck_AI duck = Instantiate(_duckPrefab, transform.position, Quaternion.identity);
            duck.gameObject.SetActive(false);
            _duckPool.Add(duck);
        }
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        for (int i = 0; i < _numberOfDucks; i++)
        {
            float _spawnPause = ((Random.value * (_maxSpawnPause - _minSpawnPause)) + _minSpawnPause);
            yield return new WaitForSeconds(_spawnPause);
            _duckPool[i].gameObject.transform.position = transform.position;
            _duckPool[i].gameObject.transform.rotation = Quaternion.identity;
            _duckPool[i].gameObject.SetActive(true);
            _duckPool[i].DefineWaypoints(_columnWaypoints, _finalWaypoint);
            _duckPool[i].SetDuckPriority(_currentDuckPriority);
            _currentDuckPriority++;
        }
        _currentDuckPriority = 1;
    }

   /* IEnumerator SpawnRoutine()
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
    } */

    // Update is called once per frame
    void Update()
    {
        
    }
}