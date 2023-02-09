using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineUtility;
using System.IO;

/*
 * ó�����ӽ��� - ������ ����, �ɼǵ����� �������� - �ɼ� ����
 * �κ� ȯ�ἳ�� ���� - �ɼǵ�����
 */
[System.Serializable]
public class GameData
{
    public int _openStage = 1;
    public int _weaponIndex = 1;
}
[System.Serializable]
public class OptionData
{
    public float _sensitivity = 5f;  //  1~5
    public float _bgm_Volume = 1;
    public float _SFX_Volume = 1;
}

public class DataManager : TSingleton<DataManager>
{
    public GameData _gameData;
    public OptionData _optionData;

    Dictionary<int, stMonsterInfo> _monsterInfos = new Dictionary<int, stMonsterInfo>();
    public Dictionary<int, stMonsterInfo> MonsterInfo { get { return _monsterInfos; }}

    Dictionary<string, Dictionary<int, stWeaponInfo>> _weaponTable = new Dictionary<string, Dictionary<int, stWeaponInfo>>();
    public Dictionary<string, Dictionary<int, stWeaponInfo>> WeaponTable { get { return _weaponTable; } }

    public int _selectMonster { get; set; }

    
    public void InitSetDatas()
    {
        base.Init();
        _gameData = new GameData();
        _optionData = new OptionData();
        //  ��� ������ ���̺��� �о�´�.
        ReadAllJson();
        //  ������ �� �⺻���� ù��° �����̸����� ���ص״�.
        _selectMonster = 1;

        if (File.Exists(Application.persistentDataPath + "/OptionData.json"))
        {
            LoadData(_optionData);
        }

    }

    public void ReadAllJson()
    {
        ReadMonsterInfoFromJson();
        ReadWeaponTableFromJson();
    }

    void ReadMonsterInfoFromJson()
    {
        TextAsset read = Resources.Load<TextAsset>("Json/MonsterInfo");
        if (read != null)
        {
            stMonInfo monInfo = JsonUtility.FromJson<stMonInfo>(read.text);
            for(int n = 0; n < monInfo._datas.Count;n++)
                _monsterInfos.Add(monInfo._datas[n].Index, monInfo._datas[n]);
        }
        else
            Debug.Log("Monster data is Null");
        Debug.Log("");
    }

    void ReadWeaponTableFromJson()
    {
        TextAsset read = Resources.Load<TextAsset>("Json/WeaponTable");
        if (read != null)
        {
            stWeaponTable weaponTable = JsonUtility.FromJson<stWeaponTable>(read.text);
            Dictionary<int, stWeaponInfo> sword = new Dictionary<int, stWeaponInfo>();
            Dictionary<int, stWeaponInfo> longSword = new Dictionary<int, stWeaponInfo>();
            Dictionary<int, stWeaponInfo> axe = new Dictionary<int, stWeaponInfo>();
            Dictionary<int, stWeaponInfo> mace = new Dictionary<int, stWeaponInfo>();
            for (int n = 0; n < weaponTable._datas.Count; n++)
            {
                switch(weaponTable._datas[n].Kind)
                {
                    case "�Ѽհ�":
                        sword.Add(weaponTable._datas[n].Index, weaponTable._datas[n]);
                        break;
                    case "���":
                        longSword.Add(weaponTable._datas[n].Index, weaponTable._datas[n]);
                        break;
                    case "����":
                        axe.Add(weaponTable._datas[n].Index, weaponTable._datas[n]);
                        break;
                    case "���̽�":
                        mace.Add(weaponTable._datas[n].Index, weaponTable._datas[n]);
                        break;
                }
            }
            _weaponTable.Add("�Ѽհ�", sword);
            _weaponTable.Add("���", longSword);
            _weaponTable.Add("����", axe);
            _weaponTable.Add("���̽�", mace);
        }
        else
            Debug.Log("Weapon data is Null");
    }

    public void SaveGameData(int stage, int weaponIDX)
    {
        //  ���� ��
        _gameData._openStage = stage;
        _gameData._weaponIndex = weaponIDX;
        SaveData(_gameData);
    }
    //  ȯ�漳�� ����
    public void SaveOptionData(float sen, float bgm, float sfx)
    {
        _optionData._sensitivity = sen;
        _optionData._bgm_Volume = bgm;
        _optionData._SFX_Volume = sfx;
        SaveData(_optionData);
    }

    public void SetInitGameData()
    {
        Debug.Log("Set Data");
        _gameData._openStage = 1;
        _gameData._weaponIndex = 1;
        if (File.Exists(Application.persistentDataPath + "/GameData.json"))
            File.Delete(Application.persistentDataPath + "/GameData.json");
    }

    private void SaveData(object data)
    {
        Debug.Log("Save Data");
        string jsonData = JsonUtility.ToJson(data, true);
        string path = Path.Combine(Application.persistentDataPath + "/" + data.ToString() + ".json");
        Debug.Log(path);
        File.WriteAllText(path, jsonData);
    }
    public void LoadData(object data)
    {
        Debug.Log("Load Data");
        string path = Path.Combine(Application.persistentDataPath + "/" + data.ToString() + ".json");
        Debug.Log(path);
        string jsonData = File.ReadAllText(path);
        switch(data.ToString())
        {
            case "OptionData":
                _optionData = JsonUtility.FromJson<OptionData>(jsonData);
                break;
            case "GameData":
                _gameData = JsonUtility.FromJson<GameData>(jsonData);
                break;
        }
    }
}
