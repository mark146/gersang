using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat : MonoBehaviour
{
    [SerializeField]
    int _level;
    [SerializeField]
    string _job;
    [SerializeField]
    int _hp;
    [SerializeField]
    int _maxHp;
    [SerializeField]
    int _mp;
    [SerializeField]
    int _maxMp;
    [SerializeField]
    int _attack;
    [SerializeField]
    int _defense;
    [SerializeField]
    float _moveSpeed;

    public int Level { get { return _level; } set { _level = value; } }
    public string Job { get { return _job; } set { _job = value; } }
    public int Hp { get { return _hp; } set { _hp = value; } }
    public int Mp { get { return _mp; } set { _mp = value; } }
    public int MaxHp { get { return _maxHp; } set { _maxHp = value; } }
    public int MaxMp { get { return _maxMp; } set { _maxMp = value; } }
    public int Attack { get { return _attack; } set { _attack = value; } }
    public int Defense { get { return _defense; } set { _defense = value; } }
    public float MoveSpeed { get { return _moveSpeed; } set { _moveSpeed = value; } }

    // 임시 데이터
    void Start()
    {
        _level = 1;
        _hp = 100;
        _maxHp = 100;
        _mp = 100;
        _maxMp = 100;
        _attack = 10;
        _defense = 5;
        _moveSpeed = 5.0f;
    }

   public void OnAttacked(Stat attacker)
    {
        int damage = Mathf.Max(0, attacker.Attack - Defense);
        Hp -= damage;

        if (Hp <= 0)
        {
            Hp = 0;
            OnDead(attacker);
        }
    }

    void OnDead(Stat attacker)
    {

        // 플레이어가 잡을 경우 경험치 증가
        //PlayerStat playerStat = attacker as PlayerStat;
        //if (playerStat != null)
        //{
        //    playerStat.Exp += 15;
        //}

        //Managers.Game.Despwan(gameObject);
    }
}
