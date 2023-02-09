using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DefineUtility;

public class IngameUIWindow : MonoBehaviour
{
    [SerializeField] GameObject _pauseMenuUI;
    [SerializeField] GameObject _skipMessage;
    [SerializeField] GameObject _newWeapon_txt;
    [SerializeField] GameObject _newMonster_txt;
    [SerializeField] GameObject _optionUI;
    [SerializeField] GameObject _criticalUI;
    [SerializeField] Image _dashBlack;
    [SerializeField] Image _skillBlack;
    [SerializeField] Image _gameOverBlackGround;
    [SerializeField] Image _monsterIconImg;
    [SerializeField] Text _remainTimeTxt;

    [SerializeField] Slider _monsterHPBar;
    [SerializeField] Slider _playerHPBar;
    [SerializeField] Slider _sensitivityBar;
    [SerializeField] Slider _bgmSlider;
    [SerializeField] Slider _sfxSlider;
    public GameObject _gameOverUI;
    public GameObject _gameClearUI;
    AudioSource _clickAudioSource;

    MonsterObj _monster;
    PlayerController _player;

    bool _isDash = false;
    bool _isSkillAttack = false;
    private void Start()
    {
        _monster = GameObject.FindGameObjectWithTag("Monster").GetComponent<MonsterObj>();
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        _clickAudioSource = GetComponent<AudioSource>();
        _clickAudioSource.volume = DataManager._instance._optionData._SFX_Volume;
        _sensitivityBar.value = DataManager._instance._optionData._sensitivity;
        _bgmSlider.value = DataManager._instance._optionData._bgm_Volume;
        _sfxSlider.value = DataManager._instance._optionData._SFX_Volume;
        _monsterIconImg.sprite = ResourcePoolManager._instance._monsterImages[IngameManager._instance._monsterObj._monInfo.Index - 1];
    }

    private void Update()
    {
        if(_player._isDash && !_isDash)
        {
            StartCoroutine(DashTimerUI());
        }
        if(_player._isSkillAttack && !_isSkillAttack)
        {
            StartCoroutine(SkillTimerUI());
        }
    }
    //  HP 변동 생길 때마다 호출되는 함수
    public void SetMonsterHPBar()
    {  
        float maxHP = _monster._monInfo.HP;

        float remainPerHP = (_monster._currentHP / maxHP * 100);
        _monsterHPBar.value = remainPerHP;
    }
    public void SetPlayerHPBar()
    {
        float maxHP = UserInfo._instance._maxHP;

        float remainPerHP = (UserInfo._instance._currentHP / maxHP * 100);
        _playerHPBar.value = remainPerHP;
    }
    public void ActivePauseUI(bool pause)
    {
        _pauseMenuUI.SetActive(pause);
    }
    public void Click_OptionButton()
    {
        _optionUI.gameObject.SetActive(true);
        _clickAudioSource.Play();
    }
    public void Click_OkayButton()
    {
        DataManager._instance.SaveGameData(UserInfo._instance._openStage, UserInfo._instance._weaponIndex);
        
        AudioSource ingameAudio = GameObject.FindGameObjectWithTag("IngameManager").GetComponent<AudioSource>();
        ingameAudio.volume = _bgmSlider.value;
        _player._audioSource.volume = _sfxSlider.value;
        _monster.baseAudioVolume = _sfxSlider.value;
        _clickAudioSource.volume = _sfxSlider.value;
        _clickAudioSource.Play();
        _optionUI.gameObject.SetActive(false);
    }
    public IEnumerator GameClearUIs()
    {
        _gameClearUI.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        _monsterHPBar.gameObject.SetActive(false);
        _playerHPBar.gameObject.SetActive(false);
        _newWeapon_txt.SetActive(true);
        _newMonster_txt.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        _newWeapon_txt.SetActive(false);
        _newMonster_txt.SetActive(false);
        _skipMessage.SetActive(true);
        _remainTimeTxt.gameObject.SetActive(true);
        StartCoroutine(RemainTimeUpdate());
    }
    public IEnumerator GameOverUIs()
    {
        yield return new WaitForSeconds(1.5f);
        _gameOverBlackGround.gameObject.SetActive(true);
        //  점점 어두워지는 화면
        Color black = Color.black;
        black.a = 0;
        while(black.a < 0.7f)
        {
            black.a += Time.deltaTime / 2;
            Debug.Log(black.a);
            _gameOverBlackGround.color = black;
            yield return null;
        }
        _gameOverUI.SetActive(true);

    }
    public IEnumerator RemainTimeUpdate()
    {
        float timer = 10;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            int remain = (int)timer;
            string message = string.Format("{0}초 후 감옥으로 이동합니다.", remain);
            _remainTimeTxt.text =  message;
            if(Input.GetKeyDown(KeyCode.B))
                break;
            yield return null;
        }
        SceneController._instance.LoadScene(eSceneType.PrisonScene);
    }
    public IEnumerator DashTimerUI()
    {
        _dashBlack.fillAmount = 1;
        _isDash = true;
        while (_dashBlack.fillAmount > 0)
        {
            _dashBlack.fillAmount -= Time.deltaTime / _player._dashRate;
            yield return null;
        }
        _isDash = false;
    }
    public IEnumerator SkillTimerUI()
    {
        _skillBlack.fillAmount = 1;
        _isSkillAttack = true;
        while (_skillBlack.fillAmount > 0)
        {
            _skillBlack.fillAmount -= Time.deltaTime / _player._skillRate;
            yield return null;
        }
        _isSkillAttack = false;
    }
    public IEnumerator ActiveTrueCriticalUI()
    {
        _criticalUI.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        _criticalUI.gameObject.SetActive(false);
    }
}
