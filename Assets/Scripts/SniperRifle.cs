using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class SniperRifle : MonoBehaviour
{
    private PlayerInputActions _playerInput;

    [SerializeField]
    private LayerMask _enemyMask;



    [SerializeField]
    private int _ammo;

    [SerializeField]
    private int _ammoCount;

    [SerializeField]
    private ParticleSystem _bulletSpark, _muzzleFlash;


    // Start is called before the first frame update
    void Start()
    {
        _ammoCount = _ammo;
    }

    private void OnEnable()
    {
        _playerInput = new PlayerInputActions();
        _playerInput.SniperRifle.Enable();
        _playerInput.SniperRifle.Fire.performed += Fire;
    }

    private void Fire(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (_ammoCount > 0)
        {
            _muzzleFlash.Play();
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
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDisable()
    {
        _playerInput.SniperRifle.Fire.performed -= Fire;
        _playerInput.SniperRifle.Disable();
    }
}
