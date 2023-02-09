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
        //  UIâ�� ���� �� ���������� ���͹�ư�� ���ش�.
        for(int n = 0; n < _monsterButtons.Length;n++)
        {
            if(n < UserInfo._instance._openStage)
                    _monsterButtons[n].gameObject.SetActive(true);
        }

        //  �⺻ ���ͷ� �ʱ�ȭ
        _monsterImg.sprite = ResourcePoolManager._instance._monsterImages[0];
        _monsterName.text = DataManager._instance.MonsterInfo[1].Name;
        DataManager._instance._selectMonster = 1;
    }
    private void OnDisable()
    {
        //  UIâ�� ���� �� ��ư�� ���� ���ش�.
        for (int n = 0; n < _monsterButtons.Length; n++)
            _monsterButtons[n].gameObject.SetActive(false);
    }

    public void GameStart()
    {
        //  ������ ���� ����
        UserInfo._instance._isOpenedUI = false;
        SceneController._instance.LoadScene(eSceneType.IngameScene);
    }

    //  ���ͼ��� ��ư�� ������ ������ ������ ������ �̸����� ��ü���ְ� ������ ������ �ε����� �޴´�.
    public void ClickMonsterButton(int monIndex)
    {
        //  ���� �̸����� ���� ������ �����´�.
        foreach (int key in DataManager._instance.MonsterInfo.Keys)
        {
            if (key == monIndex)
            {
                _monsterImg.sprite = ResourcePoolManager._instance._monsterImages[key - 1];
                break;
            }
        }

        //  ������ ������ �̸��� �־��ش�.
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
