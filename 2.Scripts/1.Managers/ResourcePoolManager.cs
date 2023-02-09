using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DefineUtility;

public class ResourcePoolManager : MonoBehaviour
{
    static ResourcePoolManager _uniqueInstance;
    public static ResourcePoolManager _instance
    {
        get { return _uniqueInstance; }
    }

    public GameObject _loadingWindow;
    public GameObject[] _monsterPrefabs;
    public GameObject _monsterProjectorObj;
    public GameObject[] _weapons;

    #region[Sprites]
    public Sprite[] _weaponIcons;
    public Sprite[] _monsterImages;
    #endregion[Sprites]

    #region[AudioClips]
    public AudioClip _clickSound;
    public AudioClip _equipWeaponSound;


    public AudioClip _dashSound;
    public AudioClip _AttackSound;
    public AudioClip[] _playerStepSounds;

    public AudioClip _biteSound;
    public AudioClip _clawSound;
    public AudioClip _hit_Normal;
    public AudioClip _hit_Critical;
    public AudioClip[] _monsterStepSounds;
    public AudioClip _nightMare_ButtBump;
    #endregion[AudioClips]

    #region[Effects]
    public List<GameObject> _sword_1_NormalEffect;
    public List<GameObject> _sword_1_skillEffect;
    public GameObject _dashEffect;
    public GameObject _explodeEffect;
    public GameObject _fireBallPrefab;
    public GameObject _flamePrefab;
    public GameObject _hitEffect;
    #endregion[Effects]


    private void Awake()
    {
        _uniqueInstance = this;
        DontDestroyOnLoad(gameObject);
    }
}
