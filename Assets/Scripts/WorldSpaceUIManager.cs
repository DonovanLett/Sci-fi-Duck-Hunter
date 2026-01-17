using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldSpaceUIManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _narratorText;

    [SerializeField]
    public RoundTextGroup[] _roundTextGroups;

    [SerializeField]
    private AudioClip _clickSoundEffect;

    private RoundManager _roundManager;

    // Singleton
    public static WorldSpaceUIManager Instance;

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
        _roundManager = FindObjectOfType<RoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TriggerFirstRound()
    {
        StartCoroutine(RoundText(0, 0));
    }

    public void TriggerRoundText(int round, int points)
    {
        StartCoroutine(RoundText(round, points));
    }

    IEnumerator RoundText(int round, int points)
    {
        RoundTextGroup currentRound = _roundTextGroups[round];
        for (int i = 0; i < currentRound._roundTexts.Length; i++)
        {
            RoundText currentText = currentRound._roundTexts[i];

            if( currentText._text.Contains("<points>"))
            {
                currentText._text = currentText._text.Replace("<points>", (points).ToString());
            }

            yield return new WaitForSeconds(currentText._pauseBeforeTyping);

            if (currentText._doesPrintWholeTextImmediately)
            {
                AudioSource.PlayClipAtPoint(_clickSoundEffect, transform.position, 1.0f);
                _narratorText.text += currentText._text;
            }
            else
            {
                foreach (char letter in currentText._text)
                {
                    AudioSource.PlayClipAtPoint(_clickSoundEffect, transform.position, 1.0f);
                    _narratorText.text += letter;
                    yield return new WaitForSeconds(0.05f); // Figure out the delay there should be between each letter
                }
            }

            yield return new WaitForSeconds(currentText._pauseAfterTyping);

            if (currentText._doesWipeText)
            {
                _narratorText.text = "";
            }
        }
        _narratorText.text = "";
        _roundManager.TriggerNextRound();
    }
}