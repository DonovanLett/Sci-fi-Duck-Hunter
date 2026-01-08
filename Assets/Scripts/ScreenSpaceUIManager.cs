using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class ScreenSpaceUIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _steadyHolder;
    [SerializeField]
    private GameObject _fireHolder;
    [SerializeField]
    private GameObject _reloadHolder;
    [SerializeField]
    private PlayableDirector _fireTimeline;

    // Singleton
    public static ScreenSpaceUIManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void TriggerSteadyText()
    {
        _steadyHolder.SetActive(true);
        _fireHolder.SetActive(false);
        if (_fireTimeline.state == PlayState.Playing)
        {
            _fireTimeline.Stop();
        }
        _reloadHolder.SetActive(false);
    }

    public void TriggerFireText()
    {
        _steadyHolder.SetActive(false);
        _fireHolder.SetActive(true);
        _fireTimeline.Play();
    }

    public void TriggerReloadText()
    {
        _steadyHolder.SetActive(false);
        _fireHolder.SetActive(false);
        if (_fireTimeline.state == PlayState.Playing)
        {
            _fireTimeline.Stop();
        }
        _reloadHolder.SetActive(true);
    }

    public void SwitchOffReloadText()
    {
        _reloadHolder.SetActive(false);
    }

    public void SwitchOffAll()
    {
        _steadyHolder.SetActive(false);
        _fireHolder.SetActive(false);
        if (_fireTimeline.state == PlayState.Playing)
        {
            _fireTimeline.Stop();
        }
        _reloadHolder.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
