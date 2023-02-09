using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Toad : MonsterObj
{
    public Transform _fireGenPos;

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

    protected override void SkillAttack()
    {
        base.SkillAttack();
        _fireBallDir = _player.transform.position - _fireGenPos.position;
        _fireBallDir.y += 4;
    }

    #region[AnimationEventFunc]
    public void InstantiateFireBall()
    {
        Instantiate(ResourcePoolManager._instance._fireBallPrefab, _fireGenPos.position, transform.rotation);
    }
    #endregion[AnimationEventFunc]
}
