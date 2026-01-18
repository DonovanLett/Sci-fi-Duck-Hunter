using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private PlayerInputActions _playerInput;

    public static GameManager Instance;

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

    private void OnEnable()
    {
        _playerInput = new PlayerInputActions();
        _playerInput.GameManager.Enable();
        _playerInput.GameManager.Quit.performed += Quit;
    }

    private void Quit(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Application.Quit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
