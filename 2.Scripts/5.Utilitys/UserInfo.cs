using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInfo : TSingleton<UserInfo>
{
    public bool _isOpenedUI { get; set; }

    public int _openStage = 1;

    public float _maxHP = 2000;
    public float _currentHP;
    public int _weaponIndex = 1;
    public int _att;
    public int _skillAtt;
    public float _attSpeed;
    public float _criticalRange;
    public float _criticalDamage;

    //  무기교체 또는 초기 무기 설정
    public void SetWeaponInfo(int index,int att,int skillatt,float attSpeed,float criticalRange, float criticalDamage)
    {
        _weaponIndex = index;
        _att = att;
        _skillAtt = skillatt;
        _attSpeed = attSpeed;
        _criticalRange = criticalRange;
        _criticalDamage = criticalDamage;
    }
}
