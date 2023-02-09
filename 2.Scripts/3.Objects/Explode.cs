using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explode : MonoBehaviour
{
    [SerializeField] SphereCollider _collider;
    [SerializeField] float _destroyTime = 2;
    AudioSource _Audiosource;
    private void Start()
    {
        _Audiosource = GetComponent<AudioSource>();
        _Audiosource.volume = DataManager._instance._optionData._SFX_Volume;
        _Audiosource.Play();
        Destroy(gameObject, _destroyTime);
        Invoke("DisableCollider", 0.2f);
    }
    void DisableCollider()
    {
        _collider.enabled = false;
    }
}
