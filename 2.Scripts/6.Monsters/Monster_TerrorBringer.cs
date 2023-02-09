using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_TerrorBringer : MonsterObj
{
    [SerializeField] Transform _flamePos;
    GameObject flameObj;

    protected override void InitDatas()
    {
        base.InitDatas();
    }

    void Start()
    {
        InitDatas();
        _anim.SetBool("Scream", true);
    }

    void Update()
    {
        MonsterUpdate();
    }


    #region[AnimationEventFunc]
    void FlameAttack()
    {
        flameObj = Instantiate(ResourcePoolManager._instance._flamePrefab, _flamePos);
    }
    #endregion[AnimationEventFunc]
}
