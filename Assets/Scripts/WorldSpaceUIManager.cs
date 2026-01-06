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

    private RoundManager _roundManager;
    // Start is called before the first frame update
    void Start()
    {
        _roundManager = FindObjectOfType<RoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TriggerRoundText(int round)
    {
        StartCoroutine(RoundText(round));
    }

    IEnumerator RoundText(int round)
    {
        RoundTextGroup currentRound = _roundTextGroups[round];
        for (int i = 0; i < currentRound._roundTexts.Length; i++)
        {
            RoundText currentText = currentRound._roundTexts[i];

            yield return new WaitForSeconds(currentText._pauseBeforeTyping);

            if (currentText._doesPrintWholeTextImmediately)
            {
                _narratorText.text = currentText._text;
            }
            else
            {
                foreach (char letter in currentText._text)
                {
                    //AudioSource.PlayClipAtPoint(_typeWriterSoundEffect, _camera.transform.position, 90f);
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