using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallObj : MonoBehaviour
{
    [SerializeField] float _speed = 10;
    Rigidbody _rigid;
    
    private void Awake()
    {
        _rigid = GetComponent<Rigidbody>();
    }
    void Start()
    {
        MonsterObj monster = IngameManager._instance._monsterObj;
        _rigid.velocity = monster._fireBallDir.normalized * _speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.gameObject.CompareTag("Monster"))
        {
            //  ���� ���� and ���� ����
            Instantiate(ResourcePoolManager._instance._explodeEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
