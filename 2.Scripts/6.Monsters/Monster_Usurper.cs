using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Usurper : MonsterObj
{
    [SerializeField] Transform _flamePos;
    GameObject flameObj;
    [SerializeField] float _buttSpeed;
    Vector3 _buttDir;
    bool _isButt = false;
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

    protected override void Attack()
    {
        int attackNum = Random.Range(1, 3);
        _anim.SetInteger("Attack", attackNum);
        _isAttack = true;
        if (attackNum == 2)
            _buttDir = _player.transform.position;
    }

    IEnumerator HeadButt()
    {
        _isButt = true;
        _anim.SetInteger("Attack", 2);
        //  돌진 사운드
        _audiosource.clip = ResourcePoolManager._instance._dashSound;
        _audiosource.Play();
        ActiveTrueAttackCollider();
        while (_isButt)
        {
            transform.position = Vector3.MoveTowards(transform.position, _buttDir, _buttSpeed * Time.deltaTime);
            yield return null;
        }
        ActiveFalseAttackCollider();
        EndAttack();
    }

    #region[AnimationEventFunc]
    void FlameAttack()
    {
        flameObj = Instantiate(ResourcePoolManager._instance._flamePrefab, _flamePos);
    }
    public override void EndSkillAttack()
    {
        base.EndSkillAttack();
    }
    void EndButt()
    {
        _isButt = false;
    }
    #endregion[AnimationEventFunc]
}