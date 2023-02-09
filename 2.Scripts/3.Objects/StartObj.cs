using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineUtility;

public class StartObj : MonoBehaviour
{
    void Start()
    {
        DataManager._instance.InitSetDatas();
        SceneController._instance.LoadScene(eSceneType.LobbyScene);
    }
}
