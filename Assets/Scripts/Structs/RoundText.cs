using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct RoundText
{
    [SerializeField]
    public float _pauseBeforeTyping;
    [SerializeField]
    public string _text;
    [SerializeField]
    public float _pauseAfterTyping;
    [SerializeField]
    public bool _doesWipeText;
    [SerializeField]
    public bool _doesPrintWholeTextImmediately; // for the 10 in the count down
}

