using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DefineUtility;


public class PrisonUIWindow : MonoBehaviour
{
    [SerializeField] GameObject _smithyUI;
    [SerializeField] GameObject _monsterSelectUI;
    [SerializeField] GameObject _interactionUI;
    [SerializeField] Text _interactionText;
    [SerializeField] GameObject _pauseMenuUI;
    [SerializeField] Image _dashBlack;
    [SerializeField] Image _skillBlack;
    [SerializeField] GameObject _optionUI;
    [SerializeField] Slider _sensitivityBar;
    [SerializeField] Slider _bgmSlider;
    [SerializeField] Slider _sfxSlider;
    [SerializeField] AudioSource _bgm_Audio;
    [SerializeField] AudioSource[] _sfx_Audio;
    [SerializeField] GameObject _saveTextUI;
    public AudioSource _click_Audio;

    bool _isPause = false;
    bool _isDash = false;
    bool _isSkillAttack = false;

    PlayerController _player;
    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        _sensitivityBar.value = DataManager._instance._optionData._sensitivity;
        _bgmSlider.value = DataManager._instance._optionData._bgm_Volume;

        _bgm_Audio.volume = DataManager._instance._optionData._bgm_Volume;
        for (int n = 0; n < _sfx_Audio.Length; n++)
            _sfx_Audio[n].volume = _sfxSlider.value = DataManager._instance._optionData._SFX_Volume;

        _click_Audio.volume = DataManager._instance._optionData._SFX_Volume;
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(_isPause)
            {
                Click_CloseMenu();
            }
            else
            {
                _pauseMenuUI.SetActive(true);
                UserInfo._instance._isOpenedUI = true;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
                _click_Audio.Play();
            }
        }
        if(_player._isDash && !_isDash)
        {
            StartCoroutine(DashTimerUI());
        }
        if(_player._isSkillAttack && !_isSkillAttack)
        {
            StartCoroutine(SkillTimerUI());
        }
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

    public void SetActiveUI(eUIType type,bool isActive)
    {
        //  F 전투선택 UI 나오게 하기
        switch(type)
        {
            case eUIType.Smithy:
                _smithyUI.gameObject.SetActive(isActive);
                UserInfo._instance._isOpenedUI = isActive;
                break;
            case eUIType.MonsterSelect:
                _monsterSelectUI.gameObject.SetActive(isActive);
                UserInfo._instance._isOpenedUI = isActive;
                break;
            case eUIType.Interaction:
                _interactionUI.gameObject.SetActive(isActive);
                break;
        }
    }
    public void SetInteractionMessage(string msg)
    {
        _interactionText.text = msg;
    }

    public void Click_GameQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE_WIN
        Application.Quit();
#endif
    }
    public void Click_OpenOption()
    {
        _optionUI.gameObject.SetActive(true);
        _click_Audio.Play();
    }
    public void Click_CloseMenu()
    {
        _pauseMenuUI.SetActive(false);
        UserInfo._instance._isOpenedUI = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _click_Audio.Play();
    }
    public void ClickCloseButton()
    {
        UserInfo._instance._isOpenedUI = false;
        _smithyUI.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _click_Audio.Play();
    }
    public void Click_SaveGameButton()
    {
        DataManager._instance.SaveGameData(UserInfo._instance._openStage, UserInfo._instance._weaponIndex);
        _click_Audio.Play();
        StartCoroutine(OpenSaveTextUI());
        //  저장됨 UI
    }

    //  환경설정 확인 버튼 클릭
    public void Click_SaveSetting()
    {
        _optionUI.gameObject.SetActive(false);
        DataManager._instance.SaveOptionData(_sensitivityBar.value,_bgmSlider.value,_sfxSlider.value);
        _bgm_Audio.volume = _bgmSlider.value;
        for (int n = 0; n < _sfx_Audio.Length; n++)
            _sfx_Audio[n].volume = _sfxSlider.value;
        _click_Audio.volume = _sfxSlider.value;
        _click_Audio.Play();
    }
    IEnumerator OpenSaveTextUI()
    {
        _saveTextUI.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        _saveTextUI.gameObject.SetActive(false);
    }
}
