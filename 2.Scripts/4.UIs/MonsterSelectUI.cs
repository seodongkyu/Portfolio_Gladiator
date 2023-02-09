using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DefineUtility;

public class MonsterSelectUI : MonoBehaviour
{
    [SerializeField] GameObject[] _monsterButtons;
    [SerializeField] Image _monsterImg;
    [SerializeField] Text _monsterName;
    [SerializeField] PrisonUIWindow _prisonUI;

    private void OnEnable()
    {
        //  UI창이 켜질 때 전투가능한 몬스터버튼만 켜준다.
        for(int n = 0; n < _monsterButtons.Length;n++)
        {
            if(n < UserInfo._instance._openStage)
                    _monsterButtons[n].gameObject.SetActive(true);
        }

        //  기본 몬스터로 초기화
        _monsterImg.sprite = ResourcePoolManager._instance._monsterImages[0];
        _monsterName.text = DataManager._instance.MonsterInfo[1].Name;
        DataManager._instance._selectMonster = 1;
    }
    private void OnDisable()
    {
        //  UI창이 꺼질 때 버튼도 전부 꺼준다.
        for (int n = 0; n < _monsterButtons.Length; n++)
            _monsterButtons[n].gameObject.SetActive(false);
    }

    public void GameStart()
    {
        //  선택한 몬스터 정보
        UserInfo._instance._isOpenedUI = false;
        SceneController._instance.LoadScene(eSceneType.IngameScene);
    }

    //  몬스터선택 버튼을 누르면 선택한 몬스터의 사진과 이름으로 교체해주고 선택한 몬스터의 인덱스를 받는다.
    public void ClickMonsterButton(int monIndex)
    {
        //  몬스터 이름으로 몬스터 사진을 가져온다.
        foreach (int key in DataManager._instance.MonsterInfo.Keys)
        {
            if (key == monIndex)
            {
                _monsterImg.sprite = ResourcePoolManager._instance._monsterImages[key - 1];
                break;
            }
        }

        //  선택한 몬스터의 이름을 넣어준다.
        _monsterName.text = DataManager._instance.MonsterInfo[monIndex].Name;
        DataManager._instance._selectMonster = monIndex;
        _prisonUI._click_Audio.Play();
    }

    public void ClickCloseButton()
    {
        UserInfo._instance._isOpenedUI = false;
        gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _prisonUI._click_Audio.Play();
    }
}
