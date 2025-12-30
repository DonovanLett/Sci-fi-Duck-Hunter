using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBox : MonoBehaviour
{

    [SerializeField]
    private Material _material;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Highlighted()
    {
        _material.EnableKeyword("_EMISSION");
    }

    public void Unhighlighted()
    {
        _material.DisableKeyword("_EMISSION");
    }

    // Update is called once per frame
    void Update()
    {

    }
}