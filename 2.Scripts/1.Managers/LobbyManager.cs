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

        //  ������ �����̴� value���� ���� ���� ������ ����
        _sensitivityBar.value = DataManager._instance._optionData._sensitivity;
        _bgmSlider.value = DataManager._instance._optionData._bgm_Volume;
        _sfxSlider.value = DataManager._instance._optionData._SFX_Volume;
        //  ����� �ҽ� ������ ����
        _bgmSource.volume = DataManager._instance._optionData._bgm_Volume;
        _clickAudio.volume = DataManager._instance._optionData._SFX_Volume;
        _fireAudio.volume = DataManager._instance._optionData._SFX_Volume;
    }

    //  ���ӽ��� ��ư Ŭ�� - ���ӽ��� �޴� ����
    public void Click_GameStartButton()
    {
        _mainMenuUI.gameObject.SetActive(false);
        _startMenuUI.gameObject.SetActive(true);
        _clickAudio.Play();
    }

    //  ȯ�漳��â�� ���� ��ư Ŭ��
    public void Click_OptionButton()
    {
        _optionUI.gameObject.SetActive(true);
        _clickAudio.Play();
    }

    //  ���� ���� ��ư Ŭ��
    public void Click_GameQuitButton()
    {        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE_WIN
        Application.Quit();
#endif
    }

    //  �����ϱ� ��ư Ŭ��
    public void Click_NewGame()
    {
        if (!File.Exists(Application.persistentDataPath + "/gameData.json"))
        {
            //  �ʱ� ��
            Click_StartNewGame();
        }
        else
        {
            //  ����� �����Ͱ� ��� ������ϴ� yes or No
            _checkStartUI.gameObject.SetActive(true);
        }
        _clickAudio.Play();
    }

    //  �ҷ����� ��ư Ŭ���� ���ϰ˻�
    public void Click_LoadGame()
    {
        if (!File.Exists(Application.persistentDataPath + "/gameData.json"))
        {
            //  ������ ����. �ҷ����� ����
            StartCoroutine(ActiveNoDataMessage());
        }
        else
        {
            //  �ҷ��� �����ͷ� ���� �� ����
            DataManager._instance.LoadData(DataManager._instance._gameData);
            UserInfo._instance._openStage = DataManager._instance._gameData._openStage;
            UserInfo._instance._weaponIndex = DataManager._instance._gameData._weaponIndex;
            SceneController._instance.LoadScene(eSceneType.PrisonScene);
        }
        _clickAudio.Play();
    }

    //  ���ӽ��� �޴����� �ڷΰ��� ��ư Ŭ��
    public void Click_Back()
    {
        _mainMenuUI.gameObject.SetActive(true);
        _startMenuUI.gameObject.SetActive(false);
        _clickAudio.Play();
    }

    //  �ҷ����� ���н� ������ ���� �޼��� ���
    IEnumerator ActiveNoDataMessage()
    {
        _noDataMessage.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        _noDataMessage.gameObject.SetActive(false);
    }

    //  ���ο� ���� ���۹�ư
    public void Click_StartNewGame()
    {
        UserInfo._instance._weaponIndex = 1;
        UserInfo._instance._openStage = 1;
        DataManager._instance.SetInitGameData();
        SceneController._instance.LoadScene(eSceneType.PrisonScene);
        _clickAudio.Play();
    }

    //  ���ο� ���� ���� ��� ��ư
    public void Click_No()
    {
        _checkStartUI.gameObject.SetActive(false);
        _clickAudio.Play();
    }

    //  ȯ�漳�� Ȯ�� ��ư
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
