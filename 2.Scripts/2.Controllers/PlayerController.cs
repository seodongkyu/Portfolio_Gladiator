using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineUtility;

public class PlayerController : MonoBehaviour
{
    Animator _anim;
    CharacterController _charControl;
    [HideInInspector] public AudioSource _audioSource;
    public Collider[] _swordColliders;

    public Transform _weaponPos;
    [SerializeField] float _moveFastSpeed;
    [SerializeField] float _moveSlowSpeed;
    [SerializeField] float _dashSpeed;
    [SerializeField] float _swordSkillMoveSpeed;
    [SerializeField] float _flyBackSpeed = 10f;
    public float _dashRate;
    public float _skillRate;
    [SerializeField] Transform _effectPos;
    Transform _left, _right;
    IngameUIWindow _ingameUIWnd;



    Vector3 _moveVec = Vector3.zero;
    Transform _camTransform;

    [HideInInspector] public bool _isDash = false;
    [HideInInspector] public bool _isDead = false;
    bool _isNormalAttack = false;
    public bool _isSkillAttack { get; set; }
    bool _isSkillMove = false;
    bool _isHit = false;
    bool _isSkillHit = false;
    bool _isFlyBack = false;

    float _moveSpeed;
    float _dashTimer = 0;
    float _skillTimer = 0;
    int _effectNum = 0;
    int _footstepNum = 0;
    [HideInInspector] public GameObject _currentWeapon;
    public bool _dead { get { return _isDead; } }


    void Awake()
    {
        _anim = GetComponent<Animator>();
        _charControl = GetComponent<CharacterController>();
        _camTransform = Camera.main.transform;
        _left = GameObject.FindGameObjectWithTag("LookLeft").transform;
        _right = GameObject.FindGameObjectWithTag("LookRight").transform;
        _audioSource = GetComponent<AudioSource>();

        _audioSource.volume = DataManager._instance._optionData._SFX_Volume;
        _skillTimer = _skillRate;
        _dashTimer = _dashRate;
        UserInfo._instance._isOpenedUI = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SetAttackColliders();
        //  초기 무기 새팅
        stWeaponInfo weapon = DataManager._instance.WeaponTable["한손검"][UserInfo._instance._weaponIndex];
        UserInfo._instance.SetWeaponInfo(weapon.Index, weapon.Attack, weapon.SkillAttack, weapon.AttackSpeed, weapon.CriticalRange, weapon.CriticalDamage);
        _currentWeapon = Instantiate(ResourcePoolManager._instance._weapons[UserInfo._instance._weaponIndex - 1], _weaponPos);
        UserInfo._instance._currentHP = UserInfo._instance._maxHP;
    }

    void Update()
    {
        if (UserInfo._instance._isOpenedUI)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            _anim.SetBool("Move_Forward", false);
            _anim.SetBool("Move_Back", false);
            _anim.SetBool("Move_Left", false);
            _anim.SetBool("Move_Right", false);
            return;
        }
        _dashTimer += Time.deltaTime;
        _skillTimer += Time.deltaTime;

        if (_isDash || _isSkillAttack || _isHit || _isDead || _isSkillHit) return;
        //  이동 및 스킬 공격
        if (!_isNormalAttack)
        {
            MoveCharacter();

            if (Input.GetMouseButtonDown(1))
            {
                if (_skillTimer < _skillRate) return;

                _skillTimer = 0;
                _anim.SetTrigger("Skill_Attack");
                _isSkillAttack = true;
                _isSkillMove = true;

                StartCoroutine(SetTransformToSkill());
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space))    //  기본공격 중일 때 Dash 가능
        {
            MoveCharacter();
            EndNormalAttackEventFunc(2);
        }

        //  기본 공격
        if (Input.GetMouseButtonDown(0))
        {
            _anim.speed = UserInfo._instance._attSpeed;
            if (!_isNormalAttack)
            {
                _anim.SetTrigger("FirstAttack");
                _anim.SetBool("IsAttack", true);
                _isNormalAttack = true;
            }
            else
            {
                if (_anim.GetBool("ThirdAttack")) return;

                if (!_anim.GetBool("SecondAttack"))
                    _anim.SetBool("SecondAttack", true);
                else
                    _anim.SetBool("ThirdAttack", true);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("MonsterAttackCollider") || other.gameObject.CompareTag("MonsterSkill") || other.gameObject.CompareTag("ButtAttack"))
        {
            if (_isDead) return;
            if (_ingameUIWnd == null)
                _ingameUIWnd = GameObject.FindGameObjectWithTag("IngameUIWindow").GetComponent<IngameUIWindow>();
            //  데미지 계산
            float MinDMG = 0;
            float MaxDMG = 0;
            if(other.gameObject.CompareTag("MonsterAttackCollider"))
            {
                MinDMG = IngameManager._instance._monsterObj._monInfo.Attack - IngameManager._instance._monsterObj._monInfo.Attack * 0.05f;
                MaxDMG = IngameManager._instance._monsterObj._monInfo.Attack + IngameManager._instance._monsterObj._monInfo.Attack * 0.05f;
            }
            else if(other.gameObject.CompareTag("MonsterSkill") || other.gameObject.CompareTag("ButtAttack"))
            {
                Monster_NighjtMare nightMare = GameObject.FindGameObjectWithTag("Monster").GetComponent<Monster_NighjtMare>();
                if (nightMare != null)
                    nightMare.EndButt();
                MinDMG = IngameManager._instance._monsterObj._monInfo.SkillAttack - IngameManager._instance._monsterObj._monInfo.SkillAttack * 0.05f;
                MaxDMG = IngameManager._instance._monsterObj._monInfo.SkillAttack + IngameManager._instance._monsterObj._monInfo.SkillAttack * 0.05f;
                _isSkillHit = true;
            }
            
            float Damage = Random.Range(MinDMG, MaxDMG);
            UserInfo._instance._currentHP -= Damage;
            _ingameUIWnd.SetPlayerHPBar();
            if (UserInfo._instance._currentHP <= 0)
            {
                // 플레이어 사망
                _isDead = true;
                _anim.SetTrigger("isDead");
                //  게임오버UI
                StartCoroutine(IngameManager._instance.GameOver());
            }
            else
            {
                if(_isSkillHit)
                {
                    // 플레이어 적방향으로 회전
                    MonsterObj monster = GameObject.FindGameObjectWithTag("IngameManager").GetComponent<IngameManager>()._monsterObj;
                    transform.rotation = Quaternion.LookRotation(monster.transform.position - transform.position);
                    _anim.SetTrigger("SkillHit");
                }
                else
                {
                    _anim.SetTrigger("Hit");
                    _isHit = true;
                }
            }
        }
    }

    //  플레이어의 공격 콜라이더들을 모두 가져온 뒤, 꺼준다.
    void SetAttackColliders()
    {
        _swordColliders = GameObject.FindGameObjectWithTag("SwordColliders").GetComponentsInChildren<Collider>();
        for(int n = 0; n < _swordColliders.Length;n++)
            _swordColliders[n].gameObject.SetActive(false);
    }


    #region [Move Character]
    void MoveCharacter()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        float rotDir = 0;
        _moveVec.x = moveX;
        _moveVec.z = moveZ;
        
        //  이동을 하지 않을 때 이동 애니메이션들 끄기
        if (_moveVec == Vector3.zero)
        {
            _anim.SetBool("Move_Forward", false);
            _anim.SetBool("Move_Back", false);
            _anim.SetBool("Move_Left", false);
            _anim.SetBool("Move_Right", false);
            _audioSource.Stop();
            //  오디오도 끄기
            return;
        }

        //  이동 방향이 앞 또는 뒤가 있을 때 좌우AxisRaw값으로 대각선으로 가야할지 체크
        if (_moveVec.z > 0)
        {
            rotDir = 45 * _moveVec.x;
            _moveVec.x = 0;
        }   
        else if (_moveVec.z < 0)
        {
            rotDir = 45 * -_moveVec.x;
            _moveVec.x = 0;
        }

        //  이동방향으로 케릭터 회전 함수
        SetDirectionCharacter(rotDir);

        //  이동값이 있을 때 Dash가 가능
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_dashRate > _dashTimer) return;

            _audioSource.clip = ResourcePoolManager._instance._dashSound;
            _audioSource.Play();
            _dashTimer = 0;
            _anim.SetTrigger("Move_Dash");
            _isDash = true;
            StartCoroutine(MoveDash());
            Vector3 dashSpawnPos = transform.position;
            dashSpawnPos.y += 3;
            _moveVec = transform.TransformDirection(_moveVec);
            GameObject dash = Instantiate(ResourcePoolManager._instance._dashEffect, dashSpawnPos,Quaternion.LookRotation(_moveVec));
            Destroy(dash, 2);
        }
        else
            _moveSpeed = SetAnimationCharacter();

        _moveVec = _moveVec * _moveSpeed;
        _moveVec = transform.TransformDirection(_moveVec);
        _charControl.SimpleMove(_moveVec);
    }// void MoveCharacter()=============================

    void SetDirectionCharacter(float dir)
    {
        //   카메라 방향 기준 진행방향 설정
        if (_moveVec == Vector3.zero) return;

        Quaternion camDir = _camTransform.rotation;
        
        if(dir == 45)
            camDir = _right.rotation;
        else if(dir == -45)
            camDir = _left.rotation;

        camDir.x = camDir.z = 0;
        transform.rotation = camDir;
    }

    //  케릭터가 움직이는 방향에 맞는 애니메이션을 틀어주고,방향에 맞는 이동속도를 return시킨다.
    float SetAnimationCharacter()
    {
        if (_moveVec.z > 0)
        {
            _anim.SetBool("Move_Forward", true);
            _anim.SetBool("Move_Back", false);
            _anim.SetBool("Move_Right", false);
            _anim.SetBool("Move_Left", false);
            
        }
        else if (_moveVec.z < 0)
        {
            _anim.SetBool("Move_Forward", false);
            _anim.SetBool("Move_Back", true);
            _anim.SetBool("Move_Right", false);
            _anim.SetBool("Move_Left", false);
            return _moveSlowSpeed;
        }
        else if (_moveVec.x > 0)
        {
            //  오른쪽
            _anim.SetBool("Move_Forward", false);
            _anim.SetBool("Move_Back", false);
            _anim.SetBool("Move_Right", true);
            _anim.SetBool("Move_Left", false);
        }
        else if (_moveVec.x < 0)
        {
            //   왼쪽
            _anim.SetBool("Move_Forward", false);
            _anim.SetBool("Move_Back", false);
            _anim.SetBool("Move_Right", false);
            _anim.SetBool("Move_Left", true);
        }
        return _moveFastSpeed;
    }

    IEnumerator MoveDash()
    {
        float timer = 0;
        float dashRate = 0.2f;
        Vector3 dashDir = transform.TransformDirection(_moveVec);
        while (true)
        {
            timer += Time.deltaTime;
            if (timer >= dashRate) break;
            _charControl.SimpleMove(dashDir.normalized * _dashSpeed);
            yield return null;
        }
        
        _isDash = false;
        yield return new WaitForSeconds(0.3f);
        _audioSource.Stop();
    }
    IEnumerator SetTransformToSkill()
    {
        while(_isSkillMove)
        {
            _charControl.Move(transform.forward * _swordSkillMoveSpeed * Time.deltaTime);
            yield return null;
        }
    }
    #endregion [Move Character]

    #region [Animation Event Func]
    void StartNormalAttackEventFunc(int attackNum)
    {
        //  다른 콜라이더 다 끄기
        ActiveFalseAttackCollider();
        _swordColliders[attackNum].gameObject.SetActive(true);
        InstantiateAttackEffect();
        _audioSource.clip = ResourcePoolManager._instance._AttackSound;
        _audioSource.Play();
    }
    void EndNormalAttackEventFunc(int attackNum)
    {
        //  공격 콜라이더 모두 끄기.
        ActiveFalseAttackCollider();
        switch (attackNum)
        {
            case 0:
                
                if (!_anim.GetBool("SecondAttack"))
                {
                    _anim.SetBool("IsAttack", false);
                    _isNormalAttack = false;
                    _anim.speed = 1;
                    _effectNum = 0;
                }
                break;
            case 1:
                _anim.SetBool("SecondAttack", false);
                if (!_anim.GetBool("ThirdAttack"))
                {
                    _anim.SetBool("IsAttack", false);
                    _isNormalAttack = false;
                    _anim.speed = 1;
                    _effectNum = 0;
                }
                break;
            case 2:
                _anim.SetBool("SecondAttack", false);
                _anim.SetBool("ThirdAttack", false);
                _anim.SetBool("IsAttack", false);
                _isNormalAttack = false;
                _anim.speed = 1;
                _effectNum = 0;
                break;
            case 3://   SkillAttack
                break;
        }
    }
    void ActiveFalseAttackCollider()
    {
        for (int n = 0; n < _swordColliders.Length; n++)
            _swordColliders[n].gameObject.SetActive(false);
    }
    void EndSkillAttackEventFunc()
    {
        _isSkillAttack = false;
        _audioSource.Stop();
    }
    void StopSkillMove()
    {
        ActiveFalseAttackCollider();
        _isSkillMove = false;
    }
    void EndPlayerHit()
    {
        _isHit = false;
        _isSkillHit = false;
    }
    void PlayerHit()
    {
        ActiveFalseAttackCollider();
        _anim.SetBool("IsAttack", false);
        _anim.SetBool("SecondAttack", false);
        _anim.SetBool("ThirdAttack", false);
        _isSkillAttack = false;
        _isNormalAttack = false;
        _isSkillMove = false;
    }
    IEnumerator PlayerFlyBack()
    {
        _isFlyBack = true;
        while (_isFlyBack)
        {
            _charControl.SimpleMove(-transform.forward * _flyBackSpeed);
            yield return null;
        }
    }
    void EndFlyBack()
    {
        _isFlyBack = false;
    }
    void InstantiateAttackEffect()
    {
        //  공격 콜라이더 켤 때 이펙트 생성
        if(_isSkillAttack)
        {
            GameObject effect = Instantiate(ResourcePoolManager._instance._sword_1_skillEffect[_effectNum], _effectPos.position, Quaternion.LookRotation(transform.forward));
            Destroy(effect, 2.0f);
            if (_effectNum == ResourcePoolManager._instance._sword_1_skillEffect.Count - 1)
                _effectNum = 0;
            else
                _effectNum++;
        }
        else
        {
            GameObject effect = Instantiate(ResourcePoolManager._instance._sword_1_NormalEffect[_effectNum], _effectPos.position, Quaternion.LookRotation(transform.forward));
            Destroy(effect, 2.0f);
            if (_effectNum == ResourcePoolManager._instance._sword_1_NormalEffect.Count - 1)
                _effectNum = 0;
            else
                _effectNum++;
        }
    }
    void PlayFootStepAudio()
    {
        // 발소리 사운드 재생
        if (_footstepNum >= ResourcePoolManager._instance._playerStepSounds.Length)
            _footstepNum = 0;

        _audioSource.clip = ResourcePoolManager._instance._playerStepSounds[_footstepNum];
        _footstepNum++;
        _audioSource.Play();
    }
    #endregion [Animation Event Func]
}
