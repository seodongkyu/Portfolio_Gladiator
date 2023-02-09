using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_NighjtMare : MonsterObj
{
    [SerializeField] float _buttSpeed;
    [SerializeField] TrailRenderer _clawTrail;

    public bool _isButt { get; set; }

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
    }

    #region[AnimationEventFunc]
    IEnumerator HeadButt()
    {
        _isButt = true;
        Vector3 dir = _player.transform.position;
        _anim.SetInteger("Attack",3);
        //  돌진 사운드
        Debug.Log("DASH AUDIO");
        _audiosource.clip = ResourcePoolManager._instance._dashSound;
        _audiosource.Play();
        ActiveTrueAttackCollider();
        while (_isButt)
        {
            transform.position = Vector3.MoveTowards(transform.position, dir, _buttSpeed * Time.deltaTime);
            yield return null;
        }
        ActiveFalseAttackCollider();
        EndAttack();
    }
    IEnumerator RotateToPlayer()
    {
        while(!_isButt && !_isDead)
        {
            transform.rotation = Quaternion.LookRotation(_player.transform.position - transform.position);
            yield return null;
        }
    }
    public void EndButt()
    {
        if(_isButt)
        {
            _audiosource.clip = ResourcePoolManager._instance._nightMare_ButtBump;
            _audiosource.Play();
        }
        _anim.SetBool("SkillAttack", false);
        _isButt = false;
    }
    public void ClawTrailEffectOn()
    {
        _clawTrail.enabled = true;
    }
    public void ClawTrailEffectOff()
    {
        _clawTrail.enabled = false;
    }
    #endregion[AnimationEventFunc]
}
