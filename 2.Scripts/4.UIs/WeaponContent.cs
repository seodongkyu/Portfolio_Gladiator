using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponContent : MonoBehaviour
{
    [SerializeField] Text _name;
    [HideInInspector] public int _weaponNumber = 0;
    SmithyUI _smithy;
    Image _icon;
    private void Awake()
    {
        _smithy = GameObject.FindGameObjectWithTag("SmithyUI").GetComponent<SmithyUI>();
        _icon = GetComponent<Image>();
    }
    void Start()
    {
        _name.text = DataManager._instance.WeaponTable["ÇÑ¼Õ°Ë"][_weaponNumber].Name;
    }

    public void ClickWeaponIcon()
    {
        _smithy.SelectWeapon(_icon.sprite, _weaponNumber);
    }
}
