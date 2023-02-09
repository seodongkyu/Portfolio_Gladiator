using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DefineUtility;
using System.IO;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] GameObject _mainMenuUI;
    [SerializeField] GameObject _startMenuUI;
    [SerializeField] GameObject _noDataMessage;
    [SerializeField] GameObject _checkStartUI;
    [SerializeField] GameObject _optionUI;
    [SerializeField] Slider _sensitivityBar;
    [SerializeField] Slider _bgmSlider;
    [SerializeField] Slider _sfxSlider;
    [SerializeField] AudioSource _bgmSource;
    [SerializeField] AudioSource _clickAudio;
    [SerializeField] AudioSource _fireAudio;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        //  설정값 슬라이더 value값을 현재 설정 값으로 설정
        _sensitivityBar.value = DataManager._instance._optionData._sensitivity;
        _bgmSlider.value = DataManager._instance._optionData._bgm_Volume;
        _sfxSlider.value = DataManager._instance._optionData._SFX_Volume;
        //  오디오 소스 볼륨값 세팅
        _bgmSource.volume = DataManager._instance._optionData._bgm_Volume;
        _clickAudio.volume = DataManager._instance._optionData._SFX_Volume;
        _fireAudio.volume = DataManager._instance._optionData._SFX_Volume;
    }

    //  게임시작 버튼 클릭 - 게임시작 메뉴 오픈
    public void Click_GameStartButton()
    {
        _mainMenuUI.gameObject.SetActive(false);
        _startMenuUI.gameObject.SetActive(true);
        _clickAudio.Play();
    }

    //  환경설정창을 여는 버튼 클릭
    public void Click_OptionButton()
    {
        _optionUI.gameObject.SetActive(true);
        _clickAudio.Play();
    }

    //  게임 종료 버튼 클릭
    public void Click_GameQuitButton()
    {        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE_WIN
        Application.Quit();
#endif
    }

    //  새로하기 버튼 클릭
    public void Click_NewGame()
    {
        if (!File.Exists(Application.persistentDataPath + "/gameData.json"))
        {
            //  초기 값
            Click_StartNewGame();
        }
        else
        {
            //  저장된 데이터가 모두 사라집니다 yes or No
            _checkStartUI.gameObject.SetActive(true);
        }
        _clickAudio.Play();
    }

    //  불러오기 버튼 클릭시 파일검사
    public void Click_LoadGame()
    {
        if (!File.Exists(Application.persistentDataPath + "/gameData.json"))
        {
            //  데이터 없음. 불러오기 실패
            StartCoroutine(ActiveNoDataMessage());
        }
        else
        {
            //  불러온 데이터로 세팅 후 시작
            DataManager._instance.LoadData(DataManager._instance._gameData);
            UserInfo._instance._openStage = DataManager._instance._gameData._openStage;
            UserInfo._instance._weaponIndex = DataManager._instance._gameData._weaponIndex;
            SceneController._instance.LoadScene(eSceneType.PrisonScene);
        }
        _clickAudio.Play();
    }

    //  게임시작 메뉴에서 뒤로가기 버튼 클릭
    public void Click_Back()
    {
        _mainMenuUI.gameObject.SetActive(true);
        _startMenuUI.gameObject.SetActive(false);
        _clickAudio.Play();
    }

    //  불러오기 실패시 데이터 없음 메세지 출력
    IEnumerator ActiveNoDataMessage()
    {
        _noDataMessage.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        _noDataMessage.gameObject.SetActive(false);
    }

    //  새로운 게임 시작버튼
    public void Click_StartNewGame()
    {
        UserInfo._instance._weaponIndex = 1;
        UserInfo._instance._openStage = 1;
        DataManager._instance.SetInitGameData();
        SceneController._instance.LoadScene(eSceneType.PrisonScene);
        _clickAudio.Play();
    }

    //  새로운 게임 시작 취소 버튼
    public void Click_No()
    {
        _checkStartUI.gameObject.SetActive(false);
        _clickAudio.Play();
    }

    //  환경설정 확인 버튼
    public void Click_SaveSetting()
    {
        _optionUI.gameObject.SetActive(false);
        DataManager._instance.SaveOptionData(_sensitivityBar.value, _bgmSlider.value, _sfxSlider.value);
        Debug.Log(DataManager._instance._optionData._bgm_Volume);
        _bgmSource.volume = DataManager._instance._optionData._bgm_Volume;
        _clickAudio.volume = DataManager._instance._optionData._SFX_Volume;
        _fireAudio.volume = DataManager._instance._optionData._SFX_Volume;
        _clickAudio.Play();
    }
}
