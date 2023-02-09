using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefineUtility
{
    public enum eUIType
    {
        Smithy,
        MonsterSelect,
        Interaction,
    }
    public enum eSceneType
    {
        LobbyScene,
        PrisonScene,
        IngameScene,
    }


    [System.Serializable]
    public struct stMonsterInfo
    {
        public int Index;
        public string Name;
        public float HP;
        public int Attack;
        public int SkillAttack;
        public float MoveSpeed;
        public float AttackRate;
        public float SkillRate;
        public float AttackRange;
    }


    [System.Serializable]
    public struct stMonInfo
    {
        public List<stMonsterInfo> _datas;
    }
    [System.Serializable]
    public struct stWeaponInfo
    {
        public string Kind;
        public string Name;
        public int Index;
        public int Attack;
        public int SkillAttack;
        public float AttackSpeed;
        public float CriticalRange;
        public float CriticalDamage;
    }
    [System.Serializable]
    public struct stWeaponTable
    {
        public List<stWeaponInfo> _datas;
    }
}
