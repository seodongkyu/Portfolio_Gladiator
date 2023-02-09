using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineUtility;
using UnityEngine.AI;



public class MonsterObj : MonoBehaviour
{
    protected Animator _anim;
    protected PlayerController _player;
    CapsuleCollider[] _colliders;
    protected NavMeshAgent _navAgent;
    IngameUIWindow _ingameUIWnd;
    Collider[] _attackColliders;
    [SerializeField] TrailRenderer _trail;
    protected AudioSource _audiosource;
    [SerializeField] AudioSource _hitAudioSource;


    public stMonsterInfo _monInfo { get; set; }

    [HideInInspector] public Vector3 _fireBallDir;
    [SerializeField] float _lookSpeed;
    [SerializeField] float _walkSpeed;
    float _attackTimer,_skillRate = 0;
    protected Vector3 _playerDir;
    protected bool _isAttack = false;
    protected bool _isDead = false;
    bool _isCriticalHit = false;
    public bool _isRotate { get; set; }

    public float _currentHP { get; set; }

    public float baseAudioVolume { get { return _audiosource.volume; } set { _audiosource.volume = _hitAudioSource.volume = value; } }

    protected virtual void InitDatas()
    {
        _anim = GetComponent<Animator>();
        _colliders = GetComponents<CapsuleCollider>();
        _navAgent = GetComponent<NavMeshAgent>();
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        _ingameUIWnd = GameObject.FindGameObjectWithTag("IngameUIWindow").GetComponent<IngameUIWindow>();
        _attackColliders = GameObject.FindGameObjectWithTag("MonsterColliders").GetComponentsInChildren<Collider>();
        _audiosource = GetComponent<AudioSource>();

        _audiosource.volume = DataManager._instance._optionData._SFX_Volume;
        _hitAudioSource.volume = DataManager._instance._optionData._SFX_Volume;
        _monInfo = DataManager._instance.MonsterInfo[DataManager._instance._selectMonster];
        _navAgent.stoppingDistance = _monInfo.AttackRange;
        _currentHP = _monInfo.HP;
        for(int n = 0; n < _attackColliders.Length;n++)
            _attackColliders[n].gameObject.SetActive(false);
        Instantiate(ResourcePoolManager._instance._monsterProjectorObj);
    }


    //  몬스터 업데이트 함수.
    protected void MonsterUpdate()
    {
        Debug.DrawRay(transform.position, _playerDir.normalized * _monInfo.AttackRange, Color.blue);

        if (_isDead || _player._dead) return;

        _attackTimer += Time.deltaTime;
        _skillRate += Time.deltaTime;
        
        if (_anim.GetBool("Scream") || _isAttack || _isCriticalHit) return;    //  몬스터가 스크림중일 때 움직이지 않게 한다.
        _playerDir = _player.transform.position - transform.position;   //  플레이어와 몬스터의 거리,방향 구하기

        if (_skillRate >= _monInfo.SkillRate)
        {
            if (_isRotate)
                _isRotate = false;
            
            SkillAttack();
            return;
        }

        //  플레이어와의 거리가 공격사거리보다 클 경우 플레이어 추격
        if (_playerDir.magnitude > _monInfo.AttackRange)
        {
            if (!_isRotate)
                ChasePlayer();
            else
                if(_attackTimer >= _monInfo.AttackRate)
                    _isRotate = false;
        }
        else // 플레이어와의 거리가 가까울 때 공격한다.
        {
            _anim.SetBool("Move", false);
            _navAgent.speed = 0;
            _navAgent.velocity = Vector3.zero;
            if (_attackTimer >= _monInfo.AttackRate)
            {
                if (_isRotate)
                    _isRotate = false;
                if (_anim.GetInteger("Attack") == 0)
                    Attack();
            }
            else // 플레이어가 공격사거리안에 들어오지만 공격이 쿨타임이면 뒷걸음질 친다.
            {
                if(!_isRotate)
                    StartCoroutine(LookPlayer());
            }
        }
    }

    protected virtual void SkillAttack()
    {
        _anim.SetBool("Move", false);
        _navAgent.speed = 0;
        _navAgent.velocity = Vector3.zero;
        transform.rotation = Quaternion.LookRotation(_playerDir);
        _anim.SetBool("SkillAttack", true);
        _isAttack = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerAttackCollider"))
        {
            GetHit(other);
        }
    }

    void GetHit(Collider other)
    {
        //  기본 데미지
        float MinDamage,MaxDamage;
        if (_player._isSkillAttack)
        {
            //  스킬공격에 맞았다면
            MinDamage = UserInfo._instance._skillAtt - (UserInfo._instance._skillAtt * 0.05f);
            MaxDamage = UserInfo._instance._skillAtt + (UserInfo._instance._skillAtt * 0.05f); 
        }
        else
        {
            //  기본공격에 맞았다면
            MinDamage = UserInfo._instance._att - (UserInfo._instance._att * 0.05f);
            MaxDamage = UserInfo._instance._att + (UserInfo._instance._att * 0.05f);
        }  
        float Damage = Random.Range(MinDamage, MaxDamage);

        _hitAudioSource.clip = ResourcePoolManager._instance._hit_Normal;
        //  치명타 확률 계산
        bool isCritical = false;
        float range = Random.Range(0, 101);
        if(range <= UserInfo._instance._criticalRange)
        {
            Damage = Damage * (UserInfo._instance._criticalDamage / 100);
            isCritical = true;
            _hitAudioSource.clip = ResourcePoolManager._instance._hit_Critical;
            StartCoroutine(_ingameUIWnd.ActiveTrueCriticalUI());
        }
        _currentHP -= Damage;
        _ingameUIWnd.SetMonsterHPBar();
        _hitAudioSource.Play();
        //  히트 이펙트 생성
        RaycastHit hit;
        LayerMask mask = LayerMask.NameToLayer("Monster");
        Vector3 dir = (transform.position - _player.transform.position).normalized;
        if(Physics.Raycast(_player.transform.position,dir,out hit,10,mask))
        {
            GameObject go = Instantiate(ResourcePoolManager._instance._hitEffect, hit.point, Quaternion.identity);
            Destroy(go, 3f);
            Time.timeScale = 0;
        }

        if (_currentHP <= 0)
        {
            _isDead = true;
            _anim.SetTrigger("Die");
            for(int n = 0; n < _colliders.Length;n++)
                _colliders[n].enabled = false;
            _isRotate = false;
            IngameManager._instance.GameClear();
        }
        else if (isCritical)
        {
            if (!_isCriticalHit && !_isAttack)
            {
                _anim.SetTrigger("GetHit");
                _isCriticalHit = true;
            }
        }  
    }

    void ChasePlayer()
    {
        _navAgent.speed = _monInfo.MoveSpeed;
        _anim.SetBool("Move", true);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_playerDir), _lookSpeed * Time.deltaTime);
        Debug.DrawRay(transform.position, _player.transform.position, Color.red);
        _navAgent.SetDestination(_player.transform.position);
        
    }

    protected virtual void Attack()
    {
        int attackNum = Random.Range(1, 3);
        _anim.SetInteger("Attack", attackNum);
        _isAttack = true;
    }

    public IEnumerator LookPlayer()
    {
        _isRotate = true;
        _anim.SetBool("LookPlayer", true);
        while (_isRotate)
        {
            _playerDir = _player.transform.position - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(_playerDir), _lookSpeed * Time.deltaTime);
            transform.position = Vector3.MoveTowards(transform.position, -_playerDir, _walkSpeed * Time.deltaTime);
            yield return null;
        }
        _anim.SetBool("LookPlayer", false);
    }



    #region [AnimationEventFunc]
    public void EndScreamming()
    {
        _anim.SetBool("Scream", false);
    }
    public void EndAttack()
    {
        _attackTimer = 0;
        _anim.SetInteger("Attack", 0);
        _isAttack = false;
    }
    public virtual void EndSkillAttack()
    {
        _attackTimer = 0;
        _skillRate = 0;
        _anim.SetBool("SkillAttack", false);
        _isAttack = false;
    }
    public void EndCriticalHit()
    {
        _isCriticalHit = false;
    }
    public void ActiveTrueAttackCollider()
    {
        _attackColliders[_anim.GetInteger("Attack") - 1].gameObject.SetActive(true);
    }
    public void ActiveFalseAttackCollider()
    {
        _attackColliders[_anim.GetInteger("Attack") - 1].gameObject.SetActive(false);
    }
    public void TrailEffectOn()
    {
        _trail.enabled = true;
    }
    public void TrailEffectOff()
    {
        _trail.enabled = false;
    }
    public void AttackSound(int attackNum)
    {
        if(attackNum == 1)
        {
            _audiosource.clip = ResourcePoolManager._instance._biteSound;
        }
        else if(attackNum == 2)
        {
            _audiosource.clip = ResourcePoolManager._instance._clawSound;
        }
        _audiosource.Play();
    }
    void PlayFootStepSound(int num)
    {
        _audiosource.clip = ResourcePoolManager._instance._monsterStepSounds[num];
        _audiosource.Play();
    }
    #endregion [AnimationEventFunc]
}
