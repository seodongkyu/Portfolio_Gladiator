using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineUtility;

public class IngameManager : MonoBehaviour
{
    #region [내부 참조]
    [SerializeField] IngameUIWindow _ingameUIWnd;
    [SerializeField] Transform _monSpawnPos;
    static IngameManager _uniqueInstance;

    bool _isPause = false;
    bool _isGameOver = false;
    bool _isGameClear = false;
    AudioSource _bgmAudio;
    #endregion [내부 참조]
    #region [외부 참조]
    public static IngameManager _instance { get { return _uniqueInstance; } }
    public MonsterObj _monsterObj { get; set; }
    public bool _isCameraMoving { get; set; }
    #endregion [외부 참조]
    private void Awake()
    {
        _uniqueInstance = this;
        _bgmAudio = GetComponent<AudioSource>();
        GameObject go = Instantiate(ResourcePoolManager._instance._monsterPrefabs[DataManager._instance._selectMonster - 1], _monSpawnPos.position, _monSpawnPos.rotation);
        _monsterObj = go.GetComponent<MonsterObj>();
    }
    void Start()
    {
        _bgmAudio.volume = DataManager._instance._optionData._bgm_Volume;
        _isCameraMoving = false;
    }

    private void Update()
    {
        if (_isGameOver || _isGameClear) return;

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(_isPause)
            {
                Click_Continue();
            }
            else
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                UserInfo._instance._isOpenedUI = true;
                Time.timeScale = 0;
                _ingameUIWnd.ActivePauseUI(true);
            }
        }
    }

    //  몬스터가 죽었을 때 승리UI코루틴 실행, 
    public void GameClear()
    {
        _isGameClear = true;
        StartCoroutine(_ingameUIWnd.GameClearUIs());
        if(UserInfo._instance._openStage == _monsterObj._monInfo.Index)
            UserInfo._instance._openStage = _monsterObj._monInfo.Index + 1;
    }

    //  플레이어가 죽었을 때 게임오버UI 오픈, 마우스 커서 나오기
    public IEnumerator GameOver()
    {
        _isGameOver = true;
        CameraController _cam = GameObject.FindGameObjectWithTag("CameraRoot").gameObject.GetComponent<CameraController>();
        if (_cam == null)
        {
            Debug.Log("cam is Null");
            Time.timeScale = 0;
        }
        else
            StartCoroutine(_cam.GameOverCameraMove());
        while(true)
        {
            yield return null;
            if (!_isCameraMoving)
            {
                StartCoroutine(_ingameUIWnd.GameOverUIs());
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                break;
            }
        }
    }

    //  다시하기 버튼 클릭
    public void Click_RetryGame()
    {
        SceneController._instance.LoadScene(eSceneType.IngameScene);
    }

    //  계속하기 버튼 클릭
    public void Click_Continue()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        UserInfo._instance._isOpenedUI = false;
        Time.timeScale = 1;
        _ingameUIWnd.ActivePauseUI(false);
    }

    //  전투 포기버튼 클릭
    public void Click_Exit()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        UserInfo._instance._isOpenedUI = false;
        Time.timeScale = 1;
        SceneController._instance.LoadScene(eSceneType.PrisonScene);
    }
}
