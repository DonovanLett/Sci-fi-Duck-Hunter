using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadStartTimer : MonoBehaviour
{
    [SerializeField]
    private SniperRifle _sniperRifle;
    [SerializeField]
    private bool _isTimerStarted;
    [SerializeField]
    private bool _isTimerDone;

    public static HeadStartTimer Instance;

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

    public void StartTimer()
    {
        if(_isTimerDone == false && _isTimerStarted == false)
        {
            StartCoroutine(Timer());
            _isTimerStarted = true;
        }
    }

    IEnumerator Timer()
    {
        float _timer = ((Random.value * (4.0f - 3.0f)) + 3.0f);
        yield return new WaitForSeconds(_timer);
        _sniperRifle.SetCanFireToTrue();
        Debug.Log("Player Can Shoot");
        // Give Player Ability To Shoot
        _isTimerDone = true;
    }

    public void OverrideTimer()
    {
        if(_isTimerDone == false)
        {
            StopAllCoroutines();
            // Give Player Ability To Shoot
            _isTimerDone = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}