using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class SniperRifle : MonoBehaviour
{
    private PlayerInputActions _playerInput;

    [SerializeField]
    private AudioClip _shotSoundEffect, _emptyClickSoundEffect, _reloadSoundEffect;

    [SerializeField]
    private LayerMask _enemyMask, _collectableMask;

    [SerializeField]
    private float _collectableReachDistance;

    private GameObject _highLightedGameObject;

    [SerializeField]
    private int _ammo;

    [SerializeField]
    private int _ammoCount;

    [SerializeField]
    private ParticleSystem _bulletSpark, _muzzleFlash;

    private bool wasHittingLastFrame = false;


    // Start is called before the first frame update
    void Start()
    {
        _ammoCount = _ammo;
    }

    private void OnEnable()
    {
        _playerInput = new PlayerInputActions();
        _playerInput.SniperRifle.Enable();
        _playerInput.Collector.Enable();
        _playerInput.SniperRifle.Fire.performed += Fire;
        _playerInput.Collector.Collect.performed += Collect;
    }

    private void Collect(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        RaycastHit hitInfo;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, _collectableReachDistance, _collectableMask))
        {
            if (hitInfo.collider.tag == "AmmoBox" && _ammoCount < _ammo)
            {
                AudioSource.PlayClipAtPoint(_reloadSoundEffect, transform.position, 1.0f);
                _ammoCount++;
            }
        }
    }

    private void Fire(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (_ammoCount > 0)
        {
            _muzzleFlash.Play();
            AudioSource.PlayClipAtPoint(_shotSoundEffect, transform.position, 1.0f);
            RaycastHit hitInfo;

            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, Mathf.Infinity, _enemyMask))
            {
                if (hitInfo.collider.tag == "Duck" && hitInfo.collider.GetComponent<Duck_AI>() != null)
                {
                    hitInfo.collider.GetComponent<Duck_AI>().OnShot();
                }
            }
            else if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, Mathf.Infinity))
            {
                ParticleSystem spark = Instantiate(_bulletSpark, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                spark.Play();
                Destroy(spark, 5.0f);
            }
            _ammoCount--;
        }
        else
        {
            AudioSource.PlayClipAtPoint(_emptyClickSoundEffect, transform.position, 1.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hitInfo;
        bool isHittingThisFrame = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, _collectableReachDistance, _collectableMask);

        
        if(isHittingThisFrame && !wasHittingLastFrame)
        {
            AmmoBox _ammoBoxScript = hitInfo.collider.GetComponent<AmmoBox>();
            if (hitInfo.collider.tag == "AmmoBox" && _ammoBoxScript != null)
            {
                _ammoBoxScript.Highlighted();
                _highLightedGameObject = hitInfo.collider.gameObject;
            }
        }

        // EXIT: was hitting, but no longer is
        if (wasHittingLastFrame && !isHittingThisFrame)
        {
            AmmoBox _ammoBoxScript = _highLightedGameObject.GetComponent<AmmoBox>();
            if (_highLightedGameObject.tag == "AmmoBox" && _ammoBoxScript != null)
            {
                _ammoBoxScript.Unhighlighted();
                _highLightedGameObject = null;
            }
        }

        wasHittingLastFrame = isHittingThisFrame;
    }

    private void OnDisable()
    {
        _playerInput.SniperRifle.Fire.performed -= Fire;
        _playerInput.Collector.Collect.performed -= Collect;
        _playerInput.SniperRifle.Disable();
        _playerInput.Collector.Disable();
    }
}