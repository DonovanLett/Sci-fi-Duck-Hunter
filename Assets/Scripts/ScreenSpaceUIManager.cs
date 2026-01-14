using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using GameDevHQ.FileBase.Plugins.FPS_Character_Controller;

public class ScreenSpaceUIManager : MonoBehaviour
{
    [Header("In-Game UI")]
    [SerializeField]
    private GameObject _steadyHolder;
    [SerializeField]
    private GameObject _fireHolder;
    [SerializeField]
    private GameObject _reloadHolder;
    [SerializeField]
    private PlayableDirector _fireTimeline;
    [SerializeField]
    private GameObject _ammoHolder, _robotHolder; // Ammo/Robot Code
    [SerializeField]
    private TMP_Text _ammoText, _robotText; // Ammo/Robot Code

    [Header("End UI")]
    [SerializeField]
    private PointSystem _pointSystem;
    [SerializeField]
    private TMP_Text _finalScoreText;
    [SerializeField]
    private AudioSource _finalScoreAudio;
    [SerializeField]
    private PlayableDirector _finalTextTimeline;

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

    public void AmmoNumber(int number)
    {
        _ammoText.text = number.ToString();
    }

    public void RobotNumber(int number)
    {
        _robotText.text = number.ToString();
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

    public void StartFinalTally()
    {
        _pointSystem = FindObjectOfType<PointSystem>();
        StartCoroutine(RisingFinalScore());
    }

    IEnumerator RisingFinalScore()
    {
        _finalScoreAudio.Play();
        for (int i = 1; i < _pointSystem.FinalTally(); i++)
        {
            _finalScoreText.text = i.ToString();
            yield return null;
        }
        _finalScoreAudio.Stop();
        _finalTextTimeline.Play();
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