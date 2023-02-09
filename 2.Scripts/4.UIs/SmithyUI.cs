using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmithyUI : MonoBehaviour
{
    [SerializeField] GameObject _weaponButton;
    [SerializeField] ScrollRect _weaponScroll;
    [SerializeField] GameObject _selectInfo;
    [SerializeField] Text _att_txt;
    [SerializeField] Text _skillatt_txt;
    [SerializeField] Text _attSpd_txt;
    [SerializeField] Text _criticalRange_txt;
    [SerializeField] Text _criticalDamage_txt;
    [SerializeField] GameObject _equip_btn;
    [SerializeField] GameObject _alreadyObj;
    [SerializeField] PrisonUIWindow _prisonUI;
    [SerializeField] GameObject _equipMsgUI;

    public Image _selectImg;
    int _selectIndex = 0;
    PlayerController _player;
    private void Start()
    {
        for (int n = 0; n < UserInfo._instance._openStage; n++)
        {
            GameObject go = Instantiate(_weaponButton, _weaponScroll.content.transform);
            Image icon = go.GetComponent<Image>();
            WeaponContent content = go.GetComponent<WeaponContent>();
            content._weaponNumber = n + 1;
            icon.sprite = ResourcePoolManager._instance._weaponIcons[n];
        }
        _selectIndex = UserInfo._instance._weaponIndex;
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }
    private void OnEnable()
    {
        _alreadyObj.SetActive(false);
    }
    private void Update()
    {
        if (UserInfo._instance._weaponIndex == _selectIndex)
        {
            _equip_btn.gameObject.SetActive(false);
        }
        else
            _equip_btn.gameObject.SetActive(true);
    }

    public void SelectWeapon(Sprite icon, int index)
    {
        _alreadyObj.SetActive(true);
        _selectImg.sprite = icon;
        _selectInfo.SetActive(true);
        //  무기 상세 정보++
        _selectIndex = index;
        _att_txt.text = DataManager._instance.WeaponTable["한손검"][index].Attack.ToString();
        _skillatt_txt.text = DataManager._instance.WeaponTable["한손검"][index].SkillAttack.ToString();
        _attSpd_txt.text = DataManager._instance.WeaponTable["한손검"][index].AttackSpeed.ToString();
        _criticalRange_txt.text = DataManager._instance.WeaponTable["한손검"][index].CriticalRange.ToString() + "  (%)";
        _criticalDamage_txt.text = DataManager._instance.WeaponTable["한손검"][index].CriticalDamage.ToString() + "  (%)";
        _prisonUI._click_Audio.Play();
    }

    public void ClickEquip()
    {
        Destroy(_player._currentWeapon);
        _player._currentWeapon = Instantiate(ResourcePoolManager._instance._weapons[_selectIndex - 1], _player._weaponPos);
        UserInfo._instance.SetWeaponInfo(DataManager._instance.WeaponTable["한손검"][_selectIndex].Index,
                                         DataManager._instance.WeaponTable["한손검"][_selectIndex].Attack,
                                         DataManager._instance.WeaponTable["한손검"][_selectIndex].SkillAttack,
                                         DataManager._instance.WeaponTable["한손검"][_selectIndex].AttackSpeed,
                                         DataManager._instance.WeaponTable["한손검"][_selectIndex].CriticalRange,
                                         DataManager._instance.WeaponTable["한손검"][_selectIndex].CriticalDamage);

        StartCoroutine(EquipSound());
    }
    IEnumerator EquipSound()
    {
        _prisonUI._click_Audio.clip = ResourcePoolManager._instance._equipWeaponSound;
        _prisonUI._click_Audio.Play();
        _equipMsgUI.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        _prisonUI._click_Audio.clip = ResourcePoolManager._instance._clickSound;
        _equipMsgUI.gameObject.SetActive(false);
    }
}
